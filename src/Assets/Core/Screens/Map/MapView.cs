using Assets.Core.Screens.Map;
using UnityEngine;
using static Assets.Core.Screens.Map.MapEnemies;

namespace Assets.Core.Map
{
    public class MapView : MonoBehaviour
    {
        public MapBattleWindowVisualizer BattleWindow;
        public MapEnemies MapEnemies;

        public void ShowBattleWindow(EnemyOnPath enemy)
        {
            if (BattleWindow != null)
            {
                BattleWindow.Show(enemy.Data);
                BattleWindow.gameObject.SetActive(true);
            }
        }

        public Battle.BattleData HideBattleWindow()
        {
            BattleWindow?.gameObject.SetActive(false);
            return BattleWindow.data;
        }
    }
}