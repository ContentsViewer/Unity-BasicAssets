using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode()]
public class BinaryEffect : MonoBehaviour
{

    [SerializeField, Range(0, 1)] float threshold = 1;
    public float Threshold
    {
        get { return threshold; }
        set { threshold = value; }
    }

    public enum DitherType
    {
        Bayer2x2, Bayer3x3, Bayer4x4, Bayer8x8, BlueNoise64x64
    };


    [SerializeField] DitherType ditherType;

    //public DitherType DitherType
    //{
    //    get { return ditherType; }
    //    set { ditherType = value; }
    //}

    // Scale factor of dither pattan
    [SerializeField, Range(1, 8)] int ditherScale = 1;

    public int DitherScale
    {
        get { return ditherScale; }
        set { ditherScale = value; }
    }

    // Color (dark)
    [SerializeField] Color color0 = Color.black;

    public Color Color0
    {
        get { return color0; }
        set { color0 = value; }
    }

    // Color (light)
    [SerializeField] Color color1 = Color.white;

    public Color Color1
    {
        get { return color1; }
        set { color1 = value; }
    }

    // Opacity
    [SerializeField, Range(0, 1)] float opacity = 1.0f;

    public float Opacity
    {
        get { return opacity; }
        set { opacity = value; }
    }

    [SerializeField]
    Shader shader;

    [SerializeField, HideInInspector] Texture2D bayer2x2Texture;
    [SerializeField, HideInInspector] Texture2D bayer3x3Texture;
    [SerializeField, HideInInspector] Texture2D bayer4x4Texture;
    [SerializeField, HideInInspector] Texture2D bayer8x8Texture;
    [SerializeField, HideInInspector] Texture2D bnoise64x64Texture;

    Texture2D DitherTexture
    {
        get
        {
            switch (ditherType)
            {
                case DitherType.Bayer2x2: return bayer2x2Texture;
                case DitherType.Bayer3x3: return bayer3x3Texture;
                case DitherType.Bayer4x4: return bayer4x4Texture;
                case DitherType.Bayer8x8: return bayer8x8Texture;
                default: return bnoise64x64Texture;
            }
        }
    }

    Material material;
    void SetupObjects()
    {
        if (material != null) return;

        material = new Material(shader);

    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/Binary");
        SetupObjects();
    }

    void Start()
    {
        SetupObjects();
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetupObjects();


        material.SetTexture("_DitherTex", DitherTexture);
        material.SetFloat("_Scale", ditherScale);
        material.SetFloat("_Threshold", threshold);
        material.SetColor("_Color0", color0);
        material.SetColor("_Color1", color1);
        material.SetFloat("_Opacity", opacity);
        Graphics.Blit(source, destination, material);
    }
}
