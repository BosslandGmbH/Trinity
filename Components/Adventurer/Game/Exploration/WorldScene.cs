using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                ExitDirections = ParseExitDirection(Name);

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

                ExitPositions = ExitDirections.GetFlags<SceneExitDirections>().Select(GetNavigableConnection);
            }
        }

        public IEnumerable<Vector3> ExitPositions { get; set; }

        public Vector2 SouthWest { get; set; }

        public Vector2 SouthEast { get; set; }

        public Vector2 NorthEast { get; set; }

        public Vector2 NorthWest { get; set; }

        public Vector3 GetNavigableConnection(SceneExitDirections direction)
        {
            foreach (var n in Nodes.Where(n => n.HasEnoughNavigableCells && n.IsConnectionNode))
            {
                switch (direction)
                {
                    case SceneExitDirections.North:
                        if (n.TopLeft.X == NorthWest.X)
                            return n.NavigableCenter;
                        break;

                    case SceneExitDirections.East:
                        if (n.BottomLeft.Y == NorthEast.Y)
                            return n.NavigableCenter;
                        break;

                    case SceneExitDirections.South:
                        if (n.BottomRight.X == SouthEast.X)
                            return n.NavigableCenter;
                        break;

                    case SceneExitDirections.West:
                        if (n.TopRight.Y == SouthWest.Y)
                            return n.NavigableCenter;
                        break;
                }
            }
            return Vector3.Zero;
        }

        public Vector2 Size { get; set; }

        internal static Regex SceneNameDirectionRegex = new Regex(@"_([NSEW]+)_", RegexOptions.Compiled);

        private SceneExitDirections ParseExitDirection(string name)
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