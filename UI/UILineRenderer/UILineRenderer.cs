using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MaskableGraphic
{
    //public new Material material;
    //public new Color32 color = new Color32(255, 255, 255, 255);

    [Range(0.001f, 100)]
    public float width = 10;
    public List<Vector2> points = new List<Vector2>();
    

    protected override void Awake()
    {
        
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        UIVertex vert = new UIVertex();
        vert.color = color;
        
        for (int i = 0; i < points.Count - 1; i++)
        {
            // Pointを定義
            Vector2? point0;
            Vector2 point1 = points[i + 0];
            Vector2 point2 = points[i + 1];
            Vector2? point3;

            if (i - 1 < 0) point0 = null;
            else point0 = points[i - 1];

            if (i + 2 > points.Count - 1) point3 = null;
            else point3 = points[i + 2];

            // 頂点座標を計算
            var normal1 = CalcNormal(point0, point1, point2);
            var normal2 = CalcNormal(point1, point2, point3);
            

            vert.position = point1 - normal1 * width;
            vert.uv0 = new Vector2(0, 0);
            vh.AddVert(vert);


            vert.position = point2 - normal2 * width;
            vert.uv0 = new Vector2(1, 0);
            vh.AddVert(vert);


            vert.position = point2 + normal2 * width;
            vert.uv0 = new Vector2(1, 1);
            vh.AddVert(vert);


            vert.position = point1 + normal1 * width;
            vert.uv0 = new Vector2(0, 1);
            vh.AddVert(vert);

            vh.AddTriangle(i * 4 + 0, i * 4 + 1, i * 4 + 2);
            vh.AddTriangle(i * 4 + 0, i * 4 + 2, i * 4 + 3);
        }

        //Debug.Log("AA");
    }

    void Update()
    {
        SetVerticesDirty();
        SetMaterialDirty();
        //var vertices = new List<Vector3>();
        //var triangles = new List<int>();
        //var uvs = new List<Vector2>();

        //for (int i = 0; i < points.Count - 1; i++)
        //{
        //    // Pointを定義
        //    Vector2? point0;
        //    Vector2 point1 = points[i + 0];
        //    Vector2 point2 = points[i + 1];
        //    Vector2? point3;

        //    if (i - 1 < 0) point0 = null;
        //    else point0 = points[i - 1];

        //    if (i + 2 > points.Count - 1) point3 = null;
        //    else point3 = points[i + 2];

        //    // 頂点座標を計算
        //    var normal1 = CalcNormal(point0, point1, point2);
        //    var normal2 = CalcNormal(point1, point2, point3);

        //    var vertex = new Vector3();
        //    vertex = point1 - normal1 * width;
        //    //vertex.uv0 = new Vector2(0, 0);
        //    vertices.Add(vertex);

        //    vertex = point2 - normal2 * width;
        //    //vertex.uv0 = new Vector2(1, 0);
        //    vertices.Add(vertex);

        //    vertex = point2 + normal2 * width;
        //    //vertex.uv0 = new Vector2(1, 1);
        //    vertices.Add(vertex);

        //    vertex = point1 + normal1 * width;
        //    //vertex.uv0 = new Vector2(0, 1);
        //    vertices.Add(vertex);

        //    triangles.Add(i * 4 + 0);
        //    triangles.Add(i * 4 + 1);
        //    triangles.Add(i * 4 + 2);

        //    triangles.Add(i * 4 + 0);
        //    triangles.Add(i * 4 + 2);
        //    triangles.Add(i * 4 + 3);

        //    uvs.Add(new Vector2(0, 0));
        //    uvs.Add(new Vector2(1, 0));
        //    uvs.Add(new Vector2(1, 1));
        //    uvs.Add(new Vector2(0, 1));
        //}

        //// 頂点を設定

        //Mesh mesh = new Mesh();
        //mesh.SetVertices(vertices);
        //mesh.SetTriangles(triangles, 0);
        //mesh.SetUVs(0, uvs);
        //mesh.RecalculateNormals();
        //mesh.RecalculateTangents();
        //canvasRenderer.SetMesh(mesh);

        //material.color = color;
        //canvasRenderer.SetMaterial(material, Texture2D.whiteTexture);

    }

    // 線を太らせる方向を計算する
    Vector2 CalcNormal(Vector2? prev, Vector2 current, Vector2? next)
    {
        var dir = Vector2.zero;
        if (prev.HasValue) dir += prev.Value - current;
        if (next.HasValue) dir += current - next.Value;
        dir = new Vector2(-dir.y, dir.x).normalized;
        return dir;
    }


}