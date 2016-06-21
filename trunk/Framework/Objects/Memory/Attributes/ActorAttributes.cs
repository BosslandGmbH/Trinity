using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects.Memory.Items;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Attributes
{
    public class ActorAttributes : Attributes
    {
        public ActorAttributes(int groupId) : base(groupId)
        {

        }

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

        public bool IsDead => Hitpoints <= 0;

        public float Hitpoints => GetAttribute<float>(ActorAttributeType.HitpointsCur);

        public float HitpointsMax => GetAttribute<float>(ActorAttributeType.HitpointsMax);

        public float HitpointsPct => HitpointsMax > 0 ? Hitpoints / HitpointsMax * 100 : 0;

        public ResourceType ResourceTypePrimary => GetAttribute<ResourceType>(ActorAttributeType.ResourceTypePrimary);

        public ResourceType ResourceTypeSecondary => GetAttribute<ResourceType>(ActorAttributeType.ResourceTypeSecondary);
    }
}




