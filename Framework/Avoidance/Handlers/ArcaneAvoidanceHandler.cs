using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Common;

namespace Trinity.Framework.Avoidance.Handlers
{
    internal class ArcaneAvoidanceHandler : IAvoidanceHandler
    {
        private static Dictionary<int, Rotator> _rotators = new Dictionary<int, Rotator>();

        public void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            CleanUpRotators();

            foreach (var actor in avoidance.Actors)
            {
                if (actor == null || !actor.IsValid || actor.IsDead || actor.CommonData == null || actor.CommonData.IsDisposed)
                {
                    continue;
                }

                var part = avoidance.Definition.GetPart(actor.ActorSnoId);

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
                    var radAngle = MathUtil.ToRadians(rotator.Angle);
                    var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 26f, radAngle)).SelectMany(n => n.AdjacentNodes).ToList();

                    var futureRadAngle = MathUtil.ToRadians((float)rotator.GetFutureAngle(TimeSpan.FromMilliseconds(500)));
                    nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 26f, futureRadAngle)).SelectMany(n => n.AdjacentNodes));
                    nodes.AddRange(centerNodes);
                    nodes = nodes.Distinct().ToList();
                    grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 30);
                }
                else
                {
                    var telegraphNodes = grid.GetNodesInRadius(actor.Position, 10f);
                    grid.FlagAvoidanceNodes(telegraphNodes, AvoidanceFlags.Avoidance, avoidance, 10);
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
            return new Rotator
            {
                RotateDuration = TimeSpan.FromSeconds(10),
                RotateAmount = 360,
                RotateAntiClockwise = actor.InternalName.ToLower().Contains("_reverse"),
                DebugLogging = false,
                StartAngleDegrees = actor.RotationDegrees
            };
        }
    }
}