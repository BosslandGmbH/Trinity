using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Trinity.Components.Adventurer.Cache;
using Zeta.Common;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Game.Grid
{
    public class MainGrid
    {
        private static readonly ConcurrentDictionary<int, Lazy<MainGrid>> WorldGrids = new ConcurrentDictionary<int, Lazy<MainGrid>>();

        public static MainGrid GetWorldGrid(int worldDynamicId)
        {
            return WorldGrids.GetOrAdd(worldDynamicId, new Lazy<MainGrid>(() => new MainGrid())).Value;
        }

        public static MainGrid Instance
        {
            get { return GetWorldGrid(AdvDia.CurrentWorldDynamicId); }
        }

        public static void ResetAll()
        {
            WorldGrids.Clear();
        }

        public GridNode[,] InnerGrid;

        public float BoxSize { get { return GridProvider.GridNodeBoxSize; } }
        public const int GridBounds = 2500;

        public int MinX = int.MaxValue;
        public int MaxX;
        public int MinY = int.MaxValue;
        public int MaxY;
        internal int GridMaxX;
        internal int GridMaxY;
        internal int BaseSize;

        public MainGrid()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            if (InnerGrid == null)
            {
                Util.Logger.Debug("[{0}] Creating grid [{1},{1}]", GetType().Name, GridBounds);
                InnerGrid = new GridNode[GridBounds, GridBounds];
            }
        }

        public void Update(IEnumerable<GridNode> nodes)
        {
            //nodes = nodes.OrderBy(n => n.Center.X).ThenBy(n => n.Center.Y).ToList();
            foreach (var node in nodes)
            {
                var nodeX = ToGridDistance(node.Center.X);
                var nodeY = ToGridDistance(node.Center.Y);
                node.GridPoint = new GridPoint(nodeX, nodeY);
                InnerGrid[nodeX, nodeY] = node;
                if (MinX > nodeX) MinX = nodeX;
                if (MaxX < nodeX) MaxX = nodeX;
                if (MinY > nodeY) MinY = nodeY;
                if (MaxY < nodeY) MaxY = nodeY;
            }

            GridMaxX = InnerGrid.GetLength(0);
            GridMaxY = InnerGrid.GetLength(1);
            BaseSize = (int)Math.Round(BoxSize / 4, MidpointRounding.AwayFromZero);

        }

        public List<GridNode> GetNeighbors(GridNode node, int distance = 1)
        {
            var neighbors = new List<GridNode>();
            if (node == null)
            {
                return neighbors;
            }
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
                        neighbors.Add(gridNode);
                    }
                }
            }
            return neighbors;
        }

        public GridNode GetNearestNodeToPosition(Vector3 position)
        {
            var x = ToGridDistance(position.X);
            var y = ToGridDistance(position.Y);

            var gridMaxX = InnerGrid.GetLength(0);
            var gridMaxY = InnerGrid.GetLength(1);

            if (x < 0 || x > gridMaxX) return default(GridNode);
            if (y < 0 || y > gridMaxY) return default(GridNode);
            return InnerGrid[x, y];
        }

        public int ToGridDistance(float value)
        {
            return (int)Math.Round((value - BoxSize / 2) / BoxSize, MidpointRounding.AwayFromZero);
        }

        public float ToWorldDistance(int gridValue)
        {
            return gridValue * BoxSize + BoxSize / 2;
        }

        public GridPoint ToGridPoint(Vector3 position)
        {
            var x = ToGridDistance(position.X);
            var y = ToGridDistance(position.Y);
            return new GridPoint(x, y);
        }

        public Vector3 GetWorldPoint(GridPoint gridPoint)
        {
            return new Vector3(ToWorldDistance(gridPoint.X), ToWorldDistance(gridPoint.Y), 0);
        }


    }

}
