using System;
using Zeta.Common;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public class WorldSceneCell
    {
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }
        public NavCellFlags NavCellFlags { get; private set; }
        public bool IsWalkable { get; private set; }

        public float Z { get; private set; }

        public WorldSceneCell(NavCell navCell, Vector2 zoneMin)
        {
            MinX = zoneMin.X + navCell.Min.X;
            MinY = zoneMin.Y + navCell.Min.Y;
            MaxX = zoneMin.X + navCell.Max.X;
            MaxY = zoneMin.Y + navCell.Max.Y;
            NavCellFlags = navCell.Flags;
            IsWalkable = NavCellFlags.HasFlag(NavCellFlags.AllowWalk);
            
            Z = Math.Max(navCell.Min.Z, navCell.Max.Z);
        }

        public bool IsInCell(float x, float y)
        {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }
    }
}