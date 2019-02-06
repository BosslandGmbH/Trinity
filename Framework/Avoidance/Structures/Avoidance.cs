using System;
using System.Collections.Generic;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Settings;
using Zeta.Common;

namespace Trinity.Framework.Avoidance.Structures
{
    public class Avoidance
    {
        public DateTime CreationTime { get; set; }
        public AvoidanceDefinition Definition { get; set; }
        public AvoidanceSettingsEntry Settings { get; set; }
        public List<TrinityActor> Actors { get; set; } = new List<TrinityActor>();
        public Vector3 StartPosition { get; set; }
        public bool IsImmune { get; set; }
        public bool IsAllowed => Settings.IsEnabled && Core.Player.CurrentHealthPct * 100 <= Settings.HealthPct;
        public bool IsExpired => DateTime.UtcNow.AddSeconds(-10) > CreationTime;

        public override string ToString()
            => $"{GetType().Name}: {Definition.Name} Ignored={!IsAllowed} Settings({Settings.HealthPct}/{Settings.DistanceMultiplier}) " +
               $"Dist={StartPosition.Distance(Core.Player.Position)} Age={CreationTime - DateTime.UtcNow:g} Immune={IsImmune}";
    }
}