using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Settings;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;


namespace Trinity.Modules
{
    public class GridEnricher : Module
    {
        public const float GlobeWeightRadiusFactor = 1f;
        public const float MonsterWeightRadiusFactor = 1f;
        public const float GizmoWeightRadiusFactor = 1f;
        public const int MaxDistance = 70;

        public HashSet<int> ActiveAvoidanceSnoIds = new HashSet<int>();

        private readonly HashSet<GizmoType> _flaggedGizmoTypes = new HashSet<GizmoType>
        {
            GizmoType.BreakableChest,
            GizmoType.BreakableDoor,
            GizmoType.Chest,
            GizmoType.DestroyableObject,
            GizmoType.SharedStash,
            GizmoType.Gate,
            GizmoType.HealingWell,
            GizmoType.LootRunSwitch,
            GizmoType.LoreChest,
            GizmoType.PoolOfReflection,
            GizmoType.Switch,
            GizmoType.Door
        };

        private readonly HashSet<GizmoType> _importantGizmoTypes = new HashSet<GizmoType>
        {
            GizmoType.Door,
            GizmoType.Switch,
            GizmoType.BreakableDoor
        };

        public List<AvoidanceNode> CurrentNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> AllNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> NearbyNodes = new List<AvoidanceNode>();
        public IOrderedEnumerable<AvoidanceNode> SafeNodesByWeight = new List<AvoidanceNode>().OrderBy(a => 1);
        public IOrderedEnumerable<AvoidanceNode> SafeNodesByDistance = new List<AvoidanceNode>().OrderBy(a => 1);
        public AvoidanceLayer KiteNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer KiteFromLayer = new AvoidanceLayer();
        public AvoidanceLayer MonsterNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer AvoidanceNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer ObstacleNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer SafeNodeLayer = new AvoidanceLayer();

        // public AvoidanceSetting Settings => Core.Settings.Avoidance;
        public int HighestNodeWeight { get; set; }

        public Vector3 MonsterCentroid { get; set; }
        public Vector3 AvoidanceCentroid { get; set; }
        public AvoidanceNode HighestNode { get; set; }
        public TrinityGrid Grid => Core.Grids.Avoidance;
        public AvoidanceSettings Settings => Core.Avoidance.Settings;

        protected override int UpdateIntervalMs => 200;
        protected override void OnPulse() => UpdateGrid();

        public void UpdateGrid()
        {
            var grid = Core.Grids.Avoidance;

            grid.IsUpdatingNodes = true;

            using (new PerformanceLogger("UpdateGrid"))
            {
                HighestNodeWeight = 0;

                if (grid.NearestNode == null || grid.NearestNode.DynamicWorldId != ZetaDia.Globals.WorldId)
                {
                    Core.Logger.Debug(LogCategory.Avoidance, $"No Player Nearest Node or WorldId Mismatch");
                    return;
                }

                var safeNodes = new AvoidanceLayer();
                var kiteNodes = new AvoidanceLayer();
                var avoidanceNodes = new AvoidanceLayer();
                var monsterNodes = new AvoidanceLayer();
                var obstacleNodes = new AvoidanceLayer();
                var activeAvoidanceSnoIds = new HashSet<int>();
                var kiteFromNodes = new AvoidanceLayer();

                var nodePool = grid.GetNodesInRadius(Core.Player.Position, node => node.IsWalkable, MaxDistance).Select(n => n.Reset()).ToList();
                //var allNodes = Grid.GetNodesInRadius(Core.Player.Position, node => node != null, MaxDistance).ToList();
                var nearestNodes = grid.GetNodesInRadius(Core.Player.Position, node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk), Settings.AvoiderLocalRadius).ToList();
                var weightSettings = Settings.WeightingOptions;

                try
                {
                    if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld)
                        return;

                    if (!nodePool.Any())
                        return;

                    foreach (var obj in Core.Targets)
                    {
                        if (obj.IsMe)
                            continue;

                        UpdateGizmoFlags(obj);
                        UpdateGlobeFlags(obj);
                        UpdateMonsterFlags(obj, monsterNodes);
                        UpdateKiteFromFlags(obj, kiteFromNodes);
                        UpdateProjectileBlockers(obj);
                    }

                    foreach (var door in Core.Actors.AllRActors.Where(a => a.Type == TrinityObjectType.Door))
                    {
                        UpdateDoorFlags(door);
                    }

                    //foreach (var obstacle in CacheData.NavigationObstacles)
                    //{
                    //    UpdateObstacleFlags(obstacle, obstacleNodes);
                    //}

                    //UpdateBacktrackFlags();

                    foreach (var avoidance in Core.Avoidance.CurrentAvoidances)
                    {
                        try
                        {
                            if (!avoidance.Settings.IsEnabled || avoidance.IsImmune)
                                continue;

                            var handler = avoidance.Definition.Handler;
                            if (handler == null)
                            {
                                Core.Logger.Error(LogCategory.Avoidance, $"Avoidance: {avoidance.Definition.Name} has no handler");
                                continue;
                            }

                            if (avoidance.IsAllowed)
                            {
                                handler.UpdateNodes(grid, avoidance);

                                avoidance.Actors.ForEach(a =>
                                {
                                    activeAvoidanceSnoIds.Add(a.ActorSnoId);
                                    Core.DBGridProvider.AddCellWeightingObstacle(a.ActorSnoId, a.CollisionRadius);
                                    //Core.Logger.Warn(LogCategory.Avoidance, $"Avoidance Flagged {a} for {avoidance.Definition.Name}, handler={avoidance.Definition.Handler.GetType().Name}");
                                });
                            }
                            else
                            {
                                //Core.Logger.Warn(LogCategory.Avoidance, $"Avoidance {avoidance.Definition.Name} is not allowed. Enabled={avoidance.Settings.IsEnabled} IsAllowed={avoidance.IsAllowed} PlayerHealth={Core.Player.CurrentHealthPct * 100} SettingsHealth={avoidance.Settings.HealthPct} ");
                            }
                        }
                        catch (Exception ex)
                        {
                            Core.Logger.Error(LogCategory.Avoidance, $"Exception in AvoidanceHandler updating nodes. Name={avoidance.Definition?.Name} Handler={avoidance.Definition?.Handler?.GetType()} {ex} {Environment.StackTrace}");
                        }
                    }

                    AvoidanceCentroid = avoidanceNodes.GetCentroid();
                    MonsterCentroid = monsterNodes.GetCentroid();

                    foreach (var node in nodePool)
                    {
                        if (weightSettings.HasFlag(WeightingOptions.Backtrack) && node.AvoidanceFlags.HasFlag(AvoidanceFlags.Backtrack))
                        {
                            node.Weight--;
                        }

                        if (weightSettings.HasFlag(WeightingOptions.AvoidanceCentroid) && node.NavigableCenter.Distance(AvoidanceCentroid) < 15f)
                        {
                            node.Weight += 2;
                        }

                        if (weightSettings.HasFlag(WeightingOptions.MonsterCentroid) && node.NavigableCenter.Distance(MonsterCentroid) > 15f)
                        {
                            node.Weight--;
                        }

                        // Disabled for performance - 12ms @80yd
                        //if (Grid.IsInKiteDirection(node.NavigableCenter, 70))
                        //{
                        //    node.AddNodeFlags(AvoidanceFlags.Kite);
                        //    node.Weight--;
                        //    kiteNodes.Add(node);
                        //}
                        //else
                        //{
                        //    node.Weight++;
                        //}

                        var anyAdjacentUnsafe = node.AdjacentNodes.Any(n =>
                            !n.NodeFlags.HasFlag(NodeFlags.AllowWalk) ||
                            n.AvoidanceFlags.HasFlag(AvoidanceFlags.Avoidance) ||
                            n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster) ||
                            n.AvoidanceFlags.HasFlag(AvoidanceFlags.KiteFrom));

                        if (!anyAdjacentUnsafe)
                        {
                            if (weightSettings.HasFlag(WeightingOptions.AdjacentSafe))
                                node.Weight--;

                            node.AddNodeFlags(AvoidanceFlags.AdjacentSafe);
                            safeNodes.Add(node);
                        }

                        if (node.Weight > HighestNodeWeight)
                        {
                            HighestNodeWeight = node.Weight;
                            HighestNode = node;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Log("Exception in UpdateGrid {0}", ex);

                    if (ex is CoroutineStoppedException)
                        throw;
                }

                //AllNodes = allNodes;
                CurrentNodes = nodePool; //.OrderBy(n => n.Weight).ThenByDescending(n => n.Distance).ToList();
                SafeNodesByWeight = safeNodes.Nodes.OrderBy(n => n.Weight).ThenByDescending(n => n.Distance);
                SafeNodesByDistance = safeNodes.Nodes.OrderBy(n => n.Distance);
                SafeNodeLayer = safeNodes;
                KiteNodeLayer = kiteNodes;
                KiteFromLayer = kiteFromNodes;
                AvoidanceNodeLayer = avoidanceNodes;
                MonsterNodeLayer = monsterNodes;
                NearbyNodes = nearestNodes;
                ObstacleNodeLayer = obstacleNodes;
                ActiveAvoidanceSnoIds = activeAvoidanceSnoIds;
            }

            grid.IsUpdatingNodes = true;
        }

        private void UpdateGizmoFlags(TrinityActor actor)
        {
            var isGizmo = _flaggedGizmoTypes.Contains(actor.GizmoType);
            if (!isGizmo)
                return;

            var importantGizmo = _importantGizmoTypes.Contains(actor.GizmoType);
            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.Radius * GizmoWeightRadiusFactor))
            {
                node.AddNodeFlags(AvoidanceFlags.Gizmo);

                if (importantGizmo)
                {
                    node.AddNodeFlags(AvoidanceFlags.NoAvoid);

                    if (node.Center.Distance(actor.Position.ToVector2()) < actor.CollisionRadius)
                    {
                        node.AddNodeFlags(AvoidanceFlags.NavigationBlocking);
                    }
                }
            }
        }

        private void UpdateDoorFlags(TrinityActor actor)
        {
            if (actor.Type != TrinityObjectType.Door)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.CollisionRadius))
            {
                // Mark area around closed doors so that monsters behind them can be ignored.
                if ((!actor.IsUsed || actor.IsLockedDoor) && !actor.IsExcludedId)
                {
                    node.AddNodeFlags(AvoidanceFlags.ClosedDoor);
                }
                else
                {
                    node.RemoveNodeFlags(AvoidanceFlags.ClosedDoor);
                }
            }
        }

        public void UpdateMonsterFlags(TrinityActor actor, AvoidanceLayer layer)
        {
            if (actor.ActorType != ActorType.Monster)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.CollisionRadius * MonsterWeightRadiusFactor))
            {
                node.Weight += 2;

                if (actor.IsHostile)
                {
                    node.HostileMonsterCount++;
                }

                node.AddNodeFlags(AvoidanceFlags.Monster);
                layer.Add(node);

                if (node.Center.Distance(actor.Position.ToVector2()) < actor.CollisionRadius)
                    node.AddNodeFlags(AvoidanceFlags.NavigationBlocking);
            }
        }

        public void UpdateKiteFromFlags(TrinityActor actor, AvoidanceLayer layer)
        {
            if (Combat.Routines.Current == null)
            {
                Core.Logger.Debug("UpdateKiteFromFlags failed to update becasue no routine is selected");
                return;
            }

            var kiteMode = Combat.Routines.Current.KiteMode;
            var kiteDistance = Combat.Routines.Current.KiteDistance;

            if (kiteMode == KiteMode.Never)
                return;

            if (actor.ActorType != ActorType.Monster || actor.IsQuestMonster || actor.IsNpc)
                return;

            var kiteFromBoss = kiteMode == KiteMode.Elites && actor.IsBoss;
            var kiteFromElites = kiteMode == KiteMode.Elites && actor.IsElite;
            var kiteAlways = kiteMode == KiteMode.Always;
            var isKiting = kiteFromBoss || kiteFromElites || kiteAlways;
            var weight = 1;

            if (!isKiting)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.CollisionRadius + kiteDistance))
            {
                if (!node.IsWalkable)
                    continue;

                node.Weight = weight;
                node.AddNodeFlags(AvoidanceFlags.KiteFrom);
                layer.Add(node);
            }
        }

        private void UpdateGlobeFlags(TrinityActor actor)
        {
            if (actor.Type != TrinityObjectType.HealthGlobe)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.Radius * GlobeWeightRadiusFactor))
            {
                if (Settings.WeightingOptions.HasFlag(WeightingOptions.Globes))
                    node.Weight -= 6;

                node.AddNodeFlags(AvoidanceFlags.Health);
            }
        }

        private void UpdateProjectileBlockers(TrinityActor actor)
        {
            if (actor.GizmoType != GizmoType.Gate)
                return;

            if (actor.ActorSnoId == 3048) // a2dun_Zolt_Sand_Wall
            {
                var startPoint = MathEx.GetPointAt(actor.Position, 15f, MathEx.WrapAngle((float)(actor.Rotation - Math.PI / 2)));
                var endPoint = MathEx.GetPointAt(actor.Position, 15f, MathEx.WrapAngle((float)(actor.Rotation + Math.PI / 2)));
                var nodes = Grid.GetRayLineAsNodes(startPoint, endPoint);
                Grid.FlagNodes(nodes, AvoidanceFlags.ProjectileBlocking);
            }
        }

        private void UpdateBacktrackFlags()
        {
            foreach (var cachedPos in Core.PlayerHistory.Cache)
            {
                if (cachedPos.WorldId != Core.Player.WorldSnoId)
                    continue;

                var nearestNode = Grid.GetNearestNode(cachedPos.Position);
                if (nearestNode == null)
                    continue;

                var weightChange = Grid.IsInKiteDirection(cachedPos.Position, 90) ? -1 : 0;
                var shouldChangeWeight = Settings.WeightingOptions.HasFlag(WeightingOptions.Globes);

                foreach (var node in nearestNode.AdjacentNodes)
                {
                    if (shouldChangeWeight)
                        node.Weight += weightChange;

                    node.AddNodeFlags(AvoidanceFlags.Backtrack);
                }

                if (shouldChangeWeight)
                    nearestNode.Weight += weightChange;

                nearestNode.AddNodeFlags(AvoidanceFlags.Backtrack);
            }
        }
    }
}