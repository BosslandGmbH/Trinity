using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Attributes
{
    public class GizmoAttributes : Attributes
    {
        public GizmoAttributes(int groupId) : base(groupId)
        {

        }

        public int GizmoState => GetCachedAttribute<int>(ActorAttributeType.GizmoState);

        public bool IsMinimapActive => GetCachedAttribute<bool>(ActorAttributeType.MinimapActive);

    }
}



