using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRendererUnit : MaskableGraphic
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
        /*
         * y
         * ↑
         *  →x
         *  
         *  twisted left:
         *                    p21l *
         *                        /  *p2
         *                       /     /
         *                      /     /
         *             int(v2) /     /
         *          v1 +------*     /
         *             *p0      *p1/  
         *   ↓ norm0   +--------+- *p11r(v4)
         *             v0      p10r(v3)
         *
         *  straight:
         *  
         *    v1     p10l p11l (v2)
         *    +----------+-----------
         *    *p0        *p1        *p2
         *    +----------+-----------
         *    v0     p10r p11r (v3)
         */
        vh.Clear();

        UIVertex vert = new UIVertex();
        vert.color = color;

        Vector2? vert0, vert1;
        vert0 = null;
        vert1 = null;

        int idx = 0;
        int vert0idx = 0;
        int vert1idx = 1;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 point0 = points[i];
            Vector2 point1 = points[i + 1];

            Vector2 vec0 = point1 - point0;
            Vector2 norm0 = new Vector2(vec0.y, -vec0.x).normalized;

            if (!vert0.HasValue)
            {
                vert0 = point0 + norm0 * width;
                vert0idx = 0;

                vert.position = vert0.Value;
                vh.AddVert(vert);

                idx++;
            }
            if (!vert1.HasValue)
            {
                vert1 = point0 - norm0 * width;
                vert1idx = 1;

                vert.position = vert1.Value;
                vh.AddVert(vert);

                idx++;
            }

            var point10r = point1 + norm0 * width;
            var point10l = point1 - norm0 * width;

            Vector2 vert2 = Vector2.zero, vert3 = Vector2.zero, vert4 = Vector2.zero;

            if (i + 2 < points.Count)
            {
                Vector2 point2 = points[i + 2];

                Vector2 vec1 = point2 - point1;
                Vector2 norm1 = new Vector2(vec1.y, -vec1.x).normalized;

                var point11r = point1 + norm1 * width;
                var point11l = point1 - norm1 * width;

                var point21r = point2 + norm1 * width;
                var point21l = point2 - norm1 * width;


                Vector2 intersection;
                if (CalLineSegmentsIntersection(vert0.Value, point10r, point11r, point21r, out intersection))
                {
                    // twisted right

                    vert3 = intersection;
                    vert2 = point10l;
                    vert4 = point11l;


                    vert.position = vert2;
                    vh.AddVert(vert);

                    vert.position = vert3;
                    vh.AddVert(vert);

                    vert.position = vert4;
                    vh.AddVert(vert);


                    vh.AddTriangle(vert0idx, vert1idx, idx);    // vert0, vert1, vert2
                    vh.AddTriangle(vert0idx, idx, idx + 1);     // vert0, vert2, vert3
                    vh.AddTriangle(idx, idx + 2, idx + 1);      // vert2, vert4, vert3

                    vert0 = vert3;
                    vert0idx = idx + 1;

                    vert1 = vert4;
                    vert1idx = idx + 2;

                    idx += 3;
                }
                else if (CalLineSegmentsIntersection(vert1.Value, point10l, point11l, point21l, out intersection))
                {
                    // twisted left

                    vert2 = intersection;
                    vert3 = point10r;
                    vert4 = point11r;


                    vert.position = vert2;
                    vh.AddVert(vert);

                    vert.position = vert3;
                    vh.AddVert(vert);

                    vert.position = vert4;
                    vh.AddVert(vert);


                    vh.AddTriangle(vert0idx, vert1idx, idx);    // vert0, vert1, vert2
                    vh.AddTriangle(vert0idx, idx, idx + 1);     // vert0, vert2, vert3
                    vh.AddTriangle(idx, idx + 2, idx + 1);      // vert2, vert4, vert3

                    vert0 = vert4;
                    vert0idx = idx + 2;

                    vert1 = vert2;
                    vert1idx = idx;

                    idx += 3;
                }
                else
                {
                    // straight

                    vert2 = point10l;
                    vert3 = point10r;


                    vert.position = vert2;
                    vh.AddVert(vert);

                    vert.position = vert3;
                    vh.AddVert(vert);


                    vh.AddTriangle(vert0idx, vert1idx, idx);    // vert0, vert1, vert2
                    vh.AddTriangle(vert0idx, idx, idx + 1);     // vert0, vert2, vert3

                    vert0 = vert3;
                    vert0idx = idx + 1;

                    vert1 = vert2;
                    vert1idx = idx;

                    idx += 2;

                }
            }
            else
            {
                vert.position = point10l;
                //vert.uv0 = new Vector2(1, 1);
                vh.AddVert(vert);

                vert.position = point10r;
                //vert.uv0 = new Vector2(0, 1);
                vh.AddVert(vert);


                vh.AddTriangle(vert0idx, vert1idx, idx);    // vert0, vert1, point10l
                vh.AddTriangle(vert0idx, idx, idx + 1);     // vert0, point10l, point10r

                idx += 2;
            }

            //// Pointを定義
            //Vector2? point0;
            //Vector2 point1 = points[i + 0];
            //Vector2 point2 = points[i + 1];
            //Vector2? point3;

            //if (i - 1 < 0) point0 = null;
            //else point0 = points[i - 1];

            //if (i + 2 > points.Count - 1) point3 = null;
            //else point3 = points[i + 2];


            //// 頂点座標を計算
            //var normal1 = CalcNormal(point0, point1, point2);
            //var normal2 = CalcNormal(point1, point2, point3);


            //vert.position = point1 - normal1 * width;
            //vert.uv0 = new Vector2(0, 0);
            //vh.AddVert(vert);


            //vert.position = point2 - normal2 * width;
            //vert.uv0 = new Vector2(1, 0);
            //vh.AddVert(vert);


            //vert.position = point2 + normal2 * width;
            //vert.uv0 = new Vector2(1, 1);
            //vh.AddVert(vert);


            //vert.position = point1 + normal1 * width;
            //vert.uv0 = new Vector2(0, 1);
            //vh.AddVert(vert);

            //vh.AddTriangle(i * 4 + 0, i * 4 + 1, i * 4 + 2);
            //vh.AddTriangle(i * 4 + 0, i * 4 + 2, i * 4 + 3);
        }
    }

    void Update()
    {
        SetVerticesDirty();
        SetMaterialDirty();
    }


    bool CalLineSegmentsIntersection(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p1.x - p0.x) * (p3.y - p2.y) - (p1.y - p0.y) * (p3.x - p2.x);
        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p2.x - p0.x) * (p3.y - p2.y) - (p2.y - p0.y) * (p3.x - p2.x)) / d;
        var v = ((p2.x - p0.x) * (p1.y - p0.y) - (p2.y - p0.y) * (p1.x - p0.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p0.x + u * (p1.x - p0.x);
        intersection.y = p0.y + u * (p1.y - p0.y);

        return true;
    }

}