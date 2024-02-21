using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;

    void Start() {
        source.clip = clips.Random();
        source.Play();
    }
}
