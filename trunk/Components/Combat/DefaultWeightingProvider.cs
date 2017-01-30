using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Trinity.Components.Combat.Resources;
using Trinity.Coroutines.Resources;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Items;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile.Common;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Components.Combat
{
    public interface IWeightingProvider
    {
        TrinityActor WeightActors(IEnumerable<TrinityActor> objects);

        bool ShouldIgnore(TrinityActor actor);
    }

    public class DefaultWeightingProvider : IWeightingProvider
    {

        /// <summary>
        /// Check if settings require the actor not be weighted.
        /// </summary>
        public bool ShouldIgnore(TrinityActor actor)
        {
            switch (actor.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                    return WeightingUtils.ShouldIgnoreGlobe(actor as TrinityItem);
                case TrinityObjectType.Unit:

                    if (actor.IsElite && !actor.IsBoss)
                        return ShouldIgnoreElite(actor);
                    else if (actor.IsTrashMob)
                        return WeightingUtils.ShouldIgnoreTrash(actor);

                    break;
            }

            return false;
        }

        /// <summary>
        /// todo refactor this is a mess
        /// </summary>
        public TrinityActor WeightActors(IEnumerable<TrinityActor> actors)
        {
            if (Combat.Routines.Current == null)
                return null;

            var objects = actors.ToList();

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

                //var isHighLevelGrift = Core.Player.TieredLootRunlevel > 55;

                //var eliteCount = CombatBase.IgnoringElites
                //    ? 0
                //    : objects.Count(u => u.IsUnit && u.IsElite);
                //var avoidanceCount = Core.Settings.Combat.Misc.AvoidAOE
                //    ? 0
                //    : objects.Count(o => o.Type == TrinityObjectType.Avoidance && o.Distance <= 50f);

                //var avoidanceNearby = Core.Settings.Combat.Misc.AvoidAOE &&
                //                      objects.Any(
                //                          o => o.Type == TrinityObjectType.Avoidance && o.Distance <= 15f);

                //var prioritizeCloseRangeUnits = (avoidanceNearby || _forceCloseRangeTarget || Player.IsRooted ||
                //                                 DateTime.UtcNow.Subtract(StuckHandler.LastStuckTime).TotalMilliseconds < 1000 &&
                //                                 ObjectCache.Count(u => u.IsUnit && u.RadiusDistance < 12f) >= 5);

                //var hiPriorityHealthGlobes = Core.Settings.Combat.Misc.HiPriorityHG;

                var isHealthEmergency = (Core.Player?.CurrentHealthPct <= Combat.Routines.Current?.EmergencyHealthPct);

                var isGateNearby = false;
                var isPriorityInteractableNearby = false;

                foreach (var r in Core.Actors.AllRActors)
                {
                    if (r.Distance <= 40f && r.ActorSnoId == (int)SNOActor.x1_Fortress_Portal_Switch)
                        isGateNearby = true;

                    if (r.Distance <= 80f && r.ActorSnoId == 454511) //p43_AD_Valor_BloodStone D1 Switch
                        isPriorityInteractableNearby = true;

                    if (r.Distance <= 40f && (r.GizmoType == GizmoType.Switch || r.IsInteractWhitelisted))
                        isPriorityInteractableNearby = true;
                }

                var shouldIgnorePackSize = Combat.Routines.Current.ShouldIgnorePackSize();

                var bossNearby = Core.Player.IsInBossEncounter && Core.Targets.Any(u => u.IsBoss && u.Distance < 60f);

                //var killQuestStepTypes = new HashSet<QuestStepObjectiveType>
                //{
                //    QuestStepObjectiveType.KillAllMonstersInWorld,
                //    QuestStepObjectiveType.KillAnyMonsterInLevelArea,
                //    QuestStepObjectiveType.KillElitePackInLevelArea,
                //    QuestStepObjectiveType.KillGroup,
                //    QuestStepObjectiveType.KillMonster,
                //    QuestStepObjectiveType.KillMonsterFromFamily,
                //    QuestStepObjectiveType.KillMonsterFromGroup,
                //};

                //var inKillQuest = ZetaDia.ActInfo.ActiveQuests.FirstOrDefault(
                //    q => q.QuestRecord.Steps.Any(
                //        s => s.QuestStepObjectiveSet.QuestStepObjectives.Any(
                //            o => killQuestStepTypes.Contains(o.ObjectiveType)))) != null;

                //var profileTagCheck = false;

                //var behaviorName = "";
                //if (ProfileManager.CurrentProfileBehavior != null)
                //{
                //    var behaviorType = ProfileManager.CurrentProfileBehavior.GetType();
                //    behaviorName = behaviorType.Name;
                //    if (!Core.Settings.Combat.Misc.ProfileTagOverride && CombatBase.IsQuestingMode ||
                //        behaviorType == typeof(WaitTimerTag) ||
                //        behaviorType == typeof(UseTownPortalTag) ||
                //        behaviorName.ToLower().Contains("townrun") ||
                //        behaviorName.ToLower().Contains("townportal"))
                //    {
                //        profileTagCheck = true;
                //    }
                //}

                var usingTownPortal = TrinityTownRun.IsWantingTownRun;

                // Highest weight found as we progress through, so we can pick the best target at the end (the one with the highest weight)
                HighestWeightFound = 0;

                var isStuck = Navigator.StuckHandler.IsStuck;

                var elites = new List<TrinityActor>();
                var ignoredByAffixElites = new List<TrinityActor>();
                _ignoredAffixes = Core.Settings.Weighting.IgnoreAffixes.GetFlags<MonsterAffixes>().ToList();

                foreach (var unit in objects.Where(u => u.IsUnit && u.IsElite))
                {
                    if (_ignoredAffixes.Any(a => unit.MonsterAffixes.HasFlag(a)))
                    {
                        ignoredByAffixElites.Add(unit);
                    }

                    string reason;
                    if (!ShouldIgnoreElite(unit, out reason))
                        elites.Add(unit);
                }



                #endregion

                //Logger.Log(TrinityLogLevel.Debug, LogCategory.Weight,
                //    "Starting weights: packSize={0} packRadius={1} MovementSpeed={2:0.0} Elites={3} AoEs={4} disableIgnoreTag={5} ({6}) closeRangePriority={7} townRun={8} questingArea={9} level={10} isQuestingMode={11} healthGlobeEmerg={12} hiPriHG={13} hiPriShrine={14}",
                //    CombatBase.CombatOverrides.EffectiveTrashSize, CombatBase.CombatOverrides.EffectiveTrashRadius,
                //    movementSpeed,
                //    eliteCount, avoidanceCount, profileTagCheck, behaviorName,
                //    PlayerMover.IsCompletelyBlocked, usingTownPortal,
                //    DataDictionary.QuestLevelAreaIds.Contains(Core.Player.LevelAreaId), Core.Player.Level,
                //    CombatBase.IsQuestingMode, isHealthEmergency, hiPriorityHealthGlobes, hiPriorityShrine);

                if (Core.Settings.Weighting.GoblinPriority == GoblinPriority.Kamikaze)
                {
                    var goblin = objects.FirstOrDefault(u => u.IsTreasureGoblin && u.Distance <= 200f);
                    if (goblin != null && !isStuck && !PlayerMover.IsCompletelyBlocked && !Core.Grids.Avoidance.IsIntersectedByFlags(Core.Player.Position, goblin.Position, AvoidanceFlags.ClosedDoor))
                    {
                        objects.Where(x => !x.IsPlayer).ForEach(o =>
                        {
                            o.WeightInfo = string.Empty;
                            o.Weight = 0;
                        });

                        IsDoingGoblinKamakazi = true;
                        KamakaziGoblin = goblin;
                        Logger.Log("Going Kamakazi on Goblin '{0} ({1})' Distance={2}", goblin.InternalName, goblin.ActorSnoId, goblin.Distance);
                        goblin.WeightInfo = "Kamakazi Target";
                        goblin.Weight = MaxWeight;
                        return goblin;
                    }
                    IsDoingGoblinKamakazi = false;
                }
                else
                {
                    IsDoingGoblinKamakazi = false;
                }

                //var riftProgressionKillAll = RiftProgression.IsInRift && !RiftProgression.IsGaurdianSpawned && !RiftProgression.RiftComplete &&
                //                             Core.Settings.Combat.Misc.RiftProgressionAlwaysKillPct < 100 && RiftProgression.CurrentProgressionPct < 100 &&
                //                             RiftProgression.CurrentProgressionPct >= Core.Settings.Combat.Misc.RiftProgressionAlwaysKillPct;

                //if (riftProgressionKillAll != _riftProgressionKillAll)
                //{
                //    _riftProgressionKillAll = riftProgressionKillAll;
                //    if (riftProgressionKillAll)
                //    {
                //        Logger.Log($"Rift Progression is now at {RiftProgression.CurrentProgressionPct} - Killing everything!");
                //        CombatBase.CombatMode = CombatMode.KillAll;
                //    }
                //    else
                //    {
                //        Logger.LogVerbose($"Reverting rift progression kill all mode back to normal combat");
                //        CombatBase.CombatMode = CombatMode.Normal;
                //    }
                //}

                var routine = Combat.Routines.Current;

                TrinityActor bestTarget = null;

                try
                {
                    #region Foreach Loop

                    var playerInCriticalAvoidance = Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position);
                    var leaderTarget = Combat.Party.Leader != null ? PartyHelper.FindLocalActor(Combat.Party.Leader.Target) : null;
                    var isLeader = Combat.Party.Leader?.IsMe ?? false;


                    foreach (var cacheObject in objects.Where(x => !x.IsPlayer))
                    {
                        if (cacheObject == null || !cacheObject.IsValid || cacheObject.Type == TrinityObjectType.Unknown)
                            continue;

                        if (isGateNearby && !cacheObject.HasBeenWalkable)
                        {
                            cacheObject.WeightInfo += "ForedWalkableForDeathGatesNeabry";
                            continue;
                        }

                        cacheObject.Weight = 0;
                        cacheObject.WeightInfo = string.Empty;
                        var reason = string.Empty;

                        if (routine != null && routine.SetWeight(cacheObject))
                        {
                            bestTarget = GetNewBestTarget(cacheObject, bestTarget);
                            continue;
                        }

                        if (Combat.Routines.Current.ShouldIgnoreNonUnits() && !cacheObject.IsUnit)
                        {
                            cacheObject.WeightInfo += "FocussingUnits";
                            continue;
                        }

                        if (cacheObject.IsUnit && !cacheObject.IsWalkable && Core.ProfileSettings.Options.CurrentSceneOptions.AlwaysRayWalk)
                        {
                            cacheObject.WeightInfo += "ProfileSceneSetting-AlwaysRayWalk";
                            continue;
                        }

                        if (PlayerMover.IsCompletelyBlocked)
                        {
                            cacheObject.WeightInfo += "PlayerBlocked";

                            //if (Core.Settings.Combat.Misc.AttackWhenBlocked)
                            //{
                            if (!cacheObject.IsUnit)
                            {
                                cacheObject.Weight = 0;
                                cacheObject.WeightInfo += "Ignoring because we are blocked. ";
                                continue;
                            }
                            if (cacheObject.RadiusDistance > 12f && cacheObject.Type != TrinityObjectType.Barricade)
                            {
                                cacheObject.Weight = 0;
                                cacheObject.WeightInfo += "Ignoring Blocked Far Away ";
                                continue;
                            }
                            cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} because we are Blocked.";
                            cacheObject.Weight = MaxWeight + ObjectDistanceFormula(cacheObject);
                            bestTarget = GetNewBestTarget(cacheObject, bestTarget);
                            continue;
                            //}
                        }

                        if (IsDoingGoblinKamakazi)
                        {
                            if (cacheObject.RActorId == KamakaziGoblin.RActorId && !isStuck)
                            {
                                cacheObject.Weight = MaxWeight;
                                cacheObject.WeightInfo += $"Maxxing {cacheObject.InternalName} - Goblin Kamakazi Run ";
                                bestTarget = GetNewBestTarget(cacheObject, bestTarget);
                                break;
                            }
                            continue;
                        }

                        if (isGateNearby && !cacheObject.IsWalkable)
                        {
                            cacheObject.Weight = 0;
                            cacheObject.WeightInfo += "Ignoring Unwalkable by Death Gate ";
                            continue;
                        }

                        if (cacheObject.IsUnit && cacheObject.Distance < 35f)
                        {
                            if (!cacheObject.HasBeenInLoS)
                            {
                                cacheObject.Weight = 0;
                                cacheObject.WeightInfo += "Ignoring - Hasn't been in line of sight";
                                continue;
                            }
                        }
                        if (cacheObject.GizmoType == GizmoType.BreakableChest)
                        {
                            if (!cacheObject.HasBeenInLoS)
                            {
                                cacheObject.Weight = 0;
                                cacheObject.WeightInfo += "Ignoring - Hasn't been in line of sight";
                                continue;
                            }
                        }
                        else if (cacheObject.IsDestroyable)
                        {
                            if (!cacheObject.HasBeenWalkable && cacheObject.GizmoType != GizmoType.BreakableDoor && cacheObject.GizmoType != GizmoType.Door && cacheObject.Type != TrinityObjectType.Barricade)
                            {
                                cacheObject.Weight = 0;
                                cacheObject.WeightInfo += "Ignoring - Destroyable hasn't been Walkable";
                                continue;
                            }
                        }
                        else if (cacheObject.Distance > 15f && !cacheObject.HasBeenWalkable && !cacheObject.IsItem && cacheObject.Type != TrinityObjectType.ProgressionGlobe && cacheObject.Type == TrinityObjectType.Door)
                        {
                            cacheObject.Weight = 0;
                            cacheObject.WeightInfo += "Ignoring - Hasn't been Walkable";
                            continue;
                        }

                        var behindClosedDoor =
                            Core.Grids.Avoidance.IsIntersectedByFlags(Core.Player.Position,
                                cacheObject.Position, AvoidanceFlags.ClosedDoor);

                        if (behindClosedDoor && cacheObject.GizmoType != GizmoType.Door)
                        {
                            cacheObject.WeightInfo += "BehindClosedDoor";

                            if (!cacheObject.IsElite && !cacheObject.IsTreasureGoblin &&
                                !cacheObject.IsInteractWhitelisted)
                            {
                                cacheObject.Weight = 0;
                                continue;
                            }
                        }

                        var item = cacheObject as TrinityItem;

                        if (WeightingUtils.ShouldIgnoreSpecialTarget(cacheObject, out reason))
                        {
                            cacheObject.WeightInfo += reason;
                            continue;
                        }

                        //if (!Settings.Advanced.BetaPlayground && Core.Avoidance.InCriticalAvoidance(cacheObject.Position) || Core.Avoidance.Grid.IsIntersectedByFlags(cacheObject.Position, ZetaDia.Me.Position, AvoidanceFlags.CriticalAvoidance))
                        //{
                        //    cacheObject.Weight = 0;
                        //    cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Intersected by Critical Avoidance.";
                        //    continue;
                        //}

                        if (cacheObject.IsWalkable && GameData.RayWalkTargetingOnlyActors.Contains(cacheObject.ActorSnoId))
                        {
                            cacheObject.WeightInfo += "AlwaysRequiresRaywalk";
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

                                    bool elitesInRangeOfUnit = objects.Any(
                                        u =>
                                            u.AcdId != cacheObject.AcdId &&
                                            u.IsElite &&
                                            u.Position.Distance2D(cacheObject.Position) <= 15f);

                                    int nearbyTrashCount =
                                        objects.Count(u => u.IsUnit && u.HitPoints > 0 && u.IsTrashMob &&
                                                           cacheObject.Position.Distance(u.Position) <=
                                                           20f);

                                    //bool ignoreSummoner = cacheObject.IsSummoner && !Core.Settings.Combat.Misc.ForceKillSummoners;
                                    //bool ignoreSummoner = cacheObject.IsSummoner && !Core.Settings.Combat.Misc.ForceKillSummoners;

                                    var isBoss = cacheObject.MonsterQuality ==
                                                 MonsterQuality.Boss;
                                    var isUnique = cacheObject.MonsterQuality ==
                                                   MonsterQuality.Unique;
                                    var isRare = cacheObject.MonsterQuality ==
                                                 MonsterQuality.Rare;
                                    var isMinion = cacheObject.MonsterQuality ==
                                                   MonsterQuality.Minion;
                                    var isChampion = cacheObject.MonsterQuality ==
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
                                            null, cacheObject.IsQuestMonster, cacheObject.IsMinimapActive,
                                            cacheObject.IsBountyObjective);

                                    #region Basic Checks

                                    if (Combat.CombatMode == CombatMode.KillAll || Core.Quests.IsKillAllRequired)
                                    {
                                        //Dist:                160     140     120     100      80     60     40      20      0
                                        //Weight (25k Max):    -77400  -53400  -32600  -15000  -600   10600  18600   23400   25000
                                        //
                                        //Formula:   MaxWeight-(Distance * Distance * RangeFactor)
                                        //           RangeFactor effects how quickly weights go into negatives on far distances.                                                                    

                                        var ignoreTrashTooFarAway = cacheObject.IsTrashMob && cacheObject.Distance > Combat.Routines.Current.TrashRange;
                                        var ignoreElitesTooFarAway = cacheObject.IsElite && cacheObject.Distance > Combat.Routines.Current.EliteRange;

                                        if (ignoreTrashTooFarAway || ignoreElitesTooFarAway)
                                        {
                                            cacheObject.WeightInfo += $"Ignore Far Away Stuff TrashRange={Combat.Routines.Current.TrashRange} EliteRange={Combat.Routines.Current.EliteRange}";
                                            cacheObject.Weight = 0;
                                            break;
                                        }

                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += "Kill All Mode";
                                        break;
                                    }

                                    if (WeightingUtils.ShouldIgnoreTrash(cacheObject, out reason))
                                    {
                                        cacheObject.WeightInfo += reason;
                                        break;
                                    }

                                    // Note, a check here to ignore monsters in critical avoidance (molten core/arcane) was causing elites to be skipped.
                                    // what needs to happen is the bot doesnt MOVE into the critical area but can still attack the monsters
                                    // Movement spells that might cause the bot to teleport ontop of a molten core the moment it becomes targetted
                                    // should have their own checks at power selection to make sure that doesnt happen.
                                    if (Core.Avoidance.InCriticalAvoidance(cacheObject.Position) && Core.Avoidance.InCriticalAvoidance(Core.Player.Position))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - in Critical Avoidance.";
                                        break;
                                    }

                                    if (cacheObject.HitPointsPct <= 0)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - is dead";
                                        break;
                                    }

                                    //TargettingInfo targetingInfo;
                                    //TargetHistory.TryGetValue(bestTarget.ObjectHash, out targetingInfo)

                                    if (!cacheObject.IsBoss && Core.Avoidance.Grid.IsIntersectedByFlags(cacheObject.Position, Core.Player.Position, AvoidanceFlags.ClosedDoor))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring Behind Closed Door";
                                        break;
                                    }

                                    //if (Core.Player.InActiveEvent && objects.Any(o => o.IsEventObject))
                                    //{
                                    //    Vector3 eventObjectPosition = objects.FirstOrDefault(o => o.IsEventObject).Position;
                                    //    if (!cacheObject.IsQuestMonster && cacheObject.Position.Distance(eventObjectPosition) > 35)
                                    //    {
                                    //        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Too Far From Event";
                                    //        break;
                                    //    }
                                    //}

                                    //if (isHealthEmergency && cacheObject.Type != TrinityObjectType.HealthGlobe && !PlayerMover.IsCompletelyBlocked)
                                    //{
                                    //    // Many 'bot ignored some elites' complaints are due to priority globe aquisition.
                                    //    if (cacheObject.IsElite && Core.Player.CurrentHealthPct < Math.Min(0.35, Core.Settings.Combat.Misc.HealthGlobeLevel))
                                    //    {
                                    //        Logger.LogDebug($"Health Globe Emergency Ignoring Elite {cacheObject.InternalName} HealthPct={Core.Player.CurrentHealthPct}");
                                    //    }
                                    //    else
                                    //    {
                                    //        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for Priority Health Globe";
                                    //        break;
                                    //    }
                                    //}

                                    //if (getHiPriorityShrine && cacheObject.Type != TrinityObjectType.Shrine && !PlayerMover.IsCompletelyBlocked)
                                    //{
                                    //    cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for Priority Shrine ";
                                    //    break;
                                    //}

                                    //if (getHiPriorityContainer && cacheObject.Type != TrinityObjectType.Container && !PlayerMover.IsCompletelyBlocked)
                                    //{
                                    //    cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for Priority Container";
                                    //    break;
                                    //}

                                    //Monster is in cache but not within kill range
                                    var killRange = DistanceForObjectType(cacheObject);
                                    var isQuestNpc = cacheObject.IsNpc && cacheObject.IsQuestGiver;

                                    if (!cacheObject.IsBoss && !cacheObject.IsTreasureGoblin &&
                                        LastTargetRActorGuid != cacheObject.RActorId &&
                                        !cacheObject.IsMinimapActive &&
                                        !cacheObject.IsQuestMonster && !cacheObject.IsBountyObjective &&
                                        cacheObject.RadiusDistance > killRange && !isQuestNpc)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - out of Kill Range ({killRange})";
                                        break;
                                    }

                                    if (cacheObject.IsTreasureGoblin)
                                    {
                                        // Original Trinity stuff for priority handling now
                                        switch (Core.Settings.Weighting.GoblinPriority)
                                        {
                                            case GoblinPriority.Normal:
                                                cacheObject.WeightInfo += "GoblinNormal ";
                                                cacheObject.Weight += 500d;
                                                break;
                                            case GoblinPriority.Prioritize:
                                                cacheObject.WeightInfo += "GoblinPrioritize ";
                                                cacheObject.Weight += 1000d;
                                                break;
                                            case GoblinPriority.Kamikaze:
                                                cacheObject.WeightInfo += "GoblinKamikaze ";
                                                cacheObject.Weight += MaxWeight;
                                                break;
                                        }
                                    }

                                    #endregion

                                    if (Core.Player.CurrentSceneSnoId == 28768 && cacheObject.ActorSnoId == (int)SNOActor.x1_Tentacle_Goatman_Melee_A && !cacheObject.IsMinimapActive)
                                    {
                                        cacheObject.WeightInfo += "Invisible Goat in Cow King rift event ";
                                        cacheObject.Weight = 0;
                                        break;
                                    }

                                    if (deadPlayer)
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Adding {cacheObject.InternalName} because we have a dead party member.";
                                        cacheObject.Weight += MaxWeight;
                                        break;
                                    }

                                    var isQuestGiverOutsideCombat = cacheObject.IsQuestGiver && !ZetaDia.Me.IsInCombat;
                                    if (!cacheObject.IsHostile && !isQuestGiverOutsideCombat && !cacheObject.IsQuestMonster)
                                    {
                                        cacheObject.WeightInfo += "Unit Not Hostile";
                                        cacheObject.Weight = MinWeight;
                                        break;
                                    }

                                    //if (!cacheObject.IsElite)
                                    //{
                                    //    if (Core.Settings.Weighting.EliteWeighting == SettingMode.Disabled)
                                    //    {
                                    //        cacheObject.WeightInfo += $"Ignore(Trash=Disabled)";
                                    //        break;
                                    //    }
                                    //    if (Core.Settings.Weighting.EliteWeighting == SettingMode.Selective && !Core.StuckHandler.IsStuck)
                                    //    {
                                    //        cacheObject.WeightInfo += $"Ignore(Trash=DisabledUnlessStuck)";
                                    //        break;
                                    //    }
                                    //}

                                    //if (cacheObject.IsInvulnerable &&
                                    //         Core.Settings.Combat.Misc.IgnoreMonstersWhileReflectingDamage)
                                    //{
                                    //    cacheObject.WeightInfo +=
                                    //        $"Ignoring {cacheObject.InternalName} because of Invulnerability ";
                                    //    cacheObject.Weight = MinWeight;
                                    //    break;
                                    //}

                                    if (Core.Player.CurrentHealthPct <= 0.25 && ZetaDia.Service.Party.NumPartyMembers < 1)
                                    {
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} Below Health Threshold ";
                                    }
                                    else if (cacheObject.IsQuestMonster || cacheObject.IsEventObject || cacheObject.IsBountyObjective)
                                    {
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} Quest Monster | Bounty | Event Objective ";
                                    }
                                    else if (cacheObject.Distance < 25 && Core.Player.IsCastingTownPortalOrTeleport() && !Legendary.HomingPads.IsEquipped)
                                    {
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} because of Town Portal";
                                    }
                                    //else if (isInHotSpot && PlayerMover.IsCompletelyBlocked)
                                    //{
                                    //    cacheObject.WeightInfo +=
                                    //        string.Format("Adding {0} due to being in Path or Hotspot ",
                                    //            cacheObject.InternalName);
                                    //}
                                    else if (PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} because we seem to be stuck *OR* if not ranged and currently rooted ";
                                    }
                                    //else if (DataDictionary.MonsterCustomWeights.ContainsKey(cacheObject.ActorSnoId))
                                    //{
                                    //    cacheObject.WeightInfo +=
                                    //        string.Format(
                                    //            "Adding {0} because monsters from the dictionary/hashlist set at the top of the code ",
                                    //            cacheObject.InternalName);
                                    //}
                                    //else if ((cacheObject.ActorSnoId == 210120 || cacheObject.ActorSnoId == 210268) &&
                                    //         cacheObject.Distance <= 25f)
                                    //{
                                    //    cacheObject.WeightInfo += string.Format("Adding {0} because of Blocking",
                                    //        cacheObject.InternalName);
                                    //}

                                    #region Trash Mob

                                    else if (cacheObject.IsTrashMob)
                                    {
                                        //var isAlwaysKillByValue = RiftProgression.IsInRift &&
                                        //                          cacheObject.RiftValuePct > 0 &&
                                        //                          cacheObject.RiftValuePct >
                                        //                          Core.Settings.Combat.Misc.RiftValueAlwaysKillUnitsAbove;
                                        //if (isAlwaysKillByValue)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format("IsHighRiftValue {0}", cacheObject.RiftValuePct);
                                        //}

                                        //if (Core.Settings.Combat.Misc.IgnoreHighHitPointTrash && !isAlwaysKillByValue)
                                        //{
                                        //    HashSet<string> highHitPointTrashMobNames = new HashSet<string>
                                        //    {
                                        //        "mallet", //
                                        //        "monstrosity", //
                                        //        "triune_berserker", //
                                        //        "beast_d",
                                        //        "thousandpounder", //5581
                                        //        "westmarchbrute", //258678, 332679
                                        //        "unburied" //6359
                                        //    };

                                        //    var unitName = cacheObject.InternalName.ToLower();
                                        //    if (highHitPointTrashMobNames.Any(name => unitName.Contains(name)))
                                        //    {
                                        //        cacheObject.WeightInfo +=
                                        //            string.Format("Ignoring {0} for High Hp Mob.",
                                        //                cacheObject.InternalName);
                                        //        break;
                                        //    }
                                        //}
                                        //else if (cacheObject.HitPointsPct <=
                                        //         Core.Settings.Combat.Misc.ForceKillTrashBelowHealth)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format(
                                        //            "Adding {0} because it is below the minimum trash mob health",
                                        //            cacheObject.InternalName);
                                        //}
                                        if (cacheObject.IsSummoner)
                                        {
                                            cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} because he is a summoner";
                                            //cacheObject.Weight += 100d;
                                        }
                                        if (Core.Player.IsInBossEncounter)
                                        {
                                            cacheObject.WeightInfo += $"BossEncounter";
                                        }
                                        //else if (cacheObject.HitPointsPct <
                                        //         Core.Settings.Combat.Misc.IgnoreTrashBelowHealthDoT &&
                                        //         cacheObject.HasDotDps)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format(
                                        //            "Ignoring {0} - Hitpoints below Health/DoT Threshold ",
                                        //            cacheObject.InternalName);
                                        //    break;
                                        //}
                                        else if (Combat.CombatMode == CombatMode.Questing)
                                        {
                                            cacheObject.WeightInfo += $"Questing Mode - Ignoring Trash Pack Size Setting.";
                                        }
                                        else if (leaderTarget != null && !isLeader && leaderTarget.Distance < 60f && Combat.Party.Leader.IsInCombat)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring Trash Pack Size for Leader's Target";
                                        }
                                        else if (shouldIgnorePackSize)
                                        {
                                            cacheObject.WeightInfo += $"Routine Ignoring Trash Pack Size.";
                                        }
                                        else if (nearbyTrashCount < Combat.Routines.Current.ClusterSize && !Core.Minimap.MinimapIconAcdIds.Contains(cacheObject.AcdId) &&
                                                 !GameData.CorruptGrowthIds.Contains(cacheObject.ActorSnoId) && !isQuestGiverOutsideCombat && !bossNearby)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring Below TrashPackSize ({nearbyTrashCount} < {Combat.Routines.Current.ClusterSize})";
                                            break;
                                        }
                                        else
                                            cacheObject.WeightInfo += $" All Filters Passed: Adding {cacheObject.InternalName} by Default.";
                                    }

                                    #endregion

                                    #region Elite / Rares / Uniques

                                    else if (isUnique || isBoss || isRare || isMinion || isChampion)
                                    {
                                        //XZ - Please add Ignore below health for elite.
                                        //if ((cacheObject.HitPointsPct <
                                        //     Settings.Combat.Misc.IgnoreEliteBelowHealthDoT) &&
                                        //    cacheObject.HasDotDps)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format("Ignoring {0} - Hitpoints below Health/DoT Threshold ", cacheObject.InternalName);
                                        //    break;
                                        //}

                                        if (cacheObject.IsSpawningBoss)
                                        {
                                            cacheObject.WeightInfo += string.Format("Boss is spawning", cacheObject.InternalName);
                                            cacheObject.Weight += 0;
                                            break;
                                        }

                                        //if (cacheObject.HitPointsPct <=
                                        //    Core.Settings.Combat.Misc.ForceKillElitesHealth && !cacheObject.IsMinion)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format("Adding {0} for Elite Under Health Threshold.",
                                        //            cacheObject.InternalName);
                                        //    cacheObject.Weight += MaxWeight;
                                        //    break;
                                        //}

                                        //if (TargetUtil.NumMobsInRangeOfPosition(cacheObject.Position,
                                        //    CombatBase.CombatOverrides.EffectiveTrashRadius) >=
                                        //    CombatBase.CombatOverrides.EffectiveTrashSize &&
                                        //    Core.Settings.Combat.Misc.ForceKillClusterElites)
                                        //{
                                        //    cacheObject.WeightInfo +=
                                        //        string.Format("Adding {0} for Elite Inside Cluster.",
                                        //            cacheObject.InternalName);

                                        //}
                                        //else 

                                        if (!cacheObject.IsBoss)
                                        {
                                            if (ShouldIgnoreElite(cacheObject, out reason))
                                            {
                                                Logger.Log(LogCategory.Weight, $"{reason}");
                                                cacheObject.WeightInfo += reason;
                                                break;
                                            }
                                        }

                                        //if (Core.Settings.Combat.Misc.IgnoreHighHitPointElites)
                                        //{
                                        //    HashSet<string> highHitPointTrashMobNames = new HashSet<string>
                                        //    {
                                        //        "mallet", //
                                        //        "monstrosity", //
                                        //        "triune_berserker", //
                                        //        "beast_d",
                                        //        "thousandpounder", //5581
                                        //        "westmarchbrute", //258678, 332679
                                        //        "unburied" //6359
                                        //    };

                                        //    var unitName = cacheObject.InternalName.ToLower();
                                        //    if (highHitPointTrashMobNames.Any(name => unitName.Contains(name)))
                                        //    {
                                        //        cacheObject.WeightInfo +=
                                        //            string.Format("Ignoring {0} for High Hp Elite Mob.",
                                        //                cacheObject.InternalName);
                                        //        break;
                                        //    }
                                        //}


                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} default Elite ";
                                        cacheObject.Weight += EliteFormula(cacheObject);
                                    }

                                    #endregion

                                    var dist = ObjectDistanceFormula(cacheObject);
                                    var last = 0; //LastTargetFormula(cacheObject);
                                    var pack = PackDensityFormula(cacheObject, objects);
                                    var health = UnitHealthFormula(cacheObject);
                                    var path = PathBlockedFormula(cacheObject);
                                    var reflect = AffixMonsterNearFormula(cacheObject, ignoredByAffixElites);
                                    var elite = EliteMonsterNearFormula(cacheObject, elites);
                                    var aoe = AoENearFormula(cacheObject) + AoEInPathFormula(cacheObject);

                                    var leaderTargetBoost = 1;
                                    if (leaderTarget != null && !isLeader && cacheObject.AcdId == leaderTarget.AcdId)
                                    {
                                        cacheObject.WeightInfo += $"Doubled weight for Leaders Target";
                                        leaderTargetBoost = 2;
                                    }

                                    cacheObject.Weight += (dist + last + pack + health + path + reflect + elite + aoe) * leaderTargetBoost;

                                    cacheObject.WeightInfo +=
                                        $" dist={dist:0.0} last={last:0.0} pack={pack:0.0} health={health:0.0} path={path:0.0} reflect={reflect:0.0} elite={elite:0.0} aoe={aoe:0.0}";

                                    break;
                                }

                            #endregion

                            #region Item

                            case TrinityObjectType.BloodShard:
                            case TrinityObjectType.Item:
                                {
                                    if (item.TrinityItemType == TrinityItemType.HoradricRelic && Core.Player.BloodShards >= Core.Player.MaxBloodShards)
                                    {
                                        cacheObject.Weight = 0;
                                        cacheObject.WeightInfo += "Max BloodShards";
                                        continue;
                                    }

                                    if (!cacheObject.IsWalkable && !cacheObject.HasBeenWalkable && cacheObject.Distance > 50f)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring unreachable.";
                                        break;
                                    }

                                    // Campaign A5 Quest "Lost Treasure of the Nephalem" - have to interact with nephalem switches first... 
                                    // Quest: x1_Adria, Id: 257120, Step: 108 - disable all looting, pickup, and objects
                                    if (Core.Player.WorldType != Act.OpenWorld && Core.Player.CurrentQuestSNO == 257120 &&
                                        Core.Player.CurrentQuestStep == 108)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} For Quest";
                                        break;
                                    }

                                    if (Core.Player.ParticipatingInTieredLootRun && objects.Any(m => m.IsUnit && m.IsBoss))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} Loot Run Boss";
                                        break;
                                    }

                                    if (Core.Player.IsInTown)
                                    {
                                        var trinityItem = Core.Actors.GetItemByAnnId(cacheObject.AnnId);
                                        if (trinityItem != null)
                                        {
                                            if (Core.Settings.Items.DontPickupInTown && !trinityItem.IsItemAssigned)
                                            {
                                                cacheObject.WeightInfo += $"Ignoring DontPickUpInTown Setting.";
                                                break;
                                            }
                                            if (!trinityItem.CanPickupItem)
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

                                    if (Core.Settings.Items.DisableLootingInCombat && Combat.IsInCombat && item.Distance > 8f)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring(DisableLootingInCombat)";
                                        break;
                                    }

                                    // Don't pickup items if we're doing a TownRun
                                    if (!Combat.Loot.IsBackpackFull && !item.IsPickupNoClick)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} for TownRun";
                                        break;
                                    }

                                    // Death's Breath Priority
                                    if (cacheObject.ActorSnoId == 361989 || cacheObject.ActorSnoId == 449044)
                                    {
                                        if (IgnoreWhenInAvoidance(cacheObject))
                                            break;

                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} - Death's Breath";
                                        break;
                                    }

                                    // Give legendaries max weight, always
                                    if (cacheObject.ItemQualityLevel >= ItemQuality.Legendary)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} - Legendary";
                                        break;
                                    }

                                    //Non Legendaries
                                    if (cacheObject.ItemQualityLevel < ItemQuality.Legendary)
                                    {
                                        if (IgnoreWhenInAvoidance(cacheObject))
                                            break;

                                        if (IgnoreWhenNearElites(cacheObject, objects))
                                            break;
                                    }

                                    if (cacheObject.ItemQualityLevel == ItemQuality.Normal)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Adding {cacheObject.InternalName} - White Item";
                                        break;
                                    }

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject);

                                    break;
                                }

                            #endregion

                            #region Gold

                            case TrinityObjectType.Gold:

                                if (!Core.Settings.Items.PickupGold)
                                {
                                    cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Pick Up Gold Setting.";
                                    break;
                                }

                                if (IgnoreWhenBlocked(cacheObject))
                                    break;

                                if (IgnoreForTownPortal(cacheObject, usingTownPortal))
                                    break;

                                if (IgnoreWhenInAvoidance(cacheObject))
                                    break;

                                cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                      EliteMonsterNearFormula(cacheObject, elites) +
                                                      AoENearFormula(cacheObject) +
                                                      AoEInPathFormula(cacheObject);
                                break;

                            #endregion

                            #region Globes

                            case TrinityObjectType.PowerGlobe:
                            case TrinityObjectType.HealthGlobe:
                            case TrinityObjectType.ProgressionGlobe:

                                if (Core.Settings.Weighting.GlobeWeighting == SettingMode.Disabled)
                                {
                                    cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Ignore Globes Setting.";
                                    break;
                                }

                                if (Core.Settings.Weighting.GlobeWeighting == SettingMode.Selective)
                                {
                                    var globeType = GetGlobeType(cacheObject);
                                    if (!Core.Settings.Weighting.GlobeTypes.HasFlag(globeType))
                                    {
                                        cacheObject.WeightInfo += $"Ignore globe: {globeType} setting.";
                                        break;
                                    }
                                }

                                if (playerInCriticalAvoidance)
                                {
                                    cacheObject.WeightInfo += "Ignoring: Player in CriticalAvoidance";
                                    break;
                                }

                                if (cacheObject.Type == TrinityObjectType.ProgressionGlobe && cacheObject.Distance <= 180f)
                                {
                                    cacheObject.WeightInfo += $"Maxxing {cacheObject.InternalName} - Progression Globe.";
                                    cacheObject.Weight += MaxWeight;
                                    break;
                                }

                                if (cacheObject.Type == TrinityObjectType.HealthGlobe)
                                {
                                    if (isHealthEmergency)
                                    {
                                        cacheObject.WeightInfo += $"Health Emergency";
                                        cacheObject.Weight += (1d - Core.Player.CurrentHealthPct) * 10000d +
                                                              ObjectDistanceFormula(cacheObject) +
                                                              EliteMonsterNearFormula(cacheObject, elites);
                                        break;
                                    }

                                    if (isHealthEmergency)
                                    {
                                        cacheObject.WeightInfo += $"Health({Core.Player.CurrentHealthPct})";
                                        cacheObject.Weight += (1d - Core.Player.PrimaryResource) * 1000d +
                                                                ObjectDistanceFormula(cacheObject) +
                                                                EliteMonsterNearFormula(cacheObject, elites);

                                        break;
                                    }

                                    if (Core.Player.CurrentHealthPct > 0.85f)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring Globe - We have plenty of Health: {Core.Player.CurrentHealthPct}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (IgnoreWhenBlocked(cacheObject))
                                        break;

                                    if (IgnoreForTownPortal(cacheObject, usingTownPortal))
                                        break;
                                }

                                if (IgnoreWhenInAvoidance(cacheObject))
                                    break;

                                cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                      EliteMonsterNearFormula(cacheObject, elites) +
                                                      AoENearFormula(cacheObject) +
                                                      AoEInPathFormula(cacheObject);
                                break;

                            #endregion

                            #region Health Wells

                            case TrinityObjectType.HealthWell:

                                if (Core.Player.CurrentHealthPct > 0.7)
                                {
                                    cacheObject.WeightInfo += $"Ignoring - We have plenty of Health: {Core.Player.CurrentHealthPct}";
                                    break;
                                }

                                if (isHealthEmergency)
                                {
                                    cacheObject.WeightInfo += $"Health Emergency";
                                    cacheObject.Weight += (1d - Core.Player.PrimaryResource) * 5000d + EliteMonsterNearFormula(cacheObject, elites) - PackDensityFormula(cacheObject, objects);
                                    break;
                                }

                                cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                      EliteMonsterNearFormula(cacheObject, elites) +
                                                      AoENearFormula(cacheObject) +
                                                      AoEInPathFormula(cacheObject);
                                break;

                            //case TrinityObjectType.HealthGlobe:
                            //    {
                            //        if (!Core.Settings.Combat.Misc.CollectHealthGlobe)
                            //        {
                            //            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Collect Health Globe Setting.";
                            //            break;
                            //        }

                            //        if (cacheObject.Distance > Core.Settings.Combat.Misc.HealthGlobeSearchDistance)
                            //        {
                            //            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Health Globe Search Distance.";
                            //            break;
                            //        }

                            //        //Ignore because we are blocked by objects or mobs.
                            //        if (PlayerMover.IsCompletelyBlocked)
                            //        {
                            //            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                            //            break;
                            //        }

                            //        //Ignore because we are TownPortaling
                            //        if (usingTownPortal)
                            //        {
                            //            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Town Portal.";
                            //            break;
                            //        }

                            //        if (Core.Settings.Loot.Pickup.IgnoreHealthGlobesInAoE && Core.Avoidance.Grid.IsLocationInFlags(cacheObject.Position, AvoidanceFlags.Avoidance))
                            //        {
                            //            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - In Avoidance.";
                            //            break;
                            //        }

                            //        if (CombatBase.KiteMode != KiteMode.Never)
                            //        {
                            //            if (Core.Avoidance.Grid.IsIntersectedByFlags(cacheObject.Position, Core.Player.Position, AvoidanceFlags.Avoidance))
                            //            {
                            //                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Kiting with Monster Obstacles.";
                            //                break;
                            //            }
                            //            if (Core.Avoidance.Grid.IsLocationInFlags(cacheObject.Position, AvoidanceFlags.Avoidance))
                            //            {
                            //                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Kiting with Time Bound Avoidance.";
                            //                break;
                            //            }
                            //        }

                            //        if (Core.Player.CurrentHealthPct > CombatBase.EmergencyHealthGlobeLimit)
                            //        {
                            //            //XZ - Add gui Variable for Party MemberHealth
                            //            if (
                            //                objects.Any(
                            //                    p => p.Type == TrinityObjectType.Player && p.RActorId != Core.Player.RActorGuid))
                            //            {
                            //                var minPartyHealth =
                            //                    objects.Where(
                            //                        p =>
                            //                            p.Type == TrinityObjectType.Player &&
                            //                            p.RActorId != Core.Player.RActorGuid)
                            //                        .Min(p => p.HitPointsPct);
                            //                if (minPartyHealth <= 25)
                            //                {
                            //                    cacheObject.WeightInfo +=
                            //                        $"Adding {cacheObject.InternalName} - Party Health Below Threshold";
                            //                    cacheObject.Weight += hiPriorityHealthGlobes
                            //                        ? MaxWeight
                            //                        : (1d - minPartyHealth) * 5000d +
                            //                          EliteMonsterNearFormula(cacheObject, elites) -
                            //                          PackDensityFormula(cacheObject, objects);
                            //                    break;
                            //                }
                            //            }

                            //            var myHealth = Core.Player.CurrentHealthPct;
                            //            // DH's logic with Blood Vengeance passive
                            //            if (Core.Player.ActorClass == ActorClass.DemonHunter &&
                            //                Core.Player.PrimaryResource <= 10 &&
                            //                Core.Hotbar.PassiveSkills.Contains(
                            //                    SNOPower.DemonHunter_Passive_Vengeance))
                            //            {
                            //                cacheObject.WeightInfo +=
                            //                    $"Adding {cacheObject.InternalName} - Reapes Wraps.";
                            //                cacheObject.Weight += hiPriorityHealthGlobes
                            //                    ? MaxWeight
                            //                    : (1d - Core.Player.PrimaryResource) * 5000d +
                            //                      EliteMonsterNearFormula(cacheObject, elites) -
                            //                      PackDensityFormula(cacheObject, objects);
                            //                break;
                            //            }

                            //            // WD's logic with Gruesome Feast passive
                            //            if (Core.Player.ActorClass == ActorClass.Witchdoctor &&
                            //                Core.Player.PrimaryResource <= 1200 &&
                            //                Core.Hotbar.PassiveSkills.Contains(
                            //                    SNOPower.Witchdoctor_Passive_GruesomeFeast))
                            //            {
                            //                cacheObject.WeightInfo +=
                            //                    $"Adding {cacheObject.InternalName} - Gruesome Feast PickUp.";
                            //                cacheObject.Weight += hiPriorityHealthGlobes
                            //                    ? MaxWeight
                            //                    : (1d - Core.Player.PrimaryResource) * 5000d +
                            //                      EliteMonsterNearFormula(cacheObject, elites) -
                            //                      PackDensityFormula(cacheObject, objects);
                            //                break;
                            //            }

                            //            //Reapers Wraps Equipped
                            //            if (Legendary.ReapersWraps.IsEquipped && Core.Player.PrimaryResource <= 50)
                            //            {
                            //                cacheObject.WeightInfo +=
                            //                    $"Adding {cacheObject.InternalName} - Reapers Wraps.";
                            //                cacheObject.Weight += hiPriorityHealthGlobes
                            //                    ? MaxWeight
                            //                    : (1d - Core.Player.PrimaryResource) * 5000d +
                            //                      EliteMonsterNearFormula(cacheObject, elites) -
                            //                      PackDensityFormula(cacheObject, objects);
                            //                break;
                            //            }
                            //            if (ZetaDia.Me.HitpointsCurrentPct > TrinityPluginSettings.Settings.Combat.Misc.HealthGlobeLevel)
                            //            {
                            //                cacheObject.WeightInfo +=
                            //                    $"Ignoring {cacheObject.InternalName} - Over health globe Threshold.";
                            //                break;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            cacheObject.WeightInfo +=
                            //                $"Maxxing {cacheObject.InternalName} - Player.CurrentHealthPct < CombatBase.EmergencyHealthGlobeLimit.";
                            //            cacheObject.Weight = MaxWeight;
                            //            break;
                            //        }

                            //        cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                            //                              //LastTargetFormula(cacheObject) +
                            //                              EliteMonsterNearFormula(cacheObject, elites) +
                            //                              AoENearFormula(cacheObject) +
                            //                              AoEInPathFormula(cacheObject);
                            //        break;
                            //    }

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

                                    // Campaign A5 Quest "Lost Treasure of the Nephalem" - have to interact with nephalem switches first... 
                                    // Quest: x1_Adria, Id: 257120, Step: 108 - disable all looting, pickup, and objects
                                    if (Core.Player.WorldType != Act.OpenWorld && Core.Player.CurrentQuestSNO == 257120 && Core.Player.CurrentQuestStep == 108)
                                    {
                                        cacheObject.Weight = 0;
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Disable for Quest";
                                        break;
                                    }

                                    if (!cacheObject.IsQuestMonster)
                                    {
                                        if (Core.Settings.Weighting.ShrineWeighting == SettingMode.Disabled)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Dont use shrines setting.";
                                            break;
                                        }

                                        if (PlayerMover.IsCompletelyBlocked)
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                            break;
                                        }

                                        if (IgnoreForTownPortal(cacheObject, usingTownPortal))
                                            break;

                                        if (Core.Settings.Weighting.ShrineWeighting == SettingMode.Selective)
                                        {
                                            var type = GetShrineType(cacheObject);
                                            if (!Core.Settings.Weighting.ShrineTypes.HasFlag(type))
                                            {
                                                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Not a selected shrine type.";
                                                break;
                                            }
                                        }
                                    }

                                    if (cacheObject.Distance < 20f && cacheObject.IsWalkable && !PlayerMover.IsBlocked)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Shrine so close i can touch it {cacheObject.InternalName}";
                                        break;
                                    }

                                    //if (GetShrineType(cacheObject) != ShrineTypes.RunSpeed &&
                                    //    GetShrineType(cacheObject) != ShrineTypes.Speed &&
                                    //    GetShrineType(cacheObject) != ShrineTypes.Fortune)
                                    //{
                                    //    var priorityMultiplier = 1;
                                    //    if (Core.Settings.WorldObject.HiPriorityShrines)
                                    //    {
                                    //        cacheObject.WeightInfo +=
                                    //            $"Adding {cacheObject.InternalName} - High Priority Shrine";
                                    //        priorityMultiplier = 100;
                                    //    }
                                    //    if (Legendary.NemesisBracers.IsEquipped)
                                    //    {
                                    //        if (elites.Any())
                                    //        {
                                    //            var eliteMultiplier = -1000;
                                    //            if (GetShrineType(cacheObject) == ShrineTypes.Shield)
                                    //                eliteMultiplier = 1000;
                                    //            cacheObject.Weight -= elites.Count * eliteMultiplier;
                                    //        }
                                    //        else
                                    //            cacheObject.Weight += 1000d;
                                    //    }

                                    //    if (elites.Any())
                                    //    {
                                    //        cacheObject.Weight += elites.Count * 1000;
                                    //        cacheObject.WeightInfo +=
                                    //            $"Adding {cacheObject.InternalName} - Higher Priority Shrine for Elites";
                                    //    }
                                    //    cacheObject.Weight += PackDensityFormula(cacheObject, objects) * priorityMultiplier;
                                    //}

                                    cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                          //LastTargetFormula(cacheObject) +
                                                          //EliteMonsterNearFormula(cacheObject, elites) +
                                                          AoENearFormula(cacheObject) +
                                                          AoEInPathFormula(cacheObject) + 50;


                                    if (cacheObject.Distance < 80f && cacheObject.IsWalkable)
                                    {
                                        cacheObject.Weight *= 4;
                                        cacheObject.WeightInfo += $"Mid-Range Shrine Boost";
                                    }
                                    if (cacheObject.Distance < 125f)
                                    {
                                        cacheObject.Weight *= 2;
                                        cacheObject.WeightInfo += $"Far-Range Shrine Boost";
                                    }

                                    break;
                                }

                            #endregion

                            #region Door and Barricade

                            case TrinityObjectType.Gate:
                                cacheObject.Weight = 0;
                                cacheObject.WeightInfo += "Ignoring Gate";
                                break;

                            case TrinityObjectType.Barricade:
                            case TrinityObjectType.Door:
                                {
                                    if (cacheObject.IsUsed)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Used.";
                                        break;
                                    }

                                    if (cacheObject.IsLockedDoor)
                                    {
                                        cacheObject.WeightInfo += $"Locked Door";
                                        break;
                                    }

                                    if (!cacheObject.IsQuestMonster)
                                    {
                                        if (IgnoreWhenBlocked(cacheObject))
                                            break;

                                        if (IgnoreForTownPortal(cacheObject, usingTownPortal))
                                            break;
                                    }

                                    // There are some doors that are unlocked by switches but there is no way currently to identify which
                                    // doors these are (model is reused for normal doors too) except by the presence of switch actors
                                    // which are to be prioritized higher than the door they open.

                                    if (cacheObject.RadiusDistance <= 5f && !isPriorityInteractableNearby)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo += $"Maxxing {cacheObject.InternalName} - Door in Close Distance.";
                                    }

                                    cacheObject.Weight += 0.3 * ((ObjectDistanceFormula(cacheObject) +
                                                                EliteMonsterNearFormula(cacheObject, elites) +
                                                                //PackDensityFormula(cacheObject, objects) +
                                                                AoENearFormula(cacheObject) +
                                                                AoEInPathFormula(cacheObject)));
                                    break;
                                }

                            #endregion

                            #region Destructible

                            case TrinityObjectType.Destructible:

                                if (!cacheObject.IsValid || cacheObject.IsUsed)
                                {
                                    cacheObject.Weight = 0;
                                    cacheObject.WeightInfo += $"Destroyed or Invalid";
                                    break;
                                }

                                if (!cacheObject.IsQuestMonster)
                                {
                                    if (IgnoreWhenBlocked(cacheObject))
                                        break;

                                    if (IgnoreForTownPortal(cacheObject, usingTownPortal))
                                        break;
                                }

                                if (GameData.ForceDestructibles.Contains(cacheObject.ActorSnoId))
                                {
                                    cacheObject.Weight = 100d;
                                    break;
                                }

                                if (cacheObject.RadiusDistance > 0 && !Core.StuckHandler.IsStuck)
                                {
                                    cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Not Stuck.";
                                    break;
                                }

                                if (cacheObject.Distance < 3f)
                                {
                                    cacheObject.Weight = MaxWeight;
                                    cacheObject.WeightInfo += $"Maxxing {cacheObject.InternalName} - Close Distance.";
                                }

                                // Fix for WhimsyShire Pinata
                                if (GameData.ResplendentChestIds.Contains(cacheObject.ActorSnoId))
                                    cacheObject.Weight += 500d;

                                cacheObject.Weight += 0.5 * (ObjectDistanceFormula(cacheObject) +
                                                           EliteMonsterNearFormula(cacheObject, elites) -
                                                           PackDensityFormula(cacheObject, objects) +
                                                           AoENearFormula(cacheObject) +
                                                           AoEInPathFormula(cacheObject));
                                break;

                            #endregion

                            #region Interactables

                            case TrinityObjectType.Interactable:
                                {
                                    if (Combat.CombatMode == CombatMode.SafeZerg)
                                    {
                                        cacheObject.WeightInfo += $"Ignore(Zerg)";
                                        break;
                                    }

                                    if (cacheObject.IsUsed)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Used.";
                                        break;
                                    }

                                    if (cacheObject.GizmoType == GizmoType.LoreChest &&
                                        !Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.Lore))
                                    {
                                        cacheObject.WeightInfo += $"Ignoring(LoreSetting)";
                                        break;
                                    }

                                    if (cacheObject.IsInteractWhitelisted && cacheObject.RadiusDistance < 120f && !behindClosedDoor)
                                    {
                                        cacheObject.WeightInfo += $"WhiteListed Interactable Far";
                                        cacheObject.Weight = 2500;
                                        break;
                                    }

                                    if (cacheObject.IsInteractWhitelisted && cacheObject.RadiusDistance < 25f)
                                    {
                                        cacheObject.WeightInfo += $"WhiteListed Interactable";
                                        cacheObject.Weight = MaxWeight;
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
                                    if (Core.Player.WorldType != Act.OpenWorld && Core.Player.CurrentQuestSNO == 257120 &&
                                        Core.Player.CurrentQuestStep == 108)
                                    {
                                        cacheObject.Weight = MaxWeight;
                                        cacheObject.WeightInfo +=
                                            $"Adding {cacheObject.InternalName} - Campaign A5 Quest Lost Treasure of the Nephalem";
                                        break;
                                    }

                                    // nearby monsters attacking us - don't try to use headtone
                                    if (cacheObject.ActorType == ActorType.Gizmo && cacheObject.GizmoType == GizmoType.Headstone &&
                                        objects.Any(u => u.IsUnit && u.RadiusDistance < 25f && u.IsFacingPlayer))
                                    {
                                        cacheObject.WeightInfo +=
                                            $"Ignoring {cacheObject.InternalName} - Units Near Headstone. ";
                                        break;
                                    }

                                    if (GameData.HighPriorityInteractables.Contains(cacheObject.ActorSnoId) &&
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

                                if (cacheObject.IsInteractWhitelisted && !behindClosedDoor)
                                {
                                    if (cacheObject.Distance < 80f)
                                    {
                                        cacheObject.WeightInfo += $"WhiteListed Interactable Container Far";
                                        cacheObject.Weight = 5000;
                                        break;
                                    }

                                    if (cacheObject.Distance < 25f)
                                    {
                                        cacheObject.WeightInfo += $"WhiteListed Interactable Container";
                                        cacheObject.Weight = MaxWeight;
                                        break;
                                    }
                                }

                                if (!cacheObject.IsQuestMonster && !cacheObject.IsMinimapActive)
                                {
                                    if (Core.Settings.Weighting.ContainerWeighting == SettingMode.Disabled)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Ignore containers setting.";
                                        break;
                                    }

                                    if (PlayerMover.IsCompletelyBlocked)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Nav Blocked.";
                                        break;
                                    }

                                    if (usingTownPortal)
                                    {
                                        cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Town Portal.";
                                        break;
                                    }

                                    if (Core.Settings.Weighting.ContainerWeighting == SettingMode.Selective)
                                    {
                                        var type = GetContainerType(cacheObject);
                                        if (!Core.Settings.Weighting.ContainerTypes.HasFlag(type))
                                        {
                                            cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Not a selected container type.";
                                            break;
                                        }
                                    }
                                }

                                cacheObject.Weight += ObjectDistanceFormula(cacheObject) +
                                                      EliteMonsterNearFormula(cacheObject, elites) -
                                                      PackDensityFormula(cacheObject, objects) +
                                                      AoENearFormula(cacheObject) +
                                                      AoEInPathFormula(cacheObject);
                                break;

                                #endregion
                        }


                        // Anti-Flip flop. A new target needs to be 15% better than current.
                        if (Combat.Targeting.CurrentTarget != null && Combat.Targeting.CurrentTarget.AnnId == cacheObject.AnnId && bestTarget != null && !bestTarget.IsDestroyable)
                        {
                            cacheObject.Weight *= 1.15;
                            cacheObject.WeightInfo += " Last Target Boost ";
                        }

                        bestTarget = GetNewBestTarget(cacheObject, bestTarget);
                    }

                    #endregion Foreach loop
                }
                catch (Exception ex)
                {
                    Logger.Log($"Exception Inside Weighting Foreach Loop {ex}");
                }

                return SetTarget(bestTarget);
            }
        }

        public double HighestWeightFound { get; set; }

        private bool ShouldIgnoreElite(TrinityActor unit)
        {
            string reason;
            return ShouldIgnoreElite(unit, out reason);
        }

        private bool ShouldIgnoreElite(TrinityActor unit, out string reason)
        {
            reason = string.Empty;

            if (!unit.IsElite)
                return false;

            if (unit.IsBoss)
                return false;

            if (Core.Player.IsCastingPortal)
            {
                reason = "Ignore(CastingPortal)";
                return true;
            }

            if (unit.IsMinimapActive)
            {
                reason = "Keep(IsMinimapActive)";
                return false;
            }

            if (Core.Settings.Weighting.EliteWeighting == SettingMode.Enabled)
            {
                reason = "Keep(Elites=Enabled)";
                return false;
            }

            if (Core.Settings.Weighting.EliteWeighting == SettingMode.Disabled)
            {
                reason = "Ignore(Elites=Disabled)";
                return true;
            }

            if (Core.Settings.Weighting.EliteWeighting == SettingMode.Selective)
            {
                var eliteType = GetEliteType(unit);
                if (!Core.Settings.Weighting.EliteTypes.HasFlag(eliteType))
                {
                    reason = $"Ignore(EliteType:{eliteType}=Disabled)";
                    return true;
                }

                var ignoredAffixMatches = _ignoredAffixes.Where(a => unit.MonsterAffixes.HasFlag(a)).ToList();
                if (ignoredAffixMatches.Any())
                {
                    var ignoredTypes = ignoredAffixMatches.Aggregate(string.Empty, (s, affixes) => s + $",{affixes}");
                    reason = $"Ignore(Affix:{ignoredTypes}=Disabled)";
                    return true;
                }
            }
            return false;
        }



        //private bool ShouldIgnoreGlobe(TrinityActor actor, out string reason)
        //{
        //    reason = string.Empty;

        //    if (Core.Settings.Weighting.EliteWeighting == SettingMode.Enabled)
        //    {
        //        reason = "Keep(Elites=Enabled)";
        //        return false;
        //    }

        //    if (Core.Settings.Weighting.EliteWeighting == SettingMode.Disabled)
        //    {
        //        reason = "Ignore(Elites=Disabled)";
        //        return true;
        //    }

        //    if (Core.Settings.Weighting.EliteWeighting == SettingMode.Selective)
        //    {
        //        var eliteType = GetEliteType(actor);
        //        if (!Core.Settings.Weighting.EliteTypes.HasFlag(eliteType))
        //        {
        //            reason = $"Ignore(EliteType:{eliteType}=Disabled)";
        //            return true;
        //        }

        //        var ignoredAffixMatches = _ignoredAffixes.Where(a => actor.MonsterAffixes.HasFlag(a)).ToList();
        //        if (ignoredAffixMatches.Any())
        //        {
        //            var ignoredTypes = ignoredAffixMatches.Aggregate(string.Empty, (s, affixes) => s + $",{affixes}");
        //            reason = $"Ignore(Affix:{ignoredTypes}=Disabled)";
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public TrinityActor KamakaziGoblin { get; set; }

        public bool IsDoingGoblinKamakazi { get; set; }

        private static bool IgnoreWhenNearElites(TrinityActor cacheObject, List<TrinityActor> objects)
        {
            if (objects.Any(u => u.IsElite && u.Position.Distance(cacheObject.Position) <= 15f))
            {
                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Near Elite";
                return true;
            }
            return false;
        }

        private static bool IgnoreWhenBlocked(TrinityActor cacheObject)
        {
            if (PlayerMover.IsCompletelyBlocked)
            {
                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Blocked.";
                return true;
            }
            return false;
        }

        private static bool IgnoreForTownPortal(TrinityActor cacheObject, bool usingTownPortal)
        {
            if (usingTownPortal)
            {
                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} - Town Portal.";
                return true;
            }
            return false;
        }

        private static bool IgnoreWhenInAvoidance(TrinityActor cacheObject)
        {
            if (Core.Avoidance.InAvoidance(cacheObject.Position))
            {
                cacheObject.WeightInfo += $"Ignoring {cacheObject.InternalName} In Avoidance.";
                return true;
            }
            return false;
        }

        private TrinityActor SetTarget(TrinityActor bestTarget)
        {
            // Set Record History
            if (bestTarget?.InternalName != null && bestTarget.ActorSnoId > 0 && bestTarget.Weight > 0)
            {
                //if (bestTarget.RActorId != LastTargetRActorGuid || bestTarget != null && bestTarget.IsMarker)
                //{
                //    Logger.Log(LogCategory.Targetting,
                //        $"Target changed to {bestTarget.ActorSnoId} // {bestTarget.InternalName} RActorGuid={bestTarget.RActorId} " +
                //        $"({bestTarget.Type}) {bestTarget.WeightInfo} TargetInfo={bestTarget.Targeting}");
                //}
                return bestTarget;
            }
            TargetUtil.ClearCurrentTarget("No good target's found in Weighting.");
            return null;
        }

        private TrinityActor GetNewBestTarget(TrinityActor cacheObject, TrinityActor bestTarget)
        {
            cacheObject.WeightInfo += cacheObject.IsNpc ? " IsNPC" : "";
            cacheObject.WeightInfo += cacheObject.NpcIsOperable ? " IsOperable" : "";

            Logger.Log(TrinityLogLevel.Debug, LogCategory.Weight,
                "Weight={0:0} name={1} sno={2} type={3} R-Dist={4:0} IsElite={5} RAGuid={6} {7}",
                cacheObject.Weight, cacheObject.InternalName, cacheObject.ActorSnoId, cacheObject.Type,
                cacheObject.RadiusDistance, cacheObject.IsElite,
                cacheObject.RActorId, cacheObject.WeightInfo);
            cacheObject.WeightInfo = cacheObject.WeightInfo;

            if (bestTarget == null)
                bestTarget = cacheObject;

            var pickNewTarget = cacheObject.Weight > 0 && cacheObject.Weight > HighestWeightFound;

            if (!pickNewTarget) return bestTarget;
            bestTarget = cacheObject;
            HighestWeightFound = cacheObject.Weight;
            return bestTarget;
        }

        /// <summary>
        /// Contains an RActorGUID and count of the number of times we've switched to this target
        /// todo evaluate, temporary placement here
        /// </summary>
        internal Dictionary<string, TargetingInfo> TargetHistory { get; } = new Dictionary<string, TargetingInfo>();

        /// <summary>
        /// How many times the player tried to interact with this object in total
        /// todo evaluate, temporary placement here
        /// </summary>
        internal Dictionary<int, int> InteractAttempts { get; } = new Dictionary<int, int>();

        /// <summary>
        /// Update history of targetting for an actor and blacklist target if nessedsary
        /// </summary>
        /// <param name="bestTarget">actor to update</param>
        /// <returns>current number of times targetted</returns>
        public int RecordTargetHistory(TrinityActor bestTarget)
        {
            return 0;

            //TargetingInfo info;
            //if (bestTarget == null)
            //    return 0;

            //if (TargetHistory.Count > 200)
            //    TargetHistory.RemoveAll(o => DateTime.UtcNow.Subtract(o.CreatedTime).TotalSeconds > 30);

            //if (TargetHistory.TryGetValue(bestTarget.ObjectHash, out info))
            //{
            //    var isNewTarget = Combat.Targeting.CurrentTarget == null || bestTarget.ObjectHash != Combat.Targeting.CurrentTarget.ObjectHash && !LastTargetIsSafeSpot && LastTargetRActorGuid > 0;
            //    if (isNewTarget)
            //    {
            //        // Targeted times is used primarily for blacklisting, 
            //        // exclude avoidance triggered target switches or things will get blacklisted too fast while kiting.      

            //        info.TargetedTimes++;
            //        info.TimeLastTargetted = DateTime.UtcNow;
            //        bestTarget.HasBeenPrimaryTarget = true;
            //    }

            //    var isEliteLowHealth = bestTarget.HitPointsPct <= 0.75 && bestTarget.IsElite;
            //    if (isEliteLowHealth)
            //        return info.TargetedTimes;

            //    if (bestTarget.IsBoss || bestTarget.IsSafeSpot || bestTarget.IsWaitSpot)
            //        return info.TargetedTimes;

            //    if (isNewTarget)
            //    {
            //        info.FirstTargetedTime = DateTime.UtcNow;
            //    }

            //    var isUnusedDoor = bestTarget.Type == TrinityObjectType.Door && !bestTarget.IsUsed;
            //    var timeAsCurrentTarget = DateTime.UtcNow.Subtract(info.FirstTargetedTime).TotalSeconds;

            //    if (info.TargetedTimes > GetBlacklistTargetTimes(bestTarget) && !bestTarget.IsElite && !isUnusedDoor)
            //    {
            //        GenericBlacklist.Blacklist(bestTarget, TimeSpan.FromSeconds(60), $"Targetted too many times ({info.TargetedTimes})");
            //        info.TargetedTimes = 0;
            //        info.BlacklistedTimes++;
            //    }
            //    else if (timeAsCurrentTarget > 120 && !bestTarget.IsElite && !isUnusedDoor && info.TargetedTimes > 25)
            //    {
            //        GenericBlacklist.Blacklist(bestTarget, TimeSpan.FromSeconds(30), $"Target timeout ({info.TotalTimeAsCurrentTarget.TotalSeconds}s))");
            //        info.TotalTimeAsCurrentTarget = TimeSpan.Zero;
            //        info.BlacklistedTimes++;
            //    }

            //    return info.TargetedTimes;
            //}

            //TargetHistory.Add(bestTarget.ObjectHash, new TargetingInfo
            //{
            //    ObjectHash = bestTarget.ObjectHash,
            //    RActorGuid = bestTarget.RActorId,
            //    Name = bestTarget.InternalName,
            //    TargetedTimes = 1,
            //    FirstTargetedTime = DateTime.UtcNow,
            //});

            //return 1;
        }

        private int LastTargetRActorGuid => Combat.Targeting.LastTarget?.RActorId ?? -1;

        private bool LastTargetIsSafeSpot => Combat.Targeting.LastTarget?.IsSafeSpot ?? false;

        private static int GetBlacklistTargetTimes(TrinityActor currentTarget)
        {
            switch (currentTarget.Type)
            {
                case TrinityObjectType.Item:
                    return currentTarget.ItemQualityLevel >= ItemQuality.Legendary ? 400 : 75;
                case TrinityObjectType.Door:
                case TrinityObjectType.ProgressionGlobe:
                    return 300;
            }
            return 150;
        }

        #region Helper Methods

        public const double MaxWeight = 50000d;
        private const double MinWeight = -1d;
        private static bool _riftProgressionKillAll;

        private List<MonsterAffixes> _ignoredAffixes;


        /// <summary>
        ///     Gets the settings distances based on elite or not.
        /// </summary>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public float DistanceForObjectType(TrinityActor cacheObject)
        {
            return cacheObject.MonsterQuality == MonsterQuality.Boss ||
                   cacheObject.MonsterQuality == MonsterQuality.Unique ||
                   cacheObject.MonsterQuality == MonsterQuality.Rare ||
                   cacheObject.MonsterQuality == MonsterQuality.Champion ||
                   cacheObject.MonsterQuality == MonsterQuality.Minion
                ? Combat.Routines.Current.EliteRange
                : Combat.Routines.Current.TrashRange;
        }

        public double GoldFormula(TrinityItem cacheObject)
        {
            return cacheObject.GoldAmount * 0.05;
        }

        /// <summary>
        ///     Gets the base weight for types of Elites/Rares/Champions/Uniques/Bosses
        /// </summary>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public double EliteFormula(TrinityActor cacheObject)
        {
            if (cacheObject.MonsterQuality ==
                MonsterQuality.Boss)
                return 1500d;
            if (cacheObject.MonsterQuality ==
                MonsterQuality.Unique)
                return 400d;
            if (cacheObject.MonsterQuality ==
                MonsterQuality.Rare && cacheObject.MonsterQuality !=
                MonsterQuality.Minion)
                return 300d;
            if (cacheObject.MonsterQuality ==
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
        public double AoENearFormula(TrinityActor cacheObject)
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
        public double AoEInPathFormula(TrinityActor cacheObject)
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
        public double AffixMonsterNearFormula(TrinityActor cacheObject,
            List<TrinityActor> monstersWithAffix)
        {
            var monsters = monstersWithAffix.Where(u => u.Position.Distance(cacheObject.Position) < 30f);
            return monsters.Aggregate<TrinityActor, double>(0, (current, monster) => current + 5000 * Math.Max(0, (int)(30 - monster.Distance)));
        }

        /// <summary>
        ///     Gets the weight for Objects near Elites.
        /// </summary>
        /// <param name="cacheObject"></param>
        /// <param name="eliteMonsters"></param>
        /// <returns></returns>
        public double EliteMonsterNearFormula(TrinityActor cacheObject, List<TrinityActor> eliteMonsters)
        {
            double weight = 0;
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
        public double ObjectDistanceFormula(TrinityActor cacheObject)
        {
            var multipler = 500d;

            // DemonHunter is very fragile and should never run past close mobs, so increase distance weighting.
            if (cacheObject.IsUnit && Core.Player.ActorClass == ActorClass.DemonHunter)
                multipler = 1000d;

            // not units (items etc) shouldnt be impacted by the trash/non-trash slider setting.
            var range = 80f;

            // Overriding these settings required for questing profiles acts1-5 and bounties.
            var isQuesting = Combat.CombatMode == CombatMode.Questing;
            var questingEliteRange = 120f;
            var questingTrashRange = 100f;

            if (cacheObject.Type == TrinityObjectType.Unit)
            {
                var eliteRange = isQuesting ? questingEliteRange : Combat.Routines.Current.EliteRange;
                var nonEliteRange = isQuesting ? questingTrashRange : Combat.Routines.Current.TrashRange;
                ; //RoutineAdapter.Routine.TrashRange; //Core.Settings.Combat.Misc.NonEliteRange;

                range = cacheObject.IsElite
                    ? eliteRange
                    : nonEliteRange;

                if (cacheObject.IsMinimapActive)
                    range *= 1.5f;
            }

            return multipler * ((range - cacheObject.RadiusDistance) / range);
        }

        public double PackDensityFormula(TrinityActor cacheObject, List<TrinityActor> objects)
        {
            if (Combat.CombatMode == CombatMode.KillAll)
                return 0;

            var pack = objects.Where(
                x => x.IsUnit && x.IsHostile && x.Position.Distance(cacheObject.Position) < Combat.Routines.Current.TrashRange && (!ShouldIgnoreElite(cacheObject) || !x.IsElite))
                .ToList();

            var packDistanceValue = pack.Sum(mob => 100d * ((Combat.Routines.Current.TrashRange - cacheObject.RadiusDistance) / Combat.Routines.Current.TrashRange));

            return packDistanceValue < 0 ? 0 : packDistanceValue;
        }

        public double RiftValueFormula(TrinityActor cacheObject, List<TrinityActor> objects)
        {
            var result = 0d;

            //if (!RiftProgression.IsInRift || !cacheObject.IsUnit)
            //    return result;

            //// get all other units within cluster radius of this unit.
            //var pack = objects.Where(x =>
            //    x.Position.Distance(cacheObject.Position) < Combat.Routines.Current.TrashRange &&
            //    (!Core.Settings.Combat.Misc.IgnoreElites || !x.IsElite))
            //    .ToList();

            //cacheObject.RiftValueInRadius = pack.Sum(mob => mob.RiftValuePct);

            //// Only boost weight of this unit if above the total weight setting.
            //if (cacheObject.RiftValueInRadius > Core.Settings.Combat.Misc.RiftValueAlwaysKillClusterValue)
            //    result = 100d * ((Combat.Routines.Current.TrashRange - cacheObject.RadiusDistance) / Combat.Routines.Current.TrashRange);


            return result <= 0 ? 0 : result;
        }

        /// <summary>
        ///     Gets the weight based on the Objects Health Percent.  Lower health gets more priority
        /// </summary>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public double UnitHealthFormula(TrinityActor cacheObject)
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
        public double PathBlockedFormula(TrinityActor cacheObject)
        {
            if (cacheObject.ActorSnoId == 3349) // Belial, can't be pathed to.
                return 0;

            var isInRift = RiftProgression.IsInRift || RiftProgression.IsGreaterRift;

            if (cacheObject.IsUnit && !(isInRift && cacheObject.IsElite) && !cacheObject.IsQuestMonster)
            {
                if (!cacheObject.IsWalkable && cacheObject.IsInLineOfSight && cacheObject.Distance > 40f && !cacheObject.IsMinimapActive && !cacheObject.IsBoss && !cacheObject.IsTreasureGoblin)
                    return -MaxWeight;

                if (Core.Grids.Avoidance.IsIntersectedByFlags(Core.Player.Position, cacheObject.Position, AvoidanceFlags.ClosedDoor, AvoidanceFlags.ProjectileBlocking))
                    return -MaxWeight;
            }

            if (!PlayerMover.IsCompletelyBlocked)
                return 0;

            if (BlockingObjects(cacheObject) > 0)
                return -MaxWeight;

            // todo fix this its causing massive bouts of the bot doing nothing while standing in groups of mobs.             
            //if(!cacheObject.IsUnit)
            //    return BlockingMonsterObjects(cacheObject) * -100d;

            if (cacheObject.Distance > 15f && !cacheObject.IsElite)
                return -MaxWeight;

            return 0;
        }

        //public static int BlockingMonsterObjects(TrinityActor cacheObject)
        //{
        //    var monsterCount = CacheData.MonsterObstacles.Count(
        //        ob => MathUtil.IntersectsPath(ob.Position, ob.Radius, Core.Player.Position,
        //            cacheObject.Position));

        //    return monsterCount;
        //}

        /// <summary>
        ///     Navigation obstacles are more critical than monster obstacles, these include script locked gates, large barricades
        ///     etc
        ///     They cannot be walked passed, and everything beyond them needs to be ignored.
        /// </summary>
        public int BlockingObjects(TrinityActor cacheObject)
        {
            var navigationCount = 0;

            //var navigationCount = CacheData.NavigationObstacles.Count(
            //    ob => MathUtil.IntersectsPath(ob.Position, ob.Radius, Core.Player.Position,
            //        cacheObject.Position));

            //var gate = CacheData.NavigationObstacles.FirstOrDefault(o => o.ActorSnoId == 108466);
            //if (gate != null)
            //    Logger.Log("NavigationObstacles contains gate {0} blockingCount: {1}={2}", gate.Name, cacheObject.InternalName, navigationCount);

            return navigationCount;
        }

        //public static double LastTargetFormula(TrinityActor cacheObject)
        //{
        //    if (PlayerMover.IsCompletelyBlocked)
        //        return 0;

        //    return cacheObject.RActorId == LastTargetRactorGUID ? 250d : 0d;
        //}

        #endregion

        public ShrineTypes GetShrineType(TrinityActor cacheObject)
        {
            switch (cacheObject.ActorSnoId)
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
                    return ShrineTypes.None;
            }
        }

        public ContainerTypes GetContainerType(TrinityActor cacheObject)
        {
            if (cacheObject.IsRareChest)
                return ContainerTypes.RareChest;

            if (cacheObject.IsChest)
                return ContainerTypes.NormalChest;

            if (cacheObject.IsWeaponRack)
                return ContainerTypes.WeaponRack;

            if (cacheObject.IsGroundClicky)
                return ContainerTypes.GroundClicky;

            if (cacheObject.IsCorpse)
                return ContainerTypes.Corpse;

            if (cacheObject.IsContainer)
                return ContainerTypes.Other;

            return ContainerTypes.None;
        }

        public GlobeTypes GetGlobeType(TrinityActor cacheObject)
        {
            switch (cacheObject.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                    if (GameData.GreaterProgressionGlobeSNO.Contains(cacheObject.ActorSnoId))
                        return GlobeTypes.GreaterRift;
                    return GlobeTypes.NephalemRift;

                case TrinityObjectType.PowerGlobe:
                    return GlobeTypes.Power;

                case TrinityObjectType.HealthGlobe:
                    return GlobeTypes.Health;
            }
            return GlobeTypes.None;
        }

        public EliteTypes GetEliteType(TrinityActor cacheObject)
        {
            switch (cacheObject.MonsterQuality)
            {
                case MonsterQuality.Champion:
                    return EliteTypes.Champion;

                case MonsterQuality.Minion:
                    return EliteTypes.Minion;

                case MonsterQuality.Rare:
                    return EliteTypes.Rare;
            }
            return EliteTypes.None;
        }
    }
}