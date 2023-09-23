using UnityEngine;

namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(Righthand), menuName = Paths.ItemsDir + nameof(Righthand))]
    public class Righthand : Dressable
    {
        public override ItemType Type => ItemType.RightHand;
    }
}
