using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Trinity.Config;
using Trinity.Config.Loot;
using Trinity.Technicals;

namespace Trinity.Settings.Loot
{
    [DataContract(Namespace = "")]
    public class ItemSetting : ITrinitySetting<ItemSetting>, INotifyPropertyChanged, ITrinitySettingEvents
    {
        #region Fields
        private ItemFilterMode _itemFilterMode;
        private PickupSetting _pickup;
        private TownRunSetting _townRun;
        private ItemRuleSetting _itemRules;
        private ItemRankSettings _itemRank;
        private ItemListSettings _itemList;


        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        public ItemSetting()
        {
            Reset();
            Pickup = new PickupSetting();
            TownRun = new TownRunSetting();
            ItemRules = new ItemRuleSetting();
            ItemRank = new ItemRankSettings();
            ItemList = new ItemListSettings();
        }
        #endregion Constructors

        #region Properties
        [DataMember(IsRequired = false)]
        [DefaultValue(ItemFilterMode.TrinityOnly)]
        public ItemFilterMode ItemFilterMode
        {
            get
            {
                return _itemFilterMode;
            }
            set
            {
                if (_itemFilterMode != value)
                {
                    _itemFilterMode = value;
                    OnPropertyChanged("ItemFilterMode");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public PickupSetting Pickup
        {
            get
            {
                return _pickup;
            }
            set
            {
                if (_pickup != value)
                {
                    _pickup = value;
                    OnPropertyChanged("Pickup");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public TownRunSetting TownRun
        {
            get
            {
                return _townRun;
            }
            set
            {
                if (_townRun != value)
                {
                    _townRun = value;
                    OnPropertyChanged("TownRun");
                }
            }
        }
        [DataMember(IsRequired = false)]
        public ItemRuleSetting ItemRules
        {
            get
            {
                return _itemRules;
            }
            set
            {
                if (_itemRules != value)
                {
                    _itemRules = value;
                    OnPropertyChanged("ItemRules");
                }
            }
        }
        [DataMember(IsRequired = false)]
        public ItemRankSettings ItemRank
        {
            get
            {
                return _itemRank;
            }
            set
            {
                if (_itemRank != value)
                {
                    _itemRank = value;
                    OnPropertyChanged("ItemRank");
                }
            }
        }
        [DataMember(IsRequired = false)]
        public ItemListSettings ItemList
        {
            get
            {
                return _itemList;
            }
            set
            {
                if (_itemList!= value)
                {
                    _itemList = value;
                    OnPropertyChanged("ItemList");
                }
            }
        }


        #endregion
        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(ItemSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public ItemSetting Clone()
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
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {

        }

        #endregion Methods

        #region ITrinitySettingEvents

        public void OnSave()
        {
            Logger.Log("ItemSetting OnSave called");
            FireOnSaveEvents();
        }

        public void OnLoaded()
        {
            Logger.Log("ItemSetting OnLoaded called");
            FireOnLoadedEvents();
        }

        public void FireOnSaveEvents()
        {
            var eventSupporters = GetInterfaceMembers<ITrinitySettingEvents>(this);
            foreach (var eventSupporter in eventSupporters)
            {
                eventSupporter.OnSave();
            }
        }

        public void FireOnLoadedEvents()
        {
            var eventSupporters = GetInterfaceMembers<ITrinitySettingEvents>(this);
            foreach (var eventSupporter in eventSupporters)
            {
                eventSupporter.OnLoaded();
            }
        }

        private IEnumerable<T> GetInterfaceMembers<T>(object obj)
        {
            var type = obj.GetType();
            return from property in type.GetProperties()
                   where typeof(T).IsAssignableFrom(property.PropertyType)
                   select GetValue<T>(obj, property);
        }

        private static T GetValue<T>(object obj, PropertyInfo propertyInfo)
        {
            return (T)propertyInfo.GetValue(obj, null);
        }

        #endregion

    }
}
