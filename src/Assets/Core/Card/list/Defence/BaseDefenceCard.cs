using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Core.Card
{
    /// <summary>
    /// Базовая защитная карта.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.CardsListDir + nameof(BaseDefenceCard), fileName = "new " + nameof(BaseDefenceCard))]
    public class BaseDefenceCard : BaseCard
    {
        public int Shields = 5;
        private void OnEnable()
        {
            this.DescriptionStrings.Add("Shields", this.Shields.ToString());
        }
        public override IEnumerator OnAcceptOnTarget(Entity.Entity Caster, IEnumerable<Entity.Entity> Target)
        {
            foreach (var t in Target)
            {
                t.Shields += this.Shields;
            }
            yield break;
        }
    }
}