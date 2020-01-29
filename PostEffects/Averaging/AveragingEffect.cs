using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[ExecuteInEditMode]
public class AveragingEffect : MonoBehaviour
{
    [SerializeField]
    Shader shader;



    [SerializeField, Range(0, 20)]
    int resolution = 0;
    public int Resolution { get { return resolution; } set { resolution = value; } }

    Material material;


    void SetupObjects()
    {
        if (material != null) return;


        material = new Material(shader);
    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/Averaging");
    }

    void Start()
    {
        SetupObjects();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetupObjects();

        if (resolution == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        CommandBuffer command = new CommandBuffer();

        int rt1 = Shader.PropertyToID("RT1");
        command.GetTemporaryRT(rt1, -resolution, -resolution, 0, FilterMode.Trilinear);
        command.SetGlobalVector(Shader.PropertyToID("_PixelSize"), new Vector4((float)resolution / Screen.width, (float)resolution / Screen.height, 0, 0));

        command.Blit((RenderTargetIdentifier)source, rt1, material);
        command.Blit(rt1, destination, material);

        Graphics.ExecuteCommandBuffer(command);
    }
}
