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
            TrinityActor target;
            TrinityPower power = null;
            Vector3 position;

            if (ShouldDashingStrike(out position))
                return DashingStrike(position);

            if (ShouldCycloneStrike())
                return CycloneStrike();

            if (Player.CurrentHealthPct < 0.35)
                return SevenSidedStrike(CurrentTarget);

            var closeEPUnits = WeightedUnits.Where(u => u.Distance < 15f && u.HasDebuff(SNOPower.Monk_ExplodingPalm)).ToList();
            var isEPReady = Legendary.Madstone.IsEquipped || WeightedUnits.Any(u => u.Distance < 15f && u.HasDebuff(SNOPower.Monk_ExplodingPalm));
            var isEnoughTime = TimeToElementStart(Element.Cold) > SevenSidedStrikeCooldownMs;
            var isColdElement = Core.Buffs.ConventionElement == Element.Cold;            

            Logger.Log(LogCategory.Routine, $"TimeToCold={TimeToElementStart(Element.Cold)} SSSCD={SevenSidedStrikeCooldownMs} IsCold={isColdElement} EPReady={isEPReady} closeEPUnits={closeEPUnits.Count}");

            if (isEPReady && ShouldSevenSidedStrike(out target) && (!Legendary.ConventionOfElements.IsEquipped || isColdElement || isEnoughTime))
                return SevenSidedStrike(target);      

            return GetExplodingPalmPrimary();
        }

        private TrinityPower GetExplodingPalmPrimary()
        {
            TrinityPower power = null;

            var target = TargetUtil.LowestHealthTarget(20f, CurrentTarget.Position, SNOPower.Monk_ExplodingPalm);

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

        protected override bool ShouldSevenSidedStrike(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.SevenSidedStrike.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(45f) && !CurrentTarget.IsTreasureGoblin)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;       
        }

        public double SevenSidedStrikeCooldownMs => (Runes.Monk.SustainedAttack.IsActive ? 14 : 30 * (1 - Player.CooldownReductionPct)) *1000;

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

