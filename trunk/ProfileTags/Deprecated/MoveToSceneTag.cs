//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.Linq; using Trinity.Framework;
//using Trinity.Components.QuestTools.Helpers;
//using Zeta.Bot.Navigation;
//using Zeta.Bot.Profile;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.Game.Internals.SNO;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;
//

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("MoveToScene")]
//    [XmlElement("TrinityMoveToScene")]
//    public class MoveToSceneTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        /// <summary>
//        /// The Scene SNOId
//        /// </summary>
//        [XmlAttribute("sceneId")]
//        public int SceneId { get; set; }

//        /// <summary>
//        /// The Scene Name, will match a sub-string
//        /// </summary>
//        [XmlAttribute("sceneName")]
//        public string SceneName { get; set; }

//        /// <summary>
//        /// The distance the bot will mark the position as visited
//        /// </summary>
//        [XmlAttribute("pathPrecision")]
//        public float PathPrecision { get; set; }

//        /// <summary>
//        /// The current player position
//        /// </summary>
//        private Vector3 myPos { get { return ZetaDia.Me.Position; } }

//        /// <summary>
//        /// The last scene SNOId we entered
//        /// </summary>
//        private int _mySceneId = -1;
//        /// <summary>
//        /// The last position we updated the ISearchGridProvider at
//        /// </summary>
//        private Vector3 _gpUpdatePosition = Vector3.Zero;


//        public MoveToSceneTag() { }

//        /// <summary>
//        /// Called when the profile behavior starts
//        /// </summary>
//        public override void OnStart()
//        {
//            Core.Logger.Log("TrinityMoveToScene OnStart()");

//            if (Math.Abs(PathPrecision) < 1f)
//                PathPrecision = 15f;

//            if (SceneId == 0 && SceneName.Trim() == String.Empty)
//            {
//                Core.Logger.Log("TrinityMoveToScene: No sceneId or sceneName specified!");
//                _isDone = true;
//            }

//        }

//        protected override Composite CreateBehavior()
//        {
//            return
//            new Sequence(
//                UpdateSearchGridProvider(),
//                PrioritySceneCheck()
//            );
//        }

//        /// <summary>
//        /// Will find and move to Prioritized Scene's based on Scene SNOId or Name
//        /// </summary>
//        /// <returns></returns>
//        private Composite PrioritySceneCheck()
//        {
//            return
//            new Decorator(ret => !(SceneId == 0 && string.IsNullOrWhiteSpace(SceneName)),
//                new Sequence(
//                    new DecoratorContinue(ret => DateTime.UtcNow.Subtract(_lastCheckedScenes).TotalMilliseconds > 1000,
//                        new Sequence(
//                            new Action(ret => _lastCheckedScenes = DateTime.UtcNow),
//                            new Action(ret => FindPrioritySceneTarget())
//                        )
//                    ),
//                    new PrioritySelector(
//                        new Decorator(ret => _prioritySceneTarget != Vector3.Zero,
//                            new PrioritySelector(
//                                new Decorator(ret => _prioritySceneTarget.Distance2D(myPos) <= PathPrecision,
//                                    new Action(ret =>
//                                    {
//                                        Core.Logger.Log("Successfully navigated to scene {0} {1} center {2} Distance {3:0}",
//                                            _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(myPos));
//                                        _isDone = true;
//                                    })
//                                ),
//                                new Action(ret => MoveToPriorityScene())
//                            )
//                        ),
//                        new Action(ret =>
//                        {
//                            Core.Logger.Log("Unable to navigate to Scene (not found) cancelling!");
//                            _isDone = true;
//                        })
//                    )
//                )
//            );
//        }
//        /// <summary>
//        /// Handles actual movement to the Priority Scene
//        /// </summary>
//        private void MoveToPriorityScene()
//        {
//            MoveResult moveResult = NavExtensions.NavigateTo(_prioritySceneTarget);

//            Core.Logger.Log("Moved to Scene ({0}) {1} - {2} Center {3} Distance {4:0}",
//                moveResult, _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(myPos));


//            if (moveResult == MoveResult.Failed || moveResult == MoveResult.PathGenerationFailed)
//            {
//                Core.Logger.Log("Unable to navigate to Scene ({0}) {1} - {2} Center {3} Distance {4:0}, cancelling!",
//                    moveResult, _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(myPos));
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
//        }


//        /// <summary>
//        /// Updates the search grid provider as needed
//        /// </summary>
//        /// <returns></returns>
//        private Composite UpdateSearchGridProvider()
//        {
//            return
//            new DecoratorContinue(ret => _mySceneId != ZetaDia.Me.SceneId || Vector3.Distance(myPos, _gpUpdatePosition) > 150,
//                new Sequence(
//                    new Action(ret => _mySceneId = ZetaDia.Me.SceneId),
//                    new Action(ret => _gpUpdatePosition = myPos),
//                    new Action(ret => MiniMapMarker.UpdateFailedMarkers())
//                )
//            );
//        }
//        private Vector3 _prioritySceneTarget = Vector3.Zero;
//        private int _prioritySceneSNOId = -1;
//        private Scene _currentPriorityScene;
//        /// <summary>
//        /// A list of Scene SNOId's that have already been investigated
//        /// </summary>
//        private readonly List<int> _priorityScenesInvestigated = new List<int>();

//        private DateTime _lastCheckedScenes = DateTime.MinValue;
//        /// <summary>
//        /// Finds a navigable point in a priority scene
//        /// </summary>
//        private void FindPrioritySceneTarget()
//        {
//            if (SceneId == 0 && string.IsNullOrWhiteSpace(SceneName))
//                return;

//            if (_prioritySceneTarget != Vector3.Zero)
//                return;

//            bool foundPriorityScene = false;

//            // find any matching priority scenes in scene manager - match by name or SNOId

//            List<Scene> pScenes;
//            var allScenes = ZetaDia.Scenes.ToList();

//            if (allScenes.Any())
//            {
//                pScenes = GetPScenesBySceneId(allScenes);

//                pScenes.AddRange(GetPScenesByName(allScenes));
//            }
//            else
//            {
//                return;
//            }

//            List<Scene> foundPriorityScenes = new List<Scene>();
//            Dictionary<int, Vector3> foundPrioritySceneIndex = new Dictionary<int, Vector3>();

//            foreach (Scene scene in pScenes)
//            {
//                try
//                {
//                    if (_priorityScenesInvestigated.Contains(scene.SceneInfo.SNOId))
//                        continue;

//                    foundPriorityScene = true;

//                    NavZone navZone = scene.Mesh.Zone;
//                    NavZoneDef zoneDef = navZone.NavZoneDef;

//                    Vector3 zoneCenter = GetNavZoneCenter(navZone);

//                    List<NavCell> navCells = zoneDef.NavCells.Where(c => c.Flags.HasFlag(NavCellFlags.AllowWalk)).ToList();

//                    if (!navCells.Any())
//                        continue;

//                    NavCell bestCell = navCells.OrderBy(c => GetNavCellCenter(c.Min, c.Max, navZone).Distance2D(zoneCenter)).FirstOrDefault();

//                    if (bestCell != null)
//                    {
//                        foundPrioritySceneIndex.Add(scene.SceneInfo.SNOId, GetNavCellCenter(bestCell, navZone));
//                        foundPriorityScenes.Add(scene);
//                    }
//                    else
//                    {
//                        Core.Logger.Log("Found Scene but could not find a navigable point!", true);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Core.Logger.Log(ex.ToString());
//                }
//            }

//            if (foundPrioritySceneIndex.Any())
//            {
//                KeyValuePair<int, Vector3> nearestPriorityScene = foundPrioritySceneIndex.OrderBy(s => s.Value.Distance2D(myPos)).FirstOrDefault();

//                _prioritySceneSNOId = nearestPriorityScene.Key;
//                _prioritySceneTarget = nearestPriorityScene.Value;
//                _currentPriorityScene = foundPriorityScenes.FirstOrDefault(s => s.SceneInfo.SNOId == _prioritySceneSNOId);

//                if (_currentPriorityScene != null)
//                {
//                    Core.Logger.Log("Found Scene {0} - {1} Center {2} Distance {3:0}",
//                        _currentPriorityScene.Name, _currentPriorityScene.SceneInfo.SNOId, _prioritySceneTarget, _prioritySceneTarget.Distance2D(myPos));
//                }
//            }

//            if (!foundPriorityScene)
//            {
//                _prioritySceneTarget = Vector3.Zero;
//            }
//        }

//        private List<Scene> GetPScenesByName(IEnumerable<Scene> allScenes)
//        {
//            if (string.IsNullOrWhiteSpace(SceneName))
//                return new List<Scene>();

//            try
//            {
//                var matchingScenes = allScenes.Where(s => s.Name.ToLower().Contains(SceneName.ToLower()));

//                return matchingScenes.ToList();
//            }
//            catch (Exception ex)
//            {
//                Core.Logger.Log(ex.ToString());
//                return new List<Scene>();
//            }
//        }

//        private List<Scene> GetPScenesBySceneId(IEnumerable<Scene> allScenes)
//        {
//            try
//            {
//                return allScenes.Where(s => s.SceneInfo.SNOId == SceneId).ToList();
//            }
//            catch (Exception ex)
//            {
//                Core.Logger.Log(ex.ToString());
//                return new List<Scene>();
//            }
//        }

//        /// <summary>
//        /// Gets the center of a given Navigation Zone
//        /// </summary>
//        /// <param name="zone"></param>
//        /// <returns></returns>
//        private Vector3 GetNavZoneCenter(NavZone zone)
//        {
//            float x = zone.ZoneMin.X + ((zone.ZoneMax.X - zone.ZoneMin.X) / 2);
//            float y = zone.ZoneMin.Y + ((zone.ZoneMax.Y - zone.ZoneMin.Y) / 2);

//            return new Vector3(x, y, 0);
//        }

//        /// <summary>
//        /// Gets the center of a given Navigation Cell
//        /// </summary>
//        /// <param name="cell"></param>
//        /// <param name="zone"></param>
//        /// <returns></returns>
//        private Vector3 GetNavCellCenter(NavCell cell, NavZone zone)
//        {
//            return GetNavCellCenter(cell.Min, cell.Max, zone);
//        }

//        /// <summary>
//        /// Gets the center of a given box with min/max, adjusted for the Navigation Zone
//        /// </summary>
//        /// <param name="min"></param>
//        /// <param name="max"></param>
//        /// <param name="zone"></param>
//        /// <returns></returns>
//        private Vector3 GetNavCellCenter(Vector3 min, Vector3 max, NavZone zone)
//        {
//            float x = zone.ZoneMin.X + min.X + ((max.X - min.X) / 2);
//            float y = zone.ZoneMin.Y + min.Y + ((max.Y - min.Y) / 2);
//            float z = min.Z + ((max.Z - min.Z) / 2);

//            return new Vector3(x, y, z);
//        }

//        private bool _isDone;
//        /// <summary>
//        /// When true, the next profile tag is used
//        /// </summary>
//        public override bool IsDone
//        {
//            get { return !IsActiveQuestStep || _isDone; }
//        }

//        public override void ResetCachedDone()
//        {
//            _isDone = false;
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
