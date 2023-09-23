using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Core.Items.Containers
{
    public class ItemsContainer : MonoBehaviour, IItemsContainer
    {
        [SerializeField]
        private List<ItemStack> items = new List<ItemStack>();
        public ItemStack[] Items
        {
            get => items.ToArray();
            set => items = new List<ItemStack>(value);
        }
        [SerializeField]
        private UnityEvent<IItemsContainer> onChanged = new UnityEvent<IItemsContainer>();
        public UnityEvent<IItemsContainer> OnChanged => onChanged;
        public UnityEvent<IItemsContainer, ItemStack> OnAdded = new UnityEvent<IItemsContainer, ItemStack>();
        /// <summary>
        /// Событие удаления предмета
        /// IItemsContainer sender, ItemStack removedStack, ItemStack newStoredStack
        /// </summary>
        public UnityEvent<IItemsContainer, ItemStack, ItemStack> OnRemoved = new UnityEvent<IItemsContainer, ItemStack, ItemStack>();
        public int MaxSize = -1;
        private bool IsUnlimitedSize => MaxSize < 0;

        public bool CanPutItem(ItemStack item)
        {
            return IsUnlimitedSize || items.Count < MaxSize;
        }

        public bool PutItem(ItemStack item)
        {
            if (!CanPutItem(item)) return false;
            var stackIndex = items.FindIndex(x => x == item);
            if (stackIndex != -1)
            {
                var stack = items[stackIndex];
                if (stack.Data.Stackable)
                {
                    stack.Count += item.Count;
                    items[stackIndex] = stack;
                    OnChanged?.Invoke(this);
                    OnAdded?.Invoke(this, stack);
                    return true;
                }
            }
            items.Add(item);
            OnChanged?.Invoke(this);
            OnAdded?.Invoke(this, item);
            return true;
        }

        public ItemStack? RemoveItem(ItemStack item, bool force = false)
        {
            var index = items.FindIndex(x => x == item);
            if (index == -1) return null;
            var i = items[index];
            if (!force && i.Count < item.Count)
                return null;
            i.Count -= item.Count;
            if (i.Count <= 0)
            {
                items.RemoveAt(index);
                var stack = i;
                i.Count = 0;
                stack.Count += item.Count;
                OnChanged?.Invoke(this);
                OnRemoved?.Invoke(this, stack, i);
                return stack;
            }
            else
            {
                items[index] = i;
                OnChanged?.Invoke(this);
                OnRemoved?.Invoke(this, item, i);
                return item;
            }
        }

        public bool HasStack(ItemStack stack)
        {
            var item = items.FirstOrDefault(x => x == stack);
            return item != default && item.Count >= stack.Count;
        }
        public bool Transition(IItemsContainer anotherContrainer, ItemStack item)
        {
            if (!anotherContrainer.CanPutItem(item))
                return false;
            var i = RemoveItem(item);
            if (!i.HasValue) return false;
            anotherContrainer.PutItem(i.Value);
            return true;
        }

        public void Clear()
        {
            var oldItems = items.ToArray();
            items.Clear();
            foreach (var item in oldItems)
            {
                OnRemoved?.Invoke(this, item, default);
            }
            onChanged?.Invoke(this);
        }
    }
}