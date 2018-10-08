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

            switch (seed.ActorType)
            {
                case ActorType.Item:
                    return new TrinityItem(seed);
                case ActorType.Player:
                    return new TrinityPlayer(seed);
            }

            return new TrinityActor(seed);
        }
    }
}
