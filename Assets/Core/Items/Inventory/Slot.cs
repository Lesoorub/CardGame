using System.Linq;
namespace Assets.Core.Items.Inventory
{
    [System.Serializable]
    public class Slot
    {
        public ItemType[] allowedTypes = new ItemType[] { ItemType.Item };
        public ItemStack item;

        public Slot(params ItemType[] allowedTypes)
        {
            this.allowedTypes = allowedTypes;
        }

        public bool CanPutItem(ItemStack item)
        {
            return this.item.isEmpty && allowedTypes.Any(x => ItemTypeAttribute.IsChildOf(item.Data.Type, x));
        }
        public bool SetItem(ItemStack item)
        {
            if (!CanPutItem(item))
                return false;
            this.item = item;
            return true;
        }
        public static implicit operator ItemStack(Slot slot) => slot.item;
    }
}
