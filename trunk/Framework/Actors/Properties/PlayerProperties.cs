using System.Linq;
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

            var cPlayer = ZetaDia.Storage.PlayerDataManager.ActivePlayerData;
            if (cPlayer != null && cPlayer.IsValid)
            {
                actor.HeroId = cPlayer.HeroId;
                actor.HeroName = cPlayer.HeroName;
            }
        }

        public static int GetAcdIdByHeroId(int heroId)
        {
            // Only works if player is in the same area.
            var player = ZetaDia.Storage.PlayerDataManager.Players.FirstOrDefault(p => p.HeroId == heroId);
            return player?.ACDId ?? -1;
        }

        public static int GetHeroIdByAcdId(int acdId)
        {
            // Only works if player is in the same area.
            var player = ZetaDia.Storage.PlayerDataManager.GetPlayerDataByAcdId(acdId);
            return player?.HeroId ?? -1;
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