using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects
{
    [DataContract(Namespace = "")]
    public sealed class SkillSettings : NotifyBase
    {
        public SkillSettings()
        {
            LoadDefaults();
        }

        public SkillSettings(Skill skillSettings)
        {
            LoadDefaults();
            SetReferenceSkill(skillSettings);
        }

        public void SetReferenceSkill(Skill skill)
        {
            if (skill == null)
                return;

            SNOPower = skill.SNOPower;
            Skill = skill;
        }

        [DataMember(EmitDefaultValue = false)]
        public SNOPower SNOPower
        {
            get { return _SNOPower; }
            set { SetField(ref _SNOPower, value); }
        }

        [IgnoreDataMember]
        public Skill Skill { get; set; }

        private float _castRange;
        private float _clusterSize;
        private float _primaryResourcePct;
        private float _secondaryResourcePct;
        private float _healthPct;
        private int _recastDelayMs;
        private UseTime _useMode;
        private UseReasons _reasons;
        private UseTarget _target;
        private ConventionMode _waitForConvention;
        private SNOPower _SNOPower;
        private int _order;

        /// <summary>
        /// Monster must be this close before a spell is cast.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public float CastRange
        {
            get { return _castRange; }
            set { SetField(ref _castRange, value); }
        }

        /// <summary>
        /// Monsters must be in a group of this many before spell can be cast.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public float ClusterSize
        {
            get { return _clusterSize; }
            set { SetField(ref _clusterSize, value); }
        }

        /// <summary>
        /// Player resource must be above this to cast spell.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public float PrimaryResourcePct
        {
            get { return _primaryResourcePct; }
            set { SetField(ref _primaryResourcePct, value); }
        }

        /// <summary>
        /// Player resource must be above this to cast spell.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public float SecondaryResourcePct
        {
            get { return _secondaryResourcePct; }
            set { SetField(ref _secondaryResourcePct, value); }
        }

        /// <summary>
        /// Player health must be below this amount to cast spell
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public float HealthPct
        {
            get { return _healthPct; }
            set { SetField(ref _healthPct, value); }
        }

        /// <summary>
        /// Minimum time that must pass before casting again is allowed
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int RecastDelayMs
        {
            get { return _recastDelayMs; }
            set { SetField(ref _recastDelayMs, value); }
        }

        /// <summary>
        /// When this spell can be used.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue(UseTime.Always)]
        public UseTime UseMode
        {
            get { return _useMode; }
            set { SetField(ref _useMode, value); }
        }

        /// <summary>
        /// How convention of elements ring should be handled.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public ConventionMode WaitForConvention
        {
            get { return _waitForConvention; }
            set { SetField(ref _waitForConvention, value); }
        }

        /// <summary>
        /// Situations when the spell should be used.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public UseReasons Reasons
        {
            get { return _reasons; }
            set { SetField(ref _reasons, value); }
        }

        /// <summary>
        /// When this spell can be used.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public UseTarget Target
        {
            get { return _target; }
            set { SetField(ref _target, value); }
        }

        #region Delegates

        [IgnoreDataMember]
        public Func<bool> BuffCondition { get; set; }

        [IgnoreDataMember]
        public Func<bool> ConventionCondition { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Order
        {
            get { return _order; }
            set { SetField(ref _order, value); }
        } 

        #endregion

        public SkillSettings Clone()
        {
            var newObj = new SkillSettings();
            PropertyCopy.Copy(this, newObj);
            return newObj;
        }

        public void Refresh()
        {
            OnPropertyChanged("");
        }
    }

    public enum UseTime
    {
        [Description("Use when it seems like a good idea")]
        Default = 0,

        [Description("Whenever Possible")]
        Always,

        [Description("Dont use it")]
        Never,

        [Description("Use when in combat only")]
        InCombat,

        [Description("Use when not in combat only")]
        OutOfCombat,

        [Description("Use restricted by other settings")]
        Selective,
    }

    public enum UseTarget
    {
        [Description("Cast on default target")]
        Default = 0,

        [Description("Cast on current target")]
        CurrentTarget,

        [Description("Cast on closest target")]
        ClosestMonster,

        [Description("Cast on elite targets")]
        ElitesOnly,

        [Description("Cast on the best cluster")]
        BestCluster,

        [Description("Cast on a safe location")]
        SafeSpot,

        [Description("Cast on Self")]
        Self,        
    }

    [Flags]
    public enum UseReasons : uint
    {
        None = 0,

        [Description("Use when avoiding")]
        Avoiding = 1,

        [Description("Use for movement")]
        Movement = 2,

        [Description("Use when surrounded by monsters")]
        Surrounded = 4,

        [Description("Use when elites are nearby or targetted")]
        Elites = 8,

        [Description("Use to continually spend player resource")]
        DumpResource = 16,

        [Description("Use when goblins are nearby or targetted")]
        Goblins = 32,

        [Description("Use when player is in trouble")]
        HealthEmergency = 64,

        [Description("Use when the target's movement is blocked")]
        Blocked = 128,

        [Description("Use associated with a buff")]
        Buff = 256,

        [Description("Use when trash are nearby above cluster size")]
        Trash = 512,
    }

    public enum ConventionMode
    {
        Always,
        GreaterRift,
        Never,
        RiftBoss,
    }

}

