using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Zeta.Game;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.Properties
{
    public class PlayerProperties
    {
        public static void Populate(TrinityPlayer actor)
        {
            if (actor.ActorType != ActorType.Player)
                return;

            if (!actor.IsAcdBased || !actor.IsAcdValid)
                return;

            var attributes = actor.Attributes as PlayerAttributes;
            actor.ActorClass = GetActorClass(actor.ActorSnoId);
            actor.IsMe = actor.RActorId == Core.Actors.ActivePlayerRActorId;

            // todo move caching from PlayerCache to here.
        }

        public static ActorClass GetActorClass(int actorSnoId)
        {
            switch (actorSnoId)
            {
                case (int)SNOActor.Wizard_Female:
                case (int)SNOActor.Wizard_Male:
                    return ActorClass.Wizard;
                case (int)SNOActor.Barbarian_Female:
                case (int)SNOActor.Barbarian_Male:
                    return ActorClass.Barbarian;
                case (int)SNOActor.Demonhunter_Female:
                case (int)SNOActor.Demonhunter_Male:
                    return ActorClass.DemonHunter;
                case (int)SNOActor.X1_Crusader_Female:
                case (int)SNOActor.X1_Crusader_Male:
                    return ActorClass.Crusader;
                case (int)SNOActor.WitchDoctor_Female:
                case (int)SNOActor.WitchDoctor_Male:
                    return ActorClass.Witchdoctor;
                case (int)SNOActor.Monk_Female:
                case (int)SNOActor.Monk_Male:
                    return ActorClass.Monk;
            }
            return ActorClass.Invalid;
        }


    }


}



