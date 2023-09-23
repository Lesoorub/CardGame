using System.Collections.Generic;
using System.Linq;
using Assets.Core.Items;
using Assets.Core.Items.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Core.Screens.Map.Inventory
{
    /// <summary>
    /// Представление инвентаря.
    /// </summary>
    public class InventoryView : MonoBehaviour
    {
        public ItemContainerVisualizer Equipments;

        List<ItemStackVisualizer> ItemVisualizers = new List<ItemStackVisualizer>();
        public Transform SlotsParent;
        public GameObject SlotPrefab;

        public UnityEvent<ItemStack> OnInventoryItemClick = new UnityEvent<ItemStack>();
        public UnityEvent<ItemStack> OnEquipmentsItemClick = new UnityEvent<ItemStack>();

        private void OnEnable()
        {
            foreach (var visualizer in this.Equipments.visualizers)
            {
                visualizer.OnClick.AddListener(this.Equipments_OnItemClick);
            }
        }

        private void OnDisable()
        {
            foreach (var visualizer in this.Equipments.visualizers)
            {
                visualizer.OnClick.RemoveListener(this.Equipments_OnItemClick);
            }
        }

        public ItemStackVisualizer CreateOrUpdateSlot(ItemStack stack)
        {
            var visualizer = this.ItemVisualizers.Find(x =>
                !x.stack.isEmpty && x.stack == stack && x.stack.Data.Stackable);
            if (visualizer != null)
            {
                this.UpdateSlotCount(stack);
                return visualizer;
            }
            var obj = Instantiate(this.SlotPrefab, this.SlotsParent);
            if (obj.TryGetComponent<ItemStackVisualizer>(out visualizer))
            {
                visualizer.SetItem(stack);
                this.ItemVisualizers.Add(visualizer);
                visualizer.OnClick.AddListener(this.Inventory_OnItemClick);
                return visualizer;
            }
            return null;
        }

        public void UpdateSlotCount(ItemStack newStack)
        {
            if (!this.ItemVisualizers.Exists(x => x.stack == newStack)) return;
            var visualizer = this.ItemVisualizers.Find(x => x.stack == newStack);
            visualizer.SetItem(newStack);
        }

        public void DeleteSlot(ItemStack stack)
        {
            var visualizer = ItemVisualizers.FirstOrDefault(x => x.stack == stack);
            if (visualizer == null) return;
            visualizer.OnClick.RemoveListener(Inventory_OnItemClick);
            this.ItemVisualizers.Remove(visualizer);
            Destroy(visualizer.gameObject);
        }

        private void Inventory_OnItemClick(ItemStackVisualizer visualizer)
        {
            this.OnInventoryItemClick?.Invoke(visualizer.stack);
        }

        private void Equipments_OnItemClick(ItemStackVisualizer visualizer)
        {
            this.OnEquipmentsItemClick?.Invoke(visualizer.stack);
        }
    }
}