using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class NodePreview : MonoBehaviour
{
    public Sprite Image;
    public Color32 ImageColor = new Color32(255, 255, 255, 255);
    [TextArea]
    public string Text;
    public List<Node> ConnectedWith;
    public Image img;
    public TMP_Text label;
    public Node node;
    private void OnValidate()
    {
        if (this.img != null)
        {
            this.img.enabled = this.Image != null;
            this.img.sprite = this.Image;
            this.img.color = this.ImageColor;
        }
        if (this.label != null)
        {
            this.label.text = this.Text;
            if (this.Text.Trim().Length > 0)
                this.name = this.Text;
        }
        if (this.node != null)
        {
            this.node.connectedWith = this.ConnectedWith.Where(x => x != null).ToList();
            foreach (var n in this.ConnectedWith)
            {
                if (n == null || n.TryGetComponent<NodePreview>(out var preview)) continue;
                if (!preview.ConnectedWith.Contains(this.node))
                    preview.ConnectedWith.Add(this.node);
            }
        }
    }
}
