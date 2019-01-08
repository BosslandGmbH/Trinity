using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Components.Coroutines;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Modules
{
    /// <summary>
    /// Creates a list of good target candidates (removing all the rubbish actors) to reduce combat processing time for weighting and target selection.
    /// </summary>
    public class TargetsCache : Module, IEnumerable<TrinityActor>
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        public ulong LastUpdatedTick;
        public List<TrinityActor> Entries = new List<TrinityActor>();
        public List<TrinityActor> Ignored = new List<TrinityActor>();
        public ILookup<TrinityObjectType, TrinityActor> ByType = EmptyLookup<TrinityObjectType, TrinityActor>.Instance;
        public ILookup<MonsterQuality, TrinityActor> ByMonsterQuality = EmptyLookup<MonsterQuality, TrinityActor>.Instance;

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            using (new PerformanceLogger("ActorCache.Update"))
            {
                if (!ZetaDia.IsInGame ||
                    ZetaDia.Service.Hero == null ||
                    !ZetaDia.Service.Hero.IsValid)
                {
                    return;
                }

                if (ZetaDia.Me == null ||
                    !ZetaDia.Me.IsValid)
                {
                    return;
                }

                var lastUpdatedTick = ZetaDia.Memory.Executor.FrameCount;
                if (LastUpdatedTick == lastUpdatedTick)
                    return;

                LastUpdatedTick = lastUpdatedTick;

                var included = new List<TrinityActor>();
                var ignored = new List<TrinityActor>();

                foreach (TrinityActor actor in Core.Actors.OfType<TrinityActor>())
                {
                    try
                    {
                        actor.CacheInfo = string.Empty;

                        if (ShouldTargetActor(actor))
                        {
                            actor.TargetCategory = TargetCategory.Normal;
                            included.Add(actor);
                        }
                        else
                        {
                            actor.TargetCategory = TargetCategory.Ignore;
                            ignored.Add(actor);
                        }
                    }
                    catch (Exception ex)
                    {
                        s_logger.Error($"[{nameof(Update)}] Error during actor update {actor.Name} {actor.ActorSnoId}", ex);
                        Clear();
                        return;
                    }
                }

                ByType = included.ToLookup(k => k.Type);
                ByMonsterQuality = included.ToLookup(k => k.MonsterQuality);

                Entries = included;
                Ignored = ignored;
            }
        }

        private bool ShouldTargetActor(TrinityActor cacheObject)
        {
            if (cacheObject == null)
            {
                s_logger.Debug($"[{nameof(ShouldTargetActor)}] IGNORE: NullObject");
                return false;
            }

            if (cacheObject.AcdId == -1)
            {
                s_logger.Verbose($"[{nameof(ShouldTargetActor)}] IGNORE: InvalidCommonData");
                return false;
            }

            if (!ShouldIncludeCommon(cacheObject))
                return false;

            if (cacheObject.ActorType == ActorType.ClientEffect)
            {
                var result = cacheObject.IsAllowedClientEffect;
                s_logger.Verbose($"[{nameof(ShouldTargetActor)}] {(result ? "TARGET" : "IGNORE")}: ClientEffect");
                return result;
            }

            if (cacheObject is TrinityItem item)
            {
                var realItem = item.ToAcdItem();
                return realItem.GetIsGold() ?
                    ShouldIncludeGold(realItem) :
                    ShouldIncludeItem(realItem);
            }

            if (cacheObject.ToDiaObject() is DiaGizmo)
                return ShouldIncludeGizmo(cacheObject);

            switch (cacheObject.Type)
            {
                case TrinityObjectType.Player:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.BuffedRegion:
                case TrinityObjectType.Gate:
                case TrinityObjectType.BloodShard:
                    s_logger.Debug($"[{nameof(ShouldTargetActor)}] TARGET: Player, PowerGlobe, HealthGlobe, ProgressionGlobe, BuffedRegion, Gate, BloodShard");
                    return true;

                case TrinityObjectType.Unit:
                    return ShouldIncludeUnit(cacheObject);

                case TrinityObjectType.Avoidance:
                    s_logger.Verbose($"[{nameof(ShouldTargetActor)}] IGNORE: Avoidance");
                    return false;

                case TrinityObjectType.Environment:
                    s_logger.Verbose($"[{nameof(ShouldTargetActor)}] IGNORE: Environment");
                    return false;

                case TrinityObjectType.Banner:
                    s_logger.Verbose($"[{nameof(ShouldTargetActor)}] IGNORE: Banner");
                    return false;

                case TrinityObjectType.Destructible:
                case TrinityObjectType.Door:
                case TrinityObjectType.Barricade:
                case TrinityObjectType.Container:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Interactable:
                case TrinityObjectType.HealthWell:
                case TrinityObjectType.CursedChest:
                case TrinityObjectType.CursedShrine:
                    return ShouldIncludeGizmo(cacheObject);

                default:
                    var objectType = cacheObject.ToDiaObject()?.GetType().FullName;
                    s_logger.Debug($"[{nameof(ShouldTargetActor)}] IGNORE: Unhandled TrinityObjectType - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}, ObjectType: \"{objectType}\", TrinityObjectType: \"{cacheObject.Type}\"");
                    return false;
            }
        }

        private static bool IsCorpulent(TrinityActor cacheObject)
        {
            switch ((SNOActor)cacheObject.ActorSnoId)
            {
                case SNOActor.Corpulent_A:
                case SNOActor.Corpulent_A_Unique_01:
                case SNOActor.Corpulent_A_Unique_02:
                case SNOActor.Corpulent_A_Unique_03:
                case SNOActor.Corpulent_B:
                case SNOActor.Corpulent_B_Unique_01:
                case SNOActor.Corpulent_C:
                case SNOActor.Corpulent_C_OasisAmbush_Unique:
                case SNOActor.Corpulent_D:
                case SNOActor.Corpulent_D_CultistSurvivor_Unique:
                case SNOActor.Corpulent_D_Unique_Spec_01:
                case SNOActor.Corpulent_Frost_A:
                case SNOActor.Corpulent_suicide_blood:
                case SNOActor.Corpulent_suicide_frost:
                case SNOActor.Corpulent_suicide_imps:
                case SNOActor.Corpulent_suicide_spiders:
                    return true;
            }

            return false;
        }

        private static bool ShouldIncludeCommon(TrinityActor cacheObject)
        {
            if (cacheObject.IsExcludedId && !(ClearArea.IsClearing && cacheObject.IsHostile))
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: ExcludedId - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.IsExcludedType)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: ExcludedType - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (!cacheObject.IsValid)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: Invalid - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.IsProfileBlacklisted)
            {
                if (!GameData.IsCursedChestOrShrine.Contains(cacheObject.ActorSnoId))
                {
                    s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: BlacklistedByProfile - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                    return false;
                }
            }

            if (cacheObject is TrinityItem item && item.ToAcdItem().GetIsCosmeticItem())
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] INCLUDE: Cosmetic - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return true;
            }

            if (cacheObject.IsUnit && cacheObject.Attributes == null)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: Unit No Attributes - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.IsUntargetable)
            {
                // Include corpulents even when they are untargetable otherwise it messes up the avoidance.
                if (IsCorpulent(cacheObject))
                    return true;

                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: Untargetable - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            var isQuestGiverOutsideTown = cacheObject.IsQuestGiver && !Core.Player.IsInTown;
            if (cacheObject.IsNpc &&
                !isQuestGiverOutsideTown &&
                !cacheObject.IsBoss)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: NPC - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.IsDead)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: IsDead - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.IsObstacle)
            {
                if (cacheObject.IsGizmo && cacheObject.IsUsed)
                {
                    s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: Used Gizmo - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                    return false;
                }

                if (cacheObject.IsMonster && !GameData.CorruptGrowthIds.Contains(cacheObject.ActorSnoId))
                {
                    s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: Monster Obstacle - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                    return false;
                }
            }

            if (cacheObject.IsBlacklisted)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: Blacklisted - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.ZDiff > ZDiffLimit(cacheObject))
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: ZDiffLimit - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (!cacheObject.IsInLineOfSight &&
                !ShouldIgnoreLoS(cacheObject))
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeCommon)}] IGNORE: No Line of Sight - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            return true;
        }

        private static bool ShouldIgnoreLoS(TrinityActor cacheObject)
        {
            if (cacheObject.IsMinimapActive)
                return true;

            if (cacheObject.IsNpc &&
                cacheObject.IsQuestGiver)
            {
                return true;
            }

            switch (cacheObject.Type)
            {
                case TrinityObjectType.Shrine:
                    if (cacheObject.RadiusDistance < 40f ||
                        cacheObject.IsWalkable)
                    {
                        return true;
                    }

                    break;

                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.BuffedRegion:
                    return true;

                case TrinityObjectType.Door:
                    if (cacheObject.RadiusDistance < 15f)
                        return true;

                    break;

                case TrinityObjectType.Barricade:
                case TrinityObjectType.Destructible:
                    if (Core.BlockedCheck.MoveSpeed < 5f &&
                        cacheObject.Distance < 10f)
                    {
                        return true;
                    }

                    break;

                case TrinityObjectType.Unit:
                    if (TrinityCombat.CombatMode == CombatMode.KillAll &&
                        cacheObject.IsWalkable)
                    {
                        return true;
                    }

                    if (cacheObject.IsElite &&
                        cacheObject.Distance < 40f ||
                        cacheObject.IsWalkable)
                    {
                        return true;
                    }

                    if (cacheObject.IsTreasureGoblin)
                        return true;

                    break;
            }

            if (cacheObject.Distance < 4)
                return true;

            if (cacheObject is TrinityItem i &&
                i.ToAcdItem().ItemQualityLevel >= ItemQuality.Legendary)
                return true;

            if (GameData.LineOfSightWhitelist.Contains(cacheObject.ActorSnoId))
                return true;

            return false;
        }

        private bool ShouldIncludeUnit(TrinityActor cacheObject)
        {
            //TODO: Uncomment to ignore juggernauts
            //if (cacheObject.CommonData.AffixIds.Contains(-464468964))
            //    return false;
            
            if (cacheObject.MonsterRace == MonsterRace.Unknown)
            {
                s_logger.Debug($"[{nameof(ShouldIncludeUnit)}] IGNORE: MonsterRace == Unknown");
                return false;
            }

            if (!cacheObject.IsQuestGiver)
            {
                if (cacheObject.IsSameTeam)
                {
                    s_logger.Verbose($"[{nameof(ShouldIncludeUnit)}] IGNORE: IsSameTeam");
                    return false;
                }

                if (cacheObject.IsNoDamage)
                {
                    s_logger.Debug($"[{nameof(ShouldIncludeUnit)}] IGNORE: IsNoDamage");
                    return false;
                }

                if (cacheObject.IsFriendly)
                {
                    s_logger.Debug($"[{nameof(ShouldIncludeUnit)}] IGNORE: IsFriendly");
                    return false;
                }

                if (cacheObject.IsInvulnerable && !cacheObject.IsElite)
                {
                    // Include corpulents even when they are invulnerable otherwise it messes up the avoidance.
                    if (IsCorpulent(cacheObject))
                    {
                        s_logger.Debug($"[{nameof(ShouldIncludeUnit)}] INCLUDE: IsCorpulent");
                        return true;
                    }
                    
                    s_logger.Verbose($"[{nameof(ShouldIncludeUnit)}] IGNORE: IsInvulnerable");
                    return false;
                }
            }

            if (cacheObject.IsSummonedByPlayer)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeUnit)}] IGNORE: SummonedByPlayer");
                return false;
            }

            if (cacheObject.IsDead)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeUnit)}] IGNORE: IsDead");
                return false;
            }

            s_logger.Verbose($"[{nameof(ShouldIncludeUnit)}] INCLUDE: Default");
            return true;
        }

        private bool ShouldIncludeGold(ACDItem item)
        {
            if (item == null)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeGold)}] IGNORE: NullItem");
                return false;
            }

            if (!Core.Settings.Items.PickupGold)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeGold)}] IGNORE: GoldPickupDisabled - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.GetGoldAmount() < Core.Settings.Items.MinGoldStack)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeGold)}] IGNORE: MinGoldStack - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            return true;
        }

        private bool ShouldIncludeItem(ACDItem item)
        {
            if (item == null)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeItem)}] IGNORE: NullItem");
                return false;
            }

            if (item.GetIsPickupNoClick() && TrinityCombat.Loot.IsBackpackFull)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeItem)}] IGNORE: Backpack is Full - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.GetIsMyDroppedItem())
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeItem)}] IGNORE: Dropped Item - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.GetIsUntargetable())
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeItem)}] IGNORE: Untargetable - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (!item.GetIsCosmeticItem() &&
                item.ItemQualityLevel <= ItemQuality.Rare4 &&
                item.Distance > CharacterSettings.Instance.LootRadius)
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeItem)}] IGNORE: OutOfRange {CharacterSettings.Instance.LootRadius} - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (!TrinityCombat.Loot.ShouldPickup(item))
            {
                s_logger.Verbose($"[{nameof(ShouldIncludeItem)}] IGNORE: LootProvider.ShouldPickup - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            return true;
        }

        private bool ShouldIncludeGizmo(TrinityActor cacheObject)
        {
            if (cacheObject.IsUsed)
            {
                s_logger.Debug($"[{nameof(ShouldIncludeGizmo)}] IGNORE: Used Gizmo - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.IsInteractWhitelisted)
            {
                s_logger.Debug($"[{nameof(ShouldIncludeGizmo)}] INTERACT: Interact Whitelist - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return true;
            }

            if (GameData.ForceDestructibles.Contains(cacheObject.ActorSnoId))
            {
                s_logger.Debug($"[{nameof(ShouldIncludeGizmo)}] INTERACT: Force Destructibles - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return true;
            }

            if (TrinityTownRun.IsVendoring && cacheObject.Distance > 10f)
            {
                s_logger.Debug($"[{nameof(ShouldIncludeGizmo)}] IGNORE: Want to Town Run - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            //TODO: Why are we ignoreing doors????
            if (GameData.DoorsToAlwaysIgnore.Contains(cacheObject.ActorSnoId))
            {
                s_logger.Debug($"[{nameof(ShouldIncludeGizmo)}] IGNORE: Always Ignore Door - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (GameData.SceneSpecificDoorsIgnore.ContainsKey(Core.Player.CurrentSceneSnoId) &&
                GameData.SceneSpecificDoorsIgnore[Core.Player.CurrentSceneSnoId] == cacheObject.ActorSnoId)
            {
                s_logger.Debug($"[{nameof(ShouldIncludeGizmo)}] IGNORE: Scene Specific Ignore Door - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            if (cacheObject.IsDestroyable &&
                !cacheObject.HasBeenWalkable &&
                cacheObject.Distance > 5f &&
                cacheObject.GizmoType != GizmoType.BreakableChest)
            {
                s_logger.Debug($"[{nameof(ShouldIncludeGizmo)}] IGNORE: Cant Reach Destructible - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
                return false;
            }

            s_logger.Verbose($"[{nameof(ShouldIncludeGizmo)}] INTERACT: Default - Item: \"{cacheObject?.Name}\", InternalName: \"{cacheObject?.InternalName}\", Sno: 0x{cacheObject?.ActorSnoId:x8}, GBId: 0x{cacheObject?.GameBalanceId:x8}");
            return true;
        }

        public static float ZDiffLimit(TrinityActor cacheObject)
        {
            var defaultValue = 50f;

            if (GameData.LineOfSightWhitelist.Contains(cacheObject.ActorSnoId))
                return defaultValue;

            if (cacheObject.IsLastTarget)
                return defaultValue;

            if (cacheObject.IsBoss)
                return defaultValue;

            if (cacheObject.IsElite)
                return defaultValue;

            switch (cacheObject.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                    return defaultValue;
                case TrinityObjectType.Avoidance:
                case TrinityObjectType.Door:
                case TrinityObjectType.Unit:
                case TrinityObjectType.Barricade:
                    return 14;
                case TrinityObjectType.Item:
                case TrinityObjectType.HealthWell:
                    return 26;
                case TrinityObjectType.Gold:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.PowerGlobe:
                    return 11;
                case TrinityObjectType.Destructible:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Container:
                    return 7;
                case TrinityObjectType.Interactable:
                    return 9;
            }

            return defaultValue;
        }

        public void Clear()
        {
            Entries.Clear();
            Ignored.Clear();
        }

        public IEnumerator<TrinityActor> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
