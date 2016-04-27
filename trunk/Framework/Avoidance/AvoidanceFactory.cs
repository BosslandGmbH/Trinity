using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Objects;
using TrinityCoroutines.Resources;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    /// <summary>
    /// Responsible for creating avoidance objects
    /// </summary>
    public class AvoidanceFactory
    {
        public static bool TryCreateAvoidance(List<IActor> actors, IActor actor, out Avoidance avoidance)
        {
            avoidance = null;

            var data = AvoidanceDataFactory.GetAvoidanceData(actor);
            if (data == null)
                return false;

            avoidance = new Avoidance
            {
                Data = data,
                CreationTime = DateTime.UtcNow,
                StartPosition = actor.Position,
                Actors = new List<IActor>() { actor }
            };

            return true;
        }

    }
}



