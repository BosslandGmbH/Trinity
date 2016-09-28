using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;

namespace Trinity.Components.Combat.Party
{
    public static class PartyHelper
    {
        public static TrinityPlayer FindPlayerByHeroId(int heroId)
        {
            return Core.Actors.GetActorsOfType<TrinityPlayer>().FirstOrDefault(p => p.AcdId == heroId);
        }

        public static TrinityActor FindLocalActor(ITargetable target)
        {
            if (target == null)
                return null;

            // Targets passed to us from another bot/diablo3 will have different Ann/Acd that wont match ours.
            // Unless we can find an identifier that doesnt changed we'll have to make a guess.            
                        
            return Core.Actors.GetActorsOfType<TrinityActor>().FirstOrDefault(a => a.ActorSnoId == target.ActorSnoId && a.Position.Distance(target.Position) <= 15f && a.WorldDynamicId == target.WorldDynamicId);
        }

        public static IPartyMember EmptyMember { get; } = new TrinityPlayer();

    }
}
