using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Helpers;
using Zeta.Common;
using Zeta.Game;

namespace Trinity
{
    // Obstacles for quick mapping of paths etc.
    public class CacheObstacleObject
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public int ActorSNO { get; set; }
        public double Weight { get; set; }
        public string Name { get; set; }
        public DateTime Expires { get; set; }
        public int RActorGUID { get; set; }
        public float Rotation { get; set; }
        public int ACDGuid { get; set; }
        public TrinityObjectType ObjectType { get; set; }

        public CacheObstacleObject()
        {
        }

        public CacheObstacleObject(Vector3 position, float radius, int actorSNO = 0, string name = "")
        {
            Position = position;
            Radius = radius;
            ActorSNO = actorSNO;
            Name = name;
            Expires = DateTime.MinValue;
        }

        public CacheObstacleObject(TrinityCacheObject tco)
        {
            ActorSNO = tco.ActorSNO;
            Radius = tco.Radius;
            Position = tco.Position;
            RActorGUID = tco.RActorGuid;
            ObjectType = tco.Type;
            Name = tco.InternalName;
        }
    }
}
