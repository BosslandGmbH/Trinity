using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Resources
{
    public static class PartyHelper
    {
        public static TrinityPlayer FindPlayerByHeroId(int heroId)
        {
            return Core.Actors.OfType<TrinityPlayer>().FirstOrDefault(p => p.AcdId == heroId);
        }

        public static TrinityActor FindLocalActor(ITargetable target)
        {
            if (target == null)
                return null;

            // Targets passed to us from another bot/diablo3 will have different Ann/Acd that wont match ours.
            // Unless we can find an identifier that doesnt change we'll have to make a guess.

            return Core.Actors.OfType<TrinityActor>().Where(a => a.ActorSnoId == target.ActorSnoId && a.Position.Distance(target.Position) <= 15f).OrderBy(u => u.Position.Distance(target.Position)).FirstOrDefault();
        }

        public static IEnumerable<TrinityPlayer> OtherPlayers 
            => Core.Actors.OfType<TrinityPlayer>().Where(p => !p.IsMe);

        public static bool AnyPlayerWithSkill(SNOPower power)
            => OtherPlayers.Any(player => player.Attributes.GetAttribute<bool>(ActorAttributeType.Skill, (int)power));

        public static bool AnyPlayerWithSkill(Skill skill)
            => AnyPlayerWithSkill(skill.SNOPower);

        public static bool IsPlayerWithSkill(IPartyMember specificPlayer, Skill skill)
            => IsPlayerWithSkill(specificPlayer, skill.SNOPower);

        public static bool IsPlayerWithSkill(IPartyMember specificPlayer, SNOPower power)
            => IsPlayerWithAttrihbute<bool>(specificPlayer, ActorAttributeType.Skill, (int)power);

        public static T IsPlayerWithAttrihbute<T>(IPartyMember specificPlayer, ActorAttributeType attribute, int argument) where T : struct
        {
            var player = FindLocalPlayer(specificPlayer);
            return player?.Attributes.GetAttribute<T>(ActorAttributeType.Skill, argument) ?? default(T);
        }

        public static TrinityActor FindLocalPlayer(IPartyMember member)
        {
            return Core.Actors.Actors.OfType<TrinityPlayer>().FirstOrDefault(p => Math.Abs(p.HitPointsMax - member.HitpointsMaxTotal) < 5);
        }
    }
}
