using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demonbuddy.Routines.Generic;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Helpers;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Objects
{
    public class SkillSettings : NotifyBase
    {
        public SkillSettings()
        {
            Items = new FullyObservableCollection<SkillUsage>();            
        }

        private ActorClass _actorClass;
        private string _name;
        private string _description;
        private string _author;
        private string _version;
        private FullyObservableCollection<SkillUsage> _items;

        public ActorClass ActorClass
        {
            get { return _actorClass; }
            set { SetField(ref _actorClass, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetField(ref _description, value); }
        }

        public string Author
        {
            get { return _author; }
            set { SetField(ref _author, value); }
        }

        public string Version
        {
            get { return _version; }
            set { SetField(ref _version, value); }
        }

        public override void LoadDefaults()
        {
            var items = new FullyObservableCollection<SkillUsage>();
            foreach (var skill in SkillUtils.ByActorClass(ActorClass))
            {
                items.Add(skill.GetDefaultSetting());
            }
            Items = items;
        }

        public FullyObservableCollection<SkillUsage> Items
        {
            get { return _items; }
            set { SetField(ref _items, value); }
        }        
    }

    public class SkillUsage : NotifyBase
    {
        private SNOPower _snoPower;
        private float _castRange;
        private float _clusterSize;
        private float _resourcePct;
        private float _healthPct;
        private float _recastDelayMs;
        private SkillUseTime _useTime;
        private SkillUseReasons _reasons;
        private SkillUseTarget _target;

        /// <summary>
        /// Skill these settings are for.
        /// </summary>
        public SNOPower SnoPower
        {
            get { return _snoPower; }
            set { SetField(ref _snoPower, value); }
        }

        /// <summary>
        /// Monster must be this close before a spell is cast.
        /// </summary>
        public float CastRange
        {
            get { return _castRange; }
            set { SetField(ref _castRange, value); }
        }

        /// <summary>
        /// Monsters must be in a group of this many before spell can be cast.
        /// </summary>
        public float ClusterSize
        {
            get { return _clusterSize; }
            set { SetField(ref _clusterSize, value); }
        }

        /// <summary>
        /// Player resource must be above (generally) or below (in some cases) this amount to cast spell.
        /// </summary>
        public float ResourcePct
        {
            get { return _resourcePct; }
            set { SetField(ref _resourcePct, value); }
        }

        /// <summary>
        /// Player health must be below this amount to cast spell
        /// </summary>
        public float HealthPct
        {
            get { return _healthPct; }
            set { SetField(ref _healthPct, value); }
        }

        /// <summary>
        /// Minimum time that must pass before casting again is allowed
        /// </summary>
        public float RecastDelayMs
        {
            get { return _recastDelayMs; }
            set { SetField(ref _recastDelayMs, value); }
        }

        /// <summary>
        /// When this spell can be used.
        /// </summary>
        public SkillUseTime UseTime
        {
            get { return _useTime; }
            set { SetField(ref _useTime, value); }
        }

        /// <summary>
        /// Situations when the spell should be used.
        /// </summary>
        public SkillUseReasons Reasons
        {
            get { return _reasons; }
            set { SetField(ref _reasons, value); }
        }

        /// <summary>
        /// When this spell can be used.
        /// </summary>
        public SkillUseTarget Target
        {
            get { return _target; }
            set { SetField(ref _target, value); }
        }

        public override void LoadDefaults()
        {
            SkillUtils.ById(SnoPower).GetDefaultSetting();
        }

    }

    public enum SkillUseTime
    {
        [Description("Use when it seems like a good idea")]
        AnyTime = 0,

        [Description("Use when not on cooldown")]
        Always,

        [Description("Use when in combat only")]
        InCombat,

        [Description("Use when not in combat only")]
        OutOfCombat
    }

    public enum SkillUseTarget
    {
        [Description("Use on default target")]
        Default = 0,

        [Description("Cast on current target")]
        CurrentTarget,

        [Description("Cast on closest target")]
        ClosestMonster,

        [Description("Cast on the best cluster")]
        BestCluster,
    }

    [Flags]
    public enum SkillUseReasons
    {
        None = 0,

        [Description("Use when avoiding")]
        Avoidance = 1 << 0,

        [Description("Use for movement")]
        Movement = 1 << 1,

        [Description("Use when surrounded by monsters")]
        Surrounded = 1 << 2,

        [Description("Use when elites are nearby or targetted")]
        Elites = 1 << 3,

        [Description("Use to continually spend player resource")]
        DumpResource = 1 << 4,

        [Description("Use when goblins are nearby or targetted")]
        Goblins = 1 << 5,

        [Description("Use when player is in trouble")]
        HealthEmergency = 1 << 6,

        [Description("Use when the target's movement is blocked")]
        Blocked = 1 << 7,

        All = ~(1 << 8),
    }
}




