using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Modules;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;


namespace Trinity.Components.Adventurer.Game.Exploration
{
    public class WorldScene
    {
        private readonly float _boxSize;
        private readonly float _boxTolerance;
        public Vector2 Center { get; private set; }

        public List<ExplorationNode> Nodes = new List<ExplorationNode>();
        public string Name { get; private set; }
        public string HashName { get; }
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        //public Rect Rect { get; private set; }
        public int LevelAreaId { get; set; }

        public bool IsIgnored { get; private set; }
        public bool HasParent { get; set; }
        public bool HasChild { get; set; }
        public bool IsTopLevel { get { return !HasParent; } }
        public bool GridCreated { get; private set; }
        public int DynamicWorldId { get; private set; }
        public int SceneId { get; private set; }
        public SceneExitDirections ExitDirections { get; private set; }

        public List<WorldSceneCell> Cells { get; private set; }

        public WorldScene SubScene { get; set; }
        public DateTime GridCreatedTime { get; set; }

        public WorldScene(Scene scene, float boxSize, float boxTolerance)
        {
            using (new PerformanceLogger("[WorldScene] ctor", false))
            {
                //                Core.Logger.Debug("[WorldScene] Scene GridSquare Size: {0} X:{1} Y:{2}", scene.Mesh.Zone.NavZoneDef.GridSquareSize,scene.Mesh.Zone.NavZoneDef.NavGridSquareCountX, scene.Mesh.Zone.NavZoneDef.NavGridSquareCountY);
                _boxSize = boxSize;
                _boxTolerance = boxTolerance;
                //Scene = scene;
                var mesh = scene.Mesh;
                Name = scene.Name;
                HashName = scene.GetSceneNameString();
                SnoId = scene.SceneInfo.SNOId;
                LevelAreaId = mesh.LevelAreaSnoId;
                Min = mesh.Zone.ZoneMin;
                Max = mesh.Zone.ZoneMax;
                Size = Max - Min;
                Center = (Max + Min) / 2;
                //Rect = new Rect(new Point(Center.X, Center.Y), new Size(_boxSize, _boxSize));
                HasChild = mesh.SubSceneId > 0;
                HasParent = mesh.ParentSceneId > 0;
                IsIgnored = ExplorationData.IgnoreScenes.Contains(Name);
                DynamicWorldId = mesh.WorldId;

                NorthWest = Min;
                NorthEast = new Vector2(Min.X, Max.Y);
                SouthEast = Max;
                SouthWest = new Vector2(Max.X, Min.Y);

                if (ExplorationHelpers.BlacklistedPositionsBySceneSnoId.ContainsKey(SnoId))
                    BlacklistedPositions = ExplorationHelpers.BlacklistedPositionsBySceneSnoId[SnoId];

                SceneId = scene.Mesh.SceneId;
                if (HasChild)
                {
                    SubScene = new WorldScene(mesh.SubScene, boxSize, boxTolerance);
                }
                Core.Logger.Verbose("[WorldScene] Created a new world scene. Name: {0} LevelArea: {1} ({2})", Name, (SNOLevelArea)LevelAreaId, LevelAreaId);
                if (LevelAreaId != AdvDia.CurrentLevelAreaId && !ExplorationData.OpenWorldIds.Contains(AdvDia.CurrentWorldId))
                {
                    Core.Logger.Verbose("[WorldScene] The scene LevelAreaID is different than the CurrentLevelAreaID");
                    Core.Logger.Verbose("[WorldScene] Scene Name: {0}", Name);
                    Core.Logger.Verbose("[WorldScene] Scene: {0} ({1})", (SNOLevelArea)LevelAreaId, LevelAreaId);
                    Core.Logger.Verbose("[WorldScene] Current: {0} ({1})", (SNOLevelArea)AdvDia.CurrentLevelAreaId, AdvDia.CurrentLevelAreaId);
                }

                CreateGrid(mesh);

                if (!ExitPositions.Any())
                {
                    ScanExitDirection(SceneExitDirections.North);
                    ScanExitDirection(SceneExitDirections.South);
                    ScanExitDirection(SceneExitDirections.East);
                    ScanExitDirection(SceneExitDirections.West);
                }
            }
        }

        private void ScanExitDirection(SceneExitDirections dir)
        {
            var n = GetNavigableConnection(dir);
            if (n != Vector3.Zero)
            {
                ExitPositions.Add(dir, n);
                ExitDirections |= dir;
            }
        }

        private SceneInfo _sceneInfo;
        public SceneInfo SceneInfo
        {
            get
            {
                if (_sceneInfo == null)
                {
                    SceneMapping.SceneData.SceneDefs.TryGetValue(SnoId, out _sceneInfo);
                }
                return _sceneInfo ?? (_sceneInfo = new SceneInfo());
            }
        }

        private RegionGroup _ignoreRegions;
        public RegionGroup IgnoreRegions
        {
            get
            {
                if (_ignoreRegions == null)
                {
                    _ignoreRegions = SceneInfo.IgnoreRegions.GetOffset(Min) as RegionGroup;
                }
                return _ignoreRegions ?? (_ignoreRegions = new RegionGroup());
            }
        }

        public Dictionary<SceneExitDirections, Vector3> ExitPositions { get; set; } = new Dictionary<SceneExitDirections, Vector3>();

        public Vector2 SouthWest { get; set; }

        public Vector2 SouthEast { get; set; }

        public Vector2 NorthEast { get; set; }

        public Vector2 NorthWest { get; set; }

        public bool IsOnEdge(ExplorationNode n, SceneExitDirections direction)
        {
            switch (direction)
            {
                case SceneExitDirections.North:
                    return n.TopLeft.X == NorthWest.X;
                case SceneExitDirections.East:
                    return n.BottomLeft.Y == NorthEast.Y;
                case SceneExitDirections.South:
                    return n.BottomRight.X == SouthEast.X;
                case SceneExitDirections.West:
                    return n.TopRight.Y == SouthWest.Y;
            }
            return false;
        }


        public Vector3 GetNavigableConnection(SceneExitDirections direction)
        {
            var nodes = Nodes.Where(n => n.IsEdgeNode && IsOnEdge(n, direction) && n.IsConnectionNode).ToList();
            if (!nodes.Any())
                return Vector3.Zero;

            //if (nodes.Count == 2 && nodes.First().IsNextTo(nodes.Last()))
            //{
            //    var start = nodes.First().NavigableCenter;
            //    var end = nodes.Last().NavigableCenter;
            //    return MathEx.CalculatePointFrom(start, end, start.Distance2D(end) / 2);
            //}

            var centerIndex = Math.Max(0, (int)Math.Round((double)nodes.Count / 2, 0)-1);
            var center = nodes.ElementAt(centerIndex);
            return center.NavigableCenter;

            //foreach (var n in Nodes.Where(n => n.HasEnoughNavigableCells && n.IsConnectionNode))
            //{
            //    switch (direction)
            //    {
            //        case SceneExitDirections.North:
            //            if (n.TopLeft.X == NorthWest.X)
            //                return n.NavigableCenter;
            //            break;

            //        case SceneExitDirections.East:
            //            if (n.BottomLeft.Y == NorthEast.Y)
            //                return n.NavigableCenter;
            //            break;

            //        case SceneExitDirections.South:
            //            if (n.BottomRight.X == SouthEast.X)
            //                return n.NavigableCenter;
            //            break;

            //        case SceneExitDirections.West:
            //            if (n.TopRight.Y == SouthWest.Y)
            //                return n.NavigableCenter;
            //            break;
            //    }
            //}
            //return Vector3.Zero;
        }

        public Vector2 Size { get; set; }

        internal static Regex SceneNameDirectionRegex = new Regex(@"_([NSEW]+)_", RegexOptions.Compiled);

        private SceneExitDirections ParseExitDirectionFromName(string name)
        {
            var match = SceneNameDirectionRegex.Match(name);
            var flag = SceneExitDirections.Unknown;
            if (match.Value.Length > 0)
            {
                if (match.Value.Contains("N"))
                    flag |= SceneExitDirections.North;
                if (match.Value.Contains("W"))
                    flag |= SceneExitDirections.West;
                if (match.Value.Contains("S"))
                    flag |= SceneExitDirections.South;
                if (match.Value.Contains("E"))
                    flag |= SceneExitDirections.East;
            }
            return flag;
        }

        public class ConnectedSceneResult
        {
            public WorldScene Scene;
            public SceneExitDirections Direction;
            public Vector2 EdgePointA;
            public Vector2 EdgePointB;
            public Vector3 ExitPosition;
        }

        public IEnumerable<ConnectedSceneResult> ConnectedScenes()
        {
            if (ExitDirections.HasFlag(SceneExitDirections.North))
            {
                yield return new ConnectedSceneResult
                {
                    Scene = Core.Scenes.CurrentWorldScenes.FirstOrDefault(
                        s => s.ExitDirections.HasFlag(SceneExitDirections.South) &&
                             s.SouthWest == NorthWest && s.SouthEast == NorthEast),
                    Direction = SceneExitDirections.North,
                    ExitPosition = ExitPositions[SceneExitDirections.North],
                    EdgePointA = NorthWest,
                    EdgePointB = NorthEast,
                };
            }
            if (ExitDirections.HasFlag(SceneExitDirections.East))
            {
                yield return new ConnectedSceneResult
                {
                    Scene = Core.Scenes.CurrentWorldScenes.FirstOrDefault(
                        s => s.ExitDirections.HasFlag(SceneExitDirections.West) &&
                             s.NorthWest == NorthEast && s.SouthWest == SouthEast),
                    Direction = SceneExitDirections.East,
                    ExitPosition = ExitPositions[SceneExitDirections.East],
                    EdgePointA = NorthEast,
                    EdgePointB = SouthEast,
                };
            }
            if (ExitDirections.HasFlag(SceneExitDirections.South))
            {
                yield return new ConnectedSceneResult
                {
                    Scene = Core.Scenes.CurrentWorldScenes.FirstOrDefault(
                        s => s.ExitDirections.HasFlag(SceneExitDirections.North) &&
                             s.NorthEast == SouthEast && s.NorthWest == SouthWest),
                    Direction = SceneExitDirections.South,
                    ExitPosition = ExitPositions[SceneExitDirections.South],
                    EdgePointA = SouthEast,
                    EdgePointB = SouthWest,
                };
            }
            if (ExitDirections.HasFlag(SceneExitDirections.West))
            {
                yield return new ConnectedSceneResult
                {
                    Scene = Core.Scenes.CurrentWorldScenes.FirstOrDefault(
                        s => s.ExitDirections.HasFlag(SceneExitDirections.East) &&
                             s.SouthEast == SouthWest && s.NorthEast == NorthWest),
                    Direction = SceneExitDirections.West,
                    ExitPosition = ExitPositions[SceneExitDirections.West],
                    EdgePointA = SouthWest,
                    EdgePointB = NorthWest,
                };
            }
        }

        public HashSet<Vector3> BlacklistedPositions { get; set; } = new HashSet<Vector3>();

        public int SnoId { get; set; }

        public Vector3 GetRelativePosition(Vector3 worldPosition)
        {
            var v2Diff = worldPosition.ToVector2() - Min;
            return new Vector3(v2Diff.X, v2Diff.Y, worldPosition.Z);
        }

        public Vector3 GetWorldPosition(Vector3 relativePosition)
        {
            var v2Diff = relativePosition.ToVector2() + Min;
            return new Vector3(v2Diff.X, v2Diff.Y, relativePosition.Z);
        }

        public bool IsInScene(Vector3 position)
        {
            return position.X >= Min.X && position.X <= Max.X && position.Y >= Min.Y && position.Y <= Max.Y;
        }

        public bool IsConnected(Vector3 position)
        {
            var targetScene = Core.Scenes.GetScene(position);
            if (targetScene == Core.Scenes.CurrentScene)
                return true;

            return IsConnected(targetScene);
        }

        public bool IsConnected(WorldScene to)
        {
            var results = TryConnectScenes(this, to);
            if (results == null || !results.Any())
                return false;

            results.ForEach(r => Core.Logger.Verbose(LogCategory.Movement, $"> {r.Scene.Name} {r.Direction} {(ExitPositions.ContainsKey(r.Direction) ? ExitPositions[r.Direction].ToString() : "")}"));
            return true;
        }

        public IEnumerable<ConnectedSceneResult> GetConnectedScenes(WorldScene to)
        {
            var results = TryConnectScenes(this, to);
            if (results == null || !results.Any())
                return Enumerable.Empty<ConnectedSceneResult>();

            return results;
        }

        private static List<ConnectedSceneResult> TryConnectScenes(WorldScene from, WorldScene to, WorldScene parent = null, HashSet<string> sceneHashes = null)
        {
            if (from == null || to == null)
                return null;

            var checkedScenes = sceneHashes ?? new HashSet<string>();

            if (!checkedScenes.Contains(from.HashName))
                checkedScenes.Add(from.HashName);

            foreach (var connection in from.ConnectedScenes().Where(s => s.Scene != parent))
            {
                if (connection.Scene == to)
                    return new List<ConnectedSceneResult> { connection };

                if (connection.Scene == null)
                    continue;

                if (checkedScenes.Contains(connection.Scene.HashName))
                    continue;

                checkedScenes.Add(connection.Scene.HashName);

                var childResults = TryConnectScenes(connection.Scene, to, from, checkedScenes);
                if (childResults == null || !childResults.Any())
                    continue;

                childResults.Insert(0, connection);
                return childResults;
            }
            return null;
        }

        private void CreateGrid(Scene.NavMesh mesh)
        {
            if (GridCreated) return;

            Cells = new List<WorldSceneCell>();

            foreach (var navCell in mesh.Zone.NavZoneDef.NavCells)
            {
                Cells.Add(new WorldSceneCell(navCell, Min));
            }
            if (SubScene != null)
            {
                foreach (var navCell in SubScene.Cells)
                {
                    Cells.Add(navCell);
                }
                if (SubScene.SubScene != null)
                {
                    foreach (var navCell in SubScene.SubScene.Cells)
                    {
                        Cells.Add(navCell);
                    }
                }
            }

            var navBoxSize = ExplorationData.ExplorationNodeBoxSize;
            var searchBeginning = navBoxSize / 2;

            for (var x = Min.X + searchBeginning; x <= Max.X; x = x + navBoxSize)
            {
                for (var y = Min.Y + searchBeginning; y <= Max.Y; y = y + navBoxSize)
                {
                    var navNode = new ExplorationNode(new Vector2(x, y), _boxSize, _boxTolerance, this);
                    Nodes.Add(navNode);
                }
            }

            GridCreated = true;
            GridCreatedTime = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}