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
using System.Linq;
using Zeta.Game.Internals.Actors;

namespace Trinity.Routines.Necromancer
{
    public sealed class NecromancerPestilenceCorpseLance : NecroMancerBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Pestilence Corpse Lance solo Necromancer";
        public string Description => "The Corpse Lance Necromancer is a powerful elite hunting build, capable of the whole spectrum of content difficulty with minor adjustments";
        public string Author => "jubisman";
        public string Version => "0.1";
        public string Url => "https://www.icy-veins.com/d3/necromancer-corpse-lance-build-patch-2-6-1-season-12";

        #endregion

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.PestilenceMastersShroud, SetBonus.Third },
                //{ Sets.TragoulsAvatar, SetBonus.Third }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Necromancer.CorpseLance, null },
            }
        };

        public TrinityPower GetBuffPower()
        {
            if (ShouldDevour())
                return Devour();

            if (ShouldBoneArmor())
                return BoneArmor();

            return null;
        }

        public TrinityPower GetOffensivePower()
        {
            TrinityActor target;
            Vector3 position;
            TrinityPower power;

            if (ShouldWalkToTarget(out target))
                return Walk(target);

            if (ShouldBloodRush(out position))
                return BloodRush(position);

            if (ShouldDecrepify(out target))
                return Decrepify(target);

            if (ShouldCommandSkeletons(out target))
                return CommandSkeletons(target);

            if (ShouldSimulacrum(out position))
                return Simulacrum(position);

            if (ShouldLandOfTheDead(out target))
                return LandOfTheDead(target);

            if (ShouldCorpseLance(out target))
                return CorpseLance(target);

            if (ShouldBoneSpear(out target))
                return BoneSpear(target);

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return Walk(TargetUtil.GetSafeSpotPosition(50f));
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            TrinityPower power;

            if (TryBloodrushMovement(destination, out power))
                return BloodRush(destination);

            return Walk(destination);
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

        protected override bool ShouldDevour()
        {
            if (!Skills.Necromancer.Devour.CanCast())
                return false;

            if (TargetUtil.CorpseCount(60f) <= 0)
                return false;

            if (Player.PrimaryResourcePct < 0.4f)
                return true;

            if (Player.CurrentHealthPct < 0.8f)
                return true;

            return false;

        }

        protected override bool ShouldBloodRush(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Necromancer.BloodRush.CanCast())
                return false;

            if (Skills.Necromancer.BloodRush.TimeSinceUse < 750)
                return false;

            if (!AllowedToUse(Settings.BloodRush, Skills.Necromancer.BloodRush))
                return false;

            Vector3 bestBuffedPosition;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();

            if (TargetUtil.BestBuffPosition(50f, bestClusterPoint, false, out bestBuffedPosition) &&
                Player.Position.Distance2D(bestBuffedPosition) > 10f && bestBuffedPosition != Vector3.Zero)
            {
                Core.Logger.Log($"Found buff position - distance: {Player.Position.Distance(bestBuffedPosition)} ({bestBuffedPosition})");
                position = bestBuffedPosition;

                return position != Vector3.Zero;
            }

            return true;
        }

        protected override bool ShouldCorpseLance(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.CorpseLance.CanCast())
                return false;

            if (Skills.Necromancer.CorpseLance.TimeSinceUse < 150)
                return false;

            var shouldRefreshNays = Legendary.NayrsBlackDeath.IsBuffActive && Core.Buffs.GetBuffStacks(SNOPower.P6_ItemPassive_Unique_Ring_066) < 3;

            if (shouldRefreshNays || Skills.Necromancer.LandOfTheDead.IsBuffActive)
            {
                target = TargetUtil.ClosestUnit(60f) ?? CurrentTarget;
                return target != null;
            }

            return false;
        }

        protected override bool ShouldBoneSpear(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.BoneSpear.CanCast())
                return false;


            if (Skills.Necromancer.LandOfTheDead.IsBuffActive)
                return false;

            target = TargetUtil.LowestHealthTarget(60f) ?? CurrentTarget;
            return target != null;

        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public NecromancerPestilenceCorpseLanceSettings Settings { get; } = new NecromancerPestilenceCorpseLanceSettings();

        public sealed class NecromancerPestilenceCorpseLanceSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _bloodRush;
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

            public SkillSettings BloodRush
            {
                get { return _bloodRush; }
                set { SetField(ref _bloodRush, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings LandOfTheDeadDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            };

            private static readonly SkillSettings BloodRushDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 3500,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                BloodRush = BloodRushDefaults.Clone();
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
