using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PencilEffect : MonoBehaviour
{
    [SerializeField]
    Shader shader;

    
    [SerializeField, Range(0, 1)]
    float saturation = 1.0f;

    Material material;


    void SetupObjects()
    {
        if (material != null) return;


        material = new Material(shader);
    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/PencilShader");
    }

    void Start()
    {
        SetupObjects();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetupObjects();

        material.SetFloat("_Saturation", saturation);
        Graphics.Blit(source, destination, material);
    }
}
