/*
*   最終更新日:
*       6.3.2016
*
*   更新履歴:
*       6.3.2016:
*           プログラムの完成
*
*       6.21.2016:
*           カメラスイッチ追加
*           Position移動に使う座標系をWorldからLocalに変更
*/


using UnityEngine;
using System.Collections;

public class Tween : MonoBehaviour
{
    public enum ValueType
    {
        LocalPositionX,
        LocalPositionY,
        LocalPositionZ,
        LocalRotationX,
        LocalRotationY,
        LocalRotationZ,
        LocalScaleX,
        LocalScaleY,
        LocalScaleZ
    }

    public enum OperationMode
    {
        Non,
        Repeat,
        PingPong,
        SmoothStep,
        SmoothStepPingPong,

        Sin,
        Cos,
        Tan,
        Random,

        Parabola,
        ParabolaPingPong
    }

    public enum FilterMode
    {
        Non,
        Min,
        Max,
        MinMax
    }

    [System.Serializable]
    public class TweenItem
    {
        public bool enabled = true;

        [Space(10)]
        public ValueType valueType;
        public OperationMode operationMode;
        public float outWeight = 1.0f;

        [Space(10)]
        public float va = 0.0f;
        public float vb = 1.0f;
        public float speed = 1.0f;
        public float offsetTime = 0.0f;

        [Space(10)]
        public FilterMode filterMode;
        public float filterMin = 0.0f;
        public float filterMax = 1.0f;

        [HideInInspector]
        public float previousValue;
    }
    

    public bool cullingMode = false;
    public TweenItem[] tweenItemList;

    
    bool cameraVisible = false;


    void Start()
    {
        StartTween();
    }
    
    public void StartTween()
    {
        foreach(var tween in tweenItemList)
        {
            tween.previousValue = 0.0f;
        }
    }
    
    void OnBecameVisible()
    {
        //カメラに写っている
        cameraVisible = true;
    }

    void OnBecameInvisible()
    {
        //カメラに写っていない
        cameraVisible = false;
    }

    private void Update()
    {
        UpdateTween(Time.time);
    }

    public void UpdateTween(float time)
    {
        var deltaPosition = Vector3.zero;
        var deltaRotationEA = Vector3.zero;
        var deltaScale = Vector3.zero;

        
        if ((!cameraVisible && cullingMode) || tweenItemList.Length <= 0)
        {
            return;
        }


        foreach (var tw in tweenItemList)
        {
            if (!tw.enabled)
                continue;

            //tween
            switch (tw.valueType)
            {
                case ValueType.LocalPositionX:
                    deltaPosition.x += TweenFloat(tw, time);
                    break;

                case ValueType.LocalPositionY:
                    deltaPosition.y += TweenFloat(tw, time);
                    break;

                case ValueType.LocalPositionZ:
                    deltaPosition.z += TweenFloat(tw, time);
                    break;

                case ValueType.LocalRotationX:
                    deltaRotationEA.x += TweenFloat(tw, time);
                    break;

                case ValueType.LocalRotationY:
                    deltaRotationEA.y += TweenFloat(tw, time);
                    break;

                case ValueType.LocalRotationZ:
                    deltaRotationEA.z += TweenFloat(tw, time);
                    break;
                    
                case ValueType.LocalScaleX:
                    deltaScale.x += TweenFloat(tw, time);
                    break;

                case ValueType.LocalScaleY:
                    deltaScale.y += TweenFloat(tw, time);
                    break;

                case ValueType.LocalScaleZ:
                    deltaScale.z += TweenFloat(tw, time);
                    break;
                    
            }
            
        }

        transform.localPosition += deltaPosition;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + deltaRotationEA);
        transform.localScale += deltaScale;
        
    }

    
    float TweenFloat(TweenItem tw, float time)
    {
        float value = tw.va;
        float v = tw.vb - tw.va;
        float t = (time + tw.offsetTime) * tw.speed;

        //Tween
        switch (tw.operationMode)
        {
            case OperationMode.Repeat:
                value = tw.va + Mathf.Repeat(t, v);
                break;

            case OperationMode.PingPong:
                value = tw.va + Mathf.PingPong(t, v);
                break;

            case OperationMode.SmoothStep:
                value = Mathf.SmoothStep(tw.va, tw.vb, Mathf.Repeat(t, 1.0f));
                break;

            case OperationMode.SmoothStepPingPong:
                value = Mathf.SmoothStep(tw.va, tw.vb, Mathf.PingPong(t, 1.0f));
                break;

            case OperationMode.Sin:
                value = tw.va + (v / 2.0f) * Mathf.Sin(t) + (v / 2.0f);
                break;

            case OperationMode.Cos:
                value = tw.va + (v / 2.0f) * Mathf.Cos(t) + (v / 2.0f);
                break;

            case OperationMode.Tan:
                value = tw.va + (v / 2.0f) * Mathf.Tan(t) + (v / 2.0f);
                break;

            case OperationMode.Random:
                value = Random.Range(tw.va, tw.vb);
                break;

            case OperationMode.Parabola:
                value = tw.va + v * Mathf.Repeat(t, 1.0f) * Mathf.Repeat(t, 1.0f);
                break;

            case OperationMode.ParabolaPingPong:
                value = tw.va + v * Mathf.PingPong(t, 1.0f) * Mathf.PingPong(t, 1.0f);
                break;
        }

        // Out
        value *= tw.outWeight;

        //Filter
        switch (tw.filterMode)
        {
            case FilterMode.Min:
                value = Mathf.Min(value, tw.filterMin);
                break;

            case FilterMode.Max:
                value = Mathf.Max(value, tw.filterMax);
                break;

            case FilterMode.MinMax:
                value = Mathf.Clamp(value, tw.filterMin, tw.filterMax);
                break;
        }

        var delta = value - tw.previousValue;
        tw.previousValue = value;
        
        return delta;
    }
}
