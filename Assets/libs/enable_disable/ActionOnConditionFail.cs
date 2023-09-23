public enum ActionOnConditionFail
{
    /// <summary>
    /// If condition(s) are false, don't draw the field at all.
    /// Не отрисовывает переменную
    /// </summary>
    DontDraw,
    /// <summary>
    ///  If condition(s) are false, just set the field as disabled.
    ///  Переменная становиться неактивной
    /// </summary>
    JustDisable,
}