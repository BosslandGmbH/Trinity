using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Util;
using Zeta.Game;
using Zeta.Game.Internals.Service;

namespace Trinity.Components.Adventurer.Game.Quests
{
    public static class BountyStatistics
    {
        public static List<BountyStatistic> Stats = new List<BountyStatistic>();

        public static void Report()
        {
            var count = Stats.Count;
            if (count == 0) return;
            var totalTime = TimeSpan.FromSeconds(Stats.Sum(s => (s.EndTime - s.StartTime).TotalSeconds));
            var averageTime = TimeSpan.FromSeconds(Stats.Average(s => (s.EndTime - s.StartTime).TotalSeconds));
            var bountiesPerHour = count / totalTime.TotalHours;
            var successful = Stats.Count(s => s.IsCompleted);
            var failed = Stats.Count(s => s.IsFailed);
            var incomplete = Stats.Count(s => !(s.IsCompleted || s.IsFailed));
            Logger.Info("[BountyStatistics] Total Time: {0:dd\\ hh\\:mm\\:ss}", totalTime);
            Logger.Info("[BountyStatistics] Average Time: {0:hh\\:mm\\:ss}", averageTime);
            Logger.Info("[BountyStatistics] Per hour: {0:0.##}", bountiesPerHour);
            Logger.Info("[BountyStatistics] Total Count: {0}", count);
            Logger.Info("[BountyStatistics] Success Count: {0}", successful);
            Logger.Info("[BountyStatistics] Incomplete Count: {0}", incomplete);
            Logger.Info("[BountyStatistics] Success Rate: {0:P}", successful/(double)count);

            foreach (var item in Stats.Where(s => !(s.IsCompleted || s.IsFailed)))
            {
                var item1 = item;
                var incompleteCount = Stats.Count(s => s.QuestId == item1.QuestId && !(s.IsCompleted || s.IsFailed));
                var successCount = Stats.Count(s => s.QuestId == item1.QuestId && s.IsCompleted);
                var failureCount = Stats.Count(s => s.QuestId == item1.QuestId && s.IsFailed);
                Logger.Info("[BountyStatistics][FailedQuest] QuestId: {0}, IncompleteCount: {1},  SuccessCount: {3}", item1.QuestId, incompleteCount, failureCount, successCount);
            }

        }

        private static long _lastPulseTime;
        public static void Pulse()
        {
            if (PluginTime.ReadyToUse(_lastPulseTime, 2000))
            {
                _lastPulseTime = PluginTime.CurrentMillisecond;
                var gameId = ZetaDia.Service.CurrentGameId;
                foreach (var bountyStatistic in Stats.Where(s => !(s.IsCompleted || s.IsFailed) && s.GameId == gameId))
                {
                    bountyStatistic.LastSeen = DateTime.UtcNow;
                }
            }
        }
    }

    public class BountyStatistic
    {
        private DateTime _endTime;
        public GameId GameId { get; set; }
        public int QuestId { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime EndTime
        {
            get
            {
                return _endTime == default(DateTime) ? LastSeen : _endTime;
            }
            set { _endTime = value; }
        }

        public DateTime LastSeen { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }

        private BountyStatistic()
        {

        }

        public static BountyStatistic GetInstance(int questId)
        {
            var gameId = ZetaDia.Service.CurrentGameId;
            var stat = BountyStatistics.Stats.FirstOrDefault(s => s.QuestId == questId && s.GameId == gameId);
            if (stat != null)
            {
                return stat;
            }
            stat = new BountyStatistic { QuestId = questId, GameId = gameId };
            BountyStatistics.Stats.Add(stat);
            return stat;
        }
    }
}
