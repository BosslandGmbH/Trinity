using Buddy.Coroutines;
using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines
{
    public class CompleteActBountiesCoroutine
    {
        private const int TYRAEL = 114622;

        private static Dictionary<Act, Vector3> TyraelPositions = new Dictionary<Act, Vector3>
        {
            {Act.A1, new Vector3(414, 537, 24)},
            {Act.A2, new Vector3(316, 264, 0)},
            {Act.A3, new Vector3(385, 421, 0)},
            {Act.A4, new Vector3(385, 421, 0)},
            {Act.A5, new Vector3(568, 745, 2)},
        };

        private readonly Act _act;
        private InteractionCoroutine _interactionCoroutine;

        public enum States
        {
            NotStarted,
            ReturningToTown,
            TownRun,
            MovingToTyrael,
            InteractingWithTyrael,
            Completed,
            Failed,
            AlreadyDone
        }

        protected bool _logStateChange;

        protected bool LogStateChange
        {
            get
            {
                if (!_logStateChange) return false;
                _logStateChange = false;
                return true;
            }
        }

        private States _state;

        public States State
        {
            get { return _state; }
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[CompleteActBounties] " + value);
                }
                _logStateChange = true;
                _state = value;
            }
        }

        public CompleteActBountiesCoroutine(Act act)
        {
            _act = act;
        }

        public virtual async Task<bool> GetCoroutine()
        {
            if (BrainBehavior.IsVendoring) return false;
            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.ReturningToTown:
                    return await ReturningToTown();

                case States.TownRun:
                    return TownRun();

                case States.MovingToTyrael:
                    return await MovingToTyrael();

                case States.InteractingWithTyrael:
                    return await InteractingWithTyrael();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();

                case States.AlreadyDone:
                    return AlreadyDone();
            }
            return false;
        }

        private bool _isDone;
        public bool IsDone { get { return _isDone; } }

        private async Task<bool> NotStarted()
        {
            await Coroutine.Sleep(1000);
            if (!BountyHelpers.IsActTurninInProgress(_act))
            {
                State = States.AlreadyDone;
                return false;
            }

            if (IsInZone)
            {
                State = States.TownRun;
                return false;
            }
            State = States.ReturningToTown;
            Core.Logger.Log("[CompleteActBounties] Time to return to the town and claim our prize, huzzah!");
            return false;
        }

        private async Task<bool> ReturningToTown()
        {
            if (IsInZone)
            {
                State = States.TownRun;
                return false;
            }
            if (!await WaypointCoroutine.UseWaypoint(WaypointFactory.ActHubs[_act])) return false;

            return false;
        }

        private bool _townRunInitiated;

        private bool TownRun()
        {
            if (BrainBehavior.IsVendoring)
            {
                return false;
            }
            if (!_townRunInitiated)
            {
                _townRunInitiated = true;
                BrainBehavior.ForceTownrun(" We need it.", true);
            }
            if (BrainBehavior.IsVendoring) return false;
            _townRunInitiated = false;

            State = States.MovingToTyrael;

            return false;
        }

        private async Task<bool> MovingToTyrael()
        {
            if (!await NavigationCoroutine.MoveTo(TyraelPositions[_act], 2)) return false;
            _interactionCoroutine = new InteractionCoroutine(TYRAEL, new TimeSpan(0, 0, 20), new TimeSpan(0, 0, 1));
            State = States.InteractingWithTyrael;
            return false;
        }

        private async Task<bool> InteractingWithTyrael()
        {
            if (await _interactionCoroutine.GetCoroutine())
            {
                if (!ZetaDia.Storage.Quests.AllQuests.Any(q => q.Quest == BountyHelpers.ActBountyFinishingQuests[_act] && q.State == QuestState.InProgress))
                {
                    State = States.Completed;
                    return false;
                }
                var tyrael = ActorFinder.FindUnit(TYRAEL);
                if (tyrael == null)
                {
                    Core.Logger.Error("[CompleteActBounties] Couldn't detect Tyrael. Failing");
                    State = States.Failed;
                    return false;
                }
                if (tyrael.IsFullyValid() && tyrael.CommonData.MarkerType == MarkerType.Exclamation)
                {
                    return false;
                }
                State = States.Completed;
            }
            return false;
        }

        private async Task<bool> Completed()
        {
            Core.Logger.Log("[ActBounties] Successfully completed {0} bounties", _act);
            _isDone = true;
            await Coroutine.Sleep(4000);
            return true;
        }

        private async Task<bool> Failed()
        {
            Core.Logger.Log("[ActBounties] Failed to completed {0} bounties", _act);
            _isDone = true;
            await Coroutine.Sleep(1000);
            return true;
        }

        private bool AlreadyDone()
        {
            Core.Logger.Debug("[CompleteActBounties] No active act {0} bounty finishing quest", _act);
            return true;
        }

        public bool IsInZone
        {
            get
            {
                return ExplorationData.ActHubWorldIds[_act] == AdvDia.CurrentWorldId;
            }
        }
    }
}