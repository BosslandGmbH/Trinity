using System.Linq;
using Trinity.Framework.Objects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines.Town
{
    public class TownActor : IFindable
    {
        public Vector3 InteractPosition { get; set; }
        public Vector3 Position { get; set; }
        public string InternalName { get; set; }
        public string Name { get; set; }
        public int ActorId { get; set; }
        public Act Act { get; set; }
        public bool IsAdventurerMode { get; set; }
        public int WorldSnoId { get; set; }
        public int LevelAreaId { get; set; }
        public float Distance => ZetaDia.Me.Position.Distance(Position);
        public ServiceType ServiceType { get; set; }
        public bool IsGizmo { get; set; }
        public bool IsUnit { get; set; }
        public DiaObject GetActor() => ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault(o => o.ActorSnoId == ActorId);
        public override string ToString() => $"{Name} Dist={Distance}";
    }
}