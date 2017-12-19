using System;
using System.Collections.Generic;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Avoidance.Structures
{
    public class AvoidancePart
    {
        /// <summary>
        /// Friendly name of the actor
        /// </summary>
        public string Name;

        /// <summary>
        /// Id of the actor
        /// </summary>
        public int ActorSnoId;

        /// <summary>
        /// Role of this component
        /// </summary>
        public PartType Type;

        /// <summary>
        /// Containing Avoidance Data.
        /// </summary>
        public AvoidanceDefinition Parent;

        /// <summary>
        /// If / how this moves
        /// </summary>
        public MovementType MovementType;

        /// <summary>
        /// The actual distance of danger without padding for settings/movement
        /// A factor of collision radius would be best but some avoidance
        /// report this incorrectly for their effect and so require a custom radius.
        /// </summary>
        public float Radius;

        /// <summary>
        /// How long this part should be avoided for.
        /// </summary>
        public TimeSpan Duration = TimeSpan.Zero;

        /// <summary>
        /// How long after creation does this part occur.
        /// </summary>
        public TimeSpan Delay = TimeSpan.Zero;

        /// <summary>
        /// Name used within D3 for SNOActor enum
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// The associated affix on the monster who casts this.
        /// </summary>
        public MonsterAffixes Affix { get; set; }

        public ActorAttributeType Attribute { get; set; }

        public SNOPower Power { get; set; }

        public SNOAnim Animation { get; set; }

        public List<SNOAnim> Animations { get; set; }

        public float AngleDegrees { get; set; }

        public Func<TrinityActor, bool> Filter { get; set; }
    }
}