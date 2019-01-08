using System.Linq;
using Zeta.Game;

namespace Trinity.Framework.Actors.Properties
{
    public class PlayerProperties
    {
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

        public static ActorClass GetActorClass(SNOActor actorSnoId)
        {
            switch (actorSnoId)
            {
                case SNOActor.Wizard_Female:
                case SNOActor.Wizard_Male:
                    return ActorClass.Wizard;

                case SNOActor.Barbarian_Female:
                case SNOActor.Barbarian_Male:
                    return ActorClass.Barbarian;

                case SNOActor.Demonhunter_Female:
                case SNOActor.Demonhunter_Male:
                    return ActorClass.DemonHunter;

                case SNOActor.X1_Crusader_Female:
                case SNOActor.X1_Crusader_Male:
                    return ActorClass.Crusader;

                case SNOActor.WitchDoctor_Female:
                case SNOActor.WitchDoctor_Male:
                    return ActorClass.Witchdoctor;

                case SNOActor.Monk_Female:
                case SNOActor.Monk_Male:
                    return ActorClass.Monk;
            }
            return ActorClass.Invalid;
        }
    }
}