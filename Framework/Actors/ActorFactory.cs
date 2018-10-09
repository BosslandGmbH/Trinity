using System;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors
{
    public static class ActorFactory
    {
        public static TrinityActor CreateActor(DiaObject seed)
        {
            if (seed == null)
                return null;

            if (seed.ActorType == ActorType.Player)
                return new TrinityPlayer(seed);
            else if (seed.ActorType == ActorType.Item)
                return new TrinityItem(seed);

            return new TrinityActor(seed);
        }

        internal static TrinityItem CreateItem(ACD acd)
        {
            return new TrinityItem(acd);
        }
    }
}
