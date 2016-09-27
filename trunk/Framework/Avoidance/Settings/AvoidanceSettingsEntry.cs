using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Avoidance.Handlers;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Helpers;

namespace Trinity.Framework.Avoidance.Settings
{
    [DataContract(Namespace = "", Name = "Avoidance")]
    public sealed class AvoidanceSettingsEntry : NotifyBase
    {
        private bool _isEnabled;
        private int _id;
        private float _distanceMultiplier;
        private int _healthPct;
        private bool _prioritize;
        private AvoidanceDefinition _definition;

        public AvoidanceSettingsEntry()
        {

        }

        public AvoidanceSettingsEntry(AvoidanceDefinition definition)
        {
            _definition = definition;
            _id = _definition.Id;
            LoadDefaults();
        }

        [IgnoreDataMember]
        public AvoidanceType Type => (AvoidanceType)Id;

        [IgnoreDataMember]
        public AvoidanceDefinition Definition => _definition ?? (_definition = AvoidanceFactory.GetAvoidanceData(_id));

        [DataMember]
        public int Id
        {
            get { return _id; }
            set { SetField(ref _id, value); }
        }

        [DefaultValue(true)]
        [DataMember]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetField(ref _isEnabled, value); }
        }

        [DefaultValue(60)]
        [DataMember(EmitDefaultValue = false)]
        public int HealthPct
        {
            get { return _healthPct; }
            set { SetField(ref _healthPct, value); }
        }

        [DefaultValue(false)]
        [DataMember(EmitDefaultValue = false)]
        public bool Prioritize
        {
            get { return _prioritize; }
            set { SetField(ref _prioritize, value); }
        }

        [DefaultValue(1)]
        [DataMember(EmitDefaultValue = false, Name = "DistM")]
        public float DistanceMultiplier
        {
            get { return _distanceMultiplier; }
            set { SetField(ref _distanceMultiplier, value); }
        }

        public override void LoadDefaults()
        {
            base.LoadDefaults();

            if (Definition.Defaults != null)
            {
                DistanceMultiplier = Definition.Defaults.DistanceMultiplier;
                Prioritize = Definition.Defaults.Prioritize;
                HealthPct = Definition.Defaults.HealthPct;
                IsEnabled = Definition.Defaults.IsEnabled;
            }
        }


        public override bool Equals(object obj) => (obj as AvoidanceSettingsEntry)?.Id == Id;
        public override int GetHashCode() => Id;
        public override string ToString() => $"{GetType().Name}: {Definition.Name}";
    }

}