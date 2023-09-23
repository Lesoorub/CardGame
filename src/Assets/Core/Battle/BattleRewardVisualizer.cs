using Assets.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Core.Battle
{
    /// <summary>
    /// ������������ ������� ����������� �������.
    /// </summary>
    public class BattleRewardVisualizer : MonoBehaviour
    {
        public Image ItemImage;
        public TMP_Text ItemCount;
        public string ItemCountFormat = "{0}";
        public TMP_Text Label;
        public string LabelFormat = "{0}";

        /// <summary>
        /// ������� ������� �� �������.
        /// </summary>
        public UnityEvent<BattleRewardVisualizer> OnClick = new UnityEvent<BattleRewardVisualizer>();

        /// <summary>
        /// ��������� ������.
        /// </summary>
        public ItemStack item;

        /// <summary>
        /// ��������� ������ ������ � ���������� ���������, �������������� ������������ ��� �����������.
        /// </summary>
        /// <param name="item"></param>
        public void Show(ItemStack item)
        {
            this.item = item;
            this.From(item);
        }

        /// <summary>
        /// ��������� ������ ������ � ���������� ���������
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
        /// ������� ���������� ������� �� �������.
        /// </summary>
        public void _OnClick()
        {
            this.OnClick?.Invoke(this);
        }
    }
}