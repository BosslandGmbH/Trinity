using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// Properties that are specific to players
    /// This class should update all values that are possible/reasonable/useful.
    /// DO NOT put settings or situational based exclusions in here, do that in weighting etc.
    /// </summary>
    public class PlayerProperties : IPropertyCollection
    {
        public DateTime CreationTime { get; } = DateTime.UtcNow;

        public void ApplyTo(TrinityCacheObject target)
        {
            target.ActorClass = this.ActorClass;
        }

        public void OnCreate(TrinityCacheObject source)
        {
            if (source.ActorType != ActorType.Player || !source.IsValid)
                return;

            var commonData = source.CommonData;
            if (commonData == null || !commonData.IsValid)
                return;

            var activePlayer = source.Object as DiaActivePlayer;
            if (activePlayer == null || !activePlayer.IsValid)
                return;

            this.ActorClass = PlayerPropertyUtils.GetActorClass(source.ActorSNO);
        }

        public ActorClass ActorClass { get; set; }

        public void Update(TrinityCacheObject source)
        {
       
        }   
    }

    public class PlayerPropertyUtils
    {
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



