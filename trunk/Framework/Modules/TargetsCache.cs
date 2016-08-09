using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Modules
{
    /// <summary>
    /// Creates a list of good target candidates (removing all the rubbish actors) to reduce combat processing time for weighting and target selection.
    /// </summary>
    public class TargetsCache : Module
    {

        /// <summary>
        /// Contains an RActorGUID and count of the number of times we've switched to this target
        /// todo evaluate, temporary placement here
        /// </summary>
        internal Dictionary<string, TargettingInfo> TargetHistory { get; } = new Dictionary<string, TargettingInfo>();

        /// <summary>
        /// How many times the player tried to interact with this object in total
        /// todo evaluate, temporary placement here
        /// </summary>
        internal Dictionary<int, int> InteractAttempts { get; } = new Dictionary<int, int>();

        public ulong LastUpdatedTick;
        public List<TrinityActor> Items = new List<TrinityActor>();
        public List<TrinityActor> Ignored = new List<TrinityActor>();
        public ILookup<TrinityObjectType, TrinityActor> ByType = EmptyLookup<TrinityObjectType, TrinityActor>.Instance;
        public ILookup<MonsterQuality, TrinityActor> ByMonsterQuality = EmptyLookup<MonsterQuality, TrinityActor>.Instance;

        private static void PreUpdateTasks()
        {
            TrinityPlugin.LastRefreshedCache = DateTime.UtcNow;
           TrinityPlugin.LastTargetPosition = TrinityPlugin.CurrentTarget != null ? TrinityPlugin.CurrentTarget.Position : Vector3.Zero;

            if (TrinityPlugin.CurrentTarget != null)
                TrinityPlugin.LastTargetRactorGUID = TrinityPlugin.CurrentTarget.RActorId;

            TrinityPlugin.LastTargetAcdId = CombatBase.CurrentTarget != null ? CombatBase.CurrentTarget.AcdId : -1;
        }

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            using (new PerformanceLogger("ActorCache.Update"))
            {
                if (!ZetaDia.IsInGame || ZetaDia.Service.Hero == null || !ZetaDia.Service.Hero.IsValid)
                    return;

                if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                    return;

                var lastUpdatedTick = ZetaDia.Memory.Executor.FrameCount;
                if (LastUpdatedTick == lastUpdatedTick) return;
                LastUpdatedTick = lastUpdatedTick;

                var included = new List<TrinityActor>();
                var ignored = new List<TrinityActor>();
                var timer = new Stopwatch();

                PreUpdateTasks();

                foreach (var actor in Core.Actors.GetActorsOfType<TrinityActor>())
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
                        Logger.LogError($"Exception updating object cache {actor.Name} {actor.ActorSnoId} {ex}");
                        Clear();
                        return;
                    }
                }

                if (Core.Settings.Advanced.LogCategories.HasFlag(LogCategory.CacheManagement))
                {
                    foreach (var o in included)
                    {
                        Logger.LogDebug($"[Cache][{o.CacheTime:N4}] Added {o}");
                    }

                    foreach (var o in ignored)
                    {
                        Logger.LogDebug($"[Cache][{o.CacheTime:N4}] Ignored {o.InternalName} RActorGuid={o.RActorId} ActorSnoId={o.ActorSnoId} Type={o.ActorType} CacheInfo={o.CacheInfo}");
                    }                   
                }

                CombatManager.Weighting.RefreshDiaGetWeights(included);

                ByType = included.ToLookup(k => k.Type);
                ByMonsterQuality = included.ToLookup(k => k.MonsterQuality);

                Items = included;
                Ignored = ignored;
            }
        }

        private bool ShouldTargetActor(TrinityActor cacheObject)
        {
            if (cacheObject == null)
            {
                Logger.LogError("NullObject");
                return false;
            }

            if (!ShouldCacheCommon(cacheObject))
                return false;

            switch (cacheObject.ActorType)
            {
                case ActorType.ClientEffect:
                    if (cacheObject.IsAllowedClientEffect)
                        return true;

                    cacheObject.AddCacheInfo("IgnoreClientEffect");
                    return false;

                default:
                    if (cacheObject.AcdId == -1)
                    {
                        cacheObject.AddCacheInfo("InvalidCommonData");
                        return false;
                    }
                    break;
            }

            switch (cacheObject.Type)
            {
                case TrinityObjectType.Player:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.BuffedRegion:
                case TrinityObjectType.Gate:
                case TrinityObjectType.BloodShard:
                    return true;

                case TrinityObjectType.Unit:
                    return ShouldCacheUnit(cacheObject);

                case TrinityObjectType.Item:
                    return ShouldCacheItem(cacheObject as TrinityItem);

                case TrinityObjectType.Gold:
                    return ShouldIncludeGold(cacheObject as TrinityItem);

                case TrinityObjectType.Avoidance:
                    cacheObject.AddCacheInfo("Avoidance");
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
                    return ShouldCacheGizmo(cacheObject);

                default:
                    cacheObject.AddCacheInfo($"Unhandled TrinityObjectType");
                    return false;
            }
        }

        private static bool ShouldCacheCommon(TrinityActor cacheObject)
        {
            if (cacheObject.IsExcludedId)
            {
                cacheObject.AddCacheInfo("ExcludedId");
                return false;
            }

            if (cacheObject.IsExcludedType)
            {
                cacheObject.AddCacheInfo("ExcludedType");
                return false;
            }

            if (!cacheObject.IsValid)
            {
                cacheObject.AddCacheInfo("Invalid");
                return false;
            }

            if (cacheObject.IsUnit && cacheObject.Attributes == null)
            {
                cacheObject.AddCacheInfo("UnitNoAttributes");
                return false;
            }

            if (cacheObject.IsUntargetable)
            {
                cacheObject.AddCacheInfo("Untargetable");
                return false;
            }

            var isQuestGiverOutsideTown = cacheObject.IsQuestGiver && !Core.Player.IsInTown;
            if (cacheObject.IsNpc && !isQuestGiverOutsideTown && !cacheObject.IsBoss)
            {
                cacheObject.AddCacheInfo("Npc");
                return false;
            }

            if (cacheObject.IsDead)
            {
                cacheObject.AddCacheInfo("Dead");
                return false;
            }

            if (cacheObject.IsObstacle)
            {
                if (cacheObject.IsGizmo && cacheObject.IsUsed)
                {
                    cacheObject.AddCacheInfo("UsedGizmoObstacle");
                    return false;
                }

                if (cacheObject.IsMonster && !DataDictionary.CorruptGrowthIds.Contains(cacheObject.ActorSnoId))
                {
                    cacheObject.AddCacheInfo("MonsterObstacle");
                    return false;
                }
            }

            if (cacheObject.IsBlacklisted)
            {
                cacheObject.AddCacheInfo("Blacklisted " + cacheObject.ObjectHash);
                return false;
            }

            if (cacheObject.ZDiff > ZDiffLimit(cacheObject))
            {
                cacheObject.AddCacheInfo("ZDiff");
                return false;
            }

            if (!cacheObject.HasBeenInLoS && !ShouldIgnoreLoS(cacheObject))
            {
                cacheObject.AddCacheInfo("HasntBeenInLoS");
                return false;
            }

            return true;
        }

        private static bool ShouldIgnoreLoS(TrinityActor cacheObject)
        {
            if (cacheObject.IsMinimapActive)
                return true;

            if (cacheObject.IsNpc && cacheObject.IsQuestGiver)
                return true;

            switch (cacheObject.Type)
            {
                case TrinityObjectType.Shrine:
                    if (cacheObject.RadiusDistance < 40f || cacheObject.IsWalkable)
                        return true;
                    break;

                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.BuffedRegion:
                        return true;
                           
                case TrinityObjectType.Door:
                    if (cacheObject.RadiusDistance < 15f)
                        return true;
                    break;

                case TrinityObjectType.Unit:
                    if (CombatBase.CombatMode == CombatMode.KillAll && cacheObject.IsWalkable)
                        return true;
                    if (cacheObject.IsElite && cacheObject.Distance < 40f || cacheObject.IsWalkable)
                        return true;
                    if (cacheObject.IsTreasureGoblin)
                        return true;
                    break;
            }

            if (cacheObject.Distance < 4) return true;
            if (cacheObject.ItemQualityLevel >= ItemQuality.Legendary) return true;
            if (DataDictionary.LineOfSightWhitelist.Contains(cacheObject.ActorSnoId)) return true;
            return false;
        }

        private bool ShouldCacheUnit(TrinityActor cacheObject)
        {
            if (cacheObject.IsSummonedByPlayer)
            {               
                UpdatePlayerSummonCounts(cacheObject);
                cacheObject.AddCacheInfo("SummonedByPlayer");
                return false;
            }
            if (cacheObject.IsSameTeam && !cacheObject.IsQuestGiver)
            {
                cacheObject.AddCacheInfo("SameTeam");
                return false;
            }

            if (cacheObject.IsNoDamage && !cacheObject.IsQuestGiver)
            {
                cacheObject.AddCacheInfo("UnitNoDamage");
                return false;
            }

            if (cacheObject.IsFriendly && !cacheObject.IsQuestGiver)
            {
                cacheObject.AddCacheInfo("Friendly");
                return false;
            }

            if (cacheObject.MonsterRace == MonsterRace.Unknown)
            {
                cacheObject.AddCacheInfo("InvalidRace");
                return false;
            }

            if (cacheObject.IsInvulnerable && !cacheObject.IsQuestGiver)
            {
                cacheObject.AddCacheInfo("Invulnerable");
                return false;
            }

            if (cacheObject.IsDead)
            {
                cacheObject.AddCacheInfo("Dead");
                return false;
            }

            return true;
        }

        private bool ShouldIncludeGold(TrinityItem cacheObject)
        {
            if (!Core.Settings.Loot.Pickup.PickupGold)
            {
                cacheObject.AddCacheInfo("GoldPickupDisabled");
                return false;
            }
            if (cacheObject.GoldAmount < Core.Settings.Loot.Pickup.MinimumGoldStack)
            {
                cacheObject.AddCacheInfo("NotEnoughGold");
                return false;
            }
            return true;
        }

        private bool ShouldCacheItem(TrinityItem cacheObject)
        {
            if (!cacheObject.IsPickupNoClick && !TrinityItemManager.CachedIsValidTwoSlotBackpackLocation)
            {
                cacheObject.AddCacheInfo("BackpackFull");
                return false;
            }

            if (cacheObject.IsMyDroppedItem)
            {
                cacheObject.AddCacheInfo("DroppedItem");
                return false;
            }

            if (cacheObject.IsUntargetable)
            {
                cacheObject.AddCacheInfo("Untargetable");
                return false;
            }

            if (cacheObject.ItemQualityLevel <= ItemQuality.Rare4 && cacheObject.Distance > CharacterSettings.Instance.LootRadius)
            {
                cacheObject.AddCacheInfo($"OutOfRange Limit={CharacterSettings.Instance.LootRadius}");
                return false;
            }

            if (!TrinityItemManager.ShouldPickupItem(cacheObject))
            {
                cacheObject.AddCacheInfo("TrinityItemManager");
                return false;
            }

            return true;
        }

        private bool ShouldCacheGizmo(TrinityActor cacheObject)
        {
            if (cacheObject.IsUsed)
            {
                cacheObject.AddCacheInfo("UsedGizmo");
                return false;
            }

            if (DataDictionary.InteractWhiteListIds.Contains(cacheObject.ActorSnoId))
            {
                cacheObject.AddCacheInfo("Interact Whitelist");
                return true;
            }

            if (cacheObject.IsPlayerHeadstone && !Core.Settings.WorldObject.AllowPlayerResurection)
            {
                cacheObject.AddCacheInfo("AllowResurectionSetting");
                return false;
            }

            if (!Core.Settings.WorldObject.UseShrine)
            {
                cacheObject.AddCacheInfo("UseShrineSetting");
                return false;
            }

            if (!DataDictionary.ForceDestructibles.Contains(cacheObject.ActorSnoId) && Core.Settings.WorldObject.DestructibleOption == DestructibleIgnoreOption.ForceIgnore)
            {
                cacheObject.AddCacheInfo("ForceIgnoreDestructibles");
                return false;
            }

            if (TrinityTownRun.IsWantingTownRun && cacheObject.Distance > 10f)
            {
                cacheObject.AddCacheInfo("WantToTownRun");
                return false;
            }

            if (cacheObject.IsContainer && !cacheObject.IsMinimapActive && cacheObject.RadiusDistance > Core.Settings.WorldObject.ContainerOpenRange)
            {
                cacheObject.AddCacheInfo("ContainerOpenRange");
                return false;
            }

            if (cacheObject.IsDestroyable && !cacheObject.HasBeenWalkable && cacheObject.Distance > 5f)
            {
                cacheObject.AddCacheInfo("CantReachDestructible");
                return false;
            }

            if (!Core.Settings.WorldObject.OpenAnyContainer && !cacheObject.IsMinimapActive)
            {
                if (cacheObject.IsRareChest && !Core.Settings.WorldObject.OpenRareChests)
                {
                    cacheObject.AddCacheInfo("OpenRareChestsSetting");
                    return false;
                }

                if (cacheObject.IsChest && !Core.Settings.WorldObject.OpenChests && !cacheObject.IsQuestMonster)
                {
                    cacheObject.AddCacheInfo("OpenChestsSetting");
                    return false;
                }

                if (cacheObject.IsCorpse && !Core.Settings.WorldObject.InspectCorpses)
                {
                    cacheObject.AddCacheInfo("InspectCorpsesSetting");
                    return false;
                }

                if (cacheObject.IsGroundClicky && !Core.Settings.WorldObject.InspectGroundClicky)
                {
                    cacheObject.AddCacheInfo("GroundClickySetting");
                    return false;
                }

                if (cacheObject.IsWeaponRack && !Core.Settings.WorldObject.InspectWeaponRacks)
                {
                    cacheObject.AddCacheInfo("WeaponRacksSetting");
                    return false;
                }

                if (cacheObject.IsWeaponRack && !Core.Settings.WorldObject.InspectWeaponRacks)
                {
                    cacheObject.AddCacheInfo("WeaponRacksSetting");
                    return false;
                }
            }

            return true;
        }

        public static float ZDiffLimit(TrinityActor cacheObject)
        {
            var defaultValue = 50f;

            if (DataDictionary.LineOfSightWhitelist.Contains(cacheObject.ActorSnoId))
                return defaultValue;

            if (cacheObject.RActorId == TrinityPlugin.LastTargetRactorGUID)
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

        private static void UpdatePlayerSummonCounts(TrinityActor cacheObject)
        {
            if (!cacheObject.IsSummonedByPlayer)
                return;

            switch (Core.Player.ActorClass)
            {
                case ActorClass.Monk:
                    if (DataDictionary.MysticAllyIds.Contains(cacheObject.ActorSnoId))
                        Core.Player.Summons.MysticAllyCount++;
                    break;
                case ActorClass.DemonHunter:
                    if (DataDictionary.DemonHunterPetIds.Contains(cacheObject.ActorSnoId))
                        Core.Player.Summons.DHPetsCount++;
                    if (DataDictionary.DemonHunterSentryIds.Contains(cacheObject.ActorSnoId) && cacheObject.Distance < 60f)
                        Core.Player.Summons.DHSentryCount++;
                    break;
                case ActorClass.Wizard:
                    if (DataDictionary.WizardHydraIds.Contains(cacheObject.ActorSnoId) && cacheObject.Distance < 60f)
                        Core.Player.Summons.HydraCount++;
                    break;
                case ActorClass.Witchdoctor:
                    if (DataDictionary.SpiderPetIds.Contains(cacheObject.ActorSnoId) && cacheObject.Distance < 100f)
                        Core.Player.Summons.SpiderPetCount++;
                    if (DataDictionary.GargantuanIds.Contains(cacheObject.ActorSnoId))
                        Core.Player.Summons.GargantuanCount++;
                    if (DataDictionary.ZombieDogIds.Contains(cacheObject.ActorSnoId))
                        Core.Player.Summons.ZombieDogCount++;
                    if (DataDictionary.FetishArmyIds.Contains(cacheObject.ActorSnoId))
                        Core.Player.Summons.FetishArmyCount++;
                    break;
                case ActorClass.Barbarian:
                    if (DataDictionary.AncientIds.Contains(cacheObject.ActorSnoId))
                        Core.Player.Summons.AncientCount++;
                    break;
            }
        }

        public void Clear()
        {
            Items.Clear();
            Ignored.Clear();
        }
    }
}