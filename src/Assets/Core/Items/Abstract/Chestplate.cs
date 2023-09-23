
using UnityEngine;
namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(Chestplate), menuName = Paths.ItemsDir + nameof(Chestplate))]
    public class Chestplate : Dressable
    {
        public override ItemType Type => ItemType.Chestplate;
    }
}
