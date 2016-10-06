using Trinity.Framework.Actors.ActorTypes;

namespace Trinity.Framework.Events
{
    public class ActorEvents
    {
        public delegate void ActorEvent(TrinityActor actor);

        public static event ActorEvent OnActorFound;
        public static event ActorEvent OnActorKilled;

        public static void FireActorFound(TrinityActor actor)
        {
            OnActorFound?.Invoke(actor);
        }

        public static void FireActorKilled(TrinityActor actor)
        {
            OnActorKilled?.Invoke(actor);
        }
    }


}
