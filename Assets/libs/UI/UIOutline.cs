using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UIOutline : Graphic
{
    public float width = 5;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = rectTransform.rect.size / 2 * new Vector2(-1, 1);
        vh.AddVert(vertex);

        vertex.position = rectTransform.rect.size / 2 * new Vector2(1, 1);
        vh.AddVert(vertex);

        vertex.position = rectTransform.rect.size / 2 * new Vector2(1, -1);
        vh.AddVert(vertex);

        vertex.position = rectTransform.rect.size / 2 * new Vector2(-1, -1);
        vh.AddVert(vertex);


        vertex.position = rectTransform.rect.size / 2 * new Vector2(-1, 1) + new Vector2(-width, width);
        vh.AddVert(vertex);

        vertex.position = rectTransform.rect.size / 2 * new Vector2(1, 1) + new Vector2(width, width);
        vh.AddVert(vertex);

        vertex.position = rectTransform.rect.size / 2 * new Vector2(1, -1) + new Vector2(width, -width);
        vh.AddVert(vertex);

        vertex.position = rectTransform.rect.size / 2 * new Vector2(-1, -1) + new Vector2(-width, -width);
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 4);
        vh.AddTriangle(4, 5, 1);

        vh.AddTriangle(1, 2, 6);
        vh.AddTriangle(5, 6, 1);

        vh.AddTriangle(2, 3, 7);
        vh.AddTriangle(6, 7, 2);

        vh.AddTriangle(3, 0, 4);
        vh.AddTriangle(7, 4, 3);
    }
}
