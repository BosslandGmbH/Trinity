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
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Attributes;
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
        private GoblinPriority _goblinPriority;

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
        [DefaultValue(SettingMode.Enabled)]
        public SettingMode EliteWeighting
        {
            get { return _eliteWeighting; }
            set { SetField(ref _eliteWeighting, value); }
        }

        [DataMember]
        [Setting, UIControl(UIControlType.FlagsCheckboxes)]
        public EliteTypes EliteTypes
        {
            get { return _eliteTypes; }
            set { SetField(ref _eliteTypes, value); }
        }

        public const MonsterAffixes IgnoreAffixesExclusions = MonsterAffixes.Elite | MonsterAffixes.Minion | MonsterAffixes.Rare | MonsterAffixes.Unique;

        [DataMember(IsRequired = false)]
        [Setting, UIControl(UIControlType.FlagsCheckboxes, UIControlOptions.Inline | UIControlOptions.NoLabel)]
        [FlagExclusion(IgnoreAffixesExclusions)]
        public MonsterAffixes IgnoreAffixes
        {
            get { return _ignoreAffixes; }
            set { SetField(ref _ignoreAffixes, value); }
        }

        [DataMember]
        [DefaultValue(GoblinPriority.Kamikaze)]
        public GoblinPriority GoblinPriority
        {
            get { return _goblinPriority; }
            set { SetField(ref _goblinPriority, value); }
        }

    }
}
