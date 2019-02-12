using System;
using System.Collections.Generic;
using Trinity.Framework.Avoidance.Settings;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Framework.Avoidance.Structures
{
    public class Avoidance
    {
        public DateTime CreationTime { get; set; }
        public AvoidanceDefinition Definition { get; set; }
        public AvoidanceSettingsEntry Settings { get; set; }
        public int RActorId { get; set; } = -1;
        public SNOActor ActorSno { get; set; } = SNOActor.Invalid;
        public Vector3 StartPosition { get; set; }
        public bool IsImmune { get; set; }
        public bool IsAllowed => Settings.IsEnabled && Core.Player.CurrentHealthPct * 100 <= Settings.HealthPct;
        public bool IsExpired => CreationTime.AddSeconds(10) > DateTime.UtcNow;

        public override string ToString()
            => $"{GetType().Name}: {Definition.Name} Ignored={!IsAllowed} Settings({Settings.HealthPct}/{Settings.DistanceMultiplier}) " +
               $"Dist={StartPosition.Distance(Core.Player.Position)} Age={CreationTime - DateTime.UtcNow:g} Immune={IsImmune}";
    }
}