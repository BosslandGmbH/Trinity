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

        public int QuestId { get; private set; }
        public int BoxSize { get; set; }
        public float BoxTolerance { get; set; }
        public bool ZergMode = false;
        public int ObjectiveSearchRadius { get; set; }
        public bool AutoSetNearbyNodesExplored { get; set; }
        public int AutoSetNearbyNodesRadius { get; set; }
        public bool IsDone { get { return _isDone; } }
        public BountyStatistic Stats { get; private set; }

        public BountyData BountyData
        {
            get { return _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(QuestId)); }
        }

        public QuestData QuestData
        {
            get { return BountyData.QuestData; }
        }

        public bool IsInZone
        {
            get
            {
                if (WaypointFactory.NearWaypoint(BountyData.WaypointNumber))
                {
                    return true;
                }
                if ((BountyData.LevelAreaIds != null && BountyData.LevelAreaIds.Contains(AdvDia.CurrentLevelAreaId)))
                {
                    return true;
                }
                if (ZetaDia.Storage.Quests.ActiveBounty != null && (int)ZetaDia.Storage.Quests.ActiveBounty.Quest == QuestId)
                {
                    return true;
                }
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
            get { return _state; }
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

        protected BountyCoroutine(int questId)
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
            //if (!IsAvailable)
            //{
            //    _isDone = true;
            //    return true;
            //}
            Stats = BountyStatistic.GetInstance(QuestId);

            LastBountyStats = Stats;

            if (Stats.StartTime == default(DateTime))
                Stats.StartTime = DateTime.UtcNow;

            if (Stats.EndTime == default(DateTime))
                Stats.EndTime = DateTime.UtcNow;

            Core.Logger.Log("[悬赏] 开始 {0} ({1})", QuestData.Name, QuestId);
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
            if (!await WaypointCoroutine.UseWaypoint(BountyData.WaypointNumber)) return false;
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

        public static int currentRandomizedBounty = -1;
        private WaitTimer _completedWaitTimer;

        private async Task<bool> Completed()
        {
            if (_completedWaitTimer == null) _completedWaitTimer = QuestId == 359927 ? new WaitTimer(TimeSpan.FromSeconds(15)) : new WaitTimer(TimeSpan.FromSeconds(3));
            if (!_completedWaitTimer.IsFinished) return false;
            _completedWaitTimer = null;
            _isDone = true;
            if (Stats != null)
            {
                Stats.EndTime = DateTime.UtcNow;
                Stats.IsCompleted = true;
                Core.Logger.Log("[悬赏] 已完成 {0} ({1}) 时间 {2:hh\\:mm\\:ss}", QuestData.Name, QuestId, Stats.EndTime - Stats.StartTime);
                currentRandomizedBounty = -1;
            }

            return true;
        }

        private async Task<bool> Failed()
        {
            Core.Logger.Error("[悬赏] 失败 {0} ({1})", QuestData.Name, QuestId);
            _isDone = true;
            Stats.EndTime = DateTime.UtcNow;
            Stats.IsFailed = true;
            await Coroutine.Sleep(1000);
            return true;
        }

        protected void CheckBountyStatus()
        {
            if (!BountyData.IsAvailable)
            {
                State = States.Completed;
                foreach (var coroutine in BountyData.Coroutines)
                {
                    coroutine.DisablePulse();
                }
                return;
            }
            //if (State != States.NotStarted && State != States.Completed && State != States.Failed)
            //{
            //    if (!IsInZone && State != States.TakingWaypoint)
            //    {
            //        Core.Logger.Log("[Bounty] Looks like we left the bounty zone, returning");
            //        State = States.TakingWaypoint;
            //    }
            //}
        }

        public virtual void Reset()
        {
            State = States.NotStarted;
            _bountyData = null;
        }
    }
}