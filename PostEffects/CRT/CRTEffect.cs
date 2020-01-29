using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class CRTEffect : MonoBehaviour
{

    [SerializeField]
    Shader shader;

    Material material;

    [SerializeField]
    [Range(0, 1)]
    float noiseX;
    public float NoiseX { get { return noiseX; } set { noiseX = value; } }

    [SerializeField]
    [Range(0, 1)]
    float rgbNoise;
    public float RGBNoise { get { return rgbNoise; } set { rgbNoise = value; } }

    [SerializeField]
    [Range(0, 1)]
    float sinNoiseScale;
    public float SinNoiseScale { get { return sinNoiseScale; } set { sinNoiseScale = value; } }

    [SerializeField]
    [Range(0, 10)]
    float sinNoiseWidth;
    public float SinNoiseWidth { get { return sinNoiseWidth; } set { sinNoiseWidth = value; } }

    [SerializeField]
    float sinNoiseOffset;
    public float SinNoiseOffset { get { return sinNoiseOffset; } set { sinNoiseOffset = value; } }

    [SerializeField]
    Vector2 offset;
    public Vector2 Offset { get { return offset; } set { offset = value; } }

    [SerializeField]
    [Range(0, 2)]
    float scanLineTail = 1.5f;
    public float ScanLineTail { get { return scanLineTail; } set { scanLineTail = value; } }

    [SerializeField]
    [Range(-10, 10)]
    float scanLineSpeed = 10;
    public float ScanLineSpeed { get { return scanLineSpeed; } set { scanLineSpeed = value; } }

    [SerializeField]
    [Range(0, 1)]
    float fadeParam = 1f;
    public float FadeParam { get { return fadeParam; } set { fadeParam = value; } }

    [SerializeField]
    float colorBias = 1f;
    public float ColorBias { get { return colorBias; } set { colorBias = value; } }


    void SetupObjects()
    {
        if (material != null) return;

        material = new Material(shader);

    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/CRT");
        SetupObjects();
    }

    void Start()
    {
        SetupObjects();
    }


    // Use this for initialization
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetupObjects();

        material.SetFloat("_NoiseX", noiseX);
        material.SetFloat("_RGBNoise", rgbNoise);
        material.SetFloat("_SinNoiseScale", sinNoiseScale);
        material.SetFloat("_SinNoiseWidth", sinNoiseWidth);
        material.SetFloat("_SinNoiseOffset", sinNoiseOffset);
        material.SetFloat("_ScanLineSpeed", scanLineSpeed);
        material.SetFloat("_ScanLineTail", scanLineTail);
        material.SetVector("_Offset", offset);
        material.SetFloat("_InternalTime", Time.unscaledTime);
        material.SetFloat("_FadeParam", fadeParam);
        material.SetFloat("_ColorBias", colorBias);

        Graphics.Blit(source, destination, material);
    }

    public IEnumerator Fade(float value, float time = 1.0f)
    {
        float t = 0f;
        float initValue = FadeParam;
        while (t < time)
        {
            t += Time.deltaTime;
            FadeParam = Mathf.Lerp(initValue, value, t / time);

            yield return 0;
        }
        FadeParam = value;
    }
}