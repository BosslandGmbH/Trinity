using System;
using Trinity.Framework.Helpers;
using System.ComponentModel;
using System.Runtime.Serialization;
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
        private float _secondaryResourcePctBelow;
        private float _primaryResourcePctBelow;

        /// <summary>
        /// Monster must be this close before a spell is cast.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue(60f)]
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
        /// Player primary resource must be above this to cast spell.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public float PrimaryResourcePct
        {
            get { return _primaryResourcePct; }
            set { SetField(ref _primaryResourcePct, value); }
        }

        /// <summary>
        /// Player primary resource must be above this to cast spell.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue(100f)]
        public float PrimaryResourcePctBelow
        {
            get { return _primaryResourcePctBelow; }
            set { SetField(ref _primaryResourcePctBelow, value); }
        }

        /// <summary>
        /// Player secondary resource must be above this to cast spell.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public float SecondaryResourcePct
        {
            get { return _secondaryResourcePct; }
            set { SetField(ref _secondaryResourcePct, value); }
        }

        /// <summary>
        /// Player secondary resource must be below this to cast spell.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue(100f)]
        public float SecondaryResourcePctBelow
        {
            get { return _secondaryResourcePctBelow; }
            set { SetField(ref _secondaryResourcePctBelow, value); }
        }

        /// <summary>
        /// Player health must be below this amount to cast spell
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue(1f)]
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
        [Description("使用时，它似乎是一个不错的想法")]
        Default = 0,

        [Description("在任何可能的时候")]
        Always,

        [Description("不使用它")]
        Never,

        [Description("仅在战斗中使用")]
        InCombat,

        [Description("仅当不在战斗中使用")]
        OutOfCombat,

        [Description("使用其他设置限制")]
        Selective,
    }

    public enum UseTarget
    {
        [Description("在默认的目标施放")]
        Default = 0,

        [Description("对当前目标施放")]
        CurrentTarget,

        [Description("对最近的目标施放")]
        ClosestMonster,

        [Description("对精英施放")]
        ElitesOnly,

        [Description("对最佳集群施放")]
        BestCluster,

        [Description("在一个安全的位置施放")]
        SafeSpot,

        [Description("对自己施放")]
        Self,        
    }

    [Flags]
    public enum UseReasons : uint
    {
        None = 0,

        [Description("使用时避免")]
        Avoiding = 1,

        [Description("用于移动")]
        Movement = 2,

        [Description("当被怪物包围使用")]
        Surrounded = 4,

        [Description("当精英们都在附近或针对性使用")]
        Elites = 8,

        [Description("使用持续消耗玩家能量")]
        DumpResource = 16,

        [Description("当地精在附近或目标时使用")]
        Goblins = 32,

        [Description("当玩家遇到麻烦时使用")]
        HealthEmergency = 64,

        [Description("当目标的移动被阻挡时使用")]
        Blocked = 128,

        [Description("使用与buff相关联")]
        Buff = 256,

        [Description("当垃圾怪靠近'群怪数量'以上时使用")]
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

