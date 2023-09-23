using System.Collections.Generic;
using System.Linq;
using Assets.Core.Card;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Core.Entity
{
    /// <summary>
    /// Менеджер карт сущности. Хранит и управляет картами в стопке и в руке
    /// </summary>
    public class CardContainer
    {
        /// <summary>
        /// Рука. Тут находятся карты которые может в данный момент использовать сущность
        /// </summary>
        public ObservableList<CardInstance> Hand { get; private set; } = new ObservableList<CardInstance>();
        /// <summary>
        /// Стопка добора карт. Заполняется в начале боя, и именно из
        /// этой стопки добираются карты в руку в начале каждого хода.
        /// Если стопка закончится, то в нее переносятся карты, в случайном порядке, из стопки сброса.
        /// </summary>
        Stack<CardInstance> NotUsed = new Stack<CardInstance>();
        /// <summary>
        /// Стопка сброса. После использования карты, если оне не сжигается, то она попадает сюда.
        /// </summary>
        Stack<CardInstance> Used = new Stack<CardInstance>();
        /// <summary>
        /// Количество карт выдаваемых каждый ход
        /// </summary>
        public int CardsPerStep;
        /// <summary>
        /// Текущее количество энергии
        /// </summary>
        public int CurEnergy;
        /// <summary>
        /// Количество энергии до которого восполяется <see cref="CurEnergy"/> каждый ход
        /// </summary>
        public int MaxEnergy;

        public delegate void ChangedArgs(CardContainer cardManager);
        public event ChangedArgs OnChanged;
        public delegate void InvokeCardArgs(CardContainer cardManager, CardInstance card);
        public event InvokeCardArgs OnInvokeCard;

        /// <summary>
        /// Конструктор инициализирующий руку
        /// </summary>
        /// <param name="cards">Все доступные карты. Они будут помещены в стопку добора</param>
        /// <param name="cardsPerStep">Количество выдаваемых карт каждый ход</param>
        /// <param name="maxEnergy">Количество энергии до которого восполяется <see cref="CurEnergy"/> каждый ход</param>
        public CardContainer(IEnumerable<CardInstance> cards, int cardsPerStep = 4, int maxEnergy = 3)
        {
            foreach (var c in cards)
                this.NotUsed.Push(c);
            this.CardsPerStep = cardsPerStep;
            this.MaxEnergy = maxEnergy;
        }

        public IEnumerable<CardInstance> GetNotUsed() => this.NotUsed;
        public IEnumerable<CardInstance> GetUsed() => this.Used;

        /// <summary>
        /// Процесс осуществление шага. Переносит карты из руки в стопку сброса, берет <see cref="CardsPerStep"/> карт из стопки добора
        /// </summary>
        public void NextStep()
        {
            foreach (var c in this.Hand)
                this.Used.Push(c);
            this.Hand.Clear();
            this.CurEnergy = this.MaxEnergy;
            for (int k = 0; k < this.CardsPerStep; k++)
            {
                this.TakeCard(true);
            }
            OnChanged?.Invoke(this);
        }

        /// <summary>
        /// Взять одну карту из стопки добра. Может вызвать <see cref="TransferUsedToNotUsed(bool)"/> перенос карт из стопки сброса 
        /// </summary>
        /// <param name="isForce"></param>
        public void TakeCard(bool isForce = false)
        {
            if (this.NotUsed.Count == 0 && this.Used.Count > 0)
            {
                this.TransferUsedToNotUsed(true);
            }
            if (this.NotUsed.Count > 0)
            {
                this.Hand.Add(this.NotUsed.Pop());
                if (!isForce)
                    OnChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Процесс переноса карт из стопки сброса в стопку добора
        /// </summary>
        /// <param name="isForce"></param>
        void TransferUsedToNotUsed(bool isForce = false)
        {
            foreach (var c in
                this.Used.Select(x => new { item = x, rnd = Random.Range(0f, 1f) })
                .OrderBy(x => x.rnd)
                .Select(x => x.item))
                this.NotUsed.Push(c);
            this.Used.Clear();
            if (!isForce)
                OnChanged?.Invoke(this);
        }

        /// <summary>
        /// Разыграть карту. Карта обязательно должна быть в руке (в стопке <see cref="Hand"/>)
        /// </summary>
        /// <param name="card">Разыгрываемая карта</param>
        /// <param name="caster">Разыгрывающая карту сущность</param>
        /// <param name="targets">Список целей</param>
        public bool InvokeCard(MonoBehaviour behaviour, CardInstance card, Entity caster, IEnumerable<Entity> targets)
        {
            if (!this.Hand.Contains(card)) 
                return false;//Если карта не в руке
            if (this.CurEnergy - card.data.BaseEnergyCost < 0) 
                return false;//Если не хватает энергии

            behaviour.StartCoroutine(card.data.OnAcceptOnTarget(caster, targets));
            this.CurEnergy -= card.data.BaseEnergyCost;
            this.Hand.Remove(card);
            this.Used.Push(card);
            OnChanged?.Invoke(this);
            OnInvokeCard?.Invoke(this, card);
            return true;
        }
    }
}