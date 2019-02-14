using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot.Navigation;

namespace Trinity.Framework.Avoidance
{
    public static class ObstacleFactory
    {
        public static ObstacleData FromActor(TrinityActor actor, float radius = 0f)
        {
            return new ObstacleData()
            {
                GizmoType = actor.GizmoType,
                ActorSno = actor.ActorSnoId,
                Position = actor.Position,
                Radius = radius == 0f ? actor.CollisionRadius : radius,
                IsDestructable = actor.IsGizmoDestructable
            };
        }
    }
}
