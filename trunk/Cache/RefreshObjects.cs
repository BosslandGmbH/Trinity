using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Trinity.Cache;
using Trinity.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.Configuration;
using Trinity.DbProvider;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using System.Windows;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    public partial class TrinityPlugin : IPlugin
    {
        /// <summary>
        /// This method will add and update necessary information about all available actors. Determines ObjectType, sets ranges, updates blacklists, determines avoidance, kiting, target weighting
        /// and the result is we will have a new target for the Target Handler. Returns true if the cache was refreshed.
        /// </summary>
        /// <returns>True if the cache was updated</returns>
        public static bool RefreshDiaObjectCache()
        {
            if (ZetaDia.Service.Hero == null)
            {
                Logger.LogError("Hero is null!");
                return false;
            }

            if (!ZetaDia.Service.Hero.IsValid)
            {
                Logger.LogError("Hero is invalid!");
                return false;
            }

            if (!ZetaDia.IsInGame)
                return false;

            if (Player.IsLoadingWorld)
                return false;

            if (!ZetaDia.Me.IsValid)
                return false;

            if (!ZetaDia.Me.CommonData.IsValid)
                return false;


            using (new PerformanceLogger("RefreshDiaObjectCache"))
            {
                LastRefreshedCache = DateTime.UtcNow;

                /*
                 *  Refresh the Cache
                 */
                using (new PerformanceLogger("RefreshDiaObjectCache.UpdateBlock"))
                {
                    CacheData.Clear();
                    GenericCache.MaintainCache();
                    GenericBlacklist.MaintainBlacklist();

                    RefreshCacheInit();

                    // Now pull up all the data and store anything we want to handle in the super special cache list
                    // Also use many cache dictionaries to minimize DB<->D3 memory hits, and speed everything up a lot
                    RefreshCacheMainLoop();

                }

                /*
                 * Add Legendary & Set Minimap Markers to ObjectCache
                 */
                RefreshCacheMarkers();


                // Reduce ignore-for-loops counter
                if (_ignoreTargetForLoops > 0)
                    _ignoreTargetForLoops--;

                /*
                 *  Set Weights, assign CurrentTarget
                 */
                Weighting.RefreshDiaGetWeights();
             

                using (new PerformanceLogger("RefreshDiaObjectCache.FinalChecks"))
                {             
                    if (CombatBase.IsDoingGoblinKamakazi && CurrentTarget != null && CurrentTarget.Type != TrinityObjectType.Door && CurrentTarget.Type != TrinityObjectType.Barricade && !CurrentTarget.InternalName.ToLower().Contains("corrupt") && CurrentTarget.Weight != Weighting.MaxWeight)
                    {
                        Logger.Log("Forcing Target to Goblin '{0} ({1})' Distance={2}", CombatBase.KamakaziGoblin.InternalName, CombatBase.KamakaziGoblin.ActorSNO, CombatBase.KamakaziGoblin.Distance);
                        CurrentTarget = CombatBase.KamakaziGoblin;
                    }

                    if (CombatBase.IsDoingGoblinKamakazi && CurrentTarget == null)
                    {
                        Logger.Log("No Target, Switching to Goblin '{0} ({1})' Distance={2}", CombatBase.KamakaziGoblin.InternalName, CombatBase.KamakaziGoblin.ActorSNO, CombatBase.KamakaziGoblin.Distance);
                        CurrentTarget = CombatBase.KamakaziGoblin;
                    }

                    // Still no target, let's see if we should backtrack or wait for wrath to come off cooldown...
                    if (CurrentTarget == null)
                    {
                        RefreshWaitTimers();
                    }

                    // Still no target, let's end it all!
                    if (CurrentTarget == null)
                    {
                        Events.OnCacheUpdatedHandler.Invoke();
                        return true;
                    }

                    if (CurrentTarget.IsUnit)
                        lastHadUnitInSights = DateTime.UtcNow;

                    if (CurrentTarget.IsBossOrEliteRareUnique)
                        lastHadEliteUnitInSights = DateTime.UtcNow;

                    if (CurrentTarget.IsBoss || CurrentTarget.IsBountyObjective)
                        lastHadBossUnitInSights = DateTime.UtcNow;

                    if (CurrentTarget.Type == TrinityObjectType.Container)
                        lastHadContainerInSights = DateTime.UtcNow;

                    // Record the last time our target changed
                    if (LastTargetRactorGUID != CurrentTarget.RActorGuid)
                    {
                        TrinityPlugin.Weighting.RecordTargetHistory();

                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.Weight,
                            "Found New Target {0} dist={1:0} IsElite={2} Radius={3:0.0} Weight={4:0} ActorSnoId={5} " +
                            "AnimSnoId={6} TargetedCount={7} Type={8} ",
                            CurrentTarget.InternalName,
                            CurrentTarget.Distance,
                            CurrentTarget.IsEliteRareUnique,
                            CurrentTarget.Radius,
                            CurrentTarget.Weight,
                            CurrentTarget.ActorSNO,
                            CurrentTarget.Animation,
                            CurrentTarget.TimesBeenPrimaryTarget,
                            CurrentTarget.Type
                            );

                        _lastPickedTargetTime = DateTime.UtcNow;
                        _targetLastHealth = 0f;
                    }
                    else
                    {
                        // We're sticking to the same target, so update the target's health cache to check for stucks
                        if (CurrentTarget.IsUnit)
                        {
                            // Check if the health has changed, if so update the target-pick time before we blacklist them again
                            if (CurrentTarget.HitPointsPct != _targetLastHealth)
                            {
                                Logger.Log(TrinityLogLevel.Debug, LogCategory.Weight, "Keeping Target {0} - CurrentTarget.HitPoints: {1:0.00} TargetLastHealth: {2:0.00} ",
                                                CurrentTarget.RActorGuid, CurrentTarget.HitPointsPct, _targetLastHealth);
                                _lastPickedTargetTime = DateTime.UtcNow;
                            }
                            // Now store the target's last-known health
                            _targetLastHealth = CurrentTarget.HitPointsPct;
                        }
                    }
                }

                // We have a target and the cached was refreshed
                Events.OnCacheUpdatedHandler.Invoke();
                return true;
            }
        }

        /// <summary>
        /// Adds Legendary & Set Minimap Markers to ObjectCache
        /// </summary>
        [Execution.TrackMethod]
        private static void RefreshCacheMarkers()
        {
            if (Execution.Restrict("RefreshCacheMarkers", 5000)) return;

            const int setItemMarkerTexture = 404424;
            const int legendaryItemMarkerTexture = 275968;

            if (!BrainBehavior.IsVendoring && !WantToTownRun && !ForceVendorRunASAP && Settings.Loot.Pickup.PickupLegendaries)
            {
                var legendaryItemMarkers = ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(m => m.IsValid &&
                                    m.Position.Distance(Player.Position) >= 45f && m.Position.Distance(Player.Position) < 300f &&
                                    (m.MinimapTextureSnoId == setItemMarkerTexture || m.MinimapTextureSnoId == legendaryItemMarkerTexture) && !Blacklist60Seconds.Contains(m.NameHash)).ToList();

                foreach (var marker in legendaryItemMarkers)
                {
                    var name = (marker.MinimapTextureSnoId == setItemMarkerTexture ? "Set Item" : "Legendary Item") + " Minimap Marker";

                    var cacheObject = new TrinityCacheObject
                    {
                        Position = new Vector3((float)Math.Floor(marker.Position.X), (float)Math.Floor(marker.Position.Y), (float)Math.Floor(marker.Position.Z)),
                        InternalName = name,
                        ActorSNO = marker.NameHash,
                        RActorGuid = marker.MinimapTextureSnoId,
                        Distance = marker.Position.Distance(Player.Position),
                        ActorType = ActorType.Item,
                        Type = TrinityObjectType.Item,
                        ItemQuality = ItemQuality.Legendary,
                        Radius = 2f,
                        Weight = 50,
                        IsMarker = true
                    };
                    cacheObject.ObjectHash = HashGenerator.GenerateItemHash(cacheObject);

                    if (GenericBlacklist.ContainsKey(cacheObject.ObjectHash))
                    {
                        Logger.LogDebug(LogCategory.CacheManagement, "Ignoring Marker because it's blacklisted {0} {1} at {2} distance {3}", name, marker.MinimapTextureSnoId, marker.Position, marker.Position.Distance(Player.Position));
                        continue;
                    }

                    Logger.LogDebug(LogCategory.CacheManagement, "Adding {0} {1} at {2} distance {3}", name, marker.MinimapTextureSnoId, marker.Position, marker.Position.Distance(Player.Position));
                    ObjectCache.Add(cacheObject);
                }

                if (legendaryItemMarkers.Any() && TrinityItemManager.CachedIsValidTwoSlotBackpackLocation)
                {
                    var legendaryItems = ZetaDia.Actors.GetActorsOfType<DiaItem>().Where(i => i.IsValid && i.IsACDBased && i.Position.Distance(ZetaDia.Me.Position) < 5f &&
                        legendaryItemMarkers.Any(im => i.Position.Distance(i.Position) < 2f));

                    foreach (var diaItem in legendaryItems)
                    {
                        Logger.LogDebug(LogCategory.CacheManagement, "Adding Legendary Item from Marker {0} dist={1} ActorSnoId={2} ACD={3} RActor={4}",
                            diaItem.Name, diaItem.Distance, diaItem.ActorSnoId, diaItem.ACDId, diaItem.RActorId);

                        ObjectCache.Add(new TrinityCacheObject()
                        {
                            Position = diaItem.Position,
                            InternalName = diaItem.Name,
                            RActorGuid = diaItem.RActorId,
                            ActorSNO = diaItem.ActorSnoId,
                            ACDGuid = diaItem.ACDId,
                            HasBeenNavigable = true,
                            HasBeenInLoS = true,
                            Distance = diaItem.Distance,
                            ActorType = ActorType.Item,
                            Type = TrinityObjectType.Item,
                            Radius = 2f,
                            Weight = 50,
                            ItemQuality = ItemQuality.Legendary,
                        });
                    }
                }
            }

            bool isRiftGuardianQuestStep = ZetaDia.CurrentQuest.QuestSnoId == 337492 && ZetaDia.CurrentQuest.StepId == 16;

            if (isRiftGuardianQuestStep)
            {
                // Add Rift Guardian POI's or Markers to ObjectCache
                const int riftGuardianMarkerTexture = 81058;
                Func<MinimapMarker, bool> riftGuardianMarkerFunc = m => m.IsValid && (m.IsPointOfInterest || m.MinimapTextureSnoId == riftGuardianMarkerTexture) &&
                    !Blacklist60Seconds.Contains(m.NameHash);

                foreach (var marker in ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(riftGuardianMarkerFunc))
                {
                    Logger.LogDebug(LogCategory.CacheManagement, "Adding Rift Guardian POI, distance {0}", marker.Position.Distance(Player.Position));
                    ObjectCache.Add(new TrinityCacheObject()
                    {
                        Position = marker.Position,
                        InternalName = "Rift Guardian",
                        Distance = marker.Position.Distance(Player.Position),
                        RActorGuid = marker.NameHash,
                        ActorType = ActorType.Monster,
                        Type = TrinityObjectType.Unit,
                        Radius = 10f,
                        Weight = 5000,
                    });
                }
            }

            if (isRiftGuardianQuestStep || Player.ParticipatingInTieredLootRun) // X1_LR_DungeonFinder
            {
                foreach (var marker in ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(m => m.IsPointOfInterest && !Blacklist60Seconds.Contains(m.NameHash)))
                {
                    ObjectCache.Add(new TrinityCacheObject()
                    {
                        Position = marker.Position,
                        InternalName = "Rift Guardian",
                        Distance = marker.Position.Distance(Player.Position),
                        RActorGuid = marker.NameHash,
                        ActorType = ActorType.Monster,
                        Type = TrinityObjectType.Unit,
                        Radius = 10f,
                        Weight = 5000,
                    });
                }
            }
        }

        private static void RefreshCacheMainLoop()
        {
            using (new PerformanceLogger("CacheManagement.RefreshCacheMainLoop"))
            {
                IEnumerable<DiaObject> refreshSource;

                if (Settings.Advanced.LogCategories.HasFlag(LogCategory.CacheManagement))
                {
                    refreshSource = ReadDebugActorsFromMemory();
                }
                else
                {
                    refreshSource = ReadActorsFromMemory();
                }
                Stopwatch t1 = new Stopwatch();

                CacheData.NavigationObstacles.Clear();
                CacheData.Monsters.Clear();
                CacheData.Avoidances.Clear();


                foreach (DiaObject currentObject in refreshSource)
                {
                    //if (currentObject.ActorType == ActorType.Projectile)
                    //{
                    //    Logger.Log($"Projectile: {currentObject.Name} at {currentObject.Position}");
                    //}

                    try
                    {
                        if (!Settings.Advanced.LogCategories.HasFlag(LogCategory.CacheManagement))
                        {
                            /*
                             *  Main Cache Function
                             */
                            bool addToCache = CacheDiaObject(currentObject);
                            if (!string.IsNullOrEmpty(c_IgnoreReason) || !string.IsNullOrEmpty(c_IgnoreSubStep))
                                CurrentCacheObject.IgnoreReason = c_IgnoreReason + "." + c_IgnoreSubStep; // (!addToCache ? ((c_IgnoreReason != "None" ? c_IgnoreReason + "." : "") + c_IgnoreSubStep) : "");
                            if (!CacheData.IgnoreReasons.ContainsKey(currentObject.RActorId))
                                CacheData.IgnoreReasons.Add(currentObject.RActorId, CurrentCacheObject.IgnoreReason);
                            if (addToCache)
                                ObjectCache.Add(CurrentCacheObject);
                        }
                        else
                        {
                            // We're debugging, slightly slower, calculate performance metrics and dump debugging to log 
                            t1.Restart();

                            /*
                             *  Main Cache Function
                             */
                            bool addToCache = CacheDiaObject(currentObject);
                            if (!string.IsNullOrEmpty(c_IgnoreReason) || !string.IsNullOrEmpty(c_IgnoreSubStep))
                                CurrentCacheObject.IgnoreReason = c_IgnoreReason + "." + c_IgnoreSubStep; // (!addToCache ? ((c_IgnoreReason != "None" ? c_IgnoreReason + "." : "") + c_IgnoreSubStep) : "");
                            if (!CacheData.IgnoreReasons.ContainsKey(currentObject.RActorId))
                                CacheData.IgnoreReasons.Add(currentObject.RActorId, CurrentCacheObject.IgnoreReason);
                            if (addToCache)
                                ObjectCache.Add(CurrentCacheObject);

                            if (t1.IsRunning)
                                t1.Stop();

                            double duration = t1.Elapsed.TotalMilliseconds;

                            // don't log stuff we never care about
                            if (duration <= 1 && c_IgnoreSubStep == "IgnoreNames")
                                continue;
                            if (CurrentCacheObject.Type == TrinityObjectType.Player)
                                continue;

                            if (c_IgnoreReason != "InternalName")
                                Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                                    "[{0:0000.00}ms] {1} {2} Type={3}/{4}/{5} Name={6} ({7}) {8} {9} Dist2Mid={10:0} Dist2Rad={11:0} ZDiff={12:0} Radius={13:0} RAGuid={14} {15}",
                                    duration,
                                    (addToCache ? "Added " : "Ignored"),
                                    CurrentCacheObject.IgnoreReason,
                                    CurrentCacheObject.ActorType,
                                    CurrentCacheObject.GizmoType != GizmoType.None ? CurrentCacheObject.GizmoType.ToString() : "",
                                    CurrentCacheObject.Type,
                                    CurrentCacheObject.InternalName,
                                    CurrentCacheObject.ActorSNO,
                                    (CurrentCacheObject.IsBoss ? " IsBoss" : ""),
                                    (c_CurrentAnimation != SNOAnim.Invalid ? " AnimSnoId: " + c_CurrentAnimation : ""),
                                    CurrentCacheObject.Distance,
                                    CurrentCacheObject.RadiusDistance,
                                    c_ZDiff,
                                    CurrentCacheObject.Radius,
                                    CurrentCacheObject.RActorGuid,
                                    CurrentCacheObject.ExtraInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        TrinityLogLevel ll = TrinityLogLevel.Debug;
                        LogCategory lc = LogCategory.CacheManagement;

                        if (ex is NullReferenceException)
                        {
                            ll = TrinityLogLevel.Error;
                            lc = LogCategory.UserInformation;
                        }

                        string gizmoType = "";
                        var giz = currentObject as DiaGizmo;
                        if (giz != null && giz.CommonData.IsValid)
                        {
                            gizmoType = "GizmoType: " + giz.CommonData.ActorInfo.GizmoType.ToString();
                        }
                        Logger.Log(ll, lc, "Error while refreshing DiaObject ActorSnoId: {0} Name: {1} Type: {2} Distance: {3:0} {4} {5}",
                                currentObject.ActorSnoId, currentObject.Name, currentObject.ActorType, currentObject.Distance, gizmoType, ex);

                    }

                    if (CurrentCacheObject.ActorType == ActorType.Monster && CurrentCacheObject.IsHostile)
                        CacheData.Monsters.Add(CurrentCacheObject);
                }

            }
        }

        // Refresh object list from Diablo 3 memory RefreshDiaObjects()
        private static void RefreshCacheInit()
        {
            using (new PerformanceLogger("RefreshDiaObjectCache.CacheInit"))
            {

                // Update when we last refreshed with current time
                LastRefreshedCache = DateTime.UtcNow;

                // Blank current/last/next targets
                LastPrimaryTargetPosition = CurrentTarget != null ? CurrentTarget.Position : Vector3.Zero;
                KiteAvoidDestination = Vector3.Zero;

                // store last target GUID
                if(CurrentTarget != null)
                    LastTargetRactorGUID = CurrentTarget.RActorGuid;

                LastTargetACDGuid = CurrentTarget != null ? CurrentTarget.ACDGuid : -1;

                //reset current target
                //CurrentTarget = null;
                // Reset all variables for target-weight finding
                CurrentBotKillRange = Settings.Combat.Misc.NonEliteRange;

                if (AnyTreasureGoblinsPresent && Settings.Combat.Misc.GoblinPriority == GoblinPriority.Kamikaze && CurrentBotKillRange < 60f)
                    CurrentBotKillRange = 60f;

                AnyTreasureGoblinsPresent = false;

                // Max kill range if we're questing
                if (DataDictionary.QuestLevelAreaIds.Contains(Player.LevelAreaId))
                    CurrentBotKillRange = 300f;

                CurrentBotLootRange = CharacterSettings.Instance.LootRadius;
                _shouldStayPutDuringAvoidance = false;

                // Not allowed to kill monsters due to profile/routine/combat targeting settings - just set the kill range to a third
                if (!ProfileManager.CurrentProfile.KillMonsters || !CombatTargeting.Instance.AllowedToKillMonsters)
                {
                    CurrentBotKillRange = 0;
                }

                // Not allowed to loots due to profile/routine/loot targeting settings - just set range to a quarter
                if (!ProfileManager.CurrentProfile.PickupLoot || !LootTargeting.Instance.AllowedToLoot)
                {
                    CurrentBotLootRange = 0;
                }

                if (Player.ActorClass == ActorClass.Barbarian && Hotbar.Contains(SNOPower.Barbarian_WrathOfTheBerserker) && GetHasBuff(SNOPower.Barbarian_WrathOfTheBerserker))
                { //!sp - keep looking for kills while WOTB is up
                    _keepKillRadiusExtendedForSeconds = Math.Max(3, _keepKillRadiusExtendedForSeconds);
                    _timeKeepKillRadiusExtendedUntil = DateTime.UtcNow.AddSeconds(_keepKillRadiusExtendedForSeconds);
                }
                // Counter for how many cycles we extend or reduce our attack/kill radius, and our loot radius, after a last kill
                if (_keepKillRadiusExtendedForSeconds > 0)
                {
                    TimeSpan diffResult = DateTime.UtcNow.Subtract(_timeKeepKillRadiusExtendedUntil);
                    _keepKillRadiusExtendedForSeconds = (int)diffResult.Seconds;
                    //DbHelper.Log(TrinityLogLevel.Verbose, LogCategory.Moving, "Kill Radius remaining " + diffResult.Seconds + "s");
                    if (_timeKeepKillRadiusExtendedUntil <= DateTime.UtcNow)
                    {
                        _keepKillRadiusExtendedForSeconds = 0;
                    }
                }
                if (_keepLootRadiusExtendedForSeconds > 0)
                    _keepLootRadiusExtendedForSeconds--;

                // Clear forcing close-range priority on mobs after XX period of time
                if (_forceCloseRangeTarget && DateTime.UtcNow.Subtract(_lastForcedKeepCloseRange).TotalMilliseconds > ForceCloseRangeForMilliseconds)
                {
                    _forceCloseRangeTarget = false;
                }

                //AnyElitesPresent = false;
                AnyMobsInRange = false;

                // Clear our very short-term destructible blacklist within 3 seconds of last attacking a destructible
                if (_needClearDestructibles && DateTime.UtcNow.Subtract(_lastDestroyedDestructible).TotalMilliseconds > 2500)
                {
                    _needClearDestructibles = false;
                    _destructible3SecBlacklist = new HashSet<int>();
                }
                // Clear our very short-term ignore-monster blacklist (from not being able to raycast on them or already dead units)
                if (NeedToClearBlacklist3 && DateTime.UtcNow.Subtract(Blacklist3LastClear).TotalMilliseconds > 3000)
                {
                    NeedToClearBlacklist3 = false;
                    Blacklist3Seconds = new HashSet<int>();
                }

                // Reset the counters for player-owned things
                PlayerOwnedMysticAllyCount = 0;
                PlayerOwnedGargantuanCount = 0;
                PlayerOwnedFetishArmyCount = 0;
                PlayerOwnedZombieDogCount = 0;
                PlayerOwnedDHPetsCount = 0;
                PlayerOwnedDHSentryCount = 0;
                PlayerOwnedHydraCount = 0;
                PlayerOwnedAncientCount = 0;

                // Here's the list we'll use to store each object
                ObjectCache = new List<TrinityCacheObject>();

            }
        }

        private static void ClearCachesOnGameChange(object sender, EventArgs e)
        {
            CacheData.FullClear();
        }




        private static List<DiaObject> ReadDebugActorsFromMemory()
        {
            return (from o in ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
                    where o.IsValid && o.CommonData != null && o.CommonData.IsValid
                    orderby o.Distance
                    select o).ToList();
        }

        private static IEnumerable<DiaObject> ReadActorsFromMemory()
        {
            return from o in ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
                   where o.IsValid && o.CommonData != null && o.CommonData.IsValid
                   select o;
        }
        private static void RefreshWaitTimers()
        {

            // See if we should wait for [playersetting] milliseconds for possible loot drops before continuing run
            if (ShouldWaitForLootDrop)
            {
                CurrentTarget = new TrinityCacheObject()
                {
                    Position = Player.Position,
                    Type = TrinityObjectType.Avoidance,
                    Weight = 250,
                    Distance = 2f,
                    Radius = 2f,
                    IsWaitSpot = true,
                    InternalName = "WaitForLootDrops"
                };
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "Waiting for loot to drop, delay: {0}ms", Settings.Combat.Misc.DelayAfterKill);
            }
        }

        public static bool ShouldWaitForLootDrop
        {
            get
            {
                if (Player.ParticipatingInTieredLootRun)
                {
                    return CurrentTarget == null &&
                           (DateTime.UtcNow.Subtract(lastHadEliteUnitInSights).TotalMilliseconds <= Settings.Combat.Misc.DelayAfterKill ||
                            DateTime.UtcNow.Subtract(lastHadBossUnitInSights).TotalMilliseconds <= 3000);
                }

                return CurrentTarget == null &&
                           (DateTime.UtcNow.Subtract(lastHadUnitInSights).TotalMilliseconds <= Settings.Combat.Misc.DelayAfterKill ||
                            DateTime.UtcNow.Subtract(lastHadEliteUnitInSights).TotalMilliseconds <= Settings.Combat.Misc.DelayAfterKill ||
                            DateTime.UtcNow.Subtract(lastHadBossUnitInSights).TotalMilliseconds <= 3000 ||
                            DateTime.UtcNow.Subtract(Composites.LastFoundHoradricCache).TotalMilliseconds <= 5000) ||
                           DateTime.UtcNow.Subtract(lastHadContainerInSights).TotalMilliseconds <= Settings.WorldObject.OpenContainerDelay;
            }
        }
    }
}
