using Buddy.Coroutines;
using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines
{
    public sealed class ExplorationCoroutine : ISubroutine
    {
        private static ExplorationCoroutine _explorationCoroutine;
        private static HashSet<SNOLevelArea> _exploreLevelAreaIds;

        public static async Task<bool> Explore(HashSet<SNOLevelArea> levelAreaIds, List<string> ignoreScenes = null, Func<bool> breakCondition = null, bool allowReExplore = true, bool useIgnoreRegions = true)
        {
            if (_explorationCoroutine == null || (_exploreLevelAreaIds != null && levelAreaIds != null && !_exploreLevelAreaIds.SetEquals(levelAreaIds)))
            {
                _explorationCoroutine = new ExplorationCoroutine(levelAreaIds, ignoreScenes, breakCondition, allowReExplore, useIgnoreRegions);
                _exploreLevelAreaIds = levelAreaIds;
            }
            if (await _explorationCoroutine.GetCoroutine())
            {
                _explorationCoroutine = null;
                return true;
            }
            return false;
        }

        private readonly HashSet<SNOLevelArea> _levelAreaIds;

        private enum States
        {
            NotStarted,
            Exploring,
            Completed
        }

        private States _state;

        private States State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[Exploration] " + value);
                    StatusText = "[Exploration] " + value;
                }
                _state = value;
            }
        }

        public ExplorationCoroutine(HashSet<SNOLevelArea> levelAreaIds, List<string> ignoreScenes = null, Func<bool> breakCondition = null, bool allowReExplore = true, bool useIgnoreRegions = true)
        {
            _levelAreaIds = levelAreaIds;
            _ignoreScenes = ignoreScenes;
            _breakCondition = breakCondition;
            _allowReExplore = allowReExplore;
            _useIgnoreRegions = useIgnoreRegions;
            Id = Guid.NewGuid();
            Core.Scenes.ScenesAdded += OnScenesAdded;
        }

        private void OnScenesAdded(List<WorldScene> provider)
        {
            // Clear the current destination when new scenes are added to force it to regenerate the current path.
            Core.Logger.Debug("[Exploration] New scenes loaded, setting current destination to null");
            _currentDestination = null;
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            if (_breakCondition != null && _breakCondition())
            {
                Core.Logger.Debug("BreakCondition Triggered");
                Core.Logger.Debug($"[Exploration] Break condition triggered, so we're done.");
                State = States.Completed;
            }

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
        private readonly Func<bool> _breakCondition;
        private readonly List<string> _ignoreScenes;
        private readonly bool _allowReExplore;
        private DateTime _explorationDataMaxWaitUntil;
        private readonly bool _useIgnoreRegions;

        private bool NotStarted()
        {
            State = States.Exploring;

            if (_ignoreScenes != null && _ignoreScenes.Any())
                ExplorationHelpers.MarkNodesAsVisited(_ignoreScenes);

            return false;
        }

        private async Task<bool> Exploring()
        {
            if (_useIgnoreRegions)
            {
                ExplorationHelpers.UpdateIgnoreRegions();
            }

            if (_currentDestination == null || _currentDestination.IsVisited)
            {
                if (_explorationDataMaxWaitUntil != DateTime.MinValue && DateTime.UtcNow > _explorationDataMaxWaitUntil)
                {
                    Core.Logger.Debug("[Exploration] Timeout waiting for exploration data");
                    State = States.Completed;
                    return false;
                }

                if (!ExplorationGrid.Instance.WalkableNodes.Any())
                {
                    if (_explorationDataMaxWaitUntil == DateTime.MinValue)
                    {
                        _explorationDataMaxWaitUntil = DateTime.UtcNow + TimeSpan.FromSeconds(15);
                    }
                    Core.Scenes.Update();
                    await Coroutine.Yield();
                    Core.Logger.Debug("[Exploration] Patiently waiting for exploration data");
                    return false;
                }

                _explorationDataMaxWaitUntil = DateTime.MinValue;

                if (_currentDestination != null)
                {
                    _currentDestination.IsCurrentDestination = false;
                }

                var destination = ExplorationHelpers.NearestWeightedUnvisitedNode(_levelAreaIds);

                // Ignore marking nodes as Visited for bounties.
                if (destination != null && ZetaDia.Storage.Quests.ActiveBounty == null)
                {
                    WorldScene destScene = destination.Scene;
                    Vector3 destinationPos = destination.NavigableCenter;

                    var exitPositions = destScene.ExitPositions;
                    var connectedScenes = destScene.ConnectedScenes();
                    var unconnectedExits =
                        exitPositions.Where(ep => connectedScenes.FirstOrDefault(cs => cs.Direction == ep.Key) == null);

                    if (destinationPos.Distance(ExplorationHelpers.PriorityPosition) >= 15)
                    {
                        if (!unconnectedExits.Any(ep => destinationPos.Distance(ep.Value) <= 15) &&
                            ZetaDia.Minimap.IsExplored(destinationPos, AdvDia.CurrentWorldDynamicId))
                        {
                            destination.IsVisited = true;
                            destination.IsKnown = true;
                            return false;
                        }
                    }
                }

                if (destination == null)
                {
                    Core.Logger.Debug($"[Exploration] No more unvisited nodes to explore, so we're done.");
                    State = States.Completed;
                    return false;
                }

                if (_currentDestination != destination)
                {
                    Core.Logger.Debug($"[Exploration] Destination Changed from {_currentDestination?.NavigableCenter} to {destination.NavigableCenter}");
                    _currentDestination = destination;
                }
                if (_currentDestination != null)
                {
                    Core.Logger.Debug($"[Exploration] Current Destination {_currentDestination?.NavigableCenter}, CanRayWalk={CanRayWalkDestination} MyPosition={AdvDia.MyPosition}");
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

                        var canClientPathTo = await AdvDia.Navigator.CanFullyClientPathTo(_currentDestination.NavigableCenter);
                        if (_currentDestination.FailedNavigationAttempts >= 10 && !canClientPathTo)
                        {
                            Core.Logger.Debug($"[Exploration] Unable to client path to {_currentDestination.NavigableCenter} and failed {_currentDestination.FailedNavigationAttempts} times; Ignoring Node.");
                            _currentDestination.IsVisited = true;
                            _currentDestination.IsIgnored = true;
                            _currentDestination.IsCurrentDestination = false;
                            _currentDestination = null;
                            _failedNavigationAttempts++;
                        }
                        else if (!CanRayWalkDestination && _currentDestination.Distance < 25f && _currentDestination.FailedNavigationAttempts >= 3 || _currentDestination.FailedNavigationAttempts >= 15)
                        {
                            Core.Logger.Debug($"[Exploration] Failed to Navigate to {_currentDestination.NavigableCenter} {_currentDestination.FailedNavigationAttempts} times; Ignoring Node.");
                            _currentDestination.IsVisited = true;
                            _currentDestination.IsIgnored = true;
                            _currentDestination.IsCurrentDestination = false;
                            _currentDestination = null;
                            _failedNavigationAttempts++;
                        }
                    }
                    else
                    {
                        Core.Logger.Debug($"[Exploration] Destination Reached!");
                        _currentDestination.FailedNavigationAttempts = 0;
                        _currentDestination.IsVisited = true;
                        _currentDestination.IsCurrentDestination = false;
                        _currentDestination = null;
                    }

                    if (_failedNavigationAttempts > 25)
                    {
                        if (_allowReExplore)
                        {
                            Core.Logger.Debug($"[Exploration] Exploration Resetting");
                            Core.Scenes.Reset();
                            Navigator.Clear();
                            _failedNavigationAttempts = 0;
                        }
                        else
                        {
                            Core.Logger.Debug($"[Exploration] too many failed navigation attempts, aborting.");
                            State = States.Completed;
                            return false;
                        }
                    }
                }
                return false;
            }

            Core.Logger.Debug($"[Exploration] We found no explore destination, so we're done.");
            Core.Scenes.Reset();
            Navigator.Clear();

            State = States.Completed;
            return false;
        }

        public bool CanRayWalkDestination => TrinityGrid.Instance.CanRayWalk(AdvDia.MyPosition, _currentDestination.NavigableCenter);

        private bool Completed()
        {
            return true;
        }

        #region ISubRoutine

        public bool IsDone => State == States.Completed;

        //Task<bool> ISubroutine.GetCoroutine()
        //{
        //    return GetCoroutine();
        //}

        public void Reset()
        {
            State = States.NotStarted;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {

        }

        #endregion
    }
}
