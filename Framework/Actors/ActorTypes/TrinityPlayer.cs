using System;
using Trinity.Framework.Helpers;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Actors.Properties;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Actors.ActorTypes
{
    public sealed class TrinityPlayer : TrinityActor, IPartyMember
    {
        public TrinityPlayer(DiaObject seed) : base(seed)
        {
        }

        public int HeroId => ZetaDia.Service.Hero.HeroId;
        public string HeroName => ZetaDia.Service.Hero.Name;
        public ActorClass ActorClass => PlayerProperties.GetActorClass(ActorSnoId);
        public int MemberId => MemoryHelper.GameBalanceNormalHash(Name);

        #region IPartyMember

        // We're severely limited in what we can know about other players in the game.
        PartyObjective IPartyMember.Objective => default(PartyObjective);

        ITargetable IPartyMember.Target => TrinityCombat.Targeting.CurrentTarget;
        float IPartyMember.LeashDistance => 100f;
        double IPartyMember.HitpointsMaxTotal => (double)HitPointsMax;
        bool IPartyMember.IsLeader => TrinityCombat.Party.Leader == this;
        bool IPartyMember.IsFollower => TrinityCombat.Party.Leader != this;
        PartyRole IPartyMember.Role => TrinityCombat.Party.Leader == this ? PartyRole.Leader : PartyRole.Follower;
        bool IPartyMember.IsInCombat => false; // No way to know if another CPlayer is in combat?

        #endregion IPartyMember

        public override void OnCreated()
        {
            //Attributes = new PlayerAttributes(FastAttributeGroupId);
            UpdateProperties();
        }

        public override void OnUpdated()
        {
            Attributes.Update(CommonData);
            CommonProperties.Populate(this);
            UnitProperties.Populate(this);
        }

        private void UpdateProperties()
        {
            CommonProperties.Update(this);
            UnitProperties.Update(this);
        }

        public override string ToString() => $"{GetType().Name}: AcdId={AcdId}, {ActorClass} {(IsMe ? "ActivePlayer" : String.Empty)}";
    }
}