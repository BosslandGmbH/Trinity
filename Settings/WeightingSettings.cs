using Trinity.Framework.Helpers;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.Serialization;
using Trinity.Framework.Objects;
using Trinity.UI.UIComponents;
using Zeta.Game.Internals.Actors;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class WeightingSettings : NotifyBase
    {
        public WeightingSettings()
        {
            base.LoadDefaults();
        }

        private SettingMode _shrineWeighting;
        private ShrineTypes _shrineTypes;
        private SettingMode _containerWeighting;
        private ContainerTypes _containerTypes;
        private GlobeTypes _globeTypes;
        private SettingMode _globeWeighting;
        private SettingMode _eliteWeighting;
        private EliteTypes _eliteTypes;
        private MonsterAffixes _ignoreAffixes;
        private TargetPriority _goblinPriority;
        private SpecialTypes _specialTypes;
        private SettingMode _trashWeighting;

        [DataMember]
        [DefaultValue(SettingMode.Enabled)]
        public SettingMode ShrineWeighting
        {
            get { return _shrineWeighting; }
            set { SetField(ref _shrineWeighting, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public ShrineTypes ShrineTypes
        {
            get { return _shrineTypes; }
            set { SetField(ref _shrineTypes, value); }
        }

        [DataMember]
        [DefaultValue(SettingMode.Disabled)]
        public SettingMode ContainerWeighting
        {
            get { return _containerWeighting; }
            set { SetField(ref _containerWeighting, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public ContainerTypes ContainerTypes
        {
            get { return _containerTypes; }
            set { SetField(ref _containerTypes, value); }
        }

        [DataMember]
        [DefaultValue(SettingMode.Enabled)]
        public SettingMode GlobeWeighting
        {
            get { return _globeWeighting; }
            set { SetField(ref _globeWeighting, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public GlobeTypes GlobeTypes
        {
            get { return _globeTypes; }
            set { SetField(ref _globeTypes, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]     
        public SpecialTypes SpecialTypes
        {
            get { return _specialTypes; }
            set { SetField(ref _specialTypes, value); }
        }

        [DataMember]
        [DefaultValue(SettingMode.Enabled)]
        public SettingMode EliteWeighting
        {
            get { return _eliteWeighting; }
            set { SetField(ref _eliteWeighting, value); }
        }

        [DataMember]
        [DefaultValue(SettingMode.Enabled)]
        public SettingMode TrashWeighting
        {
            get { return _trashWeighting; }
            set { SetField(ref _trashWeighting, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public EliteTypes EliteTypes
        {
            get { return _eliteTypes; }
            set { SetField(ref _eliteTypes, value); }
        }

        public const MonsterAffixes IgnoreAffixesExclusions = MonsterAffixes.Elite | MonsterAffixes.Minion | MonsterAffixes.Rare | MonsterAffixes.Unique;
        private TargetPriority _cosmeticPriority;

        [DataMember(IsRequired = false)]
        [Setting, UIControl(UIControlType.FlagsCheckboxes, UIControlOptions.Inline | UIControlOptions.NoLabel)]
        [FlagExclusion(IgnoreAffixesExclusions)]
        public MonsterAffixes IgnoreAffixes
        {
            get { return _ignoreAffixes; }
            set { SetField(ref _ignoreAffixes, value); }
        }

        [DataMember]
        [DefaultValue(TargetPriority.Kamikaze)]
        public TargetPriority GoblinPriority
        {
            get { return _goblinPriority; }
            set { SetField(ref _goblinPriority, value); }
        }

        [DataMember]
        [DefaultValue(TargetPriority.Kamikaze)]
        public TargetPriority CosmeticPriority
        {
            get { return _cosmeticPriority; }
            set { SetField(ref _cosmeticPriority, value); }
        }


    }
}
