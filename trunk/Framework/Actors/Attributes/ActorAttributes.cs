using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Helpers;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Actors.Attributes
{
    public class ActorAttributes : Objects.Memory.Attributes.Attributes
    {
        public ActorAttributes() { }

        public ActorAttributes(int groupId) : base(groupId) { }

        public int GizmoState => GetAttribute<int>(ActorAttributeType.GizmoState);

        public bool IsMinimapActive => GetAttribute<bool>(ActorAttributeType.MinimapActive);

        public bool HasFirebirdTemporary => GetAttribute<bool>(ActorAttributeType.PowerBuff1VisualEffectNone, (int)SNOPower.ItemPassive_Unique_Ring_733_x1);

        public bool HasFirebirdPermanent => GetAttribute<bool>(ActorAttributeType.PowerBuff4VisualEffectNone, (int)SNOPower.ItemPassive_Unique_Ring_733_x1);

        public bool IsBurrowed => GetAttribute<bool>(ActorAttributeType.Burrowed);

        public bool IsHidden => GetAttribute<bool>(ActorAttributeType.Hidden);

        public bool NPCIsOperatable => GetAttribute<bool>(ActorAttributeType.NPCIsOperatable);

        public bool HasDotDps => GetAttribute<bool>(ActorAttributeType.DOTDPS);

        public bool IsIllusion => GetCachedAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectNone, (int)SNOPower.MonsterAffix_IllusionistCast);

        public bool IsReflecting => GetAttribute<bool>(ActorAttributeType.PowerBuff3VisualEffectNone, (int)SNOPower.MonsterAffix_ReflectsDamageCast);

        public bool IsUntargetable => GetAttribute<bool>(ActorAttributeType.Untargetable);

        public bool IsRiftBoss => GetAttribute<bool>(ActorAttributeType.IsLootRunBoss);

        public bool IsInvulnerable => GetAttribute<bool>(ActorAttributeType.Invulnerable);

        public bool IsBountyObjective => GetAttribute<bool>(ActorAttributeType.BountyObjective);

        public float Hitpoints => GetAttribute<float>(ActorAttributeType.HitpointsCur);

        public float HitpointsMax => GetAttribute<float>(ActorAttributeType.HitpointsMax);

        public float HitpointsPct => HitpointsMax > 0 ? Hitpoints / HitpointsMax * 100 : 0;

        public bool IsGizmoBeenOperated => GetAttribute<bool>(ActorAttributeType.GizmoHasBeenOperated);

        public int GizmoOperatorACDId => GetAttribute<int>(ActorAttributeType.GizmoOperatorACDId);

        public bool IsChestOpen => GetAttribute<bool>(ActorAttributeType.ChestOpen);

        public int GizmoCharges => GetAttribute<int>(ActorAttributeType.GizmoCharges);

        public bool IsNoDamage => GetAttribute<bool>(ActorAttributeType.NoDamage);

        public bool IsDoorLocked => GetAttribute<bool>(ActorAttributeType.DoorLocked);

        public int TeamOverride => GetAttribute<int>(ActorAttributeType.TeamOverride);

        public int TeamId => GetAttribute<int>(ActorAttributeType.TeamId);

        public MarkerType MarkerType => GetFirstCachedAttribute<MarkerType>(ActorAttributeType.ConversationIcon); // 483: ConversationIcon(-3613)

        public int SummonerId => GetAttribute<int>(ActorAttributeType.SummonerId);

        public bool IsNPC => GetAttribute<bool>(ActorAttributeType.IsNPC);

        public int SummonedByAnnId => GetAttribute<int>(ActorAttributeType.SummonedByACDId);

        public bool HasBuffVisualEffect => GetAttribute<bool>(ActorAttributeType.BuffVisualEffect);

        public bool IsQuestMonster => GetAttribute<bool>(ActorAttributeType.QuestMonster);

        //public PetType PetType => GetAttribute<PetType>(ActorAttributeType.PetType);

        public PetType PetType => GetAttributeOrCustomDefault<PetType>(ActorAttributeType.PetType, () => PetType.None);

        public bool IsGizmoDisabledByScript => GetAttribute<bool>(ActorAttributeType.GizmoDisabledByScript);
    }
}




