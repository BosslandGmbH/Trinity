#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Coroutines.Town;
using TrinityCoroutines.Resources;
using Trinity.Helpers;
using Zeta.Bot;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

#endregion

namespace Trinity.Config
{
    [DataContract(Namespace = "")]
    public class GamblingSetting : ITrinitySetting<GamblingSetting>, INotifyPropertyChanged
    {
        private bool _amulet;
        private bool _belt;
        private bool _boots;
        private bool _bracers;
        private bool _chest;
        private bool _debug;
        private int _forceTownRunThreshold;
        private bool _gloves;
        private bool _helm;
        private int _minimumBloodShards;
        private bool _mojo;
        private bool _oneHandItem;
        private bool _orb;
        private bool _pants;
        private bool _quiver;
        private bool _ring;
        private int _saveShardsThreshold;
        private bool _shield;
        private bool _shoulders;
        private bool _shouldSaveShards;
        private bool _shouldTownRun;
        private bool _twoHandItem;
        private int _maximumBloodShards;

        public GamblingSetting()
        {
            Reset();
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool OneHandItem
        {
            get { return _oneHandItem; }
            set
            {
                _oneHandItem = value;
                OnPropertyChanged("OneHandItem");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool TwoHandItem
        {
            get { return _twoHandItem; }
            set
            {
                _twoHandItem = value;
                OnPropertyChanged("TwoHandItem");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Mojo
        {
            get { return _mojo; }
            set
            {
                _mojo = value;
                OnPropertyChanged("Mojo");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Quiver
        {
            get { return _quiver; }
            set
            {
                _quiver = value;
                OnPropertyChanged("Quiver");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Orb
        {
            get { return _orb; }
            set
            {
                _orb = value;
                OnPropertyChanged("Orb");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Helm
        {
            get { return _helm; }
            set
            {
                _helm = value;
                OnPropertyChanged("Helm");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Gloves
        {
            get { return _gloves; }
            set
            {
                _gloves = value;
                OnPropertyChanged("Gloves");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Boots
        {
            get { return _boots; }
            set
            {
                _boots = value;
                OnPropertyChanged("Boots");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Chest
        {
            get { return _chest; }
            set
            {
                _chest = value;
                OnPropertyChanged("Chest");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Belt
        {
            get { return _belt; }
            set
            {
                _belt = value;
                OnPropertyChanged("Belt");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Shoulders
        {
            get { return _shoulders; }
            set
            {
                _shoulders = value;
                OnPropertyChanged("Shoulders");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Pants
        {
            get { return _pants; }
            set
            {
                _pants = value;
                OnPropertyChanged("Pants");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Bracers
        {
            get { return _bracers; }
            set
            {
                _bracers = value;
                OnPropertyChanged("Bracers");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Shield
        {
            get { return _shield; }
            set
            {
                _shield = value;
                OnPropertyChanged("Shield");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Ring
        {
            get { return _ring; }
            set
            {
                _ring = value;
                OnPropertyChanged("Ring");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Amulet
        {
            get { return _amulet; }
            set
            {
                _amulet = value;
                OnPropertyChanged("Amulet");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool Debug
        {
            get { return _debug; }
            set
            {
                _debug = value;
                OnPropertyChanged("Debug");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ShouldSaveShards
        {
            get { return _shouldSaveShards; }
            set
            {
                _shouldSaveShards = value;
                OnPropertyChanged("ShouldSaveShards");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(1500)]                
        public int MaximumBloodShards
        {
            get
            {
                return _maximumBloodShards;                 
            }
            set
            {
                _maximumBloodShards = value;
                OnPropertyChanged("MaximumBloodShards");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool ShouldTownRun
        {
            get { return _shouldTownRun; }
            set
            {
                _shouldTownRun = value;
                OnPropertyChanged("ShouldTownRun");
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(400)]
        public int SaveShardsThreshold
        {
            get
            {
                return _saveShardsThreshold;
            }
            set
            {
                if (_saveShardsThreshold != value)
                {
                    _saveShardsThreshold = value;
                    OnPropertyChanged("SaveShardsThreshold");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(25)]
        public int MinimumBloodShards
        {
            get { return _minimumBloodShards; }
            set
            {
                if (_minimumBloodShards != value)
                {
                    _minimumBloodShards = value;
                    OnPropertyChanged("MinimumBloodShards");
                }
            }
        }


        [IgnoreDataMember]
        public List<TownInfo.VendorSlot> SelectedGambleSlots
        {
            get
            {
                var list = new List<TownInfo.VendorSlot>();
                if (TwoHandItem) list.Add(TownInfo.VendorSlot.TwoHandItem);
                if (OneHandItem) list.Add(TownInfo.VendorSlot.OneHandItem);
                if (Mojo) list.Add(TownInfo.VendorSlot.Mojo);
                if (Quiver) list.Add(TownInfo.VendorSlot.Quiver);
                if (Orb) list.Add(TownInfo.VendorSlot.Orb);
                if (Helm) list.Add(TownInfo.VendorSlot.Helm);
                if (Gloves) list.Add(TownInfo.VendorSlot.Gloves);
                if (Boots) list.Add(TownInfo.VendorSlot.Boots);
                if (Chest) list.Add(TownInfo.VendorSlot.Chest);
                if (Belt) list.Add(TownInfo.VendorSlot.Belt);
                if (Shoulders) list.Add(TownInfo.VendorSlot.Shoulder);
                if (Pants) list.Add(TownInfo.VendorSlot.Pants);
                if (Bracers) list.Add(TownInfo.VendorSlot.Bracers);
                if (Shield) list.Add(TownInfo.VendorSlot.Shield);
                if (Ring) list.Add(TownInfo.VendorSlot.Ring);
                if (Amulet) list.Add(TownInfo.VendorSlot.Amulet);

                return list;
                //return list.Any() ? list : Enum.GetValues(typeof (Town.VendorSlot)).Cast<Town.VendorSlot>().Where(slot => slot != default(Town.VendorSlot)).ToList();
            }
        }

        //public int GetMaxBloodShards()
        //{
        //    if (!ZetaDia.IsInGame || !ZetaDia.Service.IsValid || ErrorDialog.IsVisible)
        //        return _maximumBloodShards;

        //    if (!BotMain.IsRunning)
        //    {
        //        using (new MemoryHelper())
        //        {
        //            _maximumBloodShards = 500 + Zeta.Game.ZetaDia.Me.CommonData.GetAttribute<int>(Zeta.Game.Internals.Actors.ActorAttributeType.HighestSoloRiftLevel) * 10;
        //        }
        //    }
        //    else
        //    {
        //        if (TrinityPlugin.Player.MaxBloodShards != 0)
        //        {
        //            _maximumBloodShards = TrinityPlugin.Player.MaxBloodShards;
        //        }
        //    }

        //    return _maximumBloodShards;
        //} 

        public event PropertyChangedEventHandler PropertyChanged;

        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(GamblingSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public GamblingSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This will set default values for new settings if they were not present in the serialized XML (otherwise they will be
        /// the type defaults)
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
    }
}