using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using System.Linq;
using Trinity.Framework.Avoidance.Structures;

namespace Trinity.Routines.Monk
{
    public sealed class MonkSWKWol : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "DatModz's WK WoL Monk";
        public string Description => "DatModz - GR 110+ Sunwuko WoL Monk: This is a well rounded Solo Pushing Build that works well at high level g-rifts";
        public string Author => "jubisman";
        public string Version => "0.7.1";
        public string Url => "http://www.diablofans.com/builds/96442-datmodz-gr-110-sunwuko-wol-monk";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.MonkeyKingsGarb, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Monk.WaveOfLight, null },
                { Skills.Monk.SweepingWind, null },
            }
        };

        #endregion

        public TrinityPower GetBuffPower()
        {
            var closeUnit = HostileMonsters.FirstOrDefault(u => u.Distance < 40f);
            if (ShouldRefreshSpiritGuardsBuff && closeUnit != null && !Player.IsInTown)
            {
                //Core.Logger.Log(LogCategory.Routine, "Need Spirit Gaurds Buff");

                if (Skills.Monk.FistsOfThunder.IsActive)
                    return new TrinityPower(Skills.Monk.FistsOfThunder, 3f);

                else if (Skills.Monk.DeadlyReach.IsActive)
                    return new TrinityPower(Skills.Monk.DeadlyReach, 3f);

                else if (Skills.Monk.CripplingWave.IsActive)
                    return new TrinityPower(Skills.Monk.CripplingWave, 3f);

                else if (Skills.Monk.WayOfTheHundredFists.IsActive)
                    return new TrinityPower(Skills.Monk.WayOfTheHundredFists, 3f);
            }

            if (ShouldSweepingWind())
                return SweepingWind();

            if (ShouldMantraOfConviction())
                return MantraOfConviction();

            if (ShouldMantraOfHealing())
                return MantraOfConviction();

            if (ShouldMantraOfRetribution())
                return MantraOfRetribution();

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

        public TrinityPower GetOffensivePower()
        {
            Vector3 position;
            TrinityActor target;
            TrinityPower power;

            if (ShouldWalkToTarget(out target))
                return Walk(target);

            if (ShouldDashingStrike(out position))
                return DashingStrike(position);

            if (ShouldWaveOfLight(out target))
                return WaveOfLight(target);

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15f, 40f, Player.Position,
                node => !TargetUtil.AnyMobsInRangeOfPosition(node.NavigableCenter));
            return Walk(position);
        }

        private static bool ShouldWalkToTarget(out TrinityActor target)
        {
            target = null;

            if (CurrentTarget.Distance > 60f)
            {
                target = CurrentTarget;
                return target != null;
            }

            return false;
        }

        protected override bool ShouldDashingStrike(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Monk.DashingStrike.CanCast())
                return false;

            if (Skills.Monk.DashingStrike.TimeSinceUse < 750)
                return false;

            if (!AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike))
                return false;

            // Dont move from outside avoidance into avoidance.
            if (!Core.Avoidance.InAvoidance(Player.Position) && Core.Avoidance.Grid.IsLocationInFlags(position, AvoidanceFlags.Avoidance))
                return false;

            // Try to dash to Occulus AoE whenever possible
            Vector3 bestBuffedPosition;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();

            if (TargetUtil.BestBuffPosition(60f, bestClusterPoint, false, out bestBuffedPosition) &&
                Player.Position.Distance2D(bestBuffedPosition) > 10f && bestBuffedPosition != Vector3.Zero)
            {
                Core.Logger.Log($"Found buff position - distance: {Player.Position.Distance(bestBuffedPosition)} ({bestBuffedPosition})");
                position = bestBuffedPosition;

                return position != Vector3.Zero;
            }

            // Find a safespot with no monsters within kite range.
            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15f, 60f, Player.Position,
                node => !TargetUtil.AnyMobsInRangeOfPosition(node.NavigableCenter));

            return true;
        }

        protected override bool ShouldWaveOfLight(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.WaveOfLight.CanCast())
                return false;

            if (TargetUtil.AnyMobsInRange(60f))
            {
                target = TargetUtil.GetBestClusterUnit();
                return target != null;
            }

            return false;
        }

        protected override bool ShouldSweepingWind()
        {
            if (!Skills.Monk.SweepingWind.CanCast())
                return false;

            if (Skills.Monk.SweepingWind.TimeSinceUse < 5250)
                return false;

            if (Player.IsInTown)
                return false;

            return true;
        }

        protected override bool ShouldMysticAlly()
        {
            if (!Skills.Monk.MysticAlly.CanCast())
                return false;

            if (Runes.Monk.AirAlly.IsActive && Player.PrimaryResource > 150)
                return false;

            if (!TargetUtil.AnyMobsInRange(40f))
                return false;

            return true;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike) && CanDashTo(destination))
                return DashingStrike(destination);

            return Walk(destination);
        }

        protected override bool ShouldEpiphany()
        {
            if (!Skills.Monk.Epiphany.CanCast())
                return false;

            if (HasInstantCooldowns && !Skills.Monk.Epiphany.IsLastUsed)
                return true;

            if (!AllowedToUse(Settings.Epiphany, Skills.Monk.Epiphany))
                return false;

            if (!TargetUtil.AnyMobsInRange(60f))
                return false;

            return true;
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MonkSWKWoLSettings Settings { get; } = new MonkSWKWoLSettings();

        public sealed class MonkSWKWoLSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _epiphany;
            private SkillSettings _dashingStrike;
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

            public SkillSettings Epiphany
            {
                get { return _epiphany; }
                set { SetField(ref _epiphany, value); }
            }

            public SkillSettings DashingStrike
            {
                get { return _dashingStrike; }
                set { SetField(ref _dashingStrike, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings EpiphanyDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            };

            private static readonly SkillSettings DashingStrikeDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 3500,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Epiphany = EpiphanyDefaults.Clone();
                DashingStrike = DashingStrikeDefaults.Clone();
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