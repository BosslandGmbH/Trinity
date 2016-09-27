//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using Trinity.Cache;
//using Trinity.Components.Adventurer.Coroutines.RiftCoroutines;
//using Trinity.Components.Combat.Abilities.PhelonsPlayground;
//using Trinity.DbProvider;
//using Trinity.Framework;
//using Trinity.Framework.Modules;
//using Trinity.Movement;
//using Trinity.Reference;
//using Trinity.Technicals;
//using Zeta.Common;
//using Zeta.Common.Helpers;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.Game.Internals.Actors;
//using Logger = Trinity.Technicals.Logger;

//namespace Trinity.Components.Combat.Abilities
//{
//    public class WitchDoctorCombat : CombatBase
//    {
//        private static WaitTimer _bastianGeneratorWaitTimer = new WaitTimer(TimeSpan.FromSeconds(3));

//        public static bool TryGetArachnyrFirebatsPower(out TrinityPower power)
//        {
//            power = null;

//            var allUnits = Core.Targets.ByType[TrinityObjectType.Unit].Where(u => u.IsUnit && u.RadiusDistance <= 50f).ToList();

//            var clusterUnits =
//                (from u in allUnits
//                    where u.IsUnit && u.Weight > 0 && !u.IsPlayer
//                    orderby
//                        u.NearbyUnitsWithinDistance(15f) descending,
//                        u.Distance,
//                        u.HitPointsPct descending
//                    select u).ToList();

//            if (!GetHasBuff(SNOPower.Witchdoctor_FetishArmy) && Skills.WitchDoctor.FetishArmy.CanCast())
//            {
//                Logger.Warn($"Casting some Fetishes...");
//                power = new TrinityPower(SNOPower.Witchdoctor_FetishArmy);
//                return true;
//            }

//            // Turn avoidance off when channelling
//            IsAvoidanceDisabled = () => Player.IsChannelling && Player.CurrentHealthPct > 0.4 && !Core.Avoidance.InCriticalAvoidance(Player.Position);

//            // Dont get distracted by shiny objects
//            IsFocussingUnits = () => Player.IsChannelling && Player.CurrentHealthPct > 0.4;

//            // Keep killing stuff until everything is dead.
//            IsIgnoringPackSize = () => Player.IsChannelling;

//            var bestClusterUnit = clusterUnits.FirstOrDefault();

//            //Keep Hex up as much as possible for Damage Debuff
//            if (!GetHasBuff(SNOPower.Witchdoctor_Hex) && Skills.WitchDoctor.Hex.CanCast())
//            {
//                Logger.Warn($"Casting Hex for upkeep");
//                power = new TrinityPower(SNOPower.Witchdoctor_Hex);
//                return true;
//            }

//            if (bestClusterUnit != null)
//            {
//                if (!GetHasBuff(SNOPower.Witchdoctor_Hex) && Skills.WitchDoctor.Hex.CanCast())
//                {
//                    Logger.Warn($"Casting Hex");
//                    power = new TrinityPower(SNOPower.Witchdoctor_Hex);
//                    return true;
//                }

//                var percentTargetsWithHaunt = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Haunt, 8f);
//                var percentTargetsWithLocust = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Locust_Swarm, 12f);
//                var isEliteWithoutHaunt = clusterUnits.Any(u => u.IsElite && !u.HasDebuff(SNOPower.Witchdoctor_Haunt));
//                var isElitewithoutLocust = clusterUnits.Any(u => u.IsElite && !u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm));
//                var harvestStacks = Skills.WitchDoctor.SoulHarvest.BuffStacks;
//                var harvestBuffCooldown = Core.Cooldowns.GetBuffCooldown(SNOPower.Witchdoctor_SoulHarvest);
//                var harvestPossibleStackGain = 5 - harvestStacks;
//                var harvestUnitsInRange = allUnits.Count(u => u.Distance < 18f);
//                var interruptForHarvest = CanCast(SNOPower.Witchdoctor_SoulHarvest) && harvestPossibleStackGain <= harvestUnitsInRange && harvestBuffCooldown?.Remaining.TotalSeconds < 5;
//                var interruptForHaunt = percentTargetsWithHaunt < 0.2f || isEliteWithoutHaunt;
//                var interruptForLocust = percentTargetsWithLocust < 0.2f || isElitewithoutLocust && Player.PrimaryResource > 300 && Skills.WitchDoctor.LocustSwarm.CanCast();
//                var interruptForHealth = Player.CurrentHealthPct < 0.4;

//                // continue channelling firebats?
//                if (Player.IsChannelling)
//                {
//                    if (!interruptForHealth && !interruptForHaunt && !interruptForLocust && !interruptForHarvest)
//                    {
//                        Logger.Warn("Continuation of Firebats.");
//                        power = new TrinityPower(SNOPower.Witchdoctor_Firebats, 30f, Player.Position, 0, 0);
//                        return true;
//                    }

//                    if (interruptForHaunt)
//                        Logger.Warn("Interrupted Firebats to haunt");

//                    if (interruptForLocust)
//                        Logger.Warn("Interrupted Firebats to locust");

//                    if (interruptForHarvest)
//                        Logger.Warn("Interrupted Firebats to harvest");
//                }

//                // Emergency health situation
//                if (Player.CurrentHealthPct < 0.4)
//                {
//                    if (Skills.WitchDoctor.SpiritWalk.CanCast())
//                    {
//                        Logger.Warn($"Defensive Spirit Walking due to low health");
//                        Skills.WitchDoctor.SpiritWalk.Cast();
//                    }

//                    if (Player.CurrentHealthPct < 0.4 && TargetUtil.AnyMobsInRange(18f) && Skills.WitchDoctor.SoulHarvest.CanCast())
//                    {
//                        Logger.Warn("Emergency Harvest because of low health");
//                        power = new TrinityPower(SNOPower.Witchdoctor_SoulHarvest, 0, 0);
//                        return true;
//                    }

//                    if (!GetHasBuff(SNOPower.Witchdoctor_Hex) && Skills.WitchDoctor.Hex.CanCast())
//                    {
//                        Logger.Warn($"Casting Hex due to low health");
//                        power = new TrinityPower(SNOPower.Witchdoctor_Hex);
//                        return true;
//                    }
//                }

//                var targetsWithoutLocust = clusterUnits.Where(u => !u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm)).OrderBy(u => u.Distance);
//                var isAnyTargetWithLocust = clusterUnits.Any(u => u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm));

//                // Locust
//                if (Skills.WitchDoctor.LocustSwarm.CanCast() && Skills.WitchDoctor.LocustSwarm.TimeSinceUse > 1000 && targetsWithoutLocust.Any() && (!Runes.WitchDoctor.Pestilence.IsActive || !isAnyTargetWithLocust))
//                {
//                    if ((percentTargetsWithLocust < 0.5f || isElitewithoutLocust) && Player.PrimaryResource > 300 && targetsWithoutLocust.Any())
//                    {
//                        Logger.Warn("Locust");
//                        power = new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 10f, targetsWithoutLocust.First().Position, 0, 0);
//                        return true;
//                    }
//                }

//                // Soul harvest for the damage reduction of Okumbas Ornament
//                if (CanCast(SNOPower.Witchdoctor_SoulHarvest) && (bestClusterUnit.Distance < 12f || harvestStacks < 4 && TargetUtil.AnyMobsInRange(10f)) && harvestStacks < 10)
//                {
//                    Logger.Warn($"Harvest State: StackGainPossible={harvestPossibleStackGain} Units={harvestUnitsInRange} BuffRemainingSecs:{harvestBuffCooldown?.Remaining.TotalSeconds:N2}");

//                    if (harvestPossibleStackGain <= harvestUnitsInRange)
//                    {
//                        Logger.Warn($"Soul Harvest.");
//                        power = new TrinityPower(SNOPower.Witchdoctor_SoulHarvest, 0, 0);
//                        return true;
//                    }
//                }

//                if (TryBigBadVoodoo(out power))
//                    return true;

//                // Piranhas
//                if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//                    (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) && Player.PrimaryResource >= 250)
//                {
//                    power = new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Core.Clusters.BestCluster.Position, 0, 0);
//                    return true;
//                }

//                // .80 of mobs give or take. Spelltracker check is to prevent repeat casts ont he same target before the projectile arrives.                 
//                var targetsWithoutHaunt = clusterUnits.Where(u => !u.HasDebuff(SNOPower.Witchdoctor_Haunt) && !SpellTracker.IsUnitTracked(u, SNOPower.Witchdoctor_Haunt)).OrderBy(u => u.Distance);
//                if ((percentTargetsWithHaunt < 0.45f || isEliteWithoutHaunt) && targetsWithoutHaunt.Any() && Player.PrimaryResource > 50)
//                {
//                    var target = targetsWithoutHaunt.First();
//                    Logger.Warn($"Haunt on {target}");
//                    power = new TrinityPower(SNOPower.Witchdoctor_Haunt, 50f, target.AcdId, 0, 0);
//                    return true;
//                }

//                Vector3 bestBuffedPosition;
//                TargetUtil.BestBuffPosition(16f, bestClusterUnit.Position, true, out bestBuffedPosition);
//                var bestClusterUnitRadiusPosition = MathEx.GetPointAt(bestClusterUnit.Position, bestClusterUnit.CollisionRadius*1.1f, bestClusterUnit.Rotation);
//                var bestFirebatsPosition = bestBuffedPosition != Vector3.Zero ? bestBuffedPosition : bestClusterUnitRadiusPosition;
//                var distance = bestFirebatsPosition.Distance(Player.Position);

//                // Walk into cluster or buffed location.
//                if (distance > 3f && !PlayerMover.IsBlocked)
//                {
//                    if (distance > 20f && Skills.WitchDoctor.SpiritWalk.CanCast())
//                    {
//                        Logger.Warn($"Spirit Walking");
//                        Skills.WitchDoctor.SpiritWalk.Cast();
//                    }

//                    Logger.Warn($"Walking to cluster position. Dist: {bestFirebatsPosition.Distance(Player.Position)}");
//                    power = new TrinityPower(SNOPower.Walk, 3f, bestFirebatsPosition, 0, 0);
//                    return true;
//                }

//                if (Skills.WitchDoctor.Firebats.CanCast())
//                {
//                    var closestUnit = allUnits.OrderBy(u => u.Distance).FirstOrDefault();
//                    if (closestUnit != null)
//                    {
//                        Logger.Warn($"Casting Firebats");
//                        power = new TrinityPower(SNOPower.Witchdoctor_Firebats, 15f, closestUnit.AcdId, 0, 0);
//                        return true;
//                    }
//                }
//            }

//            return true;
//        }

//        //----------------------------END ARACHYR FireBats Build----------------------------

//        static WitchDoctorCombat()
//        {
//            _bastianGeneratorWaitTimer.Reset();
//        }

//        private static readonly HashSet<SNOPower> HarvesterDebuffs = new HashSet<SNOPower>
//        {
//            SNOPower.Witchdoctor_Haunt,
//            SNOPower.Witchdoctor_Locust_Swarm,
//            SNOPower.Witchdoctor_Piranhas,
//            SNOPower.Witchdoctor_AcidCloud
//        };

//        private static readonly HashSet<SNOPower> HarvesterCoreDebuffs = new HashSet<SNOPower>
//        {
//            SNOPower.Witchdoctor_Haunt,
//            SNOPower.Witchdoctor_Locust_Swarm,
//        };

//        public static Stopwatch VisionQuestRefreshTimer = new Stopwatch();
//        public static long GetTimeSinceLastVisionQuestRefresh()
//        {
//            if (!VisionQuestRefreshTimer.IsRunning)
//                VisionQuestRefreshTimer.Start();

//            return VisionQuestRefreshTimer.ElapsedMilliseconds;
//        }

//        public static bool IsHellToothPetDoc
//        {
//            get
//            {
//                return Sets.HelltoothHarness.IsFullyEquipped && Legendary.TheShortMansFinger.IsEquipped &&
//                  !(Runes.WitchDoctor.AngryChicken.IsActive || Core.Hotbar.ActivePowers.Contains(SNOPower.Witchdoctor_Hex_ChickenWalk));
//            }
//        }

//        public static bool IsHellToothFirebats
//        {
//            get
//            {
//                return Sets.HelltoothHarness.IsFullyEquipped && Legendary.StaffOfChiroptera.IsEquipped
//                       && Skills.WitchDoctor.Firebats.IsActive;
//            }
//        }

//        public static TrinityPower GetPetDocPower()
//        {
//            TrinityPower power = null;

//            // Destructible objects
//            if (UseDestructiblePower)
//            {
//                return DestroyObjectPower;
//            }

//            if (UseOOCBuff)
//            {
//                var dogCount = Passives.WitchDoctor.MidnightFeast.IsActive ? 4 : 3;
//                if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) && Player.Summons.ZombieDogCount < dogCount)
//                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
//            }

//            if (IsCurrentlyAvoiding)
//            {
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);

//                Logger.Log(LogCategory.Routine, "Avoiding, returning no power");
//                return null;
//            }

//            //if (CurrentTarget.IsSafeSpot)
//            //{
//            //    return null;
//            //}

//            if (CurrentTarget != null)
//            {
//                // Zombie Dogs
//                var dogCount = Passives.WitchDoctor.MidnightFeast.IsActive ? 4 : 3;
//                if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) && (Core.Player.Summons.ZombieDogCount < dogCount ||
//                    TargetUtil.AnyElitesInRange(30f)))
//                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);

//                // Sacrafice Spam
//                var hasDogs = Player.Summons.ZombieDogCount > 0;
//                var canCastSacrifice = Skills.WitchDoctor.Sacrifice.IsActive && Skills.WitchDoctor.Sacrifice.CanCast();
//                var shouldCastSacrifice = Settings.Combat.WitchDoctor.ZeroDogs || RiftProgression.IsNephalemRift || RiftProgression.IsGreaterRift && (TargetUtil.AnyElitesInRange(30f) || TargetUtil.ClusterExists(20f, 4));
//                if (hasDogs && canCastSacrifice && shouldCastSacrifice)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice);                  
//                }

//                // Spirit Walk when rooted
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && (Player.IsIncapacitated || Player.IsRooted))
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);

//                // Piranhas
//                if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//                    (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) &&
//                    LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
//                    Player.PrimaryResource >= 250)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, TargetUtil.GetBestClusterPoint(14f));
//                }

//                // Soul Harvest
//                if (CanCast(SNOPower.Witchdoctor_SoulHarvest) && !IsCurrentlyAvoiding)
//                {
//                    if (Player.CurrentHealthPct < 0.6 && TargetUtil.AnyMobsInRange(12f))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                    }
//                    if (TargetUtil.ClusterExists(3, 12f) && Skills.WitchDoctor.SoulHarvest.BuffStacks < 10)
//                    {
//                        Logger.Log(LogCategory.Routine, "Im going in to harvest! 4/12");
//                        MoveToSoulHarvestPoint(Core.Clusters.BestCluster);
//                    }
//                    else if(TargetUtil.AnyElitesInRange(12f) || TargetUtil.AnyMobsInRange(10f, 2))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                    }
//                }

//                // Only use wall of death for the helltooth set buff.
//                if (!GetHasBuff(JeramsRevenge) && TryWallOfDeath(out power))
//                    return power;

//                // Haunt
//                if (CanCast(SNOPower.Witchdoctor_Haunt) && Player.PrimaryResource >= 50)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Haunt, 21f, CurrentTarget.AcdId);
//                }

//                // Acid Cloud
//                if (CanCast(SNOPower.Witchdoctor_AcidCloud) && Player.PrimaryResource >= 175)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, 25f, TargetUtil.GetBestClusterPoint(15f, 30f));
//                }

//                // Spirit walk to a safe spot to cast stuff
//                var safeWalkPoint = Core.Avoidance.Avoider.SafeSpot; //GetSafeSpotPosition(45f);
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
//                {
//                    Logger.Log(LogCategory.Routine, "Tryna spiritwalk to to safe point!");
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk, 45f, safeWalkPoint);
//                }

//                // Big Bad Voodoo
//                if (CanCast(SNOPower.Witchdoctor_BigBadVoodoo) &&
//                    (Settings.Combat.WitchDoctor.UseBigBadVoodooOffCooldown || TargetUtil.AnyMobsInRange(30f)) &&
//                    !GetHasBuff(SNOPower.Witchdoctor_BigBadVoodoo))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo, 30f, Core.Clusters.BestCluster.Position);
//                }

//                // Grasp of the Dead
//                if (CanCast(SNOPower.Witchdoctor_GraspOfTheDead) &&
//                    (TargetUtil.AnyMobsInRange(30, 2) || TargetUtil.EliteOrTrashInRange(30f)) &&
//                    Player.PrimaryResource >= 150)
//                {
//                    var bestClusterPoint = TargetUtil.GetBestClusterPoint();
//                    return new TrinityPower(SNOPower.Witchdoctor_GraspOfTheDead, 25f, bestClusterPoint);
//                }

//                if (CanCast(SNOPower.Witchdoctor_Horrify) && Settings.Combat.WitchDoctor.SpamHorrify)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
//                }

//                if (CurrentTarget.IsUnit)
//                {
//                    // Move to a safespot near the target.
//                    Vector3 safePositionNearTarget;
//                    var maxDistance = Math.Max(60f, CurrentTarget.CollisionRadius + KiteDistance);
//                    if (Core.Avoidance.Avoider.TryGetSafeSpot(out safePositionNearTarget, 10f, maxDistance, CurrentTarget.Position))
//                    {
//                        Logger.Log(LogCategory.Routine, $"Moving to safe point near {CurrentTarget.InternalName}");
//                        power = new TrinityPower(SNOPower.Walk, 45f, safePositionNearTarget);
//                    }
//                    else
//                    {
//                        // Move to any safespot
//                        if (TargetUtil.AnyMobsInRange(KiteDistance))
//                        {
//                            Logger.Log(LogCategory.Routine, "Tryna get to safe point!");
//                            return new TrinityPower(SNOPower.Walk, 45f, CurrentTarget.Position);
//                        }
//                    }
//                }
//            }

//            return power;
//        }

//        public const SNOPower JeramsRevenge = SNOPower.P3_ItemPassive_Unique_Ring_010;

//        public static bool TryGetHellToothFirebatsPower(out TrinityPower power)
//        { 
//            power = null;

//            var allUnits = Core.Targets.ByType[TrinityObjectType.Unit].Where(u => u.IsUnit && u.RadiusDistance <= 50f).ToList();

//            var clusterUnits =
//                (from u in allUnits
//                 where u.IsUnit && u.Weight > 0 && !u.IsPlayer
//                 orderby                 
//                 u.NearbyUnitsWithinDistance(15f) descending,
//                 u.Distance,
//                 u.HitPointsPct descending
//                 select u).ToList();

//            // Turn avoidance off when channelling
//            IsAvoidanceDisabled = () => Player.IsChannelling && Player.CurrentHealthPct > 0.4 && !Core.Avoidance.InCriticalAvoidance(Player.Position);
//            IsKitingDisabled = () => Player.IsChannelling && Player.CurrentHealthPct > 0.4 && !Core.Avoidance.InCriticalAvoidance(Player.Position);

//            // Dont get distracted by shiny objects
//            IsFocussingUnits = () => Player.IsChannelling && Player.CurrentHealthPct > 0.4;

//            // Keep killing stuff until everything is dead.
//            IsIgnoringPackSize = () => Player.IsChannelling;

//            var bestClusterUnit = clusterUnits.FirstOrDefault();

//            //10 second 60% damage reduction should always be on to survive
//            if (!GetHasBuff(JeramsRevenge) && (ZetaDia.Me.IsInCombat || Player.CurrentHealthPct < 0.4) && bestClusterUnit != null) 
//            {
//                Logger.Warn($"Casting Wall of Death on {allUnits.FirstOrDefault()}");
//                Skills.WitchDoctor.WallOfDeath.Cast(allUnits.FirstOrDefault());
//            }

//            if (bestClusterUnit != null)
//            {
//                if (!GetHasBuff(JeramsRevenge) && ZetaDia.Me.IsInCombat && Skills.WitchDoctor.WallOfDeath.CanCast())
//                {
//                    Logger.Warn($"Casting Wall of Death on {allUnits.FirstOrDefault()}");
//                    Skills.WitchDoctor.WallOfDeath.Cast(allUnits.FirstOrDefault());
//                }

//                if (!GetHasBuff(SNOPower.Witchdoctor_Hex) && Skills.WitchDoctor.Hex.CanCast())
//                {
//                    Logger.Warn($"Casting Hex");
//                    power = new TrinityPower(SNOPower.Witchdoctor_Hex);
//                    return true;
//                }

//                var percentTargetsWithHaunt = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Haunt, 8f);
//                var percentTargetsWithLocust = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Locust_Swarm, 12f);
//                var isEliteWithoutHaunt = clusterUnits.Any(u => u.IsElite && !u.HasDebuff(SNOPower.Witchdoctor_Haunt));
//                var isElitewithoutLocust = clusterUnits.Any(u => u.IsElite && !u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm));
//                var harvestStacks = Skills.WitchDoctor.SoulHarvest.BuffStacks;
//                var harvestBuffCooldown = Core.Cooldowns.GetBuffCooldown(SNOPower.Witchdoctor_SoulHarvest);
//                var harvestPossibleStackGain = 10 - harvestStacks;
//                var harvestUnitsInRange = allUnits.Count(u => u.Distance < 12f);

//                var interruptForHarvest = CanCast(SNOPower.Witchdoctor_SoulHarvest) && harvestPossibleStackGain <= harvestUnitsInRange && harvestBuffCooldown?.Remaining.TotalSeconds < 5;
//                var interruptForHaunt = percentTargetsWithHaunt < 0.2f || isEliteWithoutHaunt;
//                var interruptForLocust = percentTargetsWithLocust < 0.2f || isElitewithoutLocust && Player.PrimaryResource > 300 && Skills.WitchDoctor.LocustSwarm.CanCast();
//                var interruptForHealth = Player.CurrentHealthPct < 0.4;

//                // continue channelling firebats?
//                if (Player.IsChannelling)
//                {
//                    if (!interruptForHealth && !interruptForHaunt && !interruptForLocust && !interruptForHarvest)
//                    {
//                        Logger.Warn("Continuation of Firebats.");
//                        power = new TrinityPower(SNOPower.Witchdoctor_Firebats, 30f, Player.Position, 0, 0);
//                        return true;
//                    }

//                    if (interruptForHaunt)
//                        Logger.Warn("Interrupted Firebats to haunt");

//                    if (interruptForLocust)
//                        Logger.Warn("Interrupted Firebats to locust");

//                    if (interruptForHarvest)
//                        Logger.Warn("Interrupted Firebats to harvest");
//                }

//                // Emergency health situation
//                if (Player.CurrentHealthPct < 0.4)
//                {
//                    if (Skills.WitchDoctor.SpiritWalk.CanCast())
//                    {
//                        Logger.Warn($"Defensive Spirit Walking");
//                        Skills.WitchDoctor.SpiritWalk.Cast();
//                    }

//                    if (Player.CurrentHealthPct < 0.4 && TargetUtil.AnyMobsInRange(12f) && Skills.WitchDoctor.SoulHarvest.CanCast())
//                    {
//                        Logger.Warn("Emergency Harvest");
//                        power = new TrinityPower(SNOPower.Witchdoctor_SoulHarvest, 0, 0);
//                        return true;
//                    }

//                    if (!GetHasBuff(JeramsRevenge) && Skills.WitchDoctor.WallOfDeath.CanCast() && allUnits.Any())
//                    {
//                        Logger.Warn($"Casting Defensive WallOfDeath on {allUnits.FirstOrDefault()}");
//                        Skills.WitchDoctor.WallOfDeath.Cast(allUnits.FirstOrDefault());
//                    }
//                }

//                var targetsWithoutLocust = clusterUnits.Where(u => !u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm)).OrderBy(u => u.Distance);
//                var isAnyTargetWithLocust = clusterUnits.Any(u => u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm));

//                // Locust
//                if (Skills.WitchDoctor.LocustSwarm.CanCast() && Skills.WitchDoctor.LocustSwarm.TimeSinceUse > 1000 && targetsWithoutLocust.Any() && (!Runes.WitchDoctor.Pestilence.IsActive || !isAnyTargetWithLocust))
//                {                    
//                    if ((percentTargetsWithLocust < 0.5f || isElitewithoutLocust) && Player.PrimaryResource > 300 && targetsWithoutLocust.Any()) {
//                        Logger.Warn("Locust");
//                        power = new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 10f, targetsWithoutLocust.First().Position, 0, 0);
//                        return true;
//                    }
//                }                 
        
//                // Soul harvest for the damage reduction of Okumbas Ornament
//                if (CanCast(SNOPower.Witchdoctor_SoulHarvest) && (bestClusterUnit.Distance < 12f || harvestStacks < 4 && TargetUtil.AnyMobsInRange(10f))  && harvestStacks < 10)
//                {
//                    Logger.Warn($"Harvest State: StackGainPossible={harvestPossibleStackGain} Units={harvestUnitsInRange} BuffRemainingSecs:{harvestBuffCooldown?.Remaining.TotalSeconds:N2}");    
                     
//                    if (harvestPossibleStackGain <= harvestUnitsInRange)
//                    {
//                        Logger.Warn($"Soul Harvest.");
//                        power = new TrinityPower(SNOPower.Witchdoctor_SoulHarvest, 0, 0);
//                        return true;
//                    }               
//                }

//                if (TryBigBadVoodoo(out power))
//                    return true;

//                // Piranhas
//                if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//                    (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) && Player.PrimaryResource >= 250)
//                {
//                    power = new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Core.Clusters.BestCluster.Position, 0, 0);
//                    return true;
//                }

//                // .80 of mobs give or take. Spelltracker check is to prevent repeat casts ont he same target before the projectile arrives.                 
//                var targetsWithoutHaunt = clusterUnits.Where(u => !u.HasDebuff(SNOPower.Witchdoctor_Haunt) && !SpellTracker.IsUnitTracked(u,SNOPower.Witchdoctor_Haunt)).OrderBy(u => u.Distance);
//                if ((percentTargetsWithHaunt < 0.45f || isEliteWithoutHaunt) && targetsWithoutHaunt.Any() && Player.PrimaryResource > 50)
//                {
//                    var target = targetsWithoutHaunt.First();
//                    Logger.Warn($"Haunt on {target}");
//                    power = new TrinityPower(SNOPower.Witchdoctor_Haunt, 50f, target.AcdId, 0, 0);
//                    return true;
//                }

//                Vector3 bestBuffedPosition;
//                TargetUtil.BestBuffPosition(16f, bestClusterUnit.Position, true, out bestBuffedPosition);
//                var bestClusterUnitRadiusPosition = MathEx.GetPointAt(bestClusterUnit.Position, bestClusterUnit.CollisionRadius * 1.1f, bestClusterUnit.Rotation);
//                var bestFirebatsPosition = bestBuffedPosition != Vector3.Zero ? bestBuffedPosition : bestClusterUnitRadiusPosition;
//                var distance = bestFirebatsPosition.Distance(Player.Position);

//                // Walk into cluster or buffed location.
//                if (distance > 3f && !PlayerMover.IsBlocked)
//                {
//                    if (distance > 20f && Skills.WitchDoctor.SpiritWalk.CanCast())
//                    {
//                        Logger.Warn($"Spirit Walking");
//                        Skills.WitchDoctor.SpiritWalk.Cast();
//                    }

//                    Logger.Warn($"Walking to cluster position. Dist: {bestFirebatsPosition.Distance(Player.Position)}");
//                    power = new TrinityPower(SNOPower.Walk, 3f, bestFirebatsPosition,0,0);
//                    return true;
//                }

//                if (Skills.WitchDoctor.Firebats.CanCast())
//                {
//                    var closestUnit = allUnits.OrderBy(u => u.Distance).FirstOrDefault();
//                    if (closestUnit != null)
//                    {
//                        Logger.Warn($"Casting Firebats");
//                        power = new TrinityPower(SNOPower.Witchdoctor_Firebats, 15f, closestUnit.AcdId, 0, 0);
//                        return true;
//                    }
//                }
//            }

//            return true;

//            /*

//            Check best position within mobs (highest density is smallest radius)
//            If Illusory boots, travel up to X yards to get there (too far = death)
//            If distance is greater that X yards and Spirit Walk is available, use Spirit Walk
//            If no illusory or Spirit walk, limit travel distance when health drops below 60%
//            Check to see if there is a beneficial ground effect within 20 yards of best position

//            Check for Necrosis (Wall of Death)
//            10 second 60% damage reduction should always be on to survive
//            Basically always needs to be up when in combat.
//            Should also be cast when buf drops and out of comabt with health under 40%

//            Check for Locust Swarm on enemies
//            .80 of mobs give or take. Prioritize Elites if in range.
//            If Pestilence rune, cast only once
//            Mana is above 300 (will be lower if wearing Topaz in helm, but I prefer amethyst)

//            Check for Haunt on enemies
//            .80 of mobs give or take. Prioritize Elites if in range.
//            *No Mana check. Rush of essence is a required passive for this build.

//            Check for best position in mobs again
//            Is there a ground effect within X yards that I should stand in before channeling?
//            Inner Sanctuary
//            Oculus circle
//            Big Bad Voodoo
//            etc.

//            Check for Soul Harvest Stacks
//            Will stacks end within the next 5 seconds? Re-Cast before channeling.
//            If stack expiration is >5 seconds but enemies withing range are greater than current stacks, recast. 
//            Stack expiriation >5 seconds, current mob count is lower than current stacks, do not cast.
//            Criteria above met, Channel Firebats
//            Do not recast Locust Swarm or Haunt unless <.20 of mobs have them both
//            Pestilence spreads so fast that you probably don't need to recast
//            Haunt moves to another enemy when the first one dies so it doesn't need to be recast often
//            Recast for Elites without them both
//            Lower Avoidance Setting if standing in survival ground effect and currently channeling.
//            Do not move unless emergency health situation arises
//            If 10 second Necrosis debuff ends and above 80% health, continue to channel
//            If Necrosis debuff ends and health is below 80%, break channeling to recast



//            In emergency Health situations (<.40) Don't just kite blindly:
//            If standing in Inner Sanctuary, wait until <.30 or <.20
//            Check for Necrosis (60% damage reduction)
//            Cast if not already up
//            Check for Soul Harvest stacks,
//            If no stacks and enemies in range, cast.
//            Check for Spirit Walk availability
//            Check for Inner Sanctuary or other sustain ground effect and if it can be reached
//            10 yards? 20 yards if illusory boots? 30+ Yards if can cast Spirit walk?
//            Attempt to continue channeling unless
//            Health drops below 20%
//            Avoidance is also triggering
//            Safe area is within X yards

//                */

//            //// Soul harvest for the damage reduction of Okumbas Ornament
//            //if (TrySoulHarvest(out power))
//            //    return true;                 

//            //// Stand in occulus circles
//            //if (TryMoveToBuffedSpot(out power, 20f, 20f, false) && !Player.IsChannelling)
//            //    return true;

//            //// Move in close to clusters.
//            //if (CanCast(SNOPower.Witchdoctor_Firebats) && TargetUtil.AnyMobsInRange(40f) && !Player.IsChannelling)
//            //{
//            //    Logger.Log("Moving into firebat position");
//            //    MoveToFirebatsPoint(Core.Clusters.BestCluster);
//            //}

//            //if (TryBigBadVoodoo(out power))
//            //    return true;

//            //// Only use wall of death for the helltooth set buff.
//            //if (!GetHasBuff(JeramsRevenge) && TryWallOfDeath(out power))
//            //    return true;           

//            //// Piranhas
//            //if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//            //    (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) &&
//            //    LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
//            //    Player.PrimaryResource >= 250)
//            //{
//            //    power = new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Core.Clusters.BestCluster.Position);
//            //    return true;
//            //}

//            //// Locust Swarm for the ring of emptyness debuff only 
//            //if (!CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm) && CanCast(SNOPower.Witchdoctor_Locust_Swarm) && !Player.IsChannelling)
//            //{
//            //    // Avoid trying to put debuffs on all monsters, or monsters that are too far away = more time spent firebatting close units.
//            //    if (TargetUtil.NumMobsInRange(10f) < 3 || TargetUtil.IsPercentOfMobsDebuffed(SNOPower.Witchdoctor_Locust_Swarm, 20f, 0.75f))
//            //    {
//            //        power = new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 20f, CurrentTarget.AcdId);
//            //        return true;
//            //    }
//            //}

//            //// Haunt for the ring of emptyness debuff only 
//            //if (!CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt) && CanCast(SNOPower.Witchdoctor_Haunt) && !Player.IsChannelling)
//            //{
//            //    // Avoid trying to put debuffs on all monsters, or monsters that are too far away = more time spent firebatting close units.
//            //    if (TargetUtil.NumMobsInRange(10f) < 3 || TargetUtil.IsPercentOfMobsDebuffed(SNOPower.Witchdoctor_Haunt, 20f, 0.75f))
//            //    {
//            //        power = new TrinityPower(SNOPower.Witchdoctor_Haunt, 20f, CurrentTarget.AcdId);
//            //        return true;
//            //    }
//            //}

//            //// If resource is really low, try to save up some before starting channelling stacks
//            //var batMana = TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) < 125 ? 75 : 225;

//            //// Firebats: Cloud of bats 
//            //if (Runes.WitchDoctor.CloudOfBats.IsActive && TargetUtil.AnyMobsInRange(Settings.Combat.WitchDoctor.FirebatsRange) &&
//            //    CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= batMana)
//            //{
//            //    power = new TrinityPower(SNOPower.Witchdoctor_Firebats);
//            //    return true;
//            //}

//            //// Firebats: Plague Bats
//            //if (Runes.WitchDoctor.PlagueBats.IsActive && TargetUtil.AnyMobsInRange(15f) &&
//            //    CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= batMana)
//            //{
//            //    var bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, 30f);
//            //    var range = Settings.Combat.WitchDoctor.FirebatsRange > 20f ? 20f : Settings.Combat.WitchDoctor.FirebatsRange;

//            //    power = new TrinityPower(SNOPower.Witchdoctor_Firebats, range, bestClusterPoint, 0, 0);
//            //    return true;
//            //}

//            //power = null;
//            //return false;
//        }

//        private static bool TrySoulHarvest(out TrinityPower power)
//        {
//            power = null;
//            if (CanCast(SNOPower.Witchdoctor_SoulHarvest) && !IsCurrentlyAvoiding)
//            {
//                if (Player.CurrentHealthPct < 0.6 && TargetUtil.AnyMobsInRange(12f))
//                {
//                    {
//                        power = new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                        return true;
//                    }
//                }
//                if (TargetUtil.ClusterExists(3, 12f) && Skills.WitchDoctor.SoulHarvest.BuffStacks < 10)
//                {
//                    Logger.Log(LogCategory.Routine, "Im going in to harvest! 4/12");
//                    MoveToSoulHarvestPoint(Core.Clusters.BestCluster);
//                }
//                else if (TargetUtil.AnyElitesInRange(12f) || TargetUtil.AnyMobsInRange(10f, 2))
//                {
//                    {
//                        power = new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }

//        public static TrinityPower GetPower()
//        {
//            TrinityPower power = null;

//            //Logger.LogNormal("Gargantuan Count = {0}", TrinityPlugin.PlayerOwnedGargantuanCount);
//            //Logger.LogNormal("Fetish Count = {0}", TrinityPlugin.PlayerOwnedFetishArmyCount);

//            // Destructible objects
//            if (UseDestructiblePower || CurrentTarget != null && DataDictionary.CorruptGrowthIds.Contains(CurrentTarget.ActorSnoId))
//            {
//                return DestroyObjectPower;
//            }

//            if (IsHellToothFirebats && TryGetHellToothFirebatsPower(out power))
//                return power;

//            bool hasRestlessGiant = Runes.WitchDoctor.RestlessGiant.IsActive;
//            bool hasWrathfulProtector = Runes.WitchDoctor.WrathfulProtector.IsActive;

//            // Gargantuan should be cast ASAP.
//            if (CanCast(SNOPower.Witchdoctor_Gargantuan))
//            {
//                var hasAllGargs = Player.Summons.GargantuanCount != 0 && (!Legendary.TheShortMansFinger.IsEquipped || Player.Summons.GargantuanCount > 2);
//                if (!hasAllGargs)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Gargantuan);
//                }

//                if (TargetUtil.AnyElitesInRange(30f) && hasWrathfulProtector) //|| hasRestlessGiant?
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Gargantuan);
//                }
//            }            

//            if (IsHellToothPetDoc)
//            {
//                return GetPetDocPower();
//            }

//            // Spam Spirit Walk
//            if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && Settings.Combat.WitchDoctor.UseSpiritWalkOffCooldown)
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
//            }

//            var hasAngryChicken = (Skills.WitchDoctor.Hex.IsActive && Runes.WitchDoctor.AngryChicken.IsActive) || Core.Hotbar.ActivePowers.Contains(SNOPower.Witchdoctor_Hex_ChickenWalk);
//            if (hasAngryChicken && Sets.ManajumasWay.IsEquipped && CanCast(SNOPower.Witchdoctor_Hex) && !ZetaDia.IsInTown)
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_Hex);
//            }

//            // Zombie Dogs should be cast ASAP.
//            if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) &&
//            ((Legendary.TheTallMansFinger.IsEquipped && Player.Summons.ZombieDogCount < 1) ||
//            (!Legendary.TheTallMansFinger.IsEquipped && Player.Summons.ZombieDogCount <= 2)))
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
//            }

//            // Summon Fetish Army
//            var isTikiTorph = Runes.WitchDoctor.TikiTorchers.IsActive;
//            var hasEnoughFetishes = isTikiTorph
//                ? Player.Summons.FetishArmyCount >= 7
//                : Player.Summons.FetishArmyCount >= 5;

//            var useFetishWithZumiSet = !hasEnoughFetishes && Sets.ZunimassasHaunt.IsFirstBonusActive || Settings.Combat.WitchDoctor.UseFetishArmyOffCooldown;
//            var useFetishNormal = !hasEnoughFetishes && !Sets.ZunimassasHaunt.IsFirstBonusActive && (TargetUtil.EliteOrTrashInRange(30f) || TargetUtil.IsEliteTargetInRange(40f)) || Settings.Combat.WitchDoctor.UseFetishArmyOffCooldown;

//            if (CanCast(SNOPower.Witchdoctor_FetishArmy) && (useFetishWithZumiSet || useFetishNormal))
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_FetishArmy);
//            }

//            if (Gems.Taeguk.IsEquipped)
//            {
//                if (IsLonFirebatsBuild)
//                {
//                    var time = CanCast(SNOPower.Witchdoctor_Piranhas) && TargetUtil.ClusterExists(15f, 60f) && CurrentTarget != null && CurrentTarget.IsUnit ? 500 : 1000;
//                    if (TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) > time && Core.Targets.ByType[TrinityObjectType.Unit].Any(a => a.IsHostile && a.Distance < 80f))
//                    {
//                        Logger.Log(LogCategory.Routine,$"Taeguk refresh TimeSinceUse={TimeSincePowerUse(SNOPower.Witchdoctor_Firebats)}");
//                        var forwardPosition = MathEx.GetPointAt(ZetaDia.Me.Position, 15f, ZetaDia.Me.Movement.Rotation);
//                        Skills.WitchDoctor.Firebats.Cast(forwardPosition);
//                    }
//                }
//            }

//            // Combat Avoidance Spells
//            if (!UseOOCBuff && IsCurrentlyAvoiding)
//            {
//                // Spirit Walk out of AoE
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
//                }

//                // Soul harvest at current location while avoiding
//                if (Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive && MinimumSoulHarvestCriteria(Core.Clusters.CloseNearby))
//                {
//                    Skills.WitchDoctor.SoulHarvest.Cast();
//                }
//            }

//            // Incapacitated or Rooted
//            if (!UseOOCBuff && (Player.IsIncapacitated || Player.IsRooted))
//            {
//                // Spirit Walk
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
//                }
//            }

//            // Combat Spells with a Target
//            if (!UseOOCBuff && !IsCurrentlyAvoiding && CurrentTarget != null)
//            {

//                var dartTargetAcdId = CurrentTarget.AcdId;
//                if (Legendary.TheDaggerOfDarts.IsEquipped)
//                {
//                    var target = TargetUtil.GetBestPierceTarget(50f);
//                    if (target != null)
//                        dartTargetAcdId = target.AcdId;
//                }

//                if (_bastianGeneratorWaitTimer.IsFinished && ShouldRefreshBastiansGeneratorBuff)
//                {
//                    if (Hotbar.Contains(SNOPower.Witchdoctor_CorpseSpider) && (!Runes.WitchDoctor.SpiderQueen.IsActive || Trinity.TrinityPlugin.PlayerOwnedSpiderPetsCount == 0))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, 50f, CurrentTarget.AcdId);
//                    }
//                    if (Hotbar.Contains(SNOPower.Witchdoctor_PoisonDart))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, 50f, dartTargetAcdId);
//                    }
//                    if (Hotbar.Contains(SNOPower.Witchdoctor_PlagueOfToads))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, 50f, CurrentTarget.AcdId);
//                    }
//                    if (Hotbar.Contains(SNOPower.Witchdoctor_Firebomb))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Firebomb, 50f, CurrentTarget.AcdId);
//                    }
//                    _bastianGeneratorWaitTimer.Reset();
//                }

//                // Summon Corpse Spider Queen
//                if (CanCast(SNOPower.Witchdoctor_CorpseSpider) && Runes.WitchDoctor.SpiderQueen.IsActive && Trinity.TrinityPlugin.PlayerOwnedSpiderPetsCount == 0)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, 60f, CurrentTarget.AcdId);
//                }


//                bool hasGraveInjustice = Core.Hotbar.PassiveSkills.Contains(SNOPower.Witchdoctor_Passive_GraveInjustice);



//                //                Debug.Print(Core.Hotbar.GetSkill(SNOPower.Witchdoctor_Hex).RuneIndex.ToString());
//                var isChicken = Core.Hotbar.ActivePowers.Contains(SNOPower.Witchdoctor_Hex_ChickenWalk);

//                bool hasVisionQuest = Core.Hotbar.PassiveSkills.Any(s => s == SNOPower.Witchdoctor_Passive_VisionQuest);

//                // Set max ranged attack range, based on Grave Injustice, and current target NOT standing in avoidance, and health > 25%
//                float rangedAttackMaxRange = 30f;
//                if (hasGraveInjustice && !CurrentTarget.IsStandingInAvoidance && Player.CurrentHealthPct > 0.25)
//                    rangedAttackMaxRange = Math.Min(Player.GoldPickupRadius + 8f, 30f);

//                // Set basic attack range, depending on whether or not we have Bears and whether or not we are a tik tank
//                float basicAttackRange = 35f;
//                if (hasGraveInjustice)
//                    basicAttackRange = rangedAttackMaxRange;
//                else if (Hotbar.Contains(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
//                    basicAttackRange = 30f;
//                else if (Legendary.TiklandianVisage.IsEquipped && !TikHorrifyCriteria(Core.Clusters.LargeCluster))
//                    basicAttackRange = 25f;
//                else if (Legendary.TiklandianVisage.IsEquipped)
//                    basicAttackRange = 1f;




//                // Summon Pets  -----------------------------------------------------------------------

//                // Hex with angry chicken, is chicken, explode!
//                if (isChicken && (TargetUtil.AnyMobsInRange(12f, 1, false) || CurrentTarget.RadiusDistance <= 10f || UseDestructiblePower)) // && CanCast(SNOPower.Witchdoctor_Hex_Explode)
//                {
//                    //Debug.Print("Attempting to cast HEx Explosion {0}", ZetaDia.Me.UsePower(SNOPower.Witchdoctor_Hex_Explode, ZetaDia.Me.Position,
//                    //    ZetaDia.WorldId));

//                    return new TrinityPower(SNOPower.Witchdoctor_Hex_Explode);
//                }

//                bool hasJaunt = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 1);
//                bool hasHonoredGuest = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 3);
//                bool hasUmbralShock = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 2);
//                bool hasSeverance = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 0);
//                bool hasHealingJourney = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 4);

//                // Spirit Walk for Goblins chasing
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) &&
//                    CurrentTarget.IsTreasureGoblin && CurrentTarget.HitPointsPct < 0.90 && CurrentTarget.RadiusDistance <= 40f)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
//                }

//                // Spirit Walk < 65% Health: Healing Journey
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && hasHealingJourney && !ZetaDia.IsInTown &&
//                    Player.CurrentHealthPct <= 0.65)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
//                }

//                // Spirit Walk < 50% Mana: Honored Guest
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && hasHonoredGuest && !ZetaDia.IsInTown &&
//                    Player.PrimaryResourcePct <= 0.5)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
//                }

//                // Spirit Walk: Other Runes
//                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && Player.CurrentHealthPct <= .5)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
//                }

//                //bool shouldRefreshVisionQuest = GetTimeSinceLastVisionQuestRefresh() > 4000;
//                bool shouldRefreshVisionQuest = !GetHasBuff(SNOPower.Witchdoctor_Passive_VisionQuest) || GetTimeSinceLastVisionQuestRefresh() > 3800;

//                // Vision Quest Passive
//                if (hasVisionQuest && shouldRefreshVisionQuest)
//                {
//                    // Poison Darts 
//                    if (CanCast(SNOPower.Witchdoctor_PoisonDart))
//                    {
//                        VisionQuestRefreshTimer.Restart();
//                        return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, basicAttackRange, dartTargetAcdId);
//                    }
//                    // Corpse Spiders
//                    if (CanCast(SNOPower.Witchdoctor_CorpseSpider) && (!Runes.WitchDoctor.SpiderQueen.IsActive || Trinity.TrinityPlugin.PlayerOwnedSpiderPetsCount == 0))
//                    {
//                        VisionQuestRefreshTimer.Restart();
//                        return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, basicAttackRange, CurrentTarget.AcdId);
//                    }
//                    // Plague Of Toads 
//                    if (CanCast(SNOPower.Witchdoctor_PlagueOfToads))
//                    {
//                        VisionQuestRefreshTimer.Restart();
//                        return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, basicAttackRange, CurrentTarget.AcdId);
//                    }
//                    // Fire Bomb 
//                    if (CanCast(SNOPower.Witchdoctor_Firebomb))
//                    {
//                        VisionQuestRefreshTimer.Restart();
//                        return new TrinityPower(SNOPower.Witchdoctor_Firebomb, basicAttackRange, CurrentTarget.AcdId); ;
//                    }
//                }

//                // Spam Horrify
//                if (CanCast(SNOPower.Witchdoctor_Horrify) && Settings.Combat.WitchDoctor.SpamHorrify)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
//                }

//                bool hasVengefulSpirit = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SoulHarvest && s.RuneIndex == 4);
//                bool hasSwallowYourSoul = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SoulHarvest && s.RuneIndex == 3);

//                // START Jade Harvester -----------------------------------------------------------------------

//                if (Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive)
//                {
//                    //LogTargetArea("BestLargeCluster", Enemies.BestLargeCluster);
//                    //LogTargetArea("BestCluster", Enemies.BestCluster);
//                    //LogTargetArea("Nearby", Enemies.Nearby);
//                    //LogTargetArea("CloseNearby", Enemies.CloseNearby);

//                    // Piranhas
//                    if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//                        (TargetUtil.ClusterExists(15f, 45f) || TargetUtil.AnyElitesInRange(45f)) &&
//                        LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
//                        Player.PrimaryResource >= 250)
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Core.Clusters.BestCluster.Position);
//                    }

//                    // Should we move to cluster for harvest
//                    if (IdealSoulHarvestCriteria(Core.Clusters.LargeCluster))
//                    {
//                        //LogTargetArea("--- Found a good harvest location...", Enemies.BestLargeCluster);
//                        MoveToSoulHarvestPoint(Core.Clusters.LargeCluster);
//                    }

//                    // Is there a slightly better position than right here
//                    if (MinimumSoulHarvestCriteria(Core.Clusters.BestCluster) && (Core.Clusters.BestCluster.EliteCount >= 2 || Core.Clusters.BestCluster.UnitCount > 4))
//                    {
//                        //LogTargetArea("--- Found an average harvest location...", Enemies.BestCluster);
//                        MoveToSoulHarvestPoint(Core.Clusters.BestCluster);
//                    }

//                    // Should we harvest right here?
//                    if (MinimumSoulHarvestCriteria(Core.Clusters.CloseNearby))
//                    {
//                        //LogTargetArea("--- Harvesting (CurrentPosition)", Enemies.CloseNearby);
//                        return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                    }

//                    // Locust Swarm
//                    if (CanCast(SNOPower.Witchdoctor_Locust_Swarm) && Player.PrimaryResource >= 300 &&
//                        !CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 20f, CurrentTarget.AcdId);
//                    }

//                    // Acid Cloud
//                    if (Skills.WitchDoctor.AcidCloud.CanCast() && Player.PrimaryResource >= 325 &&
//                        LastPowerUsed != SNOPower.Witchdoctor_AcidCloud)
//                    {
//                        Vector3 bestClusterPoint;
//                        if (Passives.WitchDoctor.GraveInjustice.IsActive)
//                            bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, Math.Min(Player.GoldPickupRadius + 8f, 30f));
//                        else
//                            bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, 30f);

//                        return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, rangedAttackMaxRange, bestClusterPoint);
//                    }

//                    // Haunt 
//                    if (CanCast(SNOPower.Witchdoctor_Haunt) && Player.PrimaryResource >= 50 && !CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Haunt, 45f, CurrentTarget.AcdId);
//                    }

//                    if (CanCast(SNOPower.Witchdoctor_Haunt) && !CurrentTarget.IsTreasureGoblin && CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm) &&
//                        CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt) && Player.PrimaryResource >= 350 && !MinimumSoulHarvestCriteria(Core.Clusters.CloseNearby))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Haunt, 45f, CurrentTarget.AcdId);
//                    }

//                    //// Save mana for locust swarm || piranhas
//                    //if (!CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm) && Player.PrimaryResource < 300)
//                    //{
//                    //    //Logger.LogNormal("Saving mana");
//                    //    return DefaultPower;
//                    //}

//                }

//                // END Jade Harvester -----------------------------------------------------------------------

//                // START LoN Firebats -----------------------------------------------------------------------

//                if (IsLonFirebatsBuild)
//                {
//                    //LogTargetArea("BestLargeCluster", Enemies.BestLargeCluster);
//                    //LogTargetArea("BestCluster", Enemies.BestCluster);
//                    //LogTargetArea("Nearby", Enemies.Nearby);
//                    //LogTargetArea("CloseNearby", Enemies.CloseNearby);

//                    // Move to best Firebats point
//                    if (CanCast(SNOPower.Witchdoctor_Firebats) && TargetUtil.AnyMobsInRange(40f))
//                    {
//                        MoveToFirebatsPoint(Core.Clusters.BestCluster);
//                    }

//                    if (TryBigBadVoodoo(out power))
//                        return power;

//                    // Soul Harvest
//                    if (CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
//                        (TargetUtil.AnyElitesInRange(16) || TargetUtil.AnyMobsInRange(16, 2) || TargetUtil.IsEliteTargetInRange(16f)))
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                    }

//                    // Piranhas
//                    if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//                        (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) &&
//                        LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
//                        Player.PrimaryResource >= 250)
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Core.Clusters.BestCluster.Position);
//                    }

//                    var batMana = TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) < 125 ? 75 : 225;
//                    // Firebats: Cloud of bats 
//                    if (Runes.WitchDoctor.CloudOfBats.IsActive && TargetUtil.AnyMobsInRange(Settings.Combat.WitchDoctor.FirebatsRange) &&
//                        CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= batMana)
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Firebats);
//                    }

//                    // Firebats: Plague Bats
//                    if (Runes.WitchDoctor.PlagueBats.IsActive && TargetUtil.AnyMobsInRange(15f) &&
//                        CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= batMana)
//                    {
//                        var bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, 30f);
//                        var range = Settings.Combat.WitchDoctor.FirebatsRange > 20f ? 20f : Settings.Combat.WitchDoctor.FirebatsRange;

//                        return new TrinityPower(SNOPower.Witchdoctor_Firebats, range, bestClusterPoint,0,0);
//                    }

//                }

//                // END LoN Firebats -----------------------------------------------------------------------

//                // Tiklandian Visage ----------------------------------------------------------------------
//                // Constantly casts Horrify and moves the middle of clusters

//                if (Legendary.TiklandianVisage.IsEquipped)
//                {
//                    // Piranhas
//                    if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//                        (TargetUtil.ClusterExists(15f, 45f) || TargetUtil.AnyElitesInRange(45f)) &&
//                        LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
//                        Player.PrimaryResource >= 250)
//                    {
//                        return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Core.Clusters.BestCluster.Position);
//                    }

//                    //Cast Horrify before we go into the fray
//                    if (CanCast(SNOPower.Witchdoctor_Horrify))
//                        return new TrinityPower(SNOPower.Witchdoctor_Horrify);

//                    // Should we move to a better position to fear people
//                    if (TikHorrifyCriteria(Core.Clusters.LargeCluster))
//                        MoveToHorrifyPoint(Core.Clusters.LargeCluster);


//                }

//                // END Tiklandian Visage ----------------------------------------------------------------------   

//                // Sacrifice
//                if (CanCast(SNOPower.Witchdoctor_Sacrifice) && Player.Summons.ZombieDogCount > 0 &&
//                    (TargetUtil.AnyElitesInRange(15, 1) || (CurrentTarget.IsElite && CurrentTarget.RadiusDistance <= 9f)))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice);
//                }

//                // Sacrifice for Circle of Life
//                bool hasCircleofLife = Core.Hotbar.PassiveSkills.Any(s => s == SNOPower.Witchdoctor_Passive_CircleOfLife);
//                if (CanCast(SNOPower.Witchdoctor_Sacrifice) && Player.Summons.ZombieDogCount > 0 && hasCircleofLife && TargetUtil.AnyMobsInRange(15f))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice);
//                }

//                if (TryWallOfDeath(out power))
//                    return power;

//                bool hasSacrifice = Hotbar.Contains(SNOPower.Witchdoctor_Sacrifice);

//                // Zombie Dogs for Sacrifice
//                if (hasSacrifice && CanCast(SNOPower.Witchdoctor_SummonZombieDog) &&
//                    (LastPowerUsed == SNOPower.Witchdoctor_Sacrifice || Player.Summons.ZombieDogCount <= 2) &&
//                    LastPowerUsed != SNOPower.Witchdoctor_SummonZombieDog)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
//                }

//                // Hex with angry chicken, check if we want to shape shift and explode
//                if (CanCast(SNOPower.Witchdoctor_Hex) && (TargetUtil.AnyMobsInRange(12f, 1, false) || CurrentTarget.RadiusDistance <= 10f) &&
//                    hasAngryChicken)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Hex);
//                }

//                // Hex Spam Cast without angry chicken
//                if (CanCast(SNOPower.Witchdoctor_Hex) && !hasAngryChicken &&
//                   (TargetUtil.AnyElitesInRange(12) || TargetUtil.AnyMobsInRange(12, 2) || TargetUtil.IsEliteTargetInRange(18f)))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Hex);
//                }

//                if (CanCast(SNOPower.Witchdoctor_SoulHarvest) && (TargetUtil.AnyElitesInRange(16) || TargetUtil.AnyMobsInRange(16, 2) || TargetUtil.IsEliteTargetInRange(16f)))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                }

//                // Mass Confuse, elites only or big mobs or to escape on low health
//                if (CanCast(SNOPower.Witchdoctor_MassConfusion) &&
//                    (TargetUtil.AnyElitesInRange(12, 1) || TargetUtil.AnyMobsInRange(12, 6) || Player.CurrentHealthPct <= 0.25 || (CurrentTarget.IsElite && CurrentTarget.RadiusDistance <= 12f)) &&
//                    !CurrentTarget.IsTreasureGoblin)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_MassConfusion, 0f, CurrentTarget.AcdId);
//                }

//                if (TryBigBadVoodoo(out power))
//                    return power;

//                // Grasp of the Dead
//                if (CanCast(SNOPower.Witchdoctor_GraspOfTheDead) &&
//                    (TargetUtil.AnyMobsInRange(30, 2) || TargetUtil.EliteOrTrashInRange(30f)) &&
//                    Player.PrimaryResource >= 150)
//                {
//                    var bestClusterPoint = TargetUtil.GetBestClusterPoint();

//                    return new TrinityPower(SNOPower.Witchdoctor_GraspOfTheDead, 25f, bestClusterPoint);
//                }

//                // Piranhas
//                if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
//                    (TargetUtil.ClusterExists(15f, 45f) || TargetUtil.AnyElitesInRange(45f)) &&
//                    Player.PrimaryResource >= 250)
//                {
//                    var bestClusterPoint = TargetUtil.GetBestClusterPoint();

//                    return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, bestClusterPoint);
//                }

//                bool hasPhobia = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 2);
//                bool hasStalker = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 4);
//                bool hasFaceOfDeath = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 1);
//                bool hasFrighteningAspect = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 0);
//                bool hasRuthlessTerror = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 3);

//                float horrifyRadius = hasFaceOfDeath ? 24f : 12f;

//                // Horrify when low on health
//                if (CanCast(SNOPower.Witchdoctor_Horrify) && Player.CurrentHealthPct <= EmergencyHealthPotionLimit && TargetUtil.AnyMobsInRange(horrifyRadius, 3))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
//                }

//                // Horrify Buff at 35% health -- Freightening Aspect
//                if (CanCast(SNOPower.Witchdoctor_Horrify) && Player.CurrentHealthPct <= 0.35 && hasFrighteningAspect)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
//                }

//                // Fetish Army
//                if (CanCast(SNOPower.Witchdoctor_FetishArmy) &&
//                    (TargetUtil.EliteOrTrashInRange(30f) || TargetUtil.IsEliteTargetInRange(30f) || Settings.Combat.WitchDoctor.UseFetishArmyOffCooldown))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_FetishArmy);
//                }

//                bool hasManitou = Runes.WitchDoctor.Manitou.IsActive;

//                // Spirit Barrage Manitou
//                if (CanCast(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource >= 100 &&
//                    TimeSincePowerUse(SNOPower.Witchdoctor_SpiritBarrage) > 18000 && hasManitou)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage);
//                }

//                bool hasResentfulSpirit = Runes.WitchDoctor.ResentfulSpirits.IsActive;
//                // Haunt 
//                if (CanCast(SNOPower.Witchdoctor_Haunt) &&
//                    Player.PrimaryResource >= 50 &&
//                    !SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.Witchdoctor_Haunt) &&
//                    LastPowerUsed != SNOPower.Witchdoctor_Haunt)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Haunt, 21f, CurrentTarget.AcdId);
//                }

//                //skillDict.Add("LocustSwarm", SNOPower.Witchdoctor_Locust_Swarm);

//                // Locust Swarm
//                if (CanCast(SNOPower.Witchdoctor_Locust_Swarm) && Player.PrimaryResource >= 300 && !Legendary.Wormwood.IsEquipped &&
//                    !SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.Witchdoctor_Locust_Swarm) && LastPowerUsed != SNOPower.Witchdoctor_Locust_Swarm)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 12f, CurrentTarget.AcdId);
//                }

//                // Sacrifice for 0 Dogs
//                if (CanCast(SNOPower.Witchdoctor_Sacrifice) &&
//                    (Settings.Combat.WitchDoctor.ZeroDogs || !WitchDoctorHasPrimaryAttack))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice, 9f);
//                }

//                var zombieChargerRange = hasGraveInjustice ? Math.Min(Player.GoldPickupRadius + 8f, 11f) : 11f;

//                // Zombie Charger aka Zombie bears Spams Bears @ Everything from 11feet away
//                if (CanCast(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_ZombieCharger, zombieChargerRange, CurrentTarget.Position);
//                }

//                // Soul Harvest Any Elites or to increase buff stacks
//                if (!Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive && CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
//                    (TargetUtil.AnyMobsInRange(14f, GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest) + 1, false) || (hasSwallowYourSoul && Player.PrimaryResourcePct <= 0.50) || TargetUtil.IsEliteTargetInRange(14f)))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                }

//                // Soul Harvest with VengefulSpirit
//                if (!Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive && CanCast(SNOPower.Witchdoctor_SoulHarvest) && hasVengefulSpirit &&
//                    TargetUtil.AnyMobsInRange(14, 3))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//                }

//                var hasVampireBats = Runes.WitchDoctor.VampireBats.IsActive;
//                var hasCloudOfBats = Runes.WitchDoctor.CloudOfBats.IsActive;

//                var fireBatsChannelCost = hasVampireBats ? 0 : 75;
//                var fireBatsMana = TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) < 125 ? fireBatsChannelCost : 225;

//                var firebatsMaintain =
//                  Trinity.TrinityPlugin.Targets.Any(u => u.IsUnit &&
//                      u.IsPlayerFacing(70f) && u.Weight > 0 &&
//                      u.Distance <= 35 &&
//                      SpellHistory.TimeSinceUse(SNOPower.Witchdoctor_Firebats) <= TimeSpan.FromMilliseconds(250d));

//                // Fire Bats:Cloud of bats 
//                if (hasCloudOfBats && (TargetUtil.AnyMobsInRange(12f)) &&
//                    CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= fireBatsMana)
//                {
//                    var range = Settings.Combat.WitchDoctor.FirebatsRange > 12f ? 12f : Settings.Combat.WitchDoctor.FirebatsRange;

//                    return new TrinityPower(SNOPower.Witchdoctor_Firebats, range, CurrentTarget.AcdId);
//                }

//                // Fire Bats fast-attack
//                if (CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= fireBatsMana &&
//                     (TargetUtil.AnyMobsInRange(Settings.Combat.WitchDoctor.FirebatsRange) || firebatsMaintain) && !hasCloudOfBats)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Firebats, Settings.Combat.WitchDoctor.FirebatsRange, CurrentTarget.Position);
//                }

//                // Acid Cloud
//                if (CanCast(SNOPower.Witchdoctor_AcidCloud) && Player.PrimaryResource >= 175)
//                {
//                    Vector3 bestClusterPoint;
//                    if (hasGraveInjustice)
//                        bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, Math.Min(Player.GoldPickupRadius + 8f, 30f));
//                    else
//                        bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, 30f);

//                    return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, rangedAttackMaxRange, bestClusterPoint);
//                }

//                bool hasWellOfSouls = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritBarrage && s.RuneIndex == 1);
//                bool hasRushOfEssence = Core.Hotbar.PassiveSkills.Any(s => s == SNOPower.Witchdoctor_Passive_RushOfEssence);

//                // Spirit Barrage + Rush of Essence
//                if (CanCast(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource >= 100 &&
//                    hasRushOfEssence && !hasManitou)
//                {
//                    if (hasWellOfSouls)
//                        return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, 21f, CurrentTarget.AcdId);

//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, 21f, CurrentTarget.AcdId);
//                }

//                // Zombie Charger backup
//                if (CanCast(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_ZombieCharger, zombieChargerRange, CurrentTarget.Position);
//                }

//                // Regular spirit barage
//                if (CanCast(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource >= 100 && !hasManitou)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, basicAttackRange, CurrentTarget.AcdId);
//                }

//                // Poison Darts fast-attack Spams Darts when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
//                if (CanCast(SNOPower.Witchdoctor_PoisonDart))
//                {
//                    VisionQuestRefreshTimer.Restart();
//                    return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, basicAttackRange, dartTargetAcdId);
//                }
//                // Corpse Spiders fast-attacks Spams Spiders when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
//                if (CanCast(SNOPower.Witchdoctor_CorpseSpider) && (!Runes.WitchDoctor.SpiderQueen.IsActive || Trinity.TrinityPlugin.PlayerOwnedSpiderPetsCount == 0))
//                {
//                    VisionQuestRefreshTimer.Restart();
//                    return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, basicAttackRange, CurrentTarget.AcdId);
//                }
//                // Toads fast-attacks Spams Toads when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
//                if (CanCast(SNOPower.Witchdoctor_PlagueOfToads))
//                {
//                    VisionQuestRefreshTimer.Restart();
//                    return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, basicAttackRange, CurrentTarget.AcdId);
//                }
//                // Fire Bomb fast-attacks Spams Bomb when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
//                if (CanCast(SNOPower.Witchdoctor_Firebomb))
//                {
//                    VisionQuestRefreshTimer.Restart();
//                    return new TrinityPower(SNOPower.Witchdoctor_Firebomb, basicAttackRange, CurrentTarget.AcdId);
//                }

//                //Hexing Pants Mod
//                if (Legendary.HexingPantsOfMrYan.IsEquipped && CurrentTarget.IsUnit &&
//                //!CanCast(SNOPower.Witchdoctor_Piranhas) && 
//                CurrentTarget.RadiusDistance > 10f)
//                {
//                    return new TrinityPower(SNOPower.Walk, 10f, CurrentTarget.Position);
//                }

//                if (Legendary.HexingPantsOfMrYan.IsEquipped && CurrentTarget.IsUnit &&
//                //!CanCast(SNOPower.Witchdoctor_Piranhas) && 
//                CurrentTarget.RadiusDistance < 10f)
//                {
//                    Vector3 vNewTarget = MathEx.CalculatePointFrom(CurrentTarget.Position, Player.Position, -10f);
//                    return new TrinityPower(SNOPower.Walk, 10f, vNewTarget);
//                }

//            }

//            // Buffs
//            if (UseOOCBuff)
//            {
//                //Spam fear at all times if Tiklandian Visage is ewquipped and fear spam is selected to keep fear buff active
//                if (CanCast(SNOPower.Witchdoctor_Horrify) && Settings.Combat.WitchDoctor.SpamHorrify && Legendary.TiklandianVisage.IsEquipped)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
//                }

//                bool hasStalker = Core.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 4);
//                // Horrify Buff When not in combat for movement speed -- Stalker
//                if (CanCast(SNOPower.Witchdoctor_Horrify) && hasStalker)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
//                }

//                // Zombie Dogs non-sacrifice build
//                if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) &&
//                ((Legendary.TheTallMansFinger.IsEquipped && Player.Summons.ZombieDogCount < 1) ||
//                (!Legendary.TheTallMansFinger.IsEquipped && Player.Summons.ZombieDogCount <= 2)))
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
//                }

//                if (CanCast(SNOPower.Witchdoctor_Gargantuan) && !hasRestlessGiant && !hasWrathfulProtector && Player.Summons.GargantuanCount == 0)
//                {
//                    return new TrinityPower(SNOPower.Witchdoctor_Gargantuan);
//                }
//            }

//            // Default Attacks
//            if (IsNull(power))
//            {
//                // Never use Melee (e.g. Range < 15f), only ranged attacks
//                var position = StuckHandler.RandomShuffle(StuckHandler.GetCirclePoints(2, 20f, Player.Position)).FirstOrDefault();
//                if (CurrentTarget != null)
//                    power = new TrinityPower(SNOPower.Walk, 20f, position);
//            }

//            return power;
//        }

//        private static bool TryWallOfDeath(out TrinityPower power)
//        {
//            power = null;
//            if (CurrentTarget == null)
//                return false;

//            if (CanCast(SNOPower.Witchdoctor_WallOfZombies) && TargetUtil.AnyMobsInRange(50f))
//            {
//                power = new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 25f, TargetUtil.GetBestClusterPoint());
//                return true;           
//            }            
//            return false;
//        }

//        private static bool TryBigBadVoodoo(out TrinityPower power)
//        {
//            power = null;
//            if (Player.IsIncapacitated || !CanCast(SNOPower.Witchdoctor_BigBadVoodoo))
//            {
//                return false;
//            }            
//            if (Settings.Combat.WitchDoctor.UseBigBadVoodooOffCooldown || TargetUtil.AnyElitesInRange(30f))
//            {
//                power = new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo);
//                return true;     
//            }                      
//            return false;
//        }

//        private static bool IsLonFirebatsBuild
//        {
//            get { return Sets.LegacyOfNightmares.IsFullyEquipped && Skills.WitchDoctor.Firebats.IsActive; }
//        }

//        private static readonly Func<TargetArea, bool> MinimumSoulHarvestCriteria = area =>

//             // Harvest is off cooldown AND at least 2 debuffs exists AND at least 40% of the units have a harvestable debuff
//             CanCast(SNOPower.Witchdoctor_SoulHarvest) && ((area.TotalDebuffCount(HarvesterCoreDebuffs) >= 2 &&
//             area.DebuffedCount(HarvesterCoreDebuffs) >= area.UnitCount * 0.4) ||

//             // OR we're gonna die if we don't
//             (Player.CurrentHealthPct <= .45 && !Skills.WitchDoctor.SpiritWalk.CanCast())) &&

//             // AND there's an elite, boss or more than 3 units or greater 35% of the units within sight are within this cluster
//             (area.EliteCount > 0 || area.BossCount > 0 || area.UnitCount >= 3 || area.UnitCount >= (float)Core.Clusters.Nearby.UnitCount * 0.35);


//        private static readonly Func<TargetArea, bool> IdealSoulHarvestCriteria = area =>

//            // Harvest is off cooldown AND at least 7 debuffs are present (can be more than 1 per unit)
//            CanCast(SNOPower.Witchdoctor_SoulHarvest) && area.TotalDebuffCount(HarvesterDebuffs) > 7 &&

//            // AND average health accross units in area is more than 30%
//            area.AverageHealthPct > 0.3f &&

//            // AND at least 2 Elites, a boss or more than 5 units or 80% of the nearby units are within this area
//            (area.EliteCount >= 2 || area.BossCount > 0 || area.UnitCount >= 5 || area.UnitCount >= (float)Core.Clusters.Nearby.UnitCount * 0.80);

//        private static readonly Func<TargetArea, bool> TikHorrifyCriteria = area =>

//            //at least 2 Elites, a boss or more than 5 units or 80% of the nearby units are within this area
//            (area.EliteCount >= 2 || area.UnitCount >= 5 || area.UnitCount >= (float)Core.Clusters.Nearby.UnitCount * 0.80);

//        private static readonly Func<TargetArea, bool> LonFireBatsCriteria = area =>

//            //at least 1 Elites or more than 5 units or mobs inside a piranhado
//            (area.EliteCount > 0 || area.UnitCount >= 5 || area.TotalDebuffCount(SNOPower.Witchdoctor_Piranhas) > 0);


//        private static readonly Action<string, TargetArea> LogTargetArea = (message, area) =>
//        {
//            Logger.LogDebug(message + " Units={0} Elites={1} DebuffedUnits={2} TotalDebuffs={4} AvgHealth={3:#.#} ---",
//                area.UnitCount,
//                area.EliteCount,
//                area.DebuffedCount(HarvesterDebuffs),
//                area.AverageHealthPct * 100,
//                area.TotalDebuffCount(HarvesterDebuffs));
//        };

//        private static void MoveToSoulHarvestPoint(TargetArea area)
//        {
//            //CombatMovement.Queue(new CombatMovement
//            //{
//            //    Name = "Jade Harvest Position",                
//            //    Destination = area.Position,                
//            //    StopCondition = m =>
//            //    {
//            //        return !CanCast(SNOPower.Witchdoctor_SoulHarvest) || PlayerMover.IsBlocked || Core.PlayerHistory.MoveSpeed < 2;
//            //    },
//            //    OnUpdate = m =>
//            //    {
//            //        // Only change destination if the new target is way better
//            //        if (IdealSoulHarvestCriteria(Core.Clusters.BestLargeCluster) &&
//            //            Core.Clusters.BestLargeCluster.Position.Distance(m.Destination) > 10f)
//            //            m.Destination = Core.Clusters.BestLargeCluster.Position;

//            //        Logger.Log(LogCategory.Routine, $"Moving to Harvest Point {m}");

//            //        if (TargetUtil.NumMobsInRange(12f) >= 3 && Skills.WitchDoctor.SoulHarvest.CanCast())
//            //            Skills.WitchDoctor.SoulHarvest.Cast();
//            //    },
//            //    OnFinished = m =>
//            //    {
//            //        if (MinimumSoulHarvestCriteria(Core.Clusters.CloseNearby))
//            //        {
//            //            //LogTargetArea("--- Harvesting (CombatMovement)", area);
//            //            Skills.WitchDoctor.SoulHarvest.Cast();
//            //        }
//            //    },
//            //    Options = new CombatMovementOptions
//            //    {                    
//            //        AcceptableDistance = 10f,
//            //        Logging = LogLevel.Verbose,
//            //    }
//            //});
//        }


//        private static void MoveToHorrifyPoint(TargetArea area)
//        {
//            //CombatMovement.Queue(new CombatMovement
//            //{
//            //    Name = "Horrify Position",
//            //    Destination = area.Position,
//            //    OnUpdate = m =>
//            //    {
//            //        // Only change destination if the new target is way better
//            //        if (TikHorrifyCriteria(Core.Clusters.BestLargeCluster) &&
//            //            Core.Clusters.BestLargeCluster.Position.Distance(m.Destination) > 15f)
//            //            m.Destination = Core.Clusters.BestLargeCluster.Position;
//            //    },
//            //    Options = new CombatMovementOptions
//            //    {
//            //        AcceptableDistance = 12f,
//            //        Logging = LogLevel.Verbose,
//            //        ChangeInDistanceLimit = 2f,
//            //        SuccessBlacklistSeconds = 3,
//            //        FailureBlacklistSeconds = 7,
//            //        TimeBeforeBlocked = 500
//            //    }

//            //});
//        }

//        private static void MoveToFirebatsPoint(TargetArea area)
//        {
//            //CombatMovement.Queue(new CombatMovement
//            //{
//            //    Name = "Firebats Position",
//            //    Destination = area.Position,
//            //    OnUpdate = m =>
//            //    {
//            //        if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
//            //            Skills.WitchDoctor.SpiritWalk.Cast();
//            //        // Only change destination if the new target is way better
//            //        if (LonFireBatsCriteria(Core.Clusters.BestCluster) &&
//            //            Core.Clusters.BestCluster.Position.Distance(m.Destination) > 15f)
//            //            m.Destination = Core.Clusters.BestCluster.Position;
//            //    },
//            //    Options = new CombatMovementOptions
//            //    {
//            //        AcceptableDistance = 8f,
//            //        Logging = LogLevel.Verbose,
//            //        ChangeInDistanceLimit = 2f,
//            //        SuccessBlacklistSeconds = 3,
//            //        FailureBlacklistSeconds = 7,
//            //        TimeBeforeBlocked = 500
//            //    }

//            //});
//        }

//        private static bool WitchDoctorHasPrimaryAttack
//        {
//            get
//            {
//                return
//                    Hotbar.Contains(SNOPower.Witchdoctor_WallOfZombies) ||
//                    Hotbar.Contains(SNOPower.Witchdoctor_Firebats) ||
//                    Hotbar.Contains(SNOPower.Witchdoctor_AcidCloud) ||
//                    Hotbar.Contains(SNOPower.Witchdoctor_ZombieCharger) ||
//                    Hotbar.Contains(SNOPower.Witchdoctor_PoisonDart) ||
//                    Hotbar.Contains(SNOPower.Witchdoctor_CorpseSpider) ||
//                    Hotbar.Contains(SNOPower.Witchdoctor_PlagueOfToads) ||
//                    Hotbar.Contains(SNOPower.Witchdoctor_Firebomb);
//            }
//        }

//        private static TrinityPower DestroyObjectPower
//        {
//            get
//            {
//                if (CurrentTarget == null)
//                    return DefaultPower;

//                if (CanCast(SNOPower.Witchdoctor_Firebomb))
//                    return new TrinityPower(SNOPower.Witchdoctor_Firebomb, 12f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_PoisonDart))
//                    return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, 15f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
//                    return new TrinityPower(SNOPower.Witchdoctor_ZombieCharger, 12f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_CorpseSpider))
//                    return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, 12f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_PlagueOfToads))
//                    return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, 12f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_AcidCloud) && Player.PrimaryResource >= 175)
//                    return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, 25f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= 75 && CurrentTarget != null && CurrentTarget.Distance < 15f)
//                    return new TrinityPower(SNOPower.Witchdoctor_Firebats, 12f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_Sacrifice) && Hotbar.Contains(SNOPower.Witchdoctor_SummonZombieDog) &&
//                    Player.Summons.ZombieDogCount > 0 && Settings.Combat.WitchDoctor.ZeroDogs)
//                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice, 12f, CurrentTarget.Position);
//                if (CanCast(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource > 100)
//                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, 12f, CurrentTarget.AcdId);
//                if (CanCast(SNOPower.Witchdoctor_Haunt) && Player.PrimaryResource > 50 && LastPowerUsed != SNOPower.Witchdoctor_Haunt)
//                    return new TrinityPower(SNOPower.Witchdoctor_Haunt, 12f, CurrentTarget.AcdId);
//                if (CanCast(SNOPower.Witchdoctor_SoulHarvest))
//                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest, 12f, CurrentTarget.AcdId);

//                return DefaultPower;
//            }
//        }

//        // This is an old, buggy version of GetSafeSportPosition (the new one is in TargetUtil).
//        // For some reason this version works better fo WD, so here it is.
//        private static Vector3 _lastSafeSpotPosition = Vector3.Zero;
//        private static DateTime _lastSafeSpotPositionTime = DateTime.MinValue;

//        private static Vector3 GetSafeSpotPosition(float distance)
//        {
//            // Maximum speed of changing safe spots is every 2s
//            if (DateTime.UtcNow.Subtract(_lastSafeSpotPositionTime).TotalSeconds < 2)
//                return _lastSafeSpotPosition;

//            // If we have a position already and its still within range and still no monster there, keep it.
//            var safeSpotIsClose = _lastSafeSpotPosition.Distance(Player.Position) < distance;
//            var safeSpotHasNoMonster = !TargetUtil.IsPositionOnMonster(_lastSafeSpotPosition);
//            if (_lastSafeSpotPosition != Vector3.Zero && safeSpotIsClose && safeSpotHasNoMonster)
//                return _lastSafeSpotPosition;

//            Func<Vector3, bool> isValid = p => NavHelper.CanRayCast(p) && !TargetUtil.IsPositionOnMonster(p) && p.Distance(Player.Position) > distance;

//            var circlePositions = StuckHandler.GetCirclePoints(8, 10, Player.Position);

//            if (distance >= 20)
//                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 20, Player.Position));

//            if (distance >= 30)
//                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 30, Player.Position));

//            var closestMonster = TargetUtil.GetClosestUnit();
//            var proximityTarget = closestMonster != null ? closestMonster.Position : Player.Position;
//            var validPositions = circlePositions.Where(isValid).OrderBy(p => p.Distance(proximityTarget));

//            _lastSafeSpotPosition = validPositions.FirstOrDefault();
//            _lastSafeSpotPositionTime = DateTime.UtcNow;
//            return _lastSafeSpotPosition;
//        }

//    }
//}
