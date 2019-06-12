using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioRandomRepeater : MonoBehaviour
{

    public AudioClip[] clipList;

    public float intervalRandomRangeMin = 0.0f;
    public float intervalRandomRangeMax = 0.5f;


    public bool IsPlaying { get; private set; }


    AudioSource audioSource;
    float switchTime;
    float waitTime;


    enum State
    {
        Wait,
        Playing
    }
    State state;

    public void PlayDontOverride()
    {
        if (IsPlaying)
        {
            return;
        }

        Play();
    }

    public void Play()
    {
        audioSource.clip = clipList[Random.Range(0, clipList.Length)];
        audioSource.Play();

        state = State.Playing;


        IsPlaying = true;


    }

    public void Stop()
    {
        IsPlaying = false;

        audioSource.Stop();
    }



    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if(clipList.Length <= 0)
        {
            Debug.LogWarning("AudioRandomRepeater >> No audioClip is registed.");
            enabled = false;
        }

    }


    void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        switch (state)
        {
            case State.Playing:
                if (!audioSource.isPlaying)
                {

                    state = State.Wait;
                    switchTime = Time.time;
                    waitTime = Random.Range(intervalRandomRangeMin, intervalRandomRangeMax);
                }

                break;


            case State.Wait:
                if(Time.time > switchTime + waitTime)
                {

                    audioSource.clip = clipList[Random.Range(0, clipList.Length)];
                    audioSource.Play();

                    state = State.Playing;
                }


                break;
        }



    }

    


}
