using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectKey : MonoBehaviour
{
    [SerializeField]
    string key;

    void Start()
    {
        if(GameObjectDictionary.Current != null)
        {
            GameObjectDictionary.Current.Add(key, gameObject);
        }
    }

    void OnDestroy()
    {
        if (GameObjectDictionary.Current != null)
        {
            GameObjectDictionary.Current.Remove(key);
        }

    }
    
}
