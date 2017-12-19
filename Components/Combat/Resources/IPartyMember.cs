using Zeta.Game;

namespace Trinity.Components.Combat.Resources
{
    public interface IPartyMember : ITargetable
    {
        int MemberId { get; }
        ActorClass ActorClass { get; }
        PartyRole Role { get; }
        PartyObjective Objective { get; }
        ITargetable Target { get; }
        float LeashDistance { get; }
        bool IsLeader { get; }
        bool IsFollower { get; }
        bool IsMe { get; }
        bool IsInCombat { get; }
        int HeroId { get; }
        double HitpointsMaxTotal { get; }
    }
}