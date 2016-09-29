using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;

namespace Trinity.Settings.Modals
{
    [DataContract(Namespace = "")]
    public class SettingsSelectionItem : NotifyBase
    {
        private bool _isSelected;
        private bool _isEnabled;
        private string _sectionName;
        private SettingsSection _section;

        public SettingsSelectionItem(SettingsSection section, string name = "")
        {
            Section = section;
            SectionName = string.IsNullOrEmpty(name) ? section.ToString() : name;
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(SettingsSection.None)]
        public SettingsSection Section
        {
            get { return _section; }
            set { SetField(ref _section, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue("")]
        public string SectionName
        {
            get { return _sectionName; }
            set { SetField(ref _sectionName, value); }
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

        public override string ToString() => Section == SettingsSection.Dynamic ? $"{SectionName} (Dynamic)" : SectionName;
    }
}
