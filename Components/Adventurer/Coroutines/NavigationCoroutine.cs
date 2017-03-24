using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;


namespace Trinity.Components.Adventurer.Coroutines
{
    public sealed class NavigationCoroutine
    {
        private static NavigationCoroutine _navigationCoroutine;
        private static Vector3 _moveToDestination = Vector3.Zero;
        private static int _moveToDistance;
        private int _unstuckAttemps;
        public Vector3 Destination;
        private readonly int _distance;
        public static MoveResult LastMoveResult { get; private set; }

        public static CoroutineResult LastResult;

        public static Vector3 LastDestination;

        public static void Reset()
        {
            _navigationCoroutine = null;
            _moveToDestination = Vector3.Zero;
        }

        public static async Task<bool> MoveTo(Vector3 destination, int distance, bool straightLinePath = false, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
        {
            //destination.Z = AdvDia.MainGridProvider.GetHeight(destination.ToVector2());

            if (_navigationCoroutine == null || _moveToDestination != destination || _moveToDistance != distance)
            {
                _navigationCoroutine = new NavigationCoroutine(destination, distance, straightLinePath);

                Core.Logger.Debug($"Created Navigation Task for {destination}, within a range of (specified={distance}, actual={_navigationCoroutine._distance}). ({callerPath.Split('\\').LastOrDefault()} > {caller} )");

                _moveToDestination = destination;
                _moveToDistance = distance;
            }

            LastDestination = _moveToDestination;

            if (await _navigationCoroutine.GetCoroutine())
            {
                LastResult = _navigationCoroutine.State == States.Completed ? CoroutineResult.Success : CoroutineResult.Failure;

                if (_navigationCoroutine.State == States.Failed)
                {
                    Core.Logger.Debug($"_navigationCoroutine.State=Failed for {destination}, within a range of (specified={distance}, actual={_navigationCoroutine._distance}). ({callerPath.Split('\\').LastOrDefault()} > {caller} )");
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

                Core.Logger.Debug($"Navigation State Changed from {_state} to {value}, Destination={Destination} Dist3D={AdvDia.MyPosition.Distance(Destination)} Dist2D={AdvDia.MyPosition.Distance2D(Destination)}");

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
                        Core.Logger.Debug("[Navigation] " + value);
                        break;
                }
                if (value != States.NotStarted)
                {
                }
                _state = value;
            }
        }

        public NavigationCoroutine(Vector3 destination, int distance, bool straightLinePath = false)
        {
            Destination = destination;
            _distance = distance;
            _mover = straightLinePath ? Mover.StraightLine : Mover.Navigator;

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
                    Core.Logger.Debug($"CanFullyClientPath={await AdvDia.Navigator.CanFullyClientPathTo(Destination)}");
                    return Failed();
            }
            return false;
        }

        private Mover _mover;
        private long _lastRaywalkCheck;
        private readonly WaitTimer _pathGenetionTimer = new WaitTimer(TimeSpan.FromSeconds(1));

        private async Task<bool> NotStarted()
        {
            var zDiff = Math.Abs((float)(Destination.Z - AdvDia.MyPosition.Z));
            var distanceToDestination = AdvDia.MyPosition.Distance(Destination);

            if (PluginEvents.CurrentProfileType == ProfileType.Rift &&
                distanceToDestination < 50f && zDiff < 3 &&
                Core.Grids.CanRayWalk(AdvDia.MyPosition, Destination))
            {
                _mover = Mover.StraightLine;
                _lastRaywalkCheck = PluginTime.CurrentMillisecond;
                Navigator.PlayerMover.MoveTowards(Destination);
            }
            else
            {
                _mover = Mover.Navigator;
            }

            //// Intercept destinations that require a gate to be used to reach them, and redirect to gate position.
            //if (DeathGates.IsInDeathGateWorld)
            //{
            //    var gatePosition = DeathGates.GetBestGatePosition(_destination);
            //    if (DeathGates.IsInOutsideRegion && !IsDeathGateIgnored(gatePosition, _deathGateIgnoreList))
            //    {
            //        Core.Logger.Debug($"Moving to use Death Gate (Currently in Outside Region) {gatePosition} Dist: {gatePosition.Distance(AdvDia.MyPosition)}");
            //        _deathGatePosition = gatePosition;
            //        State = States.MovingToDeathGate;
            //        _pathGenetionTimer.Reset();
            //        return false;
            //    }

            //    var destinationIsGate = _destination.Distance(gatePosition) < 5f;
            //    if (destinationIsGate)
            //    {
            //        Core.Logger.Debug($"Current Destination is Death Gate. {gatePosition} Dist: {gatePosition.Distance(AdvDia.MyPosition)}");
            //        _deathGatePosition = gatePosition;
            //        State = States.MovingToDeathGate;
            //        _pathGenetionTimer.Reset();
            //        return false;
            //    }

            //}

            Core.Logger.Debug("{0} {1} (Distance: {2})", (_mover == Mover.StraightLine ? "Moving towards" : "Moving to"), Destination, distanceToDestination);
            State = States.Moving;
            _pathGenetionTimer.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            // Account for portals directly below current terrain.
            var zDiff = Math.Abs((float)(Destination.Z - AdvDia.MyPosition.Z));
            var distanceToDestination = AdvDia.MyPosition.Distance(Destination);

            if (_timeout == DateTime.MaxValue)
                _timeout = DateTime.UtcNow + TimeSpan.FromSeconds(240);

            if (_mover == Mover.StraightLine && (!Core.Grids.CanRayWalk(AdvDia.MyPosition, Destination) || !await AdvDia.Navigator.CanFullyClientPathTo(Destination)))
            {
                Core.Logger.Debug("Unable to straight line path, switching to navigator pathing");
                _mover = Mover.Navigator;
            }

            if (Destination != Vector3.Zero)
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
                        if (!Core.Grids.CanRayWalk(AdvDia.MyPosition, Destination))
                        {
                            _mover = Mover.Navigator;
                        }
                        _lastRaywalkCheck = PluginTime.CurrentMillisecond;
                    }
                    switch (_mover)
                    {
                        case Mover.StraightLine:
                            Navigator.PlayerMover.MoveTowards(Destination);
                            LastMoveResult = MoveResult.Moved;
                            Core.Logger.Debug($"MoveTowards Destination={Destination} Dist3D={AdvDia.MyPosition.Distance(Destination)} Dist2D={AdvDia.MyPosition.Distance2D(Destination)}");
                            return false;

                        case Mover.Navigator:

                            if (AdvDia.Navigator != null)
                            {
                                LastMoveResult = await AdvDia.Navigator.MoveTo(Destination);
                            }
                            else
                            {
                                LastMoveResult = await Navigator.MoveTo(Destination);
                            }
                            Core.Logger.Debug($"Navigator MoveResult = {LastMoveResult}, Destination={Destination} Dist3D={AdvDia.MyPosition.Distance(Destination)} Dist2D={AdvDia.MyPosition.Distance2D(Destination)}");

                            break;
                    }
                }
                switch (LastMoveResult)
                {
                    case MoveResult.ReachedDestination:

                        if (_distance != 0 && distanceToDestination <= _distance || distanceToDestination <= 5f)
                        {
                            Core.Logger.Debug("Completed (Distance to destination: {0})", distanceToDestination);
                            State = States.Completed;
                        }
                        else
                        {
                            // DB Navigator will report ReachedDestination when failing to navigate to positions that require a death gate to reach. Redirect to gate position.
                            if (Core.Rift.IsInRift && ActorFinder.FindNearestDeathGate() != null)
                            {
                                Core.Logger.Debug($"Starting Death Gate Sequence.");
                                State = States.MovingToDeathGate;
                            }
                            else
                            {
                                Core.Logger.Debug($"Navigator reports DestinationReached but we're not at destination, failing. Mover={_mover}");
                                State = States.Failed;
                                LastMoveResult = MoveResult.Failed;
                            }
                        }
                        return false;

                    case MoveResult.Failed:
                        break;

                    case MoveResult.PathGenerationFailed:
                        Core.Logger.Debug("[Navigation] Path generation failed.");
                        if (distanceToDestination < 100 && Core.Grids.CanRayWalk(AdvDia.MyPosition, Destination))
                        {
                            _mover = Mover.StraightLine;
                            return false;
                        }
                        State = States.Failed;
                        return false;

                    case MoveResult.UnstuckAttempt:
                        await Navigator.StuckHandler.DoUnstick();
                        if (_unstuckAttemps > 3)
                        {
                            State = States.Failed;
                            return false;
                        }
                        _unstuckAttemps++;
                        Core.Logger.Debug("[Navigation] Unstuck attempt #{0}", _unstuckAttemps);
                        break;

                    case MoveResult.PathGenerated:
                    case MoveResult.Moved:
                        break;

                    case MoveResult.PathGenerating:
                        if (_pathGenetionTimer.IsFinished)
                        {
                            Core.Logger.Log("Patiently waiting for the Navigation Server");
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

        private static Dictionary<Vector3, DateTime> _deathGateIgnoreList = new Dictionary<Vector3, DateTime>();

        private InteractionCoroutine _interactionCoroutine;
        private DateTime _timeout = DateTime.MaxValue;
        private Vector3 _deathGatePosition;

        public static bool IsDeathGateIgnored(Vector3 position, IReadOnlyDictionary<Vector3, DateTime> ignoreList, int seconds = 30)
        {
            return position == Vector3.Zero || ignoreList.ContainsKey(position) && DateTime.UtcNow.Subtract(ignoreList[position]).TotalSeconds < seconds;
        }

        private MoveThroughDeathGates _deathGateCoroutine;

        private async Task<bool> MovingToDeathGate()
        {
            if (_deathGateCoroutine == null)
            {
                _deathGateCoroutine = new MoveThroughDeathGates(1, AdvDia.CurrentWorldId, 1);
            }

            if (!await _deathGateCoroutine.GetCoroutine())
            {
                return false;
            }

            _deathGateCoroutine = null;
            State = States.Moving;
            return false;

            //if (_deathGatePosition == Vector3.Zero)
            //{
            //    State = States.Moving;
            //    return false;
            //}

            //var targetGateScene = DeathGates.GetClosestSceneWithUnvisitedGate();
            //if (targetGateScene == null)
            //{
            //    Core.Logger.Debug("A TargetGateScene wasn't found, using gate in current scene");
            //    TargetGateScene = CurrentGateScene;
            //}

            //TargetGatePosition = DeathGates.SelectGate(CurrentGateScene, TargetGateScene);

            //_deathGate = DeathGates.NearestGateToPosition()// ActorFinder.FindNearestDeathGateToPosition(_deathGatePosition, _deathGateIgnoreList);

            //if (_deathGate == null)
            //{
            //    if (_deathGatePosition.Distance(AdvDia.MyPosition) < 20f)
            //    {
            //        Core.Logger.Debug($"No Gate found near position {_deathGatePosition}");
            //        LastMoveResult = MoveResult.Failed;
            //        State = States.Failed;
            //        return false;
            //    }

            //    Core.Logger.Debug($"Moving to Gate Position, Distance={_deathGatePosition.Distance(AdvDia.MyPosition)}");
            //    LastMoveResult = await CommonCoroutines.MoveTo(_deathGatePosition);
            //}
            //else if (AdvDia.MyPosition.Distance(_deathGate.Position) <= 5f && _deathGate.Position.Distance(_deathGatePosition) < 10f)
            //{
            //    Navigator.PlayerMover.MoveTowards(_deathGate.Position);
            //    await Coroutine.Sleep(500);
            //    Core.Logger.Debug($"Arrived at Gate, Distance={_deathGate.Distance}");
            //    LastMoveResult = MoveResult.ReachedDestination;
            //}
            //else
            //{
            //    Core.Logger.Debug($"Moving to Gate, {_deathGate.Name} Distance={_deathGate.Distance}");
            //    LastMoveResult = await CommonCoroutines.MoveTo(_deathGate.Position);
            //}

            //switch (LastMoveResult)
            //{
            //    case MoveResult.ReachedDestination:

            //        if (_deathGate == null)
            //        {
            //            LastMoveResult = MoveResult.Failed;
            //            State = States.Failed;
            //            return false;
            //        }

            //        Navigator.PlayerMover.MoveTowards(_deathGate.Position);
            //        await Coroutine.Sleep(500);

            //        _interactionCoroutine = new InteractionCoroutine(_deathGate.ActorSnoId, TimeSpan.FromMilliseconds(8000), TimeSpan.FromMilliseconds(500));
            //        State = States.InteractingWithDeathGate;
            //        break;
            //    case MoveResult.Failed:
            //    case MoveResult.PathGenerationFailed:
            //        State = States.Failed;
            //        break;
            //    case MoveResult.PathGenerated:
            //        break;
            //    case MoveResult.UnstuckAttempt:

            //        // DB navigation has issues with death gate scene x1_fortress_island_NE_01

            //        if (_unstuckAttemps%2 == 0)
            //        {
            //            Navigator.PlayerMover.MoveTowards(LastDestination);
            //            await Coroutine.Sleep(2000);
            //        }
            //        else
            //        {
            //            await Navigator.StuckHandler.DoUnstick();
            //        }
            //        if (_unstuckAttemps > 4)
            //        {
            //            State = States.Failed;
            //            return false;
            //        }
            //        _unstuckAttemps++;
            //        Core.Logger.Debug("[Navigation] Unstuck attempt #{0}", _unstuckAttemps);
            //        break;
            //    case MoveResult.Moved:
            //    case MoveResult.PathGenerating:
            //        break;
            //}

            //return false;
        }

        private async Task<bool> InteractingWithDeathGate()
        {
            if (!await _interactionCoroutine.GetCoroutine())
                return false;

            if (_interactionCoroutine.State == InteractionCoroutine.States.Completed)
            {
                await SetUsedGatesToIgnored();
                ClearGateDestination();
                State = States.Completed;
                return true;
            }

            ClearGateDestination();
            State = States.Failed;
            return false;
        }

        private void ClearGateDestination()
        {
            _deathGate = null;
            _deathGatePosition = Vector3.Zero;
            _deathGateIgnoreList.RemoveAll(dt => DateTime.UtcNow.Subtract(dt).TotalMinutes > 5);
            _interactionCoroutine = null;
        }

        private async Task<bool> SetUsedGatesToIgnored()
        {
            // 'Reference' positions are hardcoded gate positions by SceneSnoId.
            // These allow gate nativation as soon as a scene is discovered.
            // And may be slightly off from actual gate actor positions.

            if (_deathGate != null)
            {
                Core.Logger.Debug($"Added origin gate to ignore list. (DiaGizmo) {_deathGate.Position}");
                _deathGateIgnoreList[_deathGate.Position] = DateTime.UtcNow;
            }

            if (_deathGate?.Position != _deathGatePosition)
            {
                Core.Logger.Debug($"Added origin gate position to ignore list. (Reference) {_deathGatePosition}");
                _deathGateIgnoreList[_deathGatePosition] = DateTime.UtcNow;
            }

            var destinationGate = ActorFinder.FindNearestDeathGate();
            if (destinationGate != null)
            {
                Core.Logger.Debug($"Added destination gate to ignore list (DiaGizmo) {destinationGate}");
                _deathGateIgnoreList[destinationGate.Position] = DateTime.UtcNow;
            }

            var destinationGateReferencePosition = DeathGates.NearestGateToPosition(AdvDia.MyPosition);
            if (destinationGateReferencePosition != Vector3.Zero)
            {
                Core.Logger.Debug($"Added destination gate to ignore list (Reference) {destinationGateReferencePosition}");
                _deathGateIgnoreList[destinationGateReferencePosition] = DateTime.UtcNow;
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
            Core.Logger.Debug($"[Navigation] Navigation Error (MoveResult: {LastMoveResult}, Distance: {AdvDia.MyPosition.Distance(Destination)}) Failures={FailCount}.");

            if (LastDestination == Destination)
            {
                FailCount++;
                if (FailCount > 5)
                {
                    var distance = AdvDia.MyPosition.Distance2D(Destination);
                    var canWalkTo = Core.Grids.CanRayWalk(AdvDia.MyPosition, Destination);
                    var canStandAt = AdvDia.MainGridProvider.CanStandAt(Destination);
                    var portalNearby = ZetaDia.Actors.GetActorsOfType<GizmoPortal>().Any(g => g.Position.Distance(Destination) < 30f);
                    if (distance < 25f && !portalNearby && (!canStandAt && !canWalkTo))
                    {
                        Core.Logger.Debug($"Destination cant be reached. A");
                        //Reset();
                    }
                }
                else if (FailCount > 15)
                {
                    Core.Logger.Debug($"Destination cant be reached. B");
                    //Reset();
                }
            }
            return true;
        }

        private static void ResetNavigator()
        {
            Navigator.Clear();
            Core.Scenes.Reset();
        }

        private enum Mover
        {
            Navigator,
            StraightLine
        }
    }
}