using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Monk
{
    public sealed class MonkZDPS : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Monk Istvan's - ZDPS Heal Monk";
        public string Description => "Support build that uses high Attack Speed and Cooldown Reduction to keep mobs grouped together while helping your party members stay alive!";
        public string Author => "TwoCigars, xzjv, Phelon";
        public string Version => "0.7";
        public string Url => "http://www.icy-veins.com/d3/monk-inna-support-build-patch-2-4-3-season-9";
        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.Innas, SetBonus.Second },
                { Sets.IstvansPairedBlades, SetBonus.First }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Monk.InnerSanctuary, null },
                { Skills.Monk.CripplingWave, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;

            if (TryMantra(out power))
                return power;

            if (ShouldEpiphany())
                return Epiphany();

            if (ShouldInnerSanctuary())
                return InnerSanctuary();

            if (ShouldCycloneStrike())
                return CycloneStrike();

            if (ShouldBlindingFlash())
                return BlindingFlash();

            if (TryCripplingWave(out power))
                return power;

            return null;
        }



        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetBuffPower()
        {
            return null;
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            return Walk(destination);
        }



        //Overriding PrimaryEnergyReserve [50] to the Spirit cost of Cyclone Strike based on rune used.
        public override int PrimaryEnergyReserve => (int) CycloneStrikeSpirit;

        //private static bool ShouldWalk(float minRange, float maxRange, bool objectsInAoe)
        //{
        //    return TargetUtil.BestTankPosition(maxRange, objectsInAoe).Distance(Player.Position) < maxRange &&
        //           TargetUtil.BestTankPosition(maxRange, objectsInAoe).Distance(Player.Position) > minRange;
        //}

        public bool TryCripplingWave(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldCripplingWave(out target))
                power = CripplingWave(target);

            return power != null;
        }

        protected override bool ShouldCripplingWave(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.CripplingWave.CanCast())
                return false;

            target = TargetUtil.BestEliteInRange(45, true);
            if (target != null && target.Distance < 12)
                return true;

            target = TargetUtil.BestAoeUnit(45, true);
            if (target != null && target.Distance < 12) //removed resource check here
                return true;

            target = TargetUtil.BestAuraUnit(SNOPower.Monk_CripplingWave, 12f, true);
            if (target != null && target.Distance < 12)
                return true;

            target = TargetUtil.GetClosestUnit(12f);
            return target != null;
        }

        protected override bool ShouldInnerSanctuary()
        {
            if (!Skills.Monk.InnerSanctuary.CanCast())
                return false;

            if (Skills.Monk.InnerSanctuary.TimeSinceUse < Settings.InnerSanctuaryDelay)
                return false;

            if (TargetUtil.BestAoeUnit(45, true).Distance > Settings.InnerSanctuaryMinRange)
                return false;

            return true;
        }

        protected override bool ShouldCycloneStrike()
        {
            if (!Skills.Monk.CycloneStrike.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (Skills.Monk.CycloneStrike.TimeSinceUse < Settings.CycloneStrikeDelay)
                return false;

            var targetIsCloseElite = CurrentTarget.IsElite && CurrentTarget.Distance < CycloneStrikeRange;      //Checks for elites first
            var plentyOfTargetsToPull = TargetUtil.IsPercentUnitsWithinBand(15f, CycloneStrikeRange, 0.25);     //Checks percentage within band
            var anyTargetsInRange = TargetUtil.AnyMobsInRange(CycloneStrikeRange, Settings.CycloneStrikeMinMobs);   //Checks minimum mobs in range

            return targetIsCloseElite || plentyOfTargetsToPull || anyTargetsInRange;
        }

        protected override bool ShouldBlindingFlash()
        {
            if (!Skills.Monk.BlindingFlash.CanCast())
                return false;

            if (HasInstantCooldowns && Skills.Monk.BlindingFlash.TimeSinceUse > 2500)   //Overlap them by half a second
                return true;

            var lowHealth = Player.CurrentHealthPct <= Settings.EmergencyHealthPct;
            var enoughStuffToBlind = TargetUtil.AnyElitesInRange(20, 1) || TargetUtil.AnyMobsInRange(20, 3);
            var blindCurrentTarget = CurrentTarget != null && CurrentTarget.IsElite && CurrentTarget.RadiusDistance <= 15f;

            return lowHealth || enoughStuffToBlind || blindCurrentTarget;
        }

        protected override bool ShouldMantraOfHealing()
        {
            if (!Skills.Monk.MantraOfHealing.CanCast())
                return false;

            if (Skills.Monk.MantraOfHealing.TimeSinceUse < Settings.MantraDelay)
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            return true;
        }

        protected override bool ShouldMantraOfSalvation()
        {
            if (!Skills.Monk.MantraOfSalvation.CanCast())
                return false;

            if (Skills.Monk.MantraOfSalvation.TimeSinceUse < Settings.MantraDelay)
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            return true;
        }

        protected override bool ShouldMantraOfRetribution()
        {
            if (!Skills.Monk.MantraOfRetribution.CanCast())
                return false;

            if (Skills.Monk.MantraOfRetribution.TimeSinceUse < Settings.MantraDelay)
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            return true;
        }

        protected override bool ShouldMantraOfConviction()
        {
            if (!Skills.Monk.MantraOfConviction.CanCast())
                return false;

            if (Skills.Monk.MantraOfConviction.TimeSinceUse < Settings.MantraDelay)
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (CurrentTarget != null && CurrentTarget.IsElite)
                return true;

            if (TargetUtil.ClusterExists(15f, 30f, 3))
                return true;

            return false;
        }

        protected override bool ShouldEpiphany()
        {
            if (!AllowedToUse(Settings.Epiphany, Skills.Monk.Epiphany))
                return false;

            if (!Skills.Monk.Epiphany.CanCast())
                return false;

            if (HasInstantCooldowns && !Skills.Monk.Epiphany.IsLastUsed)
                return true;

            if (TargetUtil.BestAoeUnit(45, true).Distance > 20)
                return false;

            return true;
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MonkZDPSSettings Settings { get; } = new MonkZDPSSettings();

        public sealed class MonkZDPSSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _epiphany;
            private float _cycloneStrikeDelay;
            private int _cycloneStrikeMinMobs;
            private float _innerSanctuaryDelay;
            private int _innerSanctuaryMinRange;
            private float _mantraDelay;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(1)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(2000)]
            public float CycloneStrikeDelay
            {
                get => _cycloneStrikeDelay;
                set => SetField(ref _cycloneStrikeDelay, value);
            }

            [DefaultValue(3)]
            public int CycloneStrikeMinMobs
            {
                get => _cycloneStrikeMinMobs;
                set => SetField(ref _cycloneStrikeMinMobs, value);
            }

            [DefaultValue(5000)]
            public float InnerSanctuaryDelay
            {
                get => _innerSanctuaryDelay;
                set => SetField(ref _innerSanctuaryDelay, value);
            }

            [DefaultValue(15)]
            public int InnerSanctuaryMinRange
            {
                get => _innerSanctuaryMinRange;
                set => SetField(ref _innerSanctuaryMinRange, value);
            }

            [DefaultValue(3000)]
            public float MantraDelay
            {
                get => _mantraDelay;
                set => SetField(ref _mantraDelay, value);
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


            #region Skill Defaults

            private static readonly SkillSettings EpiphanyDefaults = new SkillSettings
            {
                UseMode = UseTime.Always,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Epiphany = EpiphanyDefaults.Clone();
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

