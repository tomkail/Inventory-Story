using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StreamedAudioPlayer {
    public float lookaheadTime = 1;
            
    public bool playing;
    public bool paused;

    // public class StreamedAudioClipItem {
    //     public AudioClip clip;
    //     public AudioSource source;
    //     public double startPlayTime;
    //     public double endPlayTime;
    //
    //     public StreamedAudioClipItem(AudioClip partialClip) {
    //         this.clip = partialClip;
    //     }
    // }
    // private double dspTimeOnPause = -1;

    // private readonly List<StreamedAudioClipItem> streamQueue = new List<StreamedAudioClipItem>();
    // StreamedAudioClipItem currentStreamItem;
            
    private readonly Queue<AudioClip> streamClipQueue = new Queue<AudioClip>();
    private List<AudioSource> audioSources = new List<AudioSource>();
    private double nextPlayTime = -1;

            
    // public void Start() {
    //     playing = true;
    //     nextPlayTime = AudioSettings.dspTime;
    //     if (currentStreamItem != null) {
    //         var timeOfClipAtPause = currentStreamItem.startPlayTime - dspTimeOnPause;
    //         currentStreamItem.source.PlayScheduled(nextPlayTime-timeOfClipAtPause);
    //         
    //     }
    // }
    //
    // public void Stop() {
    //     playing = false;
    // }
    //
    // public void Pause() {
    //     paused = true;
    //     dspTimeOnPause = AudioSettings.dspTime;
    // }
            
    public void QueuePartialClip(AudioClip partialClip) {
        streamClipQueue.Enqueue(partialClip);
        // streamQueue.Add(new StreamedAudioClipItem(partialClip));
        if(nextPlayTime < 0) {
            nextPlayTime = AudioSettings.dspTime;
        }
        // if (playing && streamClipQueue.Count == 1) {
        //     nextPlayTime = AudioSettings.dspTime;
        //     Update();
        // }
    }

    public void Update() {
        if (streamClipQueue.Count > 0 && AudioSettings.dspTime >= nextPlayTime-lookaheadTime) {
            var clip = streamClipQueue.Dequeue();
            var source = GetAudioSource();
            source.clip = clip;
            source.PlayScheduled(nextPlayTime);
            double clipLength = (double)clip.samples / clip.frequency;
            nextPlayTime += clipLength; // Update nextPlayTime for the next clip
            // Debug.Log($"Scheduled play of {clip.name} with length {clipLength} at {nextPlayTime}");
        }
    }

    private AudioSource GetAudioSource() {
        // Find an available audio source
        var source = audioSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null) {
            // If no available audio source, create a new one
            source = new GameObject("Audio Source").AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSources.Add(source);
        }
        return source;
    }
}