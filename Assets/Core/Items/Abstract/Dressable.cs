using System.Collections.Generic;

using Assets.Core.Items.Buffs;

namespace Assets.Core.Items
{
    public abstract class Dressable : ItemData
    {
        public List<ItemBuff> Buffs;
    }
}
