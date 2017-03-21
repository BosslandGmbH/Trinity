//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq; using Trinity.Framework;
//using System.Threading.Tasks;
//using Buddy.Coroutines;
//using QuestTools.Helpers;
//using QuestTools.Navigation;
//using QuestTools.ProfileTags.Complex;
//using Zeta.Bot;
//using Zeta.Bot.Coroutines;
//using Zeta.Bot.Dungeons;
//using Zeta.Bot.Logic;
//using Zeta.Bot.Navigation;
//using Zeta.Bot.Profile;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.SNO;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;
//using Trinity.Framework;

//namespace QuestTools.ProfileTags
//{
//    /// <summary>
//    /// ExploreDungeon is fuly backwards compatible with the built-in Demonbuddy ExploreArea tag. It provides additional features such as:
//    /// Moving to investigate MiniMapMarker pings and the current ExitNameHash if provided and visible (mini map marker 0 and the current exitNameHash)
//    /// Moving to investigate Priority Scenes if provided (PrioritizeScenes subtags)
//    /// Ignoring DungeonExplorer nodes in certain scenes if provided (IgnoreScenes subtags)
//    /// Reduced backtracking (via pathPrecision attribute and combat skip ahead cache)
//    /// Multiple ActorId's for the ObjectFound end type (AlternateActors sub-tags)
//    /// </summary>
//    [XmlElement("TrinityExploreDungeon")]
//    [XmlElement("ExploreDungeon")]
//    public class ExploreDungeonTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public ExploreDungeonTag() { }

//        #region XML Attributes

//        /// <summary> Selected the Route Mod. </summary>
//        [XmlAttribute("routeMode")]
//        [DefaultValue(RouteMode.NearestMinimapUnvisited)]
//        public RouteMode RouteMode { get; set; }

//        /// <summary> Sets the direction for scene based directional navigation. </summary>
//        [XmlAttribute("direction")]
//        [DefaultValue(Direction.Any)]
//        public Direction Direction { get; set; }

//        /// <summary>
//        /// Sets a custom grid segmentation Box Size (default 15)
//        /// </summary>
//        [XmlAttribute("boxSize")]
//        public int BoxSize { get; set; }

//        /// <summary>
//        /// Sets a custom grid segmentation Box Tolerance (default 0.55)
//        /// </summary>
//        [XmlAttribute("boxTolerance")]
//        public float BoxTolerance { get; set; }

//        /// <summary>
//        /// The until="" atribute must match one of these
//        /// </summary>
//        public enum ExploreEndType
//        {
//            FullyExplored = 0,
//            ObjectFound,
//            ExitFound,
//            SceneFound,
//            SceneLeftOrActorFound,
//            BountyComplete,
//            RiftComplete,
//            RiftCompleteOrFullyExplored,
//            PortalExitFound,
//            ObjectiveFound,
//            ObjectiveFoundOrBountyComplete,
//            ActorFoundOrBountyComplete,
//            BossEncounterCompleted
//        }

//        [XmlAttribute("endType")]
//        [XmlAttribute("until")]
//        public ExploreEndType EndType { get; set; }

//        /// <summary>
//        /// The distance the bot will mark dungeon nodes as "visited" (default is 1/2 of box size, minimum 10)
//        /// </summary>
//        [XmlAttribute("pathPrecision")]
//        public float PathPrecision { get; set; }

//        /// <summary>
//        /// The SNOId of the Actor that we're looking for, used with until="ObjectFound"
//        /// </summary>
//        [XmlAttribute("actorId")]
//        public int ActorId { get; set; }

//        [XmlAttribute("bossEncounter")]
//        public SNOBossEncounter BossEncounter { get; set; }

//        /// <summary>
//        /// The nameHash of the exit the bot will move to and finish the tag when found
//        /// </summary>
//        [XmlAttribute("exitNameHash")]
//        public int ExitNameHash { get; set; }

//        [XmlAttribute("ignoreGridReset")]
//        public bool IgnoreGridReset { get; set; }

//        /// <summary>
//        /// Not currently implimented
//        /// </summary>
//        [XmlAttribute("leaveWhenFinished")]
//        public bool LeaveWhenExplored { get; set; }

//        /// <summary>
//        /// The distance the bot must be from an actor before marking the tag as complete, when used with until="ObjectFound"
//        /// </summary>
//        [XmlAttribute("objectDistance")]
//        public float ObjectDistance { get; set; }

//        /// <summary>
//        /// The Scene SNOId, used with ExploreUntil="SceneFound"
//        /// </summary>
//        [XmlAttribute("sceneId")]
//        public int SceneId { get; set; }

//        /// <summary>
//        /// The Scene Name, used with ExploreUntil="SceneFound", a sub-string match will work
//        /// </summary>
//        [XmlAttribute("sceneName")]
//        public string SceneName { get; set; }

//        /// <summary>
//        /// The distance before reaching a MiniMapMarker before marking it as visited
//        /// </summary>
//        [XmlAttribute("markerDistance")]
//        public float MarkerDistance { get; set; }

//        [XmlAttribute("findExits")]
//        [DefaultValue(false)]
//        public bool FindExits { get; set; }

//        /// <summary>
//        /// Disable Mini Map Marker Scouting
//        /// </summary>
//        [XmlAttribute("ignoreMarkers")]
//        public bool IgnoreMarkers { get; set; }

//        public enum TimeoutType
//        {
//            Timer,
//            GoldInactivity,
//            None,
//        }

//        /// <summary>
//        /// The TimeoutType to use (default None, no timeout)
//        /// </summary>
//        [XmlAttribute("timeoutType")]
//        public TimeoutType ExploreTimeoutType { get; set; }

//        /// <summary>
//        /// Value in Seconds. 
//        /// The timeout value to use, when used with Timer will force-end the tag after a certain time. When used with GoldInactivity will end the tag after coinages doesn't change for the given period
//        /// </summary>
//        [XmlAttribute("timeout")]
//        [XmlAttribute("timeoutValue")]
//        public int TimeoutValue { get; set; }

//        /// <summary>
//        /// If we want to use a townportal before ending the tag when a timeout happens
//        /// </summary>
//        [XmlAttribute("townPortalOnTimeout")]
//        public bool TownPortalOnTimeout { get; set; }

//        /// <summary>
//        /// Ignore last N nodes of dungeon explorer, when using endType=FullyExplored
//        /// </summary>
//        [XmlAttribute("ignoreLastNodes")]
//        public int IgnoreLastNodes { get; set; }

//        /// <summary>
//        /// Used with IgnoreLastNodes, minimum visited node count before tag can end. 
//        /// The minVisitedNodes is purely, and only for use with ignoreLastNodes - it does not serve any other function like you expect. 
//        /// The reason this attribute exists, is to prevent prematurely exiting the dungeon exploration when used with ignoreLastNodes. 
//        /// For example, when the bot first starts exploring an area, it needs to navigate a few dungeon nodes first before other dungeon nodes even appear - otherwise with ignoreLastNodes > 2, 
//        /// the bot would immediately exit from navigation without exploring anything at all.
//        /// </summary>
//        [XmlAttribute("minVisitedNodes")]
//        public int MinVisitedNodes { get; set; }

//        [XmlAttribute("minObjectOccurances")]
//        public int MinOccurances { get; set; }

//        [XmlAttribute("interactWithObject")]
//        public bool InteractWithObject { get; set; }

//        [XmlAttribute("interactRange")]
//        public float ObjectInteractRange { get; set; }

//        [XmlAttribute("setNodesExploredAutomatically")]
//        [DefaultValue(true)]
//        public bool SetNodesExploredAutomatically { get; set; }
//        #endregion

//        #region XML Elements
//        /// <summary>
//        /// The list of Scene SNOId's or Scene Names that the bot will ignore dungeon nodes in
//        /// </summary>
//        [XmlElement("IgnoreScenes")]
//        public List<IgnoreScene> IgnoreScenes { get; set; }

//        /// <summary>
//        /// The list of Scene SNOId's or Scene Names that the bot will prioritize (only works when the scene is "loaded")
//        /// </summary>
//        [XmlElement("PriorityScenes")]
//        [XmlElement("PrioritizeScenes")]
//        public List<PrioritizeScene> PriorityScenes { get; set; }

//        /// <summary>
//        /// The list of Scene SNOId's or Scene Names that the bot will use for endtype SceneLeftOrActorFound
//        /// </summary>
//        [XmlElement("AlternateScenes")]
//        public List<AlternateScene> AlternateScenes { get; set; }

//        /// <summary>
//        /// The Ignore Scene class, used as IgnoreScenes child elements
//        /// </summary>
//        [XmlElement("IgnoreScene")]
//        public class IgnoreScene : IEquatable<Scene>
//        {
//            [XmlAttribute("sceneName")]
//            public string SceneName { get; set; }
//            [XmlAttribute("sceneId")]
//            public int SceneId { get; set; }

//            public IgnoreScene()
//            {
//                SceneId = -1;
//                SceneName = String.Empty;
//            }

//            public IgnoreScene(string name)
//            {
//                SceneName = name;
//            }
//            public IgnoreScene(int id)
//            {
//                SceneId = id;
//            }

//            public bool Equals(Scene other)
//            {
//                return (!string.IsNullOrWhiteSpace(SceneName) && other.Name.ToLowerInvariant().Contains(SceneName.ToLowerInvariant())) || other.SceneInfo.SNOId == SceneId;
//            }
//        }

//        private CachedValue<List<Area>> _ignoredAreas;
//        private List<Area> IgnoredAreas
//        {
//            get
//            {
//                if (_ignoredAreas == null)
//                    _ignoredAreas = new CachedValue<List<Area>>(GetIgnoredAreas, TimeSpan.FromSeconds(1));
//                return _ignoredAreas.Value;
//            }
//        }

//        private List<Area> GetIgnoredAreas()
//        {
//            var ignoredScenes = ZetaDia.Scenes
//                .Where(scn => scn.IsValid && scn.Mesh.Zone != null && IgnoreScenes.Any(igns => igns.Equals(scn)) && !PriorityScenes.Any(psc => psc.Equals(scn)))
//                .Select(scn => new Area(scn.Mesh.Zone.ZoneMin, scn.Mesh.Zone.ZoneMax)).ToList();
//            return ignoredScenes;
//        }

//        private class Area
//        {
//            public Area() { }
//            private Vector2 Min { get; set; }
//            private Vector2 Max { get; set; }

//            /// <summary>
//            /// Initializes a new instance of the Area class.
//            /// </summary>
//            public Area(Vector2 min, Vector2 max)
//            {
//                Min = min;
//                Max = max;
//            }

//            private bool IsPositionInside(Vector2 position)
//            {
//                return position.X >= Min.X && position.X <= Max.X && position.Y >= Min.Y && position.Y <= Max.Y;
//            }

//            public bool IsPositionInside(Vector3 position)
//            {
//                return IsPositionInside(position.ToVector2());
//            }
//        }

//        /// <summary>
//        /// The Priority Scene class, used as PrioritizeScenes child elements
//        /// </summary>
//        [XmlElement("PriorityScene")]
//        [XmlElement("PrioritizeScene")]
//        public class PrioritizeScene : IEquatable<Scene>
//        {
//            [XmlAttribute("sceneName")]
//            public string SceneName { get; set; }
//            [XmlAttribute("sceneId")]
//            [DefaultValue(-1)]
//            public int SceneId { get; set; }
//            [XmlAttribute("pathPrecision")]
//            public float PathPrecision { get; set; }

//            public PrioritizeScene()
//            {
//                PathPrecision = 15f;
//                SceneName = string.Empty;
//                SceneId = -1;
//            }

//            public PrioritizeScene(string name)
//            {
//                SceneName = name;
//                SceneId = -1;
//            }
//            public PrioritizeScene(int id)
//            {
//                SceneId = id;
//                SceneName = null;
//            }
//            public bool Equals(Scene other)
//            {
//                return (SceneName != string.Empty && other.Name.ToLowerInvariant().Contains(SceneName.ToLowerInvariant())) || other.SceneInfo.SNOId == SceneId;
//            }
//        }

//        /// <summary>
//        /// The Alternate Scene class, used as AlternateScenes child elements
//        /// </summary>
//        [XmlElement("AlternateScene")]
//        public class AlternateScene : IEquatable<Scene>
//        {
//            [XmlAttribute("sceneName")]
//            public string SceneName { get; set; }
//            [XmlAttribute("sceneId")]
//            public int SceneId { get; set; }
//            [XmlAttribute("pathPrecision")]
//            public float PathPrecision { get; set; }

//            public AlternateScene()
//            {
//                PathPrecision = 15f;
//                SceneName = string.Empty;
//                SceneId = -1;
//            }

//            public AlternateScene(string name)
//            {
//                SceneName = name;
//            }
//            public AlternateScene(int id)
//            {
//                SceneId = id;
//            }
//            public bool Equals(Scene other)
//            {
//                return (SceneName != string.Empty && other.Name.ToLowerInvariant().Contains(SceneName.ToLowerInvariant())) || other.SceneInfo.SNOId == SceneId;
//            }
//        }

//        [XmlElement("AlternateActors")]
//        public List<AlternateActor> AlternateActors { get; set; }

//        [XmlElement("AlternateActor")]
//        public class AlternateActor
//        {
//            [XmlAttribute("actorId")]
//            public int ActorId { get; set; }

//            [XmlAttribute("objectDistance")]
//            public float ObjectDistance { get; set; }

//            [XmlAttribute("interactRange")]
//            public float InteractRange { get; set; }

//            public AlternateActor()
//            {
//                ActorId = -1;
//                ObjectDistance = 60f;
//            }
//        }

//        [XmlElement("AlternateMarkers")]
//        public List<AlternateMarker> AlternateMarkers { get; set; }

//        [XmlElement("AlternateMarker")]
//        public class AlternateMarker
//        {
//            [XmlAttribute("markerNameHash")]
//            public int MarkerNameHash { get; set; }

//            [XmlAttribute("markerDistance")]
//            public float MarkerDistance { get; set; }

//            public AlternateMarker()
//            {
//                MarkerNameHash = 0;
//                MarkerDistance = 45f;
//            }
//        }

//        [XmlElement("Objectives")]
//        public List<Objective> Objectives { get; set; }

//        [XmlElement("Objective")]
//        public class Objective
//        {
//            [XmlAttribute("actorId")]
//            public int ActorId { get; set; }

//            [XmlAttribute("markerNameHash")]
//            public int MarkerNameHash { get; set; }

//            [XmlAttribute("count")]
//            public int Count { get; set; }

//            [XmlAttribute("endAnimation")]
//            public SNOAnim EndAnimation { get; set; }

//            [XmlAttribute("interact")]
//            public bool Interact { get; set; }

//            public Objective()
//            {

//            }
//        }
//        #endregion

//        readonly HashSet<Tuple<int, Vector3>> _foundObjects = new HashSet<Tuple<int, Vector3>>();

//        /// <summary>
//        /// The Position of the CurrentNode NavigableCenter
//        /// </summary>
//        private Vector3 CurrentNavTarget
//        {
//            get
//            {
//                // Priority Scenes
//                if (_prioritySceneTarget != Vector3.Zero)
//                {
//                    return _prioritySceneTarget;
//                }

//                // Special Pandemonium Fortress routes
//                if (GetIsInPandemoniumFortress())
//                {
//                    var marker = MiniMapMarker.GetNearestUnvisitedMarker(MyPosition);
//                    if (marker != null)
//                        return marker.Position;

//                    if (_lastPandFortressTarget != Vector3.Zero && GridRoute.Nodes.Any(n => !n.Visited && n.NavigableCenter == _lastPandFortressTarget))
//                        return _lastPandFortressTarget;

//                    GetPandFortressNavTarget();

//                    if (_lastPandFortressTarget != Vector3.Zero)
//                        return _lastPandFortressTarget;
//                }

//                // Default Navigation
//                if (GetRouteUnvisitedNodeCount() > 0)
//                {
//                    var navTarget = GridRoute.CurrentNode.NavigableCenter;
//                    if (navTarget == Vector3.Zero)
//                        navTarget = GridRoute.CurrentNode.Center.ToVector3();
//                    return navTarget;
//                }
//                return Vector3.Zero;
//            }
//        }

//        // Adding these for SimpleFollow compatability
//        public float X { get { return CurrentNavTarget.X; } }
//        public float Y { get { return CurrentNavTarget.Y; } }
//        public float Z { get { return CurrentNavTarget.Z; } }

//        private bool _initDone;

//        /// <summary>
//        /// The current player position
//        /// </summary>
//        private static Vector3 MyPosition { get { return ZetaDia.Me.Position; } }

//        /// <summary>
//        /// The last position we updated the SearchGridProvider at
//        /// </summary>
//        private Vector3 _gridProviderUpdatePosition = Vector3.Zero;

//        /// <summary>
//        /// Called when the profile behavior starts
//        /// </summary>
//        public override void OnStart()
//        {
//            Core.Logger.Log("ExploreDungeon Started");

//            // Don't forget this...
//            BrainBehavior.DungeonExplorer.SetNodesExploredAutomatically = SetNodesExploredAutomatically;
//            if (SetNodesExploredAutomatically)
//            {
//                Core.Logger.Log("Dungeon Explorer ignoring already explored nodes!");
//                GridRoute.SetNodesExploredAutomatically = SetNodesExploredAutomatically;
//            }

//            GridRoute.Update();

//            if (!IgnoreGridReset && !ZetaDia.Me.IsDead && DateTime.UtcNow.Subtract(Death.LastDeathTime).TotalSeconds > 3)
//            {
//                CheckResetDungeonExplorer();
//                GridRoute.Reset(BoxSize, BoxTolerance);
//                MiniMapMarker.KnownMarkers.Clear();
//            }

//            if (!_initDone)
//                Init();
            
//            _tagTimer.Restart();
//            _timesForcedReset = 0;

//            if (Objectives == null)
//                Objectives = new List<Objective>();

//            if (ObjectDistance < 1f)
//                ObjectDistance = 25f;

//            NavigationProvider.EnableDebugLogging = true;

//            PrintNodeCounts("PostInit");
//        }

//        private void SetRouteMode()
//        {
//            if (QuestToolsSettings.Instance.ForceRouteMode)
//                RouteMode = QuestToolsSettings.Instance.RouteMode;
//        }

//        /// <summary>
//        /// The main profile behavior
//        /// </summary>
//        /// <returns></returns>
//        protected override Composite CreateBehavior()
//        {
//            return
//            new Sequence(
//                new Action(ret => SetRouteMode()),
//                new DecoratorContinue(ret => !IgnoreMarkers,
//                    new Sequence(
//                        MiniMapMarker.DetectMiniMapMarkers(),
//                        MiniMapMarker.DetectMiniMapMarkers(ExitNameHash),
//                        MiniMapMarker.DetectMiniMapMarkers(Objectives),
//                        MiniMapMarker.DetectMiniMapMarkers(AlternateMarkers),
//                        new DecoratorContinue(ret => FindExits,
//                            new Action(ret => MiniMapMarker.DetectMiniMapMarkers(0, true))
//                        )
//                    )
//                ),
//                // I dunno if this will work...
//                new DecoratorContinue(ret => Navigator.StuckHandler.IsStuck,
//                    new Action(ret => _stuckCount++)),

//                new DecoratorContinue(ret => !Navigator.StuckHandler.IsStuck,
//                    new Action(ret => _stuckCount = 0)),

//                new Action(ret => CheckResetDungeonExplorer()),
//                new PrioritySelector(
//                    CheckIsObjectiveFinished(),
//                    new Decorator(ret => !IgnoreMarkers,
//                        MiniMapMarker.VisitMiniMapMarkers(MyPosition, MarkerDistance)
//                    ),
//                    PrioritySceneCheck(),
//                    new Decorator(ret => ShouldInvestigateActor(),
//                        new PrioritySelector(
//                            new Decorator(ret => CurrentActor != null && CurrentActor.IsValid &&
//                                Objectives.Any(o => o.ActorId == CurrentActor.ActorSnoId && o.Interact) &&
//                                CurrentActor.Position.Distance(ZetaDia.Me.Position) <= CurrentActor.CollisionSphere.Radius,
//                                new Sequence(
//                                    new Action(ret => CurrentActor.Interact())
//                                )
//                            ),
//                            InvestigateActor()
//                        )
//                    ),
//                    new Sequence(
//                        new DecoratorContinue(ret => DungeonRouteIsEmpty(),
//                            new Action(ret => UpdateRoute())
//                        ),
//                        CheckIsExplorerFinished()
//                    ),
//                    new DecoratorContinue(ret => DungeonRouteIsValid(),
//                        new PrioritySelector(
//                            CheckNodeFinished(),
//                            new Sequence(
//                                new Action(ret => PrintNodeCounts("MainBehavior")),
//                                new PrioritySelector(
//                                    MoveToAndUseDeathGate(),
//                                    new Action(ret => MoveToNextNode())
//                                )
//                            )
//                        )
//                    ),
//                    new Action(ret => Core.Logger.Debug("Error 1: Unknown error occured!"))
//                )
//            );
//        }

//        private bool IsSearchGridInitialized { get { return Navigator.SearchGridProvider.Width != 0; } }

//        private Vector3 _lastPathCheckTarget = Vector3.Zero;
//        private bool _lastPathCheckResult;

//        private async Task<bool> CanFullPathToCurrentNavTarget(System.Action onSuccess = null, System.Action onFailure = null)
//        {
//            bool result;
//            var navTarget = CurrentNavTarget;
//            var tooFarAway = navTarget.Distance2D(ZetaDia.Me.Position) > 500;
//            var invalidTarget = navTarget == Vector3.Zero;
//            var alreadyCheckedTarget = _lastPathCheckTarget == navTarget;

//            if (!invalidTarget && !alreadyCheckedTarget)
//            {
//                _lastPathCheckTarget = navTarget;
//                Core.Logger.Debug("Checking Path to navTarget {0}", navTarget);
//                _lastPathCheckResult = await NavigationProvider.CanPathWithinDistance(navTarget, PathPrecision);
//                if (Navigator.SearchGridProvider.Width == 0)
//                {
//                    Core.Logger.Log("Waiting for Navigation Server...");
//                }
//                if (!_lastPathCheckResult)
//                    Core.Logger.Debug("Unable to fully path to {0} with precision {1}, distance {2:0}", navTarget, PathPrecision, navTarget.Distance2D(ZetaDia.Me.Position));
//            }

//            result = !tooFarAway && _lastPathCheckResult;
            
//            if (result && onSuccess != null)
//                onSuccess();

//            if (!result && onFailure != null)
//                onFailure();

//            return result;
//        }

//        private static MainGridProvider MainGridProvider
//        {
//            get
//            {
//                return (MainGridProvider)Navigator.SearchGridProvider;
//            }
//        }
//        /// <summary>
//        /// Re-sets the DungeonExplorer, BoxSize, BoxTolerance, and Updates the current route
//        /// </summary>
//        private void CheckResetDungeonExplorer()
//        {
//            if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || !ZetaDia.WorldInfo.IsValid || !ZetaDia.Scenes.IsValid || !ZetaDia.Service.IsValid)
//                return;

//            // I added this because GridSegmentation may (rarely) reset itself without us doing it to 15/.55.
//            if ((BoxSize != 0 && BoxTolerance != 0) && (GridSegmentation.BoxSize != BoxSize || GridSegmentation.BoxTolerance != BoxTolerance) || (GetGridRouteNodeCount() == 0))
//            {
//                Core.Logger.Debug("Box Size or Tolerance has been changed! {0}/{1} NodeCount={2}", GridSegmentation.BoxSize, GridSegmentation.BoxTolerance, GetGridRouteNodeCount());

//                GridRoute.Reset(BoxSize, BoxTolerance);
//                PrintNodeCounts("GridRoute.Reset");
//            }
//        }

//        private static bool DungeonRouteIsValid()
//        {
//            return GridRoute.CurrentRoute != null && GridRoute.CurrentRoute.Any();
//        }

//        private static bool DungeonRouteIsEmpty()
//        {
//            return GridRoute.CurrentRoute != null && !GridRoute.CurrentRoute.Any();
//        }

//        private bool CurrentActorIsFinished
//        {
//            get
//            {
//                return Objectives.Any(o => o.ActorId == CurrentActor.ActorSnoId && o.EndAnimation == CurrentActor.CommonData.CurrentAnimation);
//            }
//        }

//        private DiaObject CurrentActor
//        {
//            get
//            {
//                var actor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                    .Where(diaObj => 
//                    diaObj.IsValid && 
//                    (diaObj.ActorSnoId == ActorId || Objectives.Any(o => o.ActorId != 0 && o.ActorId == diaObj.ActorSnoId)) &&
//                    PositionCache.Cache.Any(pos => pos.Distance2DSqr(diaObj.Position) >= ObjectDistance * ObjectDistance) &&
//                    _foundObjects.All(fo => fo.Equals(new Tuple<int, Vector3>(diaObj.ActorSnoId, diaObj.Position))))
//                .OrderBy(o => o.Distance)
//                .FirstOrDefault();

//                return actor != null && actor.IsValid ? actor : null;
//            }
//        }

//        private Composite InvestigateActor()
//        {
//            return new ActionRunCoroutine(async ret =>
//            {
//                RecordPosition();

//                var actor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault(a => a.IsValid && a.ActorSnoId == ActorId);

//                if (Navigator.SearchGridProvider.Width == 0)
//                {
//                    Core.Logger.Log("Waiting for Navigation Server...");
//                }

//                if (actor != null && actor.IsValid && actor.Position.Distance2DSqr(MyPosition) >= ObjectDistance * ObjectDistance)
//                   //await CommonCoroutines.MoveTo(actor.Position, string.Format("InvestigateActor {0} {1} {2}", actor.ActorSnoId, actor.Name, actor.ActorType));
//                    await Navigator.MoveTo(actor.Position, string.Format("InvestigateActor {0} {1} {2}", actor.ActorSnoId, actor.Name, actor.ActorType));
//            });
//        }

//        private bool ShouldInvestigateActor()
//        {
//            if (ActorId == 0 || Objectives.All(o => o.ActorId == 0))
//                return false;

//            var actors = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                .Where(diaObj => diaObj.IsValid && (diaObj.ActorSnoId == ActorId ||
//                    AlternateActors.Any(alternateActor => alternateActor.ActorId != 0 && alternateActor.ActorId == diaObj.ActorSnoId) ||
//                    Objectives.Any(objective => objective.ActorId != 0 && objective.ActorId == diaObj.ActorSnoId) ||
//                    diaObj.CommonData.GetAttribute<int>(ActorAttributeType.BountyObjective) > 0) &&
//                    PositionCache.Cache.Any(pos => pos.Distance2DSqr(diaObj.Position) >= ObjectDistance * ObjectDistance) &&
//                    _foundObjects.All(fo => fo.Equals(new Tuple<int, Vector3>(diaObj.ActorSnoId, diaObj.Position)))).ToList();

//            if (!actors.Any())
//                return false;

//            var actor = actors.OrderBy(a => a.Distance).FirstOrDefault();

//            if (actor != null && actor.Distance <= ObjectDistance)
//                return false;

//            return true;
//        }

//        /// <summary>
//        /// Checks if we are using a timeout and will end the tag if the timer has breached the given value
//        /// </summary>
//        /// <returns></returns>
//        private Composite TimeoutCheck()
//        {
//            return
//            new PrioritySelector(
//                new Decorator(ret => _timeoutBreached,
//                    new Sequence(
//                        new DecoratorContinue(ret => TownPortalOnTimeout,
//                            new Sequence(
//                                new Action(ret => Core.Logger.Log(
//                                    "ExploreDungeon timer tripped ({0}), tag finished, Using Town Portal!", TimeoutValue)),
//                                Zeta.Bot.CommonBehaviors.CreateUseTownPortal(),
//                                new Action(ret => _isDone = true)
//                            )
//                        ),
//                        new DecoratorContinue(ret => !TownPortalOnTimeout,
//                            new Sequence(
//                                new Action(ret => Core.Logger.Log(
//                                    "ExploreDungeon timer tripped ({0}), tag finished!", TimeoutValue)),
//                                new Action(ret => _isDone = true)
//                            )
//                        )
//                    )
//                ),
//                new Decorator(ret => ExploreTimeoutType == TimeoutType.Timer,
//                    new Action(ret => CheckSetTimer())
//                ),
//                new Decorator(ret => ExploreTimeoutType == TimeoutType.GoldInactivity,
//                    new Action(ret => CheckSetGoldInactive())
//                )
//            );
//        }

//        bool _timeoutBreached;
//        readonly Stopwatch _tagTimer = new Stopwatch();

//        /// <summary>
//        /// Will start the timer if needed, and end the tag if the timer has exceeded the TimeoutValue
//        /// </summary>
//        /// <returns></returns>
//        private RunStatus CheckSetTimer()
//        {
//            if (!_tagTimer.IsRunning)
//            {
//                _tagTimer.Start();
//                return RunStatus.Failure;
//            }
//            if (ExploreTimeoutType == TimeoutType.Timer && _tagTimer.Elapsed.TotalSeconds > TimeoutValue)
//            {
//                Core.Logger.Log("ExploreDungeon timer ended ({0}), tag finished!", TimeoutValue);
//                _timeoutBreached = true;
//                return RunStatus.Success;
//            }
//            return RunStatus.Failure;
//        }

//        private long _lastCoinage = -1;

//        /// <summary>
//        /// Will check if the bot has not picked up any gold within the allocated TimeoutValue
//        /// </summary>
//        /// <returns></returns>
//        private RunStatus CheckSetGoldInactive()
//        {
//            CheckSetTimer();
//            if (_lastCoinage == -1)
//            {
//                _lastCoinage = Core.Player.Coinage;
//                return RunStatus.Failure;
//            }
//            if (_lastCoinage != Core.Player.Coinage)
//            {
//                _tagTimer.Restart();
//                return RunStatus.Failure;
//            }
//            if (_lastCoinage == Core.Player.Coinage && _tagTimer.Elapsed.TotalSeconds > TimeoutValue)
//            {
//                Core.Logger.Log("ExploreDungeon gold inactivity timer tripped ({0}), tag finished!", TimeoutValue);
//                _timeoutBreached = true;
//                return RunStatus.Success;
//            }

//            return RunStatus.Failure;
//        }

//        private int _timesForcedReset;
//        private const int TimesForceResetMax = 20;

//        /// <summary>
//        /// Checks to see if the tag is finished as needed
//        /// </summary>
//        /// <returns></returns>
//        private Composite CheckIsExplorerFinished()
//        {
//            return
//            new PrioritySelector(
//                CheckIsObjectiveFinished(),
//                new Decorator(ret => IsSearchGridInitialized && GetRouteUnvisitedNodeCount() == 0 && _timesForcedReset > TimesForceResetMax,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log(
//                            "Visited all nodes but objective not complete, forced reset more than {0} times, finished!", TimesForceResetMax)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => IsSearchGridInitialized && GetRouteUnvisitedNodeCount() == 0,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Visited all nodes but objective not complete, forcing grid reset!")),
//                        new DecoratorContinue(ret => GridRoute.SetNodesExploredAutomatically,
//                            new Sequence(
//                                new Action(ret => Core.Logger.Log("Disabling SetNodesExploredAutomatically")),
//                                new Action(ret => GridRoute.SetNodesExploredAutomatically = false)
//                            )
//                        ),
//                        new DecoratorContinue(ret => _timesForcedReset > 2 && GetCurrentRouteNodeCount() == 1,
//                            new Sequence(
//                                new Action(ret => Core.Logger.Log("Only 1 node found and 3 grid resets, falling back to failsafe!")),
//                                new Action(ret => BoxSize = 25),
//                                new Action(ret => BoxTolerance = 0.01f),
//                                new Action(ret => IgnoreScenes.Clear())
//                            )
//                        ),
//                        new Action(ret =>
//                        {
//                            _timesForcedReset++;
//                            PositionCache.Cache.Clear();
//                            MiniMapMarker.KnownMarkers.Clear();
//                            GridRoute.Reset(BoxSize, BoxTolerance);
//                            _priorityScenesInvestigated.Clear();
//                            UpdateRoute();
//                        })
//                    )
//                )
//           );
//        }

//        private MinimapMarker GetObjectiveMarker()
//        {
//            // Get Special objective Marker
//            MinimapMarker miniMapMarker = ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                .Where(m => m.IsPointOfInterest && m.Id < 1000)
//                .OrderBy(m => m.Position.Distance2DSqr(ZetaDia.Me.Position))
//                .FirstOrDefault();

//            if (miniMapMarker == null)
//            {
//                // Get point of interest marker
//                miniMapMarker = ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                .Where(m => m.IsPointOfInterest)
//                .OrderBy(m => m.Position.Distance2DSqr(ZetaDia.Me.Position))
//                .FirstOrDefault();
//            }

//            return miniMapMarker;
//        }

//        private bool GetIsMapFullyExplored()
//        {
//            return IgnoreLastNodes > 0 && GetRouteUnvisitedNodeCount() <= IgnoreLastNodes && GetGridRouteVisitedNodeCount() >= MinVisitedNodes;
//        }

//        /// <summary>
//        /// Checks to see if the tag is finished as needed
//        /// </summary>
//        /// <returns></returns>
//        private Composite CheckIsObjectiveFinished()
//        {
//            return
//            new PrioritySelector(
//                TimeoutCheck(),

//                new Decorator(ret => EndType == ExploreEndType.BossEncounterCompleted && ZetaDia.Storage.Quests.IsBossEncounterCompleted(BossEncounter),
//                    new Sequence(
//                            new Action(ret => Core.Logger.Log("BossEncounter: {0} completed. Tag Finished.", BossEncounter)),
//                            new Action(ret => _isDone = true)
//                        )
//                ),

//                new Decorator(ret => (EndType == ExploreEndType.ObjectiveFound || EndType == ExploreEndType.ObjectiveFoundOrBountyComplete) && GetIsObjectiveInRange(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Found Objective Marker. Tag Finished.")),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.RiftComplete && GetIsRiftDone(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Rift is done. Tag Finished.")),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.RiftCompleteOrFullyExplored && (GetIsRiftDone() || GetIsMapFullyExplored()),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Rift is done. Tag Finished.")),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.PortalExitFound &&
//                    PortalExitMarker() != null && PortalExitMarker().Position.Distance2D(MyPosition) <= MarkerDistance,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Found portal exit! Tag Finished.")),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => (EndType == ExploreEndType.BountyComplete || EndType == ExploreEndType.ObjectiveFoundOrBountyComplete || EndType == ExploreEndType.ActorFoundOrBountyComplete) && GetIsBountyDone(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Bounty is done. Tag Finished.")),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.FullyExplored && GetIsMapFullyExplored(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Fully explored area! Ignoring {0} nodes. Tag Finished.", IgnoreLastNodes)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.FullyExplored && GetRouteUnvisitedNodeCount() == 0,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Fully explored area! Tag Finished.", 0)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.ExitFound && ExitNameHash != 0 && ExitNameHashInRange(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Found exitNameHash {0}!", ExitNameHash)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => (EndType == ExploreEndType.ObjectFound || EndType == ExploreEndType.SceneLeftOrActorFound || EndType == ExploreEndType.ActorFoundOrBountyComplete) && ActorId != 0 && ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                    .Any(a => a.ActorSnoId == ActorId && !GetIsGuardedBountyGizmoActivated(a) && a.Distance <= ObjectDistance),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Found Object {0}!", ActorId)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => (EndType == ExploreEndType.ObjectFound || EndType == ExploreEndType.SceneLeftOrActorFound || EndType == ExploreEndType.ActorFoundOrBountyComplete) && AlternateActorsFound(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Found Alternate Object {0}!", GetAlternateActor().ActorSnoId)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.SceneFound && ZetaDia.Me.SceneId == SceneId,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Found SceneId {0}!", SceneId)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.SceneFound && !string.IsNullOrWhiteSpace(SceneName) && ZetaDia.Me.CurrentScene.Name.ToLower().Contains(SceneName.ToLower()),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Found SceneName {0}!", SceneName)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => EndType == ExploreEndType.SceneLeftOrActorFound && SceneId != 0 && SceneIdLeft(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Left SceneId {0}!", SceneId)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => (EndType == ExploreEndType.SceneFound || EndType == ExploreEndType.SceneLeftOrActorFound) && !string.IsNullOrWhiteSpace(SceneName) && SceneNameLeft(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Left SceneName {0}!", SceneName)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Decorator(ret => ZetaDia.IsInTown && !BrainBehavior.IsVendoring,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Cannot use ExploreDungeon in town - tag finished!", SceneName)),
//                        new Action(ret => _isDone = true)
//                    )
//                )
//            );
//        }

//        private static bool GetIsGuardedBountyGizmoActivated(DiaObject actor)
//        {
//            //if (!(actor is DiaGizmo))
//            //{
//            //    return false;
//            //}
//            //if (!DataDictionary.GuardedBountyGizmoIds.Contains(actor.ActorSnoId))
//            //{
//            //    return false;
//            //}
//            //if (actor.CommonData==null || !actor.IsValid || !actor.CommonData.IsValid)
//            //{
//            //    return true;
//            //}
//            //try
//            //{
//            //    return ((DiaGizmo)actor).HasBeenOperated;
//            //}
//            //catch (Exception)
//            //{
//            //    return true;
//            //}
//        }

//        private bool GetIsObjectiveInRange()
//        {
//            if (GetObjectiveMarker() == null)
//                return false;

//            return GetObjectiveMarker().Position.Distance2D(MyPosition) <= MarkerDistance;
//        }

//        private static MinimapMarker PortalExitMarker()
//        {
//            return ZetaDia.Minimap.Markers.CurrentWorldMarkers.FirstOrDefault(m => m.IsPortalExit);
//        }

//        private bool AlternateActorsFound()
//        {
//            return AlternateActors.Any() && ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Any(o => AlternateActors.Any(a => a.ActorId == o.ActorSnoId && o.Distance <= a.ObjectDistance) && !GetIsGuardedBountyGizmoActivated(o));
//        }

//        private bool SceneIdLeft()
//        {
//            return ZetaDia.Me.SceneId != SceneId;
//        }

//        private bool SceneNameLeft()
//        {
//            return !ZetaDia.Me.CurrentScene.Name.ToLower().Contains(SceneName.ToLower()) && AlternateScenes != null && AlternateScenes.Any() && AlternateScenes.All(o => !ZetaDia.Me.CurrentScene.Name.ToLower().Contains(o.SceneName.ToLower()));
//        }

//        private DiaObject GetAlternateActor()
//        {
//            return ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                    .Where(o => AlternateActors.Any(a => a.ActorId == o.ActorSnoId && o.Distance <= a.ObjectDistance)).OrderBy(o => o.Distance).FirstOrDefault();
//        }

//        /// <summary>
//        /// Determine if the tag ExitNameHash is visible in the list of Current World Markers
//        /// </summary>
//        /// <returns></returns>
//        private bool ExitNameHashInRange()
//        {
//            return ZetaDia.Minimap.Markers.CurrentWorldMarkers.Any(m => m.NameHash == ExitNameHash && m.Position.Distance2D(MyPosition) <= MarkerDistance + 10f);
//        }

//        #region PriorityScenes
//        private Vector3 _prioritySceneTarget = Vector3.Zero;
//        private int _prioritySceneSNOId = -1;
//        private Scene _currentPriorityScene;
//        private float _priorityScenePathPrecision = -1f;
//        /// <summary>
//        /// A list of Scene SNOId's that have already been investigated
//        /// </summary>
//        private readonly List<int> _priorityScenesInvestigated = new List<int>();

//        private DateTime _lastCheckedScenes = DateTime.MinValue;

//        private int _stuckCount;

//        /// <summary>
//        /// Will find and move to Prioritized Scene's based on Scene SNOId or Name
//        /// </summary>
//        /// <returns></returns>
//        private Composite PrioritySceneCheck()
//        {
//            return
//            new Decorator(ret => PriorityScenes != null && PriorityScenes.Any(),
//                new Sequence(
//                    new DecoratorContinue(ret => DateTime.UtcNow.Subtract(_lastCheckedScenes).TotalMilliseconds > 1000,
//                        new Sequence(
//                            new Action(ret => _lastCheckedScenes = DateTime.UtcNow),
//                            new Action(ret => FindPrioritySceneTarget())
//                        )
//                    ),
//                    new Decorator(ret => _prioritySceneTarget != Vector3.Zero,
//                        new PrioritySelector(
//                            new Decorator(ret => _stuckCount > 3,
//                                new Sequence(
//                                     new Action(ret => Core.Logger.Log("Too many stuck attempts, canceling Priority Scene {0} {1} center {2} Distance {3:0}",
//                                        _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(MyPosition))),
//                                   new Action(ret => PrioritySceneMoveToFinished())
//                                )
//                            ),
//                            new Decorator(ret => _prioritySceneTarget.Distance2D(MyPosition) <= _priorityScenePathPrecision,
//                                new Sequence(
//                                    new Action(ret => Core.Logger.Log("Successfully navigated to priority scene {0} {1} center {2} Distance {3:0}",
//                                        _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(MyPosition))),
//                                    new Action(ret => PrioritySceneMoveToFinished())
//                                )
//                            ),
//                            new PrioritySelector(
//                                MoveToAndUseDeathGate(),
//                                new Action(ret => MoveToPriorityScene())
//                            )
//                        )
//                    )
//                )
//            );
//        }

//        /// <summary>
//        /// Handles actual movement to the Priority Scene
//        /// </summary>
//        private void MoveToPriorityScene()
//        {

//            if (Navigator.SearchGridProvider.Width == 0)
//            {
//                Core.Logger.Log("Waiting for Navigation Server...");
//            }

//            MoveResult moveResult = NavExtensions.NavigateTo(_prioritySceneTarget);

//            if (moveResult == MoveResult.PathGenerating)
//            {
//                Core.Logger.Log("Waiting for Navigation Server...");
//            }

//            string info = string.Format("Moved to Priority Scene {0} - {1} Center {2} MoveResult: {3}",
//                _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, moveResult);
//            Core.Logger.Debug(info);

//            if (moveResult == MoveResult.PathGenerationFailed || moveResult == MoveResult.ReachedDestination)
//            {
//                Core.Logger.Debug("Unable to navigate to Scene {0} - {1} Center {2} Distance {3:0}, cancelling!",
//                    _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(MyPosition));
//                PrioritySceneMoveToFinished();
//            }
//        }

//        /// <summary>
//        /// Sets a priority scene as finished
//        /// </summary>
//        private void PrioritySceneMoveToFinished()
//        {
//            _priorityScenesInvestigated.Add(_prioritySceneSNOId);
//            _prioritySceneSNOId = -1;
//            _prioritySceneTarget = Vector3.Zero;
//            UpdateRoute();
//        }

//        /// <summary>
//        /// Finds a navigable point in a priority scene
//        /// </summary>
//        private void FindPrioritySceneTarget()
//        {
//            if (PriorityScenes == null)
//                return;

//            if (!PriorityScenes.Any())
//                return;

//            if (_prioritySceneTarget != Vector3.Zero)
//                return;

//            bool foundPriorityScene = false;

//            // find any matching priority scenes in scene manager - match by name or SNOId

//            var scenes = ZetaDia.Scenes.Where(s => s.SceneId == -1);

//            List<Scene> priorityScenes = ZetaDia.Scenes
//                .Where(s => PriorityScenes.Any(ps => ps.SceneId != -1 && ps.SceneId != 0 && s.SceneInfo.SNOId == ps.SceneId)).ToList();

//            priorityScenes.AddRange(ZetaDia.Scenes
//                 .Where(s => PriorityScenes.Any(ps => ps.SceneName.Trim() != String.Empty && ps.SceneId == -1 && s.Name.ToLower().Contains(ps.SceneName.ToLower()))).ToList());

//            List<Scene> foundPriorityScenes = new List<Scene>();
//            Dictionary<int, Vector3> foundPrioritySceneIndex = new Dictionary<int, Vector3>();

//            foreach (Scene scene in priorityScenes)
//            {
//                if (!scene.IsValid)
//                    continue;
//                if (!scene.SceneInfo.IsValid)
//                    continue;
//                if (scene.Mesh.Zone == null)
//                {
//                    Core.Logger.Error("Scene {0} has a null NavZone! SceneGUID={1}", scene.Name, scene.SceneId);
//                }
//                if (!scene.Mesh.Zone.IsValid)
//                    continue;
//                if (!scene.Mesh.Zone.NavZoneDef.IsValid)
//                    continue;

//                if (_priorityScenesInvestigated.Contains(scene.SceneInfo.SNOId))
//                    continue;

//                foundPriorityScene = true;

//                NavZone navZone = scene.Mesh.Zone;
//                NavZoneDef zoneDef = navZone.NavZoneDef;

//                Vector3 zoneCenter = SceneSegmentation.GetNavZoneCenter(navZone);

//                List<NavCell> navCells = zoneDef.NavCells.Where(c => c.IsValid && c.Flags.HasFlag(NavCellFlags.AllowWalk)).ToList();
//                if (!navCells.Any())
//                    continue;

//                NavCell bestCell = navCells.OrderBy(c => SceneSegmentation.GetNavCellCenter(c.Min, c.Max, navZone).Distance2D(zoneCenter)).FirstOrDefault();

//                if (bestCell != null && !foundPrioritySceneIndex.ContainsKey(scene.SceneInfo.SNOId))
//                {
//                    foundPrioritySceneIndex.Add(scene.SceneInfo.SNOId, SceneSegmentation.GetNavCellCenter(bestCell, navZone));
//                    foundPriorityScenes.Add(scene);
//                }
//                else
//                {
//                    Core.Logger.Debug("Found Priority Scene but could not find a navigable point!", true);
//                }
//            }

//            if (foundPrioritySceneIndex.Any())
//            {
//                KeyValuePair<int, Vector3> nearestPriorityScene = foundPrioritySceneIndex.OrderBy(s => s.Value.Distance2D(MyPosition)).FirstOrDefault();

//                _prioritySceneSNOId = nearestPriorityScene.Key;
//                _prioritySceneTarget = nearestPriorityScene.Value;
//                _currentPriorityScene = foundPriorityScenes.FirstOrDefault(s => s.SceneInfo.SNOId == _prioritySceneSNOId);
//                _priorityScenePathPrecision = GetPriorityScenePathPrecision(priorityScenes.FirstOrDefault(s => s.SceneInfo.SNOId == nearestPriorityScene.Key));

//                Core.Logger.Debug("Found Priority Scene {0} - {1} Center {2} Distance {3:0}",
//                    _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(MyPosition));
//            }

//            if (!foundPriorityScene)
//            {
//                _prioritySceneTarget = Vector3.Zero;
//            }
//        }

//        private float GetPriorityScenePathPrecision(Scene scene)
//        {
//            return PriorityScenes.FirstOrDefault(ps => ps.SceneId != 0 && ps.SceneId == scene.SceneInfo.SNOId || scene.Name.ToLower().Contains(ps.SceneName.ToLower())).PathPrecision;
//        }


//        /// <summary>
//        /// Checks to see if the current DungeonExplorer node is in an Ignored scene, and marks the node immediately visited if so
//        /// </summary>
//        /// <returns></returns>
//        private Composite CheckIgnoredScenes()
//        {
//            return
//            new Decorator(ret => /* _timesForcedReset == 0 &&*/ IgnoreScenes != null && IgnoreScenes.Any(),
//                new PrioritySelector(
//                    new Decorator(ret => IsPositionInsideIgnoredScene(CurrentNavTarget),
//                        new Sequence(
//                            new Action(ret => SetNodeVisited("Node is in Ignored Scene"))
//                        )
//                    )
//                )
//            );
//        }

//        private bool IsPositionInsideIgnoredScene(Vector3 position)
//        {
//            return IgnoredAreas.Any(a => a.IsPositionInside(position));
//        }

//        /// <summary>
//        /// Determines if a given Vector3 is in a provided IgnoreScene (if the scene is loaded)
//        /// </summary>
//        /// <param name="position"></param>
//        /// <returns></returns>
//        private bool PositionInsideIgnoredScene(Vector3 position)
//        {
//            List<Scene> ignoredScenes = ZetaDia.Scenes
//                .Where(scn => scn.IsValid && (IgnoreScenes.Any(igscn => !string.IsNullOrWhiteSpace(igscn.SceneName) && scn.Name.ToLower().Contains(igscn.SceneName.ToLower())) ||
//                    IgnoreScenes.Any(igscn => scn.SceneInfo.SNOId == igscn.SceneId) &&
//                    !PriorityScenes.Any(psc => !string.IsNullOrWhiteSpace(psc.SceneName) && scn.Name.ToLower().Contains(psc.SceneName)) &&
//                    !PriorityScenes.Any(psc => psc.SceneId != -1 && scn.SceneInfo.SNOId != psc.SceneId))).ToList();

//            foreach (Scene scene in ignoredScenes)
//            {
//                if (scene.Mesh.Zone == null)
//                    return true;

//                Vector2 pos = position.ToVector2();
//                Vector2 min = scene.Mesh.Zone.ZoneMin;
//                Vector2 max = scene.Mesh.Zone.ZoneMax;

//                if (pos.X >= min.X && pos.X <= max.X && pos.Y >= min.Y && pos.Y <= max.Y)
//                    return true;
//            }
//            return false;
//        }

//        #region Grid and Routes
//        /// <summary>
//        /// Determines if the current node can be marked as Visited, and does so if needed
//        /// </summary>
//        /// <returns></returns>
//        private Composite CheckNodeFinished()
//        {
//            return
//            new PrioritySelector(
//                new Decorator(ret => _lastMoveResult == MoveResult.ReachedDestination,
//                    new Sequence(
//                        new Action(ret => SetNodeVisited("Reached Destination, distance to NavTarget is " + string.Format("{0:0}", CurrentNavTarget.Distance2D(MyPosition)))),
//                        new Action(ret => _lastMoveResult = MoveResult.Moved),
//                        new Action(ret => UpdateRoute())
//                    )
//                ),
//                new Decorator(ret => GridRoute.CurrentNode.Visited,
//                    new Sequence(
//                        new Action(ret => Core.Logger.Debug("Current node was already marked as visited!")),
//                        new Action(ret => GridRoute.CurrentRoute.Dequeue())
//                        )
//                ),
//                new Decorator(ret => GetRouteUnvisitedNodeCount() == 0 || !GridRoute.CurrentRoute.Any(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Debug("Error - CheckIsNodeFinished() called while Route is empty!")),
//                        new Action(ret => UpdateRoute())
//                    )
//                ),
//                new Decorator(ret => CurrentNavTarget.Distance2D(MyPosition) <= PathPrecision,
//                    new Sequence(
//                        new Action(ret => SetNodeVisited(String.Format("Node {0} is within PathPrecision ({1:0}/{2:0})",
//                            CurrentNavTarget, CurrentNavTarget.Distance2D(MyPosition), PathPrecision))),
//                        new Action(ret => UpdateRoute())
//                    )
//                ),
//                new Decorator(ret => CurrentNavTarget.Distance2D(MyPosition) <= 90f && !MainGridProvider.CanStandAt(MainGridProvider.WorldToGrid(CurrentNavTarget.ToVector2())),
//                    new Sequence(
//                        new Action(ret => SetNodeVisited("Center Not Navigable")),
//                        new Action(ret => UpdateRoute())
//                    )
//                ),
//                new Decorator(ret => PositionCache.Cache.Any(p => p.Distance2DSqr(CurrentNavTarget) <= PathPrecision * PathPrecision),
//                    new Sequence(
//                        new Action(ret => SetNodeVisited("Found node to be in position cache, marking done")),
//                        new Action(ret => UpdateRoute())
//                    )
//                ),

//                CheckIgnoredScenes()
//            );
//        }

//        /// <summary>
//        /// Updates the DungeonExplorer Route
//        /// </summary>
//        private void UpdateRoute()
//        {
//            CheckResetDungeonExplorer();

//            GridRoute.Update();
//            PrintNodeCounts("GridRoute.Update");

//            // Throw an exception if this shiz don't work
//            ValidateCurrentRoute();
//        }

//        /// <summary>
//        /// Marks the current dungeon Explorer as Visited and dequeues it from the route
//        /// </summary>
//        /// <param name="reason"></param>
//        private void SetNodeVisited(string reason = "")
//        {
//            if (GetIsInPandemoniumFortress() && CurrentNavTarget != GridRoute.CurrentNode.NavigableCenter)
//            {
//                var dungeonNode = GridRoute.Nodes.FirstOrDefault(n => n.NavigableCenter == CurrentNavTarget);
//                if (dungeonNode != null)
//                    dungeonNode.Visited = true;
//            }
//            else
//            {
//                Core.Logger.Debug("Dequeueing current node {0} - {1}", StringUtils.GetSimplePosition(CurrentNavTarget), reason);
//                GridRoute.SetCurrentNodeExplored();
//            }

//            MarkNearbyNodesVisited();
//            ClearDeathGateCheck();

//            PrintNodeCounts("SetNodeVisited");
//        }

//        public void MarkNearbyNodesVisited()
//        {
//            bool update = false;
//            foreach (var node in GridRoute.Nodes)
//            {
//                if (!node.Visited && node.NavigableCenter.Distance2DSqr(MyPosition) < (PathPrecision * PathPrecision))
//                {
//                    Core.Logger.Debug("Marking nearby node {0} as visited, distance {1:0}/{2:0}, IsVisited={3}",
//                        node.NavigableCenter, node.NavigableCenter.Distance2D(MyPosition), PathPrecision, node.Visited);
//                    node.Visited = true;
//                    update = true;
//                }
//            }
//            if (update)
//            {

//                GridRoute.Update();
//            }
//        }

//        /// <summary>
//        /// Makes sure the current route is not null! Bad stuff happens if it's null...
//        /// </summary>
//        private static void ValidateCurrentRoute()
//        {
//            if (GridRoute.CurrentRoute == null)
//            {
//                Core.Logger.Debug("DungeonExplorer CurrentRoute is null");
//                //throw new ApplicationException("DungeonExplorer CurrentRoute is null");
//            }
//        }

//        /// <summary>
//        /// Prints a plethora of useful information about the Dungeon Exploration process
//        /// </summary>
//        /// <param name="step"></param>
//        private void PrintNodeCounts(string step = "")
//        {
//            if (!QuestToolsSettings.Instance.DebugEnabled)
//                return;

//            var log = String.Format("Nodes [Unvisited: Route:{0} Grid:{1} | Grid-Visited: {2}/{3}] Box:{4}/{5} Step:{6} PP:{7:0} Dir: {8} Current: {9}",
//                GetRouteUnvisitedNodeCount(),                                // 0
//                GetGridSegmentationUnvisitedNodeCount(),                     // 2
//                GetGridRouteVisitedNodeCount(),                      // 1
//                GetGridRouteNodeCount(),                              // 3
//                GridSegmentation.BoxSize,                                    // 4
//                GridSegmentation.BoxTolerance,                               // 5
//                step,                                                        // 6
//                PathPrecision,                                               // 7
//                MathUtil.GetHeadingToPoint(CurrentNavTarget),                // 8
//                StringUtils.GetSimplePosition(CurrentNavTarget)              // 9
//                );

//            Core.Logger.Debug(log);
//        }

//        /*
//         * Dungeon Explorer Nodes
//         */
//        /// <summary>
//        /// Gets the number of unvisited nodes in the DungeonExplorer Route
//        /// </summary>
//        /// <returns></returns>
//        private static int GetRouteUnvisitedNodeCount()
//        {
//            if (GetCurrentRouteNodeCount() > 0)
//                return GridRoute.CurrentRoute.Count(n => !n.Visited);
//            return 0;
//        }

//        /// <summary>
//        /// Gets the number of visited nodes in the DungeonExplorer Route
//        /// </summary>
//        /// <returns></returns>
//        private int GetRouteVisitedNodeCount()
//        {
//            if (GetCurrentRouteNodeCount() > 0)
//                return GridRoute.CurrentRoute.Count(n => n.Visited);
//            return 0;
//        }

//        /// <summary>
//        /// Gets the number of nodes in the DungeonExplorer Route
//        /// </summary>
//        /// <returns></returns>
//        private static int GetCurrentRouteNodeCount()
//        {
//            if (GridRoute.CurrentRoute != null)
//                return GridRoute.CurrentRoute.Count;

//            return 0;
//        }

//        /*
//         *  Grid Segmentation Nodes
//         */
//        /// <summary>
//        /// Gets the number of Unvisited nodes as reported by the Grid Segmentation provider
//        /// </summary>
//        /// <returns></returns>
//        private static int GetGridSegmentationUnvisitedNodeCount()
//        {
//            if (GetGridRouteNodeCount() > 0)
//                return GridRoute.Nodes.Count(n => !n.Visited);

//            return 0;
//        }

//        /// <summary>
//        /// Gets the number of Visited nodes as reported by the Grid Segmentation provider
//        /// </summary>
//        /// <returns></returns>
//        private int GetGridRouteVisitedNodeCount()
//        {
//            if (GetCurrentRouteNodeCount() > 0)
//                return GridRoute.Nodes.Count(n => n.Visited);

//            return 0;
//        }

//        /// <summary>
//        /// Gets the total number of nodes with the current BoxSize/Tolerance as reported by the Grid Segmentation Provider
//        /// </summary>
//        /// <returns></returns>
//        private static int GetGridRouteNodeCount()
//        {
//            if (GridRoute.Nodes != null)
//                return GridRoute.Nodes.Count;

//            return 0;
//        }
//        #endregion

//        private MoveResult _lastMoveResult = MoveResult.Moved;
//        private DateTime _lastGeneratedPath = DateTime.MinValue;
//        #endregion

//        private Vector3 _lastDestination;

//        /// <summary>
//        /// Moves the bot to the next DungeonExplorer node
//        /// </summary>
//        private void MoveToNextNode()
//        {
//            var navTarget = CurrentNavTarget;
//            RecordPosition();

//            if (_lastDestination != navTarget)
//            {
//                Navigator.Clear();
//                ClearDeathGateCheck();

//                Core.Logger.Debug("New Nav Target={0} Dir={1} Dist={2:0}", StringUtils.GetSimplePosition(navTarget), MathUtil.GetHeadingToPoint(navTarget), navTarget.Distance2D(ZetaDia.Me.Position));
//                _lastDestination = navTarget;
//            }

//            if (Navigator.SearchGridProvider.Width == 0)
//            {
//                Core.Logger.Log("Waiting for Navigation Server...");
//            }

//            _lastMoveResult = NavExtensions.NavigateTo(CurrentNavTarget, string.Format("Dungeon Node {0}, Dir={1}", StringUtils.GetSimplePosition(navTarget), MathUtil.GetHeadingToPoint(navTarget)));

//            if (_lastMoveResult == MoveResult.PathGenerating)
//            {
//                Core.Logger.Log("Waiting for Navigation Server...");
//            }
//        }

//        /// <summary>
//        /// Records current position to Position Cache
//        /// </summary>
//        private void RecordPosition()
//        {
//            PositionCache.RecordPosition();

//            MarkNearbyNodesVisited();
//        }

//        /// <summary>
//        /// Initizializes the profile tag and sets defaults as needed
//        /// </summary>
//        private void Init(bool forced = false)
//        {
//            if (BoxSize == 0)
//                BoxSize = 25;

//            if (BoxTolerance < 0.01f)
//                BoxTolerance = 0.01f;

//            if (PathPrecision < 1f)
//                PathPrecision = BoxSize / 2f;

//            const float minPathPrecision = 5f;

//            if (PathPrecision < minPathPrecision)
//                PathPrecision = minPathPrecision;

//            if (ObjectDistance < 10f)
//                ObjectDistance = 10f;

//            if (MarkerDistance < 10f)
//                MarkerDistance = 10f;

//            if (TimeoutValue == 0 && ExploreTimeoutType != TimeoutType.None)
//                TimeoutValue = 1800;

//            PositionCache.Cache = new HashSet<Vector3>();
//            _priorityScenesInvestigated.Clear();
//            MiniMapMarker.KnownMarkers.Clear();
//            if (PriorityScenes == null)
//                PriorityScenes = new List<PrioritizeScene>();

//            if (IgnoreScenes == null)
//                IgnoreScenes = new List<IgnoreScene>();

//            if (AlternateActors == null)
//                AlternateActors = new List<AlternateActor>();

//            _deathGateInteractionCount = new Dictionary<Vector3, int>();

//            if (!forced)
//            {
//                Core.Logger.Debug(
//                    "Initialized ExploreDungeon: boxSize={0} boxTolerance={1:0.00} endType={2} timeoutType={3} timeoutValue={4} " +
//                    "pathPrecision={5:0} sceneId={6} actorId={7} objectDistance={8} markerDistance={9} exitNameHash={10} routeMode={11} direction={12} " +
//                    "setNodesExploredAutomatically={13}",
//                    GridSegmentation.BoxSize, GridSegmentation.BoxTolerance, EndType, ExploreTimeoutType, TimeoutValue,
//                    PathPrecision, SceneId, ActorId, ObjectDistance, MarkerDistance, ExitNameHash, RouteMode, Direction,
//                    SetNodesExploredAutomatically);
//            }
//            _initDone = true;
//        }

//        #region ProfileBehavior
//        private bool _isDone;
//        /// <summary>
//        /// When true, the next profile tag is used
//        /// </summary>
//        public override bool IsDone
//        {
//            get { return !IsActiveQuestStep || _isDone; }
//        }

//        /// <summary>
//        /// Resets this profile tag to defaults
//        /// </summary>
//        public override void ResetCachedDone()
//        {
//            Core.Logger.Debug("ExploreDungeon ResetCachedDone()");
//            _isDone = false;
//            _initDone = false;
//            _timeoutBreached = false;
//            _tagTimer.Reset();
//            MiniMapMarker.KnownMarkers.Clear();
//            base.ResetCachedDone();
//        }
//        #endregion

//        #region Adventure Mode

//        private DateTime _lastCheckRiftDone = DateTime.MinValue;

//        public bool GetIsRiftDone()
//        {
//            if (DateTime.UtcNow.Subtract(_lastCheckRiftDone).TotalSeconds < 1)
//                return false;

//            _lastCheckRiftDone = DateTime.UtcNow;

//            if (ZetaDia.Me.IsInBossEncounter)
//                return false;

//            // X1_LR_DungeonFinder
//            if (ZetaDia.Storage.CurrentWorldType == Act.OpenWorld && DataDictionary.RiftWorldIds.Contains(ZetaDia.Globals.WorldSnoId) &&
//                ZetaDia.Storage.Quests.AllQuests.Any(q => q.QuestSNO == 337492 && q.QuestStep == 10))
//            {
//                Core.Logger.Log("Rift Quest Complete!");
//                return true;
//            }

//            int riftWorldIndex = DataDictionary.RiftWorldIds.IndexOf(ZetaDia.Globals.WorldSnoId);
//            if (riftWorldIndex != -1 &&
//                ZetaDia.Minimap.Markers.CurrentWorldMarkers
//                .Any(m => m.NameHash == DataDictionary.RiftPortalHashes[riftWorldIndex] &&
//                    m.Position.Distance2D(MyPosition) <= MarkerDistance + 10f &&
//                    (Math.Abs(m.Position.Z - ZetaDia.Me.Position.Z) <= 14f || Navigator.Raycast(MyPosition, m.Position)) &&
//                    !MiniMapMarker.TownHubMarkers.Contains(m.NameHash)))
//            {
//                int marker = DataDictionary.RiftPortalHashes[riftWorldIndex];
//                Core.Logger.Log("Rift exit marker {0} within range!", marker);
//                return true;
//            }

//            return false;
//        }

//        private DateTime _lastCheckBountyDone = DateTime.MinValue;
//        public bool GetIsBountyDone()
//        {
//            try
//            {
//                if (DateTime.UtcNow.Subtract(_lastCheckBountyDone).TotalSeconds < 1)
//                    return false;

//                _lastCheckBountyDone = DateTime.UtcNow;

//                // Only valid for Adventure mode
//                if (ZetaDia.Storage.CurrentWorldType != Act.OpenWorld)
//                    return false;

//                // We're in a rift, not a bounty!
//                if (ZetaDia.Storage.CurrentWorldType == Act.OpenWorld && DataDictionary.RiftWorldIds.Contains(ZetaDia.Globals.WorldSnoId))
//                    return false;

//                if (ZetaDia.IsInTown)
//                {
//                    Core.Logger.Log("In Town, Assuming done.");
//                    return true;
//                }

//                if (ZetaDia.Me.IsInBossEncounter)
//                    return false;

//                // Bounty Turn-in
//                if (ZetaDia.Storage.Quests.AllQuests.Any(q => DataDictionary.BountyTurnInQuests.Contains(q.QuestSNO) && q.State == QuestState.InProgress))
//                {
//                    Core.Logger.Log("Bounty Turn-In available, Assuming done.");
//                    return true;
//                }

//                var b = ZetaDia.Storage.Quests.Bounties.FirstOrDefault(q => q.State == QuestState.InProgress && (q.LevelAreas.Contains((SNOLevelArea)ZetaDia.CurrentLevelAreaSnoId) || q.StartingLevelArea == (SNOLevelArea)ZetaDia.CurrentLevelAreaSnoId));
//                if (b == null && ZetaDia.Storage.Quests.ActiveBounty == null)
//                {
//                    Core.Logger.Log("No Active bounty, Assuming done.");
//                    return true;
//                }

//                if (ZetaDia.Storage.Quests.ActiveQuests.Any(q => q.Quest.ToString().ToLower().StartsWith("x1_AdventureMode_BountyTurnin") && q.State == QuestState.InProgress))
//                {
//                    Core.Logger.Log("Bounty Turn-in quest is In-Progress, Assuming done.");
//                    return true;
//                }
//                //If completed or on next step, we are good.
//                if (b.State == QuestState.Completed)
//                {
//                    Core.Logger.Log("Seems completed!");
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                Core.Logger.Log("Exception reading ActiveBounty " + ex.Message);
//            }

//            return false;
//        }
//        #endregion

//        #region Death Gate Handling

//        private Vector3 _lastPandFortressTarget = Vector3.Zero;

//        private void GetPandFortressNavTarget()
//        {
//            if (_lastPandFortressTarget != Vector3.Zero && GridRoute.Nodes.Any(n => n.Visited && n.NavigableCenter == _lastPandFortressTarget))
//            {
//                _lastPandFortressTarget = Vector3.Zero;
//            }

//            List<Tuple<Vector3, Vector2>> nodeList = GridRoute.Nodes
//                .Where(n => !n.Visited)
//                .OrderBy(n => n.Center.DistanceSqr(MyPosition.ToVector2()))
//                .Select(n => new Tuple<Vector3, Vector2>(n.NavigableCenter, n.Center))
//                .ToList();

//            if (!nodeList.Any())
//                return;

//            _lastPandFortressTarget = nodeList
//                .OrderBy(n => n.Item2.DistanceSqr(MyPosition.ToVector2()))
//                .Select(n => n.Item1)
//                .FirstOrDefault();
//        }

//        private static bool GetIsInPandemoniumFortress()
//        {
//            return DataDictionary.PandemoniumFortressWorlds.Contains(ZetaDia.Globals.WorldSnoId) ||
//                DataDictionary.PandemoniumFortressLevelAreaIds.Contains(Player.LevelAreaId);
//        }

//        private Dictionary<Vector3, int> _deathGateInteractionCount = new Dictionary<Vector3, int>();
//        private Vector3 _deathGateInteractStartPosition = Vector3.Zero;
//        private MoveResult _navTargetMoveResult;
//        private Composite MoveToAndUseDeathGate()
//        {
//            return
//            AsyncDecorator(IsValidDeathGates(),
//                new PrioritySelector(
//                    new Decorator(ret => _deathGateInteractStartPosition != Vector3.Zero && MyPosition.Distance2D(_deathGateInteractStartPosition) > 15f,
//                        new Sequence(
//                            new Action(ret => ClearDeathGateCheck()),
//                            new ActionRunCoroutine(ret => CanFullPathToCurrentNavTarget(null, () => SetNodeVisited("Unable to Path")))                            
//                        )
//                    ),
//                    new Decorator(ret => _navTargetMoveResult != MoveResult.ReachedDestination,
//                        new Sequence(
//                            new Action(ret => Core.Logger.Debug("Moving to Current Nav Target {0}", CurrentNavTarget)),
//                            new Action(ret => _navTargetMoveResult = NavExtensions.NavigateTo(CurrentNavTarget, string.Format("Current Nav Target At {0}", CurrentNavTarget)))
//                        )
//                    ),
//                    new Decorator(ret => NearestDeathGate.Position.Distance2DSqr(ZetaDia.Me.Position) > 10f * 10f,
//                        new Sequence(
//                            new Action(ret => Core.Logger.Debug("Moving to Death Gate at {0}", NearestDeathGate.Position)),
//                            new Action(ret => _lastMoveResult = NavExtensions.NavigateTo(NearestDeathGate.Position, string.Format("Nearest Death Gate At {0}", NearestDeathGate.Position)))
//                        )
//                    ),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Debug("Interacting With Death Gate")),
//                        new Action(ret => _deathGateInteractStartPosition = MyPosition),
//                        new Action(ret => AddDeathGateCount()),
//                        new Action(ret => NearestDeathGate.Interact()),
//                        new Sleep(1500)
//                    )
//                )
//            );
//        }

//        public async Task<bool> IsValidDeathGates()
//        {
//            return GetIsInPandemoniumFortress() && AnyDeathGates && !await CanFullPathToCurrentNavTarget();
//        }

//        public Composite AsyncDecorator(Task<bool> condition, Composite child)
//        {
//            return new ActionRunCoroutine(ret => condition).ExecuteCoroutine().Result ? child : new Action(ret => RunStatus.Failure);
//        }


//        //private async Task<bool> CanFullPathToCurrentNavTarget(System.Action onSuccess, System.Action onFailure)
//        //{
//        //    bool result;
//        //    var navTarget = CurrentNavTarget;
//        //    var tooFarAway = navTarget.Distance2D(ZetaDia.Me.Position) > 500;
//        //    var invalidTarget = navTarget == Vector3.Zero;
//        //    var alreadyCheckedTarget = _lastPathCheckTarget == navTarget;

//        //    if (!invalidTarget && !alreadyCheckedTarget)
//        //    {
//        //        _lastPathCheckTarget = navTarget;
//        //        Core.Logger.Debug("Checking Path to navTarget {0}", navTarget);
//        //        _lastPathCheckResult = await NavigationProvider.CanPathWithinDistance(navTarget, PathPrecision);

//        //        if (!_lastPathCheckResult)
//        //            Core.Logger.Debug("Unable to fully path to {0} with precision {1}, distance {2:0}", navTarget, PathPrecision, navTarget.Distance2D(ZetaDia.Me.Position));
//        //    }

//        //    result = !tooFarAway && _lastPathCheckResult;
            
//        //    if (result && onSuccess != null)
//        //        onSuccess();

//        //    if (!result && onFailure != null)
//        //        onFailure();

//        //    return result;
//        //}

//        private void AddDeathGateCount()
//        {
//            if (_deathGateInteractionCount.ContainsKey(NearestDeathGate.Position))
//                _deathGateInteractionCount[NearestDeathGate.Position]++;
//            else
//            {
//                _deathGateInteractionCount.Add(NearestDeathGate.Position, 1);
//            }
//        }

//        private int GetDeathGateInteractionCount(DiaObject gate)
//        {
//            if (!_deathGateInteractionCount.ContainsKey(gate.Position))
//                return 0;
//            return _deathGateInteractionCount[gate.Position];
//        }

//        /// <summary>
//        /// Resets all variables used with checking, moving to, and interacting with Death Gates
//        /// </summary>
//        private void ClearDeathGateCheck()
//        {
//            if (!GetIsInPandemoniumFortress())
//                return;

//            Core.Logger.Debug("Clearing Death Gate Check");
//            _deathGateInteractStartPosition = Vector3.Zero;
//            _navTargetMoveResult = default(MoveResult);
//            _canPathCache = new Dictionary<Vector3, bool>();
//            _lastPathCheckTarget = Vector3.Zero;
//            _nearestDeathGate = null;
//            _lastPandFortressTarget = Vector3.Zero;
//            NavigationProvider.Clear();
//            UpdateRoute();
//        }

//        private Dictionary<Vector3, bool> _canPathCache = new Dictionary<Vector3, bool>();
//        private DiaObject _nearestDeathGate;
//        public DiaObject NearestDeathGate
//        {
//            get
//            {
//                if (!GetIsInPandemoniumFortress())
//                    return null;

//                if (_nearestDeathGate != null && _nearestDeathGate.IsValid)
//                    return _nearestDeathGate;

//                var deathgates =
//                    (from o in ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
//                     where o.IsValid && DataDictionary.DeathGates.Contains(o.ActorSnoId) && o.Distance < 500 &&
//                     !_canPathCache.ContainsKey(o.Position)
//                     orderby GetDeathGateInteractionCount(o), o.Position.Distance2D(CurrentNavTarget)
//                     select o).ToList();

//                if (!deathgates.Any())
//                    return _nearestDeathGate;

//                foreach (var portal in deathgates.Where(p => !_canPathCache.ContainsKey(p.Position)))
//                {
//                    Core.Logger.Verbose("Checking Path to portal {0}", portal.Position);
//                    _canPathCache.Add(portal.Position, NavExtensions.CanPathWithinDistance(portal.Position, 10f));
//                }

//                if (QuestToolsSettings.Instance.DebugEnabled)
//                {
//                    string debugNoise = deathgates.Aggregate("Found DeathGates: ",
//                        (current, p) => current + string.Format("\n{0} distance: {1:0}, distance to node: {2:0} canPath: {3}",
//                            StringUtils.GetProfileCoordinates(p.Position), p.Distance, p.Position.Distance2D(CurrentNavTarget),
//                            _canPathCache[p.Position]));
//                    Core.Logger.Debug(debugNoise);
//                }

//                _nearestDeathGate = deathgates.FirstOrDefault(p => _canPathCache[p.Position]);
//                return _nearestDeathGate;
//            }
//        }

//        public bool AnyDeathGates
//        {
//            get
//            {
//                return NearestDeathGate != null && NearestDeathGate.IsValid;

//            }
//        }
//        #endregion

//        public DefaultNavigationProvider NavigationProvider
//        {
//            get
//            {
//                return Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();
//            }
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

///*
// * Never need to call GridSegmentation.Update()
// * GridSegmentation.Reset() is automatically called on world change
// * DungeonExplorer.Reset() will reset the current route and revisit nodes
// * DungeonExplorer.Update() will update the current route to include new scenes
// */
