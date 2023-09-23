using System.Collections.Generic;
using Assets.Core.Items;
using Assets.Core.Items.Containers;
using Assets.Core.Player.Save;
using UnityEngine;

namespace Assets.Core.Screens.Map.Inventory
{
    /// <summary>
    /// ���������� ���������.
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        public InventoryModel model;
        public InventoryView view;
        [Space(10)]
        public List<ItemStack> DEBUG;

        private void OnEnable()
        {
            this.model.Equipments = SaveManager.Instance.Equipment;
            this.model.InventoryItems = SaveManager.Instance.Inventory;

            this.model.InventoryItems?.OnAdded?.AddListener(this.InventoryItems_OnAdded);
            this.model.InventoryItems?.OnRemoved?.AddListener(this.InventoryItems_OnRemoved);

            this.view.OnEquipmentsItemClick.AddListener(this.Equipments_OnClick);
            this.view.OnInventoryItemClick.AddListener(this.InventoryItems_OnClick);

            this.view.Equipments.Show(this.model.Equipments);
            foreach (var item in this.model.InventoryItems.Items)
                this.view.CreateOrUpdateSlot(item);
        }

        private void Start()
        {
            foreach (var item in this.DEBUG)
                this.model.InventoryItems.PutItem(item);
        }

        private void OnDisable()
        {
            foreach (var item in this.model.InventoryItems.Items)
                this.view.DeleteSlot(item);
            this.model.InventoryItems?.OnAdded?.RemoveListener(this.InventoryItems_OnAdded);
            this.model.InventoryItems?.OnRemoved?.RemoveListener(this.InventoryItems_OnRemoved);

            this.view.OnEquipmentsItemClick.RemoveListener(this.Equipments_OnClick);
            this.view.OnInventoryItemClick.RemoveListener(this.InventoryItems_OnClick);
        }

        /// <summary>
        /// ���������� ���������� ��������� � ���������
        /// </summary>
        /// <param name="container">��������� � ���������</param>
        /// <param name="newStack">����� ���� � ����������</param>
        private void InventoryItems_OnAdded(IItemsContainer container, ItemStack newStack)
        {
            this.view.CreateOrUpdateSlot(newStack);
        }

        /// <summary>
        /// ���������� �������� �������� �� ���������
        /// </summary>
        /// <param name="container">�� ������ ����������</param>
        /// <param name="item">����� ������� � ������� ������</param>
        private void InventoryItems_OnRemoved(IItemsContainer container, ItemStack remove, ItemStack newStack)
        {
            if (newStack.Count == 0)
            {
                this.view.DeleteSlot(newStack);
            }
            else
            {
                this.view.UpdateSlotCount(newStack);
            }
        }

        private void InventoryItems_OnClick(ItemStack stack)
        {
            if (stack.isEmpty) return;
            if (Input.GetKey(KeyCode.Delete))
            {
                this.model.InventoryItems.RemoveItem(stack);
                return;
            }
            this.model.InventoryItems.Transition(this.model.Equipments, stack);
        }

        private void Equipments_OnClick(ItemStack stack)
        {
            if (stack.isEmpty) return;
            if (Input.GetKey(KeyCode.Delete))
            {
                this.model.Equipments.RemoveItem(stack);
                return;
            }
            this.model.Equipments.Transition(this.model.InventoryItems, stack);
        }
    }
}