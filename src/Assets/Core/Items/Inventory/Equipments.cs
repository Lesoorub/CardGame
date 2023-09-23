namespace Assets.Core.Items.Inventory
{
    [System.Serializable]
    public class Equipments
    {
        public Slot LeftHand = new Slot(ItemType.LeftHand);
        public Slot RightHand = new Slot(ItemType.RightHand);
        public Slot Helmet = new Slot(ItemType.Helmet);
        public Slot Chestplate = new Slot(ItemType.Chestplate);
        public Slot Boots = new Slot(ItemType.Boots);
    }
}
