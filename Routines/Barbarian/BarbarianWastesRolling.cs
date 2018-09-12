using Trinity.Framework.Helpers;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;


namespace Trinity.Routines.Barbarian
{
    public sealed class BarbarianWastesRolling : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Wastes / BulKathos 'Rolling' Whirlwind";
        public string Description => "Build that spins around a lot.";
        public string Author => "toxin";
        public string Version => "0.1";
        public string Url => "http://www.diablofans.com/builds/84104-the-rolling-barb-gr90";

        public Build BuildRequirements => null;

        //public Build BuildRequirements => new Build
        //{
        //    Sets = new Dictionary<Set, SetBonus>
        //    {
        //        { Sets.WrathOfTheWastes, SetBonus.Third },
        //        { Sets.BulKathossOath, SetBonus.First },
        //    },
        //    Skills = new Dictionary<Skill, Rune>
        //    {
        //        { Skills.Barbarian.Whirlwind, null },
        //    },
        //};

        #endregion

        public TrinityPower GetOffensivePower()
        {
            if (TrySpecialPower(out var power))
                return power;

            if (TrySecondaryPower(out power))
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

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        protected override bool ShouldWrathOfTheBerserker()
        {
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
        public BarbarianWastesRollingSettings Settings { get; } = new BarbarianWastesRollingSettings();

        public sealed class BarbarianWastesRollingSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(6)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get => _emergencyHealthPct;
                set => SetField(ref _emergencyHealthPct, value);
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


