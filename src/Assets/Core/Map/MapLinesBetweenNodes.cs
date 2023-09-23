using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class MapLinesBetweenNodes : MonoBehaviour
{
    [SerializeField]
    private UITransformBasedLineRenderer lineRenderer;
    [SerializeField]
    private Transform NodesParent;
    public Dictionary<Node, List<Node>> ways = new Dictionary<Node, List<Node>>();
    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorSceneManager.sceneSaved += EditorSceneManager_sceneSaved;
#endif

        ways.Clear();
        foreach (Transform t in NodesParent)
        {
            foreach (var node in t.GetComponentsInChildren<Node>(true))
            {
                if (ways.TryGetValue(node, out var list))
                    list.AddRange(node.connectedWith);
                else
                    ways.Add(node, node.connectedWith);

                foreach (var other_node in node.connectedWith)
                {
                    if (!other_node.connectedWith.Contains(node))
                    {
                        if (ways.TryGetValue(other_node, out var other_list))
                            other_list.Add(node);
                        else
                            ways.Add(other_node, new List<Node>() { node });
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorSceneManager.sceneSaved -= this.EditorSceneManager_sceneSaved;
#endif
    }

    private void EditorSceneManager_sceneSaved(Scene scene)
    {
        this.UpdateLines();
    }

    public void UpdateLines()
    {
        this.lineRenderer.points.Clear();
        foreach (var (node, neighbors) in this.ways)
        {
            if (!node.gameObject.activeSelf)
                continue;
            foreach (var other_node in neighbors)
            {
                this.lineRenderer.points.Add(node.transform);
                this.lineRenderer.points.Add(other_node.transform);
            }
        }
        this.lineRenderer.SetAllDirty();
    }
}
