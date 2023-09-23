using UnityEngine;
namespace Assets.Core.Items
{
    [CreateAssetMenu(fileName = "new " + nameof(Staff), menuName = Paths.ItemsDir + nameof(Staff))]
    public class Staff : Dressable
    {
        public override ItemType Type => ItemType.Staff;
    }
}
