//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.Linq; using Trinity.Framework;
//using QuestTools.Helpers;
//using QuestTools.Navigation;
//using QuestTools.ProfileTags.Complex;
//using Zeta.Bot;
//using Zeta.Bot.Coroutines;
//using Zeta.Bot.Navigation;
//using Zeta.Bot.Profile;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.Actors.Gizmos;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace QuestTools.ProfileTags.Movement
//{
//    [XmlElement("MoveToMapMarker")]
//    public class MoveToMapMarkerTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public MoveToMapMarkerTag() { }
//        private bool _isDone;
//        /// <summary>
//        /// Setting this to true will cause the Tree Walker to continue to the next profile tag
//        /// </summary>
//        public override bool IsDone
//        {
//            get
//            {
//                return !IsActiveQuestStep || _isDone;
//            }
//        }

//        /// <summary>
//        /// Profile Attribute to Will interact with Actor <see cref="InteractAttempts"/> times - optionally set to -1 for no interaction
//        /// </summary>
//        [XmlAttribute("interactAttempts")]
//        public int InteractAttempts { get; set; }

//        /// <summary>
//        /// Profile Attribute to The exitNameHash or hash code of the map marker you wish to find, move to, and interact with
//        /// </summary>
//        [XmlAttribute("exitNameHash")]
//        [XmlAttribute("mapMarkerNameHash")]
//        [XmlAttribute("markerNameHash")]
//        [XmlAttribute("portalNameHash")]
//        public int MapMarkerNameHash { get; set; }

//        /// <summary>
//        /// Profile Attribute to set a minimum search range for your map marker or Actor near a MiniMapMarker (if it exists) or if MaxSearchDistance is not set
//        /// </summary>
//        [XmlAttribute("pathPrecision")]
//        public float PathPrecision { get; set; }

//        [XmlAttribute("straightLinePathing")]
//        public bool StraightLinePathing { get; set; }

//        /// <summary>
//        /// Profile Attribute to set a minimum interact range for your map marker or Actor
//        /// </summary>
//        [XmlAttribute("interactRange")]
//        public float InteractRange { get; set; }

//        /// <summary>
//        /// Profile Attribute to Optionally set this to true if you're using a portal. Requires use of destinationWorldId. <seealso cref="MoveToMapMarkerTag.DestinationWorldId"/>
//        /// </summary>
//        [XmlAttribute("isPortal")]
//        public bool IsPortal { get; set; }

//        /// <summary>
//        /// Profile Attribute to Optionally set this to identify an Actor for this behavior to find, moveto, and interact with
//        /// </summary>
//        [XmlAttribute("actorId")]
//        public int ActorId { get; set; }

//        /// <summary>
//        /// Set this to the destination world ID you're moving to to end this behavior
//        /// </summary>
//        [XmlAttribute("destinationWorldId")]
//        public int DestinationWorldId { get; set; }
//        /// <summary>
//        /// Profile Attribute that is used for very distance Position coordinates; where Demonbuddy cannot make a client-side pathing request 
//        /// and has to contact the server. A value too large (usually over 300 or so) can cause pathing requests to fail or never return in un-meshed locations.
//        /// </summary>
//        [XmlAttribute("pathPointLimit")]
//        public int PathPointLimit { get; set; }

//        [XmlAttribute("x")]
//        public float X { get; set; }

//        [XmlAttribute("y")]
//        public float Y { get; set; }

//        [XmlAttribute("z")]
//        public float Z { get; set; }

//        [XmlAttribute("maxSearchDistance")]
//        public float MaxSearchDistance { get; set; }

//        /// <summary>
//        /// This is the longest time this behavior can run for. Default is 600 seconds (10 minutes).
//        /// </summary>
//        [XmlAttribute("timeoutSeconds")]
//        public int TimeoutSeconds { get; set; }

//        private Vector3 _position;
//        /// <summary>
//        /// This is the calculated position from X,Y,Z
//        /// </summary>
//        public Vector3 Position
//        {
//            get
//            {
//                if (_position == Vector3.Zero)
//                    _position = new Vector3(X, Y, Z);
//                return _position;
//            }
//        }

//        private bool _clientNavFailed;

//        private const int TimeoutSecondsDefault = 600;
//        private const int MaxStuckCountSeconds = 30;
//        private const int MaxStuckRange = 15;
//        private int _completedInteractAttempts;
//        private int _currentStuckCount;
//        private int _startWorldId = -1;

//        private Vector3 _lastPosition = Vector3.Zero;
//        private DateTime _behaviorStartTime = DateTime.MinValue;
//        private DateTime _stuckStart = DateTime.MinValue;
//        private DateTime _lastCheckedStuck = DateTime.MinValue;

//        private MinimapMarker _miniMapMarker;
//        private DiaObject _interactObject;

//        private MoveResult _lastMoveResult = MoveResult.Moved;

//        /// <summary>
//        /// The last seen position of the minimap marker, as it can disappear if you stand on it
//        /// </summary>
//        private Vector3 _mapMarkerLastPosition;

//        public override void OnStart()
//        {
//            // set defaults
//            if (Math.Abs(PathPrecision) < 1f)
//                PathPrecision = 20;
//            if (PathPointLimit == 0)
//                PathPointLimit = 250;
//            if (Math.Abs(InteractRange) < 1f)
//                InteractRange = 10;
//            if (InteractAttempts == 0)
//                InteractAttempts = 5;
//            if (TimeoutSeconds == 0)
//                TimeoutSeconds = TimeoutSecondsDefault;
//            if (MaxSearchDistance <= 0)
//                MaxSearchDistance = 10;

//            _behaviorStartTime = DateTime.UtcNow;
//            _lastPosition = Vector3.Zero;
//            _stuckStart = DateTime.UtcNow;
//            _lastCheckedStuck = DateTime.UtcNow;
//            _lastMoveResult = MoveResult.Moved;
//            _completedInteractAttempts = 0;
//            _startWorldId = ZetaDia.Globals.WorldSnoId;
//            _miniMapMarker = null;
//            _mapMarkerLastPosition = Vector3.Zero;
//            _interactObject = null;

//            Navigator.Clear();
//            Core.Logger.Debug("Initialized {0}", Status());
//        }

//        /// <summary>
//        /// Main MoveToMapMarker Behavior
//        /// </summary>
//        /// <returns></returns>
//        protected override Composite CreateBehavior()
//        {
//            return
//            new Sequence(
//                new DecoratorContinue(ret => ZetaDia.Me.IsDead || ZetaDia.Globals.IsLoadingWorld,
//                    new Sequence(
//                      new Action(ret => Core.Logger.Log("IsDead={0} IsLoadingWorld={1}", ZetaDia.Me.IsDead, ZetaDia.Globals.IsLoadingWorld)),
//                      new Action(ret => RunStatus.Failure)
//                   )
//                ),
//                CheckTimeout(),
//                new Action(ret => FindMiniMapMarker()),
//                new DecoratorContinue(ret => _miniMapMarker != null,
//                    new Action(ret => RefreshActorInfo())
//                ),
//                //new DecoratorContinue(ret => _interactObject == null && _miniMapMarker == null && Position == Vector3.Zero,
//                //    new Sequence(
//                //        new Action(ret => _miniMapMarker = GetRiftExitMarker())
//                //    )
//                //),
//                new DecoratorContinue(ret => _interactObject == null && _miniMapMarker == null && Position == Vector3.Zero,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Debug("Error: Could not find MiniMapMarker nor PortalObject nor Position {0}", Status())),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new DecoratorContinue(ret => _lastMoveResult == MoveResult.ReachedDestination && _interactObject == null,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("ReachedDestination, no object found - finished!")),
//                        new Action(ret => _isDone = true),
//                        new Action(ret => RunStatus.Failure)
//                    )
//                ),
//                new Sequence(
//                    new Action(ret => GameUI.SafeClickUIButtons()),
//                    new PrioritySelector(
//                        new Decorator(ret => GameUI.IsPartyDialogVisible,
//                            new Action(ret => Core.Logger.Log("Party Dialog is visible"))
//                        ),
//                        new Decorator(ret => GameUI.IsElementVisible(GameUI.GenericOK),
//                            new Action(ret => Core.Logger.Log("Generic OK is visible"))
//                        ),
//                        new Decorator(ret => DestinationWorldId == -1 && ZetaDia.Globals.WorldSnoId != _startWorldId,
//                            new Sequence(
//                                new Action(ret => Core.Logger.Log("World changed ({0} to {1}), destinationWorlId={2}, finished {3}",
//                                    _startWorldId, ZetaDia.Globals.WorldSnoId, DestinationWorldId, Status())),
//                                new Action(ret => _isDone = true)
//                            )
//                        ),
//                        new Decorator(ret => DestinationWorldId != 0 && ZetaDia.Globals.WorldSnoId == DestinationWorldId,
//                            new Sequence(
//                                new Action(ret => Core.Logger.Log("DestinationWorlId matched, finished {0}", Status())),
//                                new Action(ret => _isDone = true)
//                            )
//                        ),
//                        new Decorator(ret => _completedInteractAttempts > 1 && _lastPosition.Distance(ZetaDia.Me.Position) > 4f && DestinationWorldId != _startWorldId,
//                            new Sequence(
//                                new Action(ret => _isDone = true),
//                                new Action(ret => Core.Logger.Log("Moved {0:0} yards after interaction, finished {1}", _lastPosition.Distance(ZetaDia.Me.Position), Status()))
//                            )
//                        ),
//                        new Decorator(ret => _interactObject != null && _interactObject.IsValid,
//                            new PrioritySelector(
//                                new Decorator(ret => _lastMoveResult != MoveResult.ReachedDestination,
//                                    new Sequence(
//                                        new Action(ret => Core.Logger.Debug("Moving to actor {0} {1}",
//                                            _interactObject.ActorSnoId, Status())),
//                                        new ActionRunCoroutine(async ret => _lastMoveResult = await Navigator.MoveTo(_interactObject.Position))
//                                   )
//                                ),
//                                new Decorator(ret => _lastMoveResult == MoveResult.ReachedDestination && _interactObject.Distance > InteractRange,
//                                    new Sequence(
//                                        new Action(ret => Core.Logger.Log("ReachedDestination but not within InteractRange, finished")),
//                                        new Action(ret => _isDone = true)
//                                    )
//                                ),
//                                new Decorator(ret => ZetaDia.Me.Movement.IsMoving,
//                                    new Action(ret => CommonBehaviors.MoveStop())),
//                                new Sequence(
//                                    new Action(ret => _lastPosition = ZetaDia.Me.Position),
//                                    new Action(ret => _interactObject.Interact()),
//                                    new Action(ret => _completedInteractAttempts++),
//                                    new Action(ret => Core.Logger.Debug("Interacting with portal object {0}, result: {1}", _interactObject.ActorSnoId, Status())),
//                                    new Sleep(500)
//                                )
//                            )
//                        ),
//                        new Decorator(ret => _miniMapMarker != null && _interactObject == null,
//                            new PrioritySelector(
//                                new Decorator(ret => _miniMapMarker != null && _miniMapMarker.Position.Distance(ZetaDia.Me.Position) > PathPrecision,
//                                    new Sequence(
//                                        new Action(ret => Core.Logger.Debug("Moving to Map Marker {0}, {1}", _miniMapMarker.NameHash, Status())),
//                                        new ActionRunCoroutine(async ret => _lastMoveResult = await Navigator.MoveTo(_miniMapMarker.Position))
//                                    )
//                                ),
//                                new Decorator(ret => _miniMapMarker != null && _miniMapMarker.Position.Distance(ZetaDia.Me.Position) < PathPrecision,
//                                    new Sequence(
//                                        new Action(ret => Core.Logger.Debug("Successfully Moved to Map Marker {0}, {1}", _miniMapMarker.NameHash, Status())),
//                                        new Action(ret => _isDone = true)
//                                    )
//                                )
//                            )
//                        ),
//                        new Decorator(ret => _miniMapMarker == null && Position != Vector3.Zero,
//                            new Sequence(
//                                new ActionRunCoroutine(async ret => _lastMoveResult = await Navigator.MoveTo(Position)),
//                                new DecoratorContinue(ret => _lastMoveResult == MoveResult.ReachedDestination,
//                                    new Sequence(
//                                        new Action(ret => Core.Logger.Log("ReachedDestination of Position, finished.")),
//                                        new Action(ret => _isDone = true)
//                                    )
//                                )
//                            )
//                        ),
//                        new Action(ret =>
//                            Core.Logger.Error("MoveToMapMarker Error: marker={0} actor={1}/{2} completedInteracts={3} isPortal={4} dist={5} interactRange={6}",
//                            _miniMapMarker != null,
//                            _interactObject != null,
//                            (_interactObject != null && _interactObject.IsValid),
//                            _completedInteractAttempts,
//                            IsPortal,
//                            (_interactObject != null ? _interactObject.Position.Distance(ZetaDia.Me.Position) : 0f),
//                            InteractRange))
//                    )
//                )
//            );
//        }

//        private static MinimapMarker GetRiftExitMarker()
//        {
//            int index = DataDictionary.RiftWorldIds.IndexOf(ZetaDia.Globals.WorldSnoId);
//            if (index == -1)
//                return null;

//            return ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                .OrderBy(m => m.Position.Distance2D(ZetaDia.Me.Position))
//                .FirstOrDefault(m => m.NameHash == DataDictionary.RiftPortalHashes[index]);
//        }

//        private void FindMiniMapMarker()
//        {
//            // Special condition for Rift portals
//            if (DataDictionary.RiftWorldIds.Contains(ZetaDia.Globals.WorldSnoId) && Position == Vector3.Zero && ActorId == 0 && IsPortal && DestinationWorldId == -1)
//            {
//                _miniMapMarker = GetRiftExitMarker();
//                if (_miniMapMarker != null)
//                {
//                    MapMarkerNameHash = _miniMapMarker.NameHash;
//                    Core.Logger.Debug("Using Rift Style Minimap Marker: {0} dist: {1:0} isExit: {2}",
//                        _miniMapMarker.NameHash,
//                        _miniMapMarker.Position.Distance2D(ZetaDia.Me.Position),
//                        _miniMapMarker.IsPortalExit);
//                }
//            }

//            // find our map marker
//            if (_miniMapMarker == null)
//            {
//                if (Position != Vector3.Zero)
//                {
//                    _miniMapMarker = ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                        .Where(marker => marker != null && marker.NameHash == MapMarkerNameHash &&
//                            Position.Distance(marker.Position) < MaxSearchDistance)
//                        .OrderBy(o => o.Position.Distance(ZetaDia.Me.Position)).FirstOrDefault();
//                }
//                else
//                {
//                    _miniMapMarker = ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                        .Where(marker => marker != null && marker.NameHash == MapMarkerNameHash)
//                        .OrderBy(o => o.Position.Distance(ZetaDia.Me.Position)).FirstOrDefault();
//                }
//            }
//            if (_miniMapMarker != null && _miniMapMarker.Position != Vector3.Zero)
//            {
//                _mapMarkerLastPosition = _miniMapMarker.Position;
//            }

//        }
//        private PrioritySelector CheckStuck()
//        {
//            return new PrioritySelector(
//                new Decorator(ret => _currentStuckCount > 0 && DateTime.UtcNow.Subtract(_stuckStart).TotalSeconds > MaxStuckCountSeconds,
//                    new Action(delegate
//                    {
//                        Core.Logger.Debug("Looks like we're stuck since it's been {0} seconds stuck... finishing", DateTime.UtcNow.Subtract(_stuckStart).TotalSeconds);
//                        _isDone = true;
//                        return RunStatus.Success;
//                    })
//                ),
//                new Decorator(ret => DateTime.UtcNow.Subtract(_lastCheckedStuck).TotalMilliseconds < 500,
//                    new Action(delegate
//                        {
//                            return RunStatus.Success;
//                        }
//                    )
//                ),
//                new Decorator(ret => ZetaDia.Me.Position.Distance(_lastPosition) < MaxStuckRange,
//                    new Action(delegate
//                    {
//                        _currentStuckCount++;
//                        _lastCheckedStuck = DateTime.UtcNow;
//                        _lastPosition = ZetaDia.Me.Position;
//                        if (_currentStuckCount > DateTime.UtcNow.Subtract(_stuckStart).TotalSeconds * .5)
//                            _clientNavFailed = true;

//                        if (QuestTools.IsDebugLoggingEnabled)
//                        {
//                            Core.Logger.Debug("Stuck count: {0}", _currentStuckCount);
//                        }
//                        return RunStatus.Success;
//                    })
//                ),
//                new Decorator(ret => ZetaDia.Me.Position.Distance(_lastPosition) > MaxStuckRange,
//                    new Action(delegate
//                    {
//                        _currentStuckCount = 0;
//                        _lastCheckedStuck = DateTime.UtcNow;
//                        _lastPosition = ZetaDia.Me.Position;

//                        return RunStatus.Success;
//                    })
//                ),
//                new Action(delegate
//                    {
//                        _lastPosition = ZetaDia.Me.Position;
//                        return RunStatus.Success;
//                    }
//                )
//            );
//        }
//        private DecoratorContinue CheckTimeout()
//        {
//            return
//            new DecoratorContinue(ret => Math.Abs(DateTime.UtcNow.Subtract(_behaviorStartTime).TotalSeconds) > TimeoutSeconds,
//                new Sequence(
//                    new Action(ret => _isDone = true),
//                    new Action(ret => Core.Logger.Log("Timeout of {0} seconds exceeded in current behavior", TimeoutSeconds)),
//                    new Action(ret => RunStatus.Failure)
//                )
//            );
//        }

//        private void RefreshActorInfo()
//        {
//            Vector3 myPos = ZetaDia.Me.Position;

//            if (_interactObject != null && !_interactObject.IsValid)
//                _interactObject = null;

//            if ((_interactObject == null || (_interactObject != null && !_interactObject.IsValid)) && ActorId != 0)
//            {
//                _interactObject = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                    .Where(o => o.IsValid && o.ActorSnoId == ActorId && ActorWithinRangeOfMarker(o))
//                    .OrderBy(DistanceToMapMarker)
//                    .FirstOrDefault();
//            }
//            if (_interactObject != null && _interactObject.IsValid)
//            {
//                if (QuestTools.IsDebugLoggingEnabled)
//                {
//                    Core.Logger.Debug("Found actor {0} {1} {2} of distance {3} from point {4}",
//                                        _interactObject.ActorSnoId, _interactObject.Name, _interactObject.ActorType, _interactObject.Position.Distance(_mapMarkerLastPosition), _mapMarkerLastPosition);
//                }
//            }
//            else if (ActorId != 0 && Position != Vector3.Zero && _position.Distance(ZetaDia.Me.Position) <= PathPrecision)
//            {
//                // ActorId defined, use object closest to player
//                _interactObject = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where(o => o.IsValid && o.ActorSnoId == ActorId
//                   && o.Position.Distance2D(Position) <= PathPrecision).OrderBy(o => o.Distance).FirstOrDefault();
//            }
//            else if (ActorId != 0 && _mapMarkerLastPosition != Vector3.Zero && _mapMarkerLastPosition.Distance(myPos) <= PathPrecision)
//            {
//                // No ActorID defined, using Marker position to find actor closest to player
//                _interactObject = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where(o => o != null && o.IsValid && o.ActorSnoId == ActorId
//                   && o.Position.Distance2D(_mapMarkerLastPosition) <= PathPrecision).OrderBy(o => o.Distance).FirstOrDefault();
//            }
//            else if (ActorId == 0 && _mapMarkerLastPosition != Vector3.Zero && _mapMarkerLastPosition.Distance2D(myPos) <= 90)
//            {
//                // No ActorId defined, but we've found the marker, find object closest to marker position
//                _interactObject = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                    .Where(a => a.Position.Distance2D(_mapMarkerLastPosition) <= MaxSearchDistance)
//                    .OrderBy(a => a.Position.Distance2D(_mapMarkerLastPosition)).FirstOrDefault();

//                if (_interactObject != null)
//                {
//                    InteractRange = Math.Max(InteractRange, Math.Max(_interactObject.CollisionSphere.Radius, 10f));
//                    Core.Logger.Debug("Found Actor from Map Marker! mapMarkerPos={0} actor={1} {2} {3} {4}",
//                        _mapMarkerLastPosition, _interactObject.ActorSnoId, _interactObject.Name, _interactObject.ActorType, _interactObject.Position);
//                }
//            }


//            if (_interactObject == null && _mapMarkerLastPosition.Distance(ZetaDia.Me.Position) < PathPrecision)
//            {
//                Core.Logger.Debug("Could not find an actor {0} within range {1} from point {2}",
//                                   ActorId, PathPrecision, _mapMarkerLastPosition);
//            }

//            if (_interactObject is GizmoPortal && !IsPortal)
//            {
//                IsPortal = true;
//            }
//        }

//        private float DistanceToMapMarker(DiaObject o)
//        {
//            return o.Position.Distance(_miniMapMarker.Position);
//        }

//        private bool ActorWithinRangeOfMarker(DiaObject o)
//        {
//            bool test = false;

//            if (o != null && _miniMapMarker != null)
//            {
//                test = o.Position.Distance2D(_miniMapMarker.Position) <= PathPrecision;
//            }
//            return test;
//        }

//        /// <summary>
//        /// Move without a destination name
//        /// </summary>
//        /// <param name="newpos"></param>
//        /// <returns></returns>
//        private bool Move(Vector3 newpos)
//        {
//            return Move(newpos, null);
//        }

//        List<Vector3> _allPoints = new List<Vector3>();
//        List<Vector3> _validPoints = new List<Vector3>();
//        private readonly QTNavigator _qtNavigator = new QTNavigator();

//        /// <summary>
//        /// Safely Moves the player to the requested destination <seealso cref="MoveToMapMarkerTag.PathPointLimit"/>
//        /// </summary>
//        /// <param name="newpos">Vector3 of the new position</param>
//        /// <param name="destinationName">For logging purposes</param>
//        /// <returns></returns>
//        private bool Move(Vector3 newpos, string destinationName = "")
//        {
//            bool result = false;

//            if (StraightLinePathing)
//            {
//                Navigator.PlayerMover.MoveTowards(newpos);
//                _lastMoveResult = MoveResult.Moved;
//                result = true;
//            }

//            if (!ZetaDia.WorldInfo.IsGenerated)
//            {
//                if (_clientNavFailed && PathPointLimit > 20)
//                {
//                    PathPointLimit = PathPointLimit - 10;
//                }
//                else if (_clientNavFailed && PathPointLimit <= 20)
//                {
//                    PathPointLimit = 250;
//                }

//                if (newpos.Distance(ZetaDia.Me.Position) > PathPointLimit)
//                {
//                    newpos = MathEx.CalculatePointFrom(ZetaDia.Me.Position, newpos, newpos.Distance(ZetaDia.Me.Position) - PathPointLimit);
//                }
//            }
//            float destinationDistance = newpos.Distance(ZetaDia.Me.Position);

//            _lastMoveResult = _qtNavigator.MoveTo(newpos, destinationName);

//            switch (_lastMoveResult)
//            {
//                case MoveResult.Moved:
//                case MoveResult.ReachedDestination:
//                case MoveResult.UnstuckAttempt:
//                    _clientNavFailed = false;
//                    result = true;
//                    break;
//                case MoveResult.PathGenerated:
//                case MoveResult.PathGenerating:
//                case MoveResult.PathGenerationFailed:
//                case MoveResult.Failed:
//                    Navigator.PlayerMover.MoveTowards(Position);
//                    result = false;
//                    _clientNavFailed = true;
//                    break;
//            }

//            if (QuestTools.IsDebugLoggingEnabled)
//            {
//                Core.Logger.Debug("MoveResult: {0}, newpos={1} destinationName={2}",
//                    _lastMoveResult.ToString(), newpos, destinationName);
//            }
//            return result;
//        }

//        public bool isValid()
//        {
//            try
//            {
//                if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld)
//                    return false;

//                // check if everything we need here is safe to use
//                if (ZetaDia.Me != null && ZetaDia.Me.IsValid &&
//                    ZetaDia.Me.CommonData != null && ZetaDia.Me.CommonData.IsValid)
//                    return true;
//            }
//            catch
//            {
//            }
//            return false;
//        }

//        public String Status()
//        {
//            string extraInfo = "";
//            if (DataDictionary.RiftWorldIds.Contains(ZetaDia.Globals.WorldSnoId))
//                extraInfo += "IsRift ";

//            if (QuestToolsSettings.Instance.DebugEnabled)
//                return String.Format("questId={0} stepId={1} actorId={2} exitNameHash={3} isPortal={4} destinationWorldId={5} " +
//                    "pathPointLimit={6} interactAttempts={7} interactRange={8} pathPrecision={9} x=\"{10}\" y=\"{11}\" z=\"{12}\" " + extraInfo,
//                    QuestId, StepId, ActorId, MapMarkerNameHash, IsPortal, DestinationWorldId,
//                    PathPointLimit, InteractAttempts, InteractRange, PathPrecision, X, Y, Z
//                    );

//            return string.Empty;
//        }

//        public override void ResetCachedDone()
//        {
//            _isDone = false;

//            _behaviorStartTime = DateTime.UtcNow;
//            _lastPosition = Vector3.Zero;
//            _stuckStart = DateTime.UtcNow;
//            _lastCheckedStuck = DateTime.UtcNow;
//            _lastMoveResult = MoveResult.Moved;
//            _completedInteractAttempts = 0;
//            _startWorldId = ZetaDia.Globals.WorldSnoId;
//            _miniMapMarker = null;
//            _mapMarkerLastPosition = Vector3.Zero;
//            _interactObject = null;

//            base.ResetCachedDone();
//        }

//        #region IEnhancedProfileBehavior

//        public void Update()
//        {
//            UpdateBehavior();
//        }

//        public void Start()
//        {
//            OnStart();
//        }

//        public void Done()
//        {
//            _isDone = true;
//        }

//        #endregion
//    }
//}
