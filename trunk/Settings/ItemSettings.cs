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
        private PickupItemQualities _pickupItemTypes;
        private bool _keepLegendaryUnid;
        private bool _pickupGold;
        private int _minGoldStack;
        private bool _autoEquipSkills;
        private bool _autoEquipAutoDisable;
        private bool _autoEquipItems;
        private bool _autoEquipIgnoreWeapons;
        private int _freeBagSlotsInTown;
        private int _freeBagSlots;

        public ItemSettings()
        {
            base.LoadDefaults();
        }

        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        [DefaultValue(PickupItemQualities.Defaults)]
        public PickupItemQualities PickupItemQualities
        {
            get { return _pickupItemTypes; }
            set { SetField(ref _pickupItemTypes, value); }
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
        [DefaultValue(5)]
        public int FreeBagSlots
        {
            get { return _freeBagSlots; }
            set { SetField(ref _freeBagSlots, value); }
        }    

        [DataMember]
        [DefaultValue(40)]
        public int FreeBagSlotsInTown
        {
            get { return _freeBagSlotsInTown; }
            set { SetField(ref _freeBagSlotsInTown, value); }
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

    }


}
