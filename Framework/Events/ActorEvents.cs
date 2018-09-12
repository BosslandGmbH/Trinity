using System;
using Trinity.Framework.Actors.ActorTypes;

namespace Trinity.Framework.Events
{
    public class ActorEvents
    {
        public static event EventHandler<TrinityActor> OnActorFound;
        public static event EventHandler<TrinityActor> OnUnitKilled;

        public static void FireActorFound(TrinityActor actor)
        {
            OnActorFound?.Invoke(actor, actor);
        }

        public static void FireUnitKilled(TrinityActor actor)
        {
            OnUnitKilled?.Invoke(actor, actor);
        }
    }
}