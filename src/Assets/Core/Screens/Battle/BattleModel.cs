using Assets.Core.Card;
using UnityEngine;

namespace Assets.Core.Screens.Battle
{
    /// <summary>
    /// Хранит все данные о интерфейсе
    /// </summary>
    public class BattleModel : MonoBehaviour
    {
        public ObservableList<CardInstance> Cards = new ObservableList<CardInstance>();
        public ObservableList<Entity.Entity> PlayerEntities = new ObservableList<Entity.Entity>();
        public ObservableList<Entity.Entity> EnemyEntities = new ObservableList<Entity.Entity>();

        private int maxEnergy = 3;
        private int curEnergy = 3;
        public int MaxEnergy
        {
            get => this.maxEnergy;
            set
            {
                if (this.maxEnergy != value)
                {
                    this.maxEnergy = value;
                    this.OnEnergyChanged.Invoke(this.maxEnergy, this.curEnergy);
                }
            }
        }
        public int CurEnergy
        {
            get => this.curEnergy;
            set
            {
                if (this.curEnergy != value)
                {
                    this.curEnergy = value;
                    this.OnEnergyChanged.Invoke(this.maxEnergy, this.curEnergy);
                }
            }
        }
        public delegate void EnergyChangedArgs(int MaxEnergy, int CurEnergy);
        public event EnergyChangedArgs OnEnergyChanged;
    }
}