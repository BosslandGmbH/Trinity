using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Helpers;

namespace Trinity.Config.Combat
{
    [DataContract(Namespace = "")]
    public class MiscCombatSetting : NotifyBase, ITrinitySetting<MiscCombatSetting>, INotifyPropertyChanged
    {
        #region Fields
        private GoblinPriority _GoblinPriority;
        private int _NonEliteRange;
        private int _EliteRange;
        private bool _ExtendedTrashKill;
        private bool _AvoidAOE;
        private bool _KillMonstersInAoE;
        private bool _CollectHealthGlobe;
        private bool _AllowOOCMovement;
        private bool _AllowBacktracking;
        private int _DelayAfterKill;
        private bool _UseNavMeshTargeting;
        private bool _UseConventionElementOnly;
        private bool _IgnoreCoEunlessGRift;
        private bool _HiPriorityHG;
        private int _trashPackSize;
        private float _TrashPackClusterRadius;
        private bool _IgnoreElites;
        private bool _AvoidDeath;
        private bool _SkipElitesOn5NV;
        private bool _AvoidanceNavigation;
        private double _ForceKillTrashBelowHealth;
        private double _IgnoreTrashBelowHealthDoT;
        private bool _UseExperimentalSavageBeastAvoidance;
        private bool _UseExperimentalFireChainsAvoidance;
        private float _ForceKillElitesHealth;
        private bool _ForceKillSummoners;
        private bool _ProfileTagOverride;
        private bool _AvoidAoEOutOfCombat;
        private bool _FleeInGhostMode;
        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _ignoreMonstersWhileReflectingDamage;
        private FollowerBossFightMode _followerBossFightDialogMode;
        private bool _ignoreHighHitePointTrash;
        private bool _ignoreRares;
        private bool _ignoreChampions;
        private bool _tryToSnapshot;
        private double _snapshotAttackSpeed;
        private bool _ignorePowerGlobes;
        private bool _forceKillClusterElites;
        private double _riftValueIgnoreUnitsBelow;
        private double _riftValueAlwaysKillUnitsAbove;
        private double _riftValueAlwaysKillClusterValue;
        private double _riftValueAlwaysKillClusterByValueRadius;
        private bool _ignoreMinions;
        private bool _attackWhenBlocked;
        private bool _ignoreNormalProgressionGlobes;
        private bool _ignoreGreaterProgressionGlobes;
        private int _trashPackSizeMin;
        private double _riftProgressionAlwaysKillPct;
        private float _HealthGlobeLevel;
        private float _PotionLevel;
        private float _HealthGlobeLevelResource;
        private bool _waitForResInBossEncounters;

        #endregion Events

        #region Constructors
        public MiscCombatSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(0.40f)]
        public float PotionLevel
        {
            get
            {
                return _PotionLevel;
            }
            set
            {
                if (_PotionLevel != value)
                {
                    _PotionLevel = value;
                    OnPropertyChanged("PotionLevel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.45f)]
        public float HealthGlobeLevel
        {
            get
            {
                return _HealthGlobeLevel;
            }
            set
            {
                if (_HealthGlobeLevel != value)
                {
                    _HealthGlobeLevel = value;
                    OnPropertyChanged("HealthGlobeLevel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.5f)]
        public float HealthGlobeLevelResource
        {
            get
            {
                return _HealthGlobeLevelResource;
            }
            set
            {
                if (_HealthGlobeLevelResource != value)
                {
                    _HealthGlobeLevelResource = value;
                    OnPropertyChanged("HealthGlobeLevelResource");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreNormalProgressionGlobes
        {
            get
            {
                return _ignoreNormalProgressionGlobes;
            }
            set
            {
                if (_ignoreNormalProgressionGlobes != value)
                {
                    _ignoreNormalProgressionGlobes = value;
                    OnPropertyChanged("IgnoreProgressionGlobes");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreGreaterProgressionGlobes
        {
            get
            {
                return _ignoreGreaterProgressionGlobes;
            }
            set
            {
                if (_ignoreGreaterProgressionGlobes != value)
                {
                    _ignoreGreaterProgressionGlobes = value;
                    OnPropertyChanged("IgnoreGreaterProgressionGlobes");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AttackWhenBlocked
        {
            get
            {
                return _attackWhenBlocked;
            }
            set
            {
                if (_attackWhenBlocked != value)
                {
                    _attackWhenBlocked = value;
                    OnPropertyChanged("AttackWhenBlocked");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreMinions
        {
            get
            {
                return _ignoreMinions;
            }
            set
            {
                if (_ignoreMinions != value)
                {
                    _ignoreMinions = value;
                    OnPropertyChanged("IgnoreMinions");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ForceKillClusterElites
        {
            get
            {
                return _forceKillClusterElites;
            }
            set
            {
                if (_forceKillClusterElites != value)
                {
                    _forceKillClusterElites = value;
                    OnPropertyChanged("ForceKillClusterElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool TryToSnapshot
        {
            get
            {
                return _tryToSnapshot;
            }
            set
            {
                if (_tryToSnapshot != value)
                {
                    _tryToSnapshot = value;
                    OnPropertyChanged("TryToSnapshot");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(4)]
        public double SnapshotAttackSpeed
        {
            get
            {
                return _snapshotAttackSpeed;
            }
            set
            {
                if (_snapshotAttackSpeed != value)
                {
                    _snapshotAttackSpeed = value;
                    OnPropertyChanged("SnapshotAttackSpeed");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0)]
        public double RiftValueIgnoreUnitsBelow
        {
            get
            {
                return _riftValueIgnoreUnitsBelow;
            }
            set
            {
                if (_riftValueIgnoreUnitsBelow != value)
                {
                    _riftValueIgnoreUnitsBelow = value;
                    OnPropertyChanged("RiftValueIgnoreBelow");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1)]
        public double RiftValueAlwaysKillUnitsAbove
        {
            get
            {
                return _riftValueAlwaysKillUnitsAbove;
            }
            set
            {
                if (_riftValueAlwaysKillUnitsAbove != value)
                {
                    _riftValueAlwaysKillUnitsAbove = value;
                    OnPropertyChanged("RiftValueAlwaysKillUnitsAbove");
                }
            }
        }

        //[DataMember(IsRequired = false)]
        //[DefaultValue(20)]
        //public double RiftValueAlwaysKillClusterByValueRadius
        //{
        //    get
        //    {
        //        return _riftValueAlwaysKillClusterByValueRadius;
        //    }
        //    set
        //    {
        //        if (_riftValueAlwaysKillClusterByValueRadius != value)
        //        {
        //            _riftValueAlwaysKillClusterByValueRadius = value;
        //            OnPropertyChanged("RiftValueAlwaysKillClusterByValueRadius");
        //        }
        //    }
        //}

        [DataMember(IsRequired = false)]
        [DefaultValue(20)]
        public double RiftValueAlwaysKillClusterValue
        {
            get
            {
                return _riftValueAlwaysKillClusterValue;
            }
            set
            {
                if (_riftValueAlwaysKillClusterValue != value)
                {
                    _riftValueAlwaysKillClusterValue = value;
                    OnPropertyChanged("RiftValueAlwaysKillClusterValue");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(98)]
        public double RiftProgressionAlwaysKillPct
        {
            get
            {
                return _riftProgressionAlwaysKillPct;
            }
            set
            {
                if (Math.Abs(_riftProgressionAlwaysKillPct - value) > float.Epsilon)
                {
                    _riftProgressionAlwaysKillPct = value;
                    OnPropertyChanged("RiftProgressionAlwaysKillPct");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1)]
        public int TrashPackSize
        {
            get
            {
                return _trashPackSize;
            }
            set
            {
                if (_trashPackSize != value)
                {
                    _trashPackSize = value;
                    OnPropertyChanged("TrashPackSize");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1)]
        public int TrashPackSizeMin
        {
            get
            {
                return _trashPackSizeMin;
            }
            set
            {
                if (_trashPackSizeMin != value)
                {
                    _trashPackSizeMin = value;
                    OnPropertyChanged("TrashPackSizeMin");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(40f)]
        public float TrashPackClusterRadius
        {
            get
            {
                return _TrashPackClusterRadius;
            }
            set
            {
                if (_TrashPackClusterRadius != value)
                {
                    _TrashPackClusterRadius = value;
                    OnPropertyChanged("TrashPackClusterRadius");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseNavMeshTargeting
        {
            get
            {
                return _UseNavMeshTargeting;
            }
            set
            {
                if (_UseNavMeshTargeting != value)
                {
                    _UseNavMeshTargeting = value;
                    OnPropertyChanged("UseNavMeshTargeting");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseConventionElementOnly
        {
            get
            {
                return _UseConventionElementOnly;
            }
            set
            {
                if (_UseConventionElementOnly != value)
                {
                    _UseConventionElementOnly = value;
                    OnPropertyChanged("UseConventionElementOnly");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreCoEunlessGRift
        {
            get
            {
                return _IgnoreCoEunlessGRift;
            }
            set
            {
                if (_IgnoreCoEunlessGRift != value)
                {
                    _IgnoreCoEunlessGRift = value;
                    OnPropertyChanged("IgnoreCoEunlessGRift");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool HiPriorityHG
        {
            get
            {
                return _HiPriorityHG;
            }
            set
            {
                if (_HiPriorityHG != value)
                {
                    _HiPriorityHG = value;
                    OnPropertyChanged("HiPriorityHG");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(GoblinPriority.Normal)]
        public GoblinPriority GoblinPriority
        {
            get
            {
                return _GoblinPriority;
            }
            set
            {
                if (_GoblinPriority != value)
                {
                    _GoblinPriority = value;
                    OnPropertyChanged("GoblinPriority");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(60)]
        public int NonEliteRange
        {
            get
            {
                return _NonEliteRange;
            }
            set
            {
                if (_NonEliteRange != value)
                {
                    _NonEliteRange = value;
                    OnPropertyChanged("NonEliteRange");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(150)]
        public int EliteRange
        {
            get
            {
                return _EliteRange;
            }
            set
            {
                if (_EliteRange != value)
                {
                    _EliteRange = value;
                    OnPropertyChanged("EliteRange");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ExtendedTrashKill
        {
            get
            {
                return _ExtendedTrashKill;
            }
            set
            {
                if (_ExtendedTrashKill != value)
                {
                    _ExtendedTrashKill = value;
                    OnPropertyChanged("ExtendedTrashKill");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AvoidAOE
        {
            get
            {
                return _AvoidAOE;
            }
            set
            {
                if (_AvoidAOE != value)
                {
                    _AvoidAOE = value;
                    OnPropertyChanged("AvoidAOE");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool KillMonstersInAoE
        {
            get
            {
                return _KillMonstersInAoE;
            }
            set
            {
                if (_KillMonstersInAoE != value)
                {
                    _KillMonstersInAoE = value;
                    OnPropertyChanged("KillMonstersInAoE");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool CollectHealthGlobe
        {
            get
            {
                return _CollectHealthGlobe;
            }
            set
            {
                if (_CollectHealthGlobe != value)
                {
                    _CollectHealthGlobe = value;
                    OnPropertyChanged("CollectHealthGlobe");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AllowOOCMovement
        {
            get
            {
                return _AllowOOCMovement;
            }
            set
            {
                if (_AllowOOCMovement != value)
                {
                    _AllowOOCMovement = value;
                    OnPropertyChanged("AllowOOCMovement");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AllowBacktracking
        {
            get
            {
                return _AllowBacktracking;
            }
            set
            {
                if (_AllowBacktracking != value)
                {
                    _AllowBacktracking = value;
                    OnPropertyChanged("AllowBacktracking");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(600)]
        public int DelayAfterKill
        {
            get
            {
                return _DelayAfterKill;
            }
            set
            {
                if (_DelayAfterKill != value)
                {
                    _DelayAfterKill = value;
                    OnPropertyChanged("DelayAfterKill");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreElites
        {
            get
            {
                if (!_IgnoreElites)
                {
                    _ProfileTagOverride = false;
                }
                return _IgnoreElites;
            }
            set
            {
                if (!_IgnoreElites)
                {
                    _ProfileTagOverride = false;
                }
                if (_IgnoreElites != value)
                {
                    _IgnoreElites = value;
                    OnPropertyChanged("IgnoreElites");
                    OnPropertyChanged("ProfileTagOverride");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ProfileTagOverride
        {
            get
            {
                if (!_IgnoreElites)
                {
                    _ProfileTagOverride = false;
                }

                return _ProfileTagOverride;
            }
            set
            {
                if (!_IgnoreElites)
                {
                    _ProfileTagOverride = false;
                }
                if (_ProfileTagOverride != value)
                {
                    _ProfileTagOverride = value;
                    OnPropertyChanged("IgnoreElites");
                    OnPropertyChanged("ProfileTagOverride");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AvoidDeath
        {
            get
            {
                return _AvoidDeath;
            }
            set
            {
                if (_AvoidDeath != value)
                {
                    _AvoidDeath = value;
                    OnPropertyChanged("AvoidDeath");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool SkipElitesOn5NV
        {
            get
            {
                return _SkipElitesOn5NV;
            }
            set
            {
                if (_SkipElitesOn5NV != value)
                {
                    _SkipElitesOn5NV = value;
                    OnPropertyChanged("SkipElitesOn5NV");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AvoidanceNavigation
        {
            get
            {
                return _AvoidanceNavigation;
            }
            set
            {
                if (_AvoidanceNavigation != value)
                {
                    _AvoidanceNavigation = value;
                    OnPropertyChanged("AvoidanceNavigation");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(0)]
        public double ForceKillTrashBelowHealth
        {
            get
            {
                return _ForceKillTrashBelowHealth;
            }
            set
            {
                if (_ForceKillTrashBelowHealth != value)
                {
                    _ForceKillTrashBelowHealth = value;
                    OnPropertyChanged("ForceKillTrashBelowHealth");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(0.25)]
        public double IgnoreTrashBelowHealthDoT
        {
            get
            {
                return _IgnoreTrashBelowHealthDoT;
            }
            set
            {
                if (_IgnoreTrashBelowHealthDoT != value)
                {
                    _IgnoreTrashBelowHealthDoT = value;
                    OnPropertyChanged("IgnoreTrashBelowHealthDoT");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseExperimentalSavageBeastAvoidance
        {
            get
            {
                return _UseExperimentalSavageBeastAvoidance;
            }
            set
            {
                if (_UseExperimentalSavageBeastAvoidance != value)
                {
                    _UseExperimentalSavageBeastAvoidance = value;
                    OnPropertyChanged("UseExperimentalSavageBeastAvoidance");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseExperimentalFireChainsAvoidance
        {
            get
            {
                return _UseExperimentalFireChainsAvoidance;
            }
            set
            {
                if (_UseExperimentalFireChainsAvoidance != value)
                {
                    _UseExperimentalFireChainsAvoidance = value;
                    OnPropertyChanged("UseExperimentalFireChainsAvoidance");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0f)]
        public float ForceKillElitesHealth
        {
            get
            {
                return _ForceKillElitesHealth;
            }
            set
            {
                if (_ForceKillElitesHealth != value)
                {
                    _ForceKillElitesHealth = value;
                    OnPropertyChanged("ForceKillElitesHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ForceKillSummoners
        {
            get
            {
                return _ForceKillSummoners;
            }
            set
            {
                if (_ForceKillSummoners != value)
                {
                    _ForceKillSummoners = value;
                    OnPropertyChanged("ForceKillSummoners");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AvoidAoEOutOfCombat
        {
            get
            {
                return _AvoidAoEOutOfCombat;
            }
            set
            {
                if (_AvoidAoEOutOfCombat != value)
                {
                    _AvoidAoEOutOfCombat = value;
                    OnPropertyChanged("AvoidAoEOutOfCombat");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool FleeInGhostMode
        {
            get
            {
                return _FleeInGhostMode;
            }
            set
            {
                if (_FleeInGhostMode != value)
                {
                    _FleeInGhostMode = value;
                    OnPropertyChanged("FleeInGhostMode");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreMonstersWhileReflectingDamage
        {
            get
            {
                return _ignoreMonstersWhileReflectingDamage;
            }
            set
            {
                if (_ignoreMonstersWhileReflectingDamage != value)
                {
                    _ignoreMonstersWhileReflectingDamage = value;
                    OnPropertyChanged("IgnoreMonstersWhileReflectingDamage");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreHighHitePointTrash
        {
            get
            {
                return _ignoreHighHitePointTrash;
            }
            set
            {
                if (_ignoreHighHitePointTrash != value)
                {
                    _ignoreHighHitePointTrash = value;
                    OnPropertyChanged("IgnoreHighHitePointTrash");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreRares
        {
            get
            {
                return _ignoreRares;
            }
            set
            {
                if (_ignoreRares != value)
                {
                    _ignoreRares = value;
                    OnPropertyChanged("IgnoreRares");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreChampions
        {
            get
            {
                return _ignoreChampions;
            }
            set
            {
                if (_ignoreChampions != value)
                {
                    _ignoreChampions = value;
                    OnPropertyChanged("IgnoreChampions");
                }
            }
        }


        public enum FollowerBossFightMode
        {
            None = 0,
            AlwaysAccept,
            DeclineInBounty,
            AlwaysDecline,
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(FollowerBossFightMode.DeclineInBounty)]
        public FollowerBossFightMode FollowerBossFightDialogMode
        {
            get
            {
                return _followerBossFightDialogMode;
            }
            set
            {
                if (_followerBossFightDialogMode != value)
                {
                    _followerBossFightDialogMode = value;
                    OnPropertyChanged("FollowerBossFightDialogMode");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnorePowerGlobes
        {
            get
            {
                return _ignorePowerGlobes;
            }
            set
            {
                if (_ignorePowerGlobes != value)
                {
                    _ignorePowerGlobes = value;
                    OnPropertyChanged("IgnorePowerGlobes");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool WaitForResInBossEncounters
        {
            get
            {
                return _waitForResInBossEncounters;
            }
            set
            {
                if (_waitForResInBossEncounters != value)
                {
                    _waitForResInBossEncounters = value;
                    OnPropertyChanged(nameof(WaitForResInBossEncounters));
                }
            }
        }

        private HashSet<TrinityMonsterAffix> _ignoreAffixes;
        public HashSet<TrinityMonsterAffix> IgnoreAffixes
        {
            get { return _ignoreAffixes; }
            set { SetField(ref _ignoreAffixes, value); }
        }

        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(MiscCombatSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public MiscCombatSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This will set default values for new settings if they were not present in the serialized XML (otherwise they will be the type defaults)
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing()]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            FollowerBossFightDialogMode = FollowerBossFightMode.DeclineInBounty;
            IgnoreMonstersWhileReflectingDamage = false;
            IgnoreHighHitePointTrash = false;
            IgnoreRares = false;
            UseNavMeshTargeting = true;
            TrashPackClusterRadius = 40f;
            TrashPackSize = 1;
            KillMonstersInAoE = true;
            EliteRange = 120;
            NonEliteRange = 60;
            SkipElitesOn5NV = false;
            AvoidanceNavigation = true;
            ForceKillTrashBelowHealth = 0;
            IgnoreTrashBelowHealthDoT = 0.50;
            UseExperimentalSavageBeastAvoidance = true;
            UseExperimentalFireChainsAvoidance = true;
            ForceKillElitesHealth = 0;
            ForceKillSummoners = true;
            AvoidAoEOutOfCombat = true;
            FleeInGhostMode = true;
            HiPriorityHG = false;
            SnapshotAttackSpeed = 4;
            TryToSnapshot = true;

            ForceKillClusterElites = false;
            RiftValueAlwaysKillClusterValue = 10;            
            RiftValueAlwaysKillUnitsAbove = 1;
            RiftValueIgnoreUnitsBelow = 0;
            RiftProgressionAlwaysKillPct = 98;
            IgnoreMinions = false;
            AttackWhenBlocked = true;
            IgnoreNormalProgressionGlobes = false;
            WaitForResInBossEncounters = false;
        }
        #endregion Methods

    }
}
