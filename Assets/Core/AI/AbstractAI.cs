using System.Collections.Generic;

using Assets.Core.Battle;
using Assets.Core.Card;

namespace Assets.Core.AI
{
    /// <summary>
    /// Абстрактный класс ИИ противника.
    /// </summary>
    public abstract class AbstractAI
    {
        /// <summary>
        /// Расчитывает шаг на основе многих входных параметров.
        /// </summary>
        /// <param name="entity">Текущая сущность.</param>
        /// <param name="battleManager">Менеджер боя.</param>
        /// <param name="friends">Дружественные сущности.</param>
        /// <param name="entities">Вражеские сущности.</param>
        /// <returns></returns>
        public abstract IEnumerable<(CardInstance card, IEnumerable<Entity.Entity> targets)> CalculateStep(
            Entity.Entity entity,
            BattleManager battleManager,
            IEnumerable<Entity.Entity> friends,
            IEnumerable<Entity.Entity> entities);
    }
}