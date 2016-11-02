using System.Runtime.Serialization;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Game.Exploration.SceneMapping
{
    public class RectangularRegion : IWorldRegion
    {
        public static readonly RectangularRegion Zero = new RectangularRegion();

        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }

        public CombineType CombineType { get; set; } = CombineType.Add;

        public RectangularRegion() { }

        public RectangularRegion(int minX, int minY, int maxX, int maxY, CombineType combineType)
        {
            Min = new Vector2(minX, minY);
            Max = new Vector2(maxX, maxY);
            CombineType = combineType;
        }

        public bool Contains(Vector3 position) => position.X >= Min.X && position.X <= Max.X && position.Y >= Min.Y && position.Y <= Max.Y;

        public IWorldRegion Offset(Vector2 min)
        {
            return this + min;
        }

        public static RectangularRegion operator +(RectangularRegion left, Vector2 right)
        {
            var min = left.Min + right;
            var max = left.Max + right;

            return new RectangularRegion
            {
                Min = min,
                Max = max,
                CombineType = left.CombineType
            };
        }

        public override string ToString() => Min + " " + Max;
    }
}