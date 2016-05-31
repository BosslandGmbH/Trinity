using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Config.Combat;
using Trinity.Settings.Loot;

namespace Trinity.Config.Loot
{
    [DataContract(Namespace = "")]
    public class PickupSetting : ITrinitySetting<PickupSetting>, INotifyPropertyChanged
    {
        #region Fields
        private bool _pickupUpgrades;
        private bool _pickupGrayItems;
        private bool _pickupWhiteItems;
        private bool _pickupBlueWeapons;
        private bool _pickupYellowWeapons;
        private bool _pickupBlueArmor;
        private bool _pickupYellowAmor;
        private bool _pickupBlueJewlery;
        private bool _pickupYellowJewlery;
        private bool _pickupblueFollowerItems;
        private bool _pickupYellowFollowerItems;
        private bool _pickupLegendaryFollowerItems;
        private bool _pickupLegendaries;
        private int _potionCount;
        private TrinityGemType _gemType;
        private int _gemLevel;
        private int _minimumGoldStack;
        private bool _pickupGold;
        private bool _plans;
        private bool _legendaryPlans;
        private bool _designs;
        private TrinityItemQuality _miscItemQuality;
        private bool _craftMaterials;
        private bool _infernalKeys;
        private bool _pickupLowLevel;
        private bool _lootRunKey;
        private bool _ramadalinisGift;
        private bool _legendaryPotions;
        private bool _bloodShards;
        private bool _ignoreTwoHandedWeapons;

        private bool _ignoreLegendaryInAoE;
        private bool _ignoreNonLegendaryInAoE;
        private bool _ignoreLegendaryNearElites;
        private bool _ignoreRareNearElites;
        private bool _ignoreGoldInAoE;
        private bool _ignoreGoldNearElites;
        private bool _pickupDeathsBreath;
        private bool _autoEquipItems;
        private bool _autoEquipSkills;
        private bool _autoEquipIgnoreWeapons;
        private bool _alwaysVacuumItems;
        private bool _dontPickupInTown;
        private bool _disableAutoEquipAtMaxLevel;
        private bool _ignoreProgressionGlobesInAoE;
        private bool _ignoreHealthGlobesInAoE;
        private bool _stashWings;
        private bool _stashPets;
        private bool _stashTransmog;
        private ItemFilterMode _itemFilterMode;

        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PickupSetting" /> class.
        /// </summary>
        public PickupSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(ItemFilterMode.None)]
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
        [DefaultValue(true)]
        public bool AutoEquipItems
        {
            get
            {
                return _autoEquipItems;
            }
            set
            {
                if (_autoEquipItems != value)
                {
                    _autoEquipItems = value;
                    OnPropertyChanged("AutoEquipItems");
                }
            }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool DontPickupInTown
        {
            get
            {
                return _dontPickupInTown;
            }
            set
            {
                if (_dontPickupInTown != value)
                {
                    _dontPickupInTown = value;
                    OnPropertyChanged("DontPickupInTown");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AutoEquipSkills
        {
            get
            {
                return _autoEquipSkills;
            }
            set
            {
                if (_autoEquipSkills != value)
                {
                    _autoEquipSkills = value;
                    OnPropertyChanged("AutoEquipSkills");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AutoEquipIgnoreWeapons
        {
            get
            {
                return _autoEquipIgnoreWeapons;
            }
            set
            {
                if (_autoEquipIgnoreWeapons != value)
                {
                    _autoEquipIgnoreWeapons = value;
                    OnPropertyChanged("AutoEquipIgnoreWeapons");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool DisableAutoEquipAtMaxLevel
        {
            get
            {
                return _disableAutoEquipAtMaxLevel;
            }
            set
            {
                if (_disableAutoEquipAtMaxLevel != value)
                {
                    _disableAutoEquipAtMaxLevel = value;
                    OnPropertyChanged("DisableAutoEquipAtMaxLevel");
                }
            }
        }

        

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupUpgrades
        {
            get
            {
                return _pickupUpgrades;
            }
            set
            {
                if (_pickupUpgrades != value)
                {
                    _pickupUpgrades = value;
                    OnPropertyChanged("PickupUpgrades");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupGrayItems
        {
            get
            {
                return _pickupGrayItems;
            }
            set
            {
                if (_pickupGrayItems != value)
                {
                    _pickupGrayItems = value;
                    OnPropertyChanged("PickupGrayItems");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupWhiteItems
        {
            get
            {
                return _pickupWhiteItems;
            }
            set
            {
                if (_pickupWhiteItems != value)
                {
                    _pickupWhiteItems = value;
                    OnPropertyChanged("PickupWhiteItems");
                }
            }
        }
        
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupBlueWeapons
        {
            get
            {
                return _pickupBlueWeapons;
            }
            set
            {
                if (_pickupBlueWeapons != value)
                {
                    _pickupBlueWeapons = value;
                    OnPropertyChanged("PickupBlueWeapons");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupYellowWeapons
        {
            get
            {
                return _pickupYellowWeapons;
            }
            set
            {
                if (_pickupYellowWeapons != value)
                {
                    _pickupYellowWeapons = value;
                    OnPropertyChanged("PickupYellowWeapons");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupBlueArmor
        {
            get
            {
                return _pickupBlueArmor;
            }
            set
            {
                if (_pickupBlueArmor != value)
                {
                    _pickupBlueArmor = value;
                    OnPropertyChanged("PickupBlueArmor");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupYellowArmor
        {
            get
            {
                return _pickupYellowAmor;
            }
            set
            {
                if (_pickupYellowAmor != value)
                {
                    _pickupYellowAmor = value;
                    OnPropertyChanged("PickupYellowArmor");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupBlueJewlery
        {
            get
            {
                return _pickupBlueJewlery;
            }
            set
            {
                if (_pickupBlueJewlery != value)
                {
                    _pickupBlueJewlery = value;
                    OnPropertyChanged("PickupBlueJewlery");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupYellowJewlery
        {
            get
            {
                return _pickupYellowJewlery;
            }
            set
            {
                if (_pickupYellowJewlery != value)
                {
                    _pickupYellowJewlery = value;
                    OnPropertyChanged("PickupYellowJewlery");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(98)]
        public int PotionCount
        {
            get
            {
                return _potionCount;
            }
            set
            {
                if (_potionCount != value)
                {
                    _potionCount = value;
                    OnPropertyChanged("PotionCount");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(TrinityGemType.Ruby | TrinityGemType.Amethyst | TrinityGemType.Emerald | TrinityGemType.Topaz | TrinityGemType.Diamond)]
        public TrinityGemType GemType
        {
            get
            {
                return _gemType;
            }
            set
            {
                if (_gemType != value)
                {
                    _gemType = value;
                    OnPropertyChanged("GemType");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(15)]
        public int GemLevel
        {
            get
            {
                return _gemLevel;
            }
            set
            {
                if (_gemLevel != value)
                {
                    _gemLevel = value;
                    OnPropertyChanged("GemLevel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupLegendaries
        {
            get
            {
                return _pickupLegendaries;
            }
            set
            {
                if (_pickupLegendaries != value)
                {
                    _pickupLegendaries = value;
                    OnPropertyChanged("PickupLegendaries");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(150)]
        public int MinimumGoldStack
        {
            get
            {
                return _minimumGoldStack;
            }
            set
            {
                if (_minimumGoldStack != value)
                {
                    _minimumGoldStack = value;
                    OnPropertyChanged("MinimumGoldStack");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool PickupGold
        {
            get
            {
                return _pickupGold;
            }
            set
            {
                if (_pickupGold != value)
                {
                    _pickupGold = value;
                    OnPropertyChanged("PickupGold");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool Plans
        {
            get
            {
                return _plans;
            }
            set
            {
                if (_plans != value)
                {
                    _plans = value;
                    OnPropertyChanged("Plans");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool LegendaryPlans
        {
            get
            {
                return _legendaryPlans;
            }
            set
            {
                if (_legendaryPlans != value)
                {
                    _legendaryPlans = value;
                    OnPropertyChanged("LegendaryPlans");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool Designs
        {
            get
            {
                return _designs;
            }
            set
            {
                if (_designs != value)
                {
                    _designs = value;
                    OnPropertyChanged("Designs");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupBlueFollowerItems
        {
            get
            {
                return _pickupblueFollowerItems;
            }
            set
            {
                if (_pickupblueFollowerItems != value)
                {
                    _pickupblueFollowerItems = value;
                    OnPropertyChanged(nameof(PickupBlueFollowerItems));
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupYellowFollowerItems
        {
            get
            {
                return _pickupYellowFollowerItems;
            }
            set
            {
                if (_pickupYellowFollowerItems != value)
                {
                    _pickupYellowFollowerItems = value;
                    OnPropertyChanged(nameof(PickupYellowFollowerItems));
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupLegendaryFollowerItems
        {
            get
            {
                return _pickupLegendaryFollowerItems;
            }
            set
            {
                if (_pickupLegendaryFollowerItems != value)
                {
                    _pickupLegendaryFollowerItems = value;
                    OnPropertyChanged("PickupLegendaryFollowerItems");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(TrinityItemQuality.Common)]
        public TrinityItemQuality MiscItemQuality
        {
            get
            {
                return _miscItemQuality;
            }
            set
            {
                if (_miscItemQuality != value)
                {
                    _miscItemQuality = value;
                    OnPropertyChanged("MiscItemLevel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool CraftMaterials
        {
            get
            {
                return _craftMaterials;
            }
            set
            {
                if (_craftMaterials != value)
                {
                    _craftMaterials = value;
                    OnPropertyChanged("CraftMaterials");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool InfernalKeys
        {
            get
            {
                return _infernalKeys;
            }
            set
            {
                if (_infernalKeys != value)
                {
                    _infernalKeys = value;
                    OnPropertyChanged("InfernalKeys");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupLowLevel
        {
            get
            {
                return _pickupLowLevel;
            }
            set
            {
                if (_pickupLowLevel != value)
                {
                    _pickupLowLevel = value;
                    OnPropertyChanged("PickupLowLevel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool LootRunKey
        {
            get
            {
                return _lootRunKey;
            }
            set
            {
                if (_lootRunKey != value)
                {
                    _lootRunKey = value;
                    OnPropertyChanged("LootRunKey");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool RamadalinisGift
        {
            get
            {
                return _ramadalinisGift;
            }
            set
            {
                if (_ramadalinisGift != value)
                {
                    _ramadalinisGift = value;
                    OnPropertyChanged("RamadalinisGift");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool LegendaryPotions
        {
            get
            {
                return _legendaryPotions;
            }
            set
            {
                if (_legendaryPotions != value)
                {
                    _legendaryPotions = value;
                    OnPropertyChanged("LegendaryPotions");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool BloodShards
        {
            get
            {
                return _bloodShards;
            }
            set
            {
                if (_bloodShards != value)
                {
                    _bloodShards = value;
                    OnPropertyChanged("BloodShards");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreTwoHandedWeapons
        {
            get
            {
                return _ignoreTwoHandedWeapons;
            }
            set
            {
                if (_ignoreTwoHandedWeapons != value)
                {
                    _ignoreTwoHandedWeapons = value;
                    OnPropertyChanged("TwoHandedWeapons");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreLegendaryInAoE
        {
            get
            {
                return _ignoreLegendaryInAoE;
            }
            set
            {
                if (_ignoreLegendaryInAoE != value)
                {
                    _ignoreLegendaryInAoE = value;
                    OnPropertyChanged("IgnoreLegendaryInAoE");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreNonLegendaryInAoE
        {
            get
            {
                return _ignoreNonLegendaryInAoE;
            }
            set
            {
                if (_ignoreNonLegendaryInAoE != value)
                {
                    _ignoreNonLegendaryInAoE = value;
                    OnPropertyChanged("IgnoreNonLegendaryInAoE");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreLegendaryNearElites
        {
            get
            {
                return _ignoreLegendaryNearElites;
            }
            set
            {
                if (_ignoreLegendaryNearElites != value)
                {
                    _ignoreLegendaryNearElites = value;
                    OnPropertyChanged("IgnoreLegendaryNearElites");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreNonLegendaryNearElites
        {
            get
            {
                return _ignoreRareNearElites;
            }
            set
            {
                if (_ignoreRareNearElites != value)
                {
                    _ignoreRareNearElites = value;
                    OnPropertyChanged("IgnoreNonLegendaryNearElites");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreGoldInAoE
        {
            get
            {
                return _ignoreGoldInAoE;
            }
            set
            {
                if (_ignoreGoldInAoE != value)
                {
                    _ignoreGoldInAoE = value;
                    OnPropertyChanged("IgnoreGoldInAoE");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreGoldNearElites
        {
            get
            {
                return _ignoreGoldNearElites;
            }
            set
            {
                if (_ignoreGoldNearElites != value)
                {
                    _ignoreGoldNearElites = value;
                    OnPropertyChanged("IgnoreGoldNearElites");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PickupDeathsBreath
        {
            get
            {
                return _pickupDeathsBreath;
            }
            set
            {
                if (_pickupDeathsBreath != value)
                {
                    _pickupDeathsBreath = value;
                    OnPropertyChanged("PickupDeathsBreath");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AlwaysVacuumItems
        {
            get
            {
                return _alwaysVacuumItems;
            }
            set
            {
                if (_alwaysVacuumItems != value)
                {
                    _alwaysVacuumItems = value;
                    OnPropertyChanged("AlwaysVacuumItems");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IgnoreProgressionGlobesInAoE
        {
            get
            {
                return _ignoreProgressionGlobesInAoE;
            }
            set
            {
                if (_ignoreProgressionGlobesInAoE != value)
                {
                    _ignoreProgressionGlobesInAoE = value;
                    OnPropertyChanged(nameof(IgnoreProgressionGlobesInAoE));
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreHealthGlobesInAoE
        {
            get
            {
                return _ignoreHealthGlobesInAoE;
            }
            set
            {
                if (_ignoreHealthGlobesInAoE != value)
                {
                    _ignoreHealthGlobesInAoE = value;
                    OnPropertyChanged(nameof(IgnoreHealthGlobesInAoE));
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool StashTransmog
        {
            get
            {
                return _stashTransmog;
            }
            set
            {
                if (_stashTransmog != value)
                {
                    _stashTransmog = value;
                    OnPropertyChanged(nameof(StashTransmog));
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool StashPets
        {
            get
            {
                return _stashPets;
            }
            set
            {
                if (_stashPets != value)
                {
                    _stashPets = value;
                    OnPropertyChanged(nameof(StashPets));
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool StashWings
        {
            get
            {
                return _stashWings;
            }
            set
            {
                if (_stashWings != value)
                {
                    _stashWings = value;
                    OnPropertyChanged(nameof(StashWings));
                }
            }
        }

        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(PickupSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public PickupSetting Clone()
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
            DontPickupInTown = false;
            CraftMaterials = true;
            InfernalKeys = true;
            Designs = true;
            GemLevel = 0;
            Plans = true;
            LegendaryPlans = true;
            PickupLowLevel = true;
            IgnoreTwoHandedWeapons = false;
            PickupGrayItems = true;
            PickupWhiteItems = true;
            PickupBlueFollowerItems = true;
            PickupYellowFollowerItems = true;
            PickupUpgrades = true;
            PickupBlueArmor = true;
            PickupYellowArmor = true;
            PickupBlueJewlery = true;
            PickupYellowJewlery = true;
            PickupYellowWeapons = true;
            PickupBlueWeapons = true;
            MiscItemQuality = TrinityItemQuality.Legendary;
            BloodShards = true;
            LootRunKey = true;
            PickupGold = false;
            RamadalinisGift = true;
            PickupLegendaries = true;
            LegendaryPotions = true;
            PickupLegendaryFollowerItems = true;
            PickupDeathsBreath = true;
            AutoEquipIgnoreWeapons = false;
            AutoEquipItems = true;
            AutoEquipSkills = true;
            DisableAutoEquipAtMaxLevel = true;
            IgnoreProgressionGlobesInAoE = true;
            IgnoreGoldInAoE = true;
            IgnoreLegendaryInAoE = true;
            IgnoreNonLegendaryInAoE = true;
            IgnoreHealthGlobesInAoE = true;
            StashWings = true;
            StashPets = true;
        }
        #endregion Methods
    }
}
