using Assets.Core.Player.Save;
using UnityEngine;

namespace Assets.Core.Battle
{
    /// <summary>
    /// ”правл€ет наградами бо€.
    /// </summary>
    public class BattleRewarder : MonoBehaviour
    {
        public BattleManager Manager;
        public GameObject ItemRewardPrefab;
        public Transform RewardsParent;

        private void OnEnable()
        {
            this.Manager.OnBattleEnded?.AddListener(this.Manager_OnBattleEnded);
        }

        private void OnDisable()
        {
            this.Manager.OnBattleEnded?.RemoveListener(this.Manager_OnBattleEnded);
        }

        void Manager_OnBattleEnded(BattleManager manager, bool isPlayerWin)
        {
            if (!isPlayerWin) return;
            foreach (var item in BattleManager.DataOfNextBattle.RewardItems)
            {
                var visualizer = this.CreateReward();
                visualizer.Show(item);
            }
        }

        BattleRewardVisualizer CreateReward()
        {
            var visualizer = Instantiate(this.ItemRewardPrefab, this.RewardsParent)
                .GetComponent<BattleRewardVisualizer>();
            visualizer.OnClick.AddListener(this.visualizer_OnClick);
            return visualizer;
        }

        void DeleteReward(BattleRewardVisualizer visualizer)
        {
            visualizer.OnClick.RemoveListener(this.visualizer_OnClick);
            Destroy(visualizer.gameObject);
        }

        void visualizer_OnClick(BattleRewardVisualizer visualizer)
        {
            if (visualizer.item != default)
            {
                var result = SaveManager.Instance.Inventory.PutItem(visualizer.item);
                visualizer.item = default;
                this.DeleteReward(visualizer);
            }
        }
    }

}