using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.Technicals;
using Trinity.UIComponents;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class WeightingSettings : NotifyBase, ITrinitySetting<WeightingSettings>
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

        #region ITrinitySetting

        public void Reset() => TrinitySetting.Reset(this);        
        public void CopyTo(WeightingSettings setting) => TrinitySetting.CopyTo(this, setting);
        public WeightingSettings Clone() => TrinitySetting.Clone(this);

        #endregion
    }
}
