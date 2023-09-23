using System;

namespace Assets.Core.Items
{
    [Serializable]
    public struct ItemStack : IEquatable<ItemStack>
    {
        [Newtonsoft.Json.JsonIgnore]
        public ItemData Data;
        public string Data_ID
        {
            get
            {
                return Data?.ID;
            }
            set
            {
                Data = ItemDataList.Instance.GetByID(value);
            }
        }
        public int Count;
        [Newtonsoft.Json.JsonIgnore]
        public bool isEmpty => Count == 0 || Data == null;

        public ItemStack(ItemData data, int count)
        {
            Data = data;
            Count = count;
        }

        public bool Equals(ItemStack other)
        {
            return other != null && ItemData.Equals(other.Data, Data);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ItemStack stack))
                return false;
            return Equals(stack);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data, Count);
        }
        public static bool operator ==(ItemStack a, ItemStack b) => a != null && b != null && a.Equals(b);
        public static bool operator !=(ItemStack a, ItemStack b) => a == null || b == null || !a.Equals(b);

        public override string ToString()
        {
            return $"{{ Data_ID='{Data_ID}' Count={Count} }}";
        }
    }
}
