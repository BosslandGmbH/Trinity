using System;
using System.Collections.Generic;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class ExpandoContainer<T> : Container<T>, IEnumerable<T> where T : MemoryWrapper, new()   
    {
        public int Limit => ReadOffset<int>(0x15C);
        public int Bits => ReadOffset<int>(0x164);
        public int GoodFood => ReadOffset<int>(0x130 + 0x18);
        public bool IsDisposed => GoodFood != 1611526157; //unchecked((int)0xFEEDFACE);

        public new T this[short index]
        {
            get
            {
                var blockSize = 1 << Bits;
                var blockNumber = index/blockSize;
                var blockOffset = index%blockSize;
                var blockBase = Read<int>(new IntPtr(base.Allocation + 4*blockNumber));
                var itemPtr = new IntPtr(blockBase + blockOffset*ItemSize);
                var item = Create<T>(itemPtr);
                return item;
            }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            var maxIndex = (short) base.MaxIndex;
            if (maxIndex < 0)
                yield break;

            var itemSize = ItemSize;
            var blockSize = 1 << Bits;
            var blockCount = (maxIndex/blockSize) + 1;
            var blockPointers = ZetaDia.Memory.ReadArray<int>(new IntPtr(Allocation), blockCount);
            for (int i = 0; i <= maxIndex; i++)
            {
                var blockIndex = i/blockSize;
                var blockPointer = blockPointers[blockIndex];
                var blockOffset = itemSize*(i%blockSize);
                var itemAddress = new IntPtr(blockPointer + blockOffset);
                yield return Create<T>(itemAddress); 
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
