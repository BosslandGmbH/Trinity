using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("Interact")]
    public class InteractTag : TrinityProfileBehavior
    {
        [XmlAttribute("x")]
        [DefaultValue(0)]
        public float X { get; set; }

        [XmlAttribute("y")]
        [DefaultValue(0)]
        public float Y { get; set; }

        [XmlAttribute("z")]
        [DefaultValue(0)]
        public float Z { get; set; }

        [XmlAttribute("actorId")]
        [DefaultValue(0)]
        public int ActorId { get; set; }

        [XmlAttribute("worldSnoId")]
        [DefaultValue(0)]
        public int WorldSnoId { get; set; }

        public string State { get; set; } = "Not Started";

        public Vector3 Position => new Vector3(X, Y, Z);

        private bool _isDone;
        public override bool IsDone => _isDone;

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }

        private MoveToPositionCoroutine _moveToPositionTask;
        private MoveToActorCoroutine _moveToActorTask;
        private InteractionCoroutine _interactTask;

        public async Task<bool> Routine()
        {
            if (WorldSnoId != 0 && WorldSnoId != ZetaDia.CurrentWorldSnoId)
            {
                Logger.Log($"In the wrong world! Current={ZetaDia.CurrentWorldSnoId} Expected={WorldSnoId}");
                _isDone = true;
                return true;
            }

            // Move directly to a position if specified (works best for static worlds rather than exploration)

            if (Position != Vector3.Zero)
            {
                if (_moveToPositionTask == null)
                {
                    _moveToPositionTask = new MoveToPositionCoroutine(AdvDia.CurrentWorldId, new Vector3(X, Y, Z), 3);
                }

                if (!_moveToPositionTask.IsDone)
                {
                    if (!await _moveToPositionTask.GetCoroutine())
                    {
                        State = "MoveToPosition." + _moveToPositionTask.State;
                        return true;
                    }
                }
            }

            //If an actor wasn't specified try to find something to click on nearby.
            //if (ActorId == 0)
            //{
            //    var closestActor = ZetaDia.Actors.GetActorsOfType<DiaGizmo>().OrderBy(a => a.Distance).FirstOrDefault();
            //    if (closestActor != null)
            //    {
            //        Logger.Log($"Selected nearby actor {closestActor.Name} ({closestActor.ActorSnoId})");
            //        ActorId = closestActor.ActorSnoId;
            //    }
            //}

            // Find the actor without a position specified

            if (Position == Vector3.Zero)
            {
                if (_moveToActorTask == null)
                {
                    _moveToActorTask = new MoveToActorCoroutine(QuestId, AdvDia.CurrentWorldId, ActorId);
                }

                if (!_moveToActorTask.IsDone)
                {
                    if (!await _moveToActorTask.GetCoroutine())
                    {
                        State = "MoveToActor." + _moveToActorTask.State;
                        return true;
                    }
                }
            }

            // Interact

            if (_interactTask == null)
            {
                _interactTask = new InteractionCoroutine(ActorId, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1), 5);
            }

            if (!_interactTask.IsDone)
            {
                if (!await _interactTask.GetCoroutine())
                {
                    State = "Interaction." + _interactTask.State;
                    return true;
                }
            }

            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _moveToPositionTask?.Reset();
            _moveToActorTask?.Reset();
            _interactTask?.Reset();
            _isDone = false;
        }

    }
}

