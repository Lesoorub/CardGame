using System.Collections.Generic;
using System.Linq;
using Assets.Core.Battle;
using Assets.Core.Player.Save;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Core.Screens.Map
{

    public class MapEnemies : MonoBehaviour
    {
        public List<EnemyOnPath> enemies = new List<EnemyOnPath>();
        public GameObject EnemyPrefab;
        public Transform EnemiesParent;

        public MapPlayer Player;
        public UnityEvent<MapEnemies, EnemyOnPath> OnBattleCollision = new UnityEvent<MapEnemies, EnemyOnPath>();

        private void OnEnable()
        {
            this.Player.OnMovement.AddListener(this.MapPlayer_OnMovement);
            this.Player.OnNodeEnter.AddListener(this.MapPlayer_OnNodeEnter);
            this.CreateNotInstantedEnemies();
        }

        private void OnDisable()
        {
            this.Player.OnMovement.RemoveListener(this.MapPlayer_OnMovement);
            this.Player.OnNodeEnter.RemoveListener(this.MapPlayer_OnNodeEnter);
        }

        void CreateNotInstantedEnemies()
        {
            foreach (var enemy in this.enemies)
                if (enemy.NeedBattle)
                    this.CreateEnemy(enemy);
        }

        public void CreateEnemy(EnemyOnPath enemy)
        {
            if (!enemy.From.gameObject.activeSelf || enemy.To.gameObject.activeSelf) return;
            if (enemy.Instance != null) return;
            var position = this.EnemyPosition(enemy);
            enemy.Instance = Instantiate(this.EnemyPrefab, position, Quaternion.identity, this.EnemiesParent);
            if (enemy.Instance.TryGetComponent<MapEnemyVisualizer>(out var visualizer))
            {
                visualizer.Show(enemy.Data);
            }
        }

        public void WinFightWithEnemy(EnemyOnPath enemy)
        {
            enemy.NeedBattle = false;
            this.DestroyEnemy(enemy);
        }

        public void DestroyEnemy(EnemyOnPath enemy)
        {
            if (enemy.Instance != null)
                Destroy(enemy.Instance);
        }

        Vector2 EnemyPosition(EnemyOnPath enemy) => (enemy.From.transform.position + enemy.To.transform.position) / 2f;

        void MapPlayer_OnNodeEnter(MapPlayer player, Node node)
        {
            this.CreateNotInstantedEnemies();
        }

        void MapPlayer_OnMovement(MapPlayer player)
        {
            const float minCollisionDistance = 10;
            var nearestEnemy = this.enemies
                .Select(x =>
                new
                {
                    enemy = x,
                    distance = Vector2.Distance(player.transform.position, this.EnemyPosition(x))
                })
                .OrderBy(x => x.distance)
                .FirstOrDefault();

            if (nearestEnemy.distance <= minCollisionDistance)
            {
                SaveManager.Instance.Data.GoPath = player.StopPlayerMovement().Select(x => Map.GetNodeName(x)).ToList();
                this.OnBattleCollision?.Invoke(this, nearestEnemy.enemy);
            }
        }

        public void ContinuePlayerMovement()
        {
            if (SaveManager.Instance.Data.GoPath?.Count > 0)
                this.Player.GoToNode(SaveManager.Instance.Data.GoPath.Select(x => Map.GetNodeFromName(x)));
        }

        public void GoPlayerToPreviousNode()
        {
            this.Player.GoToPreviousNode();
        }

        [System.Serializable]
        public struct EnemyOnPath
        {
            public Node From;
            public Node To;
            public BattleData Data;
            public bool NeedBattle;
            [Disable]
            public GameObject Instance;

            public EnemyOnPath(Node from, Node to, BattleData data)
            {
                this.From = from;
                this.To = to;
                this.Data = data;
                this.Instance = null;
                this.NeedBattle = true;
            }
        }
    }

}