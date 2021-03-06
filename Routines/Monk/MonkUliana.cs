﻿using Trinity.Framework.Helpers;
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


namespace Trinity.Routines.Monk
{
    public sealed class MonkUliana : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Monk Uliana Routine";
        public string Description => "Uliana's Strategem set puts one of the coolest looking Monk skills — Seven-Sided Strike Seven-Sided Strike — in the Greater Rifting spotlight, along with the helpful push of the class trademark, Exploding Palm Exploding Palm. This high burst, AoE centric playstyle is available in both solo progression and speedfarming";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/monk-ulianas-strategem-seven-sided-strike-build-patch-2-4-2-season-7";

        public override int PrimaryEnergyReserve => 100;

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.UlianasStratagem, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Monk.SevenSidedStrike, null },
                { Skills.Monk.ExplodingPalm, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            if (ShouldDashingStrike(out var position))
                return DashingStrike(position);

            if (ShouldCycloneStrike())
                return CycloneStrike();

            if (ShouldSevenSidedStrike(out var target))
                return SevenSidedStrike(target);

            if (!WeightedUnits.Any(u => u.Distance < 20f && HasEP(u)))
                return GetExplodingPalmPrimary();

            return GetPrimary();
        }

        private TrinityPower GetExplodingPalmPrimary()
        {
            TrinityPower power = null;

            var target = TargetUtil.LowestHealthTarget(6f, CurrentTarget.Position, SNOPower.Monk_ExplodingPalm);
            var targetTeleport = TargetUtil.LowestHealthTarget(20f, CurrentTarget.Position, SNOPower.Monk_ExplodingPalm);

            if (IsEpiphanyActive)
                target = targetTeleport;

            if (Skills.Monk.FistsOfThunder.CanCast())
                power = FistsOfThunder(targetTeleport);

            else if (Skills.Monk.DeadlyReach.CanCast())
                power = DeadlyReach(target);

            else if (Skills.Monk.CripplingWave.CanCast())
                power = CripplingWave(target);

            else if (Skills.Monk.WayOfTheHundredFists.CanCast())
                power = WayOfTheHundredFists(target);

            return power;
        }

        private TrinityPower GetPrimary()
        {
            TrinityPower power = null;

            var target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;

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

        // There is currently a bug with Attributes sometimes not showing up in Trinity's attributes list.
        // Needs to be looked into, this is a work-around for now (or use ZetaDia lookup) 
        public bool HasEP(TrinityActor actor) 
            => actor.Attributes.Powers.ContainsKey(SNOPower.Monk_ExplodingPalm) || 
            actor.Attributes.GetAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectB, (int)SNOPower.Monk_ExplodingPalm);

        protected override bool ShouldSevenSidedStrike(out TrinityActor target)
        {
            target = null;
            var skill = Skills.Monk.SevenSidedStrike;
            var nearbyUnitsWithEP = WeightedUnits.Any(u => u.Distance < 20f && HasEP(u));
            //var nearbyUnitsWithGungdoEP = CurrentTarget.HasDebuff((SNOPower)455436);

            if (!skill.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(45f) && !CurrentTarget.IsTreasureGoblin)
                return false;

            if (!nearbyUnitsWithEP && !Legendary.Madstone.IsEquipped)
                return false;

            //if (!nearbyUnitsWithGungdoEP)
            //    return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        public double SevenSidedStrikeCooldownMs
        {
            get
            {
                var baseCooldown = Runes.Monk.SustainedAttack.IsActive ? 14 : 30;
                var multiplier = Legendary.TheFlowOfEternity.IsEquipped ? 0.6 : 1;
                return baseCooldown * multiplier * (1 - Player.CooldownReductionPct) * 1000;
            }
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
                DashingStrike(destination);

            return Walk(destination);
        }

        protected override bool ShouldDashingStrike(out Vector3 position)
        {
            position = Vector3.Zero;

            var skill = Skills.Monk.DashingStrike;
            if (skill.TimeSinceUse < 3000 && skill.Charges < MaxDashingStrikeCharges)
                return false;

            if (!AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike))
                return false;

            return base.ShouldDashingStrike(out position);
        }

        protected override bool ShouldEpiphany()
        {
            if (!AllowedToUse(Settings.Epiphany, Skills.Monk.Epiphany))
                return false;

            return base.ShouldEpiphany();
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MonkUlianaSettings Settings { get; } = new MonkUlianaSettings();

        public sealed class MonkUlianaSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _epiphany;
            private SkillSettings _dashingStrike;
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

            public SkillSettings Epiphany
            {
                get => _epiphany;
                set => SetField(ref _epiphany, value);
            }

            public SkillSettings DashingStrike
            {
                get => _dashingStrike;
                set => SetField(ref _dashingStrike, value);
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

