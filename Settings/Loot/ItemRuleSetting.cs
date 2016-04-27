using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Config;
using Trinity.Config.Loot;

namespace Trinity.Settings.Loot
{
    [DataContract(Namespace = "")]
    public class ItemRuleSetting : ITrinitySetting<ItemRuleSetting>, INotifyPropertyChanged
    {
        #region Fields
        private ItemRuleLogLevel _PickupLogLevel = ItemRuleLogLevel.Rare;
        private ItemRuleLogLevel _KeepLogLevel = ItemRuleLogLevel.Rare;
        private ItemRuleType _ItemRuleType = ItemRuleType.Soft;
        private bool _Debug;
        private bool _UseItemIDs;
        private bool _AlwaysStashAncients;
        private string _ItemRuleSetPath;
        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRuleSetting" /> class.
        /// </summary>
        public ItemRuleSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties
        [DataMember(IsRequired = false)]
        [DefaultValue(ItemRuleLogLevel.Rare)]
        public ItemRuleLogLevel PickupLogLevel
        {
            get
            {
                return _PickupLogLevel;
            }
            set
            {
                if (_PickupLogLevel != value)
                {
                    _PickupLogLevel = value;
                    OnPropertyChanged("PickupLogLevel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(ItemRuleLogLevel.Rare)]
        public ItemRuleLogLevel KeepLogLevel
        {
            get
            {
                return _KeepLogLevel;
            }
            set
            {
                if (_KeepLogLevel != value)
                {
                    _KeepLogLevel = value;
                    OnPropertyChanged("KeepLogLevel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(ItemRuleType.Soft)]
        public ItemRuleType ItemRuleType
        {
            get
            {
                return _ItemRuleType;
            }
            set
            {
                if (_ItemRuleType != value)
                {
                    _ItemRuleType = value;
                    OnPropertyChanged("ItemRuleType");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Debug
        {
            get
            {
                return _Debug;
            }
            set
            {
                if (_Debug != value)
                {
                    _Debug = value;
                    OnPropertyChanged("Debug");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UseItemIDs
        {
            get
            {
                return _UseItemIDs;
            }
            set
            {
                if (_UseItemIDs != value)
                {
                    _UseItemIDs = value;
                    OnPropertyChanged("UseItemIDs");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AlwaysStashAncients
        {
            get
            {
                return _AlwaysStashAncients;
            }
            set
            {
                if (_AlwaysStashAncients != value)
                {
                    _AlwaysStashAncients = value;
                    OnPropertyChanged("AlwaysStashAncients");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue("")]
        public string ItemRuleSetPath
        {
            get
            {
                return _ItemRuleSetPath;
            }
            set
            {
                if (_ItemRuleSetPath != value)
                {
                    _ItemRuleSetPath = value;
                    OnPropertyChanged("ItemRuleSetPath");
                }
            }
        }
        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(ItemRuleSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public ItemRuleSetting Clone()
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
            this._PickupLogLevel = ItemRuleLogLevel.Rare;
            this._KeepLogLevel = ItemRuleLogLevel.Rare;
            this._ItemRuleType = ItemRuleType.Soft;
        }

        #endregion Methods
    }
}
