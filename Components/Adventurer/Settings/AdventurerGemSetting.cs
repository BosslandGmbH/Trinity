using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;

namespace Trinity.Components.Adventurer.Settings
{
    [DataContract]
    public class AdventurerGemSetting : NotifyBase
    {
        private bool _isEnabled;
        private int _order;
        private int _limit;
        private bool _isLimited;
        private int _Sno;
        private Item _item;
        private int _maxRank;
        private string _name;
        private int _gemCount;
        private int _highestRank;
        private bool _isEquipped;
        private IEnumerable<AdventurerGem> _gems;

        public AdventurerGemSetting(Item item)
        {
            _Sno = item.Id;
            _item = item;
            _name = item.Name;
            _maxRank = item.MaxRank > 0 ? item.MaxRank : 150;
            LoadDefaults();
        }

        public Item Item
        {
            get { return _item; }
            set { SetField(ref _item, value); }
        }

        public int MaxRank
        {
            get { return _maxRank; }
            set { SetField(ref _maxRank, value); }
        }

        [DataMember]
        public int HighestRank
        {
            get { return _highestRank; }
            set { SetField(ref _highestRank, value); }
        }

        [DataMember]
        public int GemCount
        {
            get { return _gemCount; }
            set { SetField(ref _gemCount, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        [DataMember]
        public int Limit
        {
            get { return _limit; }
            set { SetField(ref _limit, value); }
        }

        [DataMember]
        public bool IsLimited
        {
            get { return _isLimited; }
            set { SetField(ref _isLimited, value); }
        }

        [DataMember]
        public int Sno
        {
            get { return _Sno; }
            set { SetField(ref _Sno, value); }
        }

        [DataMember]
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetField(ref _isEnabled, value); }
        }

        [DataMember]
        public int Order
        {
            get { return _order; }
            set { SetField(ref _order, value); }
        }

        [DataMember]
        public bool IsEquipped
        {
            get { return _isEquipped; }
            set { SetField(ref _isEquipped, value); }
        }

        [DataMember]
        public bool IsExpanded
        {
            get { return _isEquipped; }
            set { SetField(ref _isEquipped, value); }
        }

        public IEnumerable<AdventurerGem> Gems
        {
            get { return _gems; }
            set { SetField(ref _gems, value); }
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {Name} ({Sno})";
        }
    }
}