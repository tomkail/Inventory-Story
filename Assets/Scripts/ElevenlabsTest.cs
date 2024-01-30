using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ElevenLabs;
using EasyButtons;
using UnityEngine;

public class ElevenlabsTest : MonoBehaviour {
    public string text = "The quick brown fox jumps over the lazy dog.";
    string key = "0fb25e2ab954c7e03049c2010734fcfb";
    public AudioSource audioSource;
    void Start() {
        var api = new ElevenLabsClient(key);
    }

    void OnGUI() {
        var guiScale = Mathf.Max(Screen.width/280f, Screen.height/3000f);
        var oldM = GUI.matrix;
        GUI.matrix = Matrix4x4.Scale(guiScale*Vector3.one);
        GUILayout.Space(100);
        if(GUILayout.Button("Text To Speech")) {
            TextToSpeech();
        }
    }
    
    [Button]
    async void TextToSpeech() {
        var api = new ElevenLabsClient();
        var allVoices = await api.VoicesEndpoint.GetAllVoicesAsync();
        var voice = allVoices.FirstOrDefault();
        // var defaultVoiceSettings = await api.VoicesEndpoint.GetDefaultVoiceSettingsAsync();
        
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        var voiceClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(text, voice);
        stopWatch.Stop();
        UnityEngine.Debug.Log($"Elevenlabs read {text} in {stopWatch.Elapsed.TotalSeconds} seconds");
        
        audioSource.PlayOneShot(voiceClip.AudioClip);
    }
    
    [Button]
    async void StreamTextToSpeech() {
        var api = new ElevenLabsClient();
        var voice = (await api.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
        var partialClips = new Queue<AudioClip>();
        var voiceClip = await api.TextToSpeechEndpoint.StreamTextToSpeechAsync(
            text,
            voice,
            partialClip =>
            {
                // Note: Best to queue them and play them in update loop!
                // See TextToSpeech sample demo for details
                partialClips.Enqueue(partialClip);
            });
// The full completed clip:
        audioSource.clip = voiceClip.AudioClip;
    }
}
