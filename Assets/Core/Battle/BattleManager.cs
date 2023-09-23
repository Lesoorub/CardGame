using System.Collections.Generic;
using System.Linq;
using Assets.Core.AI;
using Assets.Core.Card;
using Assets.Core.Entity;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Core.Battle
{
    /// <summary>
    /// Реализует логику битвы, хранит в себе состояние боя
    /// </summary>
    public partial class BattleManager : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Список дружественных существ.
        /// </summary>
        public List<Entity.Entity> friends = new List<Entity.Entity>();

        /// <summary>
        /// Список вражеских существ.
        /// </summary>
        public List<Entity.Entity> enemies = new List<Entity.Entity>();

        /// <summary>
        /// Данные игрока.
        /// </summary>
        public PlayerData PlayerData;

        /// <summary>
        /// Флаг, определяющий ходит ли сейчас дружественная сторона, в которой состоит игрок.
        /// </summary>
        public bool isFriendsSteping = true;

        /// <summary>
        /// "Круглый массив" сущностей, определяет последовательность ходов сущностей.
        /// </summary>
        public Circle<Entity.Entity> circle = new Circle<Entity.Entity>();

        /// <summary>
        /// Данные битвы по умолчанию.
        /// </summary>
        public BattleData DefaultBattleData;

        #endregion

        #region Static Fields

        public static BattleData DataOfNextBattle;

        /// <summary>
        /// Результат предидущего боя. Если true значит игрок победил, иначе false
        /// </summary>
        public static bool PreviusBattleIsWin = false;

        #endregion

        #region Properties

        /// <summary>
        /// Флаг определяющий ходит ли сейчас "вражеская" сторона.
        /// </summary>
        public bool isSecondSteping => !this.isFriendsSteping;

        /// <summary>
        /// Получает сущность которая ходит в данный момент.
        /// </summary>
        public Entity.Entity WhoNowSteping => this.circle.GetCurrent();

        /// <summary>
        /// Сущность игрока.
        /// </summary>
        public Entity.Entity Player { get; private set; }

        #endregion

        #region Events

        public UnityEvent<BattleManager, EntityArgs> OnEntityAdded = new UnityEvent<BattleManager, EntityArgs>();
        public UnityEvent<BattleManager, EntityArgs> OnEntityRemoved = new UnityEvent<BattleManager, EntityArgs>();
        public UnityEvent<BattleManager> OnStepBegins = new UnityEvent<BattleManager>();
        public UnityEvent<BattleManager> OnStepEnded = new UnityEvent<BattleManager>();
        /// <summary>
        /// Вызывается перед первым ходом в битвы
        /// </summary>
        /// <param name="battleManager">Менеджер битвы</param>
        public UnityEvent<BattleManager> OnBattleBegins = new UnityEvent<BattleManager>();
        public UnityEvent<BattleManager, bool> OnBattleEnded = new UnityEvent<BattleManager, bool>();

        #endregion

        #region Unity Events

        private void Start()
        {
            if (DataOfNextBattle == null)
            {
                DataOfNextBattle = this.DefaultBattleData;
            }

            this.StartBattle();
        }

        private void OnDestroy()
        {
            this.EndBattle();
        }

        #endregion

        #region Event handlers


        #endregion

        #region Methods

        #region Private Methods

        /// <summary>
        /// Начинает бой.
        /// </summary>
        void StartBattle()
        {
            this.Player = new Entity.Entity(this.PlayerData);
            this.friends.Add(this.Player);
            this.OnStepBegins.AddListener(this.Player.OnStepBegins);
            this.OnStepEnded.AddListener(this.Player.OnStepEnded);
            this.OnEntityAdded?.Invoke(this, new EntityArgs(this.Player, true));

            foreach (var opponent_data in DataOfNextBattle.Entities)
            {
                var ent_data = opponent_data.entityData;
                var entity_instance = new Entity.Entity(ent_data);

                switch (opponent_data.difficult)
                {
                    case AvailableAIs.random:
                        entity_instance.ai = new RandomAI();
                        break;
                }

                this.enemies.Add(entity_instance);

                this.OnStepBegins.AddListener(entity_instance.OnStepBegins);
                this.OnStepEnded.AddListener(entity_instance.OnStepEnded);
                this.OnEntityAdded?.Invoke(this, new EntityArgs(entity_instance, false));
            }
            if (this.isFriendsSteping)
                this.circle.Add(this.friends.Concat(this.enemies));
            else
                this.circle.Add(this.enemies.Concat(this.friends));
            this.OnBattleBegins?.Invoke(this);
            this.OnStepBegins?.Invoke(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Вызывает конец бытвы.
        /// </summary>
        public void EndBattle()
        {
            if (DataOfNextBattle == null)
                return;
            PreviusBattleIsWin = this.friends.Count != 0;
            this.OnBattleEnded?.Invoke(this, PreviusBattleIsWin);
            DataOfNextBattle = null;
        }

        /// <summary>
        /// Обработка собятия нажатия конца хода игроком.
        /// Данные метод связан с событием в редакторе.
        /// </summary>
        public void _EndOfStep()
        {
            this.OnStepEnded?.Invoke(this);
            var who = circle.RotateOnce();
            while (this.circle.Count > 0 && !who.isAlive)
            {
                this.circle.Remove(who);
                who = this.circle.RotateOnce();
            }
            this.isFriendsSteping = this.friends.Contains(who);
            this.OnStepBegins?.Invoke(this);
        }

        public bool CanInvokeCard(CardInstance card, Entity.Entity caster, IEnumerable<Entity.Entity> targets)
        {
            if (this.circle.GetCurrent() != caster) //Запрет каста карт тем кто не ходит
                return false;
            if (caster.Cards.CurEnergy < card.data.BaseEnergyCost)
                return false;
            return true;
        }

        /// <summary>
        /// Выполнение (Каст) карты.
        /// </summary>
        /// <param name="card">Экзепляр карты.</param>
        /// <param name="caster">Кастующая карту сущность.</param>
        /// <param name="targets">Последовательность целей каста.
        /// Может быть как враг или друг, так и любая их комбиная 
        /// включая пустое множество.</param>
        /// <returns></returns>
        public bool InvokeCard(CardInstance card, Entity.Entity caster, IEnumerable<Entity.Entity> targets)
        {
            if (!this.CanInvokeCard(card, caster, targets))
                return false;
            if (!caster.Cards.InvokeCard(this, card, caster, targets))
                return false;
            var dies = this.friends.Concat(this.enemies).Where(x => !x.isAlive).ToArray();
            foreach (var entity in dies)
            {
                if (this.friends.Contains(entity))
                {
                    this.friends.Remove(entity);
                    this.OnEntityRemoved?.Invoke(this, new EntityArgs(entity, true));
                }
                if (this.enemies.Contains(entity))
                {
                    this.enemies.Remove(entity);
                    this.OnEntityRemoved?.Invoke(this, new EntityArgs(entity, false));
                }
            }
            if (dies.Length > 0)//Если кто-то умер, проверим закончился ли бой.
            {
                if (this.friends.Count == 0 || this.enemies.Count == 0)
                {
                    this.EndBattle();
                }
            }
            return true;
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// Аргументы сущностей.
    /// </summary>
    public struct EntityArgs
    {
        public Entity.Entity entity;
        public bool isFriend;

        public EntityArgs(Entity.Entity entity, bool isFriend)
        {
            this.entity = entity;
            this.isFriend = isFriend;
        }
    }
}