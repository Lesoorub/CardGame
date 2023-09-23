using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    public float width = 1;
    public List<Vector2> points = new List<Vector2>();
    int index = 0;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        var vertex = UIVertex.simpleVert;
        vertex.color = color;

        index = 0;
        for (int k = 1; k < points.Count; k++)
        {
            var a = points[k - 1];
            var b = points[k];
            var p = Vector2.Perpendicular((b - a).normalized) * width;

            vertex.position = (a + p);
            vh.AddVert(vertex);

            vertex.position = (a - p);
            vh.AddVert(vertex);


            vertex.position = (b + p);
            vh.AddVert(vertex);

            vertex.position = (b - p);
            vh.AddVert(vertex);

            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
            index += 4;
        }
        foreach (var p in points)
            DrawCircle(vh, p, width);
    }

    void DrawCircle(VertexHelper vh, Vector2 position, float radius, int w = 32)
    {
        var vertex = UIVertex.simpleVert;
        vertex.color = color;

        int centerIndex = index;

        vertex.position = position;
        vh.AddVert(vertex);

        vertex.position = Vector2.up * radius + position;
        vh.AddVert(vertex);
        for (float k = 1; k < w; k++)
        {
            var v = k / w;
            vertex.position = new Vector2(
                Mathf.Sin(v * Mathf.PI * 2),
                Mathf.Cos(v * Mathf.PI * 2)) * radius + position;
            vh.AddVert(vertex);
            vh.AddTriangle(centerIndex, centerIndex + (int)k - 1, centerIndex + (int)k);
        }
        vh.AddTriangle(centerIndex, centerIndex + w - 1, centerIndex + 1);

        index += w + 1;
    }
}
