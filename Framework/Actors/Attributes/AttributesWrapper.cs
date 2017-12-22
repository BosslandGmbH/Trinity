using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using AttributeParameterType = Zeta.Game.Internals.AttributeParameterType;
using DamageType = Zeta.Game.DamageType;

namespace Trinity.Framework.Actors.Attributes
{
    public class AttributesWrapper
    {
        private ACD _commonData;
        private AttributeTable _table;

        public AttributesWrapper(ACD commonData)
        {
            Update(commonData);
        }

        /// <summary>
        /// Change ACD used for attribute reading.
        /// </summary>
        /// <param name="commonData">the ACD</param>
        public void Update(ACD commonData)
        {
            // !!! Simple GetAttribute calls are using DB attributes API via CommonData uncached !!!

            if (commonData == null)
                return;

            _commonData = commonData;

            // AttributeTable/AttribtueEntry is here for enumerating a key-value pair of each attribute.            
            // A feature not provided by DB, and used for logging items, debug investigations 
            // and also for more advanced queries like finding duplicate attributes by base ActorAttributeType or ParameterType

            var group = ZetaDia.Storage.FastAttribGroups[commonData.FastAttribGroupId];
            var mapPtr = GetMapPtr(group);
            _table = new AttributeTable(mapPtr + 0x10);
        }

        /// <summary>
        /// Enumeration of all actor attributes 
        /// </summary>
        public IEnumerable<AttributeEntry> Items => _table.Items;

        #region Search Methods

        /// <summary>
        /// Get an attribute for an actor
        /// </summary>
        /// <typeparam name="T">the type of return value</typeparam>
        /// <param name="type">type of attribute to find</param>
        /// <param name="modifier">id of modifier (SNOPower etc)</param>
        /// <returns>the attribute value</returns>
        public T GetAttribute<T>(ActorAttributeType type, int modifier = -1) where T : struct
        {
            if (!IsValid) return default(T);

            return _commonData.GetAttribute<T>(type, modifier);
        }

        /// <summary>
        /// Get an attribute for an actor with custom default value
        /// </summary>
        /// <typeparam name="T">the type of return value</typeparam>
        /// <param name="type">type of attribute to find</param>
        /// <param name="defaultProducer">func for custom default value</param>
        /// <returns>the attribute value</returns>
        public T GetAttribute<T>(ActorAttributeType type, Func<T> defaultProducer) where T : struct
        {
            if (!IsValid) return defaultProducer();

            return _commonData.GetAttribute<T>(type);
        }

        /// <summary>
        /// Get an attribute for an actor using an AttributeKey
        /// </summary>
        /// <typeparam name="T">the type of return value</typeparam>
        /// <param name="key">the AttributeKey</param>
        /// <returns>the attribute value</returns>
        internal T GetAttribute<T>(AttributeKey key) where T : struct
        {
            if (!IsValid) return default(T);

            return GetAttribute<T>(key.BaseAttribute, key.ModifierId);
        }

        /// <summary>
        /// Find an attribute using a base type (will match any modifier)
        /// </summary>
        /// <typeparam name="T">the type of return value</typeparam>
        /// <param name="type">the attribute type</param>
        /// <returns>the attribute value</returns>
        internal T GetFirstBaseAttribute<T>(ActorAttributeType type) where T : struct
        {
            if (!IsValid) return default(T);

            var key = new AttributeKey((int)type);
            return GetAttribute<T>(key.BaseAttribute);
        }

        /// <summary>
        /// Find an attribute using a base type (will match any modifier)
        /// </summary>
        /// <typeparam name="T">the type of return value</typeparam>
        /// <param name="type">the attribute type</param>
        /// <returns>the attribute value</returns>
        internal AttributeEntry GetAttributeEntry(ActorAttributeType type)
        {
            if (!IsValid) return null;

            return _table.Items.FirstOrDefault(i => i.Key.Value == (int)type || i.Key.BaseAttribute == type);
        }

        /// <summary>
        /// Find all attributes matching a base or combined (base + modifier) key.
        /// </summary>
        /// <typeparam name="T">the type of return value</typeparam>
        /// <param name="type">the attribute type</param>
        /// <returns>the attribute value</returns>
        internal IEnumerable<T> GetAttributes<T>(ActorAttributeType type) where T : struct
        {
            if (!IsValid) return Enumerable.Empty<T>();

            return _table.Items.Where(i => i.Key.BaseAttribute == type || i.Key.BaseAttribute == type)
                .Select(a => a.GetValue<T>());
        }

        /// <summary>
        /// Find all attributes matching a base, indexed by modifier.
        /// </summary>
        /// <typeparam name="TKey">the modifier type (DamageType, ResourceType etc.)</typeparam>
        /// <typeparam name="TValue">the type of value to be returned (float etc.)</typeparam>
        /// <param name="attributeType">the attribute type to find</param>
        /// <returns>a dictionary of attribute values by modifier</returns>
        internal Dictionary<TKey, TValue> GetAttributes<TKey, TValue>(ActorAttributeType attributeType)
        {
            if (!IsValid) return new Dictionary<TKey, TValue>();

            return _table.Items
                .Where(a => a.Key.BaseAttribute == attributeType)
                .DistinctBy(k => k.Key.ModifierId).ToDictionary(
                k => (TKey)Convert.ChangeType(k.Key.Modifer, typeof(TKey)),
                v => (TValue)Convert.ChangeType(v.Value, typeof(TValue)));
        }

        /// <summary>
        /// Find all attributes matching a parameter type, indexed by modifier.
        /// </summary>
        /// <typeparam name="TKey">the modifier type (DamageType, ResourceType etc.)</typeparam>
        /// <typeparam name="TValue">the type of value to be returned (float etc.)</typeparam>
        /// <param name="paramType">the attribute type to find</param>
        /// <returns>a dictionary of attribute values by modifier</returns>
        internal Dictionary<TKey, TValue> GetAttributes<TKey, TValue>(AttributeParameterType paramType)
        {
            if (!_commonData.IsValid) return new Dictionary<TKey, TValue>();

            return _table.Items
                .Where(a => a.Key.Descriptor.ParameterType == paramType)
                .DistinctBy(k => k.Key.ModifierId).ToDictionary(
                k => (TKey)Convert.ChangeType(k.Key.Modifer, typeof(TKey)),
                v => (TValue)Convert.ChangeType(v.Value, typeof(TValue)));
        }

        /// <summary>
        /// Find all attributes that use a particular parameter type e.g. SNOPower.
        /// </summary>
        /// <typeparam name="T">the type of return value</typeparam>
        /// <param name="paramType">the type of modifier used by the attribute</param>
        /// <returns>the attribute value</returns>
        internal IEnumerable<T> GetAttributes<T>(AttributeParameterType paramType) where T : struct
        {
            if (!IsValid) return Enumerable.Empty<T>();

            return _table.Items.Where(i => i.Key.Descriptor.ParameterType == paramType)
                .Select(a => a.GetValue<T>());
        }

        /// <summary>
        /// Dictionary of attribute values by native key (ActorAttributeType && Modifier)
        /// It can include duplicates of the same ActorAttributeType
        /// </summary>
        public Dictionary<ActorAttributeType, float> ToDictionary()
        {
            if (!IsValid) return new Dictionary<ActorAttributeType, float>();
            return _table.Items.DistinctBy(k => k.Key.Value).ToDictionary(
                k => (ActorAttributeType)k.Key.Value,
                v => (float)Convert.ChangeType(v.Value, typeof(float)));
        }

        /// <summary>
        /// Dictionary of attribute values by base ActorAttributeType
        /// It does not include duplicates of the same ActorAttributeType
        /// </summary>
        public Dictionary<ActorAttributeType, float> ToBaseDictionary()
        {
            if (!IsValid) return new Dictionary<ActorAttributeType, float>();

            return _table.Items.DistinctBy(k => k.Key.BaseAttribute).ToDictionary(
                k => k.Key.BaseAttribute,
                v => (float)Convert.ChangeType(v.Value, typeof(float)));
        }


        #endregion

        #region Memory

        /// <summary>
        /// Get the pointer for an actors' attribute group map (varies based on player or not)
        /// </summary>
        /// <returns></returns>
        public IntPtr GetMapPtr(FastAttribGroupsEntry group)
        {
            return (group.Flags & 4) != 0 ? ZetaDia.Memory.Read<IntPtr>(group.BaseAddress + 0x00C) : group.BaseAddress + 0x010;
        }

        /// <summary>
        /// Memory object for an actor attribute Key/Value pair.
        /// </summary>
        public class AttributeEntry : NativeObject
        {
            public AttributeEntry(IntPtr ptr) : base(ptr) { }

            private AttributeKey _key;
            public AttributeKey Key => _key ?? (_key = new AttributeKey(ZetaDia.Memory.Read<int>(BaseAddress + 0x04)));
            public object Value => Key.Descriptor.IsInteger == 1 ? GetValue<int>() : GetValue<float>();

            public T GetValue<T>() where T : struct
            {
                if (typeof(T) == typeof(float))
                {
                    var result = ZetaDia.Memory.Read<float>(BaseAddress + 0x08);
                    return (T)Convert.ChangeType(Math.Truncate(result * 100) / 100, typeof(T));
                }
                return ZetaDia.Memory.Read<T>(BaseAddress + 0x08);
            }

            public override string ToString()
            {
                var modString = Key.ModifierId > 0 ? $" [ {Key.Descriptor.ParameterType}: {Key.Modifer}: {Key.ModifierId} ]" : string.Empty;
                return $"{Key.DescripterId}: {Key.BaseAttribute} ({(int)Key.BaseAttribute}){modString} i:{GetValue<int>()} f:{GetValue<float>()} Value={Value:0.##}";
            }
        }

        /// <summary>
        /// Memory Object for an actor's attributes, contains an array of ptrs to Key/Value pairs.
        /// </summary>
        public class AttributeTable : NativeObject, IEnumerable<AttributeEntry>
        {
            public AttributeTable(IntPtr ptr) : base(ptr) { }
            public IntPtr DataPtr => IsValid ? ReadOffset<IntPtr>(0x00) : IntPtr.Zero;
            public int Size => IsValid ? ReadOffset<int>(0x08) : 0;
            public IntPtr[] RowPtrs => IsValid ? ZetaDia.Memory.ReadArray<IntPtr>(DataPtr, Size) : new IntPtr[0];
            public IEnumerable<AttributeEntry> Items => RowPtrs.Where(p => p != IntPtr.Zero).Select(ptr => new AttributeEntry(ptr));
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<AttributeEntry> GetEnumerator() => Items.GetEnumerator();
        }

        /// <summary>
        /// A helper object for easy bit translations of key used to index Attributes in memory.
        /// </summary>
        public class AttributeKey
        {
            /// <summary>
            /// The key value as found in Memory Attribute table.
            /// </summary>
            public int Value;

            /// <summary>
            /// Create AttributeKey with a key value
            /// </summary>
            /// <param name="value"></param>
            public AttributeKey(int value)
            {
                Value = value;
            }

            /// <summary>
            /// Create AttributeKey using seperate attribute and modifier.
            /// </summary>
            /// <param name="attribute">the ActorAttributeType</param>
            /// <param name="modifier">the attribute specific modifier</param>
            public AttributeKey(int attribute, int modifier)
            {
                Value = (modifier << 12) + (attribute & 0xFFF);
            }

            /// <summary>
            /// The descripter id portion of a key value.
            /// </summary>
            public int DescripterId
            {
                get { return (Value & 0xFFF); }
                set { Value = (int)((Value & 0xFFFFF000) + (int)value); }
            }

            /// <summary>
            /// The base ActorAttributeType portion of a key value.
            /// </summary>
            public ActorAttributeType BaseAttribute => (ActorAttributeType)(-1 << 12) + (DescripterId & 0xFFF);

            private AttributeDescriptor? _descriptor;
            public AttributeDescriptor Descriptor => _descriptor ?? (_descriptor = DescriptorHelper.GetDescriptor(DescripterId, true)).Value;

            /// <summary>
            /// The modifier portion of a key value.
            /// </summary>
            public int ModifierId
            {
                get { return Value >> 12; }
                set { Value = (Value & 0x00000FFF) + (value << 12); }
            }

            /// <summary>
            /// The ModifierId cast to known Parameter Type if possible
            /// </summary>
            public object Modifer
            {
                get
                {
                    switch (Descriptor.ParameterType)
                    {
                        case AttributeParameterType.PowerSnoId:
                            return (SNOPower)ModifierId;
                        case AttributeParameterType.ActorType:
                            return (SNOActor)ModifierId;
                        case AttributeParameterType.ConversationSnoId:
                            return (SNOConversation)ModifierId;
                        case AttributeParameterType.InventoryLocation:
                            return (InventorySlot)ModifierId;
                        case AttributeParameterType.DamageType:
                            return (DamageType)ModifierId;
                        case AttributeParameterType.PowerSnoId2:
                            return (SNOPower)ModifierId;
                        case AttributeParameterType.ResourceType:
                            return (ResourceType)ModifierId;
                        case AttributeParameterType.RequirementType:
                            return (RequirementType)ModifierId;
                    }
                    return ModifierId;
                }
            }

            public string ModifierInfo => ModifierId > 0 ? $" [ {Descriptor.ParameterType}: {Modifer}: {ModifierId} ]" : ModifierId.ToString();

            public override int GetHashCode() => unchecked((int)(Value ^ (Value >> 12)));

            public override string ToString() => $"Id: {DescripterId} Attribute: {BaseAttribute} Modifier: {ModifierInfo}";
        }

        /// <summary>
        /// Helper to quickly access AttributeDescriptors
        /// </summary>
        public static class DescriptorHelper
        {
            private static Dictionary<int, AttributeDescriptor> _descripters;

            public static AttributeDescriptor GetDescriptor(int id, bool checkExists = false)
            {
                if (_descripters == null)
                    _descripters = ZetaDia.AttributeDescriptors.ToDictionary(descripter => descripter.Id);

                return !checkExists || _descripters.ContainsKey(id) ? _descripters[id] : default(AttributeDescriptor);
            }
        }

        #endregion

        #region Player Attributes

        public ResourceType ResourceTypePrimary => GetAttribute<ResourceType>(ActorAttributeType.ResourceTypePrimary);

        public ResourceType ResourceTypeSecondary => GetAttribute<ResourceType>(ActorAttributeType.ResourceTypeSecondary);

        public int GetChargesCurrent(SNOPower power) => GetAttribute<int>(ActorAttributeType.SkillCharges, (int)power);

        public int GetChargesMax(SNOPower power) => GetAttribute<int>(ActorAttributeType.AllowSkillChanges, (int)power);

        public int ShieldHitpoints => GetAttribute<int>(ActorAttributeType.BreakableShieldHP);

        public int HasDamageShield => GetAttribute<int>(ActorAttributeType.DamageShield);

        public int DamageShieldHitpoints => GetAttribute<int>(ActorAttributeType.DamageShieldAmount);

        #endregion

        #region Actor Attributes

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

        public bool IsShadowClone => GetAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectB, (int)SNOPower.Diablo_ShadowClones);

        public bool IsIllusion => GetAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectNone, (int)SNOPower.MonsterAffix_IllusionistCast);

        public bool IsReflecting => GetAttribute<bool>(ActorAttributeType.PowerBuff3VisualEffectNone, (int)SNOPower.MonsterAffix_ReflectsDamageCast);

        public bool IsUntargetable => GetAttribute<bool>(ActorAttributeType.Untargetable);

        public bool IsRiftBoss => GetAttribute<bool>(ActorAttributeType.IsLootRunBoss);

        public bool IsInvulnerable => GetAttribute<bool>(ActorAttributeType.Invulnerable);

        public bool IsBountyObjective => GetAttribute<bool>(ActorAttributeType.BountyObjective);

        public float Hitpoints => GetAttribute<float>(ActorAttributeType.HitpointsCur);

        public float HitpointsMax => GetAttribute<float>(ActorAttributeType.HitpointsMax);

        public float HitpointsMaxTotal => GetAttribute<float>(ActorAttributeType.HitpointsMaxTotal);

        public float HitpointsPct => HitpointsMax > 0 ? Hitpoints / HitpointsMax * 100 : 0;

        public bool IsGizmoBeenOperated => GetAttribute<bool>(ActorAttributeType.GizmoHasBeenOperated);

        public int GizmoOperatorACDId => GetAttribute<int>(ActorAttributeType.GizmoOperatorACDId);

        public bool IsChestOpen => GetAttribute<bool>(ActorAttributeType.ChestOpen);

        public int GizmoCharges => GetAttribute<int>(ActorAttributeType.GizmoCharges);

        public bool IsNoDamage => GetAttribute<bool>(ActorAttributeType.NoDamage);

        public bool IsDoorLocked => GetAttribute<bool>(ActorAttributeType.DoorLocked);

        public int TeamOverride => GetAttribute<int>(ActorAttributeType.TeamOverride);

        public int TeamId => GetAttribute<int>(ActorAttributeType.TeamId);

        public MarkerType MarkerType => GetAttribute<MarkerType>(ActorAttributeType.ConversationIcon, 0); // 483: ConversationIcon(-3613)

        public bool NpcHasInteractOptions => GetAttribute<bool>(ActorAttributeType.NPCHasInteractOptions, 0);

        public int SummonerId => GetAttribute<int>(ActorAttributeType.SummonerId);

        public bool IsNPC => GetAttribute<bool>(ActorAttributeType.IsNPC);

        public int SummonedByAnnId => GetAttribute<int>(ActorAttributeType.SummonedByACDId);

        public bool HasBuffVisualEffect => GetAttribute<bool>(ActorAttributeType.BuffVisualEffect);

        public bool IsQuestMonster => GetAttribute<bool>(ActorAttributeType.QuestMonster);

        //public PetType PetType => GetAttribute<PetType>(ActorAttributeType.PetType);

        public PetType PetType => GetAttribute<PetType>(ActorAttributeType.PetType, () => PetType.None);

        public bool IsGizmoDisabledByScript => GetAttribute<bool>(ActorAttributeType.GizmoDisabledByScript);

        //public bool IsDeletedOnServer => GetAttributeDirectlyFromTable<bool>(ActorAttributeType.DeletedOnServer);
        public bool IsDeletedOnServer => GetAttribute<bool>(ActorAttributeType.DeletedOnServer);

        public int LastDamageAnnId => GetAttribute<int>(ActorAttributeType.LastDamageACD);

        public bool IsDoorTimed => GetAttribute<int>(ActorAttributeType.DoorTimer) > 0;

        public int EffectOwnerAnnId => GetAttribute<int>(ActorAttributeType.EffectOwnerANN);


        #endregion

        #region Item Attributes

        public int GetTradePlayerLow(int index) => GetAttribute<int>(ActorAttributeType.ItemTradePlayerLow, index);
        public int GetTradePlayerHigh(int index) => GetAttribute<int>(ActorAttributeType.ItemTradePlayerHigh, index);

        public Dictionary<DamageType, float> DamageWeaponPercentTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponPercentTotal); // DamageWeaponPercentTotal (240) = i:1035489772 f:0.09 v:0.09 ModifierType=DamageType Modifier=0
        public Dictionary<SNOPower, float> Skills => GetAttributes<SNOPower, float>(ActorAttributeType.Skill); // Skill (125304932) = i:1 f:1.401298E-45 v:1 ModifierType=PowerSnoId Modifier=30592
        public Dictionary<SNOPower, float> ItemPowerPassives => GetAttributes<SNOPower, float>(ActorAttributeType.ItemPowerPassive); // ItemPowerPassive (1648481537) = i:1061326684 f:0.76 v:0.76 ModifierType=PowerSnoId Modifier=402461
        public Dictionary<int, float> Requirements => GetAttributes<int, float>(ActorAttributeType.Requirement); // Requirement (233853) = i:1116471296 f:70 v:70 ModifierType=1 Modifier=57
        public Dictionary<DamageType, float> Resistances => GetAttributes<DamageType, float>(ActorAttributeType.Resistance); // Resistance (8285) = i:1128333312 f:193 v:193 ModifierType=DamageType Modifier=2
        public Dictionary<DamageType, float> DamageWeaponMins => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMin); // DamageWeaponMin (16613) = i:1151246336 f:1269 v:1269 ModifierType=DamageType Modifier=4
        public Dictionary<DamageType, float> DamageWeaponAverages => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponAverage); // DamageWeaponAverage (232) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponMinTotalMainHands => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMinTotalMainHand); // DamageWeaponMinTotalMainHand (12822) = i:1155997696 f:1849 v:1849 ModifierType=DamageType Modifier=3
        public Dictionary<DamageType, float> DamageWeaponAverageTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponAverageTotal); // DamageWeaponAverageTotal (12521) = i:1156628480 f:1926 v:1926 ModifierType=DamageType Modifier=3
        public Dictionary<SNOPower, float> PowerDamagePercentBonuses => GetAttributes<SNOPower, float>(ActorAttributeType.PowerDamagePercentBonus); // PowerDamagePercentBonus (1332085920) = i:1038174126 f:0.11 v:0.11 ModifierType=PowerSnoId Modifier=325216
        public Dictionary<int, int> SetItemCounts => GetAttributes<int, int>(ActorAttributeType.SetItemCount); // SetItemCount (-1399073762) = i:1 f:1.401298E-45 v:1 ModifierType=17 Modifier=-341571
        public Dictionary<DamageType, float> DamageDealtPercentBonuses => GetAttributes<DamageType, float>(ActorAttributeType.DamageDealtPercentBonus); // DamageDealtPercentBonus (242) = i:1044549468 f:0.19 v:0.19 ModifierType=DamageType Modifier=0
        public Dictionary<ResourceType, float> ResourceMaxBonuses => GetAttributes<ResourceType, float>(ActorAttributeType.ResourceMaxBonus); // ResourceMaxBonus (28823) = i:1088421888 f:7 v:7 ModifierType=ResourceType Modifier=8
        public Dictionary<DamageType, float> DamageWeaponDeltas => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDelta); // DamageWeaponDelta (220) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponDeltaSubTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDeltaSubTotal); // DamageWeaponDeltaSubTotal (221) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponDeltaTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDeltaTotal); // DamageWeaponDeltaTotal (225) = i:1141891400 f:575.52 v:575.52 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponBonusDeltaX1s => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponBonusDeltaX1); // DamageWeaponBonusDeltaX1 (228) = i:1134362624 f:314 v:314 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponMinTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMinTotal); // DamageWeaponMinTotal (230) = i:1152306955 f:1398.47 v:1398.47 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponBonusMinX1s => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponBonusMinX1); // DamageWeaponBonusMinX1 (236) = i:1150484480 f:1176 v:1176 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageDeltaTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageDeltaTotal); // DamageDeltaTotal (211) = i:1120927744 f:104 v:104 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageMins => GetAttributes<DamageType, float>(ActorAttributeType.DamageMin); // DamageMin (212) = i:1119354880 f:92 v:92 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageMinTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageMinTotal); // DamageMinTotal (214) = i:1119354880 f:92 v:92 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageMinSubtotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageMinSubtotal); // DamageMinSubtotal (218) = i:1119354880 f:92 v:92 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponDeltaTotalMainHands => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDeltaTotalMainHand); // DamageWeaponDeltaTotalMainHand (16920) = i:1132986368 f:272 v:272 ModifierType=DamageType Modifier=4
        public Dictionary<DamageType, float> DamageDeltas => GetAttributes<DamageType, float>(ActorAttributeType.DamageDelta); // DamageDelta (210) = i:1120927744 f:104 v:104 ModifierType=DamageType Modifier=0
        public Dictionary<DamageType, float> DamageWeaponMaxTotals => GetAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMaxTotal); // 223: DamageWeaponMaxTotal(-3873) [DamageType: Poison: 4 ]  i:0 f:1825 Value=1825

        public float DamageWeaponMin => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponMin); // DamageWeaponMin (16613) = i:1151246336 f:1269 v:1269 ModifierType=DamageType Modifier=4
        public float DamageWeaponDelta => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponDelta); // DamageWeaponDelta (220) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0

        public float DamageWeaponMax => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponMax); // 223: DamageWeaponMaxTotal(-3873) [DamageType: Poison: 4 ]  i:0 f:1825 Value=1825
        public float DamageWeaponMaxTotal => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponMaxTotal); // 223: DamageWeaponMaxTotal(-3873) [DamageType: Poison: 4 ]  i:0 f:1825 Value=1825
        public float DamageDealtPercentBonus => GetFirstBaseAttribute<float>(ActorAttributeType.DamageDealtPercentBonus); // DamageDealtPercentBonus (242) = i:1044549468 f:0.19 v:0.19 ModifierType=DamageType Modifier=0
        public float DamageWeaponPercentTotal => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponPercentTotal); // DamageWeaponPercentTotal (240) = i:1035489772 f:0.09 v:0.09 ModifierType=DamageType Modifier=0
        public float DamageWeaponDeltaTotalMainHand => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponDeltaTotalMainHand); // DamageWeaponDeltaTotalMainHand (16920) = i:1132986368 f:272 v:272 ModifierType=DamageType Modifier=4
        public float DamageWeaponMinTotal => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponMinTotal); // DamageWeaponMinTotal (230) = i:1152306955 f:1398.47 v:1398.47 ModifierType=DamageType Modifier=0
        public float DamageWeaponBonusMinX1 => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponBonusMinX1); // 236: DamageWeaponBonusMinX1(-3860) ModKey=236 Mod=0 ModifierType=DamageType Value = 1591
        public float DamageWeaponBonusDeltaX1 => GetFirstBaseAttribute<float>(ActorAttributeType.DamageWeaponBonusDeltaX1); //228: DamageWeaponBonusDeltaX1(-3868) ModKey=228 Mod=0 ModifierType=DamageType Value = 385

        public float PotionBonusLifeOnKill => GetAttribute<float>(ActorAttributeType.PotionBonusLifeOnKill); // PotionBonusLifeOnKill (-2735) = i:1194553856 f:45938 v:45938 ModifierType=None Modifier=-1
        public float CritPercentBonusCapped => GetAttribute<float>(ActorAttributeType.CritPercentBonusCapped); // CritPercentBonusCapped (-3850) = i:1036831949 f:0.1 v:0.1 ModifierType=None Modifier=-1
        public float IntelligenceItem => GetAttribute<float>(ActorAttributeType.IntelligenceItem); // IntelligenceItem (-2898) = i:1147863040 f:940 v:940 ModifierType=None Modifier=-1
        public float ArmorItemTotal => GetAttribute<float>(ActorAttributeType.ArmorItemTotal); // ArmorItemTotal (-4059) = i:1141850112 f:573 v:573 ModifierType=None Modifier=-1
        public float ArmorItemSubTotal => GetAttribute<float>(ActorAttributeType.ArmorItemSubTotal); // ArmorItemSubTotal (-4060) = i:1141850112 f:573 v:573 ModifierType=None Modifier=-1
        public int ArmorItem => GetAttribute<int>(ActorAttributeType.ArmorItem); // ArmorItem (-4063) = i:1141855573 f:573.3333 v:573.3333 ModifierType=None Modifier=-1
        public int Vitality => GetAttribute<int>(ActorAttributeType.VitalityItem); // VitalityItem (-2897) = i:1147142144 f:896 v:896 ModifierType=None Modifier=-1
        public int Strength => GetAttribute<int>(ActorAttributeType.StrengthItem); // StrengthItem (-2900) = i:1148354560 f:970 v:970 ModifierType=None Modifier=-1
        public int Intelligence => GetAttribute<int>(ActorAttributeType.IntelligenceItem);
        public int Dexterity => GetAttribute<int>(ActorAttributeType.DexterityItem);
        public GemQuality GemQuality => GetAttribute<GemQuality>(ActorAttributeType.GemQuality);
        public float BlockAmountItemDelta => GetAttribute<float>(ActorAttributeType.BlockAmountItemDelta); // BlockAmountItemDelta (-3825) = i:1171529728 f:6788 v:6788 ModifierType=None Modifier=-1
        public float BlockAmountItemMin => GetAttribute<float>(ActorAttributeType.BlockAmountItemMin); // BlockAmountItemMin (-3826) = i:1185865728 f:22384 v:22384 ModifierType=None Modifier=-1
        public float BlockChanceItemTotal => GetAttribute<float>(ActorAttributeType.BlockChanceItemTotal); // BlockChanceItemTotal (-3833) = i:1047904256 f:0.2399902 v:0.2399902 ModifierType=None Modifier=-1
        public float BlockChanceItem => GetAttribute<float>(ActorAttributeType.BlockChanceItem); // BlockChanceItem (-3834) = i:1040515072 f:0.1298828 v:0.1298828 ModifierType=None Modifier=-1
        public float BlockChanceBonusItem => GetAttribute<float>(ActorAttributeType.BlockChanceBonusItem); // BlockChanceBonusItem (-3835) = i:1038172160 f:0.1099854 v:0.1099854 ModifierType=None Modifier=-1
        public float ResourceCostReductionPercentAll => GetAttribute<float>(ActorAttributeType.ResourceCostReductionPercentAll); // ResourceCostReductionPercentAll (-3364) = i:1028440064 f:0.04998779 v:0.04998779 ModifierType=None Modifier=-1
        public float AttacksPerSecondItemPercent => GetAttribute<float>(ActorAttributeType.AttacksPerSecondItemPercent) * 100; // AttacksPerSecondPercent (-3895) = i:1032805417 f:0.07 v:0.07 ModifierType=None Modifier=-1
        public float AttacksPerSecondPercent => GetAttribute<float>(ActorAttributeType.AttacksPerSecondPercent) * 100; // AttacksPerSecondPercent (-3895) = i:1032805417 f:0.07 v:0.07 ModifierType=None Modifier=-1
        public bool IsPrimalAncient => GetAttribute<int>(ActorAttributeType.AncientRank) == 2;
        public bool IsAncient => GetAttribute<int>(ActorAttributeType.AncientRank) > 0; // AncientRank (-3691) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public bool IsUnidentified => GetAttribute<bool>(ActorAttributeType.Unidentified); // AncientRank (-3691) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public int Post212Drop2 => GetAttribute<int>(ActorAttributeType.Post212Drop2); // Post212Drop2 (-3692) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public int Loot20Drop => GetAttribute<int>(ActorAttributeType.Loot20Drop); // Loot20Drop (-3694) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public int Seed => GetAttribute<int>(ActorAttributeType.Seed); // Seed (-3698) = i:-180189030 f:-2.466006E+32 v:-1.80189E+08 ModifierType=None Modifier=-1
        public int RequiredLevel => GetAttribute<int>(new AttributeKey((int)ActorAttributeType.Requirement, (int)RequirementType.EquipItem)); // ItemLegendaryItemLevelOverride (-3706) = i:70 f:9.809089E-44 v:70 ModifierType=None Modifier=-1

        public int ItemLegendaryItemLevelOverride => GetAttribute<int>(ActorAttributeType.ItemLegendaryItemLevelOverride);
        public int ItemBindingLevelOverride => GetAttribute<int>(ActorAttributeType.ItemBindingLevelOverride); // ItemBindingLevelOverride (-3707) = i:2 f:2.802597E-45 v:2 ModifierType=None Modifier=-1
        public int ItemBoundToACDId => GetAttribute<int>(ActorAttributeType.ItemBoundToACD); // ItemBoundToACD (-3709) = i:2014707801 f:1.216956E+34 v:2.014708E+09 ModifierType=None Modifier=-1
        public int Sockets => GetAttribute<int>(ActorAttributeType.Sockets); // Sockets (-3712) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public ItemQuality ItemQualityLevel => GetAttribute<ItemQuality>(ActorAttributeType.ItemQualityLevel); // ItemQualityLevel (-3720) = i:9 f:1.261169E-44 v:9 ModifierType=None Modifier=-1
        public int DurabilityMax => GetAttribute<int>(ActorAttributeType.DurabilityMax); // DurabilityMax (-3722) = i:286 f:4.007714E-43 v:286 ModifierType=None Modifier=-1
        public int DurabilityCur => GetAttribute<int>(ActorAttributeType.DurabilityCur); // DurabilityCur (-3723) = i:286 f:4.007714E-43 v:286 ModifierType=None Modifier=-1
        public float HitpointsOnHit => GetAttribute<float>(ActorAttributeType.HitpointsOnHit); // HitpointsOnHit (-3751) = i:1176764416 f:10496 v:10496 ModifierType=None Modifier=-1
        public int JewelRank => GetAttribute<int>(ActorAttributeType.JewelRank); // JewelRank (-2707) = i:50 f:7.006492E-44 v:50 ModifierType=None Modifier=-1
        public float OnHitFearProcChance => GetAttribute<float>(ActorAttributeType.OnHitFearProcChance); // OnHitFearProcChance (-2991) = i:1011769344 f:0.01259613 v:0.01259613 ModifierType=None Modifier=-1
        public float HitpointsOnKill => GetAttribute<float>(ActorAttributeType.HitpointsOnKill); // HitpointsOnKill (-3750) = i:1172201472 f:7116 v:7116 ModifierType=None Modifier=-1
        public float MovementScalar => GetAttribute<float>(ActorAttributeType.MovementScalar); // MovementScalar (-3927) = i:1038172160 f:0.1099854 v:0.1099854 ModifierType=None Modifier=-1
        public int ItemLegendaryItemBaseItem => GetAttribute<int>(ActorAttributeType.ItemLegendaryItemBaseItem); // ItemLegendaryItemBaseItem (-2746) = i:1146967350 f:885.3314 v:1.146967E+09 ModifierType=None Modifier=-1
        public float DamageWeaponPercentAll => GetAttribute<float>(ActorAttributeType.DamageWeaponPercentAll); // DamageWeaponPercentAll (-3857) = i:1035489772 f:0.09 v:0.09 ModifierType=None Modifier=-1
        public float DamageWeaponMinTotalAll => GetAttribute<float>(ActorAttributeType.DamageWeaponMinTotalAll); // DamageWeaponMinTotalAll (-3865) = i:1152306955 f:1398.47 v:1398.47 ModifierType=None Modifier=-1
        public float DamageWeaponDeltaTotalAll => GetAttribute<float>(ActorAttributeType.DamageWeaponDeltaTotalAll); // DamageWeaponDeltaTotalAll (-3870) = i:1141891400 f:575.52 v:575.52 ModifierType=None Modifier=-1
        public float AttacksPerSecondItemTotal => GetAttribute<float>(ActorAttributeType.AttacksPerSecondItemTotal); // AttacksPerSecondItemTotal (-3900) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1
        public float AttacksPerSecondItemSubtotal => GetAttribute<float>(ActorAttributeType.AttacksPerSecondItemSubtotal); // AttacksPerSecondItemSubtotal (-3902) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1
        public float AttacksPerSecondItem => GetAttribute<float>(ActorAttributeType.AttacksPerSecondItem); // AttacksPerSecondItem (-3904) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1
        public float AttacksPerSecondItemMainHand => GetAttribute<float>(ActorAttributeType.AttacksPerSecondItemMainHand); // AttacksPerSecondItemMainHand (-3566) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1
        public float DamageWeaponAverageTotalAll => GetAttribute<float>(ActorAttributeType.DamageWeaponAverageTotalAll); // DamageWeaponAverageTotalAll (-3862) = i:1154113536 f:1619 v:1619 ModifierType=None Modifier=-1
        public float DamageWeaponMaxTotalAll => GetAttribute<float>(ActorAttributeType.DamageWeaponMaxTotalAll); // DamageWeaponMaxTotalAll (-3872) = i:1156104192 f:1862 v:1862 ModifierType=None Modifier=-1
        public float SplashDamageEffectPercent => GetAttribute<float>(ActorAttributeType.SplashDamageEffectPercent); // SplashDamageEffectPercent (-2754) = i:1047233823 f:0.23 v:0.23 ModifierType=None Modifier=-1
        public float AttacksPerSecondItemTotalMainHand => GetAttribute<float>(ActorAttributeType.AttacksPerSecondItemTotalMainHand); // AttacksPerSecondItemTotalMainHand (-3564) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1
        public float HealthGlobeBonusHealth => GetAttribute<float>(ActorAttributeType.HealthGlobeBonusHealth); // HealthGlobeBonusHealth (-4010) = i:1191871744 f:35461 v:35461 ModifierType=None Modifier=-1
        public float GoldPickUpRadius => GetAttribute<float>(ActorAttributeType.GoldPickUpRadius); // GoldPickUpRadius (-3032) = i:1073741824 f:2 v:2 ModifierType=None Modifier=-1


        public float WeaponOnHitBlindProcChanceMainHand => GetAttribute<float>(ActorAttributeType.WeaponOnHitBlindProcChanceMainHand); // WeaponOnHitBlindProcChanceMainHand (-2948) = i:1050132480 f:0.2963867 v:0.2963867 ModifierType=None Modifier=-1
        public float WeaponOnHitBlindProcChance => GetAttribute<float>(ActorAttributeType.WeaponOnHitBlindProcChance); // WeaponOnHitBlindProcChance (-2966) = i:1050132480 f:0.2963867 v:0.2963867 ModifierType=None Modifier=-1
        public float PowerCooldownReductionPercentAll => GetAttribute<float>(ActorAttributeType.PowerCooldownReductionPercentAll); // PowerCooldownReductionPercentAll (-3888) = i:1032798208 f:0.06994629 v:0.06994629 ModifierType=None Modifier=-1
        public float SpendingResourceHealsPercent => GetAttribute<float>(ActorAttributeType.SpendingResourceHealsPercent); // SpendingResourceHealsPercent (28745) = i:1162813440 f:3314 v:3314 ModifierType=ResourceType Modifier=7
        public int TieredLootRunKeyLevel => GetAttribute<int>(ActorAttributeType.TieredLootRunKeyLevel); // TieredLootRunKeyLevel (-2712) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public float DamageAverageTotalAll => GetAttribute<float>(ActorAttributeType.DamageAverageTotalAll); // DamageAverageTotalAll (-3879) = i:1125122048 f:144 v:144 ModifierType=None Modifier=-1
        public float DamageMinTotalAll => GetAttribute<float>(ActorAttributeType.DamageMinTotalAll); // DamageMinTotalAll (-3880) = i:1119354880 f:92 v:92 ModifierType=None Modifier=-1
        public float DamageDeltaTotalAll => GetAttribute<float>(ActorAttributeType.DamageDeltaTotalAll); // DamageDeltaTotalAll (-3881) = i:1120927744 f:104 v:104 ModifierType=None Modifier=-1
        public float PotionBonusBuffDuration => GetAttribute<float>(ActorAttributeType.PotionBonusBuffDuration); // PotionBonusBuffDuration (-2737) = i:1084227584 f:5 v:5 ModifierType=None Modifier=-1
        public float PotionBonusArmorPercent => GetAttribute<float>(ActorAttributeType.PotionBonusArmorPercent); // PotionBonusArmorPercent (-2740) = i:1040522936 f:0.13 v:0.13 ModifierType=None Modifier=-1
        public float WeaponOnHitStunProcChanceMainHand => GetAttribute<float>(ActorAttributeType.WeaponOnHitStunProcChanceMainHand); // WeaponOnHitStunProcChanceMainHand (-2951) = i:1019510784 f:0.02398682 v:0.02398682 ModifierType=None Modifier=-1
        public float WeaponOnHitStunProcChance => GetAttribute<float>(ActorAttributeType.WeaponOnHitStunProcChance); // WeaponOnHitStunProcChance (-2967) = i:1019510784 f:0.02398682 v:0.02398682 ModifierType=None Modifier=-1
        public float WeaponOnHitFreezeProcChanceMainHand => GetAttribute<float>(ActorAttributeType.WeaponOnHitFreezeProcChanceMainHand); // WeaponOnHitFreezeProcChanceMainHand (-2945) = i:1014235136 f:0.01489258 v:0.01489258 ModifierType=None Modifier=-1
        public float WeaponOnHitFreezeProcChance => GetAttribute<float>(ActorAttributeType.WeaponOnHitFreezeProcChance); // WeaponOnHitFreezeProcChance (-2965) = i:1014235136 f:0.01489258 v:0.01489258 ModifierType=None Modifier=-1
        public int DyeType => GetAttribute<int>(ActorAttributeType.DyeType); // DyeType (-3695) = i:16 f:2.242078E-44 v:16 ModifierType=None Modifier=-1
        public int ItemIndestructible => GetAttribute<int>(ActorAttributeType.ItemIndestructible); // ItemIndestructible (-2893) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public int ItemLevelRequirementReduction => GetAttribute<int>(ActorAttributeType.ItemLevelRequirementReduction); // ItemLevelRequirementReduction (-2896) = i:39 f:5.465064E-44 v:39 ModifierType=None Modifier=-1
        public int ItemLevelRequirementOverride => GetAttribute<int>(ActorAttributeType.ItemLevelRequirementOverride); // ItemLevelRequirementOverride (-2895) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public float WeaponRanged => GetAttribute<float>(ActorAttributeType.WeaponRanged); // WeaponRanged (-3578) = i:1065353216 f:1 v:1 ModifierType=None Modifier=-1
        public float Weapon2H => GetAttribute<float>(ActorAttributeType.Weapon2H); // Weapon2H (-3580) = i:1065353216 f:1 v:1 ModifierType=None Modifier=-1
        public float DamagePercentReductionFromMelee => GetAttribute<float>(ActorAttributeType.DamagePercentReductionFromMelee); // DamagePercentReductionFromMelee (-2979) = i:1032805416 f:0.06999999 v:0.06999999 ModifierType=None Modifier=-1
        public bool IsVendorBought => GetAttribute<bool>(ActorAttributeType.IsVendorBought); // IsVendorBought (-3696) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public float CubeEnchantedStrengthItem => GetAttribute<float>(ActorAttributeType.CubeEnchantedStrengthItem); // CubeEnchantedStrengthItem (-3670) = i:1134723072 f:325 v:325 ModifierType=None Modifier=-1
        public int CubeEnchantedGemType => GetAttribute<int>(ActorAttributeType.CubeEnchantedGemType); // CubeEnchantedGemType (-3671) = i:3 f:4.203895E-45 v:3 ModifierType=None Modifier=-1
        public int CubeEnchantedGemRank => GetAttribute<int>(ActorAttributeType.CubeEnchantedGemRank); // CubeEnchantedGemRank (-3672) = i:65 f:9.10844E-44 v:65 ModifierType=None Modifier=-1
        public int EnchantedAffixCount => GetAttribute<int>(ActorAttributeType.EnchantedAffixCount); // EnchantedAffixCount (-3678) = i:9 f:1.261169E-44 v:9 ModifierType=None Modifier=-1
        public int EnchantedAffixSeed => GetAttribute<int>(ActorAttributeType.EnchantedAffixSeed); // EnchantedAffixSeed (-3679) = i:974478628 f:0.0005697778 v:9.744787E+08 ModifierType=None Modifier=-1
        public int EnchantedAffixNew => GetAttribute<int>(ActorAttributeType.EnchantedAffixNew); // EnchantedAffixNew (-3680) = i:-889190756 f:-8390300 v:-8.891908E+08 ModifierType=None Modifier=-1
        public int EnchantedAffixOld => GetAttribute<int>(ActorAttributeType.EnchantedAffixOld); // EnchantedAffixOld (-3681) = i:-415821016 f:-8.645464E+23 v:-4.15821E+08 ModifierType=None Modifier=-1
        public int ConsumableAddSockets => GetAttribute<int>(ActorAttributeType.ConsumableAddSockets); // ConsumableAddSockets (-3688) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public float HitpointsMaxPercentBonusItem => GetAttribute<float>(ActorAttributeType.HitpointsMaxPercentBonusItem); // HitpointsMaxPercentBonusItem (-3961) = i:1040515072 f:0.1298828 v:0.1298828 ModifierType=None Modifier=-1
        public bool IsCrafted => GetAttribute<bool>(ActorAttributeType.IsCrafted); // IsCrafted (-3697) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1
        public float ResistanceAll => GetAttribute<float>(ActorAttributeType.ResistanceAll); // ResistanceAll (-4000) = i:1120141312 f:98 v:98 ModifierType=None Modifier=-1
        public float DamagePercentReductionFromRanged => GetAttribute<float>(ActorAttributeType.DamagePercentReductionFromRanged); // DamagePercentReductionFromRanged (-2980) = i:1031127696 f:0.06 v:0.06 ModifierType=None Modifier=-1
        public float ArmorBonusItem => GetAttribute<float>(ActorAttributeType.ArmorBonusItem); // ArmorBonusItem (-4062) = i:1137082368 f:397 v:397 ModifierType=None Modifier=-1
        public float WeaponOnHitFearProcChance => GetAttribute<float>(ActorAttributeType.WeaponOnHitFearProcChance); // WeaponOnHitFearProcChance (-2968) = i:1037590528 f:0.1056519 v:0.1056519 ModifierType=None Modifier=-1

        public int ItemStackQuantity => _commonData != null && _commonData.IsValid && !_commonData.IsDisposed ? (int)_commonData.ItemStackQuantity : 0;
        //public int ItemStackQuantity => (int)((long)ItemStackQuantityHi << 32 | (uint)ItemStackQuantityLo);
        //public int ItemStackQuantityHi => GetAttribute<int>(ActorAttributeType.ItemStackQuantityHi);
        //public int ItemStackQuantityLo => GetAttribute<int>(ActorAttributeType.ItemStackQuantityLo); // ItemStackQuantityLo (-3702) = i:5000 f:7.006492E-42 v:5000 ModifierType=None Modifier=-1

        public int ItemTradeEndTime => GetAttribute<int>(ActorAttributeType.ItemTradeEndTime);
        public int SocketsFilled => GetAttribute<int>(ActorAttributeType.SocketsFilled);
        public int ArcaneDamage => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxArcane);
        public int ColdDamage => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxCold);
        public int FireDamage => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxFire);
        public int HolyDamage => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxHoly);
        public int LightningDamage => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxLightning);
        public int PoisonDamage => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxPoison);
        public int PhysicalDamageTotal => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxTotalPhysical);
        public int PhysicalDamageBase => GetAttribute<int>(ActorAttributeType.DamageWeaponMaxPhysical);
        public int PhysicalDamage => PhysicalDamageTotal - PhysicalDamageBase;
        public float DamageWeaponBonusMinPhysical => GetAttribute<float>(ActorAttributeType.DamageWeaponBonusMinPhysical);
        public float DamageDeltaPhysical => GetAttribute<float>(ActorAttributeType.DamageDeltaPhysical);
        public float LifeOnHit => GetAttribute<float>(ActorAttributeType.HitpointsOnHit);
        public float LifePercent => GetAttribute<float>(ActorAttributeType.HitpointsMaxPercentBonusItem);
        public float LifeStealPercent => GetAttribute<float>(ActorAttributeType.StealHealthPercent) * 100;
        public float HealthPerSecond => GetAttribute<float>(ActorAttributeType.HitpointsRegenPerSecond);
        public float MagicFindPercent => GetAttribute<float>(ActorAttributeType.MagicFind) * 100;
        public float GoldFindPercent => GetAttribute<float>(ActorAttributeType.GoldFind) * 100;
        public int MovementSpeedPercent => (int)Math.Round(GetAttribute<float>(ActorAttributeType.MovementScalar) * 100);
        public float PickUpRadius => GetAttribute<float>(ActorAttributeType.GoldPickUpRadius);
        public float CritPercent => GetAttribute<float>(ActorAttributeType.CritPercentBonusCapped) * 100;
        public float CritDamagePercent => GetAttribute<float>(ActorAttributeType.CritDamagePercent) * 100;
        public float BlockChancePercent => GetAttribute<float>(ActorAttributeType.BlockChanceItemTotal) * 100;
        public float Thorns => GetAttribute<float>(ActorAttributeType.ThornsFixedPhysical);
        public float ResourceCostReduction => GetAttribute<float>(ActorAttributeType.ResourceCostReductionPercentAll) * 100;
        public long ItemAssignedHero => (long)ItemAssignedHeroHi << 32 | (uint)ItemAssignedHeroLow;
        public int ItemAssignedHeroHi => GetAttribute<int>(ActorAttributeType.ItemAssignedHeroHigh);
        public int ItemAssignedHeroLow => GetAttribute<int>(ActorAttributeType.ItemAssignedHeroLow); // ItemAssignedHeroLow (-2720) = i:66795861 f:1.476562E-36 v:6.679586E+07 ModifierType=None Modifier=-1
        public int GoldAmount => GetAttribute<int>(ActorAttributeType.Gold); // Gold (-4047) = i:3717 f:5.208626E-42 v:3717 ModifierType=None Modifier=-1
        public float MinDamageTotal => DamageWeaponMinTotalAll;
        public float MaxDamageTotal => DamageWeaponMaxTotalAll;
        public float AttackSpeed => AttacksPerSecondItemTotal;
        public float DamagePerSecond => DamageWeaponAverageTotalAll * AttacksPerSecondItemTotal;
        public float WeaponDamagePercent => DamageWeaponPercentTotal * 100;
        public float AttackSpeedBonusPercent => AttacksPerSecondPercent * 100;
        public int ResourceCostReductionPercent => (int)Math.Round(ResourceCostReductionPercentAll * 100);
        public float ResistAll => GetAttribute<float>(ActorAttributeType.ResistanceAll);
        public float ResistArcane => GetAttribute<float>(ActorAttributeType.ResistanceArcane);
        public float ResistCold => GetAttribute<float>(ActorAttributeType.ResistanceCold);
        public float ResistFire => GetAttribute<float>(ActorAttributeType.ResistanceFire);
        public float ResistHoly => GetAttribute<float>(ActorAttributeType.ResistanceHoly);
        public float ResistLightning => GetAttribute<float>(ActorAttributeType.ResistanceLightning);
        public float ResistPhysical => GetAttribute<float>(ActorAttributeType.ResistancePhysical);
        public float ResistPoison => GetAttribute<float>(ActorAttributeType.ResistancePoison);
        public float AreaDamagePercent => SplashDamageEffectPercent * 100;
        public int CooldownPercent => (int)Math.Round(PowerCooldownReductionPercentAll * 100);
        public float FireSkillDamagePercentBonus => GetAttribute<float>(ActorAttributeType.DamageDealtPercentBonusFire) * 100;
        public float ColdSkillDamagePercentBonus => GetAttribute<float>(ActorAttributeType.DamageDealtPercentBonusCold) * 100;
        public float LightningSkillDamagePercentBonus => GetAttribute<float>(ActorAttributeType.DamageDealtPercentBonusLightning) * 100;
        public float ArcaneSkillDamagePercentBonus => GetAttribute<float>(ActorAttributeType.DamageDealtPercentBonusArcane) * 100;
        public float HolySkillDamagePercentBonus => GetAttribute<float>(ActorAttributeType.DamageDealtPercentBonusHoly) * 100;
        public float PoisonSkillDamagePercentBonus => GetAttribute<float>(ActorAttributeType.DamageDealtPercentBonusPoison) * 100;
        public float PhysicalSkillDamagePercentBonus => GetAttribute<float>(ActorAttributeType.DamageDealtPercentBonusPhysical) * 100;
        public float DamageAgainstElites => GetAttribute<float>(ActorAttributeType.DamagePercentBonusVsElites);
        public float DamageFromElites => GetAttribute<float>(ActorAttributeType.DamagePercentReductionFromElites);
        public float WrathRegen => GetAttribute<float>(ActorAttributeType.ResourceRegenPerSecondFaith);
        public float MaximumWrath => GetAttribute<float>(ActorAttributeType.ResourceMaxBonusFaith);
        public float ChanceToFreeze => GetAttribute<float>(ActorAttributeType.OnHitFreezeProcChance);
        public float ChanceToBlind => GetAttribute<float>(ActorAttributeType.OnHitBlindProcChance);
        public float ChanceToImmobilize => GetAttribute<float>(ActorAttributeType.OnHitImmobilizeProcChance);
        public float ChanceToStun => GetAttribute<float>(ActorAttributeType.OnHitStunProcChance);
        public float MaxDiscipline => GetAttribute<float>(ActorAttributeType.ResourceMaxBonus, (int)ResourceType.Discipline);
        public float MaxMana => GetAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusMana);
        public float MaxArcanePower => GetAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusArcanum);
        public float MaxSpirit => GetAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusSpirit);
        public float MaxFury => GetAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusFury);
        public float MaxWrath => GetAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusFaith);
        public float HatredRegen => GetAttribute<float>(ActorAttributeType.ResourceRegenPerSecondHatred);
        public float SpiritRegen => GetAttribute<float>(ActorAttributeType.ResourceRegenPerSecondSpirit);
        public float ManaRegen => GetAttribute<float>(ActorAttributeType.ResourceRegenBonusPercentMana);
        public float LifePerSpirit => GetAttribute<float>(ActorAttributeType.SpendingResourceHealsPercentSpirit);
        public float LifePerWrath => GetAttribute<float>(ActorAttributeType.SpendingResourceHealsPercentFaith);
        public float LifePerFury => GetAttribute<float>(ActorAttributeType.SpendingResourceHealsPercentFury);
        public float ArcaneOnCrit => GetAttribute<float>(ActorAttributeType.ResourceOnCritArcanum);
        public float MaxEssence => GetAttribute<float>(ActorAttributeType.ResourceMaxBonus, (int)ResourceType.Essence);

        public int Gold => GetAttribute<int>(ActorAttributeType.Gold);

        public int BlockChanceBonusPercent => (int)Math.Round(BlockChanceBonusItem * 100, MidpointRounding.AwayFromZero);

        public int PrimaryStat => GetPrimaryAttribute();

        public int GetPrimaryAttribute()
        {
            if (Strength > 0) return Strength;
            if (Intelligence > 0) return Intelligence;
            if (Dexterity > 0) return Dexterity;
            return 0;
        }

        public int GetElementalDamage(DamageType damageType)
        {
            var key = new AttributeKey((int)ActorAttributeType.DamageDealtPercentBonus, (int)damageType);
            return (int)Math.Round(GetAttribute<float>(key) * 100, MidpointRounding.AwayFromZero);
        }

        public float SkillDamagePercent(SNOPower power)
        {
            var key = new AttributeKey((int)ActorAttributeType.PowerDamagePercentBonus, (int)power);
            return GetAttribute<float>(key);
        }

        public float MaxDamage
        {
            get
            {
                var minAttr = GetAttributeEntry(ActorAttributeType.DamageWeaponMin);
                if (minAttr == null)
                    return 0f;

                var min = minAttr.GetValue<float>();
                var damageTypeId = (int)minAttr.Key.ModifierId;
                var delta = GetAttribute<float>(ActorAttributeType.DamageWeaponDeltaSubTotal, damageTypeId);
                return min + delta;
            }
        }

        public float MinDamage => DamageWeaponMin;

        #endregion

        /// <summary>
        /// If attributes can be read
        /// </summary>
        public bool IsValid => _commonData != null && _commonData.IsValid && !_commonData.IsDisposed || _table != null && _table.IsValid;

        /// <summary>
        /// Updates native objects to prevent memory reads
        /// </summary>
        public void Destroy()
        {
            _commonData = null;
            _table?.UpdatePointer(IntPtr.Zero);
        }

        public override string ToString()
            => Items.Aggregate($"Attributes {Environment.NewLine}", (c, a) => c + $" {a} {Environment.NewLine}");

    }

}