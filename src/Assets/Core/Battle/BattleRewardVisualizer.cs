using Assets.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Core.Battle
{
    /// <summary>
    /// Визуализатор награды конкретного объекта.
    /// </summary>
    public class BattleRewardVisualizer : MonoBehaviour
    {
        public Image ItemImage;
        public TMP_Text ItemCount;
        public string ItemCountFormat = "{0}";
        public TMP_Text Label;
        public string LabelFormat = "{0}";

        /// <summary>
        /// Событие нажатия на награду.
        /// </summary>
        public UnityEvent<BattleRewardVisualizer> OnClick = new UnityEvent<BattleRewardVisualizer>();

        /// <summary>
        /// Связанный объект.
        /// </summary>
        public ItemStack item;

        /// <summary>
        /// Связывает данный объект с переданным предметом, предварительно инциализируя его отображение.
        /// </summary>
        /// <param name="item"></param>
        public void Show(ItemStack item)
        {
            this.item = item;
            this.From(item);
        }

        /// <summary>
        /// Связывает данный объект с переданным предметом
        /// </summary>
        /// <param name="item"></param>
        void From(ItemStack item)
        {
            if (this.Label != null)
                this.Label.text = string.Format(format: this.LabelFormat, item.Data.LocalizeName);
            if (this.ItemImage != null)
            {
                this.ItemImage.sprite = item.Data.ImageSprite;
                this.ItemImage.color = item.Data.ImageColor;
                this.ItemImage.enabled = this.ItemImage.sprite != null;
            }
            if (this.ItemCount != null)
            {
                this.ItemCount.text = string.Format(format: this.ItemCountFormat, item.Count);
                this.ItemCount.gameObject.SetActive(item.Count > 1);
            }
        }

        /// <summary>
        /// Событие обработчик нажатия на награду.
        /// </summary>
        public void _OnClick()
        {
            this.OnClick?.Invoke(this);
        }
    }
}