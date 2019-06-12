/*
*プログラム
*   最終更新日:
*       11.11.2016
*
*   説明:
*       BGM管理をします
*
*   更新履歴:
*       5.17.2016:
*           プログラムの完成
*
*       5.22.2016:
*           スクリプト修正
*
*       7.9.2016:
*           playOnLoad設定されてかつ同じシーンを読み込まれたときにその設定されたBGMを再生しない問題を修正
*           変数IsPlayng―BGMが再生されているか―を追加
*
*       8.2.2016:
*           シーン間でデストロイされなくした
*           
*       11.11.2016:
*           UnityUpdateに伴う修正; OnLevelWasLoadedの代わりにSceneManagerを使用
*
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    public AudioSource AudioSource { get; private set; }

    public float defaultFadeInTime = 3.0f;
    public float defaultFadeOutTime = 2.0f;

    bool fadeInNextAudio;
    AudioClip nextAudioClip;


    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Awake()
    {
        AudioSource = gameObject.GetComponent<AudioSource>();
        AudioSource.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneWasLoaded;

        DontDestroyOnLoad(this);

        Instance = this;
    }

    void Start()
    {

    }


    void OnSceneWasLoaded(Scene scenename, LoadSceneMode SceneMode)
    {
        Stop();
    }

    void Update()
    {
        if (fadeInNextAudio && !AudioSource.isPlaying)
        {
            SoundManager.Instance.PlayFadeIn(AudioSource, nextAudioClip, true, 1.0f, defaultFadeInTime, true);

            fadeInNextAudio = false;
        }

    }


    public void Play(AudioClip clip)
    {
        Stop();
        AudioSource.volume = 1.0f;
        SoundManager.Instance.Play(AudioSource, clip, true);
    }

    public void Stop()
    {
        fadeInNextAudio = false;
        SoundManager.Instance.CancelFade(AudioSource);
        AudioSource.Stop();
    }


    public void PlayFadeIn(AudioClip clip)
    {
        Stop();
        SoundManager.Instance.PlayFadeIn(AudioSource, clip, true, 1.0f, defaultFadeInTime, true);
    }


    public void FadeOut()
    {
        SoundManager.Instance.FadeOutVolume(AudioSource, 0.0f, defaultFadeOutTime, false);
    }


    public void PlayFade(AudioClip clip)
    {
        SoundManager.Instance.CancelFade(AudioSource);
        SoundManager.Instance.FadeOutVolume(AudioSource, 0.0f, defaultFadeOutTime, false);
        nextAudioClip = clip;
        fadeInNextAudio = true;
    }

}
