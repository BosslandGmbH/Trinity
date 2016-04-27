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
    internal class CacheObstacleObject
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public int ActorSNO { get; set; }
        public double Weight { get; set; }
        public string Name { get; set; }
        public int HitPointsCurPct { get; set; }
        public int HitPointsCur { get; set; }
        public DateTime Expires { get; set; }
        public int RActorGUID { get; set; }
        public float Rotation { get; set; }
        public float BeamLength { get; set; }
        public int ACDGuid { get; set; }

        public List<SNOAnim> AvoidanceAnimations { get; set; }
        public float DirectionalAvoidanceDegrees { get; set; }
        public bool AvoidAtPlayerPosition { get; set; }
        public TrinityObjectType ObjectType { get; set; }

        public AvoidanceType AvoidanceType
        {
            get
            {
                return OldAvoidanceManager.GetAvoidanceType(ActorSNO);
            }
        }

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

        public static Dictionary<int, Rotator> Rotators = new Dictionary<int, Rotator>();

        private Rotator _rotator;
        public Rotator Rotator
        {
            get { return _rotator ?? (_rotator = GetRotator(RActorGUID)); }
        }        

        public static bool CreateRotator(int rActorGuid, Rotator rotator)
        {
            foreach (var r in Rotators.Where(r => DateTime.UtcNow.Subtract(r.Value.StartTime).TotalSeconds > 10).Select(r => r.Key).ToList())
            {
                Rotators.Remove(r);
            }
           
            if (!Rotators.ContainsKey(rActorGuid))
            {
                Rotators.Add(rActorGuid, rotator);
                Task.FromResult(rotator.Rotate());
                return true;
            }
            return false;
        }

        public static Rotator GetRotator(int rActorGuid)
        {
            Rotator rotator;
            if (Rotators.TryGetValue(rActorGuid, out rotator))
                return rotator;

            return null;
        }
    }
}
