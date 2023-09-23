
using UnityEngine;
namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(Shield), menuName = Paths.ItemsDir + nameof(Shield))]
    public class Shield : Dressable
    {
        public override ItemType Type => ItemType.Shield;
    }
}
