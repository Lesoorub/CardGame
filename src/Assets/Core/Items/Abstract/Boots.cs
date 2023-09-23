
using UnityEngine;
namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(Boots), menuName = Paths.ItemsDir + nameof(Boots))]
    public class Boots : Dressable
    {
        public override ItemType Type => ItemType.Boots;
    }
}
