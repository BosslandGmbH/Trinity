using System;
using System.Collections;
using System.Collections.Generic;

namespace Trinity.Framework.Objects.Memory
{
    public class ContainerHeader : MemoryWrapper
    {
        public const int SizeOf = 0x7C; // 124
        public IntPtr PrevContainer => ReadOffset<IntPtr>(0x00);
        public bool IsStart => PrevContainer == IntPtr.Zero;
        public IntPtr NextContainer => ReadOffset<IntPtr>(0x04);
        public bool IsEnd => NextContainer == BaseAddress;
        public IntPtr Data => ReadOffset<IntPtr>(0x08);
    }

    public class Container<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
    {
        public string Name => ReadString(0x00);
        public ContainerHeader Header => ReadPointer<ContainerHeader>(PtrItems - 0x08);
        public int ItemSize => ReadOffset<int>(0x104);
        public int MaxIndex => ReadOffset<int>(0x108);
        public int Count => ReadOffset<int>(0x10C);
        public IntPtr PtrItems => ReadOffset<IntPtr>(0x11C);
        public int Allocation => ReadOffset<int>(0x120);

        public T this[short index]
        {
            get
            {
                var itemSize = ItemSize;
                var blockPointer = PtrItems;
                var blockOffset = itemSize * index;
                var itemAddress = blockPointer + blockOffset;
                return Create<T>(itemAddress);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var maxIndex = (short)MaxIndex;
            if (maxIndex < 0)
                yield break;

            var itemSize = ItemSize;
            var blockPointer = PtrItems;
            if (blockPointer == IntPtr.Zero)
                yield break;

            for (var i = 0; i <= maxIndex; i++)
            {
                var blockOffset = itemSize * i;
                var itemAddress = blockPointer + blockOffset;
                yield return Create<T>(itemAddress);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}