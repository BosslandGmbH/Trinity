using System.ComponentModel;
using System.Runtime.Serialization;

namespace Trinity.Config.Combat
{
    [DataContract(Namespace = "")]
    public class CrusaderSetting : ITrinitySetting<CrusaderSetting>, IAvoidanceHealth, INotifyPropertyChanged
    {
        #region Fields
        private bool _SpamAkarats;
        private bool _SpamLaws;
        private int _TauntAoECount;
        private int _SweepAttackAoECount;
        private int _JudgmentAoECount;
        private int _ShieldGlareAoECount;
        private double _IronSkinHpPct;
        private double _ConsecrationHpPct;
        private double _LawsOfHopeHpPct;
        private double _LawsOfJusticeHpPct;
        private int _BombardmentAoECount;
        private int _FallingSwordAoECount;
        private int _HeavensFuryAoECount;
        private int _CondemnAoECount;
        private int _CondemnRange;
        private bool _SteedChargeOOC;
        private float _BlessedHammerRange;
        private float _FistOfHeavensDist;
        private float _PotionLevel;
        private float _HealthGlobeLevel;
        private float _HealthGlobeLevelResource;
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
        private bool _spamSteedCharge;
        private bool _useAkaratsGoblin;
        private CrusaderAkaratsMode _akaratsMode;
        private bool _akaratsEmergencyHealth;

        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CrusaderSetting" /> class.
        /// </summary>
        public CrusaderSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool SpamSteedCharge
        {
            get
            {
                return _spamSteedCharge;
            }
            set
            {
                if (_spamSteedCharge != value)
                {
                    _spamSteedCharge = value;
                    OnPropertyChanged("SpamSteedCharge");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AkaratsEmergencyHealth
        {
            get
            {
                return _akaratsEmergencyHealth;
            }
            set
            {
                if (_akaratsEmergencyHealth != value)
                {
                    _akaratsEmergencyHealth = value;
                    OnPropertyChanged("AkaratsEmergencyHealth");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(CrusaderAkaratsMode.Normal)]
        public CrusaderAkaratsMode AkaratsMode
        {
            get
            {
                return _akaratsMode;
            }
            set
            {
                if (_akaratsMode != value)
                {
                    _akaratsMode = value;
                    OnPropertyChanged("AkaratsMode");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseAkaratsGoblin
        {
            get
            {
                return _useAkaratsGoblin;
            }
            set
            {
                if (_useAkaratsGoblin != value)
                {
                    _useAkaratsGoblin = value;
                    OnPropertyChanged("UseAkaratsGoblin");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool SpamLaws
        {
            get
            {
                return _SpamLaws;
            }
            set
            {
                if (_SpamLaws != value)
                {
                    _SpamLaws = value;
                    OnPropertyChanged("SpamLawsOfValor");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int ProvokeAoECount
        {
            get
            {
                return _TauntAoECount;
            }
            set
            {
                if (_TauntAoECount != value)
                {
                    _TauntAoECount = value;
                    OnPropertyChanged("TauntAoECount");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int SweepAttackAoECount
        {
            get
            {
                return _SweepAttackAoECount;
            }
            set
            {
                if (_SweepAttackAoECount != value)
                {
                    _SweepAttackAoECount = value;
                    OnPropertyChanged("SweepAttackAoECount");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int JudgmentAoECount
        {
            get
            {
                return _JudgmentAoECount;
            }
            set
            {
                if (_JudgmentAoECount != value)
                {
                    _JudgmentAoECount = value;
                    OnPropertyChanged("JudgmentAoECount");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(3)]
        public int ShieldGlareAoECount
        {
            get
            {
                return _ShieldGlareAoECount;
            }
            set
            {
                if (_ShieldGlareAoECount != value)
                {
                    _ShieldGlareAoECount = value;
                    OnPropertyChanged("ShieldGlareAoECount");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(0.5)]
        public double IronSkinHpPct
        {
            get
            {
                return _IronSkinHpPct;
            }
            set
            {
                if (_IronSkinHpPct != value)
                {
                    _IronSkinHpPct = value;
                    OnPropertyChanged("IronSkinHpPct");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(0.5)]
        public double ConsecrationHpPct
        {
            get
            {
                return _ConsecrationHpPct;
            }
            set
            {
                if (_ConsecrationHpPct != value)
                {
                    _ConsecrationHpPct = value;
                    OnPropertyChanged("ConsecrationHpPct");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(0.5)]
        public double LawsOfHopeHpPct
        {
            get
            {
                return _LawsOfHopeHpPct;
            }
            set
            {
                if (_LawsOfHopeHpPct != value)
                {
                    _LawsOfHopeHpPct = value;
                    OnPropertyChanged("LawsOfHopeHpPct");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(0.5)]
        public double LawsOfJusticeHpPct
        {
            get
            {
                return _LawsOfJusticeHpPct;
            }
            set
            {
                if (_LawsOfJusticeHpPct != value)
                {
                    _LawsOfJusticeHpPct = value;
                    OnPropertyChanged("LawsOfJusticeHpPct");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int BombardmentAoECount
        {
            get
            {
                return _BombardmentAoECount;
            }
            set
            {
                if (_BombardmentAoECount != value)
                {
                    _BombardmentAoECount = value;
                    OnPropertyChanged("BombardmentAoECount");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int FallingSwordAoECount
        {
            get
            {
                return _FallingSwordAoECount;
            }
            set
            {
                if (_FallingSwordAoECount != value)
                {
                    _FallingSwordAoECount = value;
                    OnPropertyChanged("FallingSwordAoECount");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int HeavensFuryAoECount
        {
            get
            {
                return _HeavensFuryAoECount;
            }
            set
            {
                if (_HeavensFuryAoECount != value)
                {
                    _HeavensFuryAoECount = value;
                    OnPropertyChanged("HeavensFuryAoECount");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int CondemnAoECount
        {
            get
            {
                return _CondemnAoECount;
            }
            set
            {
                if (_CondemnAoECount != value)
                {
                    _CondemnAoECount = value;
                    OnPropertyChanged("CondemnAoECount");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(15)]
        public int CondemnRange
        {
            get
            {
                return _CondemnRange;
            }
            set
            {
                if (_CondemnRange != value)
                {
                    _CondemnRange = value;
                    OnPropertyChanged("CondemnRange");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool SteedChargeOOC
        {
            get
            {
                return _SteedChargeOOC;
            }
            set
            {
                if (_SteedChargeOOC != value)
                {
                    _SteedChargeOOC = value;
                    OnPropertyChanged("SteedChargeOOC");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(65f)]
        public float FistOfHeavensDist
        {
            get
            {
                return _FistOfHeavensDist;
            }
            set
            {
                if (_FistOfHeavensDist != value)
                {
                    _FistOfHeavensDist = value;
                    OnPropertyChanged("FistOfHeavensDist");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(30f)]
        public float BlessedHammerRange
        {
            get
            {
                return _BlessedHammerRange;
            }
            set
            {
                if (_BlessedHammerRange != value)
                {
                    _BlessedHammerRange = value;
                    OnPropertyChanged("BlessedHammerRange");
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
        [DefaultValue(1f)]
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
        

        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(CrusaderSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public CrusaderSetting Clone()
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

        }
        #endregion Methods
    }
}

