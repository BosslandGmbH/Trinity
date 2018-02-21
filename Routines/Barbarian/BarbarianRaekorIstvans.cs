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
using Zeta.Game.Internals.Actors;
using System.Linq;
using Zeta.Game;
using Trinity.Framework.Avoidance.Structures;

namespace Trinity.Routines.Barbarian
{
    public sealed class BarbarianRaekorIstvans : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Barbarian Raekor Istvan's";

        public string Description =>
            "GR build based on building charges during the CoE rotation and then using HotA to consume them during the Fire phase to deal enormous amounts of damage";

        public string Author => "jubisman";
        public string Version => "0.1";
        public string Url => "https://www.diablofans.com/builds/97646-asdfasdfasdfasdfasdf";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                {Sets.TheLegacyOfRaekor, SetBonus.Third},
                {Sets.IstvansPairedBlades, SetBonus.First }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                {Skills.Barbarian.FuriousCharge, null},
                {Skills.Barbarian.HammerOfTheAncients, null},
            }
        };

        #endregion

        public TrinityPower GetBuffPower()
        {
            Vector3 position;

            if (ShouldIgnorePain())
                return IgnorePain();

            if (ShouldSprint())
                return Sprint();

            if (ShouldBattleRage())
                return BattleRage();

            if (ShouldWarCry())
                return WarCry();

            if (ShouldThreateningShout(out position))
                return ThreateningShout(position);

            if (ShouldCallOfTheAncients())
                return CallOfTheAncients();

            if (ShouldWrathOfTheBerserker())
                return WrathOfTheBerserker();

            return null;
        }

        public TrinityPower GetOffensivePower()
        {
            Vector3 position;
            TrinityActor target;
            TrinityPower power;

            if (ShouldWalkToTarget(out target))
                return Walk(target);

            if (ShouldHammerOfTheAncients(out target))
                return HammerOfTheAncients(target);

            if (ShouldFuriousCharge(out position))
                return FuriousCharge(position);

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            //Core.Logger.Log("walking to safespot because all other powers failed");
            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15f, 40f, Player.Position,
                node => !TargetUtil.AnyMobsInRangeOfPosition(node.NavigableCenter));
            return Walk(position);
        }

        private static bool ShouldWalkToTarget(out TrinityActor target)
        {
            target = null;

            if (CurrentTarget.Distance > 60f)
            {
                Core.Logger.Log("Target too far away to attack. Walking closer");
                target = CurrentTarget;
                return target != null;
            }

            return false;
        }

        protected override bool ShouldBattleRage()
        {
            if (!Skills.Barbarian.BattleRage.CanCast())
                return false;

            if (Player.IsInTown)
                return false;

            if (Skills.Barbarian.BattleRage.TimeSinceUse > 4750)
                return true;

            if (Core.Buffs.GetBuffStacks(SNOPower.ItemPassive_Unique_Ring_734_x1) < 5 && Skills.Barbarian.BattleRage.TimeSinceUse > 500)
                return true;

            return false;
        }

        protected override bool ShouldWrathOfTheBerserker()
        {
            if (ShouldWaitForConventionofElements(Skills.Barbarian.WrathOfTheBerserker, Element.Fire, 300))
                return false;

            return base.ShouldWrathOfTheBerserker();
        }

        protected override bool ShouldFuriousCharge(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.FuriousCharge.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(60f))
                return false;

            if (Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, position, AvoidanceFlags.CriticalAvoidance))
                return false;

            // Try to charge to Occulus AoE before the Whack-a-mole frenzy begins
            Vector3 bestBuffedPosition;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();

            if (TargetUtil.BestBuffPosition(60f, bestClusterPoint, false, out bestBuffedPosition) &&
                Player.Position.Distance2D(bestBuffedPosition) > 10f && bestBuffedPosition != Vector3.Zero &&
                !ShouldWaitForConventionofElements(Skills.Crusader.Provoke, Element.Fire, 350))
            {
                Core.Logger.Log($"Found buff position - distance: {Player.Position.Distance(bestBuffedPosition)} ({bestBuffedPosition})");
                position = bestBuffedPosition;

                return position != Vector3.Zero;
            }

            var chargeStacks = Core.Buffs.GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_026);
            if (Core.Buffs.ConventionElement == Element.Fire && chargeStacks > 0)
                return false;

            // Quickest way to build stacks is to charge in place (if we can get a refund for every charge)
            var twelveYardsUnitCount = TargetUtil.UnitsInRangeOfPosition(Player.Position, 12f).Count(u => u.IsUnit);
            var twentyYardsUnitCount = TargetUtil.UnitsInRangeOfPosition(Player.Position, 20f).Count(u => u.IsUnit);
            if (twelveYardsUnitCount >= 3 || twentyYardsUnitCount == 1)
                position = Player.Position;
            position = TargetUtil.GetBestClusterPoint(15f, 20f, true, false);

            return position != Vector3.Zero;
        }

        protected override bool ShouldHammerOfTheAncients(out TrinityActor target)
        {
            target = null;
            var chargeStacks = Core.Buffs.GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_026);

            if (!Skills.Barbarian.HammerOfTheAncients.CanCast())
                return false;

            if (Core.Buffs.ConventionElement != Element.Fire && chargeStacks < 100)
                return false;

            // Check for FuriousCharge stacks
            if (chargeStacks <= 0)
                return false;

            Core.Logger.Log("Stop! Hammer time!");
            target = TargetUtil.ClosestUnit(30f, u => u.IsInLineOfSight) ?? CurrentTarget;
            return target != null;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            var charges = Skills.Barbarian.FuriousCharge.Charges;
            var shouldAvoid = Core.Avoidance.Avoider.ShouldAvoid;
            if (CanChargeTo(destination) && (AllowedToUse(Settings.FuriousCharge, Skills.Barbarian.FuriousCharge) || shouldAvoid))
            {
                if (IsBlocked && charges > 0)
                    return FuriousCharge(destination);

                var chargeRange = Player.Position.Distance(destination);
                if (TargetUtil.TargetsInFrontOfMe(chargeRange, 6f).Count > 3 &&
                    Skills.Barbarian.FuriousCharge.Charges > 0)
                {
                    Core.Logger.Log("Charging through enemy/destructible since it refunds a charge.");
                    return FuriousCharge(destination);
                }

                if (!IsBlocked && charges > 1)
                    return FuriousCharge(destination);
            }

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public BarbarianRaekorIstvansSettings Settings { get; } = new BarbarianRaekorIstvansSettings();

        public sealed class BarbarianRaekorIstvansSettings : NotifyBase, IDynamicSetting
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

            #region FuriousCharge

            public SkillSettings FuriousCharge
            {
                get { return _furiousCharge; }
                set { SetField(ref _furiousCharge, value); }
            }

            private static readonly SkillSettings VaultDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 500,
                Reasons = UseReasons.Blocked
            };

            private SkillSettings _furiousCharge;

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                FuriousCharge = VaultDefaults.Clone();
            }

            #endregion

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();

            public void Save()
            {
            }

            #endregion
        }

        #endregion
    }
}