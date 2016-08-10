using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Trinity.Framework.Avoidance.Structures;
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
    internal class PoisonEnchantedAvoidanceHandler : NotifyBase, IAvoidanceHandler
    {
        public PoisonEnchantedAvoidanceHandler()
        {
            base.LoadDefaults();
        }

        private int _healthThresholdPct;

        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                if (actor == null)
                    continue;

                var part = avoidance.Data.GetPart(actor.ActorSnoId);

                try
                {
                    var partIds = new HashSet<int>(avoidance.Data.Parts.Select(p => p.ActorSnoId));

                    if (part.Type == PartType.Telegraph)
                    {
                        var nodes = new List<AvoidanceNode>();
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 60f, (float)(Math.PI / 2))));
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 60f, (float)(2 * Math.PI))));
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 60f, (float)(3 * Math.PI / 2))));
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 60f, (float)(Math.PI))));
                        grid.FlagNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance, 10);
                    }
                    else
                    {
                        //var group = Core.Avoidance.Current.Where(a => partIds.Contains(a.Actors.First().ActorSnoId)
                        //            && a.CreationTime.Subtract(avoidance.CreationTime).TotalMilliseconds < 250
                        //            && a.StartPosition.Distance(avoidance.StartPosition) <= 5f);

                        var nodes = grid.GetRayLineAsNodes(actor.Position, avoidance.StartPosition).SelectMany(n => n.AdjacentNodes);
                        //nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(avoidance.StartPosition, 60f, (float)(2 * Math.PI))));
                        //nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(avoidance.StartPosition, 60f, (float)(3 * Math.PI / 2))));
                        //nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(avoidance.StartPosition, 60f, (float)(Math.PI))));
                        grid.FlagNodes(nodes, AvoidanceFlags.Avoidance, 10);

                        //Logger.Log("Poison Actor {0} ({1}) - GroupCount={2}, CreationTime={3} StartPosition={4}",
                        //    actor.InternalName, actor.RActorId, group.Count(),
                        //    avoidance.CreationTime, avoidance.StartPosition);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception {0}", ex);
                }
            }

        }


        public bool IsAllowed
        {
            get { return Core.Player.CurrentHealthPct <= HealthThresholdPct; }
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

