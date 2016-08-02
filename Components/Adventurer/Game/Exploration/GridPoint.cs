using System;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public struct GridPoint : IEquatable<GridPoint>
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public GridPoint(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        public bool Equals(GridPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public static bool operator ==(GridPoint first, GridPoint second)
        {
            return first.X == second.X && first.Y == second.Y;
        }

        public static bool operator !=(GridPoint first, GridPoint second)
        {
            return first.X != second.X || first.Y != second.Y;
        }

        static public explicit operator GridPoint(Vector3 vector3)
        {
            return new GridPoint((int)vector3.X, (int)vector3.Y);
        }

        static public implicit operator Vector3(GridPoint point)
        {
            return new Vector3(point.X, point.Y, 0);
        }
    }
}