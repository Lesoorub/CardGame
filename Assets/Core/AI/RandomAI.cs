using System.Collections.Generic;
using System.Linq;
using Assets.Core.Battle;
using Assets.Core.Card;
using UnityEngine;

namespace Assets.Core.AI
{
    /// <summary>
    /// Реализовывает логику ИИ реализующий случайный выбор карт.
    /// </summary>
    public class RandomAI : AbstractAI
    {
        public override IEnumerable<(CardInstance card, IEnumerable<Entity.Entity> targets)> CalculateStep(
            Entity.Entity entity,
            BattleManager battleManager,
            IEnumerable<Entity.Entity> friends,
            IEnumerable<Entity.Entity> entities)
        {
            while (entity.Cards.CurEnergy > 0 &&
                entity.Cards.Hand.Any(x => x.data.BaseEnergyCost <= entity.Cards.CurEnergy))
            {
                var availableCards = entity.Cards.Hand.Where(x => x.data.BaseEnergyCost <= entity.Cards.CurEnergy).ToArray();
                var card = availableCards[Random.Range(0, availableCards.Length)];
                Entity.Entity[] availableTargets = new Entity.Entity[0];
                switch (card.data.Target)
                {
                    case AcceptableTarget.None:
                        availableTargets = new Entity.Entity[0];
                        break;
                    case AcceptableTarget.Player:
                        availableTargets = new Entity.Entity[] { entity };
                        break;
                    case AcceptableTarget.Friend:
                        availableTargets = new Entity.Entity[] { friends.ElementAt(Random.Range(0, friends.Count())) };
                        break;
                    case AcceptableTarget.Enemy:
                        availableTargets = new Entity.Entity[] { entities.ElementAt(Random.Range(0, entities.Count())) };
                        break;
                    case AcceptableTarget.AllFriends:
                        availableTargets = friends.ToArray();
                        break;
                    case AcceptableTarget.AllEnemies:
                        availableTargets = entities.ToArray();
                        break;
                }
                yield return (card, availableTargets);
            }
            yield break;
        }
    }
}