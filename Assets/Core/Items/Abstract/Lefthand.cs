using UnityEngine;

namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(Lefthand), menuName = Paths.ItemsDir + nameof(Lefthand))]
    public class Lefthand : Dressable
    {
        public override ItemType Type => ItemType.LeftHand;
    }
}
