using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSceneManager : MonoBehaviour
{
    float startTime;
    void Start()
    {
        Time.timeScale = 1.0f;
        if(SceneLoader.Instance != null)
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        startTime = Time.unscaledTime;
    }

    void Update()
    {

        if (Time.unscaledTime - startTime > 2.0f && SceneLoader.Instance != null)
        {
            SceneLoader.Instance.TriggerToLoadTargetScene();
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //{

        //    SceneLoader.Instance.TriggerToLoadTargetScene();
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{

        //    Resources.UnloadUnusedAssets();
        //    System.GC.Collect();
        //}
    }

}
