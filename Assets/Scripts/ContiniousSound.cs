using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ContinuousSound : MonoBehaviour
{
    public bool PlayOnStart = true;
    private bool playing;
    public AudioClip sound;
    private AudioSource src;

    void Start()
    {
        src = GetComponent<AudioSource>();
        src.clip = sound;
        src.loop = true;
        if (PlayOnStart) {
            src.Play();
            playing = true;
            return;
        }
        playing = false;
    }

    public void ToggleSound(bool play)
    {
        if (play && !playing) {
            playing = true;
            src.Play(); 
        }
        else { 
            src.Stop();
            playing = false;
        }
    }
}

