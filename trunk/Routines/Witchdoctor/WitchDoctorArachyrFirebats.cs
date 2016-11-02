using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Witchdoctor
{
    public sealed class WitchDoctorArachyrFirebats : WitchDoctorBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Arachyr Firebats";
        public string Description => "Specialized combat for channelling firebats with Arachyr set.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.diablofans.com/builds/82419-2-4-2-helltooth-firebats-dps-105";



        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.SpiritOfArachnyr, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.WitchDoctor.Firebats, null },
            },
        };

        public override Func<bool> ShouldIgnoreKiting => IgnoreCondition;
        public override Func<bool> ShouldIgnoreAvoidance => IgnoreCondition;
        public override Func<bool> ShouldIgnorePackSize => () => Player.IsChannelling;
        public override Func<bool> ShouldIgnoreNonUnits => () => Player.IsChannelling && Player.CurrentHealthPct > 0.35;

        #endregion

        private bool IgnoreCondition()
        {
            var isFirebatsSelected = Combat.Targeting.CurrentPower?.SNOPower == SNOPower.Witchdoctor_Firebats;
            var isInAvoidance = Core.Avoidance.InCriticalAvoidance(Player.Position);
            if (isFirebatsSelected && TargetUtil.AnyMobsInRange(FireBatsRange) && Player.CurrentHealthPct > 0.5f && !isInAvoidance)
                return true;

            return Player.IsChannelling && Player.CurrentHealthPct > 0.35 && !Core.Avoidance.InCriticalAvoidance(Player.Position);
        }

        public TrinityPower GetOffensivePower()
        {
            Vector3 position;
            TrinityPower power;

            var allUnits = Core.Targets.ByType[TrinityObjectType.Unit].Where(u => u.IsUnit && u.RadiusDistance <= 50f).ToList();

            var clusterUnits =
                (from u in allUnits
                 where u.IsUnit && u.Weight > 0 && !u.IsPlayer
                 orderby
                 u.NearbyUnitsWithinDistance(15f) descending,
                 u.Distance,
                 u.HitPointsPct descending
                 select u).ToList();

            var bestClusterUnit = clusterUnits.FirstOrDefault();

            //10 second 60% damage reduction should always be on to survive
            if (!HasJeramsRevengeBuff && Player.CurrentHealthPct > 0.4 && !Core.Avoidance.InCriticalAvoidance(Player.Position) && (ZetaDia.Me.IsInCombat || Player.CurrentHealthPct < 0.4) && bestClusterUnit != null && Skills.WitchDoctor.WallOfDeath.CanCast())
            {
                Logger.Log(LogCategory.Routine, $"Casting Wall of Death on {allUnits.FirstOrDefault()}");
                return WallOfDeath(allUnits.FirstOrDefault());
            }

            if (bestClusterUnit != null)
            {
                if (!HasJeramsRevengeBuff && ZetaDia.Me.IsInCombat && Skills.WitchDoctor.WallOfDeath.CanCast())
                {
                    Logger.Log(LogCategory.Routine, $"Casting Wall of Death on {allUnits.FirstOrDefault()}");
                    return WallOfDeath(allUnits.FirstOrDefault());
                }

                if (!Player.HasBuff(SNOPower.Witchdoctor_Hex) && Skills.WitchDoctor.Hex.CanCast())
                {
                    Logger.Log(LogCategory.Routine, $"Casting Hex");
                    return Hex(CurrentTarget.Position);
                }

                var targetsWithoutLocust = clusterUnits.Where(u => !u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm)).OrderBy(u => u.Distance);
                var isAnyTargetWithLocust = clusterUnits.Any(u => u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm) && u.Distance < 45f);
                var percentTargetsWithHaunt = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Haunt, 8f);
                var percentTargetsWithLocust = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Locust_Swarm, 12f);
                var isEliteWithoutHaunt = clusterUnits.Any(u => u.IsElite && !u.HasDebuff(SNOPower.Witchdoctor_Haunt));
                var isElitewithoutLocust = clusterUnits.Any(u => u.IsElite && !u.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm));
                var harvestStacks = Skills.WitchDoctor.SoulHarvest.BuffStacks;
                var harvestBuffCooldown = Core.Cooldowns.GetBuffCooldown(SNOPower.Witchdoctor_SoulHarvest);
                var harvestPossibleStackGain = 10 - harvestStacks;
                var harvestUnitsInRange = allUnits.Count(u => u.Distance < 12f);
                var interruptForHarvest = Skills.WitchDoctor.SoulHarvest.CanCast() && harvestPossibleStackGain >= harvestUnitsInRange && harvestBuffCooldown?.Remaining.TotalMilliseconds < 500;
                var interruptForHaunt = percentTargetsWithHaunt < 0.2f || isEliteWithoutHaunt;  
                var needToSwarmElite = isElitewithoutLocust && !((Legendary.VileHive.IsEquipped || Runes.WitchDoctor.Pestilence.IsActive) && isAnyTargetWithLocust);
                var interruptForLocust = (percentTargetsWithLocust < 0.1f || needToSwarmElite) && Player.PrimaryResource > 300 && Skills.WitchDoctor.LocustSwarm.CanCast();

                // continue channelling firebats?
                if (Player.IsChannelling)
                {
                    if (!interruptForHaunt && !interruptForLocust && !interruptForHarvest)
                    {
                        Logger.Log(LogCategory.Routine, "Continuation of Firebats.");
                        return new TrinityPower(SNOPower.Witchdoctor_Firebats, 30f, Player.Position, 0, 0);
                    }

                    if (interruptForHaunt)
                        Logger.Log(LogCategory.Routine, "Interrupted Firebats to haunt");

                    if (interruptForLocust)
                        Logger.Log(LogCategory.Routine, "Interrupted Firebats to locust");

                    if (interruptForHarvest)
                        Logger.Log(LogCategory.Routine, "Interrupted Firebats to harvest");
                }

                // Emergency health situation
                if (Player.CurrentHealthPct < 0.35)
                {
                    if (Skills.WitchDoctor.SpiritWalk.CanCast())
                    {
                        Logger.Log(LogCategory.Routine, $"Defensive Spirit Walking");
                        return SpiritWalk();
                    }

                    if (TargetUtil.AnyMobsInRange(12f) && Skills.WitchDoctor.SoulHarvest.CanCast())
                    {
                        Logger.Log(LogCategory.Routine, "Emergency Harvest");
                        return SoulHarvest();
                    }

                    if (!HasJeramsRevengeBuff && Skills.WitchDoctor.WallOfDeath.CanCast() && allUnits.Any())
                    {
                        Logger.Log(LogCategory.Routine, $"Casting Defensive WallOfDeath on {allUnits.FirstOrDefault()}");
                        return WallOfDeath(allUnits.FirstOrDefault());
                    }
                }

                // Locust
                if (Skills.WitchDoctor.LocustSwarm.CanCast() && Skills.WitchDoctor.LocustSwarm.TimeSinceUse > 1000 && targetsWithoutLocust.Any() && (!Runes.WitchDoctor.Pestilence.IsActive || !isAnyTargetWithLocust))
                {
                    if ((percentTargetsWithLocust < Settings.LocustPct || needToSwarmElite) && Player.PrimaryResource > 300 && targetsWithoutLocust.Any())
                    {
                        Logger.Log(LogCategory.Routine, "Locust");
                        return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 10f, targetsWithoutLocust.First().Position, 0, 0);
                    }
                }

                // Soul harvest for the damage reduction of Okumbas Ornament
                if (Skills.WitchDoctor.SoulHarvest.CanCast() && (bestClusterUnit.Distance < 12f || harvestStacks < 4 && TargetUtil.AnyMobsInRange(10f)) && harvestStacks < 10)
                {
                    Logger.Log(LogCategory.Routine, $"Harvest State: StackGainPossible={harvestPossibleStackGain} Units={harvestUnitsInRange} BuffRemainingSecs:{harvestBuffCooldown?.Remaining.TotalSeconds:N2}");

                    if (harvestPossibleStackGain <= harvestUnitsInRange)
                    {
                        Logger.Log(LogCategory.Routine, $"Soul Harvest.");
                        return SoulHarvest();
                    }
                }

                if (ShouldBigBadVoodoo(out position))
                    return BigBadVoodoo(position);

                // Piranhas
                if (Skills.WitchDoctor.Piranhas.CanCast() && Player.PrimaryResource >= 250 &&
                    (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) && Player.PrimaryResource >= 250)
                {
                    return Piranhas(TargetUtil.GetBestClusterUnit());
                }

                // .80 of mobs give or take. Spelltracker check is to prevent repeat casts ont he same target before the projectile arrives.                 
                var targetsWithoutHaunt = clusterUnits.Where(u => !u.HasDebuff(SNOPower.Witchdoctor_Haunt) && !SpellTracker.IsUnitTracked(u, SNOPower.Witchdoctor_Haunt)).OrderBy(u => u.Distance);
                if ((percentTargetsWithHaunt < Settings.HauntPct || isEliteWithoutHaunt) && targetsWithoutHaunt.Any() && Player.PrimaryResource > 100)
                {
                    var target = targetsWithoutHaunt.First();
                    Logger.Log(LogCategory.Routine, $"Haunt on {target}");
                    return Haunt(target);
                }

                Vector3 bestBuffedPosition;
                TargetUtil.BestBuffPosition(16f, bestClusterUnit.Position, true, out bestBuffedPosition);
                var bestClusterUnitRadiusPosition = MathEx.GetPointAt(bestClusterUnit.Position, bestClusterUnit.CollisionRadius * 1.1f, bestClusterUnit.Rotation);
                var bestFirebatsPosition = bestBuffedPosition != Vector3.Zero ? bestBuffedPosition : bestClusterUnitRadiusPosition;
                var distance = bestFirebatsPosition.Distance(Player.Position);

                // Walk into cluster or buffed location.
                if (distance > 10f && !PlayerMover.IsBlocked)
                {
                    if (distance > 20f && Skills.WitchDoctor.SpiritWalk.CanCast())
                    {
                        Logger.Log(LogCategory.Routine, $"Spirit Walking");
                        return SpiritWalk();
                    }

                    Logger.Warn($"Walking to cluster position. Dist: {bestFirebatsPosition.Distance(Player.Position)}");    
                    return new TrinityPower(SNOPower.Walk, 3f, bestFirebatsPosition, 0, 0);
                }

                if (Skills.WitchDoctor.Firebats.CanCast())
                {
                    var closestUnit = allUnits.OrderBy(u => u.Distance).FirstOrDefault();
                    if (closestUnit != null)
                    {
                        Logger.Log(LogCategory.Routine, $"Casting Firebats");
                        return Firebats(closestUnit);
                    }
                }
            }

            //if (IsChannellingFirebats && Player.CurrentHealthPct > 0.5f && TargetUtil.AnyMobsInRange(FireBatsRange))
            //    return Firebats();

            //if (TrySpecialPower(out power))
            //    return power;

            //if (TrySecondaryPower(out power))
            //    return power;

            //if (TryPrimaryPower(out power))
            //    return power;

            return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 15f));
        }

        public TrinityPower GetBuffPower()
        {
            Vector3 position;

            if (ShouldSummonZombieDogs(out position))
                return IsInCombat ? SummonZombieDogs(position) : SummonZombieDogs();

            if (ShouldGargantuan(out position))
                return IsInCombat ? Gargantuan(position) : Gargantuan();

            if (Settings.SpiritWalk.UseMode == UseTime.Always && !Player.IsInTown && Skills.WitchDoctor.SpiritWalk.CanCast())
                return SpiritWalk();

            if (ShouldSpiritWalk())
                return SpiritWalk();

            if (ShouldFetishArmy())
                return FetishArmy();

            if (Skills.WitchDoctor.WallOfDeath.CanCast() && IsInCombatOrBeingAttacked)
                return WallOfDeath(TargetUtil.GetBestClusterPoint());

            if (Skills.WitchDoctor.SoulHarvest.CanCast() && (!IsChannellingFirebats || Player.CurrentHealthPct < 0.4f) && HostileMonsters.Any(u => u.Distance < 16f))
            {
                // Refresh the buff time to avoid losing 10 stacks.
                if (Skills.WitchDoctor.SoulHarvest.TimeSinceUse > 4500)
                    return SoulHarvest();

                // Build some stacks
                if (Skills.WitchDoctor.SoulHarvest.BuffStacks < MaxSoulHarvestStacks)
                    return SoulHarvest();
            }

            return null;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();
        public TrinityPower GetMovementPower(Vector3 destination)
        {
            Vector3 position;

            if (!Player.IsInTown)
            {
                if (Runes.WitchDoctor.AngryChicken.IsActive && ShouldHex(out position))
                    return Hex(position);
            }

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public override float KiteDistance => 5f;
        public override int KiteHealthPct => 90;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WitchDoctorArachyrFirebatsSettings Settings { get; } = new WitchDoctorArachyrFirebatsSettings();

        public sealed class WitchDoctorArachyrFirebatsSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;
            private SkillSettings _spiritWalk;
                        private float _hauntPct;
            private float _locustPct;

            [DefaultValue(8)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            public SkillSettings SpiritWalk
            {
                get { return _spiritWalk; }
                set { SetField(ref _spiritWalk, value); }
            }

            [DefaultValue(0.35f)]
            public float LocustPct
            {
                get { return _locustPct; }
                set { SetField(ref _locustPct, value); }
            }
             
            [DefaultValue(0.35f)]
            public float HauntPct
            {
                get { return _hauntPct; }
                set { SetField(ref _hauntPct, value); }
            }

            private static readonly SkillSettings DefaultSpiritWalkSettings = new SkillSettings
            {
                UseMode = UseTime.Default,
            };

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                SpiritWalk = DefaultSpiritWalkSettings.Clone();
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


