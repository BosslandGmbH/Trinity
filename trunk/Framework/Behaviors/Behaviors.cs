using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Framework.Behaviors
{
    public static class Behaviors
    { 
        public static MoveToMarkerBehavior MoveToMarker { get; set; } = new MoveToMarkerBehavior();

        public static MoveToActorBehavior MoveToActor{ get; set; } = new MoveToActorBehavior();

        public static MoveToInteractBehavior MoveToInteract { get; set; } = new MoveToInteractBehavior();
    }


}
