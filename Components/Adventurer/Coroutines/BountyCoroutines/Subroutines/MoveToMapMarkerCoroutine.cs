using System.Collections.Generic;
using System.Linq; using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Modules;
using Zeta.Common;


namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class MoveToMapMarkerCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private readonly int _markerHash;

        private bool _isDone;
        private States _state;

        private int _objectiveScanRange = 5000;

        #region State

        public enum States
        {
            NotStarted,
            Searching,
            Moving,
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
                    Core.Logger.Log("[MoveToMapMarker] " + value);
                }
                _state = value;
            }
        }

        #endregion State

        public bool IsDone
        {
            get { return _isDone || AdvDia.CurrentWorldId != _worldId; }
        }

        public MoveToMapMarkerCoroutine(int questId, int worldId, int markerHash, bool zergAllowSafe = true)
        {
            _questId = questId;
            _worldId = worldId;
            _markerHash = markerHash;
            _allowSafeZerg = zergAllowSafe;
        }

        public MoveToMapMarkerCoroutine(int questId, int worldId, WorldMarkerType type, bool zergAllowSafe = true)
        {
            _questId = questId;
            _worldId = worldId;
            _markerType = type;
            _allowSafeZerg = zergAllowSafe;
        }

        public MoveToMapMarkerCoroutine(int questId, int worldId, string name, bool zergAllowSafe = true)
        {
            _questId = questId;
            _worldId = worldId;
            _markerName = name;
            _allowSafeZerg = zergAllowSafe;
        }

        public async Task<bool> GetCoroutine()
        {
            if (_allowSafeZerg && PluginSettings.Current.BountyZerg && BountyData != null &&
                BountyData.QuestType != BountyQuestType.KillMonster)
            {
                Core.Logger.Verbose("Enabling SafeZerg for Kill Monster bounty");
                SafeZerg.Instance.EnableZerg();
            }
            else if (!_allowSafeZerg)
            {
                SafeZerg.Instance.DisableZerg();
            }

            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.Searching:
                    return await Searching();

                case States.Moving:
                    return await Moving();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }

        public void Reset()
        {
            _isDone = false;
            _state = States.NotStarted;
            _objectiveScanRange = 5000;
            _objectiveLocation = Vector3.Zero;
        }

        public void DisablePulse()
        {
        }

        public BountyData BountyData
        {
            get { return _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId)); }
        }

        private async Task<bool> NotStarted()
        {
            State = States.Searching;
            return false;
        }

        private async Task<bool> Searching()
        {
            if (_objectiveLocation == Vector3.Zero)
            {
                ScanForObjective();
            }
            if (_objectiveLocation != Vector3.Zero)
            {
                State = States.Moving;
                _partialMovesCount = 0;
                return false;
            }

            // End coroutine so we can fall through to exploration with a scan for specific actor.
            State = States.Failed;
            return false;

            //var levelAreas = BountyData != null && BountyData.LevelAreaIds != null && BountyData.LevelAreaIds.Any()
            //    ? BountyData.LevelAreaIds
            //    : new HashSet<int> { AdvDia.CurrentLevelAreaId };

            //if (!await ExplorationCoroutine.Explore(levelAreas)) return false;
            //ScenesStorage.Reset();
            //return false;
        }

        private int _partialMovesCount;
        private Vector3 _partialMoveLocation;
        private bool _isPartialMove;

        private async Task<bool> Moving()
        {
            if (_isPartialMove)
            {
                if (!await NavigationCoroutine.MoveTo(_partialMoveLocation, 10))
                    return false;

                Core.Logger.Debug("Reverting after partial move");
                _isPartialMove = false;
            }
            else
            {
                if (!await NavigationCoroutine.MoveTo(_objectiveLocation, 10))
                    return false;
            }

            if (AdvDia.MyPosition.Distance(_objectiveLocation) > 30 && NavigationCoroutine.LastResult == CoroutineResult.Failure)
            {
                _partialMovesCount++;
                if (_partialMovesCount < 4)
                {
                    Core.Logger.Debug("Creating partial move segment");
                    _partialMoveLocation = MathEx.CalculatePointFrom(AdvDia.MyPosition, _objectiveLocation, 125f);
                    _isPartialMove = true;
                    return false;
                }
                _previouslyFoundLocation = _objectiveLocation;
                _returnTimeForPreviousLocation = PluginTime.CurrentMillisecond;
                _partialMovesCount = 0;
                _isPartialMove = false;
                _objectiveLocation = Vector3.Zero;
                _objectiveScanRange = ActorFinder.LowerSearchRadius(_objectiveScanRange);
                if (_objectiveScanRange <= 0)
                {
                    _objectiveScanRange = 50;
                }
                State = States.Searching;
                return false;
            }
            SafeZerg.Instance.DisableZerg();
            State = States.Completed;
            return false;
        }

        private async Task<bool> Completed()
        {
            SafeZerg.Instance.DisableZerg();
            _isDone = true;
            return false;
        }

        private async Task<bool> Failed()
        {
            _isDone = true;
            return false;
        }

        private Vector3 _objectiveLocation = Vector3.Zero;
        private Vector3 _previouslyFoundLocation = Vector3.Zero;
        private long _returnTimeForPreviousLocation;

        private long _lastScanTime;
        private BountyData _bountyData;
        private bool _allowSafeZerg;
        private WorldMarkerType _markerType;
        private string _markerName;

        private void ScanForObjective()
        {
            if (_previouslyFoundLocation != Vector3.Zero && PluginTime.ReadyToUse(_returnTimeForPreviousLocation, 60000))
            {
                _objectiveLocation = _previouslyFoundLocation;
                _previouslyFoundLocation = Vector3.Zero;
                _returnTimeForPreviousLocation = PluginTime.CurrentMillisecond;
                Core.Logger.Debug("Returning previous objective location.");
                return;
            }
            if (PluginTime.ReadyToUse(_lastScanTime, 1000))
            {
                _lastScanTime = PluginTime.CurrentMillisecond;
                TrinityMarker marker = null;

                if (_markerHash != 0)
                {
                    if (_markerHash == -1)
                    {
                        marker = BountyHelpers.ScanForMarker(0, _objectiveScanRange);                        
                    }
                    else
                    {
                        marker = BountyHelpers.ScanForMarker(_markerHash, _objectiveScanRange);
                    }
                }
                else if (!string.IsNullOrEmpty(_markerName))
                {
                    marker = BountyHelpers.ScanForMarker(_markerName, _objectiveScanRange);
                }
                else if (_markerType != default(WorldMarkerType))
                {
                    marker = BountyHelpers.ScanForMarker(_markerType, _objectiveScanRange);
                }

                if (marker != null && marker.Position != Vector3.Zero)
                {
                    _objectiveLocation = marker.Position;
                    Core.Logger.Log($"[MoveToMapMarker] Found the objective {marker}");
                }
                _objectiveLocation = marker?.Position ?? Vector3.Zero;
            }
        }
    }
}