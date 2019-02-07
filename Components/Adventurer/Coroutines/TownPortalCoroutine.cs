using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Framework;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines
{
    public sealed class TownPortalCoroutine : ICoroutine
    {
        private static TownPortalCoroutine _townPortalCoroutine;

        public static async Task<bool> UseWaypoint()
        {
            if (AdvDia.IsInTown)
                return true;

            if (_townPortalCoroutine == null)
            {
                _townPortalCoroutine = new TownPortalCoroutine();
            }
            if (await _townPortalCoroutine.GetCoroutine())
            {
                _townPortalCoroutine = null;
                return true;
            }
            return false;
        }

        public TownPortalCoroutine()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public void Reset()
        {
        }

        public string StatusText { get; set; }

        private Vector3 _startingPosition;

        private enum States
        {
            NotStarted,
            ClearingArea,
            UsingTownPortal,
            UsedTownPortal,
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
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[TownPortal] " + value);
                    StatusText = "[TownPortal] " + value;
                }
                _state = value;
            }
        }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();

                case States.ClearingArea:
                    return await ClearingArea();

                case States.UsingTownPortal:
                    return await UsingTownPortal();

                case States.UsedTownPortal:
                    return await UsedTownPortal();

                case States.Completed:
                    return Completed();

                case States.Failed:
                    return Failed();
            }
            return false;
        }

        private bool NotStarted()
        {
            _startingPosition = AdvDia.MyPosition;
            State = States.ClearingArea;
            return false;
        }

        private async Task<bool> ClearingArea()
        {
            if (await ClearAreaCoroutine.Clear(_startingPosition, 60))
            {
                State = States.UsingTownPortal;
            }
            return false;
        }

        private async Task<bool> UsingTownPortal()
        {
            if (HasReachedDestionation)
            {
                State = States.Completed;
                return false;
            }

            if (await CommonCoroutines.UseTownPortal("Adventurer") == Zeta.Bot.Coroutines.CoroutineResult.Running)
                return false;

            State = States.UsedTownPortal;
            return false;
        }

        private static readonly List<int> TransportStates = new List<int> { 3, 13 };

        private async Task<bool> UsedTownPortal()
        {
            if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || ZetaDia.Globals.IsPlayingCutscene)
                return false;

            if (ZetaDia.Me == null || ZetaDia.Me.CommonData == null || !ZetaDia.Me.IsValid || !ZetaDia.Me.CommonData.IsValid)
                return false;

            if (TransportStates.Contains((int)ZetaDia.Me.CommonData.AnimationState))
            {
                return false;
            }

            State = HasReachedDestionation ? States.Completed : States.NotStarted;

            Navigator.Clear();
            return false;
        }

        private bool Completed()
        {
            return true;
        }

        private bool Failed()
        {
            return true;
        }

        private bool HasReachedDestionation => ZetaDia.IsInTown;
    }
}