using System.Collections.Generic;
using System.Threading;
using Trinity.Framework.Helpers;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoDictionary<T>
    {
        private readonly SortedList<int, T> _normalHashed = new SortedList<int, T>();
        private readonly SortedList<int, T> _itemHashed = new SortedList<int, T>();

        public void Add(string snoName, T value)
        {            
            _normalHashed[MemoryHelper.GameBalanceNormalHash(snoName)] = value;
            _itemHashed[MemoryHelper.GameBalanceItemHash(snoName)] = value;
        }

        public T this[int i]
        {
            get
            {
                T item;
                return TryGetValue(i, out item) ? item : default(T);
            }
        }

        public bool TryGetValue(int id, out T item)
        {
            if (_itemHashed.ContainsKey(id))
            {
                item = _itemHashed[id];
                return true;
            }
            if (_normalHashed.ContainsKey(id))
            {
                item = _normalHashed[id];
                return true;
            }
            item = default(T);
            return false;
        }

        public int Count => _normalHashed.Count;

        public void Clear()
        {
            _normalHashed.Clear();
            _itemHashed.Clear();
        }
    }
}