using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot.Navigation;

namespace Trinity.Components.Adventurer.Coroutines
{
    public sealed class ExplorationCoroutine
    {
        private static ExplorationCoroutine _explorationCoroutine;
        private static HashSet<int> _exploreLevelAreaIds;

        public static async Task<bool> Explore(HashSet<int> levelAreaIds, List<string> ignoreScenes = null)
        {
            if (_explorationCoroutine == null || (_exploreLevelAreaIds != null && levelAreaIds != null && !_exploreLevelAreaIds.SetEquals(levelAreaIds)))
            {
                _explorationCoroutine = new ExplorationCoroutine(levelAreaIds);
                _exploreLevelAreaIds = levelAreaIds;
            }
            if (await _explorationCoroutine.GetCoroutine())
            {
                _explorationCoroutine = null;
                return true;
            }
            return false;
        }


        private readonly HashSet<int> _levelAreaIds;

        private enum States
        {
            NotStarted,
            Exploring,
            Completed
        }

        private States _state;
        private States State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Logger.Debug("[Exploration] " + value);
                }
                _state = value;
            }
        }

        private ExplorationCoroutine(HashSet<int> levelAreaIds, List<string> ignoreScenes = null)
        {
            _levelAreaIds = levelAreaIds;// ?? new HashSet<int> { AdvDia.CurrentLevelAreaSnoId };
            //ExplorationHelpers.SyncronizeNodesWithMinimap();
        }


        private async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();
                case States.Exploring:
                    return await Exploring();
                case States.Completed:
                    return Completed();
            }
            return false;
        }

        private ExplorationNode _currentDestination;
        private int _failedNavigationAttempts;

        private bool NotStarted()
        {
            State = States.Exploring;
            return false;
        }

        //private readonly WaitTimer _newNodePickTimer = new WaitTimer(TimeSpan.FromSeconds(60));

        private async Task<bool> Exploring()
        {
            if (_currentDestination == null || _currentDestination.IsVisited)// || _newNodePickTimer.IsFinished)
            {
                //_newNodePickTimer.Stop();
                //_currentDestination = ExplorationHelpers.NearestWeightedUnvisitedNodeLocation(_levelAreaIds);
                if (_currentDestination != null)
                {
                    _currentDestination.IsCurrentDestination = false;
                }
                var destination = ExplorationHelpers.NearestWeightedUnvisitedNode(_levelAreaIds);
                if (destination == null)
                {
                    State = States.Completed;
                    return false;
                }

                if (_currentDestination != destination)
                {
                    Logger.Debug($"[Exploration] Destination Changed from {_currentDestination?.NavigableCenter} to {destination.NavigableCenter}");                    
                    _currentDestination = destination;
                }
                if (_currentDestination != null)
                {  
                    Logger.DebugSetting($"[Exploration] Current Destination {_currentDestination?.NavigableCenter}, CanRayWalk={CanRayWalkDestination} MyPosition={AdvDia.MyPosition}");                   
                    _currentDestination.IsCurrentDestination = true;
                }
                //_newNodePickTimer.Reset();
            }

            if (_currentDestination != null)
            {
                if (await NavigationCoroutine.MoveTo(_currentDestination.NavigableCenter, 15))
                {
                    if (NavigationCoroutine.LastResult == CoroutineResult.Failure && (NavigationCoroutine.LastMoveResult == MoveResult.Failed || NavigationCoroutine.LastMoveResult == MoveResult.PathGenerationFailed))
                    {
                        _currentDestination.FailedNavigationAttempts++;

                        var canClientPathTo = await AdvDia.DefaultNavigationProvider.CanFullyClientPathTo(_currentDestination.NavigableCenter);
                        if (_currentDestination.FailedNavigationAttempts >= 15 && !canClientPathTo)
                        {
                            Logger.DebugSetting($"[Exploration] Unable to client path to {_currentDestination.NavigableCenter} and failed {_currentDestination.FailedNavigationAttempts} times; Ignoring Node.");
                            _currentDestination.IsVisited = true;
                            _currentDestination.IsIgnored = true;
                            _currentDestination.IsCurrentDestination = false;
                            _currentDestination = null;
                            _failedNavigationAttempts++;
                        }
                        else if (!CanRayWalkDestination && _currentDestination.Distance < 25f && _currentDestination.FailedNavigationAttempts >= 5 || _currentDestination.FailedNavigationAttempts >= 25)
                        {
                            Logger.DebugSetting($"[Exploration] Failed to Navigate to {_currentDestination.NavigableCenter} {_currentDestination.FailedNavigationAttempts} times; Ignoring Node.");
                            _currentDestination.IsVisited = true;
                            _currentDestination.IsIgnored = true;                            
                            _currentDestination.IsCurrentDestination = false;                            
                            _currentDestination = null;
                            _failedNavigationAttempts++;
                        }                        
                    }
                    else
                    {
                        Logger.Debug($"[Exploration] Destination Reached!");
                        _currentDestination.FailedNavigationAttempts = 0;
                        _currentDestination.IsVisited = true;
                        _currentDestination.IsCurrentDestination = false;
                        _currentDestination = null;
                    }

                    if (_failedNavigationAttempts > 25)
                    {
                        Logger.Debug($"[Exploration] Expploration Resetting");
                        ScenesStorage.Reset();
                        Navigator.Clear();
                        _failedNavigationAttempts = 0;
                    }

                }
                return false;
            }

            ScenesStorage.Reset();
            Navigator.Clear();

            State = States.Completed;
            return false;
        }

        public bool CanRayWalkDestination => NavigationGrid.Instance.CanRayWalk(AdvDia.MyPosition, _currentDestination.NavigableCenter);


        private bool Completed()
        {
            return true;
        }

    }


}
