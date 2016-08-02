using System;

namespace Trinity.Components.Combat
{
    public class TargettingInfo
    {
        public string ObjectHash;
        public int RActorGuid;
        public string Name;
        public DateTime CreatedTime = DateTime.UtcNow;
        public DateTime LastTargetedTime;
        public int TargetedTimes;
        public int BlacklistedTimes;
        public TimeSpan TotalTimeAsCurrentTarget;
        public DateTime TimeLastTargetted;

        public override string ToString()
        {
            return base.ToString() + $"TimeAsCurrentTarget(ms)={TotalTimeAsCurrentTarget.TotalMilliseconds:N2}";
        }
    }
}