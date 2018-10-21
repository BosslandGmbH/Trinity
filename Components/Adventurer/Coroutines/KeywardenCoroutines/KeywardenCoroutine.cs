using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Coroutines;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Adventurer.Coroutines.KeywardenCoroutines
{
    public class KeywardenCoroutine : IDisposable, ICoroutine
    {
        private readonly KeywardenData _keywardenData;
        private Vector3 _keywardenLocation = Vector3.Zero;
        private readonly HashSet<int> _levelAreaIds;
        private WaitCoroutine _waitCoroutine;
        private DateTime _markerCooldownUntil = DateTime.MinValue;

        private enum States
        {
            NotStarted,
            TakingWaypoint,
            Searching,
            Moving,
            Waiting,
            Looting,
            Completed,
            Failed
        }

        private States _state;

        private States State
        {
            get => _state;
            set
            {
                if (_state == value) return;

                switch (value)
                {
                    case States.NotStarted:
                        break;

                    default:
                        Core.Logger.Debug("[Keywarden] " + value);
                        StatusText = "[Keywarden] " + value;
                        break;
                }
                _state = value;
            }
        }

        public KeywardenCoroutine(KeywardenData keywardenData)
        {
            _keywardenData = keywardenData;
            _levelAreaIds = new HashSet<int> { _keywardenData.LevelAreaId };
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();

                case States.TakingWaypoint:
                    return await TakingWaypoint();

                case States.Searching:
                    return await Searching();

                case States.Moving:
                    return await Moving();

                case States.Waiting:
                    return await Waiting();

                case States.Looting:
                    return await Looting();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }

        public void Reset()
        {
        }

        public string StatusText { get; set; }

        private bool NotStarted()
        {
            DisablePulse();
            _keywardenLocation = Vector3.Zero;

            if (!_keywardenData.IsAlive)
            {
                State = States.Looting;
                return false;
            }
            TargetingHelper.TurnCombatOn();
            Core.Logger.Log("[Keywarden] Lets go find da guy with da machina, shall we?");
            State = AdvDia.CurrentLevelAreaId == _keywardenData.LevelAreaId ? States.Searching : States.TakingWaypoint;
            return false;
        }

        private async Task<bool> TakingWaypoint()
        {
            DisablePulse();
            Waypoint wp = ZetaDia.Storage.ActManager.GetWaypointByLevelAreaSnoId(_keywardenData.WaypointLevelAreaId);
            if (!await WaypointCoroutine.UseWaypoint(wp.Number)) return false;
            State = States.Searching;
            return false;
        }

        private async Task<bool> Searching()
        {
            if (!_keywardenData.IsAlive)
            {
                State = States.Waiting;
                return false;
            }
            EnablePulse();

            if (_keywardenLocation != Vector3.Zero && DateTime.UtcNow > MoveCooldownUntil)
            {
                State = States.Moving;
                Core.Logger.Log("[Keywarden] It's clobberin time!");
                return false;
            }

            if (!await MoveToMarker())
                return false;

            if (!await ExplorationCoroutine.Explore(_levelAreaIds, null, CanMoveToMarker))
                return false;

            Core.Logger.Error("[Keywarden] Oh shit, that guy is nowhere to be found.");
            Core.Scenes.ResetVisited();
            State = States.Searching;
            return false;
        }

        private async Task<bool> MoveToMarker()
        {
            if (!CanMoveToMarker())
                return true;

            if (_minimapMarker.Position.Distance(AdvDia.MyPosition) < 20f)
            {
                Core.Logger.Log("[Keywarden] Finished Following marker");
                return true;
            }

            if (_markerCoroutine == null)
            {
                Core.Logger.Log("[Keywarden] Following a keywarden marker, lets see where it goes");
                _markerCoroutine = new MoveToMapMarkerCoroutine(-1, AdvDia.CurrentWorldId, _minimapMarker.NameHash);
            }

            if (!_markerCoroutine.IsDone)
            {
                if (!await _markerCoroutine.GetCoroutine())
                    return false;
            }

            if (_markerCoroutine.State == MoveToMapMarkerCoroutine.States.Failed)
            {
                var cooldownDurationSeconds = 15 + _markerMoveFailures * 2;
                _markerMoveFailures++;
                _markerCooldownUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(cooldownDurationSeconds));
                _markerCoroutine = null;
                Core.Logger.Log($"[Keywarden] Looks like we can't find a path to the keywarden marker :( on cooldown for {cooldownDurationSeconds} seconds");
                return true;
            }

            Core.Logger.Log("[Keywarden] Finished Following marker");
            _markerCoroutine = null;
            return true;
        }

        private bool CanMoveToMarker()
        {
            if (_markerCooldownUntil > DateTime.UtcNow)
            {
                Core.Logger.Debug($"Keywarden Marker on Cooldown. {(_markerCooldownUntil.Subtract(DateTime.UtcNow).TotalSeconds)}s remaining");
                return false;
            }

            _minimapMarker = GetKeywardenMarker();
            if (_minimapMarker == null)
            {
                Core.Logger.Debug("Failed to find Keywarden Marker");
            }
            return _minimapMarker != null;
        }

        private MoveToMapMarkerCoroutine _markerCoroutine;

        private async Task<bool> Moving()
        {
            EnablePulse();
            TargetingHelper.TurnCombatOn();

            if (!await NavigationCoroutine.MoveTo(_keywardenLocation, 15))
            {
                return false;
            }

            if (NavigationCoroutine.LastResult == CoroutineResult.Failure && (NavigationCoroutine.LastMoveResult == MoveResult.Failed || NavigationCoroutine.LastMoveResult == MoveResult.PathGenerationFailed))
            {
                var canClientPathTo = await AdvDia.Navigator.CanFullyClientPathTo(_keywardenLocation);
                if (!canClientPathTo)
                {
                    State = States.Searching;
                    MoveCooldownUntil = DateTime.UtcNow.AddSeconds(10);
                    Core.Logger.Debug("[Keywarden] Can't seem to get to the keywarden!");
                }
                return false;
            }

            if (_keywardenData.IsAlive)
            {
                _keywardenLocation = GetKeywardenLocation();
                if (_keywardenLocation == Vector3.Zero)
                {
                    State = States.Searching;
                }
            }
            else
            {
                Core.Logger.Log("[Keywarden] Keywarden shish kebab!");
                State = States.Waiting;
            }
            return false;
        }

        public DateTime MoveCooldownUntil = DateTime.MinValue;

        private async Task<bool> Waiting()
        {
            if (_waitCoroutine == null)
            {
                _waitCoroutine = new WaitCoroutine(5000);
            }
            Core.Logger.Log("[Keywarden] Waiting...!");
            await Coroutine.Yield();
            State = States.Looting;
            return false;
        }

        private async Task<bool> Looting()
        {
            DisablePulse();
            var loots = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).OrderBy(x => x.Distance).Where(x => x.IsFullyValid() && KeywardenDataFactory.KeyIds.Contains(x.ActorSnoId)).ToList();
            if (!loots.Any())
            {
                StatusText = "[Keywarden] No Loot!";
                Core.Logger.Log("[Keywarden] No Loot!");
                State = States.Completed;
                return false;
            }
            foreach (var loot in loots)
            {
                if (await MoveToAndInteract.Execute(loot, 0, 5))
                    return true;
                await Coroutine.Yield();
            }
            State = States.Completed;
            return false;
        }

        private async Task<bool> Completed()
        {
            StatusText = "[Keywarden] Completed";
            DisablePulse();
            return true;
        }

        private async Task<bool> Failed()
        {
            DisablePulse();
            return true;
        }

        private void Scans()
        {
            _keywardenLocation = GetKeywardenLocation();
            if (State == States.Searching)
            {
                PulseCheck();
                ZergCheck();
            }
        }

        private DateTime _lastPulseCheck = DateTime.MinValue;

        private void PulseCheck()
        {
            if (DateTime.UtcNow.Subtract(_lastPulseCheck).TotalSeconds < 5)
                return;

            _lastPulseCheck = DateTime.UtcNow;

            // marker check
        }

        private Vector3 GetKeywardenLocation()
        {
            if (_keywardenLocation != Vector3.Zero) return _keywardenLocation;
            var keywarden = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault(a => a.IsFullyValid() && a.ActorSnoId == _keywardenData.KeywardenSNO);
            return (keywarden != null) ? keywarden.Position : Vector3.Zero;
        }

        public MinimapMarker GetKeywardenMarker()
        {
            var marker = ZetaDia.Minimap.Markers.CurrentWorldMarkers.FirstOrDefault(m => m.MinimapTextureSnoId == 81058 && m.IsPointOfInterest && _keyardenMarkerHashes.Contains(m.NameHash));
            if (marker != null)
            {
                //Core.Logger.Log("Found Keywarden Marker Distance={0} NameHash={1}",
                //    marker.Position.Distance(AdvDia.MyPosition), marker.NameHash);

                return marker;
            }
            //A3 - Id = 2 MinimapTextureSnoId = 81058 NameHash = 1424459156 IsPointOfInterest = True IsPortalEntrance = False IsPortalExit = False IsWaypoint = False Location = x = "3987" y = "3668" z = "29"  Distance = 33
            //A1 - Id = 3 MinimapTextureSnoId = 81058 NameHash = 1928482775 IsPointOfInterest = True IsPortalEntrance = False IsPortalExit = False IsWaypoint = False Location = x = "1774" y = "772" z = "10"  Distance = 141
            //A4 - Id=4 MinimapTextureSnoId=81058 NameHash=-946519244 IsPointOfInterest=True IsPortalEntrance=False IsPortalExit=False IsWaypoint=False Location=x="3012" y="2139" z="-23"  Distance=426
            // Id = 5 MinimapTextureSnoId = 81058 NameHash = -38012987 IsPointOfInterest = True IsPortalEntrance = False IsPortalExit = False IsWaypoint = False Location = x = "2808" y = "4543" z = "112"  Distance = 279
            return null;
        }

        private readonly HashSet<int> _keyardenMarkerHashes = new HashSet<int>
        {
            1424459156,
            1928482775,
            -946519244,
            -38012987
        };

        private void ZergCheck()
        {
            if (PluginSettings.Current.KeywardenZergMode.HasValue && !PluginSettings.Current.KeywardenZergMode.Value)
            {
                SafeZerg.Instance.DisableZerg();
                return;
            }

            SafeZerg.Instance.EnableZerg();

            //if (PluginSettings.Current.KeywardenZergMode.HasValue && !PluginSettings.Current.KeywardenZergMode.Value)
            //{
            //    return;
            //}
            //var corruptGrowthDetectionRadius = ZetaDia.Me.ActorClass == ActorClass.Barbarian ? 30 : 20;
            //var combatState = false;

            //if (_keywardenLocation != Vector3.Zero && _keywardenLocation.Distance(AdvDia.MyPosition) <= 50f)
            //{
            //    TargetingHelper.TurnCombatOn();
            //    return;
            //}

            //if (!combatState && ZetaDia.Me.HitpointsCurrentPct <= 0.8f)
            //{
            //    combatState = true;
            //}

            //if (!combatState && _keywardenData.Act == Act.A4)
            //{
            //    if (ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Any(
            //                a =>
            //                    a.IsFullyValid() && KeywardenDataFactory.A4CorruptionSNOs.Contains(a.ActorSnoId) &&
            //                    a.IsAlive & a.Position.Distance(AdvDia.MyPosition) <= corruptGrowthDetectionRadius))
            //    {
            //        combatState = true;
            //    }
            //}

            //if (!combatState && ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Any(u => u.IsFullyValid() && u.IsAlive && KeywardenDataFactory.GoblinSNOs.Contains(u.ActorSnoId)))
            //{
            //    combatState = true;
            //}

            //if (!combatState && ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Count(u => u.IsFullyValid() && u.IsHostile && u.IsAlive && u.Position.Distance(AdvDia.MyPosition) <= 15f) >= 4)
            //{
            //    combatState = true;
            //}

            //if (combatState)
            //{
            //    TargetingHelper.TurnCombatOn();
            //}
            //else
            //{
            //    TargetingHelper.TurnCombatOff();
            //}
        }

        #region OnPulse Implementation

        private readonly WaitTimer _pulseTimer = new WaitTimer(TimeSpan.FromMilliseconds(250));
        private bool _isPulsing;
        private MinimapMarker _minimapMarker;
        private int _markerMoveFailures;

        private void EnablePulse()
        {
            if (!_isPulsing)
            {
                //Core.Logger.Debug("[Rift] Registered to pulsator.");
                Pulsator.OnPulse += OnPulse;
                _isPulsing = true;
            }
        }

        private void DisablePulse()
        {
            if (_isPulsing)
            {
                //Core.Logger.Debug("[Rift] Unregistered from pulsator.");
                Pulsator.OnPulse -= OnPulse;
                _isPulsing = false;
            }
        }

        private void OnPulse(object sender, EventArgs e)
        {
            if (!Adventurer.GetCurrentTag().StartsWith("KeywardensTag"))
            {
                DisablePulse();
                return;
            }
            if (_pulseTimer.IsFinished)
            {
                _pulseTimer.Stop();
                Scans();
                _pulseTimer.Reset();
            }
        }

        #endregion OnPulse Implementation

        public void Dispose()
        {
            DisablePulse();
        }
    }
}
