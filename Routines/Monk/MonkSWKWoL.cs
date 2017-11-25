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

namespace Trinity.Routines.Monk
{
    public sealed class MonkSWKWol : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "DatModz's WK WoL Monk";
        public string Description => "DatModz - GR 110+ Sunwuko WoL Monk: This is a well rounded Solo Pushing Build that works well at high levle g-rifts";
        public string Author => "jubisman";
        public string Version => "0.2";
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

        public TrinityPower GetOffensivePower()
        {
            Vector3 position;
            TrinityActor target;
            TrinityPower power = null;

            if (ShouldDashingStrike(out position))
                return DashingStrike(position);

            if (ShouldWaveOfLight(out target))
                return WaveOfLight(target);

            return GetPrimary();
        }


        private TrinityPower GetPrimary()
        {
            TrinityPower power = null;

            var target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;

            if (IsNoPrimary)
                power = Walk(TargetUtil.GetSafeSpotPosition(20f));

            if (Skills.Monk.FistsOfThunder.CanCast())
                power = FistsOfThunder(target);

            else if (Skills.Monk.DeadlyReach.CanCast())
                power = DeadlyReach(target);

            else if (Skills.Monk.CripplingWave.CanCast())
                power = CripplingWave(target);

            else if (Skills.Monk.WayOfTheHundredFists.CanCast())
                power = WayOfTheHundredFists(target);

            return power;
        }

        protected override bool ShouldWaveOfLight(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.WaveOfLight.CanCast())
                return false;

            if (TargetUtil.AnyMobsInRange(50f))
            {
                target = TargetUtil.GetBestClusterUnit(50f);
                return target != null;
            }

            return false;
        }

        protected override bool ShouldSweepingWind()
        {
            if (!Skills.Monk.SweepingWind.CanCast())
            {
                //Core.Logger.Log("CanCast is false for Sweeping Wind");
                return false;
            }

            /*var buffCooldownRemanining = Core.Cooldowns.GetBuffCooldownRemaining(SNOPower.Monk_SweepingWind);
            if (buffCooldownRemanining.TotalMilliseconds > 750)
                return false;*/

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

        public TrinityPower GetBuffPower()
        {
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

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike) && CanDashTo(destination))
                return DashingStrike(destination);

            return Walk(destination);
        }

        protected override bool ShouldDashingStrike(out Vector3 position)
        {
            position = Vector3.Zero;

            var skill = Skills.Monk.DashingStrike;
            if (skill.TimeSinceUse < 3000 && skill.Charges < MaxDashingStrikeCharges)
                return false;

            if (!Skills.Monk.DashingStrike.CanCast())
                return false;

            if (Skills.Monk.DashingStrike.TimeSinceUse < 750)
                return false;

            if (!AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike))
                return false;

            if (Player.CurrentHealthPct < 0.7f && Runes.Monk.BlindingSpeed.IsActive ||
                Core.Buffs.HasCastingShrine)
            {
                position = TargetUtil.GetSafeSpotPosition(40f);
                return position != Vector3.Zero;
            }

            if (Player.CurrentHealthPct >= 0.7f)
            {
                Vector3 bestBuffedPosition;
                var bestClusterPoint = TargetUtil.GetBestClusterPoint(20f);
                var safePosition = TargetUtil.GetSafeSpotPosition(40f);
                TargetUtil.BestBuffPosition(40f, bestClusterPoint, true, out bestBuffedPosition);
                var bestDashPosition = bestBuffedPosition != Vector3.Zero ? bestBuffedPosition : safePosition;
                return bestDashPosition != Vector3.Zero;
            }
            return false;
        }

        protected override bool ShouldEpiphany()
        {
            if (!AllowedToUse(Settings.Epiphany, Skills.Monk.Epiphany))
                return false;

            if (Player.IsInTown)
                return false;

            return base.ShouldEpiphany();
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