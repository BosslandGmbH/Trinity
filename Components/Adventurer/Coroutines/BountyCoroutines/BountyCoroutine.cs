using Buddy.Coroutines;
using System;
using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Common.Helpers;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines
{
    public abstract class BountyCoroutine
    {
        private bool _isDone;
        private States _state;
        private BountyData _bountyData;

        public SNOQuest QuestId { get; private set; }
        public int BoxSize { get; set; }
        public float BoxTolerance { get; set; }
        public bool ZergMode = false;
        public int ObjectiveSearchRadius { get; set; }
        public bool AutoSetNearbyNodesExplored { get; set; }
        public int AutoSetNearbyNodesRadius { get; set; }
        public bool IsDone => _isDone;
        public BountyStatistic Stats { get; private set; }

        public BountyData BountyData => _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(QuestId));

        public QuestData QuestData => BountyData.QuestData;

        public bool IsInZone
        {
            get
            {
                var wp = ZetaDia.Storage.ActManager.GetWaypointByLevelAreaSnoId(BountyData.WaypointLevelAreaId);
                if (wp != null && WaypointFactory.NearWaypoint(wp.Number))
                    return true;

                if ((BountyData.LevelAreaIds != null && BountyData.LevelAreaIds.Contains(AdvDia.CurrentLevelAreaId)))
                    return true;

                if (ZetaDia.Storage.Quests.ActiveBounty != null && ZetaDia.Storage.Quests.ActiveBounty.Quest == QuestId)
                    return true;

                return false;
            }
        }

        #region State

        public enum States
        {
            NotStarted,
            TakingWaypoint,
            InZone,
            BountyMain,
            Completed,
            Failed
        }

        protected bool _logStateChange;

        protected bool LogStateChange
        {
            get
            {
                if (!_logStateChange) return false;
                _logStateChange = false;
                return true;
            }
        }

        public States State
        {
            get => _state;
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[Bounty] " + value);
                }
                _logStateChange = true;
                _state = value;
            }
        }

        #endregion State

        protected BountyCoroutine(SNOQuest questId)
        {
            QuestId = questId;
            BoxSize = 30;
            BoxTolerance = 0.05f;
        }

        public virtual async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.TakingWaypoint:
                    return await TakingWaypoint();

                case States.InZone:
                    return await InZone();

                case States.BountyMain:
                    return await BountyMain();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }

        private WaitTimer _returningToTownWaitTimer;

        private async Task<bool> NotStarted()
        {
            if (_returningToTownWaitTimer == null)
            {
                _returningToTownWaitTimer = new WaitTimer(TimeSpan.FromSeconds(5));
                _returningToTownWaitTimer.Reset();
            }
            if (!_returningToTownWaitTimer.IsFinished) return false;
            _returningToTownWaitTimer = null;

            Stats = BountyStatistic.GetInstance(QuestId);

            LastBountyStats = Stats;

            if (Stats.StartTime == default(DateTime))
                Stats.StartTime = DateTime.UtcNow;

            if (Stats.EndTime == default(DateTime))
                Stats.EndTime = DateTime.UtcNow;

            Core.Logger.Log("[Bounty] Starting {0} ({1})", QuestData.Name, QuestId);
            if (IsInZone)
            {
                State = States.InZone;
            }
            else
            {
                Core.Logger.Debug("[Bounty] Using waypoint to reach one of the bounty LevelAreaSnoIdIds: {0}", string.Join(", ", BountyData.LevelAreaIds));
                State = States.TakingWaypoint;
            }
            return false;
        }

        public static BountyStatistic LastBountyStats { get; set; }

        private async Task<bool> TakingWaypoint()
        {
            if (!await WaypointCoroutine.UseWaypoint(BountyData.WaypointNumber))
                return false;

            State = States.InZone;
            return false;
        }

        private async Task<bool> InZone()
        {
            State = States.BountyMain;
            return false;
        }

        private async Task<bool> BountyMain()
        {
            return false;
        }

        private WaitTimer _completedWaitTimer;

        private async Task<bool> Completed()
        {
            if (_completedWaitTimer == null) _completedWaitTimer = QuestId == SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael ? new WaitTimer(TimeSpan.FromSeconds(15)) : new WaitTimer(TimeSpan.FromSeconds(3));
            if (!_completedWaitTimer.IsFinished)
                return false;

            _completedWaitTimer = null;
            _isDone = true;
            if (Stats == null) return true;

            Stats.EndTime = DateTime.UtcNow;
            Stats.IsCompleted = true;
            Core.Logger.Log("[Bounty] Completed {0} ({1}) Time {2:hh\\:mm\\:ss}", QuestData.Name, QuestId, Stats.EndTime - Stats.StartTime);

            return true;
        }

        private async Task<bool> Failed()
        {
            Core.Logger.Error("[Bounty] Failed {0} ({1})", QuestData.Name, QuestId);
            _isDone = true;
            Stats.EndTime = DateTime.UtcNow;
            Stats.IsFailed = true;
            await Coroutine.Yield();
            return true;
        }

        protected void CheckBountyStatus()
        {
            if (BountyData.IsAvailable) return;

            State = States.Completed;
            foreach (var coroutine in BountyData.Coroutines)
            {
                coroutine.DisablePulse();
            }
        }

        public virtual void Reset()
        {
            State = States.NotStarted;
            _bountyData = null;
        }
    }
}
