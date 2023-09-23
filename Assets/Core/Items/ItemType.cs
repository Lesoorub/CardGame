using System;

namespace Assets.Core.Items
{
    [Serializable]
    public enum ItemType
    {
        Item = 0,

        Armor,
        Weapon,

        //Weapon
        [ItemType(Weapon)]
        LeftHand,
        [ItemType(Weapon)]
        RightHand,

        //Armor
        [ItemType(Armor)]
        Helmet,
        [ItemType(Armor)]
        Chestplate,
        [ItemType(Armor)]
        Boots,

        //Weapon -> RightHand
        [ItemType(RightHand)]
        Sword,
        [ItemType(RightHand)]
        Staff,
        [ItemType(RightHand)]
        Bow,

        //Weapon -> LeftHand
        [ItemType(LeftHand)]
        Shield,
        [ItemType(LeftHand)]
        Quiver,
    }
}
