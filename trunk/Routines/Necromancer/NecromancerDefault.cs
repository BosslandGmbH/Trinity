using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Necromancer
{
    public sealed class NecromancerDefault : NecroMancerBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Necromancer Generic Routine";
        public string Description => "Generic class support, casts all spells whenever possible";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => string.Empty;
        public Build BuildRequirements => null;

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            if (TryCursePower(out power))
                return power;

            if (TryBloodPower(out power))
                return power;

            if (ShouldApplyCurseWithScythe() && ShouldGrimScythe(out target))
                return GrimScythe(target);

            if (TryCorpsePower(out power))
                return power;

            if (TryReanimationPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        private bool ShouldApplyCurseWithScythe()
        {
            // Inarius build with Shadowhook should rarely run out of resource to force primary,
            // so it needs to be occasionally prioritized to apply curses.
            return Runes.Necromancer.CursedScythe.IsActive && Skills.Necromancer.GrimScythe.TimeSinceUse > 2000;
        }

        protected override bool ShouldCommandSkeletons(out TrinityActor target)
        {
            if (base.ShouldCommandSkeletons(out target))
            {
                return Skills.Necromancer.CommandSkeletons.TimeSinceUse > 5000;
            }
            return false;
        }

        public TrinityPower GetDefensivePower()
        {
            return GetBuffPower();
        }

        public TrinityPower GetBuffPower()
        {
            if (Skills.Necromancer.BloodRush.CanCast())
            {
                if (CurrentTarget?.Type == TrinityObjectType.ProgressionGlobe)
                {
                    return BloodRush(CurrentTarget.Position);
                }
                if (Player.CurrentHealthPct < 0.25)
                {
                    return BloodRush(Avoider.SafeSpot);
                }
            }
            // Put up bone armor when running around with high cluster size setting and not yet fighting
            if (!Skills.Necromancer.BoneArmor.IsBuffActive && TargetUtil.AnyMobsInRange(15f, 3))
            {
                return BoneArmor();
            }
            return null;
        }

        protected override bool ShouldLandOfTheDead(out TrinityActor target)
        {
            return base.ShouldLandOfTheDead(out target) && (target.IsElite || CurrentTarget.IsElite && CurrentTarget.Distance < 20f);
        }

        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            TrinityPower power;

            if (!IsInCombat || CurrentTarget != null && CurrentTarget.IsElite && CurrentTarget.Position.Distance(destination) <= 10f)
            {
                if (TryBloodrushMovement(destination, out power))
                    return power;
            }

            return Walk(destination);
        }

        #region Settings      

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public NecromancerDefaultSettings Settings { get; } = new NecromancerDefaultSettings();

        public sealed class NecromancerDefaultSettings : NotifyBase, IDynamicSetting
        {
            //private SkillSettings _wrathOfTheBerserker;
            //private SkillSettings _furiousCharge;

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

            //public SkillSettings WrathOfTheBerserker
            //{
            //    get { return _wrathOfTheBerserker; }
            //    set { SetField(ref _wrathOfTheBerserker, value); }
            //}

            //public SkillSettings FuriousCharge
            //{
            //    get { return _furiousCharge; }
            //    set { SetField(ref _furiousCharge, value); }
            //}

            //#region Skill Defaults

            //private static readonly SkillSettings WrathOfTheBerserkerDefaults = new SkillSettings
            //{
            //    UseMode = UseTime.Selective,
            //    Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            //};

            //private static readonly SkillSettings FuriousChargeDefaults = new SkillSettings
            //{
            //    UseMode = UseTime.Default,
            //    RecastDelayMs = 200,
            //    Reasons = UseReasons.Blocked
            //};

            //#endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                //WrathOfTheBerserker = WrathOfTheBerserkerDefaults.Clone();
                //FuriousCharge = FuriousChargeDefaults.Clone();
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


