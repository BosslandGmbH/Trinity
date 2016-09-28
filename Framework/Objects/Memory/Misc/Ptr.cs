using System;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class Ptr : MemoryWrapper
    {
        public const int SizeOf = 4;

        public Ptr<TCast> Cast<TCast>() where TCast : MemoryWrapper, new()
        {
            return Create<Ptr<TCast>>(BaseAddress);
        }
    }

    public class Ptr<T> : Ptr where T : MemoryWrapper, new()
    {
        public const int SizeOf = 4;

        private T _dereference => Dereference();

        public virtual T Dereference()
        {
            return Create<T>(BaseAddress);
        }
    }

}