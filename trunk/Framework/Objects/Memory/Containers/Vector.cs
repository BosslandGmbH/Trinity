//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Trinity.Framework.Objects.Memory.Misc;
//using Zeta.Game;

//namespace Trinity.Framework.Objects.Memory.Containers
//{
//    public class Vector : Vector<MemoryWrapper> { }

//    public class Vector<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
//    {
//        public const int SizeOf = 12;
//        public IEnumerable<T> Items => ReadObjects<T>(ReadOffset<IntPtr>(0x00), Capacity, Size);
//        public int Size => ReadOffset<int>(0x04);
//        public int Capacity => ReadOffset<int>(0x08);
//        public T this[int index] => Create<T>(BaseAddress + index * Size);
//        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//    }
//}


using System;
using System.Collections.Generic;
using Trinity.Framework.Objects.Memory.Misc;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class Vector : Vector<MemoryWrapper> { }

    public class Vector<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
    {
        public const int SizeOf = 0x38; // = 56

        public Ptr<T> x00_Data => ReadObject<Ptr<T>>(0x00);
        public int x04 => ReadOffset<int>(0x04);
        public int x08_Size => ReadOffset<int>(0x08);
        public int x0C_Capacity => ReadOffset<int>(0x0C);
        public BasicAllocator x10_Allocator => ReadObject<BasicAllocator>(0x10);
        public int _x2C => ReadOffset<int>(0x2C);
        public int x30 => ReadOffset<int>(0x30);
        public int _x34 => ReadOffset<int>(0x34);

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= x08_Size)
                    throw new ArgumentOutOfRangeException();

                return x00_Data[index];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var ptr = x00_Data;
            for (int i = 0; i < x08_Size; i++)
            {
                yield return ptr[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return base.ToString() + " Size = " + x08_Size + ", Allocator = " + x10_Allocator.ToString() + ", @Data = 0x" + ReadOffset<int>(0x00).ToString("X8");
        }
    }
    public class BasicAllocator : BasicAllocator<MemoryWrapper> { }

    public class BasicAllocator<T> : MemoryWrapper where T : MemoryWrapper, new()
    {
        // 2.0.0.20874
        public const int SizeOf = 0x1C;

        //public Ptr<T> x00_Allocation => Read<Ptr<T>>(0x00);
        public int x04 => ReadOffset<int>(0x04);
        public int x08_Size => ReadOffset<int>(0x08);
        public int x0C_Flags => ReadOffset<int>(0x0C);
// 1 => can/should free, 2 => call free()/realloc() instead of using Blizzard classes.
        //public Ptr x10_MemoryVTable { get { return Dereference<Ptr>(0x10); } }
        public int _x14 => ReadOffset<int>(0x14);
        public int x18_GoodFood => ReadOffset<int>(0x18);

        public override string ToString()
        {
            string validity = x18_GoodFood == 0x600DF00D ? "Valid" : ((uint)x18_GoodFood == 0xFEEDFACE ? "Invalid" : "Corrupt");
            return base.ToString() + " Size: " + x08_Size + ", State: " + validity;
        }
    }
}