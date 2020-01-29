using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ContrastEffect : MonoBehaviour
{
    [SerializeField]
    Shader shader;
    
    [SerializeField, Range(0, 20)]
    float contrast = 10;

    public float Contrast { get => contrast; set => contrast = value; }

    Material material;

    void SetupObjects()
    {
        if (material != null) return;


        material = new Material(shader);
    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/Contrast");
    }

    void Start()
    {
        SetupObjects();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetupObjects();

        material.SetFloat("_Contrast", contrast);
        Graphics.Blit(source, destination, material);
    }
}
