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
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class ClearLevelAreaCoroutine : IBountySubroutine
    {
        private readonly SNOQuest _questId;
        private readonly SNOActor _objectiveId;
        private bool _isDone;
        private States _state;
        private Vector3 _hostileLocation = Vector3.Zero;
        private Vector3 _objectiveLocation = Vector3.Zero;
        private BountyData _bountyData;
        private readonly bool _stopWhenExplored;

        public enum States
        {
            NotStarted,
            Searching,
            KillingHostile,
            MovingToObjective,
            Completed,
            Failed
        }

        //public int ObjectiveId
        //{
        //    get { return _objectiveId; }
        //    get
        //    {
        //        if (_objectiveId == value) return;
        //        if (value > 0)
        //        {
        //            Core.Logger.Log("[ClearLevelArea] Objective ID: " + value);
        //            StatusText = "[ClearLevelArea]] Objective ID: " + value;
        //        }
        //        _objectiveId = value;
        //    }
        //}

        public States State
        {
            get => _state;
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

        public bool IsDone => _isDone;

        public BountyData BountyData => _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId));

        public ClearLevelAreaCoroutine(SNOQuest questId, bool stopWhenExplored = false, SNOActor objectiveId = SNOActor.Invalid)
        {
            _questId = questId;
            _stopWhenExplored = stopWhenExplored;
            _objectiveId = objectiveId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            SafeZerg.Instance.DisableZerg();
            TrinityCombat.SetKillMode(1000);

            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.Searching:
                    return await Searching();

                case States.MovingToObjective:
                    return await MovingToObjective();

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
            if (_objectiveLocation == Vector3.Zero)
            {
                _objectiveLocation = FindNearestObjectiveLocation();
                if (_objectiveLocation != Vector3.Zero)
                {
                    Core.Logger.Log("[ClearLevelArea] Completed");
                    State = States.MovingToObjective;
                    return false;
                }
            }

            var ids = BountyData?.LevelAreaIds ?? new HashSet<SNOLevelArea> { ZetaDia.CurrentLevelAreaSnoId };

            if (!await ExplorationCoroutine.Explore(ids, useIgnoreRegions: false))
                return false;

            if (_stopWhenExplored && ProfileConditions.PercentNodesVisited(90))
            {
                State = States.Completed;
                return false;
            }

            ExplorationGrid.Instance.WalkableNodes.ForEach(n => { n.IsVisited = false; });
            return false;
        }

        private async Task<bool> MovingToObjective()
        {
            if (_objectiveLocation == Vector3.Zero)
            {
                State = States.Searching;
                return false;
            }
            if (!await NavigationCoroutine.MoveTo(_objectiveLocation, 7)) return false;
            State = States.Completed;
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

        public Vector3 FindNearestHostileUnitLocation()
        {
            var actor = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(o => o.IsValid && o.CommonData != null && o.CommonData.IsValid && o.IsAlive && o.CommonData.MinimapVisibilityFlags != 0).OrderBy(o => o.Distance).FirstOrDefault();

            if (actor == null) return Vector3.Zero;
            Core.Logger.Debug("[ClearLevelArea] Found a hostile unit {0} at {1} distance", ((SNOActor)actor.ActorSnoId).ToString(), actor.Distance);
            return actor.Position;
        }

        public Vector3 FindNearestObjectiveLocation()
        {
            var actor = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).Where(o => o.ActorSnoId == _objectiveId && o.IsValid && o.CommonData != null && o.CommonData.IsValid && o.CommonData.MinimapVisibilityFlags != 0).OrderBy(o => o.Distance).FirstOrDefault();

            if (actor == null) return Vector3.Zero;
            Core.Logger.Debug("[ClearLevelArea] Found a hostile unit {0} at {1} distance", ((SNOActor)actor.ActorSnoId).ToString(), actor.Distance);
            return actor.Position;
        }
    }
}
