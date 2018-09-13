using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Barbarian
{
    public sealed class BarbarianEarthLeap : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Barbarian Earth LeapQuake";
        public string Description => "This routine uses furious charge to move around and attack, building fury for Ancient Spear.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/barbarian-build-leap-earthquake-with-might-of-the-earth-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.MightOfTheEarth, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Barbarian.Earthquake, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;
            Vector3 position;
 
            if (ShouldLeap(out position))
                return Leap(position);

            // Siesemic Slam variant - Make sure damage buff is up before earthquake
            if (Legendary.GirdleOfGiants.IsEquipped && (Skills.Barbarian.Earthquake.CanCast() || Skills.Barbarian.Leap.IsActive) && !IsGirdleOfGiantsBuffActive)
                return SeismicSlam(TargetUtil.GetBestClusterUnit(30f) ?? CurrentTarget);

            if (Skills.Barbarian.Avalanche.CanCast())
                return Avalanche(Player.Position);

            // Use cave in rune to group monsters before spear.
            if (Skills.Barbarian.Earthquake.CanCast())
                return Earthquake(Player.Position);

            // Spend resource to reduce leap cooldown
            if (ShouldAncientSpear(out target))
                return AncientSpear(target);

            // Fallback to defaults in case of minor variations.
            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;
            
            return Walk(Avoider.SafeSpot);
        }
     
        protected override bool ShouldLeap(out Vector3 position)
        {
            // Always Leap

            position = Vector3.Zero;
            if (!Skills.Barbarian.Leap.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected override bool ShouldAncientSpear(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.AncientSpear.CanCast())
                return false;

            target = TargetUtil.BestAoeUnit(60, true).IsInLineOfSight
                ? TargetUtil.BestAoeUnit(60, true)
                : TargetUtil.GetBestClusterUnit(10, 60, false, true, false, true);

            // wait for 95% resource would cause max damage on boulder toss
            // but the cooldown reset time for leap is possibly more important?
            //return target.Distance <= 60 && Player.PrimaryResourcePct > 0.95;

            return target?.Distance <= 60;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetBuffPower()
        {
            if (!Player.IsInTown)
            {
                if (Skills.Barbarian.ThreateningShout.CanCast() && TargetUtil.GetBestClusterUnit()?.Distance < 15f)
                    return ThreateningShout(Player.Position);

                if (Skills.Barbarian.WarCry.CanCast())
                    return WarCry();
            }

            return DefaultBuffPower();
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            // Leap out of combat for the defensive bonuses.

            if (Skills.Barbarian.Leap.CanCast() && (!Player.HasBuff(SNOPower.Barbarian_Leap) || Player.CurrentHealthPct > 0.35f) && !Player.IsInTown && HostileMonsters.Any(u => u.Distance < 20f))
                return Leap(destination);

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public BarbarianEarthLeapSettings Settings { get; } = new BarbarianEarthLeapSettings();

        public sealed class BarbarianEarthLeapSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(8)]
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


