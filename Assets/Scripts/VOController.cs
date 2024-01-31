using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ElevenLabs;
using ElevenLabs.Voices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using Utilities.Async;

public class VOController : MonoSingleton<VOController> {
    public AudioMixerGroup mixerGroup;
    public Voice voice;
    StreamedAudioPlayer streamedAudioPlayer;
    AudioSource downloadedAudioSource;

    [Space] public bool useCache = true;
    //
    // [Space]
    // [Range(0,1)]public float stability = 0.75f;
    // [Range(0,1)]public float similarityBoost = 0.45f;
    // public bool speakerBoost = true;
    // [Range(0,1)]public float style = 0.75f;
    
    public async void StreamAndCache(string message) {
        var api = new ElevenLabsClient();

        // api.VoicesEndpoint.EnableDebug = true;
        // var voices = await api.VoicesEndpoint.GetAllVoicesAsync();
        // var randomVoice = voices.Where(x => x.Category is "generated" or "professional").Random().Id;
        // Debug.Log(randomVoice);
        // var voice = (await api.VoicesEndpoint.GetVoiceAsync("uLyOUIxMCdJzvQsRzJ5I", true));
        streamedAudioPlayer = new StreamedAudioPlayer();
        streamedAudioPlayer.CreateAudioSource = CreateNewAudioSource;
        // api.TextToSpeechEndpoint.EnableDebug = true;

        if (useCache && FindCachedClip(voice, message, out string cachedClipLoadPath)) {
            LoadCachedClip(cachedClipLoadPath);
        } else {
            // , new VoiceSettings(stability, similarityBoost, speakerBoost, style)
            var voiceClip = await api.TextToSpeechEndpoint.StreamTextToSpeechAsync(message, voice, streamedAudioPlayer.QueuePartialClip);
            CacheClip(voiceClip);
        }
    }

    AudioSource CreateNewAudioSource() {
        var source = new GameObject("Audio Source").AddComponent<AudioSource>();
        source.transform.SetParent(transform);
        source.playOnAwake = false;
        source.spatialBlend = 0;
        source.outputAudioMixerGroup = mixerGroup;
        return source;
    }

    async void LoadCachedClip(string cachedClipLoadPath) {
        var loadableURL = AddLocalFilePrefixToLocalFiles(cachedClipLoadPath);
        using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(loadableURL, AudioType.OGGVORBIS)) {
            try {
                await webRequest.SendWebRequest();
            } catch (OperationCanceledException e) {
                Debug.Log("Cancelled task: " + e);
            } catch (Exception e) {
                Debug.LogError("SendWebRequest failed: " + e);
            }
            if(webRequest.result == UnityWebRequest.Result.Success) {
                var audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
                if (downloadedAudioSource == null) downloadedAudioSource = CreateNewAudioSource();
                downloadedAudioSource.clip = audioClip;
                downloadedAudioSource.Play();
            } else {
                Debug.LogWarning($"Failed to load audio at url '{loadableURL}'\n{webRequest.error}\n{webRequest.downloadHandler.error}");
            }
        }
    
        static string AddLocalFilePrefixToLocalFiles (string filePath) {
            if(filePath.StartsWith("http")) return filePath;
            else if(!filePath.StartsWith("file://")) return "file://"+filePath;
            else return filePath;
        }
    }

    static bool FindCachedClip(Voice voice, string message, out string cachedClipLoadPath) {
        var narrationDir = Path.Combine(Application.persistentDataPath, "Narration");
        cachedClipLoadPath = Path.Combine(narrationDir, GenerateFileName(voice, message));
        return File.Exists(cachedClipLoadPath);
    }

    static void CacheClip(VoiceClip voiceClip) {
        if (voiceClip == null) return;
        var narrationDir = Path.Combine(Application.persistentDataPath, "Narration");
        if(!Directory.Exists(narrationDir)) Directory.CreateDirectory(narrationDir);
        File.Copy(voiceClip.CachedPath, Path.Combine(narrationDir, GenerateFileName(voiceClip.Voice.Id, voiceClip.Text)), true);
    }
    
    public static string GenerateFileName(string voiceId, string message) {
        var stringToHash = voiceId+":"+message;
        using var md5 = MD5.Create();
        var fileName = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(stringToHash))).ToString();
        return message.Substring(0, Mathf.Min(message.Length, 20))+"_" +fileName+".ogg";
    }

    void Update() {
        streamedAudioPlayer?.Update();
    }
}