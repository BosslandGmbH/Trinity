using System;
using Trinity.Framework.Helpers;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class Ptr : MemoryWrapper
    {
        public const int SizeOf = 4;
    }

    public class Ptr<T> : Ptr where T : MemoryWrapper, new()
    {
        public const int SizeOf = 4;

        private T _dereference => Dereference();

        public virtual T Dereference()
        {
            return Create<T>(BaseAddress);
        }

        public virtual T this[int index]
        {
            get
            {
                if (!IsValid) return default(T);
                int itemSize = TypeUtil<T>.SizeOf;
                return ReadObject<T>(0x00 + index * itemSize);
            }
        }

        public virtual T[] ToArray(int count)
        {
            if (!IsValid) return default(T[]);
            return ReadObjects<T>(0x0, count).ToArray();
        }
    }

}