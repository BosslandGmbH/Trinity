using System;
using Trinity.Framework.Helpers;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Actors.Properties;
using Zeta.Game;

namespace Trinity.Framework.Actors.ActorTypes
{
    public class TrinityPlayer : TrinityActor, IPartyMember
    {
        public override AttributesWrapper Attributes { get; set; }
        public int HeroId { get; set; }
        public string HeroName { get; set; }
        public ActorClass ActorClass { get; set; }
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
            Attributes = new AttributesWrapper(CommonData);
            base.Attributes = Attributes;
            UpdateProperties();
        }

        public override void OnUpdated()
        {
            Attributes.Update(CommonData);
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            CommonProperties.Populate(this);
            UnitProperties.Populate(this);
            PlayerProperties.Populate(this);
        }

        public override string ToString() => $"{GetType().Name}: AcdId={AcdId}, {ActorClass} {(IsMe ? "ActivePlayer" : String.Empty)}";
    }
}