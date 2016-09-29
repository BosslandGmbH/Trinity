using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Zeta.Game;

namespace Trinity.Components.Combat.Resources
{
    public class SnapShot
    {
        public static SnapShotRecord Last => SnapShots.LastOrDefault();

        public static double TimeSinceLastMs => Last != null ? DateTime.UtcNow.Subtract(Last.SnapShotTime).TotalMilliseconds : -1;

        public class SnapShotRecord
        {
            public double AttacksPerSecond { get; set; }
            public DateTime SnapShotTime { get; set; }
        }

        public static List<SnapShotRecord> SnapShots = new List<SnapShotRecord>();

        public static void Record()
        {
            var snapshotRecord = new SnapShotRecord
            {
                AttacksPerSecond = ZetaDia.Me.AttacksPerSecond,
                SnapShotTime = DateTime.UtcNow
            };

            Logger.LogVerbose("Recorded Snapshot {0} AttacksPerSecond={1}",
                snapshotRecord.SnapShotTime, snapshotRecord.AttacksPerSecond);

            SnapShots.Add(snapshotRecord);

            if (SnapShots.Count > 10)
                SnapShots.Remove(SnapShots.First());
        }
    }
}




