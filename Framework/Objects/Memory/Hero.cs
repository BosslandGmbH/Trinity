using System;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public class Hero : MemoryWrapper
    {
        private static readonly Lazy<Hero> Instance = new Lazy<Hero>(() => Create<Hero>(ZetaDia.Service.Hero.BaseAddress));

        public static int PlayerTradeId => Instance.Value.ReadOffset<int>(0xA8); // Id used for Attribute ItemTradePlayerLow / list of who an item can be traded to.
    }

}

