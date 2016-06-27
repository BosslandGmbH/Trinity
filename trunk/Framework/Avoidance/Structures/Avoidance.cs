using System;
using System.Collections.Generic;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Objects;
using Zeta.Common;

namespace Trinity.Framework.Avoidance.Structures
{
    public class Avoidance
    {
        public DateTime CreationTime;        
        public AvoidanceData Data;
        public List<TrinityActor> Actors = new List<TrinityActor>();
        public Vector3 StartPosition;
        public bool IsImmune;
    }
}
