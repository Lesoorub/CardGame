using System.Collections.Generic;
using System.Linq;

namespace Assets.Core.Battle
{
    public partial class BattleManager
    {
        public class Circle<T>
        {
            Queue<T> q = new Queue<T>();
            public int Count => q.Count;
            public void Add(params T[] items)
            {
                Add(items as IEnumerable<T>);
            }
            public void Remove(params T[] items)
            {
                Remove(items as IEnumerable<T>);
            }
            public void Add(IEnumerable<T> items)
            {
                foreach (var l in items)
                    q.Enqueue(l);
            }
            public void Remove(IEnumerable<T> items)
            {
                var arr = q.ToList();
                foreach (var item in items)
                    arr.Remove(item);
                q.Clear();
                Add(arr.ToArray());
            }
            public T GetCurrent()
            {
                return q.Peek();
            }
            public T RotateOnce()
            {
                var t = q.Dequeue();
                q.Enqueue(t);
                return t;
            }
            public void RotateTo(T item)
            {
                T i = GetCurrent();
                while (!Equals(i, item))
                {
                    i = RotateOnce();
                }
            }
        }
    }
}