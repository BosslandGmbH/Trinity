using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Trinity.Framework;
using Zeta.Common.Helpers;

namespace Trinity.Routines.DemonHunter
{
    public sealed class DemonHunterMarauder : DemonHunterBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Marauder/Natalya's Cluster Arrow Routine";
        public string Description => "A focus on sentry placement and cluster arrow from range.";
        public string Author => "xzjv";
        public string Version => "0.2.1";
        public string Url => "http://www.icy-veins.com/d3/demon-hunter-sentry-cluster-arrow-build-with-the-embodiment-of-the-marauder-set-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.EmbodimentOfTheMarauder, SetBonus.Second }
            },
            Items = new List<Item>
            {
                Legendary.Manticore,
                Legendary.BombardiersRucksack
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.DemonHunter.ClusterArrow, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            Vector3 position;

            Vector3 buffPosition;
            if (ShouldWalkToGroundBuff(out buffPosition))
                return Walk(buffPosition);

            if (AllowedToUse(Settings.Vault, Skills.DemonHunter.Vault) && ShouldVault(out position))
                return Vault(position);

            if (ShouldRefreshBastiansGenerator && TryPrimaryPower(out power))
                return power;

            if (ShouldRefreshWrapsOfClarity() && TryPrimaryPower(out power))
                return power;

            if (TrySpecialPower(out power))
                return power;

            var shouldInterruptCasting = Core.Avoidance.Avoider.ShouldKite || Core.Avoidance.Avoider.ShouldAvoid;
            if (!shouldInterruptCasting && TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
                return Walk(CurrentTarget);

            return null;
        }

        private static bool ShouldRefreshWrapsOfClarity()
        {
            if (!Legendary.WrapsOfClarity.IsEquipped)
                return false;

            if (Player.IsInTown)
                return false;

            if (!TargetUtil.AnyMobsInRange(65f))
                return false;

            if (IsNoPrimary)
                return false;

            if (SpellHistory.TimeSinceGeneratorCast < 4750)
                return false;

            return true;
        }

        protected override bool ShouldVault(out Vector3 destination)
        {
            destination = Vector3.Zero;

            if (!Skills.DemonHunter.Vault.CanCast())
                return false;

            // The more we stand still the more damage we deal
            if (IsInCombat && !Core.Avoidance.Avoider.ShouldAvoid && !Core.Avoidance.Avoider.ShouldKite && Sets.EndlessWalk.IsEquipped)
                return false;

            // Try to vault to Occulus AoE whenever possible
            Vector3 bestBuffedPosition;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();

            if (TargetUtil.BestBuffPosition(45f, bestClusterPoint, false, out bestBuffedPosition) &&
                Player.Position.Distance2D(bestBuffedPosition) > 25f && bestBuffedPosition != Vector3.Zero)
            {
                Core.Logger.Log($"Found buff position - distance: {Player.Position.Distance(bestBuffedPosition)} ({bestBuffedPosition})");
                destination = bestBuffedPosition;

                return destination != Vector3.Zero;
            }

            return base.ShouldVault(out destination);
        }

        private Vector3 _lastBuffPosition;
        readonly WaitTimer _groundBuffWalkTimer = WaitTimer.FiveSeconds;
        private bool ShouldWalkToGroundBuff(out Vector3 buffPosition)
        {
            buffPosition = Vector3.Zero;
            if (CurrentTarget == null)
                return false;

            if (_lastBuffPosition != Vector3.Zero && _lastBuffPosition.Distance2D(CurrentTarget.Position) > 20)
                return false;

            if (_lastBuffPosition != Vector3.Zero && Player.Position.Distance2D(_lastBuffPosition) > 9f && !_groundBuffWalkTimer.IsFinished)
            {
                Core.Logger.Log($"Moving to buff: {_lastBuffPosition} - Distance: {Player.Position.Distance2D(_lastBuffPosition)}");
                return true;
            }

            _lastBuffPosition = Vector3.Zero;

            Vector3 bestBuffedPosition;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();

            if (TargetUtil.BestBuffPosition(20f, bestClusterPoint, false, out bestBuffedPosition) &&
                bestBuffedPosition != Vector3.Zero)
            {
                Core.Logger.Log($"Found buff: {bestBuffedPosition} - Distance: {Player.Position.Distance2D(bestBuffedPosition)}");
                buffPosition = bestBuffedPosition;
                if (bestBuffedPosition != Vector3.Zero)
                {
                    _lastBuffPosition = bestBuffedPosition;
                    _groundBuffWalkTimer.Reset();
                    return true;
                }
            }

            return false;
        }

        public TrinityPower GetDefensivePower()
        {
            if (ShouldSpikeTrap())
                return SpikeTrap();

            if (Runes.DemonHunter.FanOfDaggers.IsActive && ShouldFanOfKnives())
                return FanOfKnives();            

            if (ShouldCaltrops())
                return Caltrops();

            if (ShouldShadowPower())
                return ShadowPower();

            if (ShouldSmokeScreen())
                return SmokeScreen();

            return null;
        }

        public TrinityPower GetBuffPower()
        {
            if (AllowedToUse(Settings.Vengeance, Skills.DemonHunter.Vengeance) && ShouldVengeance())
                return Vengeance();

            if (ShouldShadowPower())
                return ShadowPower();

            if (ShouldSmokeScreen())
                return SmokeScreen();

            if (ShouldPreparation())
                return Preparation();

            if (ShouldCompanion())
                return Companion();

            return null;
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();


        public TrinityPower GetMovementPower(Vector3 destination)
        {
            var shouldAvoid = Core.Avoidance.Avoider.ShouldAvoid;
            if (!Player.IsInTown && (AllowedToUse(Settings.Vault, Skills.DemonHunter.Vault) || shouldAvoid) && CanVaultTo(destination))
            return Vault(destination);            
            
            if (!Player.IsInTown && CanStrafeTo(destination))
                return Strafe(destination);

            return Walk(destination);
        }

        #region Settings

        public override int KiteStutterDuration => 800;
        public override int KiteStutterDelay => 800;
        public override int KiteHealthPct => 90;
        public override float KiteDistance => Settings.KiteDistance;
        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DemonHunterMarauderSettings Settings { get; } = new DemonHunterMarauderSettings();

        public sealed class DemonHunterMarauderSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _vengeance;
            private SkillSettings _vault;
            private float _kiteDistance;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(1)]
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

            public SkillSettings Vault
            {
                get { return _vault; }
                set { SetField(ref _vault, value); }
            }

            public SkillSettings Vengeance
            {
                get { return _vengeance; }
                set { SetField(ref _vengeance, value); }
            }

            [DefaultValue(15f)]
            public float KiteDistance
            {
                get { return _kiteDistance; }
                set { SetField(ref _kiteDistance, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings VaultDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 1000,
                SecondaryResourcePct = 90f,
            };

            private static readonly SkillSettings VengeanceDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.Surrounded | UseReasons.HealthEmergency,
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Vault = VaultDefaults.Clone();
                Vengeance = VengeanceDefaults.Clone();
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


