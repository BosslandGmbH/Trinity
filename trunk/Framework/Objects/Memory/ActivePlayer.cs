using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects.Memory.Debug;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework.Objects.Memory
{
    public class ActivePlayer : MemoryWrapper
    {
        public ActivePlayer(IntPtr ptr) : base(ptr) { }

        public bool IsInGame => ReadOffset<int>(0x30) == 1;
    }




}




