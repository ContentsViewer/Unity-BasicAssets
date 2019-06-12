/*
*プログラム:
*   最終更新日:
*       8.2.2016
*
*   説明:
*       Sound関係を扱います
*       AudioMixerを使用します
*       AudioMixer内の各グループ名とVolumeを設定するための変数名を各リストに追加してください
*
*   更新履歴:
*       3.22.2016:
*           プログラムの完成
*
*       3.30.2016; 4.13.2016; 4.18.2016:
*           スクリプト修正
*
*       5.16.2016:
*           CancelFade追加
*
*       7.8.2016:
*           シーンをまたいだときにDestroyされたAudioSourceにアクセスする問題を修正
*
*       8.2.2016:
*           シーン間でデストロイされなくした
*/


using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    

    public AudioMixer audioMixer;
    
    
    protected virtual void Awake()
    {
        if (!audioMixer)
        {
            Debug.LogWarning("SoundManager >> AudioMixer is not set.");
        }

        DontDestroyOnLoad(this);
        Instance = this;
    }


    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    public void Play(AudioSource audioSource, AudioClip clip, bool loop = false)
    {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void PlayDontOverride(AudioSource audioSource, AudioClip clip, bool loop = false)
    {
        if (!audioSource.isPlaying)
        {
            Play(audioSource, clip, loop);
        }
    }
    

    public void PlayFadeIn(AudioSource audioSource, AudioClip clip, bool loop, float v, float t, bool init)
    {
        Play(audioSource, clip, loop);
        FadeInVolume(audioSource, v, t, init);
    }

    public void PlayFadeInDontOverride(AudioSource audioSource, AudioClip clip, bool loop, float v, float t, bool init)
    {
        if (!audioSource.isPlaying)
        {
            PlayDontOverride(audioSource, clip, loop);
            FadeInVolume(audioSource, v, t, init);
        }
    }
    
    public void StopAllSound()
    {
        AudioSource[] audios = GameObject.FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            audio.Stop();
        }
    }

    public void PauseAllSound()
    {
        AudioSource[] audios = GameObject.FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            audio.Pause();
        }
    }
    

    class Fade
    {
        public AudioSource fadeAudio;
        public float targetV;
        public float dir;
        public float time;
        public float vmin;
        public float vmax;
        public float vrate;
        public void Set(AudioSource a, float v, float d, float t)
        {
            fadeAudio = a;
            targetV = v;
            dir = d;
            time = t;
            vrate = Mathf.Abs(a.volume - v);
            if (dir < 0.0f)
            {
                vmin = v;
                vmax = 1.0f;
            }
            else
            {
                vmin = 0.0f;
                vmax = v;
            }
        }
        public Fade(AudioSource a, float v, float d, float t)
        {
            Set(a, v, d, t);
        }
    };


    List<Fade> fadeStackList = new List<Fade>();
    

    /// <summary>
    ///     指定されたAudioSourceをFadeInにします
    ///     徐々に大きくなります
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="v">ターゲット音量</param>
    /// <param name="t"></param>
    /// <param name="init"></param>
    public void FadeInVolume(AudioSource audioSource, float v, float t, bool init)
    {
        if (init)
        {
            audioSource.volume = 0.0f;
        }

        if (audioSource.volume < 1.0f && audioSource.isPlaying)
        {
            if (fadeStackList.Count <= 0)
            {
                InvokeRepeating("SoundFade", 0.0f, 0.02f);
            }

            FadeStackAdd(audioSource, v, +1.0f, t);
        }
    }
    
    /// <summary>
    ///     指定されたAudioSourceをFadeOutにします
    //      徐々に小さくなります
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <param name="init"></param>
    public void FadeOutVolume(AudioSource audioSource, float v, float t, bool init)
    {
        if (init)
        {
            audioSource.volume = 1.0f;
        }

        if (audioSource.volume > 0.0f && audioSource.isPlaying)
        {
            if (fadeStackList.Count <= 0)
            {
                InvokeRepeating("SoundFade", 0.0f, 0.02f);
            }

            FadeStackAdd(audioSource, v, -1.0f, t);
        }
    }

    //
    //  説明:
    //      Fade処理
    void SoundFade()
    {
        //DestoroyされたAudioSourceをチェック
        for (int i = 0; i < fadeStackList.Count; i++)
        {
            if (!fadeStackList[i].fadeAudio)
            {
                fadeStackList.Remove(fadeStackList[i]);
            }
        }

        foreach (Fade fade in fadeStackList)
        {
            float v = fade.fadeAudio.volume + (fade.vrate * (0.02f / fade.time)) * fade.dir;
            fade.fadeAudio.volume = v;
        }

        for (int i = 0; i < fadeStackList.Count; i++)
        {
            if (fadeStackList[i].fadeAudio.volume <= fadeStackList[i].vmin || fadeStackList[i].fadeAudio.volume >= fadeStackList[i].vmax || !fadeStackList[i].fadeAudio.isPlaying)
            {
                if (fadeStackList[i].fadeAudio.volume <= 0.0f)
                {
                    fadeStackList[i].fadeAudio.Stop();
                }
                fadeStackList.Remove(fadeStackList[i]);
            }
        }
        if (fadeStackList.Count <= 0)
        {
            CancelInvoke("SoundFade");
        }
    }

    public void CancelFade(AudioSource audioSource)
    {
        for (int i = 0; i < fadeStackList.Count; i++)
        {
            if (fadeStackList[i].fadeAudio == audioSource)
            {
                fadeStackList.Remove(fadeStackList[i]);
            }
        }
    }

    private void FadeStackAdd(AudioSource audioSource, float v, float d, float t)
    {

        foreach (Fade fadeStack in fadeStackList)
        {
            if (fadeStack.fadeAudio == audioSource)
            {
                fadeStack.Set(audioSource, v, d, t);
                return;
            }
        }

        fadeStackList.Add(new Fade(audioSource, v, d, t));
    }

    // --- これは, AudioMixerが既に持っている. ---
    //public bool MixerSetFloat(string name, float value)
    //{
    //    if (audioMixer)
    //    {
    //        return audioMixer.SetFloat(name, value);
    //    }
    //    return false;
    //}

    //public bool MixerGetFlaot(string name, out float value)
    //{
    //    if (audioMixer)
    //    {
    //        return audioMixer.GetFloat(name, out value);
    //    }
    //    value = float.NaN;
    //    return false;
    //}
    // End AudioMixerが既に持っている ---

}
