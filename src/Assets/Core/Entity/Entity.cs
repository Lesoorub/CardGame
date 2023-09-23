using Assets.Core.AI;
using Assets.Core.Battle;

using UnityEngine.Events;

namespace Assets.Core.Entity
{
    /// <summary>
    /// Игровая сущность.
    /// </summary>
    public class Entity
    {
        private int currentHealth;
        private int shields = 0;

        /// <summary>
        /// Данные сущености.
        /// </summary>
        public EntityData Data { get; private set; }
        
        /// <summary>
        /// Колода карт.
        /// </summary>
        public CardContainer Cards { get; private set; }

        /// <summary>
        /// Состояние "жив ли" сущености.
        /// </summary>
        public bool isAlive => this.CurrentHealth > 0;

        /// <summary>
        /// Текущее здоровье сущности.
        /// </summary>
        public int CurrentHealth { get => this.currentHealth; set => this.SetHealth(value); }
        
        /// <summary>
        /// Количество щитов сущности.
        /// </summary>
        public int Shields { get => this.shields; set => this.SetShields(value); }

        /// <summary>
        /// ИИ сущности.
        /// </summary>
        public AbstractAI ai;

        /// <summary>
        /// Вызывается когда параметры энтити изменяются
        /// </summary>
        public UnityEvent<Entity> OnChanged = new UnityEvent<Entity>();

        public Entity(EntityData data)
        {
            this.Data = data;
            this.CurrentHealth = data.MaximumHealth;
            this.Cards = new CardContainer(
                cards: data.GetInitialCards(),
                cardsPerStep: data.CountOfCardsTakenOnStep,
                maxEnergy: data.BaseMaxEnergy);
        }


        /// <summary>
        /// Обработка получения урона.
        /// </summary>
        /// <param name="damage">Входящий урон.</param>
        /// <param name="type">Тип входящего урона.</param>
        public void Attack(int damage, AttackType type)
        {
            if (this.Shields > 0 && type != AttackType.IgnoreShields)
            {
                var s = this.shields;
                s -= damage;
                damage = 0;
                if (s < 0)
                {
                    damage = -s;
                    s = 0;
                }
                this.Shields = s;
            }
            this.CurrentHealth -= damage;
        }

        /// <summary>
        /// Установка нового значения здоровья.
        /// </summary>
        /// <param name="newHealth"></param>
        void SetHealth(int newHealth)
        {
            if (newHealth > this.Data.MaximumHealth)
                newHealth = this.Data.MaximumHealth;
            if (newHealth != this.currentHealth)
            {
                this.currentHealth = newHealth;
                this.OnChanged.Invoke(this);
            }
        }

        void SetShields(int newShields)
        {
            if (newShields != this.shields)
            {
                this.shields = newShields;
                this.OnChanged.Invoke(this);
            }
        }

        /// <summary>
        /// Обработка события начала шага.
        /// Обязательно вызывайте базовую реализацию.
        /// </summary>
        /// <param name="battleManager">Менеджер боя.</param>
        public virtual void OnStepBegins(BattleManager battleManager)
        {
            if (battleManager.WhoNowSteping != this) 
                return;
            this.Cards.NextStep();
            this.Shields = 0;
        }

        /// <summary>
        /// Обработка события окончания шага.
        /// </summary>
        /// <param name="battleManager">Менеджер боя.</param>
        public virtual void OnStepEnded(BattleManager battleManager)
        {

        }
    }

    /// <summary>
    /// Тип атаки.
    /// </summary>
    [System.Serializable]
    public enum AttackType
    {
        /// <summary>
        /// Стандратная процедура нанесения урона
        /// </summary>
        Normal,
        /// <summary>
        /// Игнорирование щитов
        /// </summary>
        IgnoreShields
    }
}