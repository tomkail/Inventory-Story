using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ElevenLabs;
using ElevenLabs.Voices;
using UnityEngine;

public class VOController : MonoSingleton<VOController> {
    // public Dictionary<string, string> characterVoiceMap = new Dictionary<string, string>() {
    //     "Marty" => "uLyOUIxMCdJzvQsRzJ5I",
    // }
    public Voice voice;
    StreamedAudioPlayer streamedAudioPlayer;
    public async void StreamAndCache(string message) {
        var api = new ElevenLabsClient();

        // api.VoicesEndpoint.EnableDebug = true;
        // var voices = await api.VoicesEndpoint.GetAllVoicesAsync();
        // var randomVoice = voices.Where(x => x.Category is "generated" or "professional").Random().Id;
        // Debug.Log(randomVoice);
        // var voice = (await api.VoicesEndpoint.GetVoiceAsync("uLyOUIxMCdJzvQsRzJ5I", true));
        streamedAudioPlayer = new StreamedAudioPlayer();
        // api.TextToSpeechEndpoint.EnableDebug = true;

        if (FindCachedClip(voice, message)) {
            
        } else {
            var voiceClip = await api.TextToSpeechEndpoint.StreamTextToSpeechAsync(message, voice, streamedAudioPlayer.QueuePartialClip);
            CacheClip(voiceClip);
        }
    }

    static bool FindCachedClip(Voice voice, string message) {
        var narrationDir = Path.Combine(Application.persistentDataPath, "Narration");
        if (File.Exists(Path.Combine(narrationDir, GenerateFileName(voice, message)))) {
            return true;
        }
        return false;
    }

    static void CacheClip(VoiceClip voiceClip) {
        if (voiceClip == null) return;
        var narrationDir = Path.Combine(Application.persistentDataPath, "Narration");
        if(!Directory.Exists(narrationDir)) Directory.CreateDirectory(narrationDir);
        File.Copy(voiceClip.CachedPath, Path.Combine(narrationDir, GenerateFileName(voiceClip.Voice.Id, voiceClip.Text)), true);
    }

    public static Guid GenerateGuid(string @string) {
        using var md5 = MD5.Create();
        return new Guid(md5.ComputeHash(Encoding.Default.GetBytes(@string)));
    }
    public static string GenerateFileName(string voiceId, string message) {
        var stringToHash = voiceId+":"+message;
        using var md5 = MD5.Create();
        var fileName = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(stringToHash))).ToString();
        return message.Substring(0, Mathf.Min(message.Length, 20))+"_" +fileName;
    }

    void Update() {
        streamedAudioPlayer?.Update();
    }
}