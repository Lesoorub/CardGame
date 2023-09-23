using UnityEngine;

namespace Assets.Core.Items.Buffs
{
    public abstract class ItemBuff : ScriptableObject
    {
        public abstract void ApplyToEntity(Entity.Entity entity);
    }
}
