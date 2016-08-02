using System;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    [Flags]
    public enum NodeFlags : uint
    {
        None = (uint)0,
        AllowWalk = (uint)1,
        AllowFlier = (uint)2,
        AllowSpider = (uint)4,
        LevelAreaBit0 = (uint)8,
        LevelAreaBit1 = (uint)16,
        NoNavMeshIntersected = (uint)32,
        NoSpawn = (uint)64,
        Special0 = (uint)128,
        Special1 = (uint)256,
        SymbolNotFound = (uint)512,
        AllowProjectile = (uint)1024,
        AllowGhost = (uint)2048,
        RoundedCorner0 = (uint)4096,
        RoundedCorner1 = (uint)8192,
        RoundedCorner2 = (uint)16384,
        RoundedCorner3 = (uint)32768,
        NearWall = (uint)65536,
    }


}
