using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.UI.UIComponents;
using Zeta.Common;

namespace Trinity.Framework.Avoidance.Settings
{
    [DataContract(Namespace = "")]
    public sealed class AvoidanceSettings : NotifyBase
    {
        public AvoidanceSettings()
        {
            LoadDefaults();
        }

        public AvoidanceSettingsEntry GetDefinitionSettings(AvoidanceType type)
        {
            return EntriesByType.ContainsKey(type) ? EntriesByType[type] : null;
        }

        private List<AvoidanceSettingsEntry> _entries;

        [DataMember]
        public List<AvoidanceSettingsEntry> Entries
        {
            get { return _entries; }
            set { SetField(ref _entries, value); }
        }

        [DefaultValue(1)]
        [DataMember(Name = "HighestNode")]
        public int MinimumHighestNodeWeightTrigger { get; set; }

        [DefaultValue(3f)]
        [DataMember(Name = "NearbyTotal")]
        public float MinimumNearbyWeightPctTotalTrigger { get; set; }

        [DefaultValue(0.2f)]
        [DataMember(Name = "NearbyAvg")]
        public float AvoiderNearbyPctAvgTrigger { get; set; }

        [DefaultValue(15f)]
        [DataMember(Name = "LocalRadius")]
        public float AvoiderLocalRadius { get; set; }

        [DefaultValue(WeightingOptions.All)]
        [DataMember(Name = "WeightOptions")]
        public WeightingOptions WeightingOptions { get; set; }

        public override void LoadDefaults()
        { 
            base.LoadDefaults();

            Entries = AvoidanceFactory.AvoidanceData.Select(a => new AvoidanceSettingsEntry(a)).ToList();

            foreach (var entry in Entries)
            {
                entry.LoadDefaults();
            }

            EntriesByType = Entries.ToDictionary(k => k.Type, v => v);
        }

        [IgnoreDataMember]
        public Dictionary<AvoidanceType, AvoidanceSettingsEntry> EntriesByType { get; set; }

        [IgnoreDataMember]
        public ICommand SelectAllCommand => new RelayCommand(param =>
        {
            foreach (var item in Entries)
            {
                item.IsEnabled = true;
            }
        });

        [IgnoreDataMember]
        public ICommand SelectNoneCommand => new RelayCommand(param =>
        {
            foreach (var item in Entries)
            {
                item.IsEnabled = false;
            }
        });

        [IgnoreDataMember]
        public ICommand MaxHealthCommand => new RelayCommand(param =>
        {
            foreach (var item in Entries)
            {
                item.HealthPct = 100;
            }
        });

        [IgnoreDataMember]
        public ICommand SelectDefaultsCommand => new RelayCommand(param =>
        {
            LoadDefaults();
        });

        public void Load(string code)
        {
            var newObj = JsonSerializer.Deserialize<AvoidanceSettings>(code);
            var diff = Entries.Except(newObj.Entries);
            newObj.Entries.AddRange(diff);            
            PropertyCopy.Copy(newObj, this, new PropertyCopyOptions { IgnoreNulls = true });
            EntriesByType = Entries.ToDictionary(k => k.Type, v => v);
        }

        public string Save()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}