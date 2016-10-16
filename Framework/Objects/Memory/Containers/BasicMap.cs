using System;
using System.Collections.Generic;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class BasicMap<T> : MemoryWrapper where T : MemoryWrapper, ITableItem, new()
    {
        public int Mask => ReadOffset<int>(0x00);
        public int Count => ReadOffset<int>(0x04);
        public Table<T> Data => new Table<T>(BaseAddress + 0x10);
        public BasicMap() { } 

        public BasicMap(IntPtr ptr)
        {
            Update(ptr);
        }

        public bool TryGetItemByKey<TKey>(TKey modKey, out T value, Func<TKey, uint> hasher)
        {
            var hash = hasher(modKey);
            var bucketIndex = unchecked((int)(hash & Mask));
            var bucketEntry = Create<TableItem>(Data[bucketIndex]);
            while (bucketEntry != null)
            {
                if (bucketEntry.ModKey.Equals(modKey))
                {
                    value = Create<T>(bucketEntry.BaseAddress);
                    return true;
                }
                bucketEntry = Create<TableItem>(bucketEntry.Next);
            }
            value = default(T);
            return false;
        }
    }
}
