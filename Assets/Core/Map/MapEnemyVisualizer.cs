using Assets.Core.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Core.Screens.Map
{
    public class MapEnemyVisualizer : MonoBehaviour
    {
        public Image EnemyImage;
        public TMP_Text Label;

        public string LabelFormat = "{0}";
        [HideInInspector]
        public BattleData data;
        public void Show(BattleData data)
        {
            this.data = data;

            this.From(data);
        }

        void From(BattleData data)
        {
            if (data != null)
            {
                if (this.EnemyImage != null)
                {
                    this.EnemyImage.sprite = data.PreviewImage;
                    this.EnemyImage.color = data.PreviewColor;
                }
                if (this.Label != null)
                {
                    this.Label.text = string.Format(this.LabelFormat, data.PreviewName);
                }
            }
            if (this.EnemyImage != null)
                this.EnemyImage.enabled = data.PreviewImage != null;
        }
    }
}