using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Core.Items.UI
{
    public class ItemStackVisualizer : MonoBehaviour
    {
        public Image ItemImage;
        public Image AltImage;
        public TMP_Text ItemCount;
        public string ItemCountFormat = "{0}";

        public ItemStack stack;

        public UnityEvent<ItemStackVisualizer> OnClick;

        public void SetItem(ItemStack stack)
        {
            this.stack = stack;

            From(stack);
        }

        void From(ItemStack item)
        {
            if (item == default)
            {
                ItemImage.enabled = false;
                ItemCount.gameObject.SetActive(false);
                AltImage.enabled = true;
                return;
            }
            ItemImage.sprite = item.Data.ImageSprite;
            ItemImage.color = item.Data.ImageColor;
            ItemImage.enabled = ItemImage.sprite != null;
            if (AltImage != null)
                AltImage.enabled = !ItemImage.enabled;
            ItemCount.text = string.Format(ItemCountFormat, item.Count);
            ItemCount.gameObject.SetActive(item.Count != 1);
        }
        public void _OnClick()
        {
            OnClick?.Invoke(this);
        }
    }
}