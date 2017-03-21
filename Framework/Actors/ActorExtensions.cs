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
        public static TrinityActor FirstBySnoId(this IEnumerable<TrinityActor> source, int actorSnoId, float maxDistance = 200f)
        {
            return source.FirstOrDefault(a => a.ActorSnoId == actorSnoId && a.Distance <= maxDistance);
        }

        public static TrinityActor FirstByType(this IEnumerable<TrinityActor> source, TrinityObjectType type, float maxDistance = 200f)
        {
            return source.FirstOrDefault(a => a.Type == type && a.Distance <= maxDistance);
        }

        public static TrinityActor ClosestActor(this IEnumerable<TrinityActor> source, int actorSnoId, float maxDistance = 200f)
        {
            return source.OrderBy(a => a.Distance).FirstOrDefault(a => a.IsUnit && a.ActorSnoId == actorSnoId && a.Distance <= maxDistance);
        }

        public static InventorySlot GetInventorySlot(this ACD acd)
        {
            return ZetaDia.Memory.Read<InventorySlot>(acd.BaseAddress + 0x114);
        }
    }
}
