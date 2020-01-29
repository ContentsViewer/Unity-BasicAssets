using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode()]
public class FringeEffect : MonoBehaviour
{


    // Shift amount for lateral CA
    [SerializeField, Range(0, 1)]
    float lateralShift = 0.3f;

    public float LateralShift
    {
        get { return lateralShift; }
        set { lateralShift = value; }
    }

    // Axial CA strength
    [SerializeField, Range(0, 1)]
    float axialStrength = 0.8f;

    public float AxialStrength
    {
        get { return axialStrength; }
        set { axialStrength = value; }
    }

    // Shift amount for axial CA
    [SerializeField, Range(0, 1)]
    float axialShift = 0.3f;

    public float AxialShift
    {
        get { return axialShift; }
        set { axialShift = value; }
    }

    // Quality level for axial CA
    public enum QualityLevel { Low, High }

    [SerializeField]
    QualityLevel axialQuality = QualityLevel.Low;

    public QualityLevel AxialQuality
    {
        get { return axialQuality; }
        set { axialQuality = value; }
    }
    [SerializeField]
    Shader shader;

    Material material;

    void SetupObjects()
    {
        if (material != null) return;

        material = new Material(shader);

    }

    void Reset()
    {
        shader = Shader.Find("PostEffects/Fringe");
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

        var cam = GetComponent<Camera>();
        var aspect = new Vector4(cam.aspect, 1.0f / cam.aspect, 1, 0);

        material.SetVector("_CameraAspect", aspect);
        material.SetFloat("_LateralShift", lateralShift);
        material.SetFloat("_AxialStrength", axialStrength);
        material.SetFloat("_AxialShift", axialShift);

        if (axialStrength == 0)
        {
            material.DisableKeyword("AXIAL_SAMPLE_LOW");
            material.DisableKeyword("AXIAL_SAMPLE_HIGH");
        }
        else if (axialQuality == QualityLevel.Low)
        {
            material.EnableKeyword("AXIAL_SAMPLE_LOW");
            material.DisableKeyword("AXIAL_SAMPLE_HIGH");
        }
        else
        {
            material.DisableKeyword("AXIAL_SAMPLE_LOW");
            material.EnableKeyword("AXIAL_SAMPLE_HIGH");
        }

        Graphics.Blit(source, destination, material);
    }
}
