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

        public int GizmoState => GetCachedAttribute<int>(ActorAttributeType.GizmoState);

        public bool IsMinimapActive => GetCachedAttribute<bool>(ActorAttributeType.MinimapActive);

        public bool HasFirebirdTemporary => GetAttribute<bool>(ActorAttributeType.PowerBuff1VisualEffectNone, (int)SNOPower.ItemPassive_Unique_Ring_733_x1);

        public bool HasFirebirdPermanent => GetAttribute<bool>(ActorAttributeType.PowerBuff4VisualEffectNone, (int)SNOPower.ItemPassive_Unique_Ring_733_x1);

        public bool IsBurrowed => GetAttribute<bool>(ActorAttributeType.Burrowed);

        public bool NPCIsOperatable => GetAttribute<bool>(ActorAttributeType.NPCIsOperatable);

        public bool HasDotDps => GetAttribute<bool>(ActorAttributeType.DOTDPS);

        public bool IsIllusion => GetCachedAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectNone, (int)SNOPower.MonsterAffix_IllusionistCast);

        public bool IsReflecting => GetCachedAttribute<bool>(ActorAttributeType.PowerBuff3VisualEffectNone, (int)SNOPower.MonsterAffix_ReflectsDamageCast);

        public bool IsUntargetable => GetAttribute<bool>(ActorAttributeType.Untargetable);

        public bool IsInvulnerable => GetAttribute<bool>(ActorAttributeType.Invulnerable);
    }
}




