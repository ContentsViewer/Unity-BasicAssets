/*
*プログラム: SceneLoader
*   最終更新日:
*       8.5.2016
*
*   説明:
*       Sceneをロードします
*       進行状況を参照することができます
*
*   更新履歴:
*       3.20.2016:
*           プログラムの完成
*
*       3.31.2016:
*           スクリプト修正
*
*       7.9.2016:
*           シーンが読まれるときに実行するdelegate関数を追加
*
*       7.18.2016:
*           ロード中のシーンと同じ名前のシーンを再びロードする問題を修正
*
*       8.5.2016:
*           LoadSceneの返り値としてLoadに成功しているかのBool値を用意した
*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    //===外部パラメータ===================================================

    public enum State
    {
        Non,
        LoadingBridgeScene,
        WaitForTriggeredToLoadTargetScene,
        LoadingTargetScene,
        Done
    }

    public static SceneLoader Instance { get; private set; }

    public bool useBridgeScene;
    public string bridgeSceneName = "";

    public State GetState() { return state; }

    public float Progress { get; private set; }

    public bool IsLoading { get { return state == State.LoadingBridgeScene || state == State.WaitForTriggeredToLoadTargetScene || state == State.LoadingTargetScene; } }
    public string LoadingSceneName { get; private set; }
    public string TargetSceneName { get; private set; }

    public delegate void OnSceneWillBeLoadedHandler();
    public delegate void OnSceneWasLoadedHandler();


    public event OnSceneWillBeLoadedHandler OnSceneWillBeLoaded;
    public event OnSceneWasLoadedHandler OnSceneWasLoaded;


    AsyncOperation nextScene;
    State state;


    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;

        //for (int i = 0; i < SceneManager.sceneCount; i++)
        //{
        //    Debug.Log((SceneManager.GetSceneAt(i).name));
        //}
    }

    void Update()
    {

        switch (state)
        {
            case State.Non:
                return;


            case State.LoadingBridgeScene:
                Progress = nextScene.progress / 2.0f;

                if (nextScene.isDone)
                {
                    state = State.WaitForTriggeredToLoadTargetScene;
                }
                break;


            case State.WaitForTriggeredToLoadTargetScene:

                break;

            case State.LoadingTargetScene:

                if (useBridgeScene)
                {
                    Progress = 0.5f + nextScene.progress / 2.0f;
                }
                else
                {
                    Progress = nextScene.progress;
                }


                if (nextScene.isDone)
                {
                    state = State.Done;
                }

                break;


            case State.Done:
                if (OnSceneWasLoaded != null)
                {
                    OnSceneWasLoaded();
                }
                
                state = State.Non;
                break;
        }

    }


    IEnumerator LoadWork(string sceneName)
    {
        nextScene = SceneManager.LoadSceneAsync(sceneName);
        yield return nextScene;
    }


    public void TriggerToLoadTargetScene()
    {
        if (state != State.WaitForTriggeredToLoadTargetScene)
        {

            return;
        }

        StartCoroutine(LoadWork(TargetSceneName));

        if (nextScene == null)
        {
            Debug.LogWarning("TargetScene not found: " + TargetSceneName);

            return;
        }


        state = State.LoadingTargetScene;

    }

    public bool CanSceneBeLoaded(string sceneName)
    {
        if (useBridgeScene)
        {
            return Application.CanStreamedLevelBeLoaded(sceneName) &&
                Application.CanStreamedLevelBeLoaded(bridgeSceneName);
        }


        return Application.CanStreamedLevelBeLoaded(sceneName);
    }

    public bool LoadScene(string sceneName)
    {

        if (!CanSceneBeLoaded(sceneName))
        {
            Debug.LogWarning(string.Format("SceneLoader >> scene '{0}' or bridge scene '{1}'(if use bridge scene) not found.", sceneName, bridgeSceneName));
            return false;
        }

        if (IsLoading && sceneName == TargetSceneName)
        {
            return true;
        }

        if (useBridgeScene)
        {
            StartCoroutine(LoadWork(bridgeSceneName));
        }
        else
        {

            StartCoroutine(LoadWork(sceneName));
        }


        if (nextScene == null)
        {
            state = State.Non;

            Debug.LogWarning("Scene not found: " + sceneName);

            return false;
        }

        Progress = 0.0f;
        TargetSceneName = sceneName;

        if (useBridgeScene)
        {
            LoadingSceneName = bridgeSceneName;

            state = State.LoadingBridgeScene;

        }
        else
        {
            LoadingSceneName = sceneName;

            state = State.LoadingTargetScene;
        }


        if (OnSceneWillBeLoaded != null)
        {
            OnSceneWillBeLoaded();
        }

        return true;
    }


}
