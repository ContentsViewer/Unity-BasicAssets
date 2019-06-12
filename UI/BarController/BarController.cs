using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BarController : MonoBehaviour
{
    public Slider bar;
    public Image diff;

    public Color addColor = Color.green;
    public Color subtractColor = Color.red;

    public float updateTime = 1.0f;

    public float Value
    {
        get { return currentValue; }
        set
        {
            if (Mathf.Abs(value - currentValue) < float.Epsilon)
                return;

            valueChangeTime = Time.time;
            currentValue = value;

            if (prevValue < currentValue)
            {
                // add
                diffRectTrfm.anchorMin = new Vector2(prevValue, 0.0f);
                diffRectTrfm.anchorMax = new Vector2(currentValue, 1.0f);
                diff.color = addColor;

                bar.value = prevValue;
                
                dir = 1;
            }
            else
            {
                // subtract
                diffRectTrfm.anchorMin = new Vector2(currentValue, 0.0f);
                diffRectTrfm.anchorMax = new Vector2(prevValue, 1.0f);
                diff.color = subtractColor;

                bar.value = currentValue;
                
                dir = -1;
            }

        }
    }

    float currentValue;
    float prevValue;
    float valueChangeTime;
    RectTransform diffRectTrfm;
    RectTransform fillRect;
    int dir;

    void Awake()
    {
        diffRectTrfm = diff.GetComponent<RectTransform>();
        fillRect = bar.fillRect;

        currentValue = 1.0f;
        prevValue = 1.0f;
        bar.value = 1.0f;
        diffRectTrfm.anchorMin = new Vector2(1.0f, 0.0f);
        diffRectTrfm.anchorMax = new Vector2(1.0f, 1.0f);
    }

    float valueChangeDeltaTime;
    
    void Update()
    {


        if (Time.time - valueChangeTime > updateTime)
        {
            prevValue = currentValue;
            //if (currentValue - bar.value > float.Epsilon)
            //{
            //    bar.value += Time.deltaTime;
            //    if (bar.value > currentValue)
            //    {
            //        bar.value = currentValue;
            //    }
            //}
            //if (bar.value - currentValue > float.Epsilon)
            //{
            //    bar.value -= Time.deltaTime;
            //    if (bar.value < currentValue)
            //    {
            //        bar.value = currentValue;
            //    }
            //}

            if (dir > 0)
            {
                if(Mathf.Abs(currentValue - bar.value) > float.Epsilon)
                {
                    valueChangeDeltaTime += Time.deltaTime / 2.0f;
                }
                else
                {
                    valueChangeDeltaTime = 0.0f;
                }

                bar.value = Mathf.Lerp(bar.value, currentValue, valueChangeDeltaTime);
                diffRectTrfm.anchorMin = new Vector2(bar.value, 0.0f);
            }

            if (dir < 0)
            {
                if (Mathf.Abs(currentValue - diffRectTrfm.anchorMax.x) > float.Epsilon)
                {
                    valueChangeDeltaTime += Time.deltaTime / 2.0f;
                }
                else
                {
                    valueChangeDeltaTime = 0.0f;
                }

                diffRectTrfm.anchorMax = new Vector2(Mathf.Lerp(diffRectTrfm.anchorMax.x, currentValue, valueChangeDeltaTime), 1.0f);
            }



        }
    }
}
