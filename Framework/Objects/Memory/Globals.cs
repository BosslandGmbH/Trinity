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
    public class Globals : MemoryWrapper
    {
        public Globals(IntPtr ptr) : base(ptr) { }
        public int WorldId => ReadOffset<int>(0x30);
        public int GameTime => ReadOffset<int>(0xC);
        public float RiftSouls => Math.Min(ReadOffset<float>(0xF4), 650);
        public float RiftProgressionPct => RiftSouls / 650 * 100;

    }





}




