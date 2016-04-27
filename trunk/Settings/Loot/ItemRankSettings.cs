using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Trinity.Config;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Zeta.Game;

namespace Trinity.Settings.Loot
{
    [DataContract(Namespace = "")]
    public class ItemRankSettings : ITrinitySetting<ItemRankSettings>, INotifyPropertyChanged
    {
        [IgnoreDataMember]
        public string CurrentItemsList
        {
            get
            {
                try
                {
                    if (Trinity.Settings.Loot.ItemFilterMode != ItemFilterMode.ItemRanks)
                        return "Item ranking is currently disabled.";

                    var ird = ItemRanks.GetRankedItemsFromSettings(this);

                    StringBuilder sb = new StringBuilder();
                    if ((ZetaDia.Me.IsFullyValid() && ZetaDia.Me.ActorClass != ActorClass.Invalid) || ItemRankMode == ItemRankMode.AnyClass)
                    {
                        foreach (var itemRank in ird.OrderBy(i => i.Item.BaseType).ThenBy(i => i.Item.ItemType))
                        {
                            foreach (var rank in itemRank.SoftcoreRank)
                            {
                                sb.AppendLine(string.Format("{0}/{1} - {2} - pct={3} rank={4} ss={5}", itemRank.Item.BaseType, itemRank.Item.ItemType, itemRank.Item.Name,
                                    rank.PercentUsed,
                                    rank.Rank,
                                    rank.SampleSize));
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine("Could not read Hero Class. Start the bot first?");
                    }
                    return sb.ToString();
                }
                catch
                {
                    return "Error reading Items List";
                }
            }
            set { }
        }

        #region Fields
        private ItemRankMode _itemRankMode;
        private double _minimumPercent;
        private int _minimumSampleSize;
        private int _minimumRank;
        private bool _ancientItemsOnly;
        private bool _requireSocketsOnJewelry;
        #endregion Fields

        #region Properties
        [DataMember(IsRequired = false)]
        [DefaultValue(ItemRankMode.HeroOnly)]
        public ItemRankMode ItemRankMode
        {
            get
            {
                return _itemRankMode;
            }
            set
            {
                if (_itemRankMode != value)
                {
                    _itemRankMode = value;
                    OnPropertyChanged("ItemRankMode");
                    OnPropertyChanged("CurrentItemsList");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public double MinimumPercent
        {
            get
            {
                return _minimumPercent;
            }
            set
            {
                if (_minimumPercent != value)
                {
                    _minimumPercent = value;
                    OnPropertyChanged("MinimumPercent");
                    OnPropertyChanged("CurrentItemsList");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public int MinimumSampleSize
        {
            get
            {
                return _minimumSampleSize;
            }
            set
            {
                if (_minimumSampleSize != value)
                {
                    _minimumSampleSize = value;
                    OnPropertyChanged("MinimumSampleSize");
                    OnPropertyChanged("CurrentItemsList");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(4)]
        public int MinimumRank
        {
            get
            {
                return _minimumRank;
            }
            set
            {
                if (_minimumRank != value)
                {
                    _minimumRank = value;
                    OnPropertyChanged("MinimumRank");
                    OnPropertyChanged("CurrentItemsList");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AncientItemsOnly
        {
            get
            {
                return _ancientItemsOnly;
            }
            set
            {
                if (_ancientItemsOnly != value)
                {
                    _ancientItemsOnly = value;
                    OnPropertyChanged("AncientItemsOnly");
                    OnPropertyChanged("CurrentItemsList");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool RequireSocketsOnJewelry
        {
            get
            {
                return _requireSocketsOnJewelry;
            }
            set
            {
                if (_requireSocketsOnJewelry != value)
                {
                    _requireSocketsOnJewelry = value;
                    OnPropertyChanged("RequireSocketsOnJewelry");
                    OnPropertyChanged("CurrentItemsList");
                }
            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRuleSetting" /> class.
        /// </summary>
        public ItemRankSettings()
        {
            Reset();
        }
        #endregion Constructors

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(ItemRankSettings setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public ItemRankSettings Clone()
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
            _itemRankMode = ItemRankMode.HeroOnly;
            _minimumPercent = 10;
            _minimumRank = 4;
            _minimumSampleSize = 10;
        }

        #endregion Methods
    }
}
