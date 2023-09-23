using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.Card
{
    /// <summary>
    /// ������� �����.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.CardDir + nameof(BaseCard), fileName = "new " + nameof(BaseCard))]
    public abstract class BaseCard : ScriptableObject
    {
        public string Id => this.name;

        /// <summary>
        /// ������������ ������������� ���.
        /// </summary>
        public string LocalizeName;

        /// <summary>
        /// ���������� ����.
        /// </summary>
        public AcceptableTarget Target;

        /// <summary>
        /// ������� ��������� �������.
        /// </summary>
        public int BaseEnergyCost;

        /// <summary>
        /// ��������.
        /// </summary>
        public Sprite ImageSprite;

        /// <summary>
        /// ��������� ����� ��� <see cref="ImageSprite"/>.
        /// </summary>
        public Color ImageColor = Color.white;
        
        /// <summary>
        /// ��������.
        /// </summary>
        [TextArea] public string LocalizeDescription;

        /// <summary>
        /// ������ ���������� �������� � <see cref="LocalizeDescription"/>.
        /// </summary>
        public Dictionary<string, string> DescriptionStrings { get; private set; } = new Dictionary<string, string>();

        private void OnEnable()
        {
            this.DescriptionStrings.Clear();
        }

        /// <summary>
        /// ������� ������������ ��� ���������� ����� �� �������� �������.
        /// </summary>
        /// <param name="Caster">������ �����.</param>
        /// <param name="Target">�������� ����.</param>
        /// <returns>������� ���������� �����.</returns>
        public abstract IEnumerator OnAcceptOnTarget(Entity.Entity Caster, IEnumerable<Entity.Entity> Target);
    }
}