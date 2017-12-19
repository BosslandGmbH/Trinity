using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Util;
using Zeta.Game;
using Zeta.Game.Internals.Service;
using Zeta.Game.Internals.SNO;


namespace Trinity.Components.Adventurer.Game.Quests
{
    public static class BountyStatistics
    {
        public static List<BountyStatistic> Stats { get; } = new List<BountyStatistic>();

        public static void Report()
        {
            // Exclude the current bounty, it didn't really fail if we hit the stop button.
            var stats = Stats.Where(s => !s.WasLastInProgress).ToList();

            var count = stats.Count;
            if (count == 0) return;
            var totalTime = TimeSpan.FromSeconds(stats.Sum(s => (s.EndTime - s.StartTime).TotalSeconds));
            var averageTime = TimeSpan.FromSeconds(stats.Average(s => (s.EndTime - s.StartTime).TotalSeconds));
            var timeWasted = TimeSpan.FromSeconds(stats.Where(s => s.IsIncomplete || s.IsFailed).Sum(s => (s.EndTime - s.StartTime).TotalSeconds));
            var bountiesPerHour = count / totalTime.TotalHours;
            var successful = stats.Count(s => s.IsCompleted);
            var failed = stats.Count(s => s.IsFailed);
            var incomplete = stats.Count(s => s.IsIncomplete);

            Core.Logger.Log("[悬赏统计] 共耗时: {0:dd\\ hh\\:mm\\:ss}", totalTime);
            Core.Logger.Log("[悬赏统计] 平均时间: {0:hh\\:mm\\:ss}", averageTime);
            Core.Logger.Log("[悬赏统计] 每小时: {0:0.##}", bountiesPerHour);
            Core.Logger.Log("[悬赏统计] 浪费时间: {0:hh\\:mm\\:ss}", timeWasted);
            Core.Logger.Log("[悬赏统计] 已完成章节: {0}", CompletedBountyActs);
            Core.Logger.Log("[悬赏统计] 不支持的章节重新开始: {0}", RestartsFromUnsupported);
            Core.Logger.Log("[悬赏统计] 悬赏总数: {0}", count);
            Core.Logger.Log("[悬赏统计] 成功次数: {0}", successful);
            Core.Logger.Log("[悬赏统计] 失败次数: {0}", failed);
            Core.Logger.Log("[悬赏统计] 不完整数量: {0}", incomplete);
            Core.Logger.Log("[悬赏统计] 成功率: {0:#.##}%", (successful / (double)count) * 100);

            var incompleteStats = stats.Where(s => !(s.IsCompleted || s.IsFailed)).DistinctBy(s => s.QuestId).ToList();

            foreach (var item in incompleteStats)
            {
                var item1 = item;
                var incompleteCount = stats.Count(s => s.QuestId == item1.QuestId && !(s.IsCompleted || s.IsFailed));
                var successCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsCompleted);
                var failureCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsFailed);
                var wasted = stats.Where(s => s.QuestId == item1.QuestId && !(s.IsCompleted || s.IsFailed));
                var wastedAvg = wasted.Any() ? 0 : wasted.Average(s => (s.EndTime - s.StartTime).TotalSeconds);

                Core.Logger.Log($"[悬赏统计][失败的任务]任务 Id: {item1.QuestId}, 不完整 : {incompleteCount},  成功 : {successCount}, 章节 : {item1.Act}, 名称 : {item1.Name} 平均时间  : {wastedAvg}");
            }

            foreach (var item in incompleteStats)
            {
                Core.Logger.Raw($"    <RunBounty questId=\"{item.QuestId}\" name=\"{item.Name}\" />");
            }

            //Core.Logger.Log("[BountyStatistics] Slowest Bounties:");
            //foreach(var item in stats.Where(s => s.IsCompleted).OrderByDescending(s => s.Duration).Take(10))
            //{
            //    Core.Logger.Log($"[BountyStatistics] {item.Duration:hh:mm:ss}: {item.Name} ({item.QuestId})");
            //}

            //foreach (var item in stats.Where(s => s.IsCompleted))
            //{
            //    var item1 = item;
            //    var incompleteCount = stats.Count(s => s.QuestId == item1.QuestId && !(s.IsCompleted || s.IsFailed));
            //    var successCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsCompleted);
            //    var failureCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsFailed);
            //    Core.Logger.Log($"[BountyStatistics][FailedQuest] QuestId: {item1.QuestId}, IncompleteCount: {incompleteCount},  SuccessCount: {successCount}, Act: {item1.Act}, Name: {item1.Name} TimeAvg: {wastedAvg}");
            //}
        }

        private static long _lastPulseTime;

        public static void Pulse()
        {
            //if (!Core.TrinityIsReady) return;

            //if (PluginTime.ReadyToUse(_lastPulseTime, 2000))
            //{
            //    GameId gameId = default(GameId);
            //    _lastPulseTime = PluginTime.CurrentMillisecond;
            //    if (gameId.FactoryId == 0)
            //        return;

            //    foreach (var bountyStatistic in Stats.Where(s => !(s.IsCompleted || s.IsFailed) && s.GameId == gameId))
            //    {
            //        bountyStatistic.LastSeen = DateTime.UtcNow;
            //    }

            //    var isTurnIn = BountyHelpers.IsAnyActTurninInProgress();
            //    if (isTurnIn != _isTurnInInProgress)
            //    {
            //        _isTurnInInProgress = isTurnIn;
            //        if (isTurnIn)
            //        {
            //            CompletedBountyActs++;
            //        }
            //    }
            //}
        }

        public static IEnumerable<BountyStatistic> CurrentGame => Stats.Where(b => ZetaDia.Service.CurrentGameId == b.GameId);

        public static int CompletedBountyActs { get; internal set; }

        public static int RestartsFromUnsupported { get; internal set; }
 
        private static bool _isTurnInInProgress;

        public static void Reset()
        {
            Stats.Clear();
            CompletedBountyActs = 0;
            RestartsFromUnsupported = 0;
        }
    }

    public class BountyStatistic
    {
        private DateTime _endTime;
        public GameId GameId { get; set; }
        public int QuestId { get; set; }
        public DateTime StartTime { get; set; }
        public string Name { get; set; }
        public string Steps { get; set; }
        public Act Act { get; set; }
        public QuestType QuestType { get; set; }

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

        public bool IsIncomplete => !(IsCompleted || IsFailed);
        public bool WasLastInProgress => !IsFailed && this == BountyCoroutine.LastBountyStats;
        public TimeSpan Duration => (EndTime - StartTime);

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
            var quest = ZetaDia.Storage.Quests.AllQuests.FirstOrDefault(q => q.QuestSNO == questId);

            var steps = quest?.QuestRecord?.Steps.Aggregate(string.Empty,
                (str, cur) => str + cur.QuestStepObjectiveSet.QuestStepObjectives.Aggregate(string.Empty,
                        (str2, cur2) => str2 + $"{cur2.StepObjectiveName} ({cur2.ObjectiveType}), "));

            stat = new BountyStatistic
            {
                QuestId = questId,
                GameId = gameId,
                Name = quest?.DisplayName,
                QuestType = quest?.QuestType ?? default(QuestType),
                Act = quest?.QuestRecord?.Act ?? default(Act),
                Steps = steps
            };
            BountyStatistics.Stats.Add(stat);
            return stat;
        }
    }
}