using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class ParticleEmitAudioPlayer : MonoBehaviour
{
    public float delay;
    ParticleSystem particleSystem;
    AudioSource audioSource;

    float previousTime;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (!particleSystem.isEmitting) return;

        if (previousTime > particleSystem.time) previousTime = 0.0f;
        
        if (previousTime <= delay && particleSystem.time >= delay)
        {
            audioSource.Play();
        }


        previousTime = particleSystem.time;
    }
}
