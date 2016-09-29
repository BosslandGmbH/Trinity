using System.ComponentModel;
using System.Runtime.Serialization;

namespace Trinity.Settings.Combat
{
    [DataContract(Namespace = "")]
    public class MonkSetting : ITrinitySetting<MonkSetting>, IAvoidanceHealth, INotifyPropertyChanged
    {
        #region Fields
        private int _exploadingPalmMaxMobMobCount;
        private float _staticChargeMaxMobMobPct;
        private TempestRushOption _TROption;
        private float _PotionLevel;
        private float _HealthGlobeLevel;
        private float _HealthGlobeLevelResource;
        private int _TR_MinSpirit;
        private int _TR_MinDist;
        private bool _DisableMantraSpam;
        private bool _TargetBasedZigZag;
        private bool _UseDashingStrikeOOC;
        private int _MinCycloneTrashCount;
        private int _MinWoLTrashCount;
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
        private float _AvoidMageFireHealth;
        private float _AvoidMaghdaProjectilleHealth;
        private float _AvoidMoltenBallHealth;
        private float _AvoidMoltenCoreHealth;
        private float _AvoidMoltenTrailHealth;
        private float _AvoidOrbiterHealth;
        private float _AvoidPlagueCloudHealth;
        private float _AvoidPlagueHandsHealth;
        private float _AvoidPoisonEnchantedHealth;
        private float _AvoidPoisonTreeHealth;
        private float _AvoidThunderstormHealth;
        private float _AvoidShamanFireHealth;
        private float _AvoidWallOfFireHealth;
        private float _AvoidWormholeHealth;
        private float _AvoidZoltBubbleHealth;
        private float _AvoidZoltTwisterHealth;
        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _disableExplodingPalm;
        private bool _primaryBeforeSSS;
        private bool _SSSOffCD;
        private float _dashingStrikeDelay;
        private bool _waitForCritChance;
        private float _cycloneStrikeDelay;
        private bool _useEpiphanyGoblin;
        private MonkEpiphanyMode _EpiphanyMode;
        private bool _epiphanyEmergencyHealth;
        private bool _alwaysInnerSanctury;
        private bool _alwaysBlindingFlash;
        private bool _dashingStrikeUseOOCDelay;
        private float _mantraDelay;
        private float _serenityHealthPct;
        private bool _breathOfHeavenOOC;


        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MonkSetting" /> class.
        /// </summary>
        public MonkSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool BreathOfHeavenOOC
        {
            get
            {
                return _breathOfHeavenOOC;
            }
            set
            {
                if (_breathOfHeavenOOC != value)
                {
                    _breathOfHeavenOOC = value;
                    OnPropertyChanged("BreathOfHeavenOOC");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.70f)]
        public float SerenityHealthPct
        {
            get
            {
                return _serenityHealthPct;
            }
            set
            {
                if (_serenityHealthPct != value)
                {
                    _serenityHealthPct = value;
                    OnPropertyChanged("SerenityHealthPct");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(2950f)]
        public float MantraDelay
        {
            get { return _mantraDelay; }
            set
            {
                if (_mantraDelay != value)
                {
                    _mantraDelay = value;
                    OnPropertyChanged("MantraDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool DashingStrikeUseOOCDelay
        {
            get
            {
                return _dashingStrikeUseOOCDelay;
            }
            set
            {
                if (_dashingStrikeUseOOCDelay != value)
                {
                    _dashingStrikeUseOOCDelay = value;
                    OnPropertyChanged("DashingStrikeUseOOCDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool EpiphanyEmergencyHealth
        {
            get
            {
                return _epiphanyEmergencyHealth;
            }
            set
            {
                if (_epiphanyEmergencyHealth != value)
                {
                    _epiphanyEmergencyHealth = value;
                    OnPropertyChanged("EpiphanyEmergencyHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(MonkEpiphanyMode.Normal)]
        public MonkEpiphanyMode EpiphanyMode
        {
            get
            {
                return _EpiphanyMode;
            }
            set
            {
                if (_EpiphanyMode != value)
                {
                    _EpiphanyMode = value;
                    OnPropertyChanged("EpiphanyMode");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseEpiphanyGoblin
        {
            get
            {
                return _useEpiphanyGoblin;
            }
            set
            {
                if (_useEpiphanyGoblin != value)
                {
                    _useEpiphanyGoblin = value;
                    OnPropertyChanged("UseEpiphanyGoblin");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool PrimaryBeforeSSS
        {
            get
            {
                return _primaryBeforeSSS;
            }
            set
            {
                if (_primaryBeforeSSS != value)
                {
                    _primaryBeforeSSS = value;
                    OnPropertyChanged("PrimaryBeforeSSS");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool SSSOffCD
        {
            get
            {
                return _SSSOffCD;
            }
            set
            {
                if (_SSSOffCD != value)
                {
                    _SSSOffCD = value;
                    OnPropertyChanged("SSSOffCD");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(500f)]
        public float DashingStrikeDelay
        {
            get
            {
                return _dashingStrikeDelay;
            }
            set
            {
                if (_dashingStrikeDelay != value)
                {
                    _dashingStrikeDelay = value;
                    OnPropertyChanged("DashingStrikeDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(3000f)]
        public float CycloneStrikeDelay
        {
            get { return _cycloneStrikeDelay; }
            set
            {
                if (_cycloneStrikeDelay != value)
                {
                    _cycloneStrikeDelay = value;
                    OnPropertyChanged("CycloneStrikeDelay");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool DisableExplodingPalm
        {
            get
            {
                return _disableExplodingPalm;
            }
            set
            {
                if (_disableExplodingPalm != value)
                {
                    _disableExplodingPalm = value;
                    OnPropertyChanged("DisableExplodingPalm");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool WaitForCritChanceBuff
        {
            get
            {
                return _waitForCritChance;
            }
            set
            {
                if (_waitForCritChance != value)
                {
                    _waitForCritChance = value;
                    OnPropertyChanged("WaitForCritChance");
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
        [DefaultValue(0.6f)]
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
        [DefaultValue(60)]
        public float TR_MinSpirit
        {
            get
            {
                return _TR_MinSpirit;
            }
            set
            {
                if (_TR_MinSpirit != value)
                {
                    _TR_MinSpirit = (int)value;
                    OnPropertyChanged("TR_MinSpirit");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10f)]
        public float TR_MinDist
        {
            get
            {
                return _TR_MinDist;
            }
            set
            {
                if (_TR_MinDist != value)
                {
                    _TR_MinDist = (int)value;
                    OnPropertyChanged("TR_MinDist");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(TempestRushOption.Always)]
        public TempestRushOption TROption
        {
            get
            {
                return _TROption;
            }
            set
            {
                if (_TROption != value)
                {
                    _TROption = value;
                    OnPropertyChanged("TROption");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool DisableMantraSpam
        {
            get
            {
                return _DisableMantraSpam;
            }
            set
            {
                if (_DisableMantraSpam != value)
                {
                    _DisableMantraSpam = value;
                    OnPropertyChanged("DisableMantraSpam");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool TargetBasedZigZag
        {
            get
            {
                return _TargetBasedZigZag;
            }
            set
            {
                if (_TargetBasedZigZag != value)
                {
                    _TargetBasedZigZag = value;
                    OnPropertyChanged("TargetBasedZigZag");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseDashingStrikeOOC
        {
            get
            {
                return _UseDashingStrikeOOC;
            }
            set
            {
                if (_UseDashingStrikeOOC != value)
                {
                    _UseDashingStrikeOOC = value;
                    OnPropertyChanged("UseDashingStrikeOOC");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(2)]
        public int MinCycloneTrashCount
        {
            get
            {
                return _MinCycloneTrashCount;
            }
            set
            {
                if (_MinCycloneTrashCount != value)
                {
                    _MinCycloneTrashCount = (int)value;
                    OnPropertyChanged("MinCycloneTrashCount");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(2)]
        public int MinWoLTrashCount
        {
            get
            {
                return _MinWoLTrashCount;
            }
            set
            {
                if (_MinWoLTrashCount != value)
                {
                    _MinWoLTrashCount = (int)value;
                    OnPropertyChanged("MinWoLTrashCount");
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
        [DefaultValue(1f)]
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
        [DefaultValue(0.65f)]
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
        [DefaultValue(0.65f)]
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
        [DefaultValue(0.35f)]
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
        [DefaultValue(0.85f)]
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
        [DefaultValue(0.85f)]
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
        [DefaultValue(0.85f)]
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
        [DefaultValue(0.75f)]
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
        [DefaultValue(0f)]
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
        [DefaultValue(0.75f)]
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
        [DefaultValue(0.5f)]
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
        [DefaultValue(1f)]
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
        [DefaultValue(0.5f)]
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
        [DefaultValue(1f)]
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
        [DefaultValue(1f)]
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
        [DefaultValue(0f)]
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
        [DefaultValue(1f)]
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
        [DefaultValue(2)]
        public int ExploadingPalmMaxMobCount
        {
            get
            {
                return _exploadingPalmMaxMobMobCount;
            }
            set
            {
                if (_exploadingPalmMaxMobMobCount != value)
                {
                    _exploadingPalmMaxMobMobCount = value;
                    OnPropertyChanged("ExploadingPalmMaxMobCount");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.5f)]
        public float StaticChargeMaxMobPct
        {
            get
            {
                return _staticChargeMaxMobMobPct;
            }
            set
            {
                if (_staticChargeMaxMobMobPct != value)
                {
                    _staticChargeMaxMobMobPct = value;
                    OnPropertyChanged("StaticChargeMaxMobPct");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AlwaysInnerSanctury
        {
            get
            {
                return _alwaysInnerSanctury;
            }
            set
            {
                if (_alwaysInnerSanctury != value)
                {
                    _alwaysInnerSanctury = value;
                    OnPropertyChanged("AlwaysInnerSanctury");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AlwaysBlindingFlash
        {
            get
            {
                return _alwaysBlindingFlash;
            }
            set
            {
                if (_alwaysBlindingFlash != value)
                {
                    _alwaysBlindingFlash = value;
                    OnPropertyChanged("AlwaysBlindingFlash");
                }
            }
        }

        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(MonkSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public MonkSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }

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
            TR_MinSpirit = 60;
            TR_MinDist = 10;
            TargetBasedZigZag = true;
            TROption = TempestRushOption.Always;
            MinCycloneTrashCount = 2;
            MinWoLTrashCount = 2;
            AvoidGrotesqueHealth = 1;
            AvoidOrbiterHealth = 1;
            AvoidWormholeHealth = 0.50f;
            ExploadingPalmMaxMobCount = 2;
            CycloneStrikeDelay = 3000;
            DashingStrikeDelay = 500;
            PrimaryBeforeSSS = true;
            SSSOffCD = false;
            DisableExplodingPalm = false;
            StaticChargeMaxMobPct = 0.5f;
            UseEpiphanyGoblin = false;
            EpiphanyMode = MonkEpiphanyMode.Normal;
            EpiphanyEmergencyHealth = false;
            AlwaysInnerSanctury = false;
            AlwaysBlindingFlash = false;
            DashingStrikeUseOOCDelay = false;
            MantraDelay = 2950;
            SerenityHealthPct = 0.5f;
            BreathOfHeavenOOC = false;
        }
        #endregion Methods






    }
}
