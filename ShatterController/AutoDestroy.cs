using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float time = 2.0f;

    float startTime;

    void Start()
    {
        startTime = Time.time;        
    }

    void Update()
    {
        if (Time.time > startTime + time)
        {
            Destroy(gameObject);
        }
    }

}
