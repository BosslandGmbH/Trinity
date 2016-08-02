
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.UIComponents;
using TrinityCoroutines.Resources;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance.Handlers
{
    [DataContract(Namespace = "")]
    public class AnimationBeamAvoidanceHandler : NotifyBase, IAvoidanceHandler
    {
        private int _healthThresholdPct;
        private float _distanceMultiplier;

        public bool IsAllowed => Core.Player.CurrentHealthPct <= HealthThresholdPct;

        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                try
                {
                    //Logger.Log($"[BeamAvoidanceHandler] Actor={actor.Name} ActorCurrentAnimation={actor.CurrentAnimation} DiaAnim={actor.CommonData.CurrentAnimation}");

                    var part = avoidance.Data.GetPart(actor.Animation);
                    if (actor.Animation != part.Animation)
                        continue;

                    var radius = Math.Max(part.Radius, actor.Radius) * DistanceMultiplier;
                    var nonCachedRotation = actor.Rotation;
                    var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, radius, nonCachedRotation)).SelectMany(n => n.AdjacentNodes);

                    grid.FlagNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance, 10);
                }
                catch (Exception ex)
                {
                    Logger.LogDebug($"AnimationBeamAvoidanceHandler Exception reading Animation/Rotation for actor: {actor.InternalName}");
                }
            }
        }

        [DataMember]
        [Setting, DefaultValue(100)]
        [UIControl(UIControlType.Slider), Limit(1, 100)]
        [Description("Player health must be below value for this to be avoided (Default=90)")]
        public int HealthThresholdPct
        {
            get { return _healthThresholdPct; }
            set { SetField(ref _healthThresholdPct, value); }
        }

        [DataMember]
        [Setting, DefaultValue(1)]
        [UIControl(UIControlType.Slider), Limit(0, 2)]
        [Description("The safe distance from this avoidance is multiplied by this value (Default=1)")]
        public float DistanceMultiplier
        {
            get { return _distanceMultiplier; }
            set { SetField(ref _distanceMultiplier, value); }
        }

    }
}




