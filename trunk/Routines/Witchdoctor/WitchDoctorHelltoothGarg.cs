using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Settings.Combat;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Witchdoctor
{
    public sealed class WitchDoctorHelltoothGarg : WitchDoctorBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Helltooth Gargantuan";
        public string Description => "Pet based routine using the Helltooth set and gargantuans";
        public string Author => "phelon, xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/witch-doctor-gargantuan-build-with-helltooth-patch-2-4-2-season-7";


        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.HelltoothHarness, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.WitchDoctor.Gargantuan, null },
            },
            Items = new List<Item>
            {
                Legendary.TheShortMansFinger
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            // Ported from Phelon's Cold Garg routine

            TrinityPower power;
            TrinityActor target;

            var bestDpsTarget = TargetUtil.BestAoeUnit(35f, true);
            Vector3 bestDpsPosition;

            if (Core.Player.HasBuff(SNOPower.Witchdoctor_SpiritWalk))
            {
                if (Player.CurrentHealthPct < EmergencyHealthPct && TargetUtil.ClosestGlobe(35f, true) != null)
                    return Walk(TargetUtil.ClosestGlobe(35f, true).Position);

                if (TargetUtil.BestBuffPosition(35f, bestDpsTarget.Position, false, out bestDpsPosition) && bestDpsPosition.Distance2D(Player.Position) > 6f)
                    return Walk(bestDpsPosition);
            }

            if (TargetUtil.BestBuffPosition(12f, bestDpsTarget.Position, false, out bestDpsPosition) &&
                (TargetUtil.UnitsBetweenLocations(Player.Position, bestDpsPosition).Count < 6 || Legendary.IllusoryBoots.IsEquipped) &&
                bestDpsPosition.Distance2D(Player.Position) > 6f)
                return Walk(bestDpsPosition);

            if (TrySpecialPower(out power))
                return power;

            if (ShouldPiranhas(out target))
                return Piranhas(target);

            if (ShouldWallOfDeath(out target))
                return WallOfDeath(target);

            if (ShouldHaunt(out target))
                return Haunt(target);

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            // Stand still for damage buff - defualt Avoider.SafeSpot includes logic 
            // to suppress minor variations in safespot position if less than 12f
            if (Player.CurrentHealthPct > 0.8f && !TargetUtil.AnyMobsInRange(15f))
                return Walk(Avoider.SafeSpot);                

            return Walk(TargetUtil.GetLoiterPosition(bestDpsTarget, 25f));
        }

        #region Conditions

        protected override bool ShouldPiranhas(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.Piranhas.CanCast())
                return false;

            target = Runes.WitchDoctor.WaveOfMutilation.IsActive
                ? TargetUtil.GetBestPierceTarget(25, true)
                : TargetUtil.BestAoeUnit(35, true);

            return target != null;
        }

        protected override bool ShouldWallOfDeath(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.WallOfDeath.CanCast())
                return false;

            target = TargetUtil.BestAoeUnit(35, true);

            return target != null;
        }


        protected override bool ShouldHaunt(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.Haunt.CanCast())
                return false;

            if (SpellHistory.LastPowerUsed == Skills.WitchDoctor.Haunt.SNOPower)
                return false;

            target = TargetUtil.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 35, true) ?? CurrentTarget;

            return target != null;
        }

        #endregion

        #region Expressions

        protected override TrinityPower Piranhas(TrinityActor target)
            => new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);

        protected override TrinityPower WallOfDeath(TrinityActor target)
            => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 45, target.Position);

        protected override TrinityPower Haunt(TrinityActor target)
            => new TrinityPower(SNOPower.Witchdoctor_Haunt, 35f, target.AcdId);

        #endregion

        public TrinityPower GetBuffPower() => DefaultBuffPower();
        public TrinityPower GetDefensivePower() => null;
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();
        public TrinityPower GetMovementPower(Vector3 destination) => Walk(destination);

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public override KiteMode KiteMode => KiteMode.Never;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WitchDoctorHelltoothGargSettings Settings { get; } = new WitchDoctorHelltoothGargSettings();

        public sealed class WitchDoctorHelltoothGargSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;

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

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


