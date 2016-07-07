using System;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public class Storage : MemoryWrapper
    {
        public Storage(IntPtr ptr) : base(ptr) { }

        public Act CurrentAct => ReadOffset<Act>(0x02C);

        public int CurrentGameLevel => ReadOffset<int>(0x30);

        public Difficulty CurrentDifficulty => (Difficulty)ReadOffset<int>(0x04)+1;
    }

}



