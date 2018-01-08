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
using System;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;

namespace Trinity.Routines.Necromancer
{
    public sealed class NecromancerPestilenceCorpseLance : NecroMancerBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Pestilence Corpse Lance solo Necromancer";
        public string Description => "The Corpse Lance Necromancer is a powerful elite hunting build, capable of the whole spectrum of content difficulty with minor adjustments";
        public string Author => "jubisman";
        public string Version => "0.3.1";
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

        public override Func<bool> ShouldIgnorePackSize => () =>
        (Skills.Necromancer.LandOfTheDead.IsBuffActive || TargetUtil.CorpseCount(60f) > 0);

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

            if (ShouldSimulacrum(out position))
                return Simulacrum(position);

            if (ShouldWalkToTarget(out target))
            {
                //Core.Logger.Log("Walking to target");
                return Walk(target);
            }

            /*if (ShouldBloodRush(out position))
                return BloodRush(position);*/

            if (ShouldLandOfTheDead())
                return LandOfTheDead();

            if (ShouldDecrepify(out target))
                return Decrepify(target);

            if (ShouldCommandSkeletons(out target))
                return CommandSkeletons(target);

            if (ShouldCorpseLance(out target))
                return CorpseLance(target);

            if (ShouldBoneSpear(out target))
                return BoneSpear(target);

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            //Core.Logger.Log("Walking to safe spot");
            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15f, 60f, Player.Position, AvoidCondition);
            return Walk(position);
        }

        private bool AvoidCondition(AvoidanceNode n)
        {
            return HostileMonsters.FirstOrDefault(u => u.Distance < 10f) != null &&
                !Core.Avoidance.InAvoidance(n.NavigableCenter) &&
                Core.Avoidance.Grid.IsIntersectedByFlags(Player.Position, n.NavigableCenter, AvoidanceFlags.CriticalAvoidance);
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 position)
        {
            if (ShouldBloodRush(out position))
            {
                //Core.Logger.Log("TryBloodRushMovement");
                return BloodRush(position);
            }

            //Core.Logger.Log("GetMovementPower walk");
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

        protected override bool ShouldDecrepify(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.Decrepify.CanCast())
                return false;

            if (Skills.Necromancer.LandOfTheDead.IsBuffActive)
                return false;

            if (Skills.Necromancer.Decrepify.TimeSinceUse < 350)
                return false;

            target = TargetUtil.BestDecrepifyTarget(60f);
            return target != null;
        }

        protected override bool ShouldDevour()
        {
            if (!Skills.Necromancer.Devour.CanCast())
                return false;

            if (TargetUtil.CorpseCount(60f) <= 0)
                return false;

            if (Skills.Necromancer.Devour.TimeSinceUse < 350)
                return false;

            if (Player.PrimaryResourcePct < 0.4f || Player.CurrentHealthPct < 0.8f)
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

            // Dont move from outside avoidance into avoidance.
            if (!Core.Avoidance.InCriticalAvoidance(Player.Position) &&
                Core.Avoidance.Grid.IsIntersectedByFlags(Player.Position, position, AvoidanceFlags.CriticalAvoidance))
                return false;

            // Try to Rush to Occulus AoE whenever possible
            Vector3 bestBuffedPosition;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();

            if (TargetUtil.BestBuffPosition(50f, bestClusterPoint, false, out bestBuffedPosition) &&
                Player.Position.Distance2D(bestBuffedPosition) > 10f && bestBuffedPosition != Vector3.Zero
                && (Skills.Necromancer.LandOfTheDead.CanCast() || Skills.Necromancer.LandOfTheDead.IsBuffActive))
            {
                Core.Logger.Log($"Found buff position - distance: {Player.Position.Distance(bestBuffedPosition)} ({bestBuffedPosition})");
                position = bestBuffedPosition;

                return position != Vector3.Zero;
            }

            // Find a safespot with no monsters within range.
            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15f, 60f, Player.Position, AvoidCondition);

            return position != null;
        }

        protected override bool ShouldCorpseLance(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.CorpseLance.CanCast())
                return false;

            var shouldRefreshNays = Legendary.NayrsBlackDeath.IsBuffActive && Core.Buffs.GetBuffStacks(SNOPower.P6_ItemPassive_Unique_Ring_066) < 3;

            if (shouldRefreshNays || Skills.Necromancer.LandOfTheDead.IsBuffActive ||
                (Settings.UseExtraCorpses && TargetUtil.CorpseCount(60f) > 0))
            {
                target = TargetUtil.BestEliteInRange(60f) ?? CurrentTarget;
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

            var boneSpearTarget = TargetUtil.AnyElitesInRange(60f) ?
                TargetUtil.BestEliteInRange(60f, true) : TargetUtil.LowestHealthTarget(60f);

            target = boneSpearTarget ?? CurrentTarget;
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
            private bool _useExtraCorpses;

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

            [DefaultValue(false)]
            public bool UseExtraCorpses
            {
                get { return _useExtraCorpses; }
                set { SetField(ref _useExtraCorpses, value); }
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
