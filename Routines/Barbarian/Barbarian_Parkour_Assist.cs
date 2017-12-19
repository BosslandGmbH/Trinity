using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;


namespace Trinity.Routines.Barbarian
{
    public sealed class Barbarian_Parkour_Assist : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "老冰棍儿冲锋跑酷痛割蛮子";
        public string Description => "不正常人类研究所";
        public string Author => "暗夜骑士KON";
        public string Version => "0.2";
        public string Url => string.Empty;
        public Build BuildRequirements => null;

        #endregion

        /// <summary>
        /// Only cast in combat and the target is a unit
        /// </summary>
        public TrinityPower GetOffensivePower()
        {
			Vector3 position = Vector3.Zero;
            TrinityPower power;

            if (TrySpecialPower(out power))
                return power;
			
			///if (ShouldRend(out position))
            ///    return Rend(position);

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        /// <summary>
        /// Only cast when avoiding.
        /// </summary>
        public TrinityPower GetDefensivePower()
        {
            return GetBuffPower();
        }

        /// <summary>
        /// Cast always, in and out of combat.
        /// </summary>
        public TrinityPower GetBuffPower()
        {
            if (ShouldIgnorePain())
                return IgnorePain();

            if (ShouldBattleRage())
                return BattleRage();

            if (ShouldWarCry())
                return WarCry();

            if (ShouldCallOfTheAncients())
                return CallOfTheAncients();

            if (AllowedToUse(Settings.WrathOfTheBerserker, Skills.Barbarian.WrathOfTheBerserker) && ShouldWrathOfTheBerserker())
                return WrathOfTheBerserker();

            return null;
        }

        /// <summary>
        /// Only cast on destructibles/barricades
        /// </summary>
        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }

        /// <summary>
        /// Cast by all plugins for all movement.        
        /// </summary>
        public TrinityPower GetMovementPower(Vector3 destination)
        {
			if (ShouldSprint())
                return Sprint();
			
            if (AllowedToUse(Settings.FuriousCharge, Skills.Barbarian.FuriousCharge) && CanChargeTo(destination))
                return FuriousCharge(destination);

            return Walk(destination);
        }
		///protected override bool ShouldSprint()
        ///{
        ///    if (!Skills.Barbarian.Sprint.CanCast())
        ///        return false;
			
		///	   if (Skills.Barbarian.Sprint.TimeSinceUse < 3000)
        ///        return false;

        ///    return true;
        ///}

        protected override bool ShouldFuriousCharge(out Vector3 position)
        {
            position = Vector3.Zero;            

            if (!AllowedToUse(Settings.FuriousCharge, Skills.Barbarian.FuriousCharge))
                return false;

            return base.ShouldFuriousCharge(out position);
        }
		
		protected override bool ShouldRend(out Vector3 position)
        {
            position = Vector3.Zero;
            var skill = Skills.Barbarian.Rend;

            if (!skill.CanCast())
                return false;
			
			///if (Skills.Barbarian.FuriousCharge.CanCast())
            ///    return false;

            ///if (Player.PrimaryResource < 150)
            ///    return false;

            ///if (skill.IsLastUsed && IsMultiSpender && skill.TimeSinceUse < 250)
            ///    return false;

            if (skill.TimeSinceUse < 500) 
                return false;
			
            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        #region Settings      

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public Barbarian_Parkour_AssistSettings Settings { get; } = new Barbarian_Parkour_AssistSettings();

        public sealed class Barbarian_Parkour_AssistSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _wrathOfTheBerserker;
            private SkillSettings _furiousCharge;
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

            public SkillSettings WrathOfTheBerserker
            {
                get { return _wrathOfTheBerserker; }
                set { SetField(ref _wrathOfTheBerserker, value); }
            }

            public SkillSettings FuriousCharge
            {
                get { return _furiousCharge; }
                set { SetField(ref _furiousCharge, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings WrathOfTheBerserkerDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            };

            private static readonly SkillSettings FuriousChargeDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 200,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                WrathOfTheBerserker = WrathOfTheBerserkerDefaults.Clone();
                FuriousCharge = FuriousChargeDefaults.Clone();
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


