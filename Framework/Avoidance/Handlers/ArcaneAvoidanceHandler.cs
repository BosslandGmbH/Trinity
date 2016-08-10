using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.UIComponents;
using Zeta.Common;
using Extensions = Zeta.Common.Extensions;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance.Handlers
{
    [DataContract(Namespace = "")]
    internal class ArcaneAvoidanceHandler : NotifyBase, IAvoidanceHandler
    {
        public ArcaneAvoidanceHandler()
        {
            base.LoadDefaults();
        }

        private static Dictionary<int,Rotator> _rotators = new Dictionary<int, Rotator>();

        private int _healthThresholdPct;

        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {
            CleanUpRotators();

            foreach (var actor in avoidance.Actors)
            {
                if (actor == null || !actor.IsValid || actor.IsDead || actor.CommonData == null || actor.CommonData.IsDisposed)
                {
                    continue;
                }

                var part = avoidance.Data.GetPart(actor.ActorSnoId);

                if (part.MovementType == MovementType.Rotation)
                {
                    Rotator rotator;
                    if (!_rotators.TryGetValue(actor.RActorId, out rotator))
                    {
                        rotator = CreateNewRotator(actor);
                        _rotators.Add(actor.RActorId, rotator);
                        Task.FromResult(rotator.Rotate());
                    }
                    
                    var centerNodes = grid.GetNodesInRadius(actor.Position, 6f);
                    grid.FlagNodes(centerNodes, AvoidanceFlags.Avoidance, 10);

                    var radAngle = MathUtil.ToRadians(rotator.Angle);
                    var beamNodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 26f, radAngle)).SelectMany(n => n.AdjacentNodes).ToList();
                       
                    var futureRadAngle = MathUtil.ToRadians((float)rotator.GetFutureAngle(TimeSpan.FromMilliseconds(500)));
                    beamNodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 26f, futureRadAngle)).SelectMany(n => n.AdjacentNodes));

                    beamNodes = beamNodes.Distinct().ToList();
                    grid.FlagNodes(beamNodes, AvoidanceFlags.Avoidance, 30);
                }
                else
                {
                    var telegraphNodes = grid.GetNodesInRadius(actor.Position, 10f);
                    grid.FlagNodes(telegraphNodes, AvoidanceFlags.Avoidance, 10);
                }
            }

        }

        private void CleanUpRotators()
        {
            foreach (var rotator in _rotators.ToList().Where(rotator => !rotator.Value.IsRunning))
            {
                _rotators.Remove(rotator.Key);
            }
        }

        private Rotator CreateNewRotator(TrinityActor actor)
        {
            //Logger.Log("Creating New Rotator");

            return new Rotator
            {
                RotateDuration = TimeSpan.FromSeconds(10),
                RotateAmount = 360,
                RotateAntiClockwise = actor.InternalName.ToLower().Contains("_reverse"),
                DebugLogging = false,
                StartAngleDegrees = actor.RotationDegrees
            };
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

