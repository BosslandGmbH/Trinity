using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class Vector : Vector<MemoryWrapper> { }

    public class Vector<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
    {
        public const int SizeOf = 12;
        public IEnumerable<T> Items => ReadObjects<T>(ReadOffset<IntPtr>(0x00), Capacity, Size);
        public int Size => ReadOffset<int>(0x04);
        public int Capacity => ReadOffset<int>(0x08);
        public T this[int index] => Create<T>(BaseAddress + index * Size);
        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}


