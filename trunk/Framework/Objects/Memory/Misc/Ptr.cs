using System;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class Ptr
    {
        public Ptr(IntPtr ptr)
        {
            BaseAddress = ptr;
        }

        public IntPtr BaseAddress;

        public Ptr<TCast> Cast<TCast>() where TCast : new()
        {
            return new Ptr<TCast>(BaseAddress);
        }
    }

    public class Ptr<T> : Ptr where T : new()
    {
        public Ptr(IntPtr ptr) : base(ptr) { }

        private T _dereference => Dereference();

        public virtual T Dereference()
        {
            return MemoryWrapper.Create<T>(BaseAddress);
        }
    }

}