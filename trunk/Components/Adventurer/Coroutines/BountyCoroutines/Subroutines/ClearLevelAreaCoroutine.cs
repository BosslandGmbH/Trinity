using System;
using System.Collections.Generic;
using Trinity.Framework;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Combat;
using Trinity.Components.QuestTools;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class ClearLevelAreaCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private bool _isDone;
        private States _state;
        private Vector3 _hostileLocation = Vector3.Zero;
        private BountyData _bountyData;

        public enum States
        {
            NotStarted,
            Searching,
            KillingHostile,
            Completed,
            Failed
        }

        public States State
        {
            get { return _state; }
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Log("[ClearLevelArea] " + value);
                    StatusText = "[ClearLevelArea] " + value;
                }
                _state = value;
            }
        }

        public bool IsDone
        {
            get { return _isDone; }
        }

        public BountyData BountyData
        {
            get { return _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId)); }
        }

        public ClearLevelAreaCoroutine(int questId, bool stopWhenExplored = false)
        {
            _questId = questId;
            _stopWhenExplored = stopWhenExplored;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            SafeZerg.Instance.DisableZerg();
            TrinityCombat.SetKillMode(75);

            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.Searching:
                    return await Searching();

                case States.KillingHostile:
                    return await KillingHostile();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }

        private async Task<bool> NotStarted()
        {
            SafeZerg.Instance.DisableZerg();
            State = States.Searching;
            return false;
        }

        private async Task<bool> Searching()
        {
            if (_hostileLocation == Vector3.Zero)
            {
                _hostileLocation = FindNearestHostileUnitLocation();
                if (_hostileLocation != Vector3.Zero)
                {
                    State = States.KillingHostile;
                    return false;
                }
            }

            var ids = BountyData?.LevelAreaIds ?? new HashSet<int> {ZetaDia.CurrentLevelAreaSnoId};

            if (!await ExplorationCoroutine.Explore(ids, useIgnoreRegions:false))
                return false;

            if (_stopWhenExplored && ProfileConditions.PercentNodesVisited(90))
            {
                State = States.Completed;
                return false;
            }

            ExplorationGrid.Instance.WalkableNodes.ForEach(n => { n.IsVisited = false; });
            return false;
        }

        private async Task<bool> KillingHostile()
        {
            if (_hostileLocation == Vector3.Zero)
            {
                State = States.Searching;
                return false;
            }
            if (!await NavigationCoroutine.MoveTo(_hostileLocation, 10)) return false;
            _hostileLocation = FindNearestHostileUnitLocation();
            if (_hostileLocation != Vector3.Zero) return false;
            State = States.Searching;
            return false;
        }

        private async Task<bool> Completed()
        {
            TrinityCombat.ResetCombatMode();
            Core.Logger.Log("[ClearLevelArea] Completed");
            return true;
        }

        private async Task<bool> Failed()
        {
            TrinityCombat.ResetCombatMode();
            Core.Logger.Log("[ClearLevelArea] Failed");
            return true;
        }

        public void Reset()
        {
            _isDone = false;
            _state = States.NotStarted;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        private static readonly WaitTimer HostileSearchTimer = new WaitTimer(TimeSpan.FromMilliseconds(500));
        private bool _stopWhenExplored;

        public Vector3 FindNearestHostileUnitLocation()
        {
            if (!HostileSearchTimer.IsFinished) return _hostileLocation;
            HostileSearchTimer.Stop();
            var actor = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(o => o.IsValid && o.CommonData != null && o.CommonData.IsValid && o.IsAlive && o.CommonData.MinimapVisibilityFlags != 0).OrderBy(o => o.Distance).FirstOrDefault();
            HostileSearchTimer.Reset();

            if (actor == null) return Vector3.Zero;
            Core.Logger.Debug("[ClearLevelArea] Found a hostile unit {0} at {1} distance", ((SNOActor)actor.ActorSnoId).ToString(), actor.Distance);
            return actor.Position;
        }
    }
}