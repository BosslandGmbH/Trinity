//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.Linq; using Trinity.Framework;
//using System.Threading.Tasks;
//using Buddy.Coroutines;
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
//    /// <summary>
//    /// Class MoveToObjectiveTag.
//    /// </summary>
//    [XmlElement("MoveToObjective")]
//    public class MoveToObjectiveTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="MoveToObjectiveTag"/> class.
//        /// </summary>
//        public MoveToObjectiveTag() { }
//        /// <summary>
//        /// The _is done
//        /// </summary>
//        private bool _isDone;
//        /// <summary>
//        /// Setting this to true will cause the Tree Walker to continue to the next profile tag
//        /// </summary>
//        /// <value><c>true</c> if this instance is done; otherwise, <c>false</c>.</value>
//        public override bool IsDone { get { return !IsActiveQuestStep || _isDone; } }

//        /// <summary>
//        /// Profile Attribute to Will interact with Actor <see cref="InteractAttempts" /> times - optionally set to -1 for no interaction
//        /// </summary>
//        /// <value>The interact attempts.</value>
//        [XmlAttribute("interactAttempts")]
//        public int InteractAttempts { get; set; }

//        /// <summary>
//        /// Profile Attribute to set how many times the objective marker can be unreachable before being blacklisted.
//        /// </summary>
//        /// <value>The failed attempts maximum.</value>
//        [XmlAttribute("failedAttemptsMax")]
//        public int FailedAttemptsMax { get; set; }

//        /// <summary>
//        /// Profile Attribute to set a minimum search range for your map marker or Actor near a MiniMapMarker (if it exists) or if MaxSearchDistance is not set
//        /// </summary>
//        /// <value>The path precision.</value>
//        [XmlAttribute("pathPrecision")]
//        public float PathPrecision { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether [straight line pathing].
//        /// </summary>
//        /// <value><c>true</c> if [straight line pathing]; otherwise, <c>false</c>.</value>
//        [XmlAttribute("straightLinePathing")]
//        public bool StraightLinePathing { get; set; }

//        /// <summary>
//        /// Profile Attribute to set a minimum interact range for your map marker or Actor
//        /// </summary>
//        /// <value>The interact range.</value>
//        [XmlAttribute("interactRange")]
//        public float InteractRange { get; set; }

//        /// <summary>
//        /// Profile Attribute that is used for very distance Position coordinates; where Demonbuddy cannot make a client-side pathing request
//        /// and has to contact the server. A value too large (usually over 300 or so) can cause pathing requests to fail or never return in un-meshed locations.
//        /// </summary>
//        /// <value>The path point limit.</value>
//        [XmlAttribute("pathPointLimit")]
//        public int PathPointLimit { get; set; }

//        /// <summary>
//        /// Gets or sets the x.
//        /// </summary>
//        /// <value>The x.</value>
//        [XmlAttribute("x")]
//        public float X { get; set; }

//        /// <summary>
//        /// Gets or sets the y.
//        /// </summary>
//        /// <value>The y.</value>
//        [XmlAttribute("y")]
//        public float Y { get; set; }

//        /// <summary>
//        /// Gets or sets the z.
//        /// </summary>
//        /// <value>The z.</value>
//        [XmlAttribute("z")]
//        public float Z { get; set; }

//        /// <summary>
//        /// The _position
//        /// </summary>
//        private Vector3 _position;
//        /// <summary>
//        /// This is the calculated position from X,Y,Z
//        /// </summary>
//        /// <value>The position.</value>
//        public Vector3 Position
//        {
//            get
//            {
//                if (_position == Vector3.Zero)
//                    _position = new Vector3(X, Y, Z);
//                return _position;
//            }
//        }

//        /// <summary>
//        /// The _client nav failed
//        /// </summary>
//        private bool _clientNavFailed;

//        /// <summary>
//        /// The _completed interact attempts
//        /// </summary>
//        private int _completedInteractAttempts;
//        /// <summary>
//        /// The _start world identifier
//        /// </summary>
//        private int _startWorldId = -1;

//        /// <summary>
//        /// The _mini map marker
//        /// </summary>
//        private MinimapMarker _miniMapMarker;
//        /// <summary>
//        /// The _objective object
//        /// </summary>
//        private DiaObject _objectiveObject;

//        /// <summary>
//        /// The _last move result
//        /// </summary>
//        private MoveResult _lastMoveResult = MoveResult.Moved;

//        /// <summary>
//        /// The last seen position of the minimap marker, as it can disappear if you stand on it
//        /// </summary>
//        private Vector3 _mapMarkerLastPosition;

//        /// <summary>
//        /// Called when [start].
//        /// </summary>
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
//            if (FailedAttemptMax <= 0)
//                FailedAttemptMax = 1;

//            _lastMoveResult = MoveResult.Moved;
//            _completedInteractAttempts = 0;
//            _startWorldId = ZetaDia.Globals.WorldSnoId;

//            Navigator.Clear();
//            Core.Logger.Debug("Initialized {0}", Status());
//        }

//        /// <summary>
//        /// Gets the route to objective.
//        /// </summary>
//        /// <returns>List&lt;Vector3&gt;.</returns>
//        public List<Vector3> CreateRouteToObjective()
//        {
//            var route = new List<Vector3>();

//            FindObjectiveMarker();

//            if (_mapMarkerLastPosition == Vector3.Zero)
//                return route;

//            var currentScene = SceneSegmentation.CurrentScene;
//            if (!currentScene.IsExplored)
//            {
//                Core.Logger.Log("Exploring Current Scene {0}", currentScene.Name);
//                _isExploring = true;
//                return SceneSegmentation.GetSceneExploreRoute(currentScene);
//            }

//            var objectiveDirection = MathUtil.GetDirectionToPoint(_mapMarkerLastPosition);
//            var nextScene = SceneSegmentation.GetConnectedScene(objectiveDirection) ?? SceneSegmentation.GetConnectedScene();
//            if (nextScene == null)
//            {
//                Core.Logger.Warn("Unable to find connected scene");
//                _isDone = true;
//                return route;
//            }

//            Core.Logger.Log("Plotting Route through {0} ObjectiveDirection={1} SceneDirection={2}", nextScene.Name, objectiveDirection, MathUtil.GetDirectionToPoint(nextScene.Center));
//            route = SceneSegmentation.GetVectorPathToScene(nextScene);

//            return route;
//        }

//        /// <summary>
//        /// Main MoveToObjective Behavior
//        /// </summary>
//        /// <returns>Composite.</returns>
//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ret => MoveToObjectiveRoutine());
//        }

//        protected async Task<bool> MoveToObjectiveRoutine()
//        {
//            if (ZetaDia.Me.IsDead || ZetaDia.Globals.IsLoadingWorld)
//            {
//                Core.Logger.Log("IsDead={0} IsLoadingWorld={1}", ZetaDia.Me.IsDead, ZetaDia.Globals.IsLoadingWorld);
//                return false;
//            }

//            SceneSegmentation.Update();

//            if (_route.Any() || _currentRouteDestination != Vector3.Zero)
//            {
//                return MoveAlongRoute(_route);
//            }

//            // If the bot has failed to run directly towards the objective marker too many times
//            // We'll blacklist it for 30 seconds to allow adjacent scenes to be explored.
//            if (FailedAttempts >= FailedAttemptMax)
//            {
//                Core.Logger.Log("Blacklisting Objective Marker for 60 seconds");
//                BlacklistMarkerExpires = DateTime.UtcNow.AddSeconds(60);
//                FailedAttempts = 0;                
//                _route = CreateRouteToUnexploredScene();
//                return false;
//            }

//            // While the bot is currently blacklisted from directly running at the objective
//            // Generate a route to go through some nearby scenes
//            if (DateTime.UtcNow < BlacklistMarkerExpires)
//            {
//                _route = CreateRouteToObjective();                
//                return false;
//            }

//            // 'ReachedDestination' is returned when finding a path to destination has failed.
//            if (_lastMoveResult == MoveResult.ReachedDestination && _objectiveObject == null)
//            {
//                if (_miniMapMarker != null && _miniMapMarker.IsValid && _miniMapMarker.IsPointOfInterest)
//                {
//                    var distance = ZetaDia.Me.Position.Distance(_miniMapMarker.Position);
//                    if (distance > 100)
//                    {
//                        FailedAttempts++;
//                        Core.Logger.Log("Direct Pathfinding failed towards Objective Marker. Attempts={0}/{1}", FailedAttempts, FailedAttemptMax);
//                    }
//                    else if (distance < 30)
//                    {
//                        Core.Logger.Log("ReachedDestination no Objective found - finished!");
//                        _isDone = true;
//                        return true;
//                    }
//                }
//            }

//            // Find the objective minimap marker
//            FindObjectiveMarker();

//            // If the marker is found or was previously found, find a nearby actor
//            if (_mapMarkerLastPosition != Vector3.Zero)
//                RefreshActorInfo();

//            if (_objectiveObject == null && _miniMapMarker == null && Position == Vector3.Zero)
//            {
//                Core.Logger.Log("Error: Could not find Objective Marker! {0}", Status());
//                _isDone = true;
//                return true;
//            }

//            // Finish if World Changed
//            if (ZetaDia.Globals.WorldSnoId != _startWorldId)
//            {
//                Core.Logger.Log("World changed from {0} to {1}, finished {2}", _startWorldId, ZetaDia.Globals.WorldSnoId, Status());
//                _isDone = true;
//                return true;
//            }

//            // Finish because Objective Found
//            if (IsValidObjective() && _objectiveObject is DiaUnit && _objectiveObject.Position.Distance(ZetaDia.Me.Position) <= PathPrecision)
//            {
//                Core.Logger.Log("We found the objective and its a monster, ending tag so we can kill it. {0}", Status());
//                _isDone = true;
//                return true;
//            }

//            // Objective Object is available
//            if (_objectiveObject != null && _objectiveObject.IsFullyValid())
//            {
//                // Move Closer to Objective Object
//                if (_lastMoveResult != MoveResult.ReachedDestination)
//                {
//                    Core.Logger.Debug("Moving to actor {0} {1}", _objectiveObject.ActorSnoId, Status());
//                    _lastMoveResult = await CommonCoroutines.MoveTo(_objectiveObject.Position);
//                    return true;
//                }

//                if (_objectiveObject is GizmoPortal)
//                {
//                    if (_lastMoveResult == MoveResult.ReachedDestination && _objectiveObject.Distance > InteractRange)
//                    {
//                        Core.Logger.Log("ReachedDestination but not within InteractRange, finished");
//                        _isDone = true;
//                        return true;
//                    }
//                    if (GameUI.PartyLeaderBossAccept.IsVisible || GameUI.PartyFollowerBossAccept.IsVisible)
//                    {
//                        Core.Logger.Debug("Party Boss Button visible");
//                        return true;
//                    }
//                    if (ZetaDia.Me.Movement.IsMoving)
//                    {
//                        await CommonBehaviors.MoveStop().ExecuteCoroutine();
//                    }
//                    _objectiveObject.Interact();
//                    _completedInteractAttempts++;
//                    Core.Logger.Debug("Interacting with portal object {0}, result: {1}", _objectiveObject.ActorSnoId, Status());
//                    await Coroutine.Sleep(500);
//                    //GameEvents.FireWorldTransferStart();
//                    return true;
//                }

//            }

//            if (_miniMapMarker != null && ObjectiveObjectIsNullOrInvalid())
//            {
//                if (_miniMapMarker != null && _miniMapMarker.Position.Distance(ZetaDia.Me.Position) > PathPrecision)
//                {
//                    Core.Logger.Debug("Moving to Objective Marker {0}, {1}", _miniMapMarker.NameHash, Status());
//                    _lastMoveResult = await CommonCoroutines.MoveTo(_miniMapMarker.Position);
//                    return true;
//                }

//                if (_miniMapMarker != null && _miniMapMarker.Position.Distance(ZetaDia.Me.Position) < PathPrecision)
//                {
//                    Core.Logger.Debug("Successfully Moved to Objective Marker {0}, {1}", _miniMapMarker.NameHash, Status());
//                    _isDone = true;
//                    return true;
//                }
//            }
//            if (_miniMapMarker == null && Position != Vector3.Zero && Position.Distance(ZetaDia.Me.Position) > PathPrecision)
//            {
//                _lastMoveResult = CommonCoroutines.MoveTo(Position).Result;
//                if (_lastMoveResult == MoveResult.ReachedDestination)
//                {
//                    Core.Logger.Log("ReachedDestination of Position, minimap marker not found, finished.");
//                    _isDone = true;
//                    return true;
//                }
//            }

//            Core.Logger.Error("MoveToObjective Error: marker={0} actorNull={1} actorValid={2} completedInteracts={3} isPortal={4} dist={5} interactRange={6}",
//                    _miniMapMarker != null,
//                    _objectiveObject != null,
//                    (_objectiveObject != null && _objectiveObject.IsFullyValid()),
//                    _completedInteractAttempts,
//                    IsPortal,
//                    (_objectiveObject != null && _objectiveObject.IsFullyValid() ? _objectiveObject.Position.Distance(ZetaDia.Me.Position) : 0f),
//                    InteractRange);

//            return false;
//        }

//        /// <summary>
//        /// Handles moving to points on _route and dequeueing them when within range.
//        /// </summary>
//        private bool MoveAlongRoute(List<Vector3> route)
//        {
//            const float range = 20f;

//            // Discard nodes we happen to be walking near to
//            var removed = route.RemoveAll(n => n.Distance2D(ZetaDia.Me.Position) < range);
//            if (removed > 0)
//                Core.Logger.Log("Cleared {0} Nodes within {1} yards", removed, range);

//            // If we're exploring the current scene and it becomes fully explored, stop immediately.
//            if (_isExploring && SceneSegmentation.CurrentScene.IsExplored)
//            {
//                Core.Logger.Log("Scene data is now revealed, clearing route");
//                _currentRouteDestination = Vector3.Zero;
//                _isExploring = false;
//                route = CreateRouteToObjective();
//            }

//            // Start moving to the next point on the route.
//            if ((_currentRouteDestination == Vector3.Zero || _currentRouteDestination.Distance(ZetaDia.Me.Position) < range || _lastMoveResult == MoveResult.ReachedDestination || _lastMoveResult == MoveResult.Failed))
//            {
//                if (route.Any())
//                {
//                    Core.Logger.Log("Starting New Node", _lastMoveResult);
//                    var nextNode = route.FirstOrDefault();
//                    _currentRouteDestination = nextNode;
//                    route.Remove(nextNode);
//                }
//                else
//                {
//                    // Route finished - no more points
//                    _currentRouteDestination = Vector3.Zero;
//                }
//            }

//            if (_currentRouteDestination != Vector3.Zero)
//            {
//                Move(_currentRouteDestination, "RouteToObjective");
//                return true;
//            }
//            return false;
//        }

//        private bool ObjectiveObjectIsNullOrInvalid()
//        {
//            return _objectiveObject == null || (_objectiveObject != null && !_objectiveObject.IsFullyValid());
//        }

//        /// <summary>
//        /// Creates a path to the nearest unexplored scene
//        /// </summary>
//        private List<Vector3> CreateRouteToUnexploredScene()
//        {
//            var route = new List<Vector3>();
//            var unexploredScene = SceneSegmentation.GetNearestUnexploredScene();
//            if (unexploredScene != null)
//            {
//                Core.Logger.Warn("Found Unexplored Scene {0}", unexploredScene);
//                route = SceneSegmentation.GetVectorPathToScene(unexploredScene);
//                if (route != null)
//                    return route;
//            }
                
//            Core.Logger.Log("Unable to find alternate route to objective - finished!");
//            _isDone = true;
//            return route;
//        }

//        /// <summary>
//        /// Determines whether [is valid objective].
//        /// </summary>
//        /// <returns><c>true</c> if [is valid objective]; otherwise, <c>false</c>.</returns>
//        private bool IsValidObjective()
//        {
//            try
//            {
//                return (_objectiveObject != null && _objectiveObject.IsValid && _objectiveObject.CommonData != null && _objectiveObject.CommonData.IsValid &&
//                    _objectiveObject.CommonData.GetAttribute<int>(ActorAttributeType.BountyObjective) > 0) || _objectiveObject is GizmoPortal;
//            }
//            catch (Exception ex)
//            {
//                Core.Logger.Log("Exception in IsValidObjective(), {0}", ex);
//            }
//            return false;
//        }

//        /// <summary>
//        /// Finds the objective marker.
//        /// </summary>
//        private void FindObjectiveMarker()
//        {
//            // Get Special objective Marker
//            _miniMapMarker = ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                .Where(m => m.IsPointOfInterest && m.Id < 1500)
//                .OrderBy(m => m.Position.Distance2DSqr(ZetaDia.Me.Position)).FirstOrDefault();

//            if (_miniMapMarker == null)
//            {
//                // Get point of interest marker
//                _miniMapMarker = ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                .Where(m => m.IsPointOfInterest)
//                .OrderBy(m => m.Position.Distance2DSqr(ZetaDia.Me.Position)).FirstOrDefault();
//            }

//            if (_miniMapMarker != null)
//            {
//                Core.Logger.Log("Found Objective Minimap Marker: {0} dist: {1:0} isExit: {2} isEntrance {3}",
//                    _miniMapMarker.NameHash,
//                    _miniMapMarker.Position.Distance2D(ZetaDia.Me.Position),
//                    _miniMapMarker.IsPortalExit,
//                    _miniMapMarker.IsPortalEntrance);
//            }

//            if (_miniMapMarker != null && _miniMapMarker.Position != Vector3.Zero)
//            {
//                _mapMarkerLastPosition = _miniMapMarker.Position;
//            }

//        }

//        /// <summary>
//        /// Refreshes the actor information.
//        /// </summary>
//        private void RefreshActorInfo()
//        {
//            Vector3 myPos = ZetaDia.Me.Position;

//            if (_objectiveObject != null && _objectiveObject.IsValid)
//            {
//                if (QuestTools.IsDebugLoggingEnabled)
//                {
//                    Core.Logger.Log("Found actor {0} {1} {2} of distance {3} from point {4}",
//                        _objectiveObject.ActorSnoId, _objectiveObject.Name, _objectiveObject.ActorType,
//                        _objectiveObject.Position.Distance(_mapMarkerLastPosition), _mapMarkerLastPosition);
//                }
//            }
//            else if (_mapMarkerLastPosition.Distance2D(myPos) <= 200)
//            {
//                try
//                {
//                    // Monsters are the actual objective marker
//                    _objectiveObject = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true)
//                            .Where(a => a.IsFullyValid() && a.Position.Distance2D(_mapMarkerLastPosition) <= PathPrecision && a.IsAlive
//                                    && a.CommonData.GetAttribute<int>(ActorAttributeType.BountyObjective) != 0)
//                            .OrderBy(a => a.Position.Distance2D(_mapMarkerLastPosition))
//                            .FirstOrDefault();

//                    if (_objectiveObject == null)
//                    {
//                        // Portals are not the actual objective but at the marker location
//                        _objectiveObject = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                            .Where(o => o != null && o.IsValid && o.IsValid && o is GizmoPortal
//                                    && o.Position.Distance2DSqr(_mapMarkerLastPosition) <= 9f)
//                           .OrderBy(o => o.Distance)
//                           .FirstOrDefault();
//                    }

//                }
//                catch (Exception ex)
//                {
//                    Core.Logger.Debug("Failed trying to find actor {0}", ex);
//                    _mapMarkerLastPosition = Vector3.Zero;
//                }
//            }
//            else if (_mapMarkerLastPosition != Vector3.Zero && _mapMarkerLastPosition.Distance2D(myPos) <= 90)
//            {
//                _objectiveObject = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where(a => a.IsFullyValid())
//                    .OrderBy(a => a.Position.Distance2D(_mapMarkerLastPosition)).FirstOrDefault();

//                if (_objectiveObject != null && _objectiveObject.IsValid)
//                {
//                    InteractRange = _objectiveObject.CollisionSphere.Radius;
//                    Core.Logger.Log("Found Actor from Objective Marker! mapMarkerPos={0} actor={1} {2} {3} {4}",
//                        _mapMarkerLastPosition, _objectiveObject.ActorSnoId, _objectiveObject.Name, _objectiveObject.ActorType, _objectiveObject.Position);
//                }
//            }

//            if (_objectiveObject != null && _objectiveObject.IsValid)
//            {
//                if (IsValidObjective())
//                {
//                    // need to lock on to a specific actor or we'll just keep finding other things near the marker.
//                    Core.Logger.Log("Found our Objective Actor! mapMarkerPos={0} actor={1} {2} {3} {4}",
//                        _mapMarkerLastPosition, _objectiveObject.ActorSnoId, _objectiveObject.Name, _objectiveObject.ActorType, _objectiveObject.Position);
//                }
//            }

//            if (_objectiveObject is GizmoPortal && !IsPortal)
//            {
//                IsPortal = true;
//            }
//        }

//        /// <summary>
//        /// Gets or sets a value indicating whether this instance is portal.
//        /// </summary>
//        /// <value><c>true</c> if this instance is portal; otherwise, <c>false</c>.</value>
//        public bool IsPortal { get; set; }

//        /// <summary>
//        /// The _QT navigator
//        /// </summary>
//        private readonly QTNavigator _qtNavigator = new QTNavigator();
//        /// <summary>
//        /// The _route
//        /// </summary>
//        private List<Vector3> _route = new List<Vector3>();
//        /// <summary>
//        /// The _is exploring
//        /// </summary>
//        private bool _isExploring;
//        /// <summary>
//        /// The blacklist marker expires
//        /// </summary>
//        private DateTime BlacklistMarkerExpires = DateTime.MinValue;
//        /// <summary>
//        /// The failed attempts
//        /// </summary>
//        private int FailedAttempts;
//        /// <summary>
//        /// The failed attempt maximum
//        /// </summary>
//        private int FailedAttemptMax;

//        /// <summary>
//        /// Safely Moves the player to the requested destination <seealso cref="MoveToObjectiveTag.PathPointLimit" />
//        /// </summary>
//        /// <param name="newpos">Vector3 of the new position</param>
//        /// <param name="destinationName">For logging purposes</param>
//        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
//        private bool Move(Vector3 newpos, string destinationName = "")
//        {
//            bool result = false;

//            if (StraightLinePathing)
//            {
//                Navigator.PlayerMover.MoveTowards(newpos);
//                _lastMoveResult = MoveResult.Moved;
//                result = true;
//            }

//            if (_clientNavFailed && PathPointLimit > 20)
//            {
//                PathPointLimit = PathPointLimit - 10;
//            }
//            else if (_clientNavFailed && PathPointLimit <= 20)
//            {
//                PathPointLimit = 250;
//            }
//            if (newpos.Distance(ZetaDia.Me.Position) > PathPointLimit)
//            {
//                newpos = MathEx.CalculatePointFrom(ZetaDia.Me.Position, newpos, newpos.Distance(ZetaDia.Me.Position) - PathPointLimit);

//                var endposDistance = _mapMarkerLastPosition.Distance(ZetaDia.Me.Position);

//                Core.Logger.Log("Plotting closer point: {0}{5} Distance={1} on path to {2}{6} Distance={3} (PPL:{4})",
//                    newpos,
//                    newpos.Distance(ZetaDia.Me.Position),
//                    _mapMarkerLastPosition,
//                    endposDistance,
//                    PathPointLimit,
//                    ProfileUtils.CanPathToLocation(newpos) ? " (Navigable)" : string.Empty,
//                    ProfileUtils.CanPathToLocation(_mapMarkerLastPosition) ? " (Navigable)" : string.Empty
//                    );
//            }

//            float destinationDistance = newpos.Distance(ZetaDia.Me.Position);

//            _lastMoveResult = _qtNavigator.MoveTo(newpos, destinationName + String.Format(" distance={0:0}", destinationDistance));

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
//                Core.Logger.Debug("MoveResult: {0}, newpos={1} Distance={2}, destinationName={3}",
//                    _lastMoveResult.ToString(), newpos, newpos.Distance(ZetaDia.Me.Position), destinationName);
//            }
//            return result;
//        }

//        /// <summary>
//        /// Determines whether this instance is valid.
//        /// </summary>
//        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
//        public bool IsValid()
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

//        /// <summary>
//        /// Statuses this instance.
//        /// </summary>
//        /// <returns>String.</returns>
//        public String Status()
//        {
//            string extraInfo = "";
//            if (DataDictionary.RiftWorldIds.Contains(ZetaDia.Globals.WorldSnoId))
//                extraInfo += "IsRift ";

//            var target = new Vector3(X, Y, Z);
//            if (target == Vector3.Zero && _miniMapMarker != null)
//                target = _miniMapMarker.Position;
//            if (target == Vector3.Zero && _route.Any())
//                target = _currentRouteDestination;

//            if (QuestToolsSettings.Instance.DebugEnabled)
//                return String.Format("questId={0} stepId={1} " +
//                    "pathPointLimit={2} interactAttempts={3} interactRange={4} pathPrecision={5} x=\"{6}\" y=\"{7}\" z=\"{8}\" " + extraInfo,
//                    QuestId, StepId, PathPointLimit, InteractAttempts, InteractRange, PathPrecision, target.X, target.Y, target.Z
//                    );

//            return string.Empty;
//        }

//        /// <summary>
//        /// Resets the cached done.
//        /// </summary>
//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            base.ResetCachedDone();
//        }

//        #region IEnhancedProfileBehavior

//        /// <summary>
//        /// For ProfileBehavior to run correctly Behavior property has to contain the composite
//        /// Call UpdateBehavior() in implementation.
//        /// </summary>
//        public void Update()
//        {
//            UpdateBehavior();
//        }

//        /// <summary>
//        /// Many tags use OnStart for setup and default params
//        /// Call OnStart() in implementation.
//        /// </summary>
//        public void Start()
//        {
//            OnStart();
//        }

//        /// <summary>
//        /// Method that should end the ProfileBehavior
//        /// Set _isDone = true in implementation.
//        /// Call Done() on children if INodeContainer
//        /// </summary>
//        public void Done()
//        {
//            _isDone = true;
//        }

//        #endregion


//        /// <summary>
//        /// Gets or sets the _current route destination.
//        /// </summary>
//        /// <value>The _current route destination.</value>
//        public Vector3 _currentRouteDestination { get; set; }
//    }
//}
