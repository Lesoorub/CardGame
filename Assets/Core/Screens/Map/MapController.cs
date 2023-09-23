using Assets.Core.Battle;
using Assets.Core.Screens.Map;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Core.Screens.Map.MapEnemies;

namespace Assets.Core.Map
{
    public class MapController : MonoBehaviour
    {
        public MapView view;
        public string BattleScene;

        static EnemyOnPath enemy;

        private void OnEnable()
        {
            view.MapEnemies?.OnBattleCollision?.AddListener(MapEnemies_OnBattleCollision);
        }
        private void OnDisable()
        {
            view.MapEnemies?.OnBattleCollision?.RemoveListener(MapEnemies_OnBattleCollision);
        }

        private void Start()
        {
            if (enemy.Data != null && enemy.Data == BattleManager.DataOfNextBattle)
            {
                if (BattleManager.PreviusBattleIsWin)
                {
                    view.MapEnemies.WinFightWithEnemy(enemy);
                    view.MapEnemies.ContinuePlayerMovement();
                }
                else
                {
                    view.MapEnemies.GoPlayerToPreviousNode();
                }
            }
        }

        void MapEnemies_OnBattleCollision(MapEnemies sender, EnemyOnPath enemy)
        {
            MapController.enemy = enemy;
            view.ShowBattleWindow(enemy);
        }

        public void _EnterBattle()
        {
            view.HideBattleWindow();
            BattleManager.DataOfNextBattle = enemy.Data;
            SceneManager.LoadScene(BattleScene);
        }

        public void _RejectBattle()
        {
            view.HideBattleWindow();
            view.MapEnemies.GoPlayerToPreviousNode();
        }
    }
}
