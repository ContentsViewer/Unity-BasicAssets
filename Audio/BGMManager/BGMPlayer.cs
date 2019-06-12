using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioClip clip;
    
    void Start()
    {
        BGMManager.Instance.PlayFadeIn(clip);
    }
}
