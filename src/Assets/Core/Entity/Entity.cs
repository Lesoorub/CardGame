using Assets.Core.AI;
using Assets.Core.Battle;

using UnityEngine.Events;

namespace Assets.Core.Entity
{
    /// <summary>
    /// ������� ��������.
    /// </summary>
    public class Entity
    {
        private int currentHealth;
        private int shields = 0;

        /// <summary>
        /// ������ ���������.
        /// </summary>
        public EntityData Data { get; private set; }
        
        /// <summary>
        /// ������ ����.
        /// </summary>
        public CardContainer Cards { get; private set; }

        /// <summary>
        /// ��������� "��� ��" ���������.
        /// </summary>
        public bool isAlive => this.CurrentHealth > 0;

        /// <summary>
        /// ������� �������� ��������.
        /// </summary>
        public int CurrentHealth { get => this.currentHealth; set => this.SetHealth(value); }
        
        /// <summary>
        /// ���������� ����� ��������.
        /// </summary>
        public int Shields { get => this.shields; set => this.SetShields(value); }

        /// <summary>
        /// �� ��������.
        /// </summary>
        public AbstractAI ai;

        /// <summary>
        /// ���������� ����� ��������� ������ ����������
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
        /// ��������� ��������� �����.
        /// </summary>
        /// <param name="damage">�������� ����.</param>
        /// <param name="type">��� ��������� �����.</param>
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
        /// ��������� ������ �������� ��������.
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
        /// ��������� ������� ������ ����.
        /// ����������� ��������� ������� ����������.
        /// </summary>
        /// <param name="battleManager">�������� ���.</param>
        public virtual void OnStepBegins(BattleManager battleManager)
        {
            if (battleManager.WhoNowSteping != this) 
                return;
            this.Cards.NextStep();
            this.Shields = 0;
        }

        /// <summary>
        /// ��������� ������� ��������� ����.
        /// </summary>
        /// <param name="battleManager">�������� ���.</param>
        public virtual void OnStepEnded(BattleManager battleManager)
        {

        }
    }

    /// <summary>
    /// ��� �����.
    /// </summary>
    [System.Serializable]
    public enum AttackType
    {
        /// <summary>
        /// ����������� ��������� ��������� �����
        /// </summary>
        Normal,
        /// <summary>
        /// ������������� �����
        /// </summary>
        IgnoreShields
    }
}