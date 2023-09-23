using Assets.Core.Battle;
using TMPro;
using UnityEngine;

namespace Assets.Core.Map
{
    public class MapBattleWindowVisualizer : MonoBehaviour
    {
        public TMP_Text HeaderLabel;
        public TMP_Text DescriptionLabel;

        public BattleData data;

        public void Show(BattleData data)
        {
            this.data = data;
            this.From(data);
        }

        void From(BattleData data)
        {
            this.HeaderLabel.text = data.HeaderText;
            this.DescriptionLabel.text = data.DescriptionText;
        }
    }

}