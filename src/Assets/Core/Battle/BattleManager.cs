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
    /// ��������� ������ �����, ������ � ���� ��������� ���
    /// </summary>
    public partial class BattleManager : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// ������ ������������� �������.
        /// </summary>
        public List<Entity.Entity> friends = new List<Entity.Entity>();

        /// <summary>
        /// ������ ��������� �������.
        /// </summary>
        public List<Entity.Entity> enemies = new List<Entity.Entity>();

        /// <summary>
        /// ������ ������.
        /// </summary>
        public PlayerData PlayerData;

        /// <summary>
        /// ����, ������������ ����� �� ������ ������������� �������, � ������� ������� �����.
        /// </summary>
        public bool isFriendsSteping = true;

        /// <summary>
        /// "������� ������" ���������, ���������� ������������������ ����� ���������.
        /// </summary>
        public Circle<Entity.Entity> circle = new Circle<Entity.Entity>();

        /// <summary>
        /// ������ ����� �� ���������.
        /// </summary>
        public BattleData DefaultBattleData;

        #endregion

        #region Static Fields

        public static BattleData DataOfNextBattle;

        /// <summary>
        /// ��������� ����������� ���. ���� true ������ ����� �������, ����� false
        /// </summary>
        public static bool PreviusBattleIsWin = false;

        #endregion

        #region Properties

        /// <summary>
        /// ���� ������������ ����� �� ������ "���������" �������.
        /// </summary>
        public bool isSecondSteping => !this.isFriendsSteping;

        /// <summary>
        /// �������� �������� ������� ����� � ������ ������.
        /// </summary>
        public Entity.Entity WhoNowSteping => this.circle.GetCurrent();

        /// <summary>
        /// �������� ������.
        /// </summary>
        public Entity.Entity Player { get; private set; }

        #endregion

        #region Events

        public UnityEvent<BattleManager, EntityArgs> OnEntityAdded = new UnityEvent<BattleManager, EntityArgs>();
        public UnityEvent<BattleManager, EntityArgs> OnEntityRemoved = new UnityEvent<BattleManager, EntityArgs>();
        public UnityEvent<BattleManager> OnStepBegins = new UnityEvent<BattleManager>();
        public UnityEvent<BattleManager> OnStepEnded = new UnityEvent<BattleManager>();
        /// <summary>
        /// ���������� ����� ������ ����� � �����
        /// </summary>
        /// <param name="battleManager">�������� �����</param>
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
        /// �������� ���.
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
        /// �������� ����� �����.
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
        /// ��������� ������� ������� ����� ���� �������.
        /// ������ ����� ������ � �������� � ���������.
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
            if (this.circle.GetCurrent() != caster) //������ ����� ���� ��� ��� �� �����
                return false;
            if (caster.Cards.CurEnergy < card.data.BaseEnergyCost)
                return false;
            return true;
        }

        /// <summary>
        /// ���������� (����) �����.
        /// </summary>
        /// <param name="card">�������� �����.</param>
        /// <param name="caster">��������� ����� ��������.</param>
        /// <param name="targets">������������������ ����� �����.
        /// ����� ���� ��� ���� ��� ����, ��� � ����� �� �������� 
        /// ������� ������ ���������.</param>
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
            if (dies.Length > 0)//���� ���-�� ����, �������� ���������� �� ���.
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
    /// ��������� ���������.
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