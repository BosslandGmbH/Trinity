using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors
{
    public static class ActorExtensions
    {
        public static InventorySlot GetInventorySlot(this ACD acd)
        {
            return ZetaDia.Memory.Read<InventorySlot>(acd.BaseAddress + 0x114);
        }

    }
}
