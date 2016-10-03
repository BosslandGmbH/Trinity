using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Coroutines.Resources;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Settings.Loot;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class KanaisCubeSetting : NotifyBase
    {
        private ItemSelectionType _rareUpgradeTypes;
        private int _conversionQuantityThreshold;
        private bool _createVeiledCrystals;
        private bool _createReusableParts;
        private bool _createArcaneDust;
        private int _deathsBreathMinimum;
        private CubeExtractOption _extractLegendaryPowers;
        private bool _cubeExtractFromStash;

        public KanaisCubeSetting()
        {
            base.LoadDefaults();
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CreateReusableParts
        {
            get { return _createReusableParts; }
            set { SetField(ref _createReusableParts, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CreateVeiledCrystals
        {
            get { return _createVeiledCrystals; }
            set { SetField(ref _createVeiledCrystals, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CreateArcaneDust
        {
            get { return _createArcaneDust; }
            set { SetField(ref _createArcaneDust, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(25000)]
        public int ConversionQuantityThreshold
        {
            get { return _conversionQuantityThreshold; }
            set { SetField(ref _conversionQuantityThreshold, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(0)]
        public int DeathsBreathMinimum
        {
            get { return _deathsBreathMinimum; }
            set { SetField(ref _deathsBreathMinimum, value); }
        }


        [DataMember(IsRequired = false)]
        [DefaultValue(default(ItemSelectionType))]
        public ItemSelectionType RareUpgradeTypes
        {
            get { return _rareUpgradeTypes; }
            set { SetField(ref _rareUpgradeTypes, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(CubeExtractOption.None)]
        public CubeExtractOption ExtractLegendaryPowers
        {
            get { return _extractLegendaryPowers; }
            set { SetField(ref _extractLegendaryPowers, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool CubeExtractFromStash
        {
            get { return _cubeExtractFromStash; }
            set { SetField(ref _cubeExtractFromStash, value); }
        }

        public HashSet<ItemSelectionType> GetRareUpgradeSettings()
        {
            return new HashSet<ItemSelectionType>(Enum.GetValues(RareUpgradeTypes.GetType()).Cast<Enum>().Where(RareUpgradeTypes.HasFlag).Cast<ItemSelectionType>());
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

    }
}
