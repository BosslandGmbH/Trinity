using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Routines.DemonHunter;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Monk
{
    public sealed class MonkRaimentShenlongs : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Monk Raiment Shenlogs";
        public string Description => "Build for the thousand storms set used with shenlongs fist weapons.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/monk-shenlong-raiment-build-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.ThousandStorms, SetBonus.Third },
                { Sets.ShenlongsSpirit, SetBonus.First },
            },
        };

        public override Func<bool> ShouldIgnoreKiting => ShouldBeastMode;
        public override Func<bool> ShouldIgnoreAvoidance => ShouldBeastMode;
        public override Func<bool> ShouldIgnorePackSize => ShouldBeastMode;
        public override Func<bool> ShouldIgnoreNonUnits => ShouldBeastMode;

        #endregion

        private bool ShouldBeastMode()
        {
            // Maximize time spent attacking while the shenlongs damage buff is active.

            if(!HasShenLongBuff)
                return false;

            if(Player.CurrentHealthPct < 0.4f)
                return false;

            if (CurrentTarget?.Type == TrinityObjectType.ProgressionGlobe || CurrentTarget?.Type == TrinityObjectType.HealthGlobe)
                return false;

            if (Core.Avoidance.InCriticalAvoidance(Player.Position))
                return false;

            return true;
        }

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            Vector3 position;

            if (ShouldDashingStrike(out position))
            {
                Logger.Log(LogCategory.Routine, $"In Combat dash");
                return DashingStrike(position);
            }

            if (ShouldCycloneStrike())
                CycloneStrike();

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        public TrinityPower GetBuffPower()
        {
            if (ShouldSweepingWind())
                return SweepingWind();

            if (ShouldMantraOfConviction())
                return MantraOfConviction();

            if (ShouldMantraOfHealing())
                return MantraOfConviction();

            if (ShouldMantraOfSalvation())
                return MantraOfSalvation();

            if (ShouldEpiphany())
                return Epiphany();

            if (ShouldMysticAlly())
                return MysticAlly();

            if (ShouldBreathOfHeaven())
                return BreathOfHeaven();

            if (ShouldSerenity())
                return Serenity();

            if (ShouldBlindingFlash())
                return BlindingFlash();

            if (ShouldInnerSanctuary())
                return InnerSanctuary();

            return null;
        }

        public bool ShouldConserveSpirit 
            => HasShenLongBuff && Player.CurrentHealthPct > 0.6
            && (Player.PrimaryResourcePct < 0.35 || !HasShenLongBuff && Player.PrimaryResourcePct > 0.80);

        protected override bool ShouldDashingStrike(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Monk.DashingStrike.CanCast())
                return false;

            var charges = Skills.Monk.DashingStrike.Charges;

            if (ShouldConserveSpirit)
                return false;

            if (HasRaimentDashBuff)
            {                
                position = CurrentTarget.Position;

                // We need to keep attacking to generate spirit to keep shenlongs buff, 
                // so close distance to target rather than walk. Doesn't apply if eiphany can teleprt us there.
                if (!Skills.Monk.Epiphany.IsBuffActive && CurrentTarget.Distance > 15f && Player.PrimaryResourcePct > 0.65)
                    return true;

                // Otherwise just dash to refresh the RaimentDashBuff
                var buffRefreshTime = Runes.Monk.BlindingSpeed.IsActive || Runes.Monk.Radiance.IsActive ? 3500 : 5500;
                return Skills.Monk.DashingStrike.TimeSinceUse > buffRefreshTime && TargetUtil.PierceHitsMonster(position);
            }

            // Need to get the damge buff up asap. It only procs if we hit something.
            var target = TargetUtil.GetBestClusterUnit(55f) ?? CurrentTarget;
            if (target != null && target.Distance < 55f && TargetUtil.PierceHitsMonster(position))
            {
                position = target.Position;
                return true;
            }

            // Don't dash if it would cost spirit
            if (Player.PrimaryResource > 75 || charges != MaxDashingStrikeCharges)
                return false;

            return position != Vector3.Zero;
        }

        protected override bool ShouldEpiphany()
        {
            return IsInCombat && Skills.Monk.Epiphany.CanCast();
        }

        protected override bool ShouldBreathOfHeaven()
        {
            if (!Skills.Monk.BreathOfHeaven.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.Monk_BreathOfHeaven))
                return false;

            if (!TargetUtil.AnyMobsInRange(20))
                return false;

            var needSpiritBuff = Runes.Monk.InfusedWithLight.IsActive && (Player.PrimaryResourcePct < 0.5f || !HasShenLongBuff);
            var needDamage = Runes.Monk.BlazingWrath.IsActive && TargetUtil.AnyMobsInRange(20);
            var needHealing = Player.CurrentHealthPct <= 0.6f;

            return needSpiritBuff || needHealing || needDamage;
        }

        protected override bool ShouldCycloneStrike()
        {
            var skill = Skills.Monk.CycloneStrike;
            if (!skill.CanCast())
                return false;
            
            if (Legendary.LefebvresSoliloquy.IsEquipped)
            {
                var shouldRefreshBuff = skill.TimeSinceUse > 4500 || !skill.IsBuffActive;
                return shouldRefreshBuff && TargetUtil.AnyMobsInRange(CycloneStrikeRange);
            }

            var refereshTime = skill.DistanceFromLastUsePosition < 20f ? 4000 : 2000;
            if (skill.TimeSinceUse < refereshTime)
                return false;

            // Don't spend resource while the buff is close to being lost or gained
            if (ShouldConserveSpirit && Player.CurrentHealthPct > 0.5f)
                return false;

            var targetIsCloseElite = CurrentTarget.IsElite && CurrentTarget.Distance < CycloneStrikeRange;
            var plentyOfTargetsToPull = TargetUtil.IsPercentUnitsWithinBand(15f, CycloneStrikeRange, 0.25);
            return targetIsCloseElite || plentyOfTargetsToPull;
        }

        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            var enoughTimeDelay = Skills.Monk.DashingStrike.TimeSinceUse > 750;
            var charges = Skills.Monk.DashingStrike.Charges;           

            // Only dash when it won't cost spirit.
            var isResourceEfficient = charges == MaxDashingStrikeCharges && Player.PrimaryResource < 75;
            var followingLeader = !IsInCombat && IsBlocked && MyPartyObjective == PartyObjective.FollowLeader;

            if (CanDashTo(destination) && (isResourceEfficient || followingLeader) && !ShouldConserveSpirit && enoughTimeDelay)
            {
                Logger.Log(LogCategory.Routine, $"Movement Dash {destination} Dist: {destination.Distance(Core.Player.Position)}");
                return DashingStrike(destination);
            }

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MonkRaimentShenlongsSettings Settings { get; } = new MonkRaimentShenlongsSettings();

        public sealed class MonkRaimentShenlongsSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(6)]
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

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => base.LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}

