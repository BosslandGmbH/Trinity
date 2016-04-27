using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class LinkedList<T> : MemoryWrapper, IEnumerable where T : struct
    {
        public ListItem First => ReadObject<ListItem>(0x00);
        public ListItem Last => ReadObject<ListItem>(0x04);
        public int Count => ReadOffset<int>(0x08);

        public IEnumerator<T> GetEnumerator()
        {
            var node = First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class ListItem : MemoryWrapper
        {           
            private static int _sizeOfValue = TypeUtil<T>.SizeOf;

            public static int SizeOf = _sizeOfValue + 8;

            public T Value => ReadOffset<T>(0x00);
            public ListItem Previous => ReadObject<ListItem>(_sizeOfValue + 0x00);
            public ListItem Next => ReadObject<ListItem>(_sizeOfValue + 0x04);

            public override string ToString()
            {
                return Value.ToString();
            }
        }
    }

}

