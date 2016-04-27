using System;
using Zeta.Common;

namespace Trinity.Combat.Abilities
{
    class SpellHistoryItem
    {
        public TrinityPower Power { get; set; }
        public DateTime UseTime { get; set; }
        public Vector3 MyPosition { get; set; }
        public Vector3 TargetPosition { get; set; }

        public TimeSpan TimeSinceUse
        {
            get
            {
                return DateTime.UtcNow.Subtract(UseTime);
            }
        }

        public TimeSpan TimeDistanceFrom(SpellHistoryItem other)
        {
            if (other.UseTime < this.UseTime)
                return other.UseTime.Subtract(this.UseTime);
            else
                return this.UseTime.Subtract(other.UseTime);
        }

    }
}
