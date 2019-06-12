using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Tween))]
public class TweenEditor : Editor
{
    Tween tween = null;

    Vector3 initialLocalPosition;
    Vector3 initialLocalRotationEA;
    Vector3 initialLocalScale;

    void SaveInitialPose(Tween tween)
    {
        initialLocalPosition = tween.transform.localPosition;
        initialLocalRotationEA = tween.transform.localRotation.eulerAngles;
        initialLocalScale = tween.transform.localScale;
    }

    void RevertPose(Tween tween)
    {
        tween.transform.localPosition = initialLocalPosition;
        tween.transform.localRotation = Quaternion.Euler(initialLocalRotationEA);
        tween.transform.localScale = initialLocalScale;
    }


    void OnEnable()
    {
        tween = target as Tween;
    }

    void OnDisable()
    {
        Stop();
        tween = null;
    }

    //void OnSceneGUI()
    //{
    //    if (tween && isPlaying)
    //    {
    //        //var prevBackgroundColor = GUI.backgroundColor;
    //        //GUI.backgroundColor = Color.red;
    //        //serializedObject.Update();
            
    //        //tween.UpdateTween((float)EditorApplication.timeSinceStartup);

    //        //var trfm = tween.GetComponent<Transform>();

    //        //var serializedTrfm = new SerializedObject(tween.transform);
    //        //serializedTrfm.Update();
    //        //var localPositionProperty = serializedTrfm.FindProperty("m_LocalPosition");
    //        //var localRotationProperty = serializedTrfm.FindProperty("m_LocalRotation");
    //        //var localScaleProperty = serializedTrfm.FindProperty("m_LocalScale");
    //        //localPositionProperty.vector3Value = tween.transform.localPosition;
    //        //localRotationProperty.quaternionValue = tween.transform.localRotation;
    //        //localScaleProperty.vector3Value = tween.transform.localScale;

    //        //Debug.Log(localPositionProperty);
    //        //var property = serializedTrfm.GetIterator();

    //        //while(true)
    //        //{

    //        //    Debug.Log(property.name);
    //        //    if (!property.Next(true))
    //        //        break;
    //        //}
    //        //serializedTrfm.ApplyModifiedProperties();
    //        //Debug.Log(trfm.localRotation);
    //        //serializedTrfm.ApplyModifiedPropertiesWithoutUndo();
    //        //serializedObject.ApplyModifiedProperties();

    //        //Debug.Log(tween);
    //        //GUI.backgroundColor = prevBackgroundColor;
    //    }

    //}

    bool isPlaying = false;

    public override void OnInspectorGUI()
    {
        if (!tween)
            return;


        EditorGUILayout.LabelField("Preview");
        var prevBackgroundColor = GUI.backgroundColor;

        if (isPlaying)
        {
            tween.UpdateTween((float)EditorApplication.timeSinceStartup);

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Stop"))
                Stop();
        }
        else if (!isPlaying)
        {
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Play"))
                Play();
        }

        GUI.backgroundColor = prevBackgroundColor;
        
        DrawDefaultInspector();
    }

    void Play()
    {
        if (isPlaying)
            return;

        tween.StartTween();
        SaveInitialPose(tween);
        isPlaying = true;
    }

    void Stop()
    {
        if (!isPlaying)
            return;
        
        RevertPose(tween);
        isPlaying = false;
    }
}
