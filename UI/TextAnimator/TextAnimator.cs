using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class TextAnimator : MonoBehaviour
{

    public enum UpdateMode
    {
        ScaledTime,
        UnscaledTime
    }

    public string[] texts;
    public float speed;
    public UpdateMode updateMode;
    
    float lastUpdateTime;
    int textsIndex;
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();    
    }

    void Update()
    {
        if(GetTime() - lastUpdateTime > 1.0f / speed)
        {
            if(texts.Length > 0)
            {
                text.text = texts[textsIndex];
                textsIndex = ++textsIndex % texts.Length;
            }

            lastUpdateTime = GetTime();
        }
        
    }

    float GetTime()
    {
        switch (updateMode)
        {
            default:
            case UpdateMode.ScaledTime:
                return Time.time;

            case UpdateMode.UnscaledTime:
                return Time.unscaledTime;

        }
    }
}
