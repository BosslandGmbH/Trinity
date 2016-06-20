using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Trinity.Cache;
using Trinity.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Movement;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile.Common;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    public partial class TrinityPlugin
    {
        public class Weighting
        {
            public static void RefreshDiaGetWeights(List<TrinityCacheObject> objects)
            {
                using (new PerformanceLogger("RefreshDiaObjectCache.Weighting"))
                {
                    #region Variables

                    var deadPlayer = false;
                    try
                    {
                        deadPlayer = ZetaDia.Service.Party.NumPartyMembers < 1 &&
                                     ZetaDia.Actors.GetActorsOfType<DiaPlayer>().Any(x => x.IsDead);
                    }
                    catch
                    {
                        deadPlayer = false;
                    }

                    var isHighLevelGrift = Player.TieredLootRunlevel > 55;

                    var movementSpeed = PlayerMover.GetMovementSpeed();

                    var eliteCount = CombatBase.IgnoringElites
                        ? 0
                        : objects.Count(u => u.IsUnit && u.IsElite);
                    var avoidanceCount = Settings.Combat.Misc.AvoidAOE
                        ? 0
                        : objects.Count(o => o.Type == TrinityObjectType.Avoidance && o.Distance <= 50f);

                    var avoidanceNearby = Settings.Combat.Misc.AvoidAOE &&
                                          objects.Any(
                                              o => o.Type == TrinityObjectType.Avoidance && o.Distance <= 15f);

                    //var prioritizeCloseRangeUnits = (avoidanceNearby || _forceCloseRangeTarget || Player.IsRooted ||
                    //                                 DateTime.UtcNow.Subtract(StuckHandler.LastStuckTime).TotalMilliseconds < 1000 &&
                    //                                 ObjectCache.Count(u => u.IsUnit && u.RadiusDistance < 12f) >= 5);

                    var hiPriorityHealthGlobes = Settings.Combat.Misc.HiPriorityHG;

                    var healthGlobeEmergency = (Player.CurrentHealthPct <= CombatBase.EmergencyHealthGlobeLimit ||
                                                Player.PrimaryResourcePct <= CombatBase.HealthGlobeResource &&
                                                Legendary.ReapersWraps.IsEquipped) &&
                                               objects.Any(g => g.Type == TrinityObjectType.HealthGlobe) &&
                                               Settings.Combat.Misc.CollectHealthGlobe;

                    var hiPriorityShrine = Settings.WorldObject.HiPriorityShrines;

                    var getHiPriorityShrine = objects.Any(s => s.Type == TrinityObjectType.Shrine) &&
                                              hiPriorityShrine;

                    var getHiPriorityContainer = Settings.WorldObject.HiPriorityContainers &&
                                                 objects.Any(c => c.Type == TrinityObjectType.Container);

                    var profileTagCheck = false;

                    var behaviorName = "";
                    if (ProfileManager.CurrentProfileBehavior != null)
                    {
                        var behaviorType = ProfileManager.CurrentProfileBehavior.GetType();
                        behaviorName = behaviorType.Name;
                        if (!Settings.Combat.Misc.ProfileTagOverride && CombatBase.IsQuestingMode ||
                            behaviorType == typeof(WaitTimerTag) ||
                            behaviorType == typeof(UseTownPortalTag) ||
                            behaviorName.ToLower().Contains("townrun") ||
                            behaviorName.ToLower().Contains("townportal"))
                        {
                            profileTagCheck = true;
                        }
                    }

                    var usingTownPortal = TrinityTownRun.IsWantingTownRun;

                    // Highest weight found as we progress through, so we can pick the best target at the end (the one with the highest weight)
                    HighestWeightFound = 0;

                    var isStuck = Navigator.StuckHandler.IsStuck;

                    var elites = new List<TrinityCacheObject>();
                    var eliteswithAffixes = new List<TrinityCacheObject>();

                    var ignoredAffixes = Settings.Combat.Misc.IgnoreAffixes.GetFlags<MonsterAffixes>().ToList();
                    foreach (var unit in objects.Where(u => u.IsUnit))
                    {
                        if ((unit.IsRare || unit.IsChampion || unit.IsMinion) &&
                            ignoredAffixes.Any(a => unit.MonsterAffixes.HasFlag(a)))
                            eliteswithAffixes.Add(unit);
                        if (unit.IsElite)
                            elites.Add(unit);
                    }

                    #endregion

                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Weight,
                        "Starting weights: packSize={0} packRadius={1} MovementSpeed={2:0.0} Elites={3} AoEs={4} disableIgnoreTag={5} ({6}) closeRangePriority={7} townRun={8} questingArea={9} level={10} isQuestingMode={11} healthGlobeEmerg={12} hiPriHG={13} hiPriShrine={14}",
                        CombatBase.CombatOverrides.EffectiveTrashSize, CombatBase.CombatOverrides.EffectiveTrashRadius,
                        movementSpeed,
                        eliteCount, avoidanceCount, profileTagCheck, behaviorName,
                        PlayerMover.IsCompletelyBlocked, usingTownPortal,
                        DataDictionary.QuestLevelAreaIds.Contains(Player.LevelAreaId), Player.Level,
                        CombatBase.IsQuestingMode, healthGlobeEmergency, hiPriorityHealthGlobes, hiPriorityShrine);

                    if (Settings.Combat.Misc.GoblinPriority == GoblinPriority.Kamikaze)
                    {
                        var goblin = objects.FirstOrDefault(u => u.IsTreasureGoblin && u.Distance <= 200f);
                        if (goblin != null && !isStuck && !PlayerMover.IsCompletelyBlocked)
                        {
                            CombatBase.IsDoingGoblinKamakazi = true;
                            CombatBase.KamakaziGoblin = goblin;
                            Logger.Log("Going Kamakazi on Goblin '{0} ({1})' Distance={2}", goblin.InternalName,
                                goblin.ActorSNO, goblin.Distance);
                            CurrentTarget = goblin;
                        }
                        else
                        {
                            CombatBase.IsDoingGoblinKamakazi = false;
                        }
                    }
                    else
                    {
                        CombatBase.IsDoingGoblinKamakazi = false;
                    }

                    var riftProgressionKillAll = RiftProgression.IsInRift && !RiftProgression.IsGaurdianSpawned && !RiftProgression.RiftComplete &&
                                                 Settings.Combat.Misc.RiftProgressionAlwaysKillPct < 100 && RiftProgression.CurrentProgressionPct < 100 &&
                                                 RiftProgression.CurrentProgressionPct >= Settings.Combat.Misc.RiftProgressionAlwaysKillPct;

                    if (riftProgressionKillAll != _riftProgressionKillAll)
                    {
                        _riftProgressionKillAll = riftProgressionKillAll;
                        if (riftProgressionKillAll)
                        {
                            Logger.Log($"Rift Progression is now at {RiftProgression.CurrentProgressionPct} - Killing everything!");
                            CombatBase.CombatMode = CombatMode.KillAll;
                        }
                        else
                        {
                            Logger.LogVerbose($"Reverting rift progression kill all mode back to normal combat");
                            CombatBase.CombatMode = CombatMode.On;
                        }
                    }

                    #region Foreach Loop

                    TrinityCacheObject bestTarget = null;
                    foreach (var cacheObject in objects.Where(x => !x.IsPlayer))
                    {
                        if (cacheObject == null || !cacheObject.IsValid)
                            continue;

                        cacheObject.Weight = 0;
                        cacheObject.WeightInfo = string.Empty;

                        if (PlayerMover.IsCompletelyBlocked)
                        {
                            cacheObject.WeightInfo += "PlayerBlocked";

                            if (Settings.Combat.Misc.AttackWhenBlocked)
                            {
                                if (!cacheObject.IsUnit)
                                {
                                    cacheObject.Weight = 0;
                                    cacheObject.WeightInfo += "Ignoring because we are blocked. ";
                                    continue;
                                }
                                if (cacheObject.Distance > 12f)
                                {
                                    cacheObject.Weight = 0;
                                    cacheObject.WeightInfo += "Ignoring Blocked Far Away ";
                                    continue;
                                }
                                cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} because we are Blocked.";
                                cacheObject.Weight = MaxWeight + ObjectDistanceFormula(cacheObject);
                                bestTarget = GetNewBestTarget(cacheObject, bestTarget);
                                continue;
                            }
                        }

                        if (CombatBase.IsDoingGoblinKamakazi)
                        {
                            if (cacheObject.RActorGuid == CombatBase.KamakaziGoblin.RActorGuid && !isStuck)
                            {
                                cacheObject.Weight = MaxWeight;
                                cacheObject.WeightInfo += $"Maxxing {cacheObject.InternalName} - Goblin Kamakazi Run ";
                                break;
                            }
                            continue;
                        }

                        if (cacheObject.ItemType == TrinityItemType.HoradricRelic && Player.BloodShards >= Player.MaxBloodShards)
                        {
                            cacheObject.Weight = 0;
                            cacheObject.WeightInfo += string.Format("Max BloodShards ", cacheObject.InternalName);
                            continue;
                        }

                        if (!Settings.Advanced.BetaPlayground && Core.Avoidance.InCriticalAvoidance(cacheObject.Position) || Core.Avoidance.Grid.IsIntersectedByFlags(cacheObject.Position, ZetaDia.Me.Position, AvoidanceFlags.CriticalAvoidance))
                        {
                            cacheObject.Weight = 0;
                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Intersected by Critical Avoidance.";
                            continue;
                        }

                        cacheObject.Weight = MinWeight;
                        switch (cacheObject.Type)
                        {
                            #region Unit

                            case TrinityObjectType.Unit:
                                {
                                    #region Unit Variables

                                    //bool isInHotSpot = GroupHotSpots.CacheObjectIsInHotSpot(cacheObject) || cacheObject.IsNavBlocking();

                                    bool elitesInRangeOfUnit = !CombatBase.IgnoringElites &&
                                                               objects.Any(
                                                                   u =>
                                                                       u.ACDGuid != cacheObject.ACDGuid &&
                                                                       u.IsElite &&
                                                                       u.Position.Distance2D(cacheObject.Position) <= 15f);

                                    int nearbyTrashCount =
                                        objects.Count(u => u.IsUnit && u.HitPoints > 0 && u.IsTrashMob &&
                                                               cacheObject.Position.Distance(u.Position) <=
                                                               CombatBase.CombatOverrides.EffectiveTrashRadius);

                                    bool ignoreSummoner = cacheObject.IsSummoner && !Settings.Combat.Misc.ForceKillSummoners;


                                    var isBoss = cacheObject.CommonData.MonsterQualityLevel ==
                                                 MonsterQuality.Boss;
                                    var isUnique = cacheObject.CommonData.MonsterQualityLevel ==
                                                   MonsterQuality.Unique;
                                    var isRare = cacheObject.CommonData.MonsterQualityLevel ==
                                                 MonsterQuality.Rare;
                                    var isMinion = cacheObject.CommonData.MonsterQualityLevel ==
                                                   MonsterQuality.Minion;
                                    var isChampion = cacheObject.CommonData.MonsterQualityLevel ==
                                                     MonsterQuality.Champion;

                                    #endregion


                                    // Only ignore monsters we have a rift value for and below the settings threshold.
                                    //if (RiftProgression.IsInRift && cacheObject.RiftValuePct > 0 &&
                                    //    cacheObject.RiftValuePct < Settings.Combat.Misc.RiftValueIgnoreUnitsBelow &&
                                    //    !cacheObject.IsElite && !PlayerMover.IsCompletelyBlocked)
                                    //{
                                    //    cacheObject.WeightInfo +=
                                    //        string.Format("Ignoring {0} - low rift value ({1} / Setting={2})",
                                    //            cacheObject.InternalName, cacheObject.RiftValuePct,
                                    //            Settings.Combat.Misc.RiftValueIgnoreUnitsBelow);

                                    //    break;
                                    //}


                                    cacheObject.WeightInfo =
                                        string.Format(
                                            "ShouldIgnore={3} nearbyCount={0} radiusDistance={1:0} hotspot={2} elitesInRange={4} hitPointsPc={5:0.0} summoner={6} quest={7} minimap={8} bounty={9} ",
                                            nearbyTrashCount, cacheObject.RadiusDistance, false,
                                            usingTownPortal, elitesInRangeOfUnit, cacheObject.HitPointsPct,
                                            ignoreSummoner, cacheObject.IsQuestMonster, cacheObject.IsMinimapActive,
                                            cacheObject.IsBountyObjective);

                                    #region Basic Checks

                                    if (CombatBase.CombatMode == CombatMode.KillAll)
                                    {
                                        //Dist:                160     140     120     100      80     60     40      20      0
                                        //Weight (25k Max):    -77400  -53400  -32600  -15000  -600   10600  18600   23400   25000
                                        //
                                        //Formula:   MaxWeight-(Distance * Distance * RangeFactor)
                                        //           RangeFactor effects how quickly weights go into negatives on far distances.                                                                    

                                        var ignoreTrashTooFarAway = cacheObject.IsTrashMob &&
                                                                    cacheObject.Distance >
                                                                    Settings.Combat.Misc.NonEliteRange;
                                        var ignoreElitesTooFarAway = cacheObject.IsElite &&
                                                                     cacheObject.Distance > Settings.Combat.Misc.EliteRange;
                                        if (ignoreTrashTooFarAway || ignoreElitesTooFarAway)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format("Ignore Far Away Stuff TrashRange={0} EliteRange={1}",
                                                    Settings.Combat.Misc.NonEliteRange, Settings.Combat.Misc.EliteRange);
                                            cacheObject.Weight = 0;
                                            break;
                                        }

                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += "Kill All Mode";
                                        break;
                                    }

                                    if (Core.Avoidance.InCriticalAvoidance(cacheObject.Position))
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format("Ignoring {0} - in Critical Avoidance.", cacheObject.InternalName);
                                        break;
                                    }

                                    if (cacheObject.HitPointsPct <= 0)
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format("Ignoring {0} - is dead", cacheObject.InternalName);
                                        break;
                                    }

                                    if (Player.InActiveEvent && objects.Any(o => o.IsEventObject))
                                    {
                                        Vector3 eventObjectPosition =
                                            objects.FirstOrDefault(o => o.IsEventObject).Position;

                                        if (!cacheObject.IsQuestMonster &&
                                            cacheObject.Position.Distance(eventObjectPosition) > 35)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format("Ignoring {0} - Too Far From Event",
                                                    cacheObject.InternalName);
                                            break;
                                        }
                                    }

                                    if (healthGlobeEmergency && cacheObject.Type != TrinityObjectType.HealthGlobe && !PlayerMover.IsCompletelyBlocked)
                                    {
                                        // Many 'bot ignored some elites' complaints are due to priority globe aquisition.
                                        if (cacheObject.IsElite && Player.CurrentHealthPct < Math.Min(0.35, Settings.Combat.Misc.HealthGlobeLevel))
                                        {
                                            Logger.LogDebug($"Health Globe Emergency Ignoring Elite {cacheObject.InternalName} HealthPct={Player.CurrentHealthPct}");
                                        }
                                        else
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for Priority Health Globe";
                                            break;
                                        }
                                    }

                                    if (getHiPriorityShrine && cacheObject.Type != TrinityObjectType.Shrine && !PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for Priority Shrine ";
                                        break;
                                    }

                                    if (getHiPriorityContainer && cacheObject.Type != TrinityObjectType.Container && !PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for Priority Container";
                                        break;
                                    }

                                    //Monster is in cache but not within kill range
                                    var killRange = DistanceForObjectType(cacheObject);
                                    if (!cacheObject.IsBoss && !cacheObject.IsTreasureGoblin &&
                                        LastTargetRactorGUID != cacheObject.RActorGuid &&
                                        !cacheObject.IsQuestMonster && !cacheObject.IsBountyObjective &&
                                        cacheObject.RadiusDistance > killRange)
                                    {
                                        cacheObject.WeightInfo += string.Format("Ignoring {0} - out of Kill Range ({1})",
                                            cacheObject.InternalName, killRange);
                                        break;
                                    }

                                    if (cacheObject.IsTreasureGoblin)
                                    {
                                        // Original Trinity stuff for priority handling now
                                        switch (Settings.Combat.Misc.GoblinPriority)
                                        {
                                            case GoblinPriority.Normal:
                                                // Treating goblins as "normal monsters". Ok so I lied a little in the config, they get a little extra weight really! ;)
                                                cacheObject.WeightInfo += "GoblinNormal ";
                                                cacheObject.Weight += 500d;
                                                break;
                                            case GoblinPriority.Prioritize:
                                                // Super-high priority option below... 
                                                cacheObject.WeightInfo += "GoblinPrioritize ";
                                                cacheObject.Weight += 1000d;
                                                break;
                                            case GoblinPriority.Kamikaze:
                                                // KAMIKAZE SUICIDAL TREASURE GOBLIN RAPE AHOY!
                                                cacheObject.WeightInfo += "GoblinKamikaze ";
                                                cacheObject.Weight += MaxWeight;
                                                break;
                                        }
                                    }

                                    #endregion



                                    #region Special Case Monsters

                                    if (Player.CurrentSceneSnoId == 28768 && cacheObject.ActorSNO == (int)SNOActor.x1_Tentacle_Goatman_Melee_A && !cacheObject.CommonData.IsVisibleOnMinimap)
                                    {
                                        cacheObject.WeightInfo += "Invisible Goat in Cow King rift event ";
                                        cacheObject.Weight = 0;
                                        break;
                                    }

                                    #endregion

                                    if (deadPlayer)
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format("Adding {0} because we have a dead party member.",
                                                cacheObject.InternalName);
                                        cacheObject.Weight += MaxWeight;
                                        break;
                                    }

                                    if (!cacheObject.IsHostile)
                                    {
                                        cacheObject.WeightInfo += "Unit Not Hostile";
                                        cacheObject.Weight = MinWeight;
                                        break;
                                    }

                                    if (cacheObject.IsInvulnerable &&
                                             Settings.Combat.Misc.IgnoreMonstersWhileReflectingDamage)
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format("Ignoring {0} because of Invulnerability ",
                                                cacheObject.InternalName);
                                        cacheObject.Weight = MinWeight;
                                        break;
                                    }

                                    if (Player.CurrentHealthPct <= 0.25 && ZetaDia.Service.Party.NumPartyMembers < 1)
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format("Adding {0} Below Health Threshold ",
                                                cacheObject.InternalName);
                                    }
                                    else if (cacheObject.IsQuestMonster || cacheObject.IsEventObject ||
                                             cacheObject.IsBountyObjective)
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format("Adding {0} Quest Monster | Bounty | Event Objective ",
                                                cacheObject.InternalName);
                                    }
                                    else if (cacheObject.Distance < 25 && Player.IsCastingTownPortalOrTeleport() &&
                                             !Legendary.HomingPads.IsEquipped)
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format("Adding {0} because of Town Portal",
                                                cacheObject.InternalName);
                                    }
                                    //else if (isInHotSpot && PlayerMover.IsCompletelyBlocked)
                                    //{
                                    //    cacheObject.WeightInfo +=
                                    //        string.Format("Adding {0} due to being in Path or Hotspot ",
                                    //            cacheObject.InternalName);
                                    //}
                                    else if (PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo +=
                                            string.Format(
                                                "Adding {0} because we seem to be stuck *OR* if not ranged and currently rooted ",
                                                cacheObject.InternalName);
                                    }
                                    //else if (DataDictionary.MonsterCustomWeights.ContainsKey(cacheObject.ActorSNO))
                                    //{
                                    //    cacheObject.WeightInfo +=
                                    //        string.Format(
                                    //            "Adding {0} because monsters from the dictionary/hashlist set at the top of the code ",
                                    //            cacheObject.InternalName);
                                    //}
                                    //else if ((cacheObject.ActorSNO == 210120 || cacheObject.ActorSNO == 210268) &&
                                    //         cacheObject.Distance <= 25f)
                                    //{
                                    //    cacheObject.WeightInfo += string.Format("Adding {0} because of Blocking",
                                    //        cacheObject.InternalName);
                                    //}

                                    #region Trash Mob

                                    else if (cacheObject.IsTrashMob)
                                    {
                                        var isAlwaysKillByValue = RiftProgression.IsInRift &&
                                                                  cacheObject.RiftValuePct > 0 &&
                                                                  cacheObject.RiftValuePct >
                                                                  Settings.Combat.Misc.RiftValueAlwaysKillUnitsAbove;
                                        //if (isAlwaysKillByValue)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format("IsHighRiftValue {0}", cacheObject.RiftValuePct);
                                        //}

                                        if (Settings.Combat.Misc.IgnoreHighHitPointTrash && !isAlwaysKillByValue)
                                        {
                                            HashSet<string> highHitPointTrashMobNames = new HashSet<string>
                                            {
                                                "mallet", //
                                                "monstrosity", //
                                                "triune_berserker", //
                                                "beast_d",
                                                "thousandpounder", //5581
                                                "westmarchbrute", //258678, 332679
                                                "unburied" //6359
                                            };

                                            var unitName = cacheObject.InternalName.ToLower();
                                            if (highHitPointTrashMobNames.Any(name => unitName.Contains(name)))
                                            {
                                                cacheObject.WeightInfo +=
                                                    string.Format("Ignoring {0} for High Hp Mob.",
                                                        cacheObject.InternalName);
                                                break;
                                            }
                                        }
                                        else if (cacheObject.HitPointsPct <=
                                                 Settings.Combat.Misc.ForceKillTrashBelowHealth)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format(
                                                    "Adding {0} because it is below the minimum trash mob health",
                                                    cacheObject.InternalName);
                                        }
                                        else if (cacheObject.IsSummoner && Settings.Combat.Misc.ForceKillSummoners)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format("Adding {0} because he is a summoner",
                                                    cacheObject.InternalName);
                                            //cacheObject.Weight += 500d;
                                        }
                                        else if (cacheObject.HitPointsPct <
                                                 Settings.Combat.Misc.IgnoreTrashBelowHealthDoT &&
                                                 cacheObject.HasDotDPS)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format(
                                                    "Ignoring {0} - Hitpoints below Health/DoT Threshold ",
                                                    cacheObject.InternalName);
                                            break;
                                        }
                                        else if (nearbyTrashCount < CombatBase.CombatOverrides.EffectiveTrashSize &&
                                                 !DataDictionary.CorruptGrowthIds.Contains(cacheObject.ActorSNO))
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring Below TrashPackSize ({nearbyTrashCount} < {CombatBase.CombatOverrides.EffectiveTrashSize})";
                                            break;
                                        }
                                        else
                                            cacheObject.WeightInfo +=
                                                string.Format(" All Filters Passed: Adding {0} by Default.",
                                                    cacheObject.InternalName);
                                    }

                                    #endregion

                                    #region Elite / Rares / Uniques

                                    else if (isUnique || isBoss || isRare || isMinion || isChampion)
                                    {

                                        //XZ - Please add Ignore below health for elite.
                                        //if ((cacheObject.HitPointsPct <
                                        //     Settings.Combat.Misc.IgnoreEliteBelowHealthDoT) &&
                                        //    cacheObject.HasDotDPS)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format("Ignoring {0} - Hitpoints below Health/DoT Threshold ", cacheObject.InternalName);
                                        //    break;
                                        //}

                                        if (cacheObject.HitPointsPct <=
                                            Settings.Combat.Misc.ForceKillElitesHealth && !cacheObject.IsMinion)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format("Adding {0} for Elite Under Health Threshold.",
                                                    cacheObject.InternalName);
                                            cacheObject.Weight += MaxWeight;
                                            break;
                                        }
                                        if (TargetUtil.NumMobsInRangeOfPosition(cacheObject.Position,
                                            CombatBase.CombatOverrides.EffectiveTrashRadius) >=
                                            CombatBase.CombatOverrides.EffectiveTrashSize &&
                                            Settings.Combat.Misc.ForceKillClusterElites)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format("Adding {0} for Elite Inside Cluster.",
                                                    cacheObject.InternalName);

                                        }
                                        else if ((Settings.Combat.Misc.IgnoreElites ||
                                                  Settings.Combat.Misc.IgnoreRares && isRare ||
                                                  Settings.Combat.Misc.IgnoreMinions && isMinion ||
                                                  Settings.Combat.Misc.IgnoreChampions && isChampion) &&
                                                 !cacheObject.IsBoss)
                                        {
                                            cacheObject.WeightInfo +=
                                                string.Format(
                                                    "Ignoring {0} for Ignore Elite/Minion cThreshold.",
                                                    cacheObject.InternalName);
                                            break;
                                        }
                                        else if (Settings.Combat.Misc.IgnoreHighHitPointElites)
                                        {
                                            HashSet<string> highHitPointTrashMobNames = new HashSet<string>
                                            {
                                                "mallet", //
                                                "monstrosity", //
                                                "triune_berserker", //
                                                "beast_d",
                                                "thousandpounder", //5581
                                                "westmarchbrute", //258678, 332679
                                                "unburied" //6359
                                            };

                                            var unitName = cacheObject.InternalName.ToLower();
                                            if (highHitPointTrashMobNames.Any(name => unitName.Contains(name)))
                                            {
                                                cacheObject.WeightInfo +=
                                                    string.Format("Ignoring {0} for High Hp Elite Mob.",
                                                        cacheObject.InternalName);
                                                break;
                                            }
                                        }

                                        if (!cacheObject.IsBoss)
                                        {
                                            var ignoredAffixMatches = ignoredAffixes.Where(a => cacheObject.MonsterAffixes.HasFlag(a)).ToList();
                                            if (ignoredAffixMatches.Any())
                                            {

                                                //Logger.Log($"Ignoring {cacheObject.InternalName} due to {string.Join(",", ignoredAffixMatches)}");
                                                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} due to {ignoredAffixMatches.FirstOrDefault()} Affix ";
                                                break;
                                            }
                                        }
                                        cacheObject.WeightInfo += string.Format("Adding {0} default Elite ",
                                            cacheObject.InternalName);
                                        cacheObject.Weight += EliteFormula(cacheObject);
                                    }

                                    #endregion

                                    var dist = ObjectDistanceFormula(cacheObject);
                                    var last = 0; //LastTargetFormula(cacheObject);
                                    var pack = PackDensityFormula(cacheObject, objects);
                                    var health = UnitHealthFormula(cacheObject);
                                    var path = PathBlockedFormula(cacheObject);
                                    var reflect = AffixMonsterNearFormula(cacheObject, eliteswithAffixes);
                                    var elite = EliteMonsterNearFormula(cacheObject, elites);
                                    var aoe = AoENearFormula(cacheObject) + AoEInPathFormula(cacheObject);

                                    cacheObject.Weight += dist + last + pack + health + path + reflect + elite + aoe;

                                    cacheObject.WeightInfo +=
                                        string.Format(
                                            " dist={0:0.0} last={1:0.0} pack={2:0.0} health={3:0.0} path={4:0.0} reflect={5:0.0} elite={6:0.0} aoe={7:0.0}",
                                            dist, last, pack, health, path, reflect, elite, aoe);

                                    break;
                                }


                            #endregion

                            #region Item

                            case TrinityObjectType.Item:
                                {
                                    var isTwoSquare = true;
                                    var item = cacheObject.Item;
                                    if (item != null)
                                    {
                                        var commonData = item.CommonData;
                                        if (commonData != null && commonData.IsValid)
                                            isTwoSquare = commonData.IsTwoSquareItem;
                                    }


                                    // Campaign A5 Quest "Lost Treasure of the Nephalem" - have to interact with nephalem switches first... 
                                    // Quest: x1_Adria, Id: 257120, Step: 108 - disable all looting, pickup, and objects
                                    if (Player.WorldType != Act.OpenWorld && Player.CurrentQuestSNO == 257120 &&
                                        Player.CurrentQuestStep == 108)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} For Quest";
                                        break;
                                    }

                                    if (Player.ParticipatingInTieredLootRun && objects.Any(m => m.IsUnit && m.IsBoss))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} Loot Run Boss";
                                        break;
                                    }

                                    if (Player.IsInTown)
                                    {
                                        var cachedItem = ActorManager.GetItemByAnnId(cacheObject.AnnId);
                                        if (cachedItem != null)
                                        {
                                            if (Settings.Loot.Pickup.DontPickupInTown && !cachedItem.IsItemAssigned)
                                            {
                                                cacheObject.WeightInfo += $"Ignoring DontPickUpInTown Setting.";
                                                break;
                                            }
                                            if (!cachedItem.CanPickupItem())
                                            {
                                                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - unable to pickup.";
                                                break;
                                            }
                                        }
                                    }

                                    if (DropItems.DroppedItemAnnIds.Contains(cacheObject.AnnId))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring previously dropped item";
                                    }

                                    // Don't pickup items if we're doing a TownRun
                                    if (!TrinityItemManager.CachedIsValidTwoSlotBackpackLocation)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for TownRun";
                                        break;
                                    }

                                    // Death's Breath Priority
                                    if (cacheObject.ActorSNO == 361989 || cacheObject.ActorSNO == 449044)
                                    {
                                        // Ignore Non-Legendaries in AoE
                                        if (Settings.Loot.Pickup.IgnoreNonLegendaryInAoE && Core.Avoidance.InAvoidance(cacheObject.Position) ||
                                                Core.Avoidance.InCriticalAvoidance(cacheObject.Position))
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Legendary in AoE";
                                            break;
                                        }

                                        if (!Settings.Loot.Pickup.PickupDeathsBreath)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} due to settings";
                                            break;
                                        }

                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} - Death's Breath";
                                        break;
                                    }

                                    // Give legendaries max weight, always
                                    if (cacheObject.ItemQuality >= ItemQuality.Legendary)
                                    {
                                        // Ignore Legendaries in AoE
                                        if (Settings.Loot.Pickup.IgnoreLegendaryInAoE && Core.Avoidance.InAvoidance(cacheObject.Position) ||
                                                Core.Avoidance.InCriticalAvoidance(cacheObject.Position))
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Legendary in AoE";
                                            break;
                                        }

                                        // Ignore Legendaries near Elites
                                        if (Settings.Loot.Pickup.IgnoreLegendaryNearElites &&
                                            objects.Any(
                                                u =>
                                                    u.IsElite &&
                                                    u.Position.Distance(cacheObject.Position) <= 15f))
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Legendary near Elite";
                                            break;
                                        }
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} - Legendary";
                                        break;
                                    }

                                    //Non Legendaries
                                    if (cacheObject.ItemQuality < ItemQuality.Legendary)
                                    {
                                        // Ignore Non-Legendaries in AoE
                                        if (Settings.Loot.Pickup.IgnoreNonLegendaryInAoE && Core.Avoidance.InAvoidance(cacheObject.Position) ||
                                                Core.Avoidance.InCriticalAvoidance(cacheObject.Position))
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Legendary in AoE";
                                            break;
                                        }
                                        // Ignore Non-Legendaries near Elites
                                        if (Settings.Loot.Pickup.IgnoreNonLegendaryNearElites &&
                                            objects.Any(
                                                u =>
                                                    u.IsElite &&
                                                    u.Position.Distance(cacheObject.Position) <= 15f))
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Non Legendary near Elite";
                                            break;
                                        }
                                    }

                                    if (cacheObject.ItemQuality == ItemQuality.Normal)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} - White Item";
                                        break;
                                    }

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);

                                    break;
                                }

                            #endregion

                            #region Gold

                            case TrinityObjectType.Gold:
                                {
                                    if (!Settings.Loot.Pickup.PickupGold)
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Ignoring {cacheObject.InternalName} - Pick Up Gold Setting.";
                                        break;
                                    }
                                    //Ignore because we are blocked by objects or mobs.
                                    if (PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                        break;
                                    }

                                    //Ignore because we are TownPortaling
                                    if (usingTownPortal)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                        break;
                                    }
                                    // Campaign A5 Quest "Lost Treasure of the Nephalem" - have to interact with nephalem switches first... 
                                    // Quest: x1_Adria, Id: 257120, Step: 108 - disable all looting, pickup, and objects
                                    if (Player.WorldType != Act.OpenWorld && Player.CurrentQuestSNO == 257120 &&
                                        Player.CurrentQuestStep == 108)
                                    {
                                        cacheObject.Weight = 0;
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - DisableForQuest";
                                        break;
                                    }

                                    // Ignore gold near Elites
                                    if (Settings.Loot.Pickup.IgnoreGoldNearElites &&
                                        objects.Any(
                                            u =>
                                                u.IsElite &&
                                                u.Position.Distance(cacheObject.Position) <= 15f))
                                    {
                                        break;
                                    }

                                    // Ignore gold in AoE
                                    if (Settings.Loot.Pickup.IgnoreGoldInAoE && Core.Avoidance.Grid.IsLocationInFlags(cacheObject.Position, AvoidanceFlags.Avoidance))
                                    {
                                        break;
                                    }

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);

                                    break;
                                }

                            #endregion

                            #region Power Globe

                            case TrinityObjectType.PowerGlobe:
                                {
                                    if (Settings.Combat.Misc.IgnorePowerGlobes)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Power Globe Setting.";
                                        break;
                                    }

                                    //Ignore because we are blocked by objects or mobs.
                                    if (PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                        break;
                                    }
                                    //Ignore because we are TownPortaling
                                    if (usingTownPortal)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                        break;
                                    }

                                    if (Settings.Loot.Pickup.IgnoreNonLegendaryInAoE && Core.Avoidance.Grid.IsLocationInFlags(cacheObject.Position, AvoidanceFlags.Avoidance))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - In Avoidance.";
                                        break;
                                    }

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);
                                    break;
                                }

                            #endregion

                            #region Progression Globe

                            case TrinityObjectType.ProgressionGlobe:
                                {
                                    ////Ignore because we are blocked by objects or mobs.
                                    //if (PlayerMover.IsCompletelyBlocked)
                                    //{
                                    //    cacheObject.WeightInfo += string.Format("Ignoring {0} - Nav Blocked.",
                                    //        cacheObject.InternalName);
                                    //    break;
                                    //}
                                    ////Ignore because we are TownPortaling
                                    //if (TownRun.IsTryingToTownPortal())
                                    //{
                                    //    cacheObject.WeightInfo += string.Format("Ignoring {0} - Town Portal.",
                                    //        cacheObject.InternalName);
                                    //    break;
                                    //}

                                    if (Settings.Loot.Pickup.IgnoreProgressionGlobesInAoE && Core.Avoidance.Grid.IsLocationInFlags(cacheObject.Position, AvoidanceFlags.CriticalAvoidance))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - In Avoidance.";
                                        break;
                                    }

                                    if (Settings.Combat.Misc.IgnoreNormalProgressionGlobes && DataDictionary.NormalProgressionGlobeSNO.Contains(cacheObject.ActorSNO))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} because of settings.";
                                        break;
                                    }

                                    if (Settings.Combat.Misc.IgnoreGreaterProgressionGlobes && DataDictionary.GreaterProgressionGlobeSNO.Contains(cacheObject.ActorSNO))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} because of settings.";
                                        break;
                                    }

                                    if (cacheObject.Distance <= 180f)
                                    {
                                        cacheObject.WeightInfo += $"Maxxing {cacheObject.InternalName} - Progression Globe.";
                                        cacheObject.Weight += MaxWeight;
                                        break;
                                    }

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);
                                    break;
                                }

                            #endregion

                            #region Health Globe && Health Wells

                            case TrinityObjectType.HealthWell:
                            case TrinityObjectType.HealthGlobe:
                                {
                                    if (!Settings.Combat.Misc.CollectHealthGlobe)
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Ignoring {cacheObject.InternalName} - Collect Health Globe Setting.";
                                    }

                                    //Ignore because we are blocked by objects or mobs.
                                    if (PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                        break;
                                    }

                                    //Ignore because we are TownPortaling
                                    if (usingTownPortal)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                        break;
                                    }

                                    if (Settings.Loot.Pickup.IgnoreHealthGlobesInAoE && Core.Avoidance.Grid.IsLocationInFlags(cacheObject.Position, AvoidanceFlags.Avoidance))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - In Avoidance.";
                                        break;
                                    }

                                    // do not collect health globes if we are kiting and health globe is too close to monster or avoidance
                                    if (CombatBase.KiteMode != KiteMode.Never)
                                    {
                                        if (Core.Avoidance.Grid.IsIntersectedByFlags(cacheObject.Position, Player.Position, AvoidanceFlags.Avoidance))
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Kiting with Monster Obstacles.";
                                            break;
                                        }
                                        if (Core.Avoidance.Grid.IsLocationInFlags(cacheObject.Position, AvoidanceFlags.Avoidance))
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Kiting with Time Bound Avoidance.";
                                            break;
                                        }
                                    }

                                    if (Player.CurrentHealthPct > CombatBase.EmergencyHealthGlobeLimit)
                                    {
                                        //XZ - Add gui Variable for Party MemberHealth
                                        if (
                                            objects.Any(
                                                p => p.Type == TrinityObjectType.Player && p.RActorGuid != Player.RActorGuid))
                                        {
                                            var minPartyHealth =
                                                objects.Where(
                                                    p =>
                                                        p.Type == TrinityObjectType.Player &&
                                                        p.RActorGuid != Player.RActorGuid)
                                                    .Min(p => p.HitPointsPct);
                                            if (minPartyHealth <= 25)
                                            {
                                                cacheObject.WeightInfo +=
                                                    $"Adding {cacheObject.InternalName} - Party Health Below Threshold";
                                                cacheObject.Weight += hiPriorityHealthGlobes
                                                    ? MaxWeight
                                                    : (1d - minPartyHealth) * 5000d +
                                                      EliteMonsterNearFormula(cacheObject, elites) -
                                                      PackDensityFormula(cacheObject, objects);
                                                break;
                                            }
                                        }
                                        var myHealth = Player.CurrentHealthPct;
                                        // DH's logic with Blood Vengeance passive
                                        if (Player.ActorClass == ActorClass.DemonHunter &&
                                            Player.PrimaryResource <= 10 &&
                                            CacheData.Hotbar.PassiveSkills.Contains(
                                                SNOPower.DemonHunter_Passive_Vengeance))
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Adding {cacheObject.InternalName} - Reapes Wraps.";
                                            cacheObject.Weight += hiPriorityHealthGlobes
                                                ? MaxWeight
                                                : (1d - Player.PrimaryResource) * 5000d +
                                                  EliteMonsterNearFormula(cacheObject, elites) -
                                                  PackDensityFormula(cacheObject, objects);
                                            break;
                                        }

                                        // WD's logic with Gruesome Feast passive
                                        if (Player.ActorClass == ActorClass.Witchdoctor &&
                                            Player.PrimaryResource <= 1200 &&
                                            CacheData.Hotbar.PassiveSkills.Contains(
                                                SNOPower.Witchdoctor_Passive_GruesomeFeast))
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Adding {cacheObject.InternalName} - Reapes Wraps.";
                                            cacheObject.Weight += hiPriorityHealthGlobes
                                                ? MaxWeight
                                                : (1d - Player.PrimaryResource) * 5000d +
                                                  EliteMonsterNearFormula(cacheObject, elites) -
                                                  PackDensityFormula(cacheObject, objects);
                                            break;
                                        }

                                        //Reapers Wraps Equipped
                                        if (Legendary.ReapersWraps.IsEquipped && Player.PrimaryResource <= 50)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Adding {cacheObject.InternalName} - Reapes Wraps.";
                                            cacheObject.Weight += hiPriorityHealthGlobes
                                                ? MaxWeight
                                                : (1d - Player.PrimaryResource) * 5000d +
                                                  EliteMonsterNearFormula(cacheObject, elites) -
                                                  PackDensityFormula(cacheObject, objects);
                                            break;
                                        }
                                        //XZ - Set this to be a value to ignore globes above certain health.
                                        if (ZetaDia.Me.HitpointsCurrentPct > 0.80)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Over 80% health.";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Maxxing {cacheObject.InternalName} - Player.CurrentHealthPct < CombatBase.EmergencyHealthGlobeLimit.";
                                        cacheObject.Weight = MaxWeight;
                                        break;
                                    }

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);
                                    break;
                                }

                            #endregion

                            #region Shrine

                            case TrinityObjectType.CursedShrine:
                            case TrinityObjectType.Shrine:
                                {
                                    if (cacheObject.IsUsed)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Used.";
                                        break;
                                    }

                                    if (!cacheObject.IsQuestMonster)
                                    {
                                        if (!Settings.WorldObject.UseShrine)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Dont use shrines setting.";
                                            break;
                                        }

                                        //XZ - Please Add this.
                                        //if (!Settings.WorldObject.UseCursedShrine && cacheObject.Type == TrinityObjectType.CursedShrine)
                                        //    break;

                                        //Ignore because we are blocked by objects or mobs.
                                        if (PlayerMover.IsCompletelyBlocked)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                            break;
                                        }

                                        //Ignore because we are TownPortaling
                                        if (usingTownPortal)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                            break;
                                        }
                                    }
                                    // Campaign A5 Quest "Lost Treasure of the Nephalem" - have to interact with nephalem switches first... 
                                    // Quest: x1_Adria, Id: 257120, Step: 108 - disable all looting, pickup, and objects
                                    if (Player.WorldType != Act.OpenWorld && Player.CurrentQuestSNO == 257120 &&
                                        Player.CurrentQuestStep == 108)
                                    {
                                        cacheObject.Weight = 0;
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Disable for Quest";
                                        break;
                                    }

                                    // todo missing a check for shrine type setting checkboxes ?

                                    if (cacheObject.Distance < 12f)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Shrine so close i can touch it {cacheObject.InternalName}";
                                        break;
                                    }

                                    if (GetShrineType(cacheObject) != ShrineTypes.RunSpeed &&
                                        GetShrineType(cacheObject) != ShrineTypes.Speed &&
                                        GetShrineType(cacheObject) != ShrineTypes.Fortune)
                                    {
                                        var priorityMultiplier = 1;
                                        if (Settings.WorldObject.HiPriorityShrines)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Adding {cacheObject.InternalName} - High Priority Shrine";
                                            priorityMultiplier = 100;
                                        }
                                        if (Legendary.NemesisBracers.IsEquipped)
                                        {
                                            if (elites.Any())
                                            {
                                                var eliteMultiplier = -1000;
                                                if (GetShrineType(cacheObject) == ShrineTypes.Shield)
                                                    eliteMultiplier = 1000;
                                                cacheObject.Weight -= elites.Count * eliteMultiplier;
                                            }
                                            else
                                                cacheObject.Weight += 1000d;
                                        }

                                        if (elites.Any())
                                        {
                                            cacheObject.Weight += elites.Count * 1000;
                                            cacheObject.WeightInfo +=
                                                $"Adding {cacheObject.InternalName} - Higher Priority Shrine for Elites";
                                        }
                                        cacheObject.Weight += PackDensityFormula(cacheObject, objects) * priorityMultiplier;
                                    }

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          //EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);
                                    break;
                                }

                            #endregion

                            #region Door and Barricade

                            case TrinityObjectType.Barricade:
                            case TrinityObjectType.Door:
                                {
                                    if (cacheObject.IsUsed)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Used.";
                                        break;
                                    }

                                    if (!cacheObject.IsQuestMonster)
                                    {
                                        //Ignore because we are blocked by objects or mobs.
                                        if (PlayerMover.IsCompletelyBlocked)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                            break;
                                        }

                                        //Ignore because we are TownPortaling
                                        if (usingTownPortal && cacheObject.Distance > 25f)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                            break;
                                        }
                                    }
                                    // We're standing on the damn thing... open it!!
                                    if (cacheObject.RadiusDistance <= 10f)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo +=
                                            $"Maxxing {cacheObject.InternalName} - Door in Close Distance.";
                                    }
                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) +
                                                          PackDensityFormula(cacheObject, objects) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);
                                    break;
                                }

                            #endregion

                            #region Destructible

                            case TrinityObjectType.Destructible:
                                {
                                    if (!cacheObject.IsQuestMonster)
                                    {
                                        //Ignore because we are blocked by objects or mobs.
                                        if (PlayerMover.IsCompletelyBlocked)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                            break;
                                        }

                                        //Ignore because we are TownPortaling
                                        if (usingTownPortal && cacheObject.Distance > 25f)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                            break;
                                        }
                                    }
                                    if (DataDictionary.ForceDestructibles.Contains(cacheObject.ActorSNO))
                                    {
                                        cacheObject.Weight = 100d;
                                        break;
                                    }

                                    // Not Stuck, skip!
                                    if (Settings.WorldObject.DestructibleOption == DestructibleIgnoreOption.OnlyIfStuck &&
                                        cacheObject.RadiusDistance > 0 &&
                                        (DateTime.UtcNow.Subtract(PlayerMover.LastGeneratedStuckPosition).TotalSeconds > 3))
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Ignoring {cacheObject.InternalName} - Destructible Settings.";
                                        break;
                                    }

                                    //// We're standing on the damn thing... break it
                                    if (cacheObject.Distance < 3f)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo +=
                                            $"Maxxing {cacheObject.InternalName} - Close Distance.";
                                    }

                                    //// Fix for WhimsyShire Pinata
                                    if (DataDictionary.ResplendentChestIds.Contains(cacheObject.ActorSNO))
                                        cacheObject.Weight += 500d;
                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) -
                                                          PackDensityFormula(cacheObject, objects) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);
                                    break;
                                }

                            #endregion

                            #region Interactables

                            case TrinityObjectType.Interactable:
                                {
                                    if (cacheObject.IsUsed)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Used.";
                                        break;
                                    }

                                    if (!cacheObject.IsQuestMonster)
                                    {
                                        //Ignore because we are blocked by objects or mobs.
                                        if (PlayerMover.IsCompletelyBlocked)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                            break;
                                        }

                                        //Ignore because we are TownPortaling
                                        if (usingTownPortal && cacheObject.Distance > 25f)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                            break;
                                        }
                                    }
                                    // Campaign A5 Quest "Lost Treasure of the Nephalem" - have to interact with nephalem switches first... 
                                    // Quest: x1_Adria, Id: 257120, Step: 108 - disable all looting, pickup, and objects
                                    if (Player.WorldType != Act.OpenWorld && Player.CurrentQuestSNO == 257120 &&
                                        Player.CurrentQuestStep == 108)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo +=
                                            $"Adding {cacheObject.InternalName} - Campaign A5 Quest Lost Treasure of the Nephalem";
                                        break;
                                    }

                                    // nearby monsters attacking us - don't try to use headtone
                                    if (cacheObject.Object is DiaGizmo && cacheObject.Gizmo != null &&
                                        cacheObject.Gizmo.CommonData != null &&
                                        cacheObject.Gizmo.CommonData.ActorInfo != null &&
                                        cacheObject.Gizmo.CommonData.ActorInfo.GizmoType == GizmoType.Headstone &&
                                        objects.Any(u => u.IsUnit && u.RadiusDistance < 25f && u.IsFacingPlayer))
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Ignoring {cacheObject.InternalName} - Units Near Headstone. ";
                                        break;
                                    }

                                    if (DataDictionary.HighPriorityInteractables.Contains(cacheObject.ActorSNO) &&
                                        cacheObject.RadiusDistance <= 30f)
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Maxxing {cacheObject.InternalName} - High Priority Interactable.. ";
                                        cacheObject.Weight = MaxWeight;
                                        break;
                                    }

                                    if (cacheObject.IsQuestMonster)
                                    {
                                        cacheObject.Weight += 5000d;
                                    }
                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          ////LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) -
                                                          PackDensityFormula(cacheObject, objects) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);

                                    break;
                                }

                            #endregion

                            #region Container

                            case TrinityObjectType.Container:
                                {
                                    if (!cacheObject.IsQuestMonster)
                                    {
                                        //Ignore because we are blocked by objects or mobs.
                                        if (PlayerMover.IsCompletelyBlocked)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                            break;
                                        }

                                        //Ignore because we are TownPortaling
                                        if (usingTownPortal)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                            break;
                                        }
                                    }

                                    float maxRange = Settings.WorldObject.ContainerOpenRange;
                                    var isRare = cacheObject.InternalName.ToLower().Contains("chest_rare") ||
                                                 DataDictionary.ResplendentChestIds.Contains(cacheObject.ActorSNO);
                                    if (isRare)
                                        maxRange = 250f;
                                    if (cacheObject.Distance > maxRange)
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Ignoring {cacheObject.InternalName} - Too Far away. ";
                                        break;
                                    }

                                    if (Legendary.HarringtonWaistguard.IsEquipped)
                                    {
                                        if (Legendary.HarringtonWaistguard.IsBuffActive)
                                        {
                                            cacheObject.WeightInfo +=
                                                $"Ignoring {cacheObject.InternalName} - Harring Buff is Already up. ";
                                            break;
                                        }
                                        cacheObject.Weight += 1000d;
                                    }
                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          ////LastTargetFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) -
                                                          PackDensityFormula(cacheObject, objects) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);
                                    break;
                                }

                                #endregion
                        }

                        bestTarget = GetNewBestTarget(cacheObject, bestTarget);
                    }

                    #endregion Foreach loop

                    // Set Record History
                    if (bestTarget?.InternalName != null && bestTarget.ActorSNO > 0 && bestTarget.Weight > 0)
                    {
                        //TargetUtil.ClearCurrentTarget("Clearing for Weight");
                        LastTargetIsSafeSpot = bestTarget != null && CurrentTarget != null && CurrentTarget.IsSafeSpot;
                        CurrentTarget = bestTarget;

                        //Logger.Log($"Last Guid = {LastTargetRactorGUID}");

                        var timesTargetted = RecordTargetHistory(bestTarget);

                        if (bestTarget.RActorGuid != LastTargetRactorGUID || bestTarget != null && bestTarget.IsMarker)
                        {
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.UserInformation,
                                $"Target changed to {CurrentTarget.ActorSNO} // {CurrentTarget.InternalName} RActorGuid={CurrentTarget.RActorGuid} " +
                                $"({CurrentTarget.Type}) {CurrentTarget.WeightInfo} TargetTimes={timesTargetted}");
                        }
                        return;
                    }
                    TargetUtil.ClearCurrentTarget("No good target's found in Weighting.");
                    //var text = bestTarget != null ? bestTarget.Weight : 0;
                    //Logger.Log(" CACHE COUNT: " + ObjectCache.Count(x => !x.IsPlayer) + " Weight: " + text);

                }
            }

            private static TrinityCacheObject GetNewBestTarget(TrinityCacheObject cacheObject, TrinityCacheObject bestTarget)
            {
                cacheObject.WeightInfo += cacheObject.IsNPC ? " IsNPC" : "";
                cacheObject.WeightInfo += cacheObject.NPCIsOperable ? " IsOperable" : "";

                Logger.Log(TrinityLogLevel.Debug, LogCategory.Weight,
                    "Weight={0:0} name={1} sno={2} type={3} R-Dist={4:0} IsElite={5} RAGuid={6} {7}",
                    cacheObject.Weight, cacheObject.InternalName, cacheObject.ActorSNO, cacheObject.Type,
                    cacheObject.RadiusDistance, cacheObject.IsElite,
                    cacheObject.RActorGuid, cacheObject.WeightInfo);
                cacheObject.WeightInfo = cacheObject.WeightInfo;

                if (bestTarget == null)
                    bestTarget = cacheObject;

                // Use the highest weight, and if at max weight, the closest
                var pickNewTarget = cacheObject.Weight > 0 &&
                                    (cacheObject.Weight > HighestWeightFound ||
                                     (cacheObject.Weight == HighestWeightFound && (CurrentTarget == null || cacheObject.Distance < CurrentTarget.Distance)));

                if (!pickNewTarget) return bestTarget;
                bestTarget = cacheObject;
                HighestWeightFound = cacheObject.Weight;
                return bestTarget;
            }

            /// <summary>
            /// Update history of targetting for an actor and blacklist target if nessedsary
            /// </summary>
            /// <param name="bestTarget">actor to update</param>
            /// <returns>current number of times targetted</returns>
            public static int RecordTargetHistory(TrinityCacheObject bestTarget)
            {
                TargettingInfo info;
                if (bestTarget == null)
                    return 0;

                if (CacheData.TargetHistory.Count > 200)
                    CacheData.TargetHistory.RemoveAll(o => DateTime.UtcNow.Subtract(o.CreatedTime).TotalSeconds > 30);

                if (CacheData.TargetHistory.TryGetValue(bestTarget.ObjectHash, out info))
                {
                    var isNewTarget = bestTarget.ObjectHash != CurrentTarget.ObjectHash && !LastTargetIsSafeSpot && LastTargetRactorGUID > 0;
                    if (isNewTarget)
                    {
                        // Targeted times is used primarily for blacklisting, 
                        // exclude avoidance triggered target switches or things will get blacklisted too fast while kiting.      

                        info.TargetedTimes++;
                        bestTarget.HasBeenPrimaryTarget = true;
                    }

                    var isEliteLowHealth = bestTarget.HitPointsPct <= 0.75 && bestTarget.IsElite;
                    if (isEliteLowHealth)
                        return info.TargetedTimes;

                    if (bestTarget.IsBoss || bestTarget.IsSafeSpot || bestTarget.IsWaitSpot)
                        return info.TargetedTimes;

                    if (!isNewTarget)
                    {
                        var diff = DateTime.UtcNow.Subtract(info.LastTargetedTime);
                        info.TimeAsCurrentTarget = info.TimeAsCurrentTarget + diff;
                    }

                    if (info.TargetedTimes > GetBlacklistTargetTimes(bestTarget))
                    {
                        GenericBlacklist.Blacklist(bestTarget, TimeSpan.FromSeconds(60), $"Targetted too many times ({info.TargetedTimes})");
                        info.TargetedTimes = 0;
                        info.BlacklistedTimes++;
                    }
                    //else if (info.TimeAsCurrentTarget.TotalSeconds > 30 && !bestTarget.IsBoss)
                    //{
                    //    GenericBlacklist.Blacklist(bestTarget, TimeSpan.FromSeconds(30), $"Target timeout ({info.TimeAsCurrentTarget.TotalSeconds}s))");
                    //    info.TimeAsCurrentTarget = TimeSpan.Zero;                        
                    //    info.BlacklistedTimes++;
                    //}
                    else
                    {
                        info.LastTargetedTime = DateTime.UtcNow;
                    }

                    return info.TargetedTimes;
                }

                CacheData.TargetHistory.Add(bestTarget.ObjectHash, new TargettingInfo
                {
                    ObjectHash = bestTarget.ObjectHash,
                    RActorGuid = bestTarget.RActorGuid,
                    Name = bestTarget.InternalName,
                    TargetedTimes = 1,
                    LastTargetedTime = DateTime.UtcNow,
                });

                return 1;
            }

            private static int GetBlacklistTargetTimes(TrinityCacheObject currentTarget)
            {
                switch (currentTarget.Type)
                {
                    case TrinityObjectType.Item:
                        return currentTarget.ItemQuality >= ItemQuality.Legendary ? 400 : 75;
                    case TrinityObjectType.Door:
                    case TrinityObjectType.ProgressionGlobe:
                        return 300;
                }
                return 150;
            }

            #region Shrines

            public enum ShrineTypes
            {
                Unknown,
                //Regular
                Fortune, //Is this still in game?
                Frenzied,
                Reloaded, //Other Run Speed ???
                Enlightened,
                Glow,
                RunSpeed,
                Goblin,
                Hoarder,
                //Pylon
                Shield,
                Speed,
                Casting,
                Damage,
                Conduit
            }

            public static ShrineTypes GetShrineType(TrinityCacheObject cacheObject)
            {
                switch (cacheObject.ActorSNO)
                {
                    case (int)SNOActor.a4_Heaven_Shrine_Global_Fortune:
                    case (int)SNOActor.Shrine_Global_Fortune:
                        return ShrineTypes.Fortune;

                    case (int)SNOActor.a4_Heaven_Shrine_Global_Frenzied:
                    case (int)SNOActor.Shrine_Global_Frenzied:
                        return ShrineTypes.Frenzied;

                    case (int)SNOActor.a4_Heaven_Shrine_Global_Reloaded:
                    case (int)SNOActor.Shrine_Global_Reloaded:
                        return ShrineTypes.RunSpeed;

                    case (int)SNOActor.a4_Heaven_Shrine_Global_Enlightened:
                    case (int)SNOActor.Shrine_Global_Enlightened:
                        return ShrineTypes.Enlightened;

                    case (int)SNOActor.Shrine_Global_Glow:
                        return ShrineTypes.Glow;

                    case (int)SNOActor.a4_Heaven_Shrine_Global_Hoarder:
                    case (int)SNOActor.Shrine_Global_Hoarder:
                        return ShrineTypes.Hoarder;

                    case (int)SNOActor.x1_LR_Shrine_Infinite_Casting:
                        return ShrineTypes.Casting;

                    case (int)SNOActor.x1_LR_Shrine_Electrified_TieredRift:
                    case (int)SNOActor.x1_LR_Shrine_Electrified:
                        return ShrineTypes.Conduit;

                    case (int)SNOActor.x1_LR_Shrine_Invulnerable:
                        return ShrineTypes.Shield;

                    case (int)SNOActor.x1_LR_Shrine_Run_Speed:
                        return ShrineTypes.Shield;

                    case (int)SNOActor.x1_LR_Shrine_Damage:
                        return ShrineTypes.Damage;

                    case (int)SNOActor.Shrine_TreasureGoblin:
                        return ShrineTypes.Goblin;

                    default:
                        return ShrineTypes.Unknown;
                }
            }

            #endregion

            #region Helper Methods

            public const double MaxWeight = 50000d;
            private const double MinWeight = -1d;
            private static bool _riftProgressionKillAll;

            private static double GetLastHadUnitsInSights()
            {
                return Math.Max(DateTime.UtcNow.Subtract(lastHadUnitInSights).TotalMilliseconds,
                    DateTime.UtcNow.Subtract(lastHadEliteUnitInSights).TotalMilliseconds);
            }

            /// <summary>
            ///     Gets the settings distances based on elite or not.
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <returns></returns>
            public static float DistanceForObjectType(TrinityCacheObject cacheObject)
            {
                return cacheObject.CommonData.MonsterQualityLevel == MonsterQuality.Boss ||
                       cacheObject.CommonData.MonsterQualityLevel == MonsterQuality.Unique ||
                       cacheObject.CommonData.MonsterQualityLevel == MonsterQuality.Rare ||
                       cacheObject.CommonData.MonsterQualityLevel == MonsterQuality.Champion ||
                       cacheObject.CommonData.MonsterQualityLevel == MonsterQuality.Minion
                    ? Settings.Combat.Misc.EliteRange
                    : Settings.Combat.Misc.NonEliteRange;
            }

            public static double GoldFormula(TrinityCacheObject cacheObject)
            {
                return cacheObject.GoldAmount * 0.05;
            }

            /// <summary>
            ///     Gets the base weight for types of Elites/Rares/Champions/Uniques/Bosses
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <returns></returns>
            public static double EliteFormula(TrinityCacheObject cacheObject)
            {
                if (cacheObject.CommonData.MonsterQualityLevel ==
                    MonsterQuality.Boss)
                    return 1500d;
                if (cacheObject.CommonData.MonsterQualityLevel ==
                    MonsterQuality.Unique)
                    return 400d;
                if (cacheObject.CommonData.MonsterQualityLevel ==
                    MonsterQuality.Rare && cacheObject.CommonData.MonsterQualityLevel !=
                    MonsterQuality.Minion)
                    return 300d;
                if (cacheObject.CommonData.MonsterQualityLevel ==
                    MonsterQuality.Champion)
                    return 200d;
                //if (cacheObject.CommonData.MonsterQualityLevel ==
                //    Zeta.Game.Internals.Actors.MonsterQuality.Minion)
                //    return 100d;
                return 100d;
            }

            /// <summary>
            ///     Gets the weight for objects near AoE
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <returns></returns>
            public static double AoENearFormula(TrinityCacheObject cacheObject)
            {
                double weight = 0;

                // todo TimeBoundAvoidance no longer exists, update formula possibly using ...
                //var node = Core.Avoidance.Grid.GetNearestNode(cacheObject.Position);
                //if (node != null)
                //{
                //    weight = node.Weight * 
                //}

                //if (!Settings.Combat.Misc.KillMonstersInAoE)
                //    return weight;
                //var avoidances = CacheData.TimeBoundAvoidance.Where(u => u.Position.Distance(cacheObject.Position) < 15);
                //foreach (var avoidance in avoidances)
                //{
                //    weight -= 25 * Math.Max(1, 15 - avoidance.Radius);
                //}

                return weight;
            }

            /// <summary>
            ///     Gets the Weight of Objects that have AoE in their path from us.
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <returns></returns>
            public static double AoEInPathFormula(TrinityCacheObject cacheObject)
            {
                double weight = 0;

                // todo TimeBoundAvoidance no longer exists, update formula possibly using ...
                //var node = Core.Avoidance.Grid.GetNearestNode(cacheObject.Position);
                //if (node != null)
                //{
                //    weight = node.Weight * 
                //}

                //if (!Settings.Combat.Misc.KillMonstersInAoE)
                //    return weight;
                //var avoidances =
                //    CacheData.TimeBoundAvoidance.Where(
                //        aoe => MathUtil.IntersectsPath(aoe.Position, aoe.Radius, Player.Position,
                //            cacheObject.Position));
                //foreach (var avoidance in avoidances)
                //{
                //    weight -= 10 * (Math.Max(0, 15 - avoidance.Radius)) / 15;
                //}

                return weight;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <param name="monstersWithAffix"></param>
            /// <returns></returns>
            public static double AffixMonsterNearFormula(TrinityCacheObject cacheObject,
                List<TrinityCacheObject> monstersWithAffix)
            {
                var monsters = monstersWithAffix.Where(u => u.Position.Distance(cacheObject.Position) < 30f);
                return monsters.Aggregate<TrinityCacheObject, double>(0, (current, monster) => current + 5000 * Math.Max(0, (int)(30 - monster.Distance)));
            }

            /// <summary>
            ///     Gets the weight for Objects near Elites.
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <param name="eliteMonsters"></param>
            /// <returns></returns>
            public static double EliteMonsterNearFormula(TrinityCacheObject cacheObject,
                List<TrinityCacheObject> eliteMonsters)
            {
                double weight = 0;
                if (!Settings.Combat.Misc.IgnoreElites && !Settings.Combat.Misc.IgnoreChampions &&
                    !Settings.Combat.Misc.IgnoreRares)
                    return 0;
                var monsters = eliteMonsters.Where(u => u.Position.Distance(cacheObject.Position) < 10f);
                foreach (var monster in monsters)
                {
                    weight -= 50 * (Math.Max(0, 10 - monster.RadiusDistance)) / 10;
                }
                return weight;
            }

            /// <summary>
            ///     Gets the weight based on distance of object.  The closer the unit the more priority.
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <returns></returns>
            public static double ObjectDistanceFormula(TrinityCacheObject cacheObject)
            {
                var multipler = 500d;

                // DemonHunter is very fragile and should never run past close mobs, so increase distance weighting.
                if (cacheObject.IsUnit && Player.ActorClass == ActorClass.DemonHunter)
                    multipler = 1000d;

                // not units (items etc) shouldnt be impacted by the trash/non-trash slider setting.
                var range = 80f;

                if (cacheObject.Type == TrinityObjectType.Unit)
                {
                    range = cacheObject.IsElite
                        ? Settings.Combat.Misc.EliteRange
                        : Settings.Combat.Misc.NonEliteRange;
                }

                return multipler * ((range - cacheObject.RadiusDistance) / range);
            }

            public static double PackDensityFormula(TrinityCacheObject cacheObject, List<TrinityCacheObject> objects)
            {
                // Fix for questing/bounty mode when kill-all is required
                if (CombatBase.CombatOverrides.EffectiveTrashSize == 1)
                    return 0;

                //todo: find out why this formula is being applied to non-unit actors - destructibles, globes etc.

                var pack = objects.Where(
                    x => x.IsUnit && x.IsHostile && x.Position.Distance(cacheObject.Position) < CombatBase.CombatOverrides.EffectiveTrashRadius && (!Settings.Combat.Misc.IgnoreElites || !x.IsElite))
                    .ToList();

                var packDistanceValue = pack.Sum(mob => 100d * ((CombatBase.CombatOverrides.EffectiveTrashRadius - cacheObject.RadiusDistance) / CombatBase.CombatOverrides.EffectiveTrashRadius));

                return packDistanceValue < 0 ? 0 : packDistanceValue;
            }

            public static double RiftValueFormula(TrinityCacheObject cacheObject, List<TrinityCacheObject> objects)
            {
                var result = 0d;

                if (!RiftProgression.IsInRift || !cacheObject.IsUnit)
                    return result;

                // get all other units within cluster radius of this unit.
                var pack = objects.Where(x =>
                    x.Position.Distance(cacheObject.Position) < CombatBase.CombatOverrides.EffectiveTrashRadius &&
                    (!Settings.Combat.Misc.IgnoreElites || !x.IsElite))
                    .ToList();

                cacheObject.RiftValueInRadius = pack.Sum(mob => mob.RiftValuePct);

                // Only boost weight of this unit if above the total weight setting.
                if (cacheObject.RiftValueInRadius > Settings.Combat.Misc.RiftValueAlwaysKillClusterValue)
                    result = 100d * ((CombatBase.CombatOverrides.EffectiveTrashRadius - cacheObject.RadiusDistance) / CombatBase.CombatOverrides.EffectiveTrashRadius);


                return result <= 0 ? 0 : result;
            }

            /// <summary>
            ///     Gets the weight based on the Objects Health Percent.  Lower health gets more priority
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <returns></returns>
            public static double UnitHealthFormula(TrinityCacheObject cacheObject)
            {
                // Sometimes the game returns infinity hitpoints for whatever reason
                if (double.IsInfinity(cacheObject.HitPointsPct))
                    return 1;

                // Fix for near-zero rounding errors health=-0.000441553586736365
                if (Math.Abs(cacheObject.HitPointsPct - 1) < double.Epsilon)
                    return 0;

                return 200d * ((1 - cacheObject.HitPointsPct) / 100);
            }

            /// <summary>
            ///     Gets the weight based on the Objects distance from us and if they are in our path or next hotspot.
            /// </summary>
            /// <param name="cacheObject"></param>
            /// <returns></returns>
            public static double PathBlockedFormula(TrinityCacheObject cacheObject)
            {
                if (cacheObject.ActorSNO == 3349) // Belial, can't be pathed to.
                    return 0;

                if (cacheObject.IsUnit && RunningTime.TotalSeconds > 10 && Core.Grids.Avoidance.IsIntersectedByFlags(Player.Position, cacheObject.Position, AvoidanceFlags.ClosedDoor))
                    return -MaxWeight;

                if (!PlayerMover.IsCompletelyBlocked)
                    return 0;

                if (BlockingObjects(cacheObject) > 0)
                    return -MaxWeight;

                // todo fix this its causing massive bouts of the bot doing nothing while standing in groups of mobs.             
                //if(!cacheObject.IsUnit)
                //    return BlockingMonsterObjects(cacheObject) * -100d;

                if (!cacheObject.IsUnit || PlayerMover.IsCompletelyBlocked && cacheObject.Distance > 15f && !cacheObject.IsElite && Settings.Combat.Misc.AttackWhenBlocked)
                    return -MaxWeight;

                return 0;
            }

            //public static int BlockingMonsterObjects(TrinityCacheObject cacheObject)
            //{
            //    var monsterCount = CacheData.MonsterObstacles.Count(
            //        ob => MathUtil.IntersectsPath(ob.Position, ob.Radius, CacheData.Player.Position,
            //            cacheObject.Position));

            //    return monsterCount;
            //}

            /// <summary>
            ///     Navigation obstacles are more critical than monster obstacles, these include script locked gates, large barricades
            ///     etc
            ///     They cannot be walked passed, and everything beyond them needs to be ignored.
            /// </summary>
            public static int BlockingObjects(TrinityCacheObject cacheObject)
            {
                var navigationCount = 0;

                //var navigationCount = CacheData.NavigationObstacles.Count(
                //    ob => MathUtil.IntersectsPath(ob.Position, ob.Radius, CacheData.Player.Position,
                //        cacheObject.Position));

                //var gate = CacheData.NavigationObstacles.FirstOrDefault(o => o.ActorSNO == 108466);
                //if (gate != null)
                //    Logger.Log("NavigationObstacles contains gate {0} blockingCount: {1}={2}", gate.Name, cacheObject.InternalName, navigationCount);

                return navigationCount;
            }
            //public static double LastTargetFormula(TrinityCacheObject cacheObject)
            //{
            //    if (PlayerMover.IsCompletelyBlocked)
            //        return 0;

            //    return cacheObject.RActorGuid == LastTargetRactorGUID ? 250d : 0d;
            //}

            #endregion
        }
    }

    public class TargettingInfo
    {
        public string ObjectHash;
        public int RActorGuid;
        public string Name;
        public DateTime CreatedTime = DateTime.UtcNow;
        public DateTime LastTargetedTime;
        public int TargetedTimes;
        public int BlacklistedTimes;
        public TimeSpan TimeAsCurrentTarget;

        public override string ToString()
        {
            return base.ToString() + $"TimeAsCurrentTarget(ms)={TimeAsCurrentTarget.TotalMilliseconds:N2}";
        }
    }
}