using System.ComponentModel;
using System.Runtime.Serialization;

namespace Trinity.Config.Combat
{
    [DataContract(Namespace = "")]
    public class WizardSetting : ITrinitySetting<WizardSetting>, IAvoidanceHealth, INotifyPropertyChanged
    {
        #region Fields
        private float _PotionLevel;
        private float _HealthGlobeLevel;
        private float _HealthGlobeLevelResource;
        private int _KiteLimit;
        private bool _NoEnergyTwister;
        private bool _NoArcaneStrike;
        private bool _NoArcaneBlast;
        private bool _DisableDisintegrationWave;
        private bool _ArchonElitesOnly;
        private int _ArchonMobCount;
        private int _BlackHoleAoECount;
        private float _SafePassageHealthPct;
        private bool _TeleportOOC;
        private float _TeleportDelay;
        private bool _TeleportUseOOCDelay;
        private float _ArchonMobDistance;
        private float _ArchonEliteDistance;
        private float _teleportDelay;
        private WizardKiteOption _KiteOption;
        private WizardArchonCancelOption _ArchonCancelOption;
        private int _ArchonCancelSeconds;
        private float _AvoidArcaneHealth;
        private float _AvoidAzmoBodiesHealth;
        private float _AvoidAzmoFireBallHealth;
        private float _AvoidAzmoPoolsHealth;
        private float _AvoidBeesWaspsHealth;
        private float _AvoidBelialHealth;
        private float _AvoidButcherFloorPanelHealth;
        private float _AvoidDesecratorHealth;
        private float _AvoidDiabloMeteorHealth;
        private float _AvoidDiabloPrisonHealth;
        private float _AvoidDiabloRingOfFireHealth;
        private float _AvoidFrozenPulseHealth;
        private float _AvoidGhomGasHealth;
        private float _AvoidGrotesqueHealth;
        private float _AvoidIceBallsHealth;
        private float _AvoidIceTrailHealth;
        private float _AvoidOrbiterHealth;
        private float _AvoidMageFireHealth;
        private float _AvoidMaghdaProjectilleHealth;
        private float _AvoidMoltenBallHealth;
        private float _AvoidMoltenCoreHealth;
        private float _AvoidMoltenTrailHealth;
        private float _AvoidPlagueCloudHealth;
        private float _AvoidPlagueHandsHealth;
        private float _AvoidPoisonEnchantedHealth;
        private float _AvoidPoisonTreeHealth;
        private float _AvoidShamanFireHealth;
        private float _AvoidThunderstormHealth;
        private float _AvoidWallOfFireHealth;
        private float _AvoidWormholeHealth;
        private float _AvoidZoltBubbleHealth;
        private float _AvoidZoltTwisterHealth;
        private bool _alwaysExplosiveBlast;
        private bool _findClustersWhenNotArchon;
        private bool _alwaysArchon;

        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardSetting" /> class.
        /// </summary>
        public WizardSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool TeleportOOC
        {
            get
            {
                return _TeleportOOC;
            }
            set
            {
                if (_TeleportOOC != value)
                {
                    _TeleportOOC = value;
                    OnPropertyChanged("TeleportOOC");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(500f)]
        public float TeleportDelay
        {
            get
            {
                return _TeleportDelay;
            }
            set
            {
                if (_TeleportDelay != value)
                {
                    _TeleportDelay = value;
                    OnPropertyChanged("TeleportDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool TeleportUseOOCDelay
        {
            get
            {
                return _TeleportUseOOCDelay;
            }
            set
            {
                if (_TeleportUseOOCDelay != value)
                {
                    _TeleportUseOOCDelay = value;
                    OnPropertyChanged("TeleportUseOOCDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ArchonElitesOnly
        {
            get
            {
                return _ArchonElitesOnly;
            }
            set
            {
                if (_ArchonElitesOnly != value)
                {
                    _ArchonElitesOnly = value;
                    OnPropertyChanged("ArchonElitesOnly");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool NoEnergyTwister
        {
            get
            {
                return _NoEnergyTwister;
            }
            set
            {
                if (_NoEnergyTwister != value)
                {
                    _NoEnergyTwister = value;
                    OnPropertyChanged("NoEnergyTwister");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool NoArcaneStrike
        {
            get
            {
                return _NoArcaneStrike;
            }
            set
            {
                if (_NoArcaneStrike != value)
                {
                    _NoArcaneStrike = value;
                    OnPropertyChanged("NoArcaneStrike");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool NoArcaneBlast
        {
            get
            {
                return _NoArcaneBlast;
            }
            set
            {
                if (_NoArcaneBlast != value)
                {
                    _NoArcaneBlast = value;
                    OnPropertyChanged("NoArcaneBlast");
                }
            }
        }

		[DataMember(IsRequired = false)]
        [DefaultValue(false)]
		public bool DisableDisintegrationWave
        {
            get
            {
                return _DisableDisintegrationWave;
            }
            set
            {
                if (_DisableDisintegrationWave != value)
                {
                    _DisableDisintegrationWave = value;
                    OnPropertyChanged("DisableDisintegrationWave");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(2)]
        public int ArchonMobCount
        {
            get
            {
                return _ArchonMobCount;
            }
            set
            {
                if (_ArchonMobCount != value)
                {
                    _ArchonMobCount = value;
                    OnPropertyChanged("ArchonMobCount");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(2)]
        public int BlackHoleAoECount
        {
            get
            {
                return _BlackHoleAoECount;
            }
            set
            {
                if (_BlackHoleAoECount != value)
                {
                    _BlackHoleAoECount = value;
                    OnPropertyChanged("BlackHoleAoECount");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float SafePassageHealthPct
        {
            get
            {
                return _SafePassageHealthPct;
            }
            set
            {
                if (_SafePassageHealthPct != value)
                {
                    _SafePassageHealthPct = value;
                    OnPropertyChanged("SafePassageHealthPct");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(35f)]
        public float ArchonMobDistance
        {
            get
            {
                return _ArchonMobDistance;
            }
            set
            {
                if (_ArchonMobDistance != value)
                {
                    _ArchonMobDistance = value;
                    OnPropertyChanged("ArchonMobDistance");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(15f)]
        public float ArchonEliteDistance
        {
            get
            {
                return _ArchonEliteDistance;
            }
            set
            {
                if (_ArchonEliteDistance != value)
                {
                    _ArchonEliteDistance = value;
                    OnPropertyChanged("ArchonEliteDistance");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(300)]
        public int ArchonCancelSeconds
        {
            get
            {
                return _ArchonCancelSeconds;
            }
            set
            {
                if (_ArchonCancelSeconds != value)
                {
                    _ArchonCancelSeconds = value;
                    OnPropertyChanged("ArchonCancelSeconds");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(WizardArchonCancelOption.Never)]
        public WizardArchonCancelOption ArchonCancelOption
        {
            get
            {
                return _ArchonCancelOption;
            }
            set
            {
                if (_ArchonCancelOption != value)
                {
                    _ArchonCancelOption = value;
                    OnPropertyChanged("ArchonCancelOption");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(WizardKiteOption.Anytime)]
        public WizardKiteOption KiteOption
        {
            get
            {
                return _KiteOption;
            }
            set
            {
                if (_KiteOption != value)
                {
                    _KiteOption = value;
                    OnPropertyChanged("KiteOption");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.35f)]
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
        [DefaultValue(0.8f)]
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
        [DefaultValue(0)]
        public int KiteLimit
        {
            get
            {
                return _KiteLimit;
            }
            set
            {
                if (_KiteLimit != value)
                {
                    _KiteLimit = value;
                    OnPropertyChanged("KiteLimit");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidArcaneHealth
        {
            get
            {
                return _AvoidArcaneHealth;
            }
            set
            {
                if (_AvoidArcaneHealth != value)
                {
                    _AvoidArcaneHealth = value;
                    OnPropertyChanged("AvoidArcaneHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.8f)]
        public float AvoidDesecratorHealth
        {
            get
            {
                return _AvoidDesecratorHealth;
            }
            set
            {
                if (_AvoidDesecratorHealth != value)
                {
                    _AvoidDesecratorHealth = value;
                    OnPropertyChanged("AvoidDesecratorHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidMoltenCoreHealth
        {
            get
            {
                return _AvoidMoltenCoreHealth;
            }
            set
            {
                if (_AvoidMoltenCoreHealth != value)
                {
                    _AvoidMoltenCoreHealth = value;
                    OnPropertyChanged("AvoidMoltenCoreHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.9f)]
        public float AvoidMoltenTrailHealth
        {
            get
            {
                return _AvoidMoltenTrailHealth;
            }
            set
            {
                if (_AvoidMoltenTrailHealth != value)
                {
                    _AvoidMoltenTrailHealth = value;
                    OnPropertyChanged("AvoidMoltenTrailHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.9f)]
        public float AvoidPoisonTreeHealth
        {
            get
            {
                return _AvoidPoisonTreeHealth;
            }
            set
            {
                if (_AvoidPoisonTreeHealth != value)
                {
                    _AvoidPoisonTreeHealth = value;
                    OnPropertyChanged("AvoidPoisonTreeHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.9f)]
        public float AvoidPoisonEnchantedHealth
        {
            get
            {
                return _AvoidPoisonEnchantedHealth;
            }
            set
            {
                if (_AvoidPoisonEnchantedHealth != value)
                {
                    _AvoidPoisonEnchantedHealth = value;
                    OnPropertyChanged("AvoidPoisonEnchantedHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidGrotesqueHealth
        {
            get
            {
                return _AvoidGrotesqueHealth;
            }
            set
            {
                if (_AvoidGrotesqueHealth != value)
                {
                    _AvoidGrotesqueHealth = value;
                    OnPropertyChanged("AvoidGrotesqueHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.9f)]
        public float AvoidPlagueCloudHealth
        {
            get
            {
                return _AvoidPlagueCloudHealth;
            }
            set
            {
                if (_AvoidPlagueCloudHealth != value)
                {
                    _AvoidPlagueCloudHealth = value;
                    OnPropertyChanged("AvoidPlagueCloudHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.9f)]
        public float AvoidIceBallsHealth
        {
            get
            {
                return _AvoidIceBallsHealth;
            }
            set
            {
                if (_AvoidIceBallsHealth != value)
                {
                    _AvoidIceBallsHealth = value;
                    OnPropertyChanged("AvoidIceBallsHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidPlagueHandsHealth
        {
            get
            {
                return _AvoidPlagueHandsHealth;
            }
            set
            {
                if (_AvoidPlagueHandsHealth != value)
                {
                    _AvoidPlagueHandsHealth = value;
                    OnPropertyChanged("AvoidPlagueHandsHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.25f)]
        public float AvoidBeesWaspsHealth
        {
            get
            {
                return _AvoidBeesWaspsHealth;
            }
            set
            {
                if (_AvoidBeesWaspsHealth != value)
                {
                    _AvoidBeesWaspsHealth = value;
                    OnPropertyChanged("AvoidBeesWaspsHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidAzmoPoolsHealth
        {
            get
            {
                return _AvoidAzmoPoolsHealth;
            }
            set
            {
                if (_AvoidAzmoPoolsHealth != value)
                {
                    _AvoidAzmoPoolsHealth = value;
                    OnPropertyChanged("AvoidAzmoPoolsHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidAzmoBodiesHealth
        {
            get
            {
                return _AvoidAzmoBodiesHealth;
            }
            set
            {
                if (_AvoidAzmoBodiesHealth != value)
                {
                    _AvoidAzmoBodiesHealth = value;
                    OnPropertyChanged("AvoidAzmoBodiesHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.85f)]
        public float AvoidShamanFireHealth
        {
            get
            {
                return _AvoidShamanFireHealth;
            }
            set
            {
                if (_AvoidShamanFireHealth != value)
                {
                    _AvoidShamanFireHealth = value;
                    OnPropertyChanged("AvoidShamanFireHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.25f)]
        public float AvoidFrozenPulseHealth
        {
            get
            {
                return _AvoidFrozenPulseHealth;
            }
            set
            {
                if (_AvoidFrozenPulseHealth != value)
                {
                    _AvoidFrozenPulseHealth = value;
                    OnPropertyChanged("AvoidFrozenPulseHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.60f)]
        public float AvoidGhomGasHealth
        {
            get
            {
                return _AvoidGhomGasHealth;
            }
            set
            {
                if (_AvoidGhomGasHealth != value)
                {
                    _AvoidGhomGasHealth = value;
                    OnPropertyChanged("AvoidGhomGasHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidAzmoFireBallHealth
        {
            get
            {
                return _AvoidAzmoFireBallHealth;
            }
            set
            {
                if (_AvoidAzmoFireBallHealth != value)
                {
                    _AvoidAzmoFireBallHealth = value;
                    OnPropertyChanged("AvoidAzmoFireBallHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidBelialHealth
        {
            get
            {
                return _AvoidBelialHealth;
            }
            set
            {
                if (_AvoidBelialHealth != value)
                {
                    _AvoidBelialHealth = value;
                    OnPropertyChanged("AvoidBelialHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1f)]
        public float AvoidButcherFloorPanelHealth
        {
            get
            {
                return _AvoidButcherFloorPanelHealth;
            }
            set
            {
                if (_AvoidButcherFloorPanelHealth != value)
                {
                    _AvoidButcherFloorPanelHealth = value;
                    OnPropertyChanged("AvoidButcherFloorPanelHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.8f)]
        public float AvoidDiabloMeteorHealth
        {
            get
            {
                return _AvoidDiabloMeteorHealth;
            }
            set
            {
                if (_AvoidDiabloMeteorHealth != value)
                {
                    _AvoidDiabloMeteorHealth = value;
                    OnPropertyChanged("AvoidDiabloMeteorHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.9f)]
        public float AvoidDiabloPrisonHealth
        {
            get
            {
                return _AvoidDiabloPrisonHealth;
            }
            set
            {
                if (_AvoidDiabloPrisonHealth != value)
                {
                    _AvoidDiabloPrisonHealth = value;
                    OnPropertyChanged("AvoidDiabloPrisonHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.9f)]
        public float AvoidDiabloRingOfFireHealth
        {
            get
            {
                return _AvoidDiabloRingOfFireHealth;
            }
            set
            {
                if (_AvoidDiabloRingOfFireHealth != value)
                {
                    _AvoidDiabloRingOfFireHealth = value;
                    OnPropertyChanged("AvoidDiabloRingOfFireHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.8f)]
        public float AvoidIceTrailHealth
        {
            get
            {
                return _AvoidIceTrailHealth;
            }
            set
            {
                if (_AvoidIceTrailHealth != value)
                {
                    _AvoidIceTrailHealth = value;
                    OnPropertyChanged("AvoidIceTrailHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.8f)]
        public float AvoidOrbiterHealth
        {
            get
            {
                return _AvoidOrbiterHealth;
            }
            set
            {
                if (_AvoidOrbiterHealth != value)
                {
                    _AvoidOrbiterHealth = value;
                    OnPropertyChanged("AvoidOrbiterHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.3f)]
        public float AvoidMageFireHealth
        {
            get
            {
                return _AvoidMageFireHealth;
            }
            set
            {
                if (_AvoidMageFireHealth != value)
                {
                    _AvoidMageFireHealth = value;
                    OnPropertyChanged("AvoidMageFireHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.7f)]
        public float AvoidMaghdaProjectilleHealth
        {
            get
            {
                return _AvoidMaghdaProjectilleHealth;
            }
            set
            {
                if (_AvoidMaghdaProjectilleHealth != value)
                {
                    _AvoidMaghdaProjectilleHealth = value;
                    OnPropertyChanged("AvoidMaghdaProjectilleHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.1f)]
        public float AvoidMoltenBallHealth
        {
            get
            {
                return _AvoidMoltenBallHealth;
            }
            set
            {
                if (_AvoidMoltenBallHealth != value)
                {
                    _AvoidMoltenBallHealth = value;
                    OnPropertyChanged("AvoidMoltenBallHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.5f)]
        public float AvoidWallOfFireHealth
        {
            get
            {
                return _AvoidWallOfFireHealth;
            }
            set
            {
                if (_AvoidWallOfFireHealth != value)
                {
                    _AvoidWallOfFireHealth = value;
                    OnPropertyChanged("AvoidWallOfFireHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.5f)]
        public float AvoidWormholeHealth
        {
            get
            {
                return _AvoidWormholeHealth;
            }
            set
            {
                if (_AvoidWormholeHealth != value)
                {
                    _AvoidWormholeHealth = value;
                    OnPropertyChanged("AvoidWormholeHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.6f)]
        public float AvoidZoltBubbleHealth
        {
            get
            {
                return _AvoidZoltBubbleHealth;
            }
            set
            {
                if (_AvoidZoltBubbleHealth != value)
                {
                    _AvoidZoltBubbleHealth = value;
                    OnPropertyChanged("AvoidZoltBubbleHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.7f)]
        public float AvoidZoltTwisterHealth
        {
            get
            {
                return _AvoidZoltTwisterHealth;
            }
            set
            {
                if (_AvoidZoltTwisterHealth != value)
                {
                    _AvoidZoltTwisterHealth = value;
                    OnPropertyChanged("AvoidZoltTwisterHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.50f)]
        public float AvoidThunderstormHealth
        {
            get
            {
                return _AvoidThunderstormHealth;
            }
            set
            {
                if (_AvoidThunderstormHealth != value)
                {
                    _AvoidThunderstormHealth = value;
                    OnPropertyChanged("AvoidThunderstormHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AlwaysExplosiveBlast
        {
            get
            {
                return _alwaysExplosiveBlast;
            }
            set
            {
                if (_alwaysExplosiveBlast != value)
                {
                    _alwaysExplosiveBlast = value;
                    OnPropertyChanged(nameof(AlwaysExplosiveBlast));
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AlwaysArchon
        {
            get
            {
                return _alwaysArchon;
            }
            set
            {
                if (_alwaysArchon != value)
                {
                    _alwaysArchon = value;
                    OnPropertyChanged(nameof(AlwaysArchon));
                }
            }
        }

        

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool FindClustersWhenNotArchon
        {
            get
            {
                return _findClustersWhenNotArchon;
            }
            set
            {
                if (_findClustersWhenNotArchon != value)
                {
                    _findClustersWhenNotArchon = value;
                    OnPropertyChanged(nameof(FindClustersWhenNotArchon));
                }
            }
        }

        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(WizardSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public WizardSetting Clone()
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
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            ArchonMobDistance = 35;
            ArchonMobCount = 3;
            ArchonCancelSeconds = 300;
            ArchonCancelOption = WizardArchonCancelOption.RebuffMagicWeaponFamiliar;
            AvoidGrotesqueHealth = 1;
            AvoidOrbiterHealth = 1;
            AvoidWormholeHealth = 0.50f;
            ArchonElitesOnly = true;
            ArchonEliteDistance = 15f;
            BlackHoleAoECount = 2;
            TeleportOOC = false;
            TeleportDelay = 500f;
            TeleportUseOOCDelay = false;
            SafePassageHealthPct = 1f;
            NoEnergyTwister = false;            
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ArchonCancelOption = WizardArchonCancelOption.Never;
        }

        #endregion Methods
    }
}
