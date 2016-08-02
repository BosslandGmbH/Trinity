using System;

namespace Trinity.Components.Adventurer.Game.Grid
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
    }
}