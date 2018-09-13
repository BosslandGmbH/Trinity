using System.ComponentModel;
using System.Configuration;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.UI.UIComponents;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public sealed class AdvancedSettings : NotifyBase
    { 
        private bool _lazyRaider;
        private bool _tpsEnabled;
        private int _tpsLimit;
        private LogCategory _logCategories;
        private bool _goldInactivityEnabled;
        private bool _xpInactivityEnabled;
        private int _inactivityTimer;
        private bool _outputReports;
        private bool _showBattleTag;
        private bool _showHeroName;
        private bool _showHeroClass;
        private bool _disableAllMovement;
        private bool _logStats;
        private bool _logItems;
        private GameStopReasons _stopReasons;
        private bool _logAllItems;
        private bool _logDroppedItems;

        public AdvancedSettings()
        {
            base.LoadDefaults();
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(LogCategory.None | LogCategory.Behavior | LogCategory.Routine | LogCategory.Avoidance | LogCategory.Targetting)]
        public LogCategory LogCategories
        {
            get => _logCategories;
            set => SetField(ref _logCategories, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LogAllItems
        {
            get => _logAllItems;
            set => SetField(ref _logAllItems, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LogDroppedItems
        {
            get => _logDroppedItems;
            set => SetField(ref _logDroppedItems, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LazyRaider
        {
            get => _lazyRaider;
            set => SetField(ref _lazyRaider, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool GoldInactivityEnabled
        {
            get => _goldInactivityEnabled;
            set => SetField(ref _goldInactivityEnabled, value);
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool XpInactivityEnabled
        {
            get => _xpInactivityEnabled;
            set => SetField(ref _xpInactivityEnabled, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(800)]
        public int InactivityTimer
        {
            get => _inactivityTimer;
            set => SetField(ref _inactivityTimer, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool TpsEnabled
        {
            get => _tpsEnabled;
            set => SetField(ref _tpsEnabled, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(20)]
        public int TpsLimit
        {
            get => _tpsLimit;
            set => SetField(ref _tpsLimit, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool OutputReports
        {
            get => _outputReports;
            set => SetField(ref _outputReports, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ShowBattleTag
        {
            get => _showBattleTag;
            set => SetField(ref _showBattleTag, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ShowHeroName
        {
            get => _showHeroName;
            set => SetField(ref _showHeroName, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ShowHeroClass
        {
            get => _showHeroClass;
            set => SetField(ref _showHeroClass, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool DisableAllMovement
        {
            get => _disableAllMovement;
            set => SetField(ref _disableAllMovement, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LogStats
        {
            get => _logStats;
            set => SetField(ref _logStats, value);
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LogItems
        {
            get => _logItems;
            set => SetField(ref _logItems, value);
        }

        [DataMember(IsRequired = false)]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public GameStopReasons StopReasons
        {            
            get => _stopReasons;
            set => SetField(ref _stopReasons, value);
        }

    }
}

