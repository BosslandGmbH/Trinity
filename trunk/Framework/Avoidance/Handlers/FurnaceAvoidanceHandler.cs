using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.UIComponents;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Extensions = Zeta.Common.Extensions;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance.Handlers
{
    [DataContract(Namespace = "")]
    internal class FurnaceAvoidanceHandler : NotifyBase, IAvoidanceHandler
    { 
        private int _healthThresholdPct;

        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                if (actor == null)
                    continue;

                var part = avoidance.Data.GetPart(actor.ActorSNO);

                if (actor.IsDead || actor.CommonData == null || !actor.CommonData.IsValid || actor.CommonData.IsDisposed)
                {
                    Logger.LogVerbose("Actor {0} CommonData Invalid ({1})", actor.InternalName, part.Name);
                    continue;
                }

                if (part.Type == PartType.VisualEffect)
                {
                    if (actor.CommonData.GetAttribute<int>(((int)part.Power << 12) + ((int)part.Attribute & 0xFFF)) == 1)
                    {
                        Logger.Log("Power {0} on {1} ({1}) in Attribute {2}", part.Power, actor.InternalName, part.Name, part.Attribute);
                        var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 30f, actor.DiaUnit.Movement.Rotation)).SelectMany(n => n.AdjacentNodes).Distinct();
                        grid.FlagNodes(nodes, AvoidanceFlags.Avoidance, 10);
                    }
                }
                else
                {
                    var obstacleNodes = grid.GetNodesInRadius(actor.Position, part.Radius);
                    grid.FlagNodes(obstacleNodes, AvoidanceFlags.NavigationBlocking, 5);
                }
            }

        }


        public bool IsAllowed
        {
            get { return TrinityPlugin.Player.CurrentHealthPct <= HealthThresholdPct; }
        }

        [DataMember]
        [Setting, DefaultValue(90)]
        [UIControl(UIControlType.Slider), Limit(1, 100)]
        [Description("Player health must be below value for this to be avoided (Default=90)")]
        public int HealthThresholdPct
        {
            get { return _healthThresholdPct; }
            set { SetField(ref _healthThresholdPct, value); }
        }

    }
}

