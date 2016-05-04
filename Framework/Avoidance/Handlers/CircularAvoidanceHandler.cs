using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.UIComponents;
using TrinityCoroutines.Resources;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance.Handlers
{
    [DataContract(Namespace = "")]
    public class CircularAvoidanceHandler : NotifyBase, IAvoidanceHandler
    {
        private int _healthThresholdPct;
        private float _distanceMultiplier;

        public bool IsAllowed => TrinityPlugin.Player.CurrentHealthPct <= HealthThresholdPct;

        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                var part = avoidance.Data.GetPart(actor.ActorSNO);
                var radius = Math.Max(part.Radius, actor.Radius);
                var finalRadius = radius*DistanceMultiplier;
                var nodes = grid.GetNodesInRadius(actor.Position, finalRadius);

                if (part.Severity == Severity.Extreme)
                {
                    TrinityPlugin.MainGridProvider.AddCellWeightingObstacle(actor.ActorSNO, finalRadius);

                    foreach (var node in nodes.Where(node => node != null && node.AvoidanceFlags.HasFlag(AvoidanceFlags.AllowWalk)))
                    {
                        node.Weight += 50;
                        node.AddNodeFlags(AvoidanceFlags.CriticalAvoidance);
                        node.AddNodeFlags(AvoidanceFlags.Avoidance);
                    }
                }
                else
                {
                    foreach (var node in nodes.Where(node => node != null && node.AvoidanceFlags.HasFlag(AvoidanceFlags.AllowWalk)))
                    {
                        node.Weight += 10;
                        node.AddNodeFlags(AvoidanceFlags.Avoidance);
                    }
                }
            
            }
        }

        [DataMember]
        [Setting, DefaultValue(100)]
        [UIControl(UIControlType.Slider), Limit(1, 100)]
        [Description("Player health must be below value for this to be avoided (Default=100)")]
        public int HealthThresholdPct
        {
            get { return _healthThresholdPct; }
            set { SetField(ref _healthThresholdPct, value); }
        }

        [DataMember]
        [Setting, DefaultValue(1)]
        [UIControl(UIControlType.Slider), Limit(0, 2)]
        [Description("The safe distance from this avoidance is multiplied by value (Default=1)")]
        public float DistanceMultiplier
        {
            get { return _distanceMultiplier; }
            set { SetField(ref _distanceMultiplier, value); }
        }

    }
}




