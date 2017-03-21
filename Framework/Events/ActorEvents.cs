using Trinity.Framework.Actors.ActorTypes;

namespace Trinity.Framework.Events
{
    public class ActorEvents
    {
        public delegate void ActorEvent(TrinityActor actor);

        public static event ActorEvent OnActorFound;

        public static event ActorEvent OnUnitKilled;

        public static void FireActorFound(TrinityActor actor)
        {
            OnActorFound?.Invoke(actor);
        }

        public static void FireUnitKilled(TrinityActor actor)
        {
            OnUnitKilled?.Invoke(actor);
        }
    }
}