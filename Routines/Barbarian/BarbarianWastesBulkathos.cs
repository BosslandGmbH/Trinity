using Trinity.Framework.Helpers;
using System.Linq;
using Trinity.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Barbarian
{
    public sealed class BarbarianWastesBulkathos : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Wastes Set + BulKathos Whirlwind";
        public string Description => "Build that spins around a lot.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/barbarian-whirlwind-build-with-bul-kathoss-oath-and-wrath-of-the-wastes-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.WrathOfTheWastes, SetBonus.Third },
                { Sets.BulKathossOath, SetBonus.First },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Barbarian.Whirlwind, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power) && (power.SNOPower != SNOPower.Barbarian_GroundStomp || Skills.Barbarian.IgnorePain.CanCast()))
                return power;

            // Zoom around
            if (Skills.Barbarian.Whirlwind.CanCast())
            {
                var destination = Player.CurrentHealthPct > 0.6f
                    ? TargetUtil.GetZigZagTarget(CurrentTarget.Position, 60f, true)
                    : Avoider.SafeSpot;

                return Whirlwind(destination);
            }

            return null;
        }

        protected override bool ShouldGroundStomp(out Vector3 position)
        {
            return base.ShouldGroundStomp(out position) && Skills.Barbarian.IgnorePain.CanCast();
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        protected override bool ShouldWrathOfTheBerserker()
        {
            if (!AllowedToUse(Settings.WrathOfTheBerserker, Skills.Barbarian.WrathOfTheBerserker))
                return false;

            return IsInCombat && base.ShouldWrathOfTheBerserker();
        }

        public TrinityPower GetBuffPower()
        {
            // Cast on CD for fury generation.
            if (ZetaDia.Me.IsInCombat && Skills.Barbarian.ThreateningShout.CanCast())
                return ThreateningShout();

            return DefaultBuffPower();
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            var destinationPortal =
                Core.Actors.FirstOrDefault(g => g.IsPortal && g.Position.DistanceSqr(destination) < 5 * 5);

            if (destinationPortal != null)
                return Walk(destination);

            // All walking is replaced by Whirlwind if possible.

            if (Skills.Barbarian.Whirlwind.CanCast() && !Player.IsInTown)
            {
                if (CurrentTarget == null || !(CurrentTarget.IsGizmo && CurrentTarget.RadiusDistance <= 1f))
                    return Whirlwind(destination);
            }

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public BarbarianWastesBulkathosSettings Settings { get; } = new BarbarianWastesBulkathosSettings();

        public sealed class BarbarianWastesBulkathosSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;
            private SkillSettings _wrathOfTheBerserker;

            [DefaultValue(6)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            public SkillSettings WrathOfTheBerserker
            {
                get { return _wrathOfTheBerserker; }
                set { SetField(ref _wrathOfTheBerserker, value); }
            }

            private static readonly SkillSettings WrathOfTheBerserkerDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            };

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                WrathOfTheBerserker = WrathOfTheBerserkerDefaults.Clone();
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
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }


        #endregion
    }
}


