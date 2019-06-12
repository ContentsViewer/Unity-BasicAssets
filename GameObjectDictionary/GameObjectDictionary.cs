using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDictionary : MonoBehaviour
{
    static GameObjectDictionary Instance { get;  set; }

    public static Dictionary<string, GameObject> Current { get; private set; }
    void Awake()
    {
        Current = new Dictionary<string, GameObject>();
        Instance = this;
    }
    

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Current = null;
        }
    }
    

}
