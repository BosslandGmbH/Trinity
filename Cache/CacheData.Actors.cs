using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Cache.Properties;
using Trinity.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.Coroutines.Town;
using Trinity.Framework;
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

namespace Trinity.Cache
{
    public class ActorCache
    {
        public ulong LastUpdatedTick;
        public List<TrinityCacheObject> Items = new List<TrinityCacheObject>();
        public List<TrinityCacheObject> Ignored = new List<TrinityCacheObject>();
        public ILookup<TrinityObjectType, TrinityCacheObject> ByType = EmptyLookup<TrinityObjectType, TrinityCacheObject>.Instance;
        public ILookup<MonsterQuality, TrinityCacheObject> ByMonsterQuality = EmptyLookup<MonsterQuality, TrinityCacheObject>.Instance;

        private static void PreUpdateTasks()
        {
            TrinityPlugin.LastRefreshedCache = DateTime.UtcNow;
            TrinityPlugin.LastTargetPosition = TrinityPlugin.CurrentTarget != null ? TrinityPlugin.CurrentTarget.Position : Vector3.Zero;

            if (TrinityPlugin.CurrentTarget != null)
                TrinityPlugin.LastTargetRactorGUID = TrinityPlugin.CurrentTarget.RActorGuid;

            TrinityPlugin.LastTargetACDGuid = CombatBase.CurrentTarget != null ? CombatBase.CurrentTarget.ACDGuid : -1;
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

                var included = new List<TrinityCacheObject>();
                var ignored = new List<TrinityCacheObject>();
                var timer = new Stopwatch();

                PreUpdateTasks();

                foreach (var diaObject in ZetaDia.Actors.GetActorsOfType<DiaObject>(true, true))
                {
                    try
                    {
                        timer.Restart();
                        var actor = new TrinityCacheObject(diaObject);
                        timer.Stop();
                        actor.CacheTime = timer.Elapsed.TotalMilliseconds;

                        if (ShouldCacheActor(actor))
                        {
                            included.Add(actor);
                        }
                        else
                        {
                            actor.IsIgnored = true;
                            ignored.Add(actor);
                        }                       
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Exception updating object cache {diaObject.Name} {diaObject.ActorSnoId} {ex}");
                        PropertyLoader.Clear();
                        return;
                    }
                }

                if (TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.CacheManagement))
                {
                    foreach (var o in included)
                        Logger.LogDebug($"[Cache][{o.CacheTime:N4}] Added {o}");

                    foreach (var o in ignored)
                        Logger.LogDebug($"[Cache][{o.CacheTime:N4}] Ignored {o.InternalName} RActorGuid={o.RActorGuid} ActorSnoId={o.ActorSNO} Type={o.ActorType} CacheInfo={o.CacheInfo}");

                    Logger.LogDebug($"[Cache] PropertyCollections={PropertyLoader.Count}");
                }

                TrinityPlugin.Weighting.RefreshDiaGetWeights(included);

                ByType = included.ToLookup(k => k.Type);
                ByMonsterQuality = included.ToLookup(k => k.MonsterQuality);             

                Items = included;
                Ignored = ignored;
            }
        }

        

        private bool ShouldCacheActor(TrinityCacheObject cacheObject)
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
            }

            switch (cacheObject.Type)
            {
                case TrinityObjectType.Player:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.BuffedRegion:
                case TrinityObjectType.BloodShard:
                    return true;

                case TrinityObjectType.Unit:
                    return ShouldCacheUnit(cacheObject);

                case TrinityObjectType.Item:
                    return ShouldCacheItem(cacheObject);

                case TrinityObjectType.Gold:
                    return ShouldIncludeGold(cacheObject);

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

        private static bool ShouldCacheCommon(TrinityCacheObject cacheObject)
        {
            if (cacheObject.IsFrozen)
                cacheObject.AddCacheInfo("Frozen");

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

            if (cacheObject.ActorAttributes == null)
                cacheObject.AddCacheInfo("NoAttributes");

            if (cacheObject.IsDead)
            {
                cacheObject.AddCacheInfo("Dead");
                return false;
            }

            if (cacheObject.IsObstacle && cacheObject.IsGizmo && cacheObject.IsUsed)
            {
                cacheObject.AddCacheInfo("UsedObstacle");
                return false;
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

        private static bool ShouldIgnoreLoS(TrinityCacheObject cacheObject)
        {
            switch (cacheObject.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.BuffedRegion:
                    return true;
            }

            if (cacheObject.Distance < 4) return true;
            if (cacheObject.IsBoss) return true;
            if (cacheObject.IsTreasureGoblin) return true;
            if (cacheObject.ItemQuality >= ItemQuality.Legendary) return true;
            if (DataDictionary.LineOfSightWhitelist.Contains(cacheObject.ActorSNO)) return true;
            return false;
        }

        private bool ShouldCacheUnit(TrinityCacheObject cacheObject)
        {
            if (cacheObject.IsSameTeam)
            {
                cacheObject.AddCacheInfo("SameTeam");
                return false;
            }

            if (cacheObject.IsAlly)
            {
                cacheObject.AddCacheInfo("SameTeam");
                return false;
            }

            if (cacheObject.IsSummonedByPlayer)
            {
                // todo: This doesnt seem like a good place for this sort of processing to happen.                
                UpdatePlayerSummonCounts(cacheObject);

                cacheObject.AddCacheInfo("SummonedByPlayer");
                return false;
            }

            if (cacheObject.IsDead)
            {
                cacheObject.AddCacheInfo("Dead");
                return false;
            }

            return true;
        }

        private bool ShouldIncludeGold(TrinityCacheObject cacheObject)
        {
            if (!TrinityPlugin.Settings.Loot.Pickup.PickupGold)
            {
                cacheObject.AddCacheInfo("GoldPickupDisabled");
                return false;
            }
            if (cacheObject.GoldAmount < TrinityPlugin.Settings.Loot.Pickup.MinimumGoldStack)
            {
                cacheObject.AddCacheInfo("NotEnoughGold");
                return false;
            }
            return true;
        }

        private bool ShouldCacheItem(TrinityCacheObject cacheObject)
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

            if (cacheObject.ItemQuality <= ItemQuality.Rare4 && cacheObject.Distance > CharacterSettings.Instance.LootRadius)
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

        private bool ShouldCacheGizmo(TrinityCacheObject cacheObject)
        {
            if (cacheObject.IsUsed)
            {
                cacheObject.AddCacheInfo("UsedGizmo");
                return false;
            }

            if (cacheObject.IsPlayerHeadstone && !TrinityPlugin.Settings.WorldObject.AllowPlayerResurection)
            {
                cacheObject.AddCacheInfo("AllowResurectionSetting");
                return false;
            }

            if (cacheObject.IsGizmoDisabledByScript)
            {
                cacheObject.AddCacheInfo("DisabledByScript");
                return false;
            }

            if (!TrinityPlugin.Settings.WorldObject.UseShrine)
            {
                cacheObject.AddCacheInfo("UseShrineSetting");
                return false;
            }

            if (!DataDictionary.ForceDestructibles.Contains(cacheObject.ActorSNO) && TrinityPlugin.Settings.WorldObject.DestructibleOption == DestructibleIgnoreOption.ForceIgnore)
            {
                cacheObject.AddCacheInfo("ForceIgnoreDestructibles");
                return false;
            }

            if (TrinityTownRun.IsWantingTownRun && cacheObject.Distance > 10f)
            {
                cacheObject.AddCacheInfo("WantToTownRun");
                return false;
            }

            if (cacheObject.IsContainer && cacheObject.RadiusDistance > TrinityPlugin.Settings.WorldObject.ContainerOpenRange)
            {
                cacheObject.AddCacheInfo("ContainerOpenRange");
                return false;
            }

            if (cacheObject.IsDestroyable && !cacheObject.HasBeenWalkable && cacheObject.Distance > 5f)
            {
                cacheObject.AddCacheInfo("CantReachDestructible");
                return false;
            }

            if (!TrinityPlugin.Settings.WorldObject.OpenAnyContainer)
            {
                if (cacheObject.IsRareChest && !TrinityPlugin.Settings.WorldObject.OpenRareChests)
                {
                    cacheObject.AddCacheInfo("OpenRareChestsSetting");
                    return false;
                }

                if (cacheObject.IsChest && !TrinityPlugin.Settings.WorldObject.OpenChests)
                {
                    cacheObject.AddCacheInfo("OpenChestsSetting");
                    return false;
                }

                if (cacheObject.IsCorpse && !TrinityPlugin.Settings.WorldObject.InspectCorpses)
                {
                    cacheObject.AddCacheInfo("InspectCorpsesSetting");
                    return false;
                }

                if (cacheObject.IsGroundClicky && !TrinityPlugin.Settings.WorldObject.InspectGroundClicky)
                {
                    cacheObject.AddCacheInfo("GroundClickySetting");
                    return false;
                }

                if (cacheObject.IsWeaponRack && !TrinityPlugin.Settings.WorldObject.InspectWeaponRacks)
                {
                    cacheObject.AddCacheInfo("WeaponRacksSetting");
                    return false;
                }

                if (cacheObject.IsWeaponRack && !TrinityPlugin.Settings.WorldObject.InspectWeaponRacks)
                {
                    cacheObject.AddCacheInfo("WeaponRacksSetting");
                    return false;
                }
            }

            return true;
        }

        public static float ZDiffLimit(TrinityCacheObject cacheObject)
        {
            var defaultValue = 50f;

            if (DataDictionary.LineOfSightWhitelist.Contains(cacheObject.ActorSNO))
                return defaultValue;

            if (cacheObject.RActorGuid == TrinityPlugin.LastTargetRactorGUID)
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

        private static void UpdatePlayerSummonCounts(TrinityCacheObject cacheObject)
        {
            if (!cacheObject.IsSummonedByPlayer)
                return;

            switch (TrinityPlugin.Player.ActorClass)
            {
                case ActorClass.Monk:
                    if (DataDictionary.MysticAllyIds.Contains(cacheObject.ActorSNO))
                        TrinityPlugin.Player.Summons.MysticAllyCount++;
                    break;
                case ActorClass.DemonHunter:
                    if (DataDictionary.DemonHunterPetIds.Contains(cacheObject.ActorSNO))
                        TrinityPlugin.Player.Summons.DHPetsCount++;
                    if (DataDictionary.DemonHunterSentryIds.Contains(cacheObject.ActorSNO) && cacheObject.Distance < 60f)
                        TrinityPlugin.Player.Summons.DHSentryCount++;
                    break;
                case ActorClass.Wizard:
                    if (DataDictionary.WizardHydraIds.Contains(cacheObject.ActorSNO) && cacheObject.Distance < 60f)
                        TrinityPlugin.Player.Summons.HydraCount++;
                    break;
                case ActorClass.Witchdoctor:
                    if (DataDictionary.SpiderPetIds.Contains(cacheObject.ActorSNO) && cacheObject.Distance < 100f)
                        TrinityPlugin.Player.Summons.SpiderPetCount++;
                    if (DataDictionary.GargantuanIds.Contains(cacheObject.ActorSNO))
                        TrinityPlugin.Player.Summons.GargantuanCount++;
                    if (DataDictionary.ZombieDogIds.Contains(cacheObject.ActorSNO))
                        TrinityPlugin.Player.Summons.ZombieDogCount++;
                    if (DataDictionary.FetishArmyIds.Contains(cacheObject.ActorSNO))
                        TrinityPlugin.Player.Summons.FetishArmyCount++;
                    break;
                case ActorClass.Barbarian:
                    if (DataDictionary.AncientIds.Contains(cacheObject.ActorSNO))
                        TrinityPlugin.Player.Summons.AncientCount++;
                    break;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}


