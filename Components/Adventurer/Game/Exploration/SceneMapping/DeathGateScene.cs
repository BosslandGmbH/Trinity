using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Cache;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Game.Exploration.SceneMapping
{
    public class DeathGateScene
    {
        public string Name { get; set; }
        public int SnoId { get; set; }
        public DeathGateType Type { get; set; }
        public Vector3 RelativeExitPosition { get; set; }
        public Vector3 RelativeEnterPosition { get; set; }
        public float Depth => DeepPortalPosition.X + DeepPortalPosition.Y;
        public RegionGroup Regions { get; set; } = new RegionGroup();
        public WorldScene WorldScene => ScenesStorage.CurrentWorldScenes.FirstOrDefault(s => s.SnoId == SnoId);
        public Vector3 DeepPortalPosition => WorldScene?.GetWorldPosition(RelativeExitPosition) ?? Vector3.Zero;
        public Vector3 ShallowPortalPosition => WorldScene?.GetWorldPosition(RelativeEnterPosition) ?? Vector3.Zero;
        public List<Vector3> PortalPositions => new List<Vector3> { DeepPortalPosition, ShallowPortalPosition };
        public Vector3 ClosestGateToPosition(Vector3 position) => PortalPositions.OrderBy(p => p.Distance2D(position)).FirstOrDefault();
        public Vector3 FarthestGateFromPosition(Vector3 position) => PortalPositions.OrderByDescending(p => p.Distance2D(position)).FirstOrDefault();
        public bool IsValid => WorldScene != null;
        public float Distance => ShallowPortalPosition.Distance(AdvDia.MyPosition);
        public override string ToString() => Name + SnoId + Type;
    }
}