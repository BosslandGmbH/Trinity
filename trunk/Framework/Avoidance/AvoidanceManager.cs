using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adventurer.Game.Exploration;
using Buddy.Coroutines;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Objects;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{

    /// <summary>
    ///     Maintains a current list of avoidances and utilities to work with them.
    /// </summary>
    public class AvoidanceManager : Utility
    {
        public AvoidanceManager()
        {
            Avoider = new DefaultAvoider();
        }
    
        public IAvoider Avoider { get; set; }

        public const float GlobeWeightRadiusFactor = 1f;
        public const float MonsterWeightRadiusFactor = 1f;
        public const float AvoidanceWeightRadiusFactor = 1f;
        public const float GizmoWeightRadiusFactor = 1f;
        public const int MaxDistance = 60;

        private readonly Dictionary<int, IActor> _cachedActors = new Dictionary<int, IActor>();
        private readonly HashSet<int> _currentRActorIds = new HashSet<int>();

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
            GizmoType.Switch
        };

        private readonly HashSet<GizmoType> _importantGizmoTypes = new HashSet<GizmoType>
        {
            GizmoType.Door,
            GizmoType.Switch,
            GizmoType.BreakableDoor
        };

        public List<Structures.Avoidance> CurrentAvoidances = new List<Structures.Avoidance>();
        public List<AvoidanceNode> CurrentNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> AllNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> NearbyNodes = new List<AvoidanceNode>();
        public IOrderedEnumerable<AvoidanceNode> SafeNodesByWeight = new List<AvoidanceNode>().OrderBy(a => 1);
        public IOrderedEnumerable<AvoidanceNode> SafeNodesByDistance = new List<AvoidanceNode>().OrderBy(a => 1);
        public AvoidanceAreaStats NearbyStats = new AvoidanceAreaStats();
        public AvoidanceLayer KiteNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer MonsterNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer AvoidanceNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer ObstacleNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer SafeNodeLayer = new AvoidanceLayer();


        public int HighestNodeWeight { get; set; }
        public Vector3 MonsterCentroid { get; set; }
        public Vector3 AvoidanceCentroid { get; set; }
        public AvoidanceNode HighestNode { get; set; }

        public AvoidanceGrid Grid => AvoidanceGrid.Instance;

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            if (!TrinityPlugin.IsPluginEnabled || ZetaDia.IsLoadingWorld)
                return;

            UpdateAvoidances();
            RemoveExpiredAvoidances();
            UpdateGrid();
            UpdateStats();
        }

        private void UpdateStats()
        {
            NearbyStats.Update(NearbyNodes);
        }

        private void UpdateAvoidances()
        {
            if (!TrinityPlugin.Settings.Avoidance.Avoidances.Any(a => a.IsEnabled))
                return;

            _currentRActorIds.Clear();

            var source = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Select(a => new TrinityCacheObject(a) as IActor).ToList();

            foreach (var actor in source)
            {
                if (actor == null)
                    continue;

                if (!AvoidanceDataFactory.AvoidanceDataDictionary.ContainsKey(actor.ActorSNO) && !actor.IsElite)
                    continue;

                var rActorId = actor.RActorGuid;

                IActor existingActor;

                _currentRActorIds.Add(rActorId);

                var isValid = IsValid(actor);

                if (_cachedActors.TryGetValue(rActorId, out existingActor))
                {
                    if (!isValid)
                    {
                        _cachedActors.Remove(rActorId);
                    }
                    else
                    {
                        existingActor.Position = actor.Position;
                        existingActor.Distance = actor.Distance;
                    }
                    continue;
                }

                if (!isValid)
                    continue;

                Structures.Avoidance avoidance;
                if (AvoidanceDataFactory.TryCreateAvoidance(source, actor, out avoidance))
                {
                    Logger.Log("Created new Avoidance from {0} RActorId={1} ({2})", actor.InternalName, actor.RActorGuid, avoidance.Data.Name);
                    _cachedActors.Add(rActorId, actor);
                    CurrentAvoidances.Add(avoidance);
                }
            }
        }

        private static bool IsValid(IActor actor)
        {
            return !actor.IsDead && (actor.CommonData == null || actor.CommonData.IsValid && !actor.CommonData.IsDisposed);
        }

        private void RemoveExpiredAvoidances()
        {
            foreach (var avoidance in CurrentAvoidances)
            {
                avoidance.Actors.RemoveAll(a => !_currentRActorIds.Contains(a.RActorGuid));
            }
            CurrentAvoidances.RemoveAll(a => !a.Actors.Any());
        }

        public void UpdateGrid()
        {
            using (new PerformanceLogger("UpdateGrid"))
            {
                HighestNodeWeight = 0;

                if (Grid.NearestNode == null || Grid.NearestNode.DynamicWorldId != ZetaDia.WorldId)
                {
                    return;
                }

                var safeNodes = new AvoidanceLayer();
                var kiteNodes = new AvoidanceLayer();
                var avoidanceNodes = new AvoidanceLayer();
                var monsterNodes = new AvoidanceLayer();
                var obstacleNodes = new AvoidanceLayer();
                var activeAvoidanceIds = new HashSet<int>();

                var nodePool = Grid.GetNodesInRadius(TrinityPlugin.Player.Position, node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk), MaxDistance).Select(n => n.Reset()).ToList();
                var allNodes = Grid.GetNodesInRadius(TrinityPlugin.Player.Position, node => node != null, MaxDistance).ToList();
                var nearestNodes = Grid.GetNodesInRadius(TrinityPlugin.Player.Position, node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk), TrinityPlugin.Settings.Avoidance.AvoiderLocalRadius);

                try
                {
                    if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
                        return;

                    if (!nodePool.Any())
                        return;

                    foreach (var obj in TrinityPlugin.ObjectCache)
                    {
                        if (obj.IsMe)
                            continue;

                        UpdateGizmoFlags(obj);
                        UpdateGlobeFlags(obj);
                        UpdateAvoidanceFlags(obj, avoidanceNodes);
                        UpdateMonsterFlags(obj, monsterNodes);
                    }

                    foreach (var obstacle in CacheData.NavigationObstacles)
                    {
                        UpdateObstacleFlags(obstacle, obstacleNodes);
                    }

                    UpdateBacktrackFlags();
                    
                    foreach (var avoidance in Core.Avoidance.CurrentAvoidances)
                    {
                        if (!avoidance.Data.IsEnabled)
                            continue;

                        var handler = avoidance.Data.Handler;
                        if (handler == null)
                            continue;

                        if (handler.IsAllowed)
                        {
                            avoidance.Actors.Select(a => a.ActorSNO).ForEach(id => activeAvoidanceIds.Add(id));
                            handler.UpdateNodes(Grid, avoidance);
                        }
                    }

                    AvoidanceCentroid = avoidanceNodes.GetCentroid();
                    MonsterCentroid = monsterNodes.GetCentroid();

                    foreach (var node in nodePool)
                    {
                        if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.Backtrack))
                        {
                            node.Weight--;
                        }

                        if (node.NavigableCenter.Distance(AvoidanceCentroid) > 15f)
                        {
                            node.Weight += 2;
                        }

                        if (node.NavigableCenter.Distance(MonsterCentroid) > 15f)
                        {
                            node.Weight--;
                        }

                        if (Grid.IsInKiteDirection(node.NavigableCenter, 70))
                        {
                            node.AddNodeFlags(AvoidanceFlags.Kite);
                            node.Weight--;
                            kiteNodes.Add(node);
                        }
                        else
                        {
                            node.Weight++;
                        }

                        if (node.AdjacentNodes.All(n => !n.AvoidanceFlags.HasFlag(AvoidanceFlags.Avoidance) && !n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster) && n.NodeFlags.HasFlag(NodeFlags.AllowWalk)))
                        {
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
                    Logger.Log("Exception in UpdateGrid {0}", ex);

                    if (ex is CoroutineStoppedException)
                        throw;
                }

                AllNodes = allNodes;
                CurrentNodes = nodePool.OrderBy(n => n.Weight).ThenByDescending(n => n.Distance).ToList();
                SafeNodesByWeight = safeNodes.Nodes.OrderBy(n => n.Weight).ThenByDescending(n => n.Distance);
                SafeNodesByDistance = safeNodes.Nodes.OrderBy(n => n.Distance);
                SafeNodeLayer = safeNodes;
                KiteNodeLayer = kiteNodes;
                AvoidanceNodeLayer = avoidanceNodes;
                MonsterNodeLayer = monsterNodes;
                NearbyNodes = nearestNodes;
                ObstacleNodeLayer = obstacleNodes;
                ActiveAvoidanceIds = activeAvoidanceIds;
            }
        }

        public HashSet<int> ActiveAvoidanceIds { get; set; }

        private void UpdateGizmoFlags(IActor actor)
        {
            var isGizmo = _flaggedGizmoTypes.Contains(actor.GizmoType);
            if (!isGizmo)
                return;

            var importantGizmo = _importantGizmoTypes.Contains(actor.GizmoType);
            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.Radius*GizmoWeightRadiusFactor))
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

        private void UpdateAvoidanceFlags(IActor obj, AvoidanceLayer layer)
        {
            if (!DataDictionary.AvoidanceSNO.Contains(obj.ActorSNO))
                return;

            foreach (var node in Grid.GetNodesInRadius(obj.Position, obj.Radius*AvoidanceWeightRadiusFactor))
            {
                node.AddNodeFlags(AvoidanceFlags.Avoidance);
                node.Weight += 4;
                layer.Add(node);
            }
        }

        public void UpdateMonsterFlags(IActor actor, AvoidanceLayer layer)
        {
            if (actor.ActorType != ActorType.Monster)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.CollisionRadius*MonsterWeightRadiusFactor))
            {
                node.Weight += 2;
                node.AddNodeFlags(AvoidanceFlags.Monster);
                layer.Add(node);

                if (node.Center.Distance(actor.Position.ToVector2()) < actor.CollisionRadius)
                    node.AddNodeFlags(AvoidanceFlags.NavigationBlocking);
            }
        }

        public void UpdateObstacleFlags(CacheObstacleObject actor, AvoidanceLayer layer)
        {
            if (!DataDictionary.PathFindingObstacles.ContainsKey(actor.ActorSNO))
                return;

            var radius = DataDictionary.PathFindingObstacles[actor.ActorSNO];

            foreach (var node in Grid.GetNodesInRadius(actor.Position, radius))
            {
                node.NodeFlags &= ~NodeFlags.AllowWalk;
                node.IsWalkable = false;
                layer.Add(node);
            }
        }

        private void UpdateGlobeFlags(IActor actor)
        {
            if (actor.Type != ObjectType.HealthGlobe)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.Radius*GlobeWeightRadiusFactor))
            {
                node.Weight -= 6;
                node.AddNodeFlags(AvoidanceFlags.Health);
            }
        }

        private void UpdateBacktrackFlags()
        {
            foreach (var cachedPos in Core.PlayerHistory.Cache)
            {
                if (cachedPos.WorldId != TrinityPlugin.CurrentWorldId)
                    continue;

                var nearestNode = Grid.GetNearestNode(cachedPos.Position);
                if (nearestNode == null)
                    continue;

                var weightChange = Grid.IsInKiteDirection(cachedPos.Position, 90) ? -1 : 0;

                foreach (var node in nearestNode.AdjacentNodes)
                {
                    node.Weight += weightChange;
                    node.AddNodeFlags(AvoidanceFlags.Backtrack);
                }

                nearestNode.Weight += weightChange;
                nearestNode.AddNodeFlags(AvoidanceFlags.Backtrack);
            }
        }



    }
}