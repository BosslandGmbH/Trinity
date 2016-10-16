using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Misc;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class Allocator : Allocator<MemoryWrapper> { }

    public class Allocator<T> : MemoryWrapper where T : MemoryWrapper, new()
    {
        public const int SizeOf = 0x1C; // 2.0.0.20874
        public int x00_ElementSize => ReadOffset<int>(0x00);
        public int x04_Limit => ReadOffset<int>(0x04);
        public SinglyLinkedList<AllocatorBlock<T>> x08_Blocks => ReadObject<SinglyLinkedList<AllocatorBlock<T>>>(0x08);
        public int x10_Flags => ReadOffset<int>(0x10);
        public int x14_MemVT => ReadOffset<int>(0x14);
        public int x18_GoodFood => ReadOffset<int>(0x18);

        public List<T> ToList()
        {
            var list = new List<T>(x04_Limit);
            foreach (var block in x08_Blocks)
            {
                var elementsAddr = block.x00_PtrElements.BaseAddress;
                var isSlotFree = block.x24_FreeSpaceBitmap;
                for (int i = 0; i < block.x08_Limit && list.Count < block.x10_ElementCount; i++)
                {
                    if (isSlotFree[i])
                        continue;
                    var element = ReadAbsoluteObject<T>(elementsAddr + block.x0C_ElementSize * i);
                    list.Add(element);
                }
            }
            return list;
        }

        public override string ToString()
        {
            string state = x18_GoodFood == 0x600DF00D ? "Valid" :
                (uint)x18_GoodFood == 0xFEEDFACE ? "Disposed" : "Corrupt";
            return x04_Limit + "x" + x00_ElementSize + " bytes, Blocks: " + x08_Blocks.x00_Count + ", State: " + state;
        }
    }

    public class AllocatorBlock : AllocatorBlock<MemoryWrapper> { }

    public class AllocatorBlock<T> : MemoryWrapper where T : MemoryWrapper, new()
    {
        public const int SizeOf = 0x30; // 48

        public List<T> x00_Elements => ReadObjects<T>(0x00, x08_Limit);
        public Ptr<T> x00_PtrElements => ReadObject<Ptr<T>>(0x00);
        public T x04_NextFreeElement => ReadObject<T>(0x04);
        public int x08_Limit => ReadOffset<int>(0x08);
        public int x0C_ElementSize => ReadOffset<int>(0x0C);
        public int x10_ElementCount => ReadOffset<int>(0x10);
        public int x14 => ReadOffset<int>(0x14);
        public int x18 => ReadOffset<int>(0x18);
        public int x1C_FreeCount => ReadOffset<int>(0x1C);
        public int x20 => ReadOffset<int>(0x20);
        public BitArray x24_FreeSpaceBitmap => new BitArray(ReadArray<byte>(0x24, (x08_Limit + 7) / 8).ToArray());
        public int x28 => ReadOffset<int>(0x28);
        public int x2C_GoodFood => ReadOffset<int>(0x2C);
    }

    public class SinglyLinkedList<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
    {
        public const int SizeOf = 8;

        public int x00_Count => ReadOffset<int>(0x00);

        public Node x04_First => ReadPointer<Node>(0x04);

        //ReadAbsoluteObject<Node>(ReadOffset<IntPtr>(0x04))

        public IEnumerator<T> GetEnumerator()
        {
            var node = x04_First;
            while (node != null)
            {
                yield return node.x04_Element;
                node = node.x00_Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Node : MemoryWrapper
        {
            public static readonly int SizeOf = GetSizeOf();
            private static int GetSizeOf()
            {
                int sizeOf = 4;
                sizeOf += TypeUtil<T>.SizeOf;
                return sizeOf;
            }
            
            public Node x00_Next => ReadPointer<Node>(0x00);
            public T x04_Element => ReadObject<T>(0x04);
        }
    }
}
