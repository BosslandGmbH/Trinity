using Zeta.Common;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Adventurer.Game.Grid
{
    public class GridNode
    {
        private Vector2 _center;
        private NavCellDefinition _navCellDefinition;
        private GridScene _gridScene;

        public Vector2 Center { get; private set; }
        public Vector3 NavigableCenter { get { return new Vector3(Center.X, Center.Y, GetHeight()); } }
        public NavCellFlags Flags { get { return _navCellDefinition.Flags; } }
        public GridPoint GridPoint { get; set; }

        private GridNode() { }

        public static GridNode Create(Vector2 center, GridScene gridScene, NavCellDefinition navCell)
        {
            var def = new GridNode
            {
                _center = center,
                Center = center + gridScene.Min,

                _navCellDefinition = navCell,
                _gridScene = gridScene
            };
            return def;
        }

        public float GetHeight()
        {
            var dimensions = _navCellDefinition.Max - _navCellDefinition.Min;
            float distanceToMin;
            float totalDistance;
            if (dimensions.X > dimensions.Y)
            {
                distanceToMin = _center.X - _navCellDefinition.Min.X;
                totalDistance = dimensions.X;
            }
            else
            {
                distanceToMin = _center.Y - _navCellDefinition.Min.Y;
                totalDistance = dimensions.Y;
            }
            var ratio = distanceToMin / totalDistance;
            return (_navCellDefinition.Min.Z + _navCellDefinition.Max.Z) * ratio + _gridScene.BaseHeight;
        }
    }
}