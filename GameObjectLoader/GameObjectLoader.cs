/*
*プログラム: GameObjectLoader
*   最終更新日:
*       3.16.2016
*
*   説明:
*       Awake, Start, Update, FixedUpdate関数実行時に登録されたGameObjectをロードします
*       一度GameObjectがロードされるとシーン間で破棄されません
*
*   更新履歴:
*       3.16.2016:
*           プログラムの完成
*/


/*
*プログラム: GameObjectLoader
*   最終更新日:
*       3.16.2016
*
*   説明:
*       Awake, Start, Update, FixedUpdate関数実行時に登録されたGameObjectをロードします
*       一度GameObjectがロードされるとシーン間で破棄されません
*
*   更新履歴:
*       3.16.2016:
*           プログラムの完成
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectLoader : MonoBehaviour
{
    //===外部パラメータ(inspector表示)=========================================
    public GameObject[] loadGameObjectListAwake;
    public GameObject[] loadGameObjectListStart;
    public GameObject[] loadGameObjectListUpdate;
    public GameObject[] loadGameObjectListFixedUpdate;


    //===外部パラメータ=====================================================
    [System.NonSerialized]
    public Dictionary<string, GameObject> loadedGameObjectListAwake = new Dictionary<string, GameObject>();
    [System.NonSerialized]
    public bool loadedAwake = false;
    [System.NonSerialized]
    public Dictionary<string, GameObject> loadedGameObjectListStart = new Dictionary<string, GameObject>();
    [System.NonSerialized]
    public bool loadedStart = false;
    [System.NonSerialized]
    public Dictionary<string, GameObject> loadedGameObjectListUpdate = new Dictionary<string, GameObject>();
    [System.NonSerialized]
    public bool loadedUpdate = false;
    [System.NonSerialized]
    public Dictionary<string, GameObject> loadedGameObjectListFixedUpdate = new Dictionary<string, GameObject>();
    [System.NonSerialized]
    public bool loadedFixedUpdate = false;

    //===内部パラメータ=========================================================
    bool loaded = false;


    void Awake()
    {
        //各ゲームオブジェクトのロードチェック
        //初回起動チェック
        bool loadedAll = false;
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gos)
        {
            GameObjectLoader ol = go.GetComponent<GameObjectLoader>();
            if (ol)
            {
                if (ol.loaded)
                {
                    loadedAll = true;
                    break;
                }
            }
        }
        if (loadedAll)
        {
            Destroy(gameObject);
            return;
        }
        loaded = true;


        //Awake処理
        if (!loadedAwake)
        {
            loadedAwake = true;
            LoadGameObject(loadGameObjectListAwake, loadedGameObjectListAwake);

        }
    }

    void Start()
    {
        //Start処理
        if (!loadedStart)
        {
            loadedStart = true;
            LoadGameObject(loadGameObjectListStart, loadedGameObjectListStart);
        }
    }

    void Update()
    {
        //Update処理
        if (!loadedUpdate)
        {
            loadedUpdate = true;
            LoadGameObject(loadGameObjectListUpdate, loadedGameObjectListUpdate);
        }
    }

    void FixedUpdate()
    {
        //FixedUpdate処理
        if (!loadedFixedUpdate)
        {
            loadedFixedUpdate = true;
            LoadGameObject(loadGameObjectListFixedUpdate, loadedGameObjectListFixedUpdate);
        }
    }


    void LoadGameObject(GameObject[] loadGameObjectList, Dictionary<string, GameObject> loadedGameObjectList)
    {
        //GameObjectLoaderがシーン移動で削除されないようにする
        //(ロードされたオブジェクトも消えない)
        DontDestroyOnLoad(this);

        //登録されているゲームオブジェクトをロード
        foreach (GameObject go in loadGameObjectList)
        {
            if (go)
            {
                if (loadedGameObjectList.ContainsKey(go.name))
                {
                    //ロード済み
                }
                else
                {
                    //ロード
                    GameObject goInstance = Instantiate(go) as GameObject;
                    goInstance.name = go.name;
                    goInstance.transform.parent = gameObject.transform;
                    loadedGameObjectList.Add(go.name, goInstance);
                    Debug.Log(string.Format("Loaded GameObject {0}", go.name));
                }
            }
        }

    }
}


//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class GameObjectLoader : MonoBehaviour
//{
//    //===外部パラメータ(inspector表示)=========================================
//    public GameObject[] loadGameObjectList;

//    //===外部パラメータ=====================================================
//    [System.NonSerialized]
//    public Dictionary<string, GameObject> loadedGameObjectList = new Dictionary<string, GameObject>();
//    [System.NonSerialized]
//    public bool loadedAwake = false;

//    public bool LoadComplete = false;
//    //===内部パラメータ=========================================================
//    bool loaded = false;


//    void Awake()
//    {
//        //各ゲームオブジェクトのロードチェック
//        //初回起動チェック
//        bool loadedAll = false;
//        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
//        foreach (GameObject go in gos)
//        {
//            GameObjectLoader ol = go.GetComponent<GameObjectLoader>();
//            if (ol)
//            {
//                if (ol.loaded)
//                {
//                    loadedAll = true;
//                    break;
//                }
//            }
//        }
//        if (loadedAll)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        loaded = true;


//        //Awake処理
//        if (!loadedAwake)
//        {
//            StartCoroutine(LoadGameObject(loadGameObjectList, loadedGameObjectList));
//            loadedAwake = true;
//        }
//    }


//    IEnumerator LoadGameObject(GameObject[] loadGameObjectList, Dictionary<string, GameObject> loadedGameObjectList)
//    {
//        //GameObjectLoaderがシーン移動で削除されないようにする
//        //(ロードされたオブジェクトも消えない)
//        DontDestroyOnLoad(this);
//        LoadComplete = false;
//        float nextframetime = Time.realtimeSinceStartup + 0.01f;
//        //登録されているゲームオブジェクトをロード
//        foreach (GameObject go in loadGameObjectList)
//        {
//            if (Time.realtimeSinceStartup >= nextframetime)
//            {
//                yield return null;
//                nextframetime = Time.realtimeSinceStartup + 0.01f;
//            }


//            if (go)
//            {
//                if (loadedGameObjectList.ContainsKey(go.name))
//                {
//                    //ロード済み
//                }
//                else
//                {
//                    //ロード
//                    GameObject goInstance = Instantiate(go) as GameObject;
//                    goInstance.SetActive(false);
//                    goInstance.name = go.name;
//                    goInstance.transform.parent = gameObject.transform;
//                    loadedGameObjectList.Add(go.name, goInstance);
//                    //Debug.Log(string.Format("Loaded GameObject {0}", go.name));
//                }
//            }
//        }

//        foreach(var it in loadedGameObjectList){
//            if (Time.realtimeSinceStartup >= nextframetime)
//            {
//                yield return null;
//                nextframetime = Time.realtimeSinceStartup + 0.01f;
//            }

//            it.Value.SetActive(true);
//        }
//        LoadComplete = true;
//    }
//}
