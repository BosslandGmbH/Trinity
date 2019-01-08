using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Settings
{
    [DataContract]
    public sealed class AdventurerGemSetting : NotifyBase
    {
        private bool _isEnabled;
        private int _order;
        private int _limit;
        private bool _isLimited;
        private SNOActor _sno;
        private Item _item;
        private int _maxRank;
        private string _name;
        private int _gemCount;
        private int _highestRank;
        private bool _isEquipped;
        private IEnumerable<AdventurerGem> _gems;

        public AdventurerGemSetting(Item item)
        {
            _sno = item.Id;
            _item = item;
            _name = item.Name;
            _maxRank = item.MaxRank > 0 ? item.MaxRank : 150;
            LoadDefaults();
        }

        public Item Item
        {
            get => _item;
            set => SetField(ref _item, value);
        }

        public int MaxRank
        {
            get => _maxRank;
            set => SetField(ref _maxRank, value);
        }

        [DataMember]
        public int HighestRank
        {
            get => _highestRank;
            set => SetField(ref _highestRank, value);
        }

        [DataMember]
        public int GemCount
        {
            get => _gemCount;
            set => SetField(ref _gemCount, value);
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        [DataMember]
        public int Limit
        {
            get => _limit;
            set => SetField(ref _limit, value);
        }

        [DataMember]
        public bool IsLimited
        {
            get => _isLimited;
            set => SetField(ref _isLimited, value);
        }

        [DataMember]
        public SNOActor Sno
        {
            get => _sno;
            set => SetField(ref _sno, value);
        }

        [DataMember]
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetField(ref _isEnabled, value);
        }

        [DataMember]
        public int Order
        {
            get => _order;
            set => SetField(ref _order, value);
        }

        [DataMember]
        public bool IsEquipped
        {
            get => _isEquipped;
            set => SetField(ref _isEquipped, value);
        }

        [DataMember]
        public bool IsExpanded
        {
            get => _isEquipped;
            set => SetField(ref _isEquipped, value);
        }

        public IEnumerable<AdventurerGem> Gems
        {
            get => _gems;
            set => SetField(ref _gems, value);
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {Name} ({Sno})";
        }
    }
}