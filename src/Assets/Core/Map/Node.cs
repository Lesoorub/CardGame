using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public bool alwaysVisible = false;
    [Space]
    public UITransformBasedLineRenderer lineRenderer;
    public TMPro.TMP_Text nodeName;
    public List<Node> connectedWith = new List<Node>();
    public UnityEvent<Node> OnClick = new UnityEvent<Node>();

    private void Awake()
    {
        this.gameObject.SetActive(this.alwaysVisible);
    }
    private void OnValidate()
    {
        this.nodeName.text = name;
    }
    private void OnDisable()
    {
        foreach (var p in this.connectedWith)
            p.OnValidate();
    }
    private void OnEnable()
    {
        foreach (var p in this.connectedWith)
            p.OnValidate();
    }
    public void _OnOpenInvoke() => this.OnClick?.Invoke(this);
}
