
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
    public class AnimationConeAvoidanceHandler : NotifyBase, IAvoidanceHandler
    {
        public AnimationConeAvoidanceHandler()
        {
            base.LoadDefaults();
        }

        private int _healthThresholdPct;
        private float _distanceMultiplier;
        private float _arcMultiplier;

        public bool IsAllowed => Core.Player.CurrentHealthPct <= HealthThresholdPct;

        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                try
                {
                    Logger.Log($"[BeamAvoidanceHandler] Actor={actor.InternalName} ActorCurrentAnimation={actor.Animation} DiaAnim={actor.Animation}");

                    var part = avoidance.Data.GetPart(actor.Animation);
                    if (actor.Animation != part.Animation)
                        continue;
                    
                    var radius = Math.Max(part.Radius, actor.Radius) * DistanceMultiplier;
                    var nonCachedRotation = actor.Rotation;
                    var arcDegrees = Math.Max(15, part.AngleDegrees) * ArcMultiplier;
                    var nodes = grid.GetConeAsNodes(actor.Position, arcDegrees, radius, nonCachedRotation);
                    grid.FlagNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance, 10);
                }
                catch (Exception ex)
                {
                    Logger.LogDebug($"AnimationConeAvoidanceHandler Exception for Actor: {actor.InternalName}. {ex}");
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
        [Description("The angle of the cone area arc is multiplied by this value (Default=1)")]
        public float ArcMultiplier
        {
            get { return _arcMultiplier; }
            set { SetField(ref _arcMultiplier, value); }
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




