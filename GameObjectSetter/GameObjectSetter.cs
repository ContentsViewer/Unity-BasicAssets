using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSetter : MonoBehaviour
{

    [System.Serializable]
    public class GameObjectAndPositionReferencePair
    {
        public GameObject gameObject;
        public GameObject positionReference;
    }

    public GameObjectAndPositionReferencePair[] gameObjectAndPositionReferencePairList;
    

    public void Set()
    {
        foreach(var gameObjectAndPositionReferencePair in gameObjectAndPositionReferencePairList)
        {
            gameObjectAndPositionReferencePair.gameObject.transform.position = gameObjectAndPositionReferencePair.positionReference.transform.position;
        }
    }
}
