namespace Assets.Core.Card
{
    /// <summary>
    /// Возможные цели.
    /// </summary>
    [System.Serializable]
    [System.Flags]
    public enum AcceptableTarget
    {
        /// <summary>
        /// Нет целей.
        /// </summary>
        None = 0,
        /// <summary>
        /// Целью может являться только игрок.
        /// </summary>
        Player = 0b00000001,
        /// <summary>
        /// Целью может являться только дружественное существо.
        /// </summary>
        Friend = 0b00000010,
        /// <summary>
        /// Целью может являться только вражеское существо.
        /// </summary>
        Enemy = 0b00000100,
        /// <summary>
        /// Целями являются только все дружественные существа.
        /// </summary>
        AllFriends = 0b00001000,
        /// <summary>
        /// Целями являются только все вражеские существа.
        /// </summary>
        AllEnemies = 0b00010000,
    }
}