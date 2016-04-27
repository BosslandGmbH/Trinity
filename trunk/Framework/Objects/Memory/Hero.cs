using System;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public class Hero : MemoryWrapper
    {
        public Hero(IntPtr ptr) : base(ptr) { }
        public int PlayerTradeId => ReadOffset<int>(0xA8); // Id used for Attribute ItemTradePlayerLow / list of who an item can be traded to.
    }

}

