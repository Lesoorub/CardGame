using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Core.Card
{
    /// <summary>
    /// Визуализатор карты.
    /// </summary>
    public class CardVisualizer : MonoBehaviour
    {
        public TMP_Text Name;
        public TMP_Text Description;
        public TMP_Text EnergyCost;
        public Image CardImage;
        public CardInstance card { get; private set; }

        public Color32 NoEnergyColor = Color.red;
        public Color32 EnoghtEnergyColor = Color.white;

        /// <summary>
        /// Энегрии достаточно для каста.
        /// </summary>
        public bool HasEnergy { set => this.EnergyCost.color = value ? this.EnoghtEnergyColor : this.NoEnergyColor; }

        public UnityEvent<CardVisualizer> OnClickDown;
        public UnityEvent<CardVisualizer> OnClickUp;

        /// <summary>
        /// Связывает объект с данными и обновляет отображение.
        /// </summary>
        /// <param name="card"></param>
        public void Show(CardInstance card)
        {
            this.card = card;
            this.From(card.data);
        }

        /// <summary>
        /// Обновляет отоюражение в соответствии с картой.
        /// </summary>
        /// <param name="card">Карта.</param>
        void From(BaseCard card)
        {
            this.CardImage.sprite = card.ImageSprite;
            this.CardImage.color = card.ImageColor;
            this.CardImage.enabled = this.CardImage.sprite != null;
            this.Name.text = card.LocalizeName;
            var str = card.LocalizeDescription;
            foreach (var pair in card.DescriptionStrings)
                str = str.Replace($"{{{pair.Key}}}", pair.Value);
            this.Description.text = str;
            this.EnergyCost.text = card.BaseEnergyCost.ToString();
        }

        /// <summary>
        /// Событие нажатия карты.
        /// </summary>
        public void _ClickDown()
        {
            this.OnClickDown?.Invoke(this);
        }

        /// <summary>
        /// События отжатия карты.
        /// </summary>
        public void _ClickUp()
        {
            this.OnClickUp?.Invoke(this);
        }
    }

}