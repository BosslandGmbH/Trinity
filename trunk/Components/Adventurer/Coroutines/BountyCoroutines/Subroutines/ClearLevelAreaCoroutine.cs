using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Components.Adventurer.Util.Logger;

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
                    Util.Logger.Info("[ClearLevelArea] " + value);
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


        public ClearLevelAreaCoroutine(int questId)
        {
            _questId = questId;


        }

        public async Task<bool> GetCoroutine()
        {
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

            if (!await ExplorationCoroutine.Explore(BountyData.LevelAreaIds)) return false;
            ExplorationGrid.Instance.WalkableNodes.ForEach(n=> { n.IsVisited = false; });
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
            throw new NotImplementedException();
        }

        private async Task<bool> Failed()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _isDone = false;
            _state = States.NotStarted;
        }

        public void DisablePulse()
        {
        }

        private static readonly WaitTimer HostileSearchTimer = new WaitTimer(TimeSpan.FromMilliseconds(500));
        public Vector3 FindNearestHostileUnitLocation()
        {
            if (!HostileSearchTimer.IsFinished) return _hostileLocation;
            HostileSearchTimer.Stop();
            var actor = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(o => o.IsValid && o.CommonData != null && o.CommonData.IsValid && o.IsAlive && o.CommonData.MinimapVisibilityFlags != 0).OrderBy(o => o.Distance).FirstOrDefault();
            HostileSearchTimer.Reset();

            if (actor == null) return Vector3.Zero;
            Util.Logger.Debug("[ClearLevelArea] Found a hostile unit {0} at {1} distance", ((SNOActor)actor.ActorSnoId).ToString(), actor.Distance);
            return actor.Position;
        }
    }
}
