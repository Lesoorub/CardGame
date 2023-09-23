using System.Linq;
using Assets.Core.Items.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Core.Items.Containers
{
    public class SlotsContainer : MonoBehaviour, IItemsContainer
    {
        public Slot[] Slots = new Slot[0];
        public ItemStack[] Items
        {
            get => Slots.Select(x => x.item).ToArray();
            set
            {
                int index = 0;
                foreach (var item in value)
                {
                    Slots[index].item = item;

                    index++;
                    if (index == Slots.Length) break;
                }
            }
        }
        [SerializeField]
        private UnityEvent<IItemsContainer> onChanged = new UnityEvent<IItemsContainer>();
        public UnityEvent<IItemsContainer> OnChanged => onChanged;

        public bool CanPutItem(ItemStack item)
        {
            return Slots.Any(x => x.CanPutItem(item));
        }

        public bool HasStack(ItemStack stack)
        {
            return Slots.Any(x => x.item == stack && x.item.Count >= stack.Count);
        }

        public bool PutItem(ItemStack item)
        {
            if (!CanPutItem(item)) return false;
            var slot = Slots.FirstOrDefault(x => x.CanPutItem(item));
            slot.item = item;
            OnChanged?.Invoke(this);
            return true;
        }

        public ItemStack? RemoveItem(ItemStack item, bool force = false)
        {
            var slot = Slots.FirstOrDefault(x => x.item == item && x.item.Count >= item.Count);
            if (slot == default) return null;
            slot.item.Count -= item.Count;
            if (slot.item.Count <= 0)
            {
                var stack = new ItemStack(slot.item.Data, slot.item.Count + item.Count);
                slot.item = default;
                OnChanged?.Invoke(this);
                return stack;
            }
            OnChanged?.Invoke(this);
            return item;
        }

        public void Clear()
        {
            foreach (var slot in Slots)
                slot.item = default;
            onChanged?.Invoke(this);
        }
    }
}