using Assets.Core.Items.Containers;
using UnityEngine;

namespace Assets.Core.Screens.Map.Inventory
{
    /// <summary>
    /// ������ ���������.
    /// </summary>
    public class InventoryModel : MonoBehaviour
    {
        [Disable]
        public SlotsContainer Equipments;
        [Disable]
        public ItemsContainer InventoryItems;
    }
}