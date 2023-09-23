
using UnityEngine;

namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(ItemData), menuName = Paths.ItemsDir + nameof(ItemData))]
    public class ItemData : ScriptableObject
    {
        public virtual ItemType Type => ItemType.Item;
        public string ID => name;
        public Sprite ImageSprite;
        public Color ImageColor = Color.white;
        public bool Stackable = false;

        public string LocalizeName;
    }
}
