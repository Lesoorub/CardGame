using System.Collections;
using System.Collections.Generic;

using Assets.Core.Entity;

using UnityEngine;

namespace Assets.Core.Card
{
    /// <summary>
    /// Базовая атакующая карта.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.CardsListDir + nameof(BaseAttackCard), fileName = "new " + nameof(BaseAttackCard))]
    public class BaseAttackCard : BaseCard
    {
        public AttackType AttackType = AttackType.Normal;
        public int Damage = 5;
        public int Times = 1;
        [Tooltip("Time in seconds")]
        public float TimeBetweenAttacks = 0.2f;
        private void OnEnable()
        {
            this.DescriptionStrings.Add("Damage", this.Damage.ToString());
            this.DescriptionStrings.Add("Times", this.Times.ToString());
        }
        public override IEnumerator OnAcceptOnTarget(Entity.Entity Caster, IEnumerable<Entity.Entity> Target)
        {
            foreach (var t in Target)
            {
                for (int k = 0; k < this.Times; k++)
                {
                    t.Attack(this.Damage, AttackType.Normal);
                    if (this.Times > 1)
                        yield return new WaitForSeconds(this.TimeBetweenAttacks);
                }
            }
            yield break;
        }
    }
}