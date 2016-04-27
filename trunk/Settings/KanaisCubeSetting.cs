using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Config.Combat;
using Trinity.Settings.Loot;
using TrinityCoroutines.Resources;
using Trinity.Technicals;

namespace Trinity.Config
{
    [DataContract(Namespace = "")]
    public class KanaisCubeSetting : ITrinitySetting<KanaisCubeSetting>, INotifyPropertyChanged
    {
        #region Fields

        private ItemSelectionType _rareUpgradeTypes;
        private int _conversionQuantityThreshold;
        private bool _createVeiledCrystals;
        private bool _createReusableParts;
        private bool _createArcaneDust;
        private int _deathsBreathMinimum;
        private CubeExtractOption _extractLegendaryPowers;
        private bool _cubeExtractFromStash;

        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="KanaisCubeSetting" /> class.
        /// </summary>
        public KanaisCubeSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CreateReusableParts
        {
            get
            {
                return _createReusableParts;
            }
            set
            {
                if (_createReusableParts != value)
                {
                    _createReusableParts = value;
                    OnPropertyChanged("CreateReusableParts");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CreateVeiledCrystals
        {
            get
            {
                return _createVeiledCrystals;
            }
            set
            {
                if (_createVeiledCrystals != value)
                {
                    _createVeiledCrystals = value;
                    OnPropertyChanged("CreateVeiledCrystals");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CreateArcaneDust
        {
            get
            {
                return _createArcaneDust;
            }
            set
            {
                if (_createArcaneDust != value)
                {
                    _createArcaneDust = value;
                    OnPropertyChanged("CreateArcaneDust");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(25000)]
        public int ConversionQuantityThreshold
        {
            get
            {
                return _conversionQuantityThreshold;
            }
            set
            {
                if (_conversionQuantityThreshold != value)
                {
                    _conversionQuantityThreshold = value;
                    OnPropertyChanged("ConversionQuantityThreshold");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0)]
        public int DeathsBreathMinimum
        {
            get
            {
                return _deathsBreathMinimum;
            }
            set
            {
                if (_deathsBreathMinimum != value)
                {
                    _deathsBreathMinimum = value;
                    OnPropertyChanged("DeathsBreathMinimum");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(default(ItemSelectionType))]
        public ItemSelectionType RareUpgradeTypes
        {
            get
            {
                return _rareUpgradeTypes;
            }
            set
            {
                if (_rareUpgradeTypes != value)
                {
                    _rareUpgradeTypes = value;
                    OnPropertyChanged("RareUpgradeTypes");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(CubeExtractOption.None)]
        public CubeExtractOption ExtractLegendaryPowers
        {
            get
            {
                return _extractLegendaryPowers;
            }
            set
            {
                if (_extractLegendaryPowers != value)
                {
                    _extractLegendaryPowers = value;
                    OnPropertyChanged("ExtractLegendaryPowers");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CubeExtractFromStash
        {
            get
            {
                return _cubeExtractFromStash;
            }
            set
            {
                if (_cubeExtractFromStash != value)
                {
                    _cubeExtractFromStash = value;
                    OnPropertyChanged("CubeExtractFromStash");
                }
            }
        }

        public HashSet<ItemSelectionType> GetRareUpgradeSettings()
        {
            var selectedTypes = TrinityPlugin.Settings.KanaisCube.RareUpgradeTypes;
            return new HashSet<ItemSelectionType>(Enum.GetValues(selectedTypes.GetType()).Cast<Enum>().Where(selectedTypes.HasFlag).Cast<ItemSelectionType>());
        }

        public HashSet<InventoryItemType> GetCraftingMaterialTypes()
        {
            var result = new HashSet<InventoryItemType>();
            if (CreateArcaneDust)
                result.Add(InventoryItemType.ArcaneDust);
            if (CreateReusableParts)
                result.Add(InventoryItemType.ReusableParts);
            if (CreateVeiledCrystals)
                result.Add(InventoryItemType.VeiledCrystal);
            return result;
        }

        #endregion Properties

        #region Methods
        public void Reset()
        {            
            TrinitySetting.Reset(this);
        }

        public void CopyTo(KanaisCubeSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public KanaisCubeSetting Clone()
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
            foreach (var p in GetType().GetProperties())
            {
                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
                {
                    p.SetValue(this, dv.Value);
                }
            }
        }
        #endregion Methods
    }
}
