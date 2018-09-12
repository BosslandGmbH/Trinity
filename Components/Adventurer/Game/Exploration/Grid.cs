using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Game.Exploration.Algorithms;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public abstract class Grid<T> : IGrid<T> where T : INode
    {
        public delegate void GridUpdatedEventHandler(object sender, SceneData newNodes);

        //public event GridUpdatedEventHandler Updated;

        public SplitArray<T> InnerGrid;

        public abstract float BoxSize { get; }
        public abstract int GridBounds { get; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public int MinX = int.MaxValue;
        public int MaxX = 0;
        public int MinY = int.MaxValue;
        public int MaxY = 0;
        internal int GridMaxX;
        internal int GridMaxY;
        internal int BaseSize;

        protected Grid()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            Created = DateTime.UtcNow;
            LastUpdated = DateTime.MinValue;
            var worldId = ZetaDia.Globals.WorldId;
      
            Core.Logger.Debug("[{0}] Creating grid [{1},{1}] ZetaWorldId={2} AdvDiaWorldId={3}", GetType().Name, GridBounds, worldId, AdvDia.CurrentWorldDynamicId);

            InnerGrid = new SplitArray<T>(GridBounds, GridBounds);
            WorldDynamicId = AdvDia.CurrentWorldDynamicId;

            var currentScenes = Core.Scenes.CurrentWorldScenes;
            if (currentScenes.Any() && currentScenes.First().DynamicWorldId == worldId)
            {
                Core.Logger.Debug("[{0}] Importing Current World Data from SceneStorage", GetType().Name);
                Update(Core.Scenes.CreateSceneData(currentScenes, currentScenes.First().DynamicWorldId));
            }

            GridStore.Grids.Add(new WeakReference<IGrid>(this));           
        }

        public void Update(ISceneData newSceneData)
        {
            Core.Logger.Debug($"Update called for {this.GetType().Name}");

            var data = newSceneData as SceneData;
            if (data?.WorldDynamicId == AdvDia.CurrentWorldDynamicId)
            {
                LastUpdated = DateTime.UtcNow;
                OnUpdated(data);
            }
        }


        protected virtual void OnUpdated(SceneData newNodes)
        {
        }

        protected void UpdateInnerGrid(IEnumerable<INode> nodes)
        {
            //nodes = nodes.OrderBy(n => n.Center.X).ThenBy(n => n.Center.Y).ToList();

            var worldId = AdvDia.CurrentWorldDynamicId;

            foreach (var node in nodes)
            {                
                if (node.DynamicWorldId != worldId)
                {
                    Core.Logger.Debug("[{0}] A node has different worldId than current world, skipping", GetType().Name);
                    return;
                }
       
                var nodeX = ToGridDistance(node.Center.X);  //(int)Math.Round((node.Center.X - MinX - boxSize / 2) / boxSize);
                var nodeY = ToGridDistance(node.Center.Y);  //(int)Math.Round((node.Center.Y - MinY - boxSize / 2) / boxSize);
                InnerGrid[nodeX, nodeY] = (T)node;
                if (MinX > nodeX) MinX = nodeX;
                if (MaxX < nodeX) MaxX = nodeX;
                if (MinY > nodeY) MinY = nodeY;
                if (MaxY < nodeY) MaxY = nodeY;
                node.GridPoint = new GridPoint(nodeX, nodeY);
            }

            GridMaxX = InnerGrid.GetLength(0);
            GridMaxY = InnerGrid.GetLength(1);
            BaseSize = (int)Math.Round(BoxSize / 4, MidpointRounding.AwayFromZero);
        }

        public T NearestNode => GetNearestNode(Core.Actors.ActivePlayerPosition);

        //public T GetNearestNode(Vector3 position)
        //{
        //    var x = ToGridDistance(position.X);
        //    var y = ToGridDistance(position.Y);

        //    var gridMaxX = InnerGrid.GetLength(0);
        //    var gridMaxY = InnerGrid.GetLength(1);

        //    if (x < 0 || x > gridMaxX) return default(T);
        //    if (y < 0 || y > gridMaxY) return default(T);

        //    return (T)InnerGrid[x, y];
        //}

        public T GetNearestNode(float x, float y)
        {
            return GetNearestNode(new Vector3(x, y, 0));
        }

        public T GetNearestNode(GridPoint gridPoint)
        {
            if (gridPoint.X < 0 || gridPoint.X > GridMaxX) return default(T);
            if (gridPoint.Y < 0 || gridPoint.Y > GridMaxY) return default(T);

            return (T)InnerGrid[gridPoint.X, gridPoint.Y];
        }

        public T GetNearestNode(Vector3 position)
        {
            var x = ToGridDistance(position.X);
            var y = ToGridDistance(position.Y);

            if (x < 0 || x > GridMaxX) return default(T);
            if (y < 0 || y > GridMaxY) return default(T);

            return (T)InnerGrid[x, y];
        }

        public int ToGridDistance(float value)
        {
            return (int)Math.Round((value - BoxSize / 2) / BoxSize, MidpointRounding.AwayFromZero);
        }

        public float ToWorldDistance(int gridValue)
        {
            return gridValue * BoxSize + BoxSize / 2;
        }

        public List<T> GetNeighbors(T node, int distance = 1)
        {
            var neighbors = new List<T>();
            if (node == null)
            {
                return neighbors;
            }

            if (WorldDynamicId != node.DynamicWorldId)
                return neighbors;

            var gridPoint = node.GridPoint;
            if (gridPoint == default(GridPoint)) return neighbors;

            for (var x = gridPoint.X - distance; x <= gridPoint.X + distance; x++)
            {
                if (x < 0 || x > GridMaxX) continue;
                for (var y = gridPoint.Y - distance; y <= gridPoint.Y + distance; y++)
                {
                    if (y < 0 || y > GridMaxY) continue;

                    // Excluding itself
                    if (x == gridPoint.X && y == gridPoint.Y) continue;
                    var gridNode = InnerGrid[x, y];
                    if (gridNode != null)
                    {
                        neighbors.Add((T)gridNode);
                    }
                }
            }
            return neighbors;
        }

        public List<T> GetNodesInRadius(Vector3 center, Func<T, bool> condition, float maxDistance = 30f, float minDistance = 0f)
        {
            var neighbors = new List<T>();
            var node = GetNearestNode(center);
            if (node == null)
                return neighbors;

            // Snapping to nearest grid point causes an offset of the circle retreived from the proper original position.
            // To get an accurate circle of points we need to draw further and measure distance back to v3 center.
            // The max distance, needs to be float so it's measurable between the grid nodes
            // or resulting circle is noticibly big or too small.

            var gridDistanceMin = ToGridDistance(minDistance);
            var gridDistanceMax = ToGridDistance(maxDistance);

            if (gridDistanceMax < 5)
                gridDistanceMax = gridDistanceMax + 1;

            var gridX = ToGridDistance(node.Center.X);
            var gridY = ToGridDistance(node.Center.Y);

            // A rough rounding pass reduces the number of nodes that need their distance measured.
            var isRoughRounded = gridDistanceMax > 10;
            var roundWidth = gridDistanceMax >= 3 ? (int)Math.Round(gridDistanceMax * 0.35, 0, MidpointRounding.AwayFromZero) : 0;
            var edgelength = gridDistanceMax * 2 + 1;
            var v2Center = center.ToVector2();

            var worldDistanceMaxSqr = maxDistance * maxDistance;
            var worldDistanceMinSqr = minDistance * minDistance;

            if (condition != null && condition(node))
                neighbors.Add(node);

            var row = -1;
            for (var x = gridX - gridDistanceMax; x <= gridX + gridDistanceMax; x++)
            {
                row++;

                if (x < 0 || x > GridMaxX)
                    continue;

                var col = 0;
                for (var y = gridY - gridDistanceMax; y <= gridY + gridDistanceMax; y++)
                {
                    col++;
                    if (y < 0 || y > GridMaxY)
                        continue;

                    if (x == gridX && y == gridY)
                        continue;

                    var gridNode = InnerGrid[x, y];
                    if (gridNode != null)
                    {
                        if (gridDistanceMin > 0 && (x > gridX - gridDistanceMin && x < gridX + gridDistanceMin && y > gridY - gridDistanceMin && y < gridY + gridDistanceMin))
                        {
                            if (gridNode.Center.DistanceSqr(v2Center) <= worldDistanceMinSqr)
                                continue;
                        }

                        if (!isRoughRounded || row <= roundWidth || col <= roundWidth || edgelength - row <= roundWidth || edgelength - col <= roundWidth)
                        {
                            if (gridNode.Center.DistanceSqr(v2Center) >= worldDistanceMaxSqr)
                                continue;
                        }

                        if (condition != null && condition((T)gridNode))
                            neighbors.Add((T)gridNode);
                    }
                }
            }
            return neighbors;
        }

        public List<T> GetNodesInRadius(Vector3 center, float radius)
        {
            var node = GetNearestNode(center);
            return node != null ? GetNodesInRadius(node, radius) : new List<T>();
        }

        public List<T> GetNodesInRadius(T node, float radius)
        {
            return GetNodesInRadius(node.Center.ToVector3(), n => true, radius);
        }

        public T GetNodeInDirection(T node, Direction direction)
        {
            if (node == null)
                return default(T);

            var x = ToGridDistance(node.Center.X);
            var y = ToGridDistance(node.Center.Y);

            switch (direction)
            {
                case Direction.West: x -= BaseSize; break;
                case Direction.North: y += BaseSize; break;
                case Direction.East: x += BaseSize; break;
                case Direction.South: y -= BaseSize; break;
                case Direction.NorthWest: x -= BaseSize; y += BaseSize; break;
                case Direction.SouthWest: x -= BaseSize; y -= BaseSize; break;
                case Direction.SouthEast: x += BaseSize; y -= BaseSize; break;
                case Direction.NorthEast: x += BaseSize; y += BaseSize; break;
            }

            if (!IsValidNodePosition(x, y))
                return default(T);

            return (T)InnerGrid[x, y];
        }

        protected IEnumerable<GridPoint> GetRayLine(Vector3 from, Vector3 to)
        {
            var gridFrom = ToGridPoint(from);
            var gridTo = ToGridPoint(to);
            return Bresenham.GetPointsOnLine(gridFrom, gridTo);
        }

        public GridPoint ToGridPoint(Vector3 position)
        {
            var x = ToGridDistance(position.X);
            var y = ToGridDistance(position.Y);
            return new GridPoint(x, y);
        }

        public bool IsValidNodePosition(int x, int y)
        {
            return x > 0 && x < GridMaxX && y > 0 && y < GridMaxY;
        }

        public Vector3 GetWorldPoint(GridPoint gridPoint)
        {
            return new Vector3(ToWorldDistance(gridPoint.X), ToWorldDistance(gridPoint.Y), 0);
        }

        public abstract bool CanRayCast(Vector3 @from, Vector3 to);

        public abstract bool CanRayWalk(Vector3 @from, Vector3 to);

        public abstract void Reset();

        public int WorldDynamicId { get; set; }
    }
}