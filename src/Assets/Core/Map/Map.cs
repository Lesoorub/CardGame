using System.Collections.Generic;
using System.Linq;
using Assets.Core.Player.Save;
using DijkstraAlgortm;
using UnityEngine;

namespace Assets.Core.Screens.Map
{

    public class Map : MonoBehaviour
    {
        [SerializeField]
        private MapLinesBetweenNodes Lines;
        [SerializeField]
        private MapPlayer Player;
        [SerializeField]
        private Transform NodesParent;
        static List<Node> nodes = new List<Node>();
        Dictionary<Node, List<Node>> ways => this.Lines.ways;

        void OnEnable()
        {
            this.Player?.OnNodeEnter.AddListener(this.Player_OnNodeEnter);

            nodes.Clear();
            foreach (Transform t in this.NodesParent)
            {
                foreach (var node in t.GetComponentsInChildren<Node>(true))
                {
                    node.OnClick.AddListener(this.OnNodeClick);
                    nodes.Add(node);
                }
            }

            SaveManager.Instance.AfterLoad?.AddListener(this.SaveManager_AfterLoad);
            SaveManager.Instance.BeforeSave?.AddListener(this.SaveManager_BeforeSave);
        }

        void OnDisable()
        {
            Player?.OnNodeEnter.RemoveListener(this.Player_OnNodeEnter);
            foreach (var node in nodes)
                node.OnClick.RemoveListener(this.OnNodeClick);
            SaveManager.Instance.AfterLoad?.RemoveListener(this.SaveManager_AfterLoad);
            SaveManager.Instance.BeforeSave?.RemoveListener(this.SaveManager_BeforeSave);
        }

        void Start()
        {
            this.SaveManager_AfterLoad(SaveManager.Instance);
        }

        void SaveManager_AfterLoad(SaveManager manager)
        {
            Debug.Log($"SaveManager_AfterLoad");
            this.NodesParent.position = manager.Data.MapPosition;

            if (manager.Data.VisitedNodes.Count > 0)//Если есть посещенные узлы
            {
                //Активируем все посещенные узлы
                foreach (var node in nodes)
                {
                    var state = manager.Data.VisitedNodes.Contains(GetNodeName(node));
                    node.gameObject.SetActive(state);
                }
            }
            else//Если посещенных узлов нету
            {
                //Записываем все отображаемые узлы в посещенные
                manager.Data.VisitedNodes = nodes
                    .Where(x => x.gameObject.activeSelf)
                    .Select(x => GetNodeName(x))
                    .ToList();
            }
            //Ищем узел в котором находится игрок
            var curNode = nodes
                .FirstOrDefault(x => string.Equals(
                    GetNodeName(x),
                    manager.Data.LastNodeName));

            if (curNode != null)//Если узел найден, то телепортируем туда игрока
                this.Player.CurrentNode = curNode;

            this.Lines.UpdateLines();
        }
        void SaveManager_BeforeSave(SaveManager manager)
        {
            manager.Data.LastNodeName = GetNodeName(this.Player.CurrentNode);
            manager.Data.MapPosition = this.NodesParent.position;
            manager.Data.VisitedNodes = nodes
                .Where(x => x.gameObject.activeSelf)
                .Select(x => GetNodeName(x))
                .ToList();
        }

        void Player_OnNodeEnter(MapPlayer player, Node node)
        {
            bool needRedrawLines = false;
            foreach (var n in this.ways[node])
            {
                if (!n.gameObject.activeSelf)
                    needRedrawLines = true;
                n.gameObject.SetActive(true);
            }
            if (needRedrawLines)
            {
                this.Lines.UpdateLines();
            }
        }

        void OnNodeClick(Node node)
        {
            var from = this.Player.CurrentNode;
            var to = node;

            if (from == to)
            {

            }
            //Debug.Log($"from={from} to={to}");

            var algoritm = new Dijkstra(GetGraph());
            var (path, error) = algoritm.FindShortestPath(GetNodeName(from), GetNodeName(to));

            if (error)
            {
                Debug.Log("Path not found");
            }
            else
            {
                var pathNodes = path.Select(x => GetNodeFromName(x.Name));
                //Debug.Log($"Path=[{string.Join(", ", pathNodes)}]");
                this.Player.GoToNode(pathNodes);
            }
        }

        public static string GetNodeName(Node node) => nodes.IndexOf(node).ToString();

        public static Node GetNodeFromName(string name) => nodes[int.Parse(name)];

        DijkstraAlgortm.Graph GetGraph()
        {
            var g = new DijkstraAlgortm.Graph();


            foreach (var (node, connected) in ways)
            {
                g.AddVertex(GetNodeName(node));
            }

            foreach (var (node, connected) in ways)
            {
                foreach (var other_node in connected)
                    g.AddEdge(GetNodeName(node), GetNodeName(other_node), 1);
            }

            return g;
        }

    }
}