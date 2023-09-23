using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UITransformBasedLineRenderer : Graphic
{
    public float width = 1;
    public Color32 StartColor = new Color32(255, 255, 255, 255);
    public Color32 EndColor = new Color32(255, 255, 255, 255);
    public List<Transform> points = new List<Transform>();
    int index = 0;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        var vertex = UIVertex.simpleVert;

        index = 0;
        for (int k = 1; k < points.Count; k += 2)
        {
            var a = (Vector2)(points[k - 1].position - transform.position) / transform.lossyScale.z;
            var b = (Vector2)(points[k].position - transform.position) / transform.lossyScale.z;
            var p = Vector2.Perpendicular((b - a).normalized) * width;

            vertex.position = (a + p);
            vertex.color = StartColor * color;
            vh.AddVert(vertex);

            vertex.position = (a - p);
            vertex.color = StartColor * color;
            vh.AddVert(vertex);


            vertex.position = (b + p);
            vertex.color = EndColor * color;
            vh.AddVert(vertex);

            vertex.position = (b - p);
            vertex.color = EndColor * color;
            vh.AddVert(vertex);

            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
            index += 4;
        }
        for (int k = 1; k < points.Count; k += 2)
        {
            DrawCircle(vh, (points[k - 1].position - transform.position) / transform.lossyScale.z, width, StartColor * color);
            DrawCircle(vh, (points[k].position - transform.position) / transform.lossyScale.z, width, EndColor * color);
        }
    }

    void DrawCircle(VertexHelper vh, Vector2 position, float radius, Color32 color, int w = 32)
    {
        var vertex = UIVertex.simpleVert;
        int centerIndex = index;

        vertex.position = position;
        vertex.color = color;
        vh.AddVert(vertex);

        vertex.position = Vector2.up * radius + position;
        vertex.color = color;
        vh.AddVert(vertex);
        for (float k = 1; k < w; k++)
        {
            var v = k / w;
            vertex.position = new Vector2(
                Mathf.Sin(v * Mathf.PI * 2),
                Mathf.Cos(v * Mathf.PI * 2)) * radius + position;
            vertex.color = color;
            vh.AddVert(vertex);
            vh.AddTriangle(centerIndex, centerIndex + (int)k - 1, centerIndex + (int)k);
        }
        vh.AddTriangle(centerIndex, centerIndex + w - 1, centerIndex + 1);

        index += w + 1;
    }
}