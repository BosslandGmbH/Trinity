using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.ProfileTags
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

        [XmlAttribute("explore")]
        [DefaultValue(false)]
        public bool Explore { get; set; }

        [XmlAttribute("maxRange")]
        [DefaultValue(80f)]
        public float MaxRange { get; set; }

        [XmlAttribute("interactAttempts")]
        [DefaultValue(3)]
        public int InteractAttempts { get; set; }

        #region ScenePosition

        [XmlAttribute("sceneX")]
        [DefaultValue(0)]
        public float RelativeSceneX { get; set; }

        [XmlAttribute("sceneY")]
        [DefaultValue(0)]
        public float RelativeSceneY { get; set; }

        [XmlAttribute("sceneZ")]
        [DefaultValue(0)]
        public float RelativeSceneZ { get; set; }

        [XmlAttribute("sceneSnoId")]
        [DefaultValue(0)]
        public int SceneSnoId { get; set; }

        [XmlAttribute("sceneName")]
        [DefaultValue("")]
        public string SceneName { get; set; }

        #endregion

        public string State { get; set; } = "Not Started";

        public Vector3 Position => new Vector3(X, Y, Z);

        private bool _isDone;
        public override bool IsDone => _isDone;

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }

        private MoveToActorCoroutine _moveToActorTask;
        private InteractionCoroutine _interactTask;
        private ISubroutine _movementTask;

        public async Task<bool> Routine()
        {
            if (WorldSnoId != 0 && WorldSnoId != ZetaDia.CurrentWorldSnoId)
            {
                Logger.Log($"In the wrong world! Current={ZetaDia.CurrentWorldSnoId} Expected={WorldSnoId}");
                _isDone = true;
                return true;
            }

            //// Move directly to a position if specified

            //if (Position != Vector3.Zero)
            //{
            //    if (_moveToPositionTask == null)
            //    {
            //        _moveToPositionTask = new MoveToPositionCoroutine(AdvDia.CurrentWorldId, new Vector3(X, Y, Z), 3);
            //    }

            //    if (!_moveToPositionTask.IsDone)
            //    {
            //        if (!await _moveToPositionTask.GetCoroutine())
            //        {
            //            State = "MoveToPosition." + _moveToPositionTask.State;
            //            return true;
            //        }
            //    }
            //}

            if (_movementTask == null)
            {
                var targetAbsolutePosition = new Vector3(X, Y, Z);
                var targetRelativePosition = new Vector3(RelativeSceneX, RelativeSceneY, RelativeSceneZ);

                var isPositionProvided = targetAbsolutePosition != Vector3.Zero || targetRelativePosition != Vector3.Zero;
                if (isPositionProvided)
                {
                    if (ZetaDia.WorldInfo.IsGenerated && targetAbsolutePosition != Vector3.Zero && targetRelativePosition == Vector3.Zero)
                    {
                        Logger.LogError("[MoveToPosition] The current world is auto-generatd, you need to use sceneX,sceneY,sceneZ + either 'sceneName' or 'sceneSnoId' attributes");
                        _isDone = true;
                        return true;
                    }

                    var canUseAbsolutePositioning = !ZetaDia.WorldInfo.IsGenerated && targetAbsolutePosition != Vector3.Zero;
                    if (canUseAbsolutePositioning)
                    {
                        _movementTask = new MoveToPositionCoroutine(AdvDia.CurrentWorldId, targetAbsolutePosition, 3);
                    }
                    else if (targetRelativePosition != Vector3.Zero)
                    {
                        if (SceneSnoId > 0)
                        {
                            _movementTask = new MoveToScenePositionCoroutine(SceneSnoId, targetRelativePosition);
                        }
                        else if (!string.IsNullOrEmpty(SceneName))
                        {
                            _movementTask = new MoveToScenePositionCoroutine(SceneName, targetRelativePosition);
                        }
                        else
                        {
                            Logger.LogError("[MoveToPosition] A sceneName or sceneSnoId, and a relative position (sceneX,sceneY,sceneZ) are required for dynamically generated worlds");
                        }
                    }
                    else
                    {
                        Logger.LogError("[MoveToPosition] No valid coodinates were specified");
                    }
                }
            }

            if (_movementTask != null && !await _movementTask.GetCoroutine())
                return true;

            // Find the actor without a position specified

            if (Position == Vector3.Zero)
            {
                var actor = ActorFinder.FindObject(ActorId);
                if (actor != null && actor.Distance <= MaxRange || Explore)
                {
                    if (_moveToActorTask == null)
                    {
                        if (actor != null)
                        {
                            Logger.Log($"Found actor '{actor?.Name}' (Distance={actor?.Distance}) for MoveToActorCoroutine, now going there.");
                        }
                        else
                        {
                            Logger.Log($"Can't find actor nearby, exploring...");
                        }

                        _moveToActorTask = new MoveToActorCoroutine(QuestId, AdvDia.CurrentWorldId, ActorId, (int)MaxRange);
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
            }

            // Interact

            if (_interactTask == null)
            {
                _interactTask = new InteractionCoroutine(ActorId, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1), InteractAttempts);
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
            _moveToActorTask?.Reset();
            _interactTask?.Reset();
            _movementTask?.Reset();
            _isDone = false;
        }

    }
}

