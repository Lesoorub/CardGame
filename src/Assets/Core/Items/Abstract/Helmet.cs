
using UnityEngine;
namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(Helmet), menuName = Paths.ItemsDir + nameof(Helmet))]
    public class Helmet : Dressable
    {
        public override ItemType Type => ItemType.Helmet;
    }
}
