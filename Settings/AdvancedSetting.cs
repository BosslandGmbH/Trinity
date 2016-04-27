using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Technicals;

namespace Trinity.Config
{
    [DataContract(Namespace = "")]
    public class AdvancedSetting : ITrinitySetting<AdvancedSetting>, INotifyPropertyChanged
    {
        #region Fields
        private bool _LazyRaiderClickToPause;
        private bool _UnstuckerEnabled;
        private bool _AllowRestartGame;
        private bool _TPSEnabled;
        private int _TPSLimit;
        private int _CacheRefreshRate;
        private int? _cacheLowPriorityRefreshRate;
        private int? _cacheWeightThresholdPct;
        private bool _LogStuckLocation;
        private bool _DebugInStatusBar;
        private LogCategory _LogCategories;
        private bool _GoldInactivityEnabled;
        private bool _XpInactivityEnabled;
        private int _InactivityTimer;
        private bool _LogDroppedItems;
        private bool _OutputReports;
        private bool _ItemRulesLogs;
        private bool _ShowBattleTag;
        private bool _ShowHeroName;
        private bool _ShowHeroClass;
        private bool _DisableAllMovement;
        private bool _AllowDuplicateMessages;
        private bool _exportNewActorMeta;
        private bool _throttleAPS;
        private int _throttleAPSActionCount;
        private bool _forceSpecificGambleSettings;
        private bool _isDbInactivityEnabled;
        private bool _useTrinityDeathHandler;
        private bool _useExperimentalAvoidance;
        private bool _useExperimentalTownRun;

        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedSetting" /> class.
        /// </summary>
        public AdvancedSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties
        [DataMember(IsRequired = false)]
        [DefaultValue(LogCategory.UserInformation)]
        public LogCategory LogCategories
        {
            get
            {
                return _LogCategories;
            }
            set
            {
                if (_LogCategories != value)
                {
                    _LogCategories = value;
                    OnPropertyChanged("LogCategories");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ThrottleAPS
        {
            get
            {
                return _throttleAPS;
            }
            set
            {
                if (_throttleAPS != value)
                {
                    _throttleAPS = value;
                    OnPropertyChanged("ThrottleAPS");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public int ThrottleAPSActionCount
        {
            get
            {
                return _throttleAPSActionCount;
            }
            set
            {
                if (_throttleAPSActionCount != value)
                {
                    _throttleAPSActionCount = value;
                    OnPropertyChanged("ThrottleAPSActionCount");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LazyRaiderClickToPause
        {
            get
            {
                return _LazyRaiderClickToPause;
            }
            set
            {
                if (_LazyRaiderClickToPause != value)
                {
                    _LazyRaiderClickToPause = value;
                    OnPropertyChanged("LazyRaiderClickToPause");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UnstuckerEnabled
        {
            get
            {
                return _UnstuckerEnabled;
            }
            set
            {
                if (_UnstuckerEnabled != value)
                {
                    _UnstuckerEnabled = value;
                    OnPropertyChanged("UnstuckerEnabled");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AllowRestartGame
        {
            get
            {
                return _AllowRestartGame;
            }
            set
            {
                if (_AllowRestartGame != value)
                {
                    _AllowRestartGame = value;
                    OnPropertyChanged("AllowRestartGame");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool GoldInactivityEnabled
        {
            get
            {
                return _GoldInactivityEnabled;
            }
            set
            {
                if (_GoldInactivityEnabled != value)
                {
                    _GoldInactivityEnabled = value;
                    OnPropertyChanged("GoldInactivityEnabled");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IsDBInactivityEnabled
        {
            get
            {
                return _isDbInactivityEnabled;
            }
            set
            {
                if (_isDbInactivityEnabled != value)
                {
                    _isDbInactivityEnabled = value;
                    OnPropertyChanged("IsDBInactivityEnabled");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool XpInactivityEnabled
        {
            get
            {
                return _XpInactivityEnabled;
            }
            set
            {
                if (_XpInactivityEnabled != value)
                {
                    _XpInactivityEnabled = value;
                    OnPropertyChanged("XpInactivityEnabled");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(800)]
        public int InactivityTimer
        {
            get
            {
                return _InactivityTimer;
            }
            set
            {
                if (_InactivityTimer != value)
                {
                    _InactivityTimer = value;
                    OnPropertyChanged("InactivityTimer");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool TPSEnabled
        {
            get
            {
                return _TPSEnabled;
            }
            set
            {
                if (_TPSEnabled != value)
                {
                    _TPSEnabled = value;
                    OnPropertyChanged("TPSEnabled");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(8)]
        public int TPSLimit
        {
            get
            {
                return _TPSLimit;
            }
            set
            {
                if (_TPSLimit != value)
                {
                    _TPSLimit = value;
                    OnPropertyChanged("TPSLimit");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(300)]
        public int CacheRefreshRate
        {
            get
            {
                return _CacheRefreshRate;
            }
            set
            {
                if (_CacheRefreshRate != value)
                {
                    _CacheRefreshRate = value;
                    OnPropertyChanged("CacheRefreshRate");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(500)]
        public int CacheLowPriorityRefreshRate
        {
            get
            {
                if (!_cacheLowPriorityRefreshRate.HasValue)
                    _cacheLowPriorityRefreshRate = 500;

                return _cacheLowPriorityRefreshRate.Value;
            }
            set
            {
                if (_cacheLowPriorityRefreshRate.HasValue && _cacheLowPriorityRefreshRate.Value != value)
                    OnPropertyChanged("CacheLowPriorityRefreshRate");

                _cacheLowPriorityRefreshRate = value;          
            }
        }

        [DataMember(IsRequired = false)]
        public int CacheWeightThresholdPct
        {
            get
            {
                if (!_cacheWeightThresholdPct.HasValue)
                    _cacheWeightThresholdPct = 50;
                
                return _cacheWeightThresholdPct.Value;
            }
            set
            {
                if (_cacheWeightThresholdPct.HasValue && _cacheWeightThresholdPct.Value != value)
                    OnPropertyChanged("CacheWeightThresholdPct");

                _cacheWeightThresholdPct = value;
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool DebugInStatusBar
        {
            get
            {
                return _DebugInStatusBar;
            }
            set
            {
                if (_DebugInStatusBar != value)
                {
                    _DebugInStatusBar = value;
                    OnPropertyChanged("DebugInStatusBar");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LogStuckLocation
        {
            get
            {
                return _LogStuckLocation;
            }
            set
            {
                if (_LogStuckLocation != value)
                {
                    _LogStuckLocation = value;
                    OnPropertyChanged("LogStuckLocation");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool LogDroppedItems
        {
            get
            {
                return _LogDroppedItems;
            }
            set
            {
                if (_LogDroppedItems != value)
                {
                    _LogDroppedItems = value;
                    OnPropertyChanged("LogDroppedItems");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool OutputReports
        {
            get
            {
                return _OutputReports;
            }
            set
            {
                if (_OutputReports != value)
                {
                    _OutputReports = value;
                    OnPropertyChanged("OutputReports");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ItemRulesLogs
        {
            get
            {
                return _ItemRulesLogs;
            }
            set
            {
                if (_ItemRulesLogs != value)
                {
                    _ItemRulesLogs = value;
                    OnPropertyChanged("ItemRulesLogs");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ShowBattleTag
        {
            get
            {
                return _ShowBattleTag;
            }
            set
            {
                if (_ShowBattleTag != value)
                {
                    _ShowBattleTag = value;
                    OnPropertyChanged("ShowBattleTag");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ShowHeroName
        {
            get
            {
                return _ShowHeroName;
            }
            set
            {
                if (_ShowHeroName != value)
                {
                    _ShowHeroName = value;
                    OnPropertyChanged("ShowHeroName");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ShowHeroClass
        {
            get
            {
                return _ShowHeroClass;
            }
            set
            {
                if (_ShowHeroClass != value)
                {
                    _ShowHeroClass = value;
                    OnPropertyChanged("ShowHeroClass");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool DisableAllMovement
        {
            get
            {
                return _DisableAllMovement;
            }
            set
            {
                if (_DisableAllMovement != value)
                {
                    _DisableAllMovement = value;
                    OnPropertyChanged("DisableAllMovement");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AllowDuplicateMessages
        {
            get
            {
                return _AllowDuplicateMessages;
            }
            set
            {
                if (_AllowDuplicateMessages != value)
                {
                    _AllowDuplicateMessages = value;
                    OnPropertyChanged("AllowDuplicateMessages");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ExportNewActorMeta
        {
            get
            {
                return _exportNewActorMeta;
            }
            set
            {
                if (_exportNewActorMeta != value)
                {
                    _exportNewActorMeta = value;
                    OnPropertyChanged("ExportNewCacheMeta");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ForceSpecificGambleSettings
        {
            get
            {
                return _forceSpecificGambleSettings;
            }
            set
            {
                if (_forceSpecificGambleSettings != value)
                {
                    _forceSpecificGambleSettings = value;
                    OnPropertyChanged("ForceSpecificGambleSettings");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseTrinityDeathHandler
        {
            get
            {
                return _useTrinityDeathHandler;
            }
            set
            {
                if (_useTrinityDeathHandler != value)
                {
                    _useTrinityDeathHandler = value;
                    OnPropertyChanged("UseTrinityDeathHandler");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseExperimentalAvoidance
        {
            get
            {
                return _useExperimentalAvoidance;
            }
            set
            {
                if (_useExperimentalAvoidance != value)
                {
                    _useExperimentalAvoidance = value;
                    OnPropertyChanged("UseExperimentalAvoidance");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseExperimentalTownRun
        {
            get
            {
                return _useExperimentalTownRun;
            }
            set
            {
                if (_useExperimentalTownRun != value)
                {
                    _useExperimentalTownRun = value;
                    OnPropertyChanged("UseExperimentalTownRun");
                }
            }
        }

        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(AdvancedSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public AdvancedSetting Clone()
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
            ThrottleAPS = true;
            ThrottleAPSActionCount = 10;
            CacheRefreshRate = 100;
            OutputReports = true;
            ItemRulesLogs = true;
            LogDroppedItems = true;
            InactivityTimer = 600;
            UnstuckerEnabled = true;
            IsDBInactivityEnabled = false;
            UseTrinityDeathHandler = false;
            UseExperimentalAvoidance = false;
        }
        #endregion Methods
    }
}

