using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;
using Zeta.Common;

namespace Trinity.Framework.Avoidance.Handlers
{
    internal class ArcaneAvoidanceHandler : BaseAvoidanceHandler
    {
        private static readonly Dictionary<int, Rotator> _rotators = new Dictionary<int, Rotator>();

        public override bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = Core.Actors.RactorByRactorId<TrinityActor>(avoidance.RActorId);
            if (actor == null || !actor.IsValid)
                return false;

            CleanUpRotators();

            var part = avoidance.Definition.GetPart(actor.ActorSnoId);

            if (part?.MovementType == MovementType.Rotation)
            {
                Rotator rotator;
                if (!_rotators.TryGetValue(actor.RActorId, out rotator))
                {
                    rotator = CreateNewRotator(actor);
                    _rotators.Add(actor.RActorId, rotator);
                    Task.FromResult(rotator.Rotate());
                }

                var centerNodes = grid.GetNodesInRadius(actor.Position, 8f);
                var radAngle = MathUtil.ToRadians(rotator.Angle);
                var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 28f, radAngle)).SelectMany(n => n.AdjacentNodes).ToList();

                var futureRadAngle = MathUtil.ToRadians((float)rotator.GetFutureAngle(TimeSpan.FromMilliseconds(500)));
                nodes.AddRange(grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 28f, futureRadAngle)).SelectMany(n => n.AdjacentNodes));
                nodes.AddRange(centerNodes);
                nodes = nodes.Distinct().ToList();

                const int defaultWeightModification = 32;
                HandleNavigationGrid(grid, nodes, avoidance, actor, 0f, defaultWeightModification);
            }
            else
            {
                var telegraphNodes = grid.GetNodesInRadius(actor.Position, 12f);

                const int defaultWeightModification = 12;
                HandleNavigationGrid(grid, telegraphNodes, avoidance, actor, 0f, defaultWeightModification);
            }

            Core.DBGridProvider.AddCellWeightingObstacle(actor.RActorId, ObstacleFactory.FromActor(actor));
            return true;
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