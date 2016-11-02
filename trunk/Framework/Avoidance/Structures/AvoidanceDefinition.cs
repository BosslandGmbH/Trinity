using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Avoidance.Handlers;
using Trinity.Framework.Avoidance.Settings;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Avoidance.Structures
{
    /// <summary>
    /// Represents the permanent properties of a type of avoidance.
    /// </summary>
    public class AvoidanceDefinition 
    {
        public int Id { get; set; }

        /// <summary>
        /// Friendly/Common Name
        /// </summary>
        public string Name { get; set; }

        public string Description { get; set; }

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
        public IAvoidanceHandler Handler { get; set; }

        /// <summary>
        /// Children / Associated parts
        /// </summary>
        public List<AvoidancePart> Parts = new List<AvoidancePart>();

        public Element Element { get; set; }

        public MonsterAffixes Affix { get; set; }

        public string InfoUrl { get; set; }
        public AvoidanceType Type { get; set; }
        public AvoidanceSettingsEntry Defaults { get; set; }
        public string Group { get; set; }

        /// <summary>
        /// Retrieve a part by ActorSnoId
        /// </summary>
        public AvoidancePart GetPart(int actorSnoId)
        {
            return Parts.FirstOrDefault(p => p.ActorSnoId == actorSnoId);
        }

        public AvoidancePart GetPart(SNOAnim actorAnimation)
        {
            return Parts.FirstOrDefault(p => p.Animation == actorAnimation);
        }

    }

}


