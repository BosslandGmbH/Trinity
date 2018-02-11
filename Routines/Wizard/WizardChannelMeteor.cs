using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Trinity.Framework.Avoidance.Structures;
using System;
using Trinity.Components.Combat;
using Zeta.Game.Internals.Actors;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardChannelMeteor : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Channel Meteor Wizard";
        public string Description => "Channeling immense powers to call forth destruction from the sky, the Meteor Wizard alternates" +
            " the might of two sets according to his whims and conjures fiery death. This bursty, hard hitting playstyle is available in " +
            "two Greater Rift solo and regular Rift farming variations, explained in that order.";
        public string Author => "jubisman";
        public string Version => "0.3.1";
        public string Url => "https://www.icy-veins.com/d3/wizard-channeling-meteor-firebird-build-patch-2-6-1-season-12";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TalRashasElements, SetBonus.Third },
                //{ Sets.FirebirdsFinery, SetBonus.Third }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Wizard.Meteor, null },
            }
        };

        #endregion

        public override float TrashRange => 40f;
        public override float EliteRange => 60f;
        public override Func<bool> ShouldIgnoreKiting => IgnoreCondition;
        public override Func<bool> ShouldIgnorePackSize => () => Player.IsChannelling;
        public override Func<bool> ShouldIgnoreNonUnits => () => Player.IsChannelling && Player.CurrentHealthPct > 0.35;

        private bool IgnoreCondition()
        {
            var isArcaneTorrentSelected = TrinityCombat.Targeting.CurrentPower?.SNOPower == SNOPower.Wizard_ArcaneTorrent;
            var isInAvoidance = Core.Avoidance.InCriticalAvoidance(Player.Position);
            if (isArcaneTorrentSelected && TargetUtil.AnyMobsInRange(30f) && Player.CurrentHealthPct > 0.5f && !isInAvoidance)
                return true;

            return Player.IsChannelling && Player.CurrentHealthPct > 0.35;
        }

        public TrinityPower GetOffensivePower()
        {
            Vector3 position;
            TrinityActor target;
            TrinityPower power;

            if (ShouldContinueChanneling(out target))
                return ArcaneTorrent(target);

            if (ShouldWalkToTarget(out target))
                return Walk(target);

            if (ShouldTeleport(out position))
                return Teleport(position);

            if (ShouldFrostNova())
                return FrostNova();

            if (ShouldFamiliar())
                return Familiar();

            if (ShouldMeteor(out target))
                return Meteor(target);

            if (ShouldArcaneTorrent(out target))
                return ArcaneTorrent(target);

            if (TryPrimaryPower(out power))
                return power;

            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15f, 40f, Player.Position,
                node => !TargetUtil.AnyMobsInRangeOfPosition(node.NavigableCenter));
            return Walk(position);
        }

        private static bool ShouldWalkToTarget(out TrinityActor target)
        {
            target = null;

            if (CurrentTarget.Distance > 60f)
            {
                target = CurrentTarget;
                return target != null;
            }

            return false;
        }

        protected override bool ShouldFrostNova()
        {
            if (!Skills.Wizard.FrostNova.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(60f))
                return false;

            return true;
        }

        protected override bool ShouldTeleport(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (Skills.Wizard.Teleport.TimeSinceUse < 200)
                return false;

            if (!AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport))
                return false;

            // Dont move from outside avoidance into avoidance.
            if (!Core.Avoidance.InAvoidance(Player.Position) && Core.Avoidance.Grid.IsLocationInFlags(position, AvoidanceFlags.Avoidance))
                return false;

            Vector3 bestBuffedPosition;
            var hasAPDs = Legendary.AncientParthanDefenders.IsEquipped;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();
            var teleportPoint = hasAPDs ? bestClusterPoint : TargetUtil.GetSafeSpotPosition(40f);
            var oculusMobs = hasAPDs ? TargetUtil.NumMobsInRangeOfPosition(teleportPoint, 10f) > 3 : true;
            var distance = hasAPDs ? 25f : 50f;

            if (TargetUtil.BestBuffPosition(distance, bestClusterPoint, false, out bestBuffedPosition) &&
                Player.Position.Distance2D(bestBuffedPosition) > 10f && bestBuffedPosition != Vector3.Zero && oculusMobs)
            {
                Core.Logger.Log($"Found buff position - distance: {Player.Position.Distance(bestBuffedPosition)} ({bestBuffedPosition})");
                position = bestBuffedPosition;
                return position != Vector3.Zero;
            }

            position = teleportPoint;
            return position != Vector3.Zero;
        }

        protected override bool ShouldMeteor(out TrinityActor target)
        {
            target = null;

            if (Skills.Wizard.Meteor.TimeSinceUse < 3000)
                return false;

            //Core.Logger.Log("Cast Meteor After {0} miliseconds", Skills.Wizard.Meteor.TimeSinceUse);
            return base.ShouldMeteor(out target);
        }

        protected override bool ShouldArcaneTorrent(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArcaneTorrent.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit(15f, 40f) ?? CurrentTarget;
            return target != null;
        }

        private bool ShouldContinueChanneling(out TrinityActor target)
        {
            Vector3 position;
            var interruptForTeleport = ShouldTeleport(out position);
            var interruptForMeteor = ShouldMeteor(out target);

            if (!Player.IsChannelling)
                return false;

            if (Player.IsChannelling)
            {
                if (interruptForTeleport)
                    return false;

                if (interruptForMeteor)
                    return false;
            }

            target = TargetUtil.GetClosestUnit(60f) ?? CurrentTarget;
            return target != null;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetBuffPower()
        {
            return DefaultBuffPower();
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            var shouldAvoid = Core.Avoidance.Avoider.ShouldAvoid;
            if ((AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport) || shouldAvoid) && CanTeleportTo(destination))
                return Teleport(destination);

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public override float KiteDistance => 5f;
        public override int KiteHealthPct => 35;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardChannelMeteorSettings Settings { get; } = new WizardChannelMeteorSettings();

        public sealed class WizardChannelMeteorSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
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

            public SkillSettings Teleport
            {
                get { return _teleport; }
                set { SetField(ref _teleport, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings TeleportDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 200,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Teleport = TeleportDefaults.Clone();
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