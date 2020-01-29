using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;

[ExecuteInEditMode]
public class PixelationEffect : MonoBehaviour
{
    [SerializeField]
    Shader shader;

    [SerializeField]
    float pixelNumberX = 200;

    [SerializeField]
    float pixelNumberY = 200;
    



    Material material;

    void SetupObjects()
    {
        if (material != null) return;



        material = new Material(shader);


    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/Pixelation");

        SetupObjects();
    }

    void Start()
    {
        SetupObjects();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetupObjects();

        //CommandBuffer command = new CommandBuffer();

        //int rt1 = Shader.PropertyToID("RT1");
        //command.GetTemporaryRT(rt1, -(int)pixelNumberX, -(int)pixelNumberY, 0, FilterMode.Point);

        //command.Blit((RenderTargetIdentifier)source, rt1);
        //command.Blit(rt1, destination);
        //Graphics.ExecuteCommandBuffer(command);
        material.SetFloat("_PixelNumberX", pixelNumberX);
        material.SetFloat("_PixelNumberY", pixelNumberY);

        Graphics.Blit(source, destination, material);

    }
}
