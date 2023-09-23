using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Core.Items
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class ItemTypeAttribute : Attribute
    {
        public ItemType parent;
        static Dictionary<ItemType, ItemType[]> values = new Dictionary<ItemType, ItemType[]>();
        public ItemTypeAttribute(ItemType parent)
        {
            this.parent = parent;
        }
        static ItemTypeAttribute()
        {
            IEnumerable<ItemType> Get(ItemType enumValue)
            {
                var p = enumValue.GetAttributeOfType<ItemTypeAttribute>()?.parent;
                if (p.HasValue && p.Value != ItemType.Item)
                {
                    foreach (var t in Get(p.Value))
                        yield return t;
                }
                else if (!p.HasValue)
                {
                    yield break;
                }
                else
                {
                    yield return p.Value;
                }
            }

            foreach (ItemType enumValue in Enum.GetValues(typeof(ItemType)))
            {
                values.Add(enumValue, Get(enumValue).ToArray());
            }
        }
        public static bool IsChildOf(ItemType item, ItemType parent)
        {
            return
                item == parent ||
                (values.TryGetValue(item, out var arr) && arr.Contains(parent));
        }
    }
}
