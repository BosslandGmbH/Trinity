using System;
using System.Collections;
using System.Collections.Generic;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class Container<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
    {
        public int ItemSize => ReadOffset<int>(0x104);
        public int MaxIndex => ReadOffset<int>(0x108);
        public int Count => ReadOffset<int>(0x10C);
        public int PtrItems => ReadOffset<int>(0x11C);
        public int Allocation => ReadOffset<int>(0x120);

        public T this[short index]
        {
            get
            {
                var itemSize = ItemSize;
                var blockPointer = PtrItems;
                var blockOffset = itemSize * index;
                var itemAddress = blockPointer + blockOffset;
                return Create<T>(new IntPtr(itemAddress));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var maxIndex = (short)MaxIndex;
            if (maxIndex < 0)
                yield break;

            var itemSize = ItemSize;
            var blockPointer = PtrItems;
            if (blockPointer == 0)
                yield break;

            for (var i = 0; i <= maxIndex; i++)
            {
                var blockOffset = itemSize * i;
                var itemAddress = blockPointer + blockOffset;
                yield return Create<T>(new IntPtr(itemAddress));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
