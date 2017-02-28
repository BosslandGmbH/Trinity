﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework.Helpers;
using Trinity.ProfileTags;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Service;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Components.Adventurer.Util.Logger;

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

            Logger.Info("[BountyStatistics] Total Time: {0:dd\\ hh\\:mm\\:ss}", totalTime);
            Logger.Info("[BountyStatistics] Average Time: {0:hh\\:mm\\:ss}", averageTime);
            Logger.Info("[BountyStatistics] Per hour: {0:0.##}", bountiesPerHour);
            Logger.Info("[BountyStatistics] Time Wasted: {0:hh\\:mm\\:ss}", timeWasted);
            Logger.Info("[BountyStatistics] Completed Acts: {0}", CompletedBountyActs);
            Logger.Info("[BountyStatistics] Unsupported Act Restarts: {0}", RestartsFromUnsupported);
            Logger.Info("[BountyStatistics] Total Bounties: {0}", count);
            Logger.Info("[BountyStatistics] Success Count: {0}", successful);
            Logger.Info("[BountyStatistics] Failed Count: {0}", failed);
            Logger.Info("[BountyStatistics] Incomplete Count: {0}", incomplete);
            Logger.Info("[BountyStatistics] Success Rate: {0:#.##}%", (successful/(double)count)*100);

            var incompleteStats = stats.Where(s => !(s.IsCompleted || s.IsFailed)).DistinctBy(s => s.QuestId).ToList();

            foreach (var item in incompleteStats)
            {
                var item1 = item;
                var incompleteCount = stats.Count(s => s.QuestId == item1.QuestId && !(s.IsCompleted || s.IsFailed));
                var successCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsCompleted);
                var failureCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsFailed);
                var wasted = stats.Where(s => s.QuestId == item1.QuestId && !(s.IsCompleted || s.IsFailed));
                var wastedAvg = wasted.Any() ? 0 : wasted.Average(s => (s.EndTime - s.StartTime).TotalSeconds);
                  
                Logger.Info($"[BountyStatistics][FailedQuest] QuestId: {item1.QuestId}, IncompleteCount: {incompleteCount},  SuccessCount: {successCount}, Act: {item1.Act}, Name: {item1.Name} TimeAvg: {wastedAvg}");
            }

            foreach (var item in incompleteStats)
            {
                Logger.Raw($"    <RunBounty questId=\"{item.QuestId}\" name=\"{item.Name}\" />");
            }

            //Logger.Info("[BountyStatistics] Slowest Bounties:");
            //foreach(var item in stats.Where(s => s.IsCompleted).OrderByDescending(s => s.Duration).Take(10))
            //{
            //    Logger.Info($"[BountyStatistics] {item.Duration:hh:mm:ss}: {item.Name} ({item.QuestId})");
            //}

            //foreach (var item in stats.Where(s => s.IsCompleted))
            //{
            //    var item1 = item;
            //    var incompleteCount = stats.Count(s => s.QuestId == item1.QuestId && !(s.IsCompleted || s.IsFailed));
            //    var successCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsCompleted);
            //    var failureCount = stats.Count(s => s.QuestId == item1.QuestId && s.IsFailed);

            //    Logger.Info($"[BountyStatistics][FailedQuest] QuestId: {item1.QuestId}, IncompleteCount: {incompleteCount},  SuccessCount: {successCount}, Act: {item1.Act}, Name: {item1.Name} TimeAvg: {wastedAvg}");
            //}

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

                var isTurnIn = BountyHelpers.IsAnyActTurninInProgress();
                if (isTurnIn != _isTurnInInProgress)
                {
                    _isTurnInInProgress = isTurnIn;
                    if (isTurnIn)
                    {
                        CompletedBountyActs++;
                    }
                }

            }
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
            var quest = ZetaDia.ActInfo.AllQuests.FirstOrDefault(q => q.QuestSNO == questId);

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
