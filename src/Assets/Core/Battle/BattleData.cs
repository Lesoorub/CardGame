using System.Collections.Generic;
using Assets.Core.AI;
using Assets.Core.Entity;
using Assets.Core.Items;
using UnityEngine;

namespace Assets.Core.Battle
{
    /// <summary>
    /// <see cref="ScriptableObject"/> данных о бою.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.BattleDir + nameof(BattleData), fileName = "new " + nameof(BattleData))]
    public class BattleData : ScriptableObject
    {
        /// <summary>
        /// Превью изображение боя.
        /// </summary>
        public Sprite PreviewImage;

        /// <summary>
        /// Множетель цвета <see cref="PreviewImage"/>.
        /// </summary>
        public Color32 PreviewColor = Color.white;

        /// <summary>
        /// Название превью.
        /// </summary>
        public string PreviewName;

        /// <summary>
        /// Название заголовка.
        /// </summary>
        public string HeaderText;

        /// <summary>
        /// Текст в описании боя.
        /// </summary>
        [TextArea]
        public string DescriptionText;

        /// <summary>
        /// Список существ с которыми предстоит битва.
        /// </summary>
        public List<EnemyEntity> Entities = new List<EnemyEntity>();

        /// <summary>
        /// Список наград.
        /// </summary>
        public List<ItemStack> RewardItems = new List<ItemStack>();
    }

    /// <summary>
    /// Объект объединяющий данные существ и их используемый в последующем интелект.
    /// </summary>
    [System.Serializable]
    public class EnemyEntity
    {
        public EntityData entityData;
        public AvailableAIs difficult;
    }
}