using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class PtrTable<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
    {
        public IntPtr DataPtr => ReadOffset<IntPtr>(0x00);
        public int BlockSize => ReadOffset<int>(0x08);
        public IntPtr[] RowPtrs => ZetaDia.Memory.ReadArray<IntPtr>(DataPtr, BlockSize);
        public IntPtr GetPtrByIndex(int index) => DataPtr + index * 4;
        public IntPtr GetEntryAddrByIndex(int index) => Read<IntPtr>(GetPtrByIndex(index));
        public T this[int index] => ReadAbsoluteObject<T>(GetEntryAddrByIndex(index));
        public List<T> Items => RowPtrs.Where(ptr => ptr != IntPtr.Zero).Select(ReadAbsoluteObject<T>).ToList();
        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}