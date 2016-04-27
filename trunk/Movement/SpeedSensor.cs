using System;
using Zeta.Common;

namespace Trinity
{
    internal class SpeedSensor : IComparable<SpeedSensor>
    {
        public Vector3 Location { get; set; }
        public TimeSpan TimeSinceLastMove { get; set; }
        public double Distance { get; set; }
        public int WorldID { get; set; }
        public DateTime Timestamp { get; set; }

        public SpeedSensor()
        {
            this.Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Comparison is done via the Timestamp property
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SpeedSensor other)
        {
            return Timestamp.CompareTo(other.Timestamp);
        }
    }
}
