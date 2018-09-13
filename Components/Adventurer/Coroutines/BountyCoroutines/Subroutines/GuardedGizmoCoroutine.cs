using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class GuardedGizmoCoroutine : IBountySubroutine, IDisposable
    {
        private readonly int _questId;
        private InteractionCoroutine _interactionCoroutine;
        private bool _isDone;
        private BountyData _bountyData;
        private Dictionary<Vector3, GuardedGizmo> _guardedGizmos = new Dictionary<Vector3, GuardedGizmo>();
        private GuardedGizmo _currentGizmo;

        public enum States
        {
            NotStarted,
            SearchingForGizmo,
            MovingToGizmo,
            PreClearingGizmoArea,
            InteractingWithGizmo,
            ClearingGizmoArea,
            Completed
        }

        private States _state;

        public States State
        {
            get => _state;
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Log("[GuardedGizmo] " + value);
                    StatusText = "[GuardedGizmo] " + value;
                }

                _state = value;
            }
        }

        public int GizmoSNO { get; private set; }

        public int ObjectSearchRadius
        {
            get
            {
                if (!ExplorationData.OpenWorldIds.Contains(AdvDia.CurrentWorldId))
                {
                    return _objectSearchRadius / 2;
                }
                return _objectSearchRadius;
            }
            set => _objectSearchRadius = value;
        }

        public GuardedGizmoCoroutine(int questId, int actorId)
        {
            _questId = questId;
            GizmoSNO = actorId;
            ObjectSearchRadius = 1000;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public bool IsDone => _isDone;

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();

                case States.SearchingForGizmo:
                    return await SearchingForGizmo();

                case States.MovingToGizmo:
                    return await MovingToGizmo();

                case States.PreClearingGizmoArea:
                    return await PreClearingGizmoArea();

                case States.InteractingWithGizmo:
                    return await InteractingWithGizmo();

                case States.ClearingGizmoArea:
                    return await ClearingGizmoArea();

                case States.Completed:
                    return Completed();
            }
            return false;
        }

        public void Reset()
        {
            State = States.NotStarted;
            _isDone = false;
            _guardedGizmos = new Dictionary<Vector3, GuardedGizmo>();
            _currentGizmo = null;
        }

        public string StatusText { get; set; }

        public BountyData BountyData => _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId));

        private bool NotStarted()
        {
            if (PluginSettings.Current.BountyZerg) SafeZerg.Instance.EnableZerg();
            State = States.SearchingForGizmo;
            if (AdvDia.CurrentLevelAreaId == 263493)
            {
                _objectSearchRadius = 150;
            }
            return false;
        }

        private int _objectSearchRadius;

        private async Task<bool> SearchingForGizmo()
        {
            if (PluginSettings.Current.BountyZerg) SafeZerg.Instance.EnableZerg();
            EnablePulse();

            PulseChecks();

            _currentGizmo =
                _guardedGizmos.Values.Where(g => !g.HasBeenOperated)
                    .OrderBy(g => g.Position.DistanceSqr(AdvDia.MyPosition))
                    .FirstOrDefault();

            if (_currentGizmo != null)
            {
                State = States.MovingToGizmo;
                return false;
            }

            if (!await ExplorationCoroutine.Explore(BountyData.LevelAreaIds)) return false;
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> MovingToGizmo()
        {
            if (PluginSettings.Current.BountyZerg) SafeZerg.Instance.EnableZerg();
            EnablePulse();
            if (_currentGizmo == null)
            {
                State = States.SearchingForGizmo;
                return false;
            }

            if (!await NavigationCoroutine.MoveTo(_currentGizmo.Position, _currentGizmo.InteractDistance))
                return false;

            if (NavigationCoroutine.LastResult == CoroutineResult.Failure && AdvDia.MyPosition.Distance(_currentGizmo.Position) > _currentGizmo.InteractDistance + 20)
            {
                Core.PlayerMover.MoveTowards(_currentGizmo.Position);
                ObjectSearchRadius = 150;
                _guardedGizmos.Remove(_currentGizmo.Position);
                Core.Logger.Log("[Bounty] Gizmo is out of reach, lowering the search radius to {0}", ObjectSearchRadius);
                State = States.SearchingForGizmo;
                return false;
            }
            var gizmo =
                ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                    .FirstOrDefault(g => g.ActorSnoId == GizmoSNO && g.Position.Distance(_currentGizmo.Position) < 5);
            if (gizmo == null)
            {
                _currentGizmo.HasBeenOperated = true;
                State = States.SearchingForGizmo;
                return false;
            }
            State = States.PreClearingGizmoArea;
            return false;
        }

        private async Task<bool> PreClearingGizmoArea()
        {
            SafeZerg.Instance.DisableZerg();
            EnablePulse();
            if (!await ClearAreaCoroutine.Clear(_currentGizmo.Position, 70)) return false;
            _interactionCoroutine = new InteractionCoroutine(GizmoSNO, new TimeSpan(0, 0, 7), new TimeSpan(0, 0, 1), 3);
            State = States.InteractingWithGizmo;
            return false;
        }

        private async Task<bool> InteractingWithGizmo()
        {
            SafeZerg.Instance.DisableZerg();
            EnablePulse();
            PulseChecks();
            //Refresh actor just in case
            if (_currentGizmo.Untargateble)
            {
                State = States.ClearingGizmoArea;
                return false;
            }
            if (_currentGizmo.HasBeenOperated)
            {
                State = States.SearchingForGizmo;
                return false;
            }

            if (!await _interactionCoroutine.GetCoroutine())
                return false;

            _interactionCoroutine = null;
            State = States.SearchingForGizmo;
            ObjectSearchRadius = 300;

            return false;
        }

        private async Task<bool> ClearingGizmoArea()
        {
            SafeZerg.Instance.DisableZerg();
            EnablePulse();
            if (!_currentGizmo.Untargateble)
            {
                State = States.MovingToGizmo;
                return false;
            }
            if (!await ClearAreaCoroutine.Clear(_currentGizmo.Position, 90, true)) return false;
            State = States.SearchingForGizmo;
            return false;
        }

        private bool Completed()
        {
            SafeZerg.Instance.DisableZerg();
            DisablePulse();
            State = States.NotStarted;
            return false;
        }

        private void PulseChecks()
        {
            var gizmos = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).Where(g => g.IsFullyValid() && g.ActorSnoId == GizmoSNO && g.Distance < ObjectSearchRadius);
            foreach (var gizmo in gizmos.Where(gizmo => gizmo.IsFullyValid()))
            {
                Vector3 position;
                bool hasBeenOperated;
                bool untargetable;
                try
                {
                    position = gizmo.Position;
                    hasBeenOperated = !ActorFinder.IsGizmoInteractable(gizmo);
                    untargetable = gizmo.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable) == 1;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("ReadProcessMemory"))
                    {
                        continue;
                    }
                    throw;
                }

                GuardedGizmo guardedGizmo;
                if (!_guardedGizmos.ContainsKey(position))
                {
                    guardedGizmo = new GuardedGizmo();
                    guardedGizmo.Position = position;
                    guardedGizmo.InteractDistance = (int)Math.Round(gizmo.CollisionSphere.Radius + 1, MidpointRounding.AwayFromZero);
                    _guardedGizmos.Add(position, guardedGizmo);
                }
                else
                {
                    guardedGizmo = _guardedGizmos[gizmo.Position];
                }
                guardedGizmo.HasBeenOperated = !ActorFinder.IsGizmoInteractable(gizmo);
                guardedGizmo.Untargateble = untargetable;
            }
        }

        #region OnPulse Implementation

        private readonly WaitTimer _pulseTimer = new WaitTimer(TimeSpan.FromMilliseconds(250));
        private bool _isPulsing;

        private void EnablePulse()
        {
            if (!_isPulsing)
            {
                Core.Logger.Debug("[GuardedGizmo] Registered to pulsator.");
                Pulsator.OnPulse += OnPulse;
                _isPulsing = true;
            }
        }

        public void DisablePulse()
        {
            if (_isPulsing)
            {
                Core.Logger.Debug("[GuardedGizmo] Unregistered from pulsator.");
                Pulsator.OnPulse -= OnPulse;
                _isPulsing = false;
            }
        }

        private void OnPulse(object sender, EventArgs e)
        {
            if (_pulseTimer.IsFinished)
            {
                _pulseTimer.Stop();
                PulseChecks();
                _pulseTimer.Reset();
            }
        }

        #endregion OnPulse Implementation

        public void Dispose()
        {
            DisablePulse();
        }

        private class GuardedGizmo
        {
            public Vector3 Position { get; set; }
            public bool HasBeenOperated { get; set; }
            public bool Untargateble { get; set; }
            public int InteractDistance { get; set; }
        }
    }
}