using Assets.Core.Items;
using UnityEngine.Events;

/// <summary>
/// Интерфейс взаимодействия с контейнером
/// </summary>
public interface IItemsContainer
{
    /// <summary>
    /// Ивент срабатывающий при любом изменении контейнера
    /// </summary>
    UnityEvent<IItemsContainer> OnChanged { get; }

    /// <summary>
    /// Список предметов
    /// </summary>
    ItemStack[] Items { get; set; }

    /// <summary>
    /// Можно ли добавить данный предмет в инвентарь
    /// </summary>
    /// <param name="item">Дабавляемый предмет</param>
    /// <returns>Вернет true если данный предмет можно добавить, иначе false</returns>
    bool CanPutItem(ItemStack item);

    /// <summary>
    /// Добавить предмет в инвентарь
    /// </summary>
    /// <param name="item">Дабавляемый предмет</param>
    /// <returns>Вернет true если данный предмет был добавлен, иначе false</returns>
    bool PutItem(ItemStack item);

    /// <summary>
    /// Проверяет существует ли стак с таким же или большим количеством предметов в инвенторе
    /// </summary>
    /// <param name="item">Предмет и его количество</param>
    /// <returns>Вернет true если такой стак существует, иначе false</returns>
    bool HasStack(ItemStack stack);

    /// <summary>
    /// Удаляет предмет из инвентаря
    /// </summary>
    /// <param name="item">Какой предмет и сколько</param>
    /// <param name="force">Если предмета не хватает, то все равно удалить его</param>
    /// <returns>Возвращает удаленные предметы</returns>
    ItemStack? RemoveItem(ItemStack item, bool force = false);

    /// <summary>
    /// Сбрасывает состояние интерфейса до начальных значений, очищая список предметов
    /// </summary>
    void Clear();
}