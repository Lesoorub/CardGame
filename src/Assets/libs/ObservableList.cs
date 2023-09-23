using System.Collections.Generic;
using System.Linq;

namespace Assets.Core
{
    /// <summary>
    /// Надстройка над списком с инветами добавления, удаления и изменения.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ObservableList<T> : List<T>
    {

        #region Events

        /// <summary>
        /// Происходит при любом обновлении списка.
        /// </summary>
        public event ChangedArgs Changed;
        public delegate void ChangedArgs(IEnumerable<T> changes);

        /// <summary>
        /// Происходит при вызове функции <see cref="Add(T)"/> или <see cref="AddRange(IEnumerable{T})"/>.
        /// </summary>
        public event AddedArgs Added;
        public delegate void AddedArgs(IEnumerable<T> changes);

        /// <summary>
        /// Происходит при вызрове функции <see cref="Remove(T)"/> или <see cref="RemoveAt(int)"/>.
        /// </summary>
        public event RemovedArgs Removed;
        public delegate void RemovedArgs(IEnumerable<T> changes);

        #endregion

        #region Constructors

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public ObservableList() { }

        /// <summary>
        /// Инициализирует список на основе <see cref="IEnumerable{T}"/> элеметов.
        /// </summary>
        /// <param name="list"></param>
        public ObservableList(IEnumerable<T> list)
        {
            this.AddRange(list);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Очищает список.
        /// </summary>
        public new void Clear()
        {
            var l = this.ToList();
            base.Clear();
            Changed?.Invoke(l);
            Removed?.Invoke(l);
        }

        /// <summary>
        /// Добавляет новый элемент в список.
        /// </summary>
        /// <param name="item"></param>
        public new void Add(T item)
        {
            base.Add(item);
            Changed?.Invoke(changes: new List<T>() { item });
            Added?.Invoke(changes: new List<T>() { item });
        }

        /// <summary>
        /// Добавляет переданное перечисление в конец.
        /// </summary>
        /// <param name="list">Добавляемое перечисление.</param>
        public new void AddRange(IEnumerable<T> list)
        {
            base.AddRange(collection: list);
            Changed?.Invoke(changes: list);
            Added?.Invoke(changes: list);
        }

        /// <summary>
        /// Удаляет переданный элемент из текущего списка.
        /// </summary>
        /// <param name="item">Удаляемый элемент.</param>
        public new void Remove(T item)
        {
            base.Remove(item);
            Changed?.Invoke(changes: new List<T>() { item });
            Removed?.Invoke(changes: new List<T>() { item });
        }

        /// <summary>
        /// Удалить элемент из списка по индексу.
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            if (this.Count < index || index < 0)
                return;
            T item = this[index];
            base.RemoveAt(index);
            Changed?.Invoke(changes: new List<T>() { item });
            Removed?.Invoke(changes: new List<T>() { item });
        }

        #endregion
    }
}