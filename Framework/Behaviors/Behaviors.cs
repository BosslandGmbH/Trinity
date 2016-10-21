using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Framework.Behaviors
{
    public static class Behaviors
    { 
        public static MoveToMarkerBehavior MoveToMarker { get; } = new MoveToMarkerBehavior();

        public static MoveToActorBehavior MoveToActor{ get; } = new MoveToActorBehavior();

        public static MoveToInteractBehavior MoveToInteract { get; } = new MoveToInteractBehavior();

        public static WaitAfterUnitDeathBehavior WaitAfterUnitDeath { get; } = new WaitAfterUnitDeathBehavior();

    }


}
