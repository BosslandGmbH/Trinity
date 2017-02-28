using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Attributes;
using Trinity.UI;
using Trinity.UI.UIComponents;
using Zeta.Game.Internals.Actors;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class ItemSettings : NotifyBase
    {
        private SettingMode _gamblingMode;
        private GambleSlotTypes _gamblingTypes;
        private int _gamblingMinShards;
        private DropInTownOption _dropInTownMode;
        private LegendaryMode _legendaryMode;
        private bool _dontPickupInTown;
        private SpecialItemTypes _specialItems;
        private TrinityGemType _gemTypes;
        private int _gemLevel;
        private PickupItemQualities _pickupTypes;
        private bool _dontWalkToLowQuality;
        private bool _keepLegendaryUnid;
        private bool _pickupGold;
        private int _minGoldStack;
        private bool _autoEquipSkills;
        private bool _autoEquipAutoDisable;
        private bool _autoEquipItems;
        private bool _autoEquipIgnoreWeapons;
        private int _gamblingMinSpendingShards;
        private bool _stashTreasureBags;
        //private PickupItemQualities _inCombatLootQualities;
        //private SettingMode _inCombatLooting;
        private bool _disableLootingInCombat;
        private bool _useTypeStashingEquipment;
        private bool _useTypeStashingOther;


        public const int SchemaVersion = 1;
        private bool _buyStashTabs;

        [DataMember]
        public int Version { get; set; }

        public ItemSettings()
        {
            base.LoadDefaults();

        }

        public override void OnPopulated()
        {
            Migration();
        }

        private void Migration()
        {
            if (Version < 1)
            {
                SpecialItems |= SpecialItemTypes.TieredLootrunKey; // added .680
            }
            Version = SchemaVersion;
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        [DefaultValue(PickupItemQualities.None)]
        public PickupItemQualities PickupQualities
        {
            get { return _pickupTypes; }
            set { SetField(ref _pickupTypes, value); }
        }

        //[DataMember]
        //[Setting, UIControl(UIControlType.FlagsCheckboxes)]
        //public PickupItemQualities InCombatLootQualities
        //{
        //    get { return _inCombatLootQualities; }
        //    set { SetField(ref _inCombatLootQualities, value); }
        //}

        [DataMember]
        [DefaultValue(true)]
        public bool BuyStashTabs
        {
            get { return _buyStashTabs; }
            set { SetField(ref _buyStashTabs, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool DontWalkToLowQuality
        {
            get { return _dontWalkToLowQuality; }
            set { SetField(ref _dontWalkToLowQuality, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        [DefaultValue(SpecialItemTypes.Defaults)]
        public SpecialItemTypes SpecialItems
        {
            get { return _specialItems; }
            set { SetField(ref _specialItems, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        [DefaultValue(TrinityGemType.All)]
        public TrinityGemType GemTypes
        {
            get { return _gemTypes; }
            set { SetField(ref _gemTypes, value); }
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public GambleSlotTypes GamblingTypes
        {
            get { return _gamblingTypes; }
            set { SetField(ref _gamblingTypes, value); }
        }

        [DataMember]
        [DefaultValue(SettingMode.Enabled)]
        public SettingMode GamblingMode
        {
            get { return _gamblingMode; }
            set { SetField(ref _gamblingMode, value); }
        }

        //[DataMember]
        //[DefaultValue(SettingMode.Enabled)]
        //public SettingMode InCombatLooting
        //{
        //    get { return _inCombatLooting; }
        //    set { SetField(ref _inCombatLooting, value); }
        //}

        [DataMember]
        [DefaultValue(DropInTownOption.None)]
        public DropInTownOption DropInTownMode
        {
            get { return _dropInTownMode; }
            set { SetField(ref _dropInTownMode, value); }
        }

        [DataMember]
        [DefaultValue(LegendaryMode.AlwaysStash)]
        public LegendaryMode LegendaryMode
        {
            get { return _legendaryMode; }
            set { SetField(ref _legendaryMode, value); }
        }

        [DataMember]
        [DefaultValue(25)]
        public int GamblingMinShards
        {
            get { return _gamblingMinShards; }
            set { SetField(ref _gamblingMinShards, value); }
        }

        [DataMember]
        [DefaultValue(100)]
        public int GamblingMinSpendingShards
        {
            get { return _gamblingMinSpendingShards; }
            set { SetField(ref _gamblingMinSpendingShards, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool DontPickupInTown
        {
            get { return _dontPickupInTown; }
            set { SetField(ref _dontPickupInTown, value); }
        }

        [DataMember]
        [DefaultValue(15)]
        public int GemLevel
        {
            get { return _gemLevel; }
            set { SetField(ref _gemLevel, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool KeepLegendaryUnid
        {
            get { return _keepLegendaryUnid; }
            set { SetField(ref _keepLegendaryUnid, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool StashTreasureBags
        {
            get { return _stashTreasureBags; }
            set { SetField(ref _stashTreasureBags, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool PickupGold
        {
            get { return _pickupGold; }
            set { SetField(ref _pickupGold, value); }
        }

        [DataMember]
        [DefaultValue(200)]
        public int MinGoldStack
        {
            get { return _minGoldStack; }
            set { SetField(ref _minGoldStack, value); }
        }

        [DataMember]
        [DefaultValue(true)]
        public bool AutoEquipItems
        {
            get { return _autoEquipItems; }
            set { SetField(ref _autoEquipItems, value); }
        }

        [DataMember]
        [DefaultValue(true)]
        public bool AutoEquipSkills
        {
            get { return _autoEquipSkills; }
            set { SetField(ref _autoEquipSkills, value); }
        }

        [DataMember]
        [DefaultValue(true)]
        public bool AutoEquipAutoDisable
        {
            get { return _autoEquipAutoDisable; }
            set { SetField(ref _autoEquipAutoDisable, value); }
        }

        [DataMember]
        [DefaultValue(true)]
        public bool AutoEquipIgnoreWeapons
        {
            get { return _autoEquipIgnoreWeapons; }
            set { SetField(ref _autoEquipIgnoreWeapons, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool DisableLootingInCombat
        {
            get { return _disableLootingInCombat; }
            set { SetField(ref _disableLootingInCombat, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool UseTypeStashingEquipment
        {
            get { return _useTypeStashingEquipment; }
            set { SetField(ref _useTypeStashingEquipment, value); }
        }

        [DataMember]
        [DefaultValue(false)]
        public bool UseTypeStashingOther
        {
            get { return _useTypeStashingOther; }
            set { SetField(ref _useTypeStashingOther, value); }
        }



    }


}
