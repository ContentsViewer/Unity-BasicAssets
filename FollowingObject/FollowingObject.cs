using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingObject : MonoBehaviour
{
    public GameObject target;
    public float smoothTime = 0.5f;
    public float maxSpeed = 100.0f;

    Vector3 currentVelocity;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (!target) return;

        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);
        
    }
}
