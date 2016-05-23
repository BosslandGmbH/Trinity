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
        private bool _ignoreHighHitPointTrash;
        #region Elite Affixes
        private bool _ignoreHighHitPointElites;
        private bool _ignoreArcaneElites;
        private bool _ignoreAvengerElites;
        private bool _ignoreDesecratorElites;
        private bool _ignoreElectrifiedElites;
        private bool _ignoreExtraHealthElites;
        private bool _ignoreFastElites;
        private bool _ignoreFireChainsElites;
        private bool _ignoreFrozenElites;
        private bool _ignoreFrozenPulseElites;
        private bool _ignoreHealthLinkElites;
        private bool _ignoreHordeElites;
        private bool _ignoreIllusionistElites;
        private bool _ignoreJailerElites;
        private bool _ignoreKnockbackElites;
        private bool _ignoreMissileDampeningElites;
        private bool _ignoreMoltenElites;
        private bool _ignoreMortarElites;
        private bool _ignoreNightmarishElites;
        private bool _ignoreOrbiterElites;
        private bool _ignorePlaguedElites;
        private bool _ignorePoisonEnchantedElites;
        private bool _ignoreShieldingElites;
        private bool _ignoreTeleporterElites;
        private bool _ignoreThunderstormElites;
        private bool _ignoreVampiricElites;
        private bool _ignoreVortexElites;
        private bool _ignoreWallerElites;
        private bool _ignoreWormholeElites;
        #endregion
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
        public bool IgnoreHighHitPointTrash
        {
            get
            {
                return _ignoreHighHitPointTrash;
            }
            set
            {
                if (_ignoreHighHitPointTrash != value)
                {
                    _ignoreHighHitPointTrash = value;
                    OnPropertyChanged("IgnoreHighHitPointTrash");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreArcaneElites
        {
            get
            {
                return _ignoreArcaneElites;
            }
            set
            {
                if (_ignoreArcaneElites != value)
                {
                    _ignoreArcaneElites = value;
                    OnPropertyChanged("IgnoreArcaneElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreAvengerElites
        {
            get
            {
                return _ignoreAvengerElites;
            }
            set
            {
                if (_ignoreAvengerElites != value)
                {
                    _ignoreAvengerElites = value;
                    OnPropertyChanged("IgnoreAvengerElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreDesecratorElites
        {
            get
            {
                return _ignoreDesecratorElites;
            }
            set
            {
                if (_ignoreDesecratorElites != value)
                {
                    _ignoreDesecratorElites = value;
                    OnPropertyChanged("IgnoreDesecratorElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreElectrifiedElites
        {
            get
            {
                return _ignoreElectrifiedElites;
            }
            set
            {
                if (_ignoreElectrifiedElites != value)
                {
                    _ignoreElectrifiedElites = value;
                    OnPropertyChanged("IgnoreElectrifiedElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreExtraHealthElites
        {
            get
            {
                return _ignoreExtraHealthElites;
            }
            set
            {
                if (_ignoreExtraHealthElites != value)
                {
                    _ignoreExtraHealthElites = value;
                    OnPropertyChanged("IgnoreExtraHealthElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreFastElites
        {
            get
            {
                return _ignoreFastElites;
            }
            set
            {
                if (_ignoreFastElites != value)
                {
                    _ignoreFastElites = value;
                    OnPropertyChanged("IgnoreFastElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreFireChainsElites
        {
            get
            {
                return _ignoreFireChainsElites;
            }
            set
            {
                if (_ignoreFireChainsElites != value)
                {
                    _ignoreFireChainsElites = value;
                    OnPropertyChanged("IgnoreFireChainsElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreFrozenElites
        {
            get
            {
                return _ignoreFrozenElites;
            }
            set
            {
                if (_ignoreFrozenElites != value)
                {
                    _ignoreHighHitPointElites = value;
                    OnPropertyChanged("IgnoreFrozenElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreFrozenPulseElites
        {
            get
            {
                return _ignoreFrozenPulseElites;
            }
            set
            {
                if (_ignoreFrozenPulseElites != value)
                {
                    _ignoreFrozenPulseElites = value;
                    OnPropertyChanged("IgnoreHighHitPointElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreHealthLinkElites
        {
            get
            {
                return _ignoreHealthLinkElites;
            }
            set
            {
                if (_ignoreHealthLinkElites != value)
                {
                    _ignoreHealthLinkElites = value;
                    OnPropertyChanged("IgnoreHealthLinkElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreHordeElites
        {
            get
            {
                return _ignoreHordeElites;
            }
            set
            {
                if (_ignoreHordeElites != value)
                {
                    _ignoreHordeElites = value;
                    OnPropertyChanged("IgnoreHordeElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreIllusionistElites
        {
            get
            {
                return _ignoreIllusionistElites;
            }
            set
            {
                if (_ignoreIllusionistElites != value)
                {
                    _ignoreIllusionistElites = value;
                    OnPropertyChanged("IgnoreIllusionistElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreJailerElites
        {
            get
            {
                return _ignoreJailerElites;
            }
            set
            {
                if (_ignoreJailerElites != value)
                {
                    _ignoreJailerElites = value;
                    OnPropertyChanged("IgnoreJailerElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreKnockbackElites
        {
            get
            {
                return _ignoreKnockbackElites;
            }
            set
            {
                if (_ignoreKnockbackElites != value)
                {
                    _ignoreKnockbackElites = value;
                    OnPropertyChanged("IgnoreKnockbackElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreMissileDampeningElites
        {
            get
            {
                return _ignoreMissileDampeningElites;
            }
            set
            {
                if (_ignoreMissileDampeningElites != value)
                {
                    _ignoreMissileDampeningElites = value;
                    OnPropertyChanged("IgnoreMissileDampeningElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreMoltenElites
        {
            get
            {
                return _ignoreMoltenElites;
            }
            set
            {
                if (_ignoreMoltenElites != value)
                {
                    _ignoreMoltenElites = value;
                    OnPropertyChanged("IgnoreMoltenElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreMortarElites
        {
            get
            {
                return _ignoreMortarElites;
            }
            set
            {
                if (_ignoreMortarElites != value)
                {
                    _ignoreMortarElites = value;
                    OnPropertyChanged("IgnoreMortarElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreNightmarishElites
        {
            get
            {
                return _ignoreNightmarishElites;
            }
            set
            {
                if (_ignoreNightmarishElites != value)
                {
                    _ignoreNightmarishElites = value;
                    OnPropertyChanged("IgnoreNightmarishElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreOrbiterElites
        {
            get
            {
                return _ignoreOrbiterElites;
            }
            set
            {
                if (_ignoreOrbiterElites != value)
                {
                    _ignoreOrbiterElites = value;
                    OnPropertyChanged("IgnoreOrbiterElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnorePlaguedElites
        {
            get
            {
                return _ignorePlaguedElites;
            }
            set
            {
                if (_ignorePlaguedElites != value)
                {
                    _ignorePlaguedElites = value;
                    OnPropertyChanged("IgnorePlaguedElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnorePoisonEnchantedElites
        {
            get
            {
                return _ignorePoisonEnchantedElites;
            }
            set
            {
                if (_ignorePoisonEnchantedElites != value)
                {
                    _ignorePoisonEnchantedElites = value;
                    OnPropertyChanged("IgnorePosionEnchantedElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreShieldingElites
        {
            get
            {
                return _ignoreShieldingElites;
            }
            set
            {
                if (_ignoreShieldingElites != value)
                {
                    _ignoreShieldingElites = value;
                    OnPropertyChanged("IgnoreShieldingElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreTeleporterElites
        {
            get
            {
                return _ignoreTeleporterElites;
            }
            set
            {
                if (_ignoreTeleporterElites != value)
                {
                    _ignoreTeleporterElites = value;
                    OnPropertyChanged("IgnoreTeleporterElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreThunderstormElites
        {
            get
            {
                return _ignoreThunderstormElites;
            }
            set
            {
                if (_ignoreThunderstormElites != value)
                {
                    _ignoreThunderstormElites = value;
                    OnPropertyChanged("IgnoreThunderstormElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreVampiricElites
        {
            get
            {
                return _ignoreVampiricElites;
            }
            set
            {
                if (_ignoreVampiricElites != value)
                {
                    _ignoreVampiricElites = value;
                    OnPropertyChanged("IgnoreVampiricElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreVortexElites
        {
            get
            {
                return _ignoreVortexElites;
            }
            set
            {
                if (_ignoreVortexElites != value)
                {
                    _ignoreVortexElites = value;
                    OnPropertyChanged("IgnoreVortexElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreWallerElites
        {
            get
            {
                return _ignoreWallerElites;
            }
            set
            {
                if (_ignoreWallerElites != value)
                {
                    _ignoreWallerElites = value;
                    OnPropertyChanged("IgnoreWallerElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreWormholeElites
        {
            get
            {
                return _ignoreWormholeElites;
            }
            set
            {
                if (_ignoreWormholeElites != value)
                {
                    _ignoreWormholeElites = value;
                    OnPropertyChanged("IgnoreWormholeElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreHighHitPointElites
        {
            get
            {
                return _ignoreHighHitPointElites;
            }
            set
            {
                if (_ignoreHighHitPointElites != value)
                {
                    _ignoreHighHitPointElites = value;
                    OnPropertyChanged("IgnoreHighHitPointElites");
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
            IgnoreHighHitPointTrash = false;
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
