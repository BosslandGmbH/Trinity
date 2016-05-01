using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Avoidance.Handlers;

namespace Trinity.Framework.Avoidance.Structures
{
    /// <summary>
    /// Represents the permanent properties of a type of avoidance.
    /// </summary>
    public class AvoidanceData : IAvoidanceSetting
    {
        public AvoidanceData()
        {

        }

        /// <summary>
        /// Friendly/Common Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The GameBalanceId on elites/bosses that can create this avoidance.
        /// </summary>
        public int AffixGbId { get; set; }

        public bool IsEnabledByDefault { get; set; }

        /// <summary>
        /// Distance from center to boundary
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Damage type
        /// </summary>
        public Element DamageType { get; set; }

        /// <summary>
        /// Time between apperance and damage being dealt
        /// </summary>
        public TimeSpan DamageDelay { get; set; }

        /// <summary>
        /// enables doing things with this avoidance
        /// </summary>
        //public Func<IAvoidanceHandler> HandlerProducer { get; set; }
        public IAvoidanceHandler Handler { get; set; }

        /// <summary>
        /// Children / Associated parts
        /// </summary>
        public List<AvoidancePart> Parts = new List<AvoidancePart>();

        /// <summary>
        /// if avoidance will be avoided or not
        /// </summary>
        public bool IsEnabled { get; set; }

        public Element Element { get; set; }

        /// <summary>
        /// Retrieve a part by ActorSnoId
        /// </summary>
        public AvoidancePart GetPart(int actorSnoId)
        {
            return Parts.FirstOrDefault(p => p.ActorSnoId == actorSnoId);
        }

    }

}


