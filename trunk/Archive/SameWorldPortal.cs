//using System;
//using Zeta.Common;

//namespace Trinity.Cache
//{
//    class SameWorldPortal : IEquatable<SameWorldPortal>
//    {
//        public int ActorSNO { get; set; }
//        public int RActorGUID { get; set; }
//        public Vector3 StartPosition { get; set; }
//        public DateTime LastInteract { get; set; }
//        public int WorldID { get; set; }

//        public SameWorldPortal()
//        {
//            StartPosition = TrinityPlugin.Player.Position;
//            WorldID = TrinityPlugin.Player.WorldSnoId;
//            LastInteract = DateTime.UtcNow;
//        }

//        public override bool Equals(object other)
//        {
//            if (other is SameWorldPortal)
//                return Equals((SameWorldPortal)other);
//            else
//                return false;
//        }

//        public bool Equals(SameWorldPortal other)
//        {
//            return this.RActorId == other.RActorId;
//        }

//        public override int GetHashCode()
//        {
//            return this.RActorId.GetHashCode();
//        }

//        public static bool operator ==(SameWorldPortal a, SameWorldPortal b)
//        {
//            return a.RActorId == b.RActorId;
//        }

//        public static bool operator !=(SameWorldPortal a, SameWorldPortal b)
//        {
//            return a.RActorId != b.RActorId;
//        }
//    }
//}
