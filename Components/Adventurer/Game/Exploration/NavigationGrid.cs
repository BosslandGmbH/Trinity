using System;
using System.Linq;
using Trinity.Components.Adventurer.Cache;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public sealed class NavigationGrid : Grid<NavigationNode>
    {
        private const int GRID_BOUNDS = 2500;        

        private static Lazy<NavigationGrid> _currentGrid;

        public static NavigationGrid GetWorldGrid(int worldDynamicId)
        {
            if (_currentGrid == null)
            {
                _currentGrid = new Lazy<NavigationGrid>(() => new NavigationGrid());
                return _currentGrid.Value;
            }
  
            if (_currentGrid == null || _currentGrid.Value == null || ZetaDia.WorldId != _currentGrid.Value.WorldDynamicId)
                _currentGrid = new Lazy<NavigationGrid>(() => new NavigationGrid());

            return _currentGrid.Value;
        }        

        public static NavigationGrid Instance
        {
            get { return GetWorldGrid(AdvDia.CurrentWorldDynamicId); }
        }

        public override float BoxSize
        {
            get
            {
                return 2.5f;
                //return ExplorationData.NavigationNodeBoxSize;
            }
        }

        public override int GridBounds
        {
            get { return GRID_BOUNDS; }
        }

        protected bool MarkNodesNearWall
        {
            get { return true; }
        }

        public override void Reset()
        {
            _currentGrid = null;
        }

        public override bool CanRayCast(Vector3 from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowProjectile));
        }

        public override bool CanRayWalk(Vector3 from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk));
        }

        protected override void OnUpdated(SceneData newNodes)
        {
            var nodes = newNodes.ExplorationNodes.SelectMany(n => n.Nodes).ToList();

            UpdateInnerGrid(nodes);

            foreach (var iNode in nodes.Where(n => n.NodeFlags.HasFlag(NodeFlags.AllowWalk)))
            {
                var node = iNode as NavigationNode;
                if(node == null)
                    continue;                

                node.AStarValue = 1;

                if (!MarkNodesNearWall)
                    continue;

                if (GetNeighbors(node).Any(n => (n.NodeFlags & NodeFlags.AllowWalk) == 0))
                {
                    node.NodeFlags |= NodeFlags.NearWall;

                    node.AStarValue = 2;

                    if (node == node.ExplorationNode.NavigableCenterNode)
                    {
                        var newCenterNode = GetNeighbors(node).FirstOrDefault(n =>
                            n.NodeFlags.HasFlag(NodeFlags.AllowWalk) &&
                            !n.NodeFlags.HasFlag(NodeFlags.NearWall));
                        if (newCenterNode != null && newCenterNode is NavigationNode)
                        {
                            node.ExplorationNode.NavigableCenterNode = newCenterNode as NavigationNode;
                        }
                    }
                }
            }

            Util.Logger.Debug("[NavigationGrid] Updated");
        }
    }
}
