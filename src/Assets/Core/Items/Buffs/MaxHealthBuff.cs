using UnityEngine;

namespace Assets.Core.Items.Buffs
{
    [CreateAssetMenu(fileName = "new " + nameof(MaxHealthBuff), menuName = Paths.ItemsBuffsDir + nameof(MaxHealthBuff))]
    public class MaxHealthBuff : ItemBuff
    {
        public int MaxHealth = 0;
        public override void ApplyToEntity(Entity.Entity entity)
        {
            entity.CurrentHealth += MaxHealth;
        }
    }
}
