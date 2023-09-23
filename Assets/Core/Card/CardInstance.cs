namespace Assets.Core.Card
{
    /// <summary>
    /// Экземпляр карты.
    /// </summary>
    public class CardInstance
    {
        /// <summary>
        /// Информация о карте.
        /// </summary>
        public BaseCard data;

        /// <summary>
        /// Визуализацтор карты.
        /// </summary>
        public CardVisualizer visualizer;

        /// <summary>
        /// Создает экземпляр карты на основе базовой карты. Не связывает с визуализатором.
        /// </summary>
        /// <param name="card">Данные карты.</param>
        public CardInstance(BaseCard card)
        {
            this.data = card;
        }
    }
}