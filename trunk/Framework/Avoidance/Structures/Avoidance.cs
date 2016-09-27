﻿using System;
using System.Collections.Generic;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Settings;
using Trinity.Objects;
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
        public bool IsAllowed => Settings.IsEnabled && Core.Player.CurrentHealthPct*100 < Settings.HealthPct;
    }
}
