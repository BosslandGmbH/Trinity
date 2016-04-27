using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Avoidance;
using Trinity.Helpers;

namespace Trinity.Settings
{
    public class AvoiderSettings : NotifyBase
    {
        private double _weightPctAvg;
        private double _weightPctTotal;
        private double _highestWeight;
        private double _measurementRadius;
        private AvoidanceMode _mode;

        [DataMember]
        [DefaultValue(AvoidanceMode.StayClose)]
        public AvoidanceMode Mode
        {
            get { return _mode; }
            set { SetField(ref _mode, value); }
        }

        [DataMember]
        [DefaultValue(15f)]
        public double MeasurementRadius
        {
            get { return _measurementRadius; }
            set { SetField(ref _measurementRadius, value); }
        }

        [DataMember]
        [DefaultValue(0.3)]
        public double WeightPctAvg
        {
            get { return _weightPctAvg; }
            set { SetField(ref _weightPctAvg, value); }
        }

        [DataMember]
        [DefaultValue(5)]
        public double WeightPctTotal
        {
            get { return _weightPctTotal; }
            set { SetField(ref _weightPctTotal, value); }
        }

        [DataMember]
        [DefaultValue(2)]
        public double HighestWeight
        {
            get { return _highestWeight; }
            set { SetField(ref _highestWeight, value); }
        }
    }
}
