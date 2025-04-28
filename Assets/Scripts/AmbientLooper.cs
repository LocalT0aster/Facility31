using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientLooper : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip ambientClip;
    [Range(0, 1)] public float volume = 0.5f;
    public bool playOnAwake = true;
    public bool fadeInOnStart = true;
    public float fadeDuration = 3f;

    private AudioSource audioSource;
    private float targetVolume;
    private float fadeTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = ambientClip;
        audioSource.volume = 0;
        audioSource.loop = true;
        targetVolume = volume;
    }

    private void Start()
    {
        if (playOnAwake)
        {
            audioSource.Play();
            if (fadeInOnStart) fadeTimer = fadeDuration;
        }
    }

    private void Update()
    {
        // Handle fade in
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, targetVolume, 1 - (fadeTimer / fadeDuration));
        }
    }

    public void SetVolume(float newVolume)
    {
        targetVolume = Mathf.Clamp01(newVolume);
        fadeTimer = fadeDuration; // Apply fade to volume change
    }
}
