using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Helpers;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class SettingsSelectionItem : NotifyBase
    {
        private bool _isSelected;
        private bool _isEnabled;
        private SettingsSection _section;

        public SettingsSelectionItem(SettingsSection item)
        {
            Section = item;
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(SettingsSection.None)]
        public SettingsSection Section
        {
            get { return _section; }
            set { SetField(ref _section, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetField(ref _isSelected, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetField(ref _isEnabled, value); }
        }
    }
}