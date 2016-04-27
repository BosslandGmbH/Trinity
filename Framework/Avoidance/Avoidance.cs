using System;
using System.Collections.Generic;
using Trinity.Framework.Avoidance.Handlers;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Objects;
using Zeta.Common;

namespace Trinity.Framework.Avoidance
{
    public class Avoidance
    {
        public DateTime CreationTime;        
        public AvoidanceData Data;
        public List<IActor> Actors = new List<IActor>();
        public Vector3 StartPosition;
    }
}
