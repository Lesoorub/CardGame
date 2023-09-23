using System.Collections.Generic;
using System.Linq;
using Assets.Core.Card;
using Assets.Core.Entity.Preview;
using Assets.Core.Items.Inventory;
using UnityEngine;

namespace Assets.Core.Entity
{
    /// <summary>
    /// <see cref="ScriptableObject"/> данных о сущности.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.EntitiesDir + nameof(EntityData), fileName = "new " + nameof(EntityData))]
    public class EntityData : ScriptableObject
    {
        /// <summary>
        /// Превью сущности.
        /// </summary>
        public EntityPreview preview;

        /// <summary>
        /// Максимально кол-во здоровья.
        /// </summary>
        public int MaximumHealth = 50;

        /// <summary>
        /// Базовое кол-во энергии.
        /// </summary>
        public int BaseMaxEnergy = 3;

        /// <summary>
        /// Экиперовка.
        /// </summary>
        public Equipments Equipments = new Equipments();

        /// <summary>
        /// Количество карт выдоваемых в начале шага.
        /// </summary>
        public int CountOfCardsTakenOnStep = 4;

        /// <summary>
        /// Постоянные карты, выдаются каждый ход.
        /// </summary>
        public List<BaseCard> PermamentCards = new List<BaseCard>();

        /// <summary>
        /// Получить перечень изначальных карт.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CardInstance> GetInitialCards()
        {
            return this.PermamentCards.Select(x => new CardInstance(x));
        }
    }
}