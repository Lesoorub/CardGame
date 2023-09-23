using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.Card
{
    /// <summary>
    /// Базовая карта.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.CardDir + nameof(BaseCard), fileName = "new " + nameof(BaseCard))]
    public abstract class BaseCard : ScriptableObject
    {
        public string Id => this.name;

        /// <summary>
        /// Отображаемое локализиуемое имя.
        /// </summary>
        public string LocalizeName;

        /// <summary>
        /// Допустимые цели.
        /// </summary>
        public AcceptableTarget Target;

        /// <summary>
        /// Базовая стоимость энергии.
        /// </summary>
        public int BaseEnergyCost;

        /// <summary>
        /// Картинка.
        /// </summary>
        public Sprite ImageSprite;

        /// <summary>
        /// Множитель цвета для <see cref="ImageSprite"/>.
        /// </summary>
        public Color ImageColor = Color.white;
        
        /// <summary>
        /// Описание.
        /// </summary>
        [TextArea] public string LocalizeDescription;

        /// <summary>
        /// Список заменяемых значений в <see cref="LocalizeDescription"/>.
        /// </summary>
        public Dictionary<string, string> DescriptionStrings { get; private set; } = new Dictionary<string, string>();

        private void OnEnable()
        {
            this.DescriptionStrings.Clear();
        }

        /// <summary>
        /// События происходящее при применении карты на перечень существ.
        /// </summary>
        /// <param name="Caster">Кастер карты.</param>
        /// <param name="Target">Перечень карт.</param>
        /// <returns>Процесс применения карты.</returns>
        public abstract IEnumerator OnAcceptOnTarget(Entity.Entity Caster, IEnumerable<Entity.Entity> Target);
    }
}