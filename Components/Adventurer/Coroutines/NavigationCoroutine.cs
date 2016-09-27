using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines
{

    public sealed class NavigationCoroutine
    {
        private static NavigationCoroutine _navigationCoroutine;
        private static Vector3 _moveToDestination = Vector3.Zero;
        private static int _moveToDistance;
        private int _unstuckAttemps;
        private Vector3 _destination;
        private readonly int _distance;
        public static MoveResult LastMoveResult { get; private set; }

        public static CoroutineResult LastResult;

        public static Vector3 LastDestination;

        public static async Task<bool> MoveTo(Vector3 destination, int distance, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
        {
            //destination.Z = AdvDia.MainGridProvider.GetHeight(destination.ToVector2());

            if (_navigationCoroutine == null || _moveToDestination != destination || _moveToDistance != distance)
            {
                _navigationCoroutine = new NavigationCoroutine(destination, distance);

                Util.Logger.DebugSetting($"Created Navigation Task for {destination}, within a range of (specified={distance}, actual={_navigationCoroutine._distance}). ({callerPath.Split('\\').LastOrDefault()} > {caller} )");

                _moveToDestination = destination;
                _moveToDistance = distance;
            }

            LastDestination = _moveToDestination;

            if (await _navigationCoroutine.GetCoroutine())
            {
                LastResult = _navigationCoroutine.State == States.Completed ? CoroutineResult.Success : CoroutineResult.Failure;

                if (_navigationCoroutine.State == States.Failed)
                {
                    Util.Logger.DebugSetting($"_navigationCoroutine.State=Failed for {destination}, within a range of (specified={distance}, actual={_navigationCoroutine._distance}). ({callerPath.Split('\\').LastOrDefault()} > {caller} )");
                    return true;
                } 

                _navigationCoroutine = null;
                return true;
            }
            return false;
        }

        private enum States
        {
            NotStarted,
            Moving,
            MovingToDeathGate,
            InteractingWithDeathGate,
            Completed,
            Failed
        }

        private States _state;
        private States State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;

                Util.Logger.DebugSetting($"Navigation State Changed from {_state} to {value}, Destination={_destination} Dist3D={AdvDia.MyPosition.Distance(_destination)} Dist2D={AdvDia.MyPosition.Distance2D(_destination)}");

                switch (value)
                {
                    case States.NotStarted:
                        break;
                    case States.Moving:
                    case States.MovingToDeathGate:
                        break;
                    case States.InteractingWithDeathGate:
                    case States.Completed:
                    case States.Failed:
                        Util.Logger.Debug("[Navigation] " + value);
                        break;
                }
                if (value != States.NotStarted)
                {

                }
                _state = value;
            }
        }

        private NavigationCoroutine(Vector3 destination, int distance)
        {
            _destination = destination;
            _distance = distance;
            if (_distance < 5)
            {
                _distance = 5;
            }
        }        

        public async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();
                case States.Moving:
                    return await Moving();
                case States.MovingToDeathGate:
                    return await MovingToDeathGate();
                case States.InteractingWithDeathGate:
                    return await InteractingWithDeathGate();
                case States.Completed:
                    return Completed();
                case States.Failed:
                    Util.Logger.DebugSetting($"CanFullyClientPath={await AdvDia.DefaultNavigationProvider.CanFullyClientPathTo(_destination)}");
                    return Failed();
            }
            return false;
        }

        private Mover _mover;
        private long _lastRaywalkCheck;
        private readonly WaitTimer _pathGenetionTimer = new WaitTimer(TimeSpan.FromSeconds(1));

        private async Task<bool> NotStarted()
        {
            var zDiff = Math.Abs((float)(_destination.Z - AdvDia.MyPosition.Z));
            var distanceToDestination = AdvDia.MyPosition.Distance(_destination);

            if (PluginEvents.CurrentProfileType == ProfileType.Rift &&
                distanceToDestination < 50f && zDiff < 3 &&
                NavigationGrid.Instance.CanRayWalk(AdvDia.MyPosition, _destination))
            {
                _mover = Mover.StraightLine;
                _lastRaywalkCheck = PluginTime.CurrentMillisecond;
                Navigator.PlayerMover.MoveTowards(_destination);
            }
            else
            {
                _mover = Mover.Navigator;
            }

            if (DeathGates.IsInDeathGateWorld && DeathGates.IsInOutsideRegion)
            {
                var gatePosition = DeathGates.GetBestGatePosition(_destination);
                if (gatePosition != Vector3.Zero)
                {
                    Logger.Warn($"Moving to use Death Gate {gatePosition} Dist: {gatePosition.Distance(AdvDia.MyPosition)}");
                    _deathGatePosition = gatePosition;
                    State = States.MovingToDeathGate;
                    _pathGenetionTimer.Reset();
                    return false;
                }
            }

            Logger.Debug("[Navigation] {0} {1} (Distance: {2})", (_mover == Mover.StraightLine ? "Moving towards" : "Moving to"), _destination, distanceToDestination);
            State = States.Moving;
            _pathGenetionTimer.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            // Account for portals directly below current terrain.
            var zDiff = Math.Abs((float)(_destination.Z - AdvDia.MyPosition.Z));
            var distanceToDestination = AdvDia.MyPosition.Distance(_destination);

            if (_timeout == DateTime.MaxValue)
                _timeout = DateTime.UtcNow + TimeSpan.FromSeconds(240);

            if (_mover == Mover.StraightLine && (!NavigationGrid.Instance.CanRayWalk(AdvDia.MyPosition, _destination) || !await AdvDia.DefaultNavigationProvider.CanFullyClientPathTo(_destination)))
            {
                Logger.DebugSetting("Unable to straight line path, switching to navigator pathing");
                _mover = Mover.Navigator;
            }

            if (_destination != Vector3.Zero)
            {
                if (_distance != 0 && distanceToDestination <= _distance && zDiff < 4)
                {
                    Navigator.PlayerMover.MoveStop();
                    LastMoveResult = MoveResult.ReachedDestination;
                }
                else
                {

                    if (_mover == Mover.StraightLine && PluginTime.ReadyToUse(_lastRaywalkCheck, 200))
                    {
                        if (!NavigationGrid.Instance.CanRayWalk(AdvDia.MyPosition, _destination))
                        {
                            _mover = Mover.Navigator;
                        }
                        _lastRaywalkCheck = PluginTime.CurrentMillisecond;
                    }
                    switch (_mover)
                    {
                        case Mover.StraightLine:
                            Navigator.PlayerMover.MoveTowards(_destination);
                            LastMoveResult = MoveResult.Moved;
                            Logger.DebugSetting($"MoveTowards Destination={_destination} Dist3D={AdvDia.MyPosition.Distance(_destination)} Dist2D={AdvDia.MyPosition.Distance2D(_destination)}");
                            return false;
                        case Mover.Navigator:

                            if (AdvDia.Navigator != null)
                            {
                                LastMoveResult = await AdvDia.Navigator.MoveTo(_destination);
                            }
                            else
                            {
                                LastMoveResult = await Navigator.MoveTo(_destination);
                            }
                            Logger.DebugSetting($"Navigator MoveResult = {LastMoveResult}, Destination={_destination} Dist3D={AdvDia.MyPosition.Distance(_destination)} Dist2D={AdvDia.MyPosition.Distance2D(_destination)}");
                            break;
                    }


                }
                switch (LastMoveResult)
                {
                    case MoveResult.ReachedDestination:
                        if (_distance != 0 && distanceToDestination <= _distance || distanceToDestination <= 5f)
                        {
                            Logger.Debug("[Navigation] Completed (Distance to destination: {0})", distanceToDestination);
                            State = States.Completed;
                        }
                        else
                        {
                            //_deathGate = ActorFinder.FindNearestDeathGate(_deathGateIgnoreList);
                            //if (_deathGate != null)
                            //{
                            //    Logger.Warn($"Moving to use Death Gate {_deathGate.Name} Dist: {_deathGate.Distance}");
                            //    State = States.MovingToDeathGate;
                            //} 
                            var gatePosition = DeathGates.GetBestGatePosition(_destination);
                            if (gatePosition != Vector3.Zero)
                            {
                                Logger.Warn($"Moving to use Death Gate {gatePosition} Dist: {gatePosition.Distance(AdvDia.MyPosition)}");
                                _deathGatePosition = gatePosition;
                                State = States.MovingToDeathGate;
                            }
                            else
                            {
                                Logger.Debug($"Navigator reports DestinationReached but we're not at destination, failing. Mover={_mover}");
                                State = States.Failed;
                                LastMoveResult = MoveResult.Failed;
                            }
                        }
                        return false;
                    case MoveResult.Failed:
                        break;
                    case MoveResult.PathGenerationFailed:
                        Logger.Debug("[Navigation] Path generation failed.");
                        if (distanceToDestination < 100 && NavigationGrid.Instance.CanRayWalk(AdvDia.MyPosition, _destination))
                        {
                            _mover = Mover.StraightLine;
                            return false;
                        }
                        State = States.Failed;
                        return false;
                    case MoveResult.UnstuckAttempt:

                        await Navigator.StuckHandler.DoUnstick();

                        if (_unstuckAttemps > 1)
                        {
                            State = States.Failed;
                            return false;
                        }
                        _unstuckAttemps++;
                        Logger.Debug("[Navigation] Unstuck attempt #{0}", _unstuckAttemps);
                        break;
                    case MoveResult.PathGenerated:
                    case MoveResult.Moved:
                        break;
                    case MoveResult.PathGenerating:
                        if (_pathGenetionTimer.IsFinished)
                        {
                            Logger.Info("Patiently waiting for the Navigation Server");
                            _pathGenetionTimer.Reset();
                        }
                        break;
                }
                return false;
            }
            State = States.Completed;
            return false;
        }

        private DiaGizmo _deathGate;
        private List<int> _deathGateIgnoreList = new List<int>();

        private InteractionCoroutine _interactionCoroutine;
        private DateTime _timeout = DateTime.MaxValue;
        private Vector3 _deathGatePosition;

        private async Task<bool> MovingToDeathGate()
        {
            if (_deathGatePosition == Vector3.Zero)
            {
                State = States.Moving;
                return false;
            }
   
            _deathGate = ActorFinder.FindNearestDeathGate(_deathGateIgnoreList);

            if (_deathGate == null)
            {
                LastMoveResult = await CommonCoroutines.MoveTo(_deathGatePosition);
            }
            else if (AdvDia.MyPosition.Distance(_deathGate.Position) <= 7f && _deathGate.Position.Distance(_deathGatePosition) < 15f)
            {
                Navigator.PlayerMover.MoveTowards(_deathGate.Position);
                await Coroutine.Sleep(500);
                Logger.Warn($"Arrived at Gate, Distance={_deathGate.Distance}");
                LastMoveResult = MoveResult.ReachedDestination;
            }
            else
            {
                LastMoveResult = await CommonCoroutines.MoveTo(_deathGate.Position);
            }

            switch (LastMoveResult)
            {
                case MoveResult.ReachedDestination:

                    if (_deathGate == null)
                    {
                        LastMoveResult = MoveResult.Failed;
                        State = States.Failed;
                        return false;
                    }

                    Navigator.PlayerMover.MoveTowards(_deathGate.Position);
                    await Coroutine.Sleep(500);

                    _interactionCoroutine = new InteractionCoroutine(_deathGate.ActorSnoId, TimeSpan.FromMilliseconds(2000), new TimeSpan(0, 0, 3));
                    State = States.InteractingWithDeathGate;       
                    break;

                    //var distance = AdvDia.MyPosition.Distance(_deathGate.Position);
                    //if (_deathGate != null && distance <= 12f)
                    //{
                    //    Navigator.PlayerMover.MoveTowards(_deathGate.Position);
                    //    _interactionCoroutine = new InteractionCoroutine(_deathGate.ActorSnoId, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 3));
                    //    State = States.InteractingWithDeathGate;
                    //}
                    //else
                    //{
                    //    _deathGateIgnoreList.Add(_deathGate.ACDId);
                    //    State = States.Moving;
                    //}
                    //break;

                case MoveResult.Failed:
                case MoveResult.PathGenerationFailed:
                    State = States.Failed;
                    break;
                case MoveResult.PathGenerated:
                    break;
                case MoveResult.UnstuckAttempt:
                    if (_unstuckAttemps > 1)
                    {
                        State = States.Failed;
                        return false;
                    }
                    _unstuckAttemps++;
                    Logger.Debug("[Navigation] Unstuck attempt #{0}", _unstuckAttemps);
                    break;
                case MoveResult.Moved:
                case MoveResult.PathGenerating:
                    break;
            }

            return false;
        }

        private async Task<bool> InteractingWithDeathGate()
        {
            if (await _interactionCoroutine.GetCoroutine())
            {
                if (_interactionCoroutine.State == InteractionCoroutine.States.TimedOut)
                {
                    Logger.Debug("[Bounty] Near death gate, but interaction timed out.");

                    var gate = ActorFinder.FindNearestDeathGate(new IndexedList<int>());
                    if (gate.Distance > 15f && gate.InLineOfSight)
                    {
                        Logger.Debug("[Bounty] Starting fallback death gate interaction.");
                        await CommonCoroutines.MoveTo(gate.Position, "Death Gate");
                        await CommonCoroutines.MoveAndStop(gate.Position, 2f, "Death Gate");
                        await Coroutine.Sleep(500);
                        var startPosition = ZetaDia.Me.Position;
                        gate.Interact();
                        await Coroutine.Sleep(2000);
                        if (ZetaDia.Me.Position.Distance(startPosition) > 10f)
                        {
                            Logger.Debug("[Bounty] Successfully used death gate");
                            State = States.Completed;                            
                            return false;
                        }
                    }

                    State = States.Failed;
                    _interactionCoroutine = null;
                }
                else
                {
                    _deathGate = null;
                    _deathGatePosition = Vector3.Zero;
                    _deathGateIgnoreList = new List<int>();
                    State = States.Moving;
                    _interactionCoroutine = null;
                }
            }

            return false;
        }

        private bool Completed()
        {
            return true;
        }

        public int FailCount { get; set; }

        private bool Failed(bool reset = false)
        {
            Logger.Debug($"[Navigation] Navigation Error (MoveResult: {LastMoveResult}, Distance: {AdvDia.MyPosition.Distance(_destination)}) Failures={FailCount}.");


            if (LastDestination == _destination)
            {
                FailCount++;
                if (FailCount > 5)
                {
                    var distance = AdvDia.MyPosition.Distance2D(_destination);
                    var canWalkTo = NavigationGrid.Instance.CanRayWalk(AdvDia.MyPosition, _destination);
                    var canStandAt = AdvDia.MainGridProvider.CanStandAt(_destination);
                    var portalNearby = ZetaDia.Actors.GetActorsOfType<GizmoPortal>().Any(g => g.Position.Distance(_destination) < 30f);
                    if (distance < 25f && !portalNearby && (!canStandAt && !canWalkTo))
                    {
                        Logger.Debug($"Destination cant be reached. A");
                        //Reset();
                    }
                }
                else if (FailCount > 15)
                {
                    Logger.Debug($"Destination cant be reached. B");
                    //Reset();
                }
            }
            return true;
        }

        private static void Reset()
        {
            Navigator.Clear();
            ScenesStorage.Reset();            
        }

        private enum Mover
        {
            Navigator,
            StraightLine
        }
    }


}
