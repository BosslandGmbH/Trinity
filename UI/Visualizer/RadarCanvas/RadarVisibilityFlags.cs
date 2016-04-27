using System;

namespace Trinity.UI.UIComponents.RadarCanvas
{
    [Flags]
    public enum RadarVisibilityFlags
    {
        None = 0,
        Terrain = 1 << 0,
        SceneInfo = 1 << 1,
        CurrentPath = 1 << 2,
        CurrentTarget = 1 << 3,
        UnwalkableNodes = 1 << 4,
        WalkableNodes = 1 << 5,
        BacktrackNodes = 1 << 6,
        KiteNodes = 1 << 7,
        SafeNodes = 1 << 8,
        Avoidance = 1 << 9,        
        Monsters = 1 << 10,
        Gizmos = 1 << 11,
        RangeGuide = 1 << 12,
        KiteDirection = 1 << 13,
        Clusters = 1 << 14,
        ActivePlayer = 1 << 15,
        Weighting = 1 << 16,
        Misc = 1 << 17,
        Projectile = 1 << 18,
        RiftValue = 1 << 19,
        CombatRadius = 1 << 20,
        NotInCache = 1 << 21,
        All = ~(1 << 22),        
    }
}







