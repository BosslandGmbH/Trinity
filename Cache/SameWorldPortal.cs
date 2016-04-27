using System;
using Zeta.Common;

namespace Trinity.Cache
{
    class SameWorldPortal : IEquatable<SameWorldPortal>
    {
        public int ActorSNO { get; set; }
        public int RActorGUID { get; set; }
        public Vector3 StartPosition { get; set; }
        public DateTime LastInteract { get; set; }
        public int WorldID { get; set; }

        public SameWorldPortal()
        {
            StartPosition = Trinity.Player.Position;
            WorldID = Trinity.Player.WorldID;
            LastInteract = DateTime.UtcNow;
        }

        public override bool Equals(object other)
        {
            if (other is SameWorldPortal)
                return Equals((SameWorldPortal)other);
            else
                return false;
        }

        public bool Equals(SameWorldPortal other)
        {
            return this.RActorGUID == other.RActorGUID;
        }

        public override int GetHashCode()
        {
            return this.RActorGUID.GetHashCode();
        }

        public static bool operator ==(SameWorldPortal a, SameWorldPortal b)
        {
            return a.RActorGUID == b.RActorGUID;
        }

        public static bool operator !=(SameWorldPortal a, SameWorldPortal b)
        {
            return a.RActorGUID != b.RActorGUID;
        }
    }
}
