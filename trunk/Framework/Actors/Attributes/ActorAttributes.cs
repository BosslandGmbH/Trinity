using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects.Memory;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using AttributeParameterType = Trinity.Framework.Objects.Memory.AttributeParameterType;
using DamageType = Trinity.Framework.Objects.Memory.DamageType;

namespace Trinity.Framework.Actors.Attributes
{
    public class ActorAttributes : Objects.Memory.Attributes
    {
        public class AttributeKey
        {
            public AttributeKey(int value)
            {
                Value = value;
            }

            public int Value;

            public AttributeKey(ActorAttributeType attribute, SNOPower power)
            {
                Value = ((int)power << 12) + ((int)attribute & 0xFFF);
            }

            public AttributeKey(int attribute, int modifier)
            {
                Value = (modifier << 12) + (attribute & 0xFFF);
            }

            public int DescripterId
            {
                get { return (Value & 0xFFF); }
                set { Value = (int)((Value & 0xFFFFF000) + (int)value); }
            }

            public ActorAttributeType BaseAttribute => (ActorAttributeType)(-1 << 12) + (DescripterId & 0xFFF);

            private AttributeDescriptor? _descriptor;
            public AttributeDescriptor Descriptor => _descriptor ?? (_descriptor = DescriptorHelper.GetDescriptor(DescripterId)).Value;

            public int ModifierId
            {
                get { return Value >> 12; }
                set { Value = (Value & 0x00000FFF) + (value << 12); }
            }

            public object Modifer
            {
                get
                {
                    switch (Descriptor.ParameterType)
                    {
                        case Zeta.Game.Internals.AttributeParameterType.PowerSnoId:
                            return (SNOPower)ModifierId;
                        case Zeta.Game.Internals.AttributeParameterType.ActorType:
                            return (SNOActor)ModifierId;
                        case Zeta.Game.Internals.AttributeParameterType.ConversationSnoId:
                            return (SNOConversation)ModifierId;
                        case Zeta.Game.Internals.AttributeParameterType.InventoryLocation:
                            return (InventorySlot)ModifierId;
                        case Zeta.Game.Internals.AttributeParameterType.DamageType:
                            return (DamageType)ModifierId;
                        case Zeta.Game.Internals.AttributeParameterType.PowerSnoId2:
                            return (SNOPower)ModifierId;
                        case Zeta.Game.Internals.AttributeParameterType.ResourceType:
                            return (ResourceType)ModifierId;
                        case Zeta.Game.Internals.AttributeParameterType.RequirementType:
                            return (RequirementType)ModifierId;
                    }
                    return ModifierId;
                }
            }

            public string ModifierInfo => ModifierId > 0 ? $" [ {Descriptor.ParameterType}: {Modifer}: {ModifierId} ]" : ModifierId.ToString();

            public override int GetHashCode() => unchecked((int)(Value ^ (Value >> 12)));

            public override string ToString() => $"Id: {DescripterId} Attribute: {BaseAttribute} Modifier: {ModifierInfo}";
        }

        public static class DescriptorHelper
        {
            private static Dictionary<int, Zeta.Game.Internals.AttributeDescriptor> _descripters;

            public static Zeta.Game.Internals.AttributeDescriptor GetDescriptor(int id, bool checkExists = false)
            {
                if (_descripters == null)
                    _descripters = ZetaDia.AttributeDescriptors.ToDictionary(descripter => descripter.Id);

                return !checkExists || _descripters.ContainsKey(id) ? _descripters[id] : default(Zeta.Game.Internals.AttributeDescriptor);
            }
        }

        public ActorAttributes()
        {
        }

        public ActorAttributes(int groupId) : base(groupId)
        {
        }

        public Dictionary<SNOPower, float> Powers => GetAttributes<SNOPower, float>(AttributeParameterType.PowerSnoId);

        public int GizmoState => GetAttribute<int>(ActorAttributeType.GizmoState);

        public bool IsMinimapActive => GetAttribute<bool>(ActorAttributeType.MinimapActive);

        public int MinimapIconOverride => GetAttribute<int>(ActorAttributeType.MinimapIconOverride);

        public bool HasFirebirdTemporary => GetAttribute<bool>(ActorAttributeType.PowerBuff1VisualEffectNone, (int)SNOPower.ItemPassive_Unique_Ring_733_x1);

        public bool HasFirebirdPermanent => GetAttribute<bool>(ActorAttributeType.PowerBuff4VisualEffectNone, (int)SNOPower.ItemPassive_Unique_Ring_733_x1);

        public bool IsBurrowed => GetAttribute<bool>(ActorAttributeType.Burrowed);

        public bool IsHidden => GetAttribute<bool>(ActorAttributeType.Hidden);

        public bool IsUsingBossbar => GetAttribute<bool>(ActorAttributeType.UsingBossbar);

        public bool NPCIsOperatable => GetAttribute<bool>(ActorAttributeType.NPCIsOperatable);

        public bool HasDotDps => GetAttribute<bool>(ActorAttributeType.DOTDPS);

        public bool IsShadowClone => GetCachedAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectB, (int)SNOPower.Diablo_ShadowClones);

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

        //public int TeamId => GetAttribute<int>(ActorAttributeType.TeamId);
        public int TeamId => GetAttributeDirectlyFromTable<int>(ActorAttributeType.TeamId);

        public MarkerType MarkerType => GetAttribute<MarkerType>(ActorAttributeType.ConversationIcon, 0); // 483: ConversationIcon(-3613)

        public bool NpcHasInteractOptions => GetAttribute<bool>(ActorAttributeType.NPCHasInteractOptions, 0);

        public int SummonerId => GetAttribute<int>(ActorAttributeType.SummonerId);

        public bool IsNPC => GetAttribute<bool>(ActorAttributeType.IsNPC);

        public int SummonedByAnnId => GetAttribute<int>(ActorAttributeType.SummonedByACDId);

        public bool HasBuffVisualEffect => GetAttribute<bool>(ActorAttributeType.BuffVisualEffect);

        public bool IsQuestMonster => GetAttribute<bool>(ActorAttributeType.QuestMonster);

        //public PetType PetType => GetAttribute<PetType>(ActorAttributeType.PetType);

        public PetType PetType => GetAttributeOrCustomDefault<PetType>(ActorAttributeType.PetType, () => PetType.None);

        public bool IsGizmoDisabledByScript => GetAttribute<bool>(ActorAttributeType.GizmoDisabledByScript);

        //public bool IsDeletedOnServer => GetAttributeDirectlyFromTable<bool>(ActorAttributeType.DeletedOnServer);
        public bool IsDeletedOnServer => GetAttribute<bool>(ActorAttributeType.DeletedOnServer);

        public int LastDamageAnnId => GetAttribute<int>(ActorAttributeType.LastDamageACD);

        public bool IsDoorTimed => GetAttribute<int>(ActorAttributeType.DoorTimer) > 0;

        public int EffectOwnerAnnId => GetAttribute<int>(ActorAttributeType.EffectOwnerANN);


    }
}