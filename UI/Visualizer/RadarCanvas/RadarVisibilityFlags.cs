using System;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    [Flags]
    public enum RadarVisibilityFlags
    {
        None = 0,
        Terrain = 1 << 0,
        SceneInfo = 1 << 1,
        CurrentPath = 1 << 2,
        CurrentTarget = 1 << 3,
        WalkableNodes = 1 << 4,
        BacktrackNodes = 1 << 5,
        SafeNodes = 1 << 6,
        Avoidance = 1 << 7,        
        Monsters = 1 << 8,
        Gizmos = 1 << 9,
        RangeGuide = 1 << 10,
        KiteDirection = 1 << 11,
        Clusters = 1 << 12,
        ActivePlayer = 1 << 13,
        Weighting = 1 << 14,
        Misc = 1 << 15,
        Projectile = 1 << 16,
        RiftValue = 1 << 17,
        NotInCache = 1 << 18,
        RadarDebug = 1 << 19,
        KiteFromNodes = 1 << 20,
        ExploreNodes = 1 << 21,
        Markers = 1 << 22,
        All = ~(1 << 23),        
    }
}








