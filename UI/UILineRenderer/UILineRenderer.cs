using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UILineRenderer : MaskableGraphic
{
    public static readonly int MaxPointCountPerRenderer = 16250;
    //public static readonly int MaxPointCountPerRenderer = 3;

    [Range(0.001f, 100)]
    public float width = 10;
    public List<Vector2> points = new List<Vector2>();

    List<UILineRendererUnit> rendererUnits = new List<UILineRendererUnit>();

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    void Update()
    {
        SetVerticesDirty();
        SetMaterialDirty();

        int neededRendererCount = (points.Count - 1) / (MaxPointCountPerRenderer - 1) + 1;

        for (int i = rendererUnits.Count - 1; i >= 0; i--)
        {
            if (!rendererUnits[i])
            {
                rendererUnits.RemoveAt(i);
                //Debug.Log("as");
            }
        }

        for (; rendererUnits.Count < neededRendererCount;)
        {
            var newGameObject = new GameObject();
            newGameObject.transform.SetParent(transform, false);
            newGameObject.name = "Unit";
            rendererUnits.Add(newGameObject.AddComponent<UILineRendererUnit>());
        }

        for (; rendererUnits.Count > neededRendererCount;)
        {
            DestroyImmediate(rendererUnits[rendererUnits.Count - 1].gameObject);
            rendererUnits.RemoveAt(rendererUnits.Count - 1);
        }

        //Debug.Log(uiLineRenderers.Count);

        int start = 0;
        int end = 0;

        // プロパティの同期
        for (int i = 0; i < rendererUnits.Count; i++)
        {
            var renderer = rendererUnits[i];
            renderer.rectTransform.anchorMin = Vector2.zero;
            renderer.rectTransform.anchorMax = Vector2.one;
            renderer.rectTransform.offsetMax = Vector2.zero;
            renderer.rectTransform.offsetMin = Vector2.zero;
            renderer.rectTransform.pivot = Vector2.zero;
            //Debug.Log(renderer.rectTransform.offsetMax);
            renderer.color = color;
            renderer.material = material;
            renderer.width = width;
            
            start = end;
            end = Mathf.Min(start + MaxPointCountPerRenderer - 1, points.Count - 1);
            SyncPointsRange(points, start, end - start + 1, renderer.points);
        }
    }

    void SyncPointsRange(List<Vector2> from, int start, int count, List<Vector2> target)
    {
        for (; target.Count < count;)
        {
            target.Add(new Vector2(0.0f, 0.0f));
        }

        if (target.Count > 0)
        {
            target.RemoveRange(count, target.Count - count);
        }

        for(int i = 0; i < count; i++)
        {
            target[i] = from[start + i];
        }
    }

}
