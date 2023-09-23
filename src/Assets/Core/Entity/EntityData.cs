using System.Collections.Generic;
using System.Linq;
using Assets.Core.Card;
using Assets.Core.Entity.Preview;
using Assets.Core.Items.Inventory;
using UnityEngine;

namespace Assets.Core.Entity
{
    /// <summary>
    /// <see cref="ScriptableObject"/> ������ � ��������.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.EntitiesDir + nameof(EntityData), fileName = "new " + nameof(EntityData))]
    public class EntityData : ScriptableObject
    {
        /// <summary>
        /// ������ ��������.
        /// </summary>
        public EntityPreview preview;

        /// <summary>
        /// ����������� ���-�� ��������.
        /// </summary>
        public int MaximumHealth = 50;

        /// <summary>
        /// ������� ���-�� �������.
        /// </summary>
        public int BaseMaxEnergy = 3;

        /// <summary>
        /// ����������.
        /// </summary>
        public Equipments Equipments = new Equipments();

        /// <summary>
        /// ���������� ���� ���������� � ������ ����.
        /// </summary>
        public int CountOfCardsTakenOnStep = 4;

        /// <summary>
        /// ���������� �����, �������� ������ ���.
        /// </summary>
        public List<BaseCard> PermamentCards = new List<BaseCard>();

        /// <summary>
        /// �������� �������� ����������� ����.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CardInstance> GetInitialCards()
        {
            return this.PermamentCards.Select(x => new CardInstance(x));
        }
    }
}