using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Trinity.Settings.Combat
{
    [DataContract(Namespace = "")]
    public class BarbarianSetting : ITrinitySetting<BarbarianSetting>, IAvoidanceHealth, INotifyPropertyChanged
    {
        #region Fields
        private float _PotionLevel;
        private float _HealthGlobeLevel;
        private float _HealthGlobeLevelResource;
        private int _KiteLimit;
        private bool _UseWOTBGoblin;
        private bool _FuryDumpWOTB;
        private bool _FuryDumpAlways;
        private bool _WOTBHardOnly;
        private bool _TargetBasedZigZag;
        private bool _ThreatShoutOOC;
        private int _MinThreatShoutMobCount;
        private bool _IgnoreAvoidanceInWOTB;
        private bool _IgnoreGoldInWOTB;
        private bool _ignorePainOffCooldown;
        private float _MinHotaHealth;
        private BarbarianWOTBMode _WOTBMode;
        private BarbarianSprintMode _SprintMode;
        private bool _UseLeapOOC;
        private bool _UseChargeOOC;
        private bool _WOTBEmergencyHealth;
        private int _rendWaitDelay;
        private int _ancientSpearWaitDelay;
        private int _threateningShoutWaitDelay;
        private int _warCryWaitDelay;
        private float _ignorePainMinHealthPct;

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
        public float _WWMoveStopDistance;
        private bool _WWMoveAlways;

        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BarbarianSetting" /> class.
        /// </summary>
        public BarbarianSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Common
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
        [DefaultValue(12f)]
        public float WWMoveStopDistance
        {
            get
            {
                return _WWMoveStopDistance;
            }
            set
            {
                if (_WWMoveStopDistance != value)
                {
                    _WWMoveStopDistance = value;
                    OnPropertyChanged("WWMoveStopDistance");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool WWMoveAlways
        {
            get
            {
                return _WWMoveAlways;
            }
            set
            {
                if (_WWMoveAlways != value)
                {
                    _WWMoveAlways = value;
                    OnPropertyChanged("WWMoveAlways");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(0.55f)]
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
        [DefaultValue(0.5f)]
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
        [DefaultValue(0.55f)]
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
        [DefaultValue(0.25f)]
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
        [DefaultValue(0.8f)]
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
        [DefaultValue(0.8f)]
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
        [DefaultValue(0.7f)]
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
        [DefaultValue(1f)]
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
        [DefaultValue(0.7f)]
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
#endregion

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(3500)]
        public int RendWaitDelay
        {
            get
            {
                return _rendWaitDelay;
            }
            set
            {
                if (_rendWaitDelay != value)
                {
                    _rendWaitDelay = value;
                    OnPropertyChanged("RendWaitDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(3500)]
        public int AncientSpearWaitDelay
        {
            get
            {
                return _ancientSpearWaitDelay;
            }
            set
            {
                if (_ancientSpearWaitDelay != value)
                {
                    _ancientSpearWaitDelay = value;
                    OnPropertyChanged("AncientSpearWaitDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(3500)]
        public int ThreateningShoutWaitDelay
        {
            get
            {
                return _threateningShoutWaitDelay;
            }
            set
            {
                if (_threateningShoutWaitDelay != value)
                {
                    _threateningShoutWaitDelay = value;
                    OnPropertyChanged("ThreateningShoutWaitDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(3500)]
        public int WarCryWaitDelay
        {
            get
            {
                return _warCryWaitDelay;
            }
            set
            {
                if (_warCryWaitDelay != value)
                {
                    _warCryWaitDelay = value;
                    OnPropertyChanged("WarCryWaitDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool WOTBEmergencyHealth
        {
            get
            {
                return _WOTBEmergencyHealth;
            }
            set
            {
                if (_WOTBEmergencyHealth != value)
                {
                    _WOTBEmergencyHealth = value;
                    OnPropertyChanged("WOTBEmergencyHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseWOTBGoblin
        {
            get
            {
                return _UseWOTBGoblin;
            }
            set
            {
                if (_UseWOTBGoblin != value)
                {
                    _UseWOTBGoblin = value;
                    OnPropertyChanged("UseWOTBGoblin");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool FuryDumpWOTB
        {
            get
            {
                return _FuryDumpWOTB;
            }
            set
            {
                if (_FuryDumpWOTB != value)
                {
                    _FuryDumpWOTB = value;
                    OnPropertyChanged("FuryDumpWOTB");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool FuryDumpAlways
        {
            get
            {
                return _FuryDumpAlways;
            }
            set
            {
                if (_FuryDumpAlways != value)
                {
                    _FuryDumpAlways = value;
                    OnPropertyChanged("FuryDumpAlways");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool WOTBHardOnly
        {
            get
            {
                return _WOTBHardOnly;
            }
            set
            {
                if (_WOTBHardOnly != value)
                {
                    _WOTBHardOnly = value;
                    OnPropertyChanged("WOTBHardOnly");
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
        [DefaultValue(true)]
        public bool ThreatShoutOOC
        {
            get
            {
                return _ThreatShoutOOC;
            }
            set
            {
                if (_ThreatShoutOOC != value)
                {
                    _ThreatShoutOOC = value;
                    OnPropertyChanged("ThreatShoutOOC");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1)]
        public int MinThreatShoutMobCount
        {
            get
            {
                return _MinThreatShoutMobCount;
            }
            set
            {
                if (_MinThreatShoutMobCount != value)
                {
                    _MinThreatShoutMobCount = value;
                    OnPropertyChanged("MinThreatShoutMobCount");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreAvoidanceInWOTB
        {
            get
            {
                return _IgnoreAvoidanceInWOTB;
            }
            set
            {
                if (_IgnoreAvoidanceInWOTB != value)
                {
                    _IgnoreAvoidanceInWOTB = value;
                    OnPropertyChanged("IgnoreAvoidanceInWOTB");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreGoldInWOTB
        {
            get
            {
                return _IgnoreGoldInWOTB;
            }
            set
            {
                if (_IgnoreGoldInWOTB != value)
                {
                    _IgnoreGoldInWOTB = value;
                    OnPropertyChanged("IgnoreGoldInWOTB");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(0.40f)]
        public float MinHotaHealth
        {
            get
            {
                return _MinHotaHealth;
            }
            set
            {
                if (_MinHotaHealth != value)
                {
                    _MinHotaHealth = value;
                    OnPropertyChanged("MinHotaHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0.40f)]
        public float IgnorePainMinHealthPct
        {
            get
            {
                return _ignorePainMinHealthPct;
            }
            set
            {
                if (_ignorePainMinHealthPct != value)
                {
                    _ignorePainMinHealthPct = value;
                    OnPropertyChanged("IgnorePainMinHealthPct");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(BarbarianWOTBMode.Normal)]
        public BarbarianWOTBMode WOTBMode
        {
            get
            {
                return _WOTBMode;
            }
            set
            {
                if (_WOTBMode != value)
                {
                    _WOTBMode = value;
                    OnPropertyChanged("WOTBMode");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseLeapOOC
        {
            get
            {
                return _UseLeapOOC;
            }
            set
            {
                if (_UseLeapOOC != value)
                {
                    _UseLeapOOC = value;
                    OnPropertyChanged("UseLeapOOC");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(BarbarianSprintMode.MovementOnly)]
        public BarbarianSprintMode SprintMode
        {
            get
            {
                return _SprintMode;
            }
            set
            {
                if (_SprintMode != value)
                {
                    _SprintMode = value;
                    OnPropertyChanged("SprintMode");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseChargeOOC
        {
            get
            {
                return _UseChargeOOC;
            }
            set
            {
                if (_UseChargeOOC != value)
                {
                    _UseChargeOOC = value;
                    OnPropertyChanged("UseChargeOOC");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnorePainOffCooldown
        {
            get
            {
                return _ignorePainOffCooldown;
            }
            set
            {
                if (_ignorePainOffCooldown != value)
                {
                    _ignorePainOffCooldown = value;
                    OnPropertyChanged("IgnorePainOffCooldown");
                }
            }
        }


        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(BarbarianSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public BarbarianSetting Clone()
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
            foreach (var p in GetType().GetProperties())
            {
                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
                {
                    p.SetValue(this, dv.Value);
                }
            }
        }

        /// <summary>
        /// This will run after the settings have been deserialized - used for migrating settings. Setting anything here will overwrite any user settings!
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (WOTBHardOnly)
            {
                WOTBMode = BarbarianWOTBMode.HardElitesOnly;
                WOTBHardOnly = false;
            }
        }
        #endregion Methods
    }
}
