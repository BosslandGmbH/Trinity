using System;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class Map<T> : MemoryWrapper where T : MemoryWrapper, ITableItem, new()
    {
        public int Mask => ReadOffset<int>(0x00);

        public int Count => ReadOffset<int>(0x04);

        public Table<T> Data => new Table<T>(BaseAddress + 0x10);

        public Map() { } 

        public Map(IntPtr ptr)
        {
            Update(ptr);
        }
    }
}
