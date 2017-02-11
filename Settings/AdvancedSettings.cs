using System.ComponentModel;
using System.Configuration;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Attributes;
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

        public AdvancedSettings()
        {
            base.LoadDefaults();
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(LogCategory.UserInformation | LogCategory.Behavior | LogCategory.Routine | LogCategory.Avoidance | LogCategory.Targetting)]
        public LogCategory LogCategories
        {
            get { return _logCategories; }
            set { SetField(ref _logCategories, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LazyRaider
        {
            get { return _lazyRaider; }
            set { SetField(ref _lazyRaider, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool GoldInactivityEnabled
        {
            get { return _goldInactivityEnabled; }
            set { SetField(ref _goldInactivityEnabled, value); }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool XpInactivityEnabled
        {
            get { return _xpInactivityEnabled; }
            set { SetField(ref _xpInactivityEnabled, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(800)]
        public int InactivityTimer
        {
            get { return _inactivityTimer; }
            set { SetField(ref _inactivityTimer, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool TpsEnabled
        {
            get { return _tpsEnabled; }
            set { SetField(ref _tpsEnabled, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(20)]
        public int TpsLimit
        {
            get { return _tpsLimit; }
            set { SetField(ref _tpsLimit, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool OutputReports
        {
            get { return _outputReports; }
            set { SetField(ref _outputReports, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ShowBattleTag
        {
            get { return _showBattleTag; }
            set { SetField(ref _showBattleTag, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ShowHeroName
        {
            get { return _showHeroName; }
            set { SetField(ref _showHeroName, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool ShowHeroClass
        {
            get { return _showHeroClass; }
            set { SetField(ref _showHeroClass, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool DisableAllMovement
        {
            get { return _disableAllMovement; }
            set { SetField(ref _disableAllMovement, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LogStats
        {
            get { return _logStats; }
            set { SetField(ref _logStats, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool LogItems
        {
            get { return _logItems; }
            set { SetField(ref _logItems, value); }
        }

        [DataMember(IsRequired = false)]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public GameStopReasons StopReasons
        {            
            get { return _stopReasons; }
            set { SetField(ref _stopReasons, value); }
        }

    }
}

