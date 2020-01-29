using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class UnsharpMaskingEffect : MonoBehaviour
{
    [SerializeField]
    Shader shader;
    
    [SerializeField, Range(0, 20)]
    float intensity = 0;
    public float Intensity { get { return intensity; } set { intensity = value; } }

    Material material;


    void SetupObjects()
    {
        if (material != null) return;


        material = new Material(shader);
    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/UnsharpMasking");
    }

    void Start()
    {
        SetupObjects();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetupObjects();

        material.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, destination, material);
    }
}
