using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adventurer.Game.Exploration;
using Adventurer.UI.UIComponents;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework.Actors;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Objects;
using Trinity.Technicals;
using Trinity.UI.RadarUI;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    public class AvoidanceManager : Module
    {
        public AvoidanceManager()
        {
            Avoider = new DefaultAvoider();
        }

        public bool InAvoidance(Vector3 position)
        {
            return Grid.IsLocationInFlags(position, AvoidanceFlags.Avoidance);
        }

        public bool InCriticalAvoidance(Vector3 position)
        {
            return Grid.IsLocationInFlags(position, AvoidanceFlags.CriticalAvoidance);
        }

        public IAvoider Avoider { get; set; }
        public const float GlobeWeightRadiusFactor = 1f;
        public const float MonsterWeightRadiusFactor = 1f;
        public const float AvoidanceWeightRadiusFactor = 1f;
        public const float GizmoWeightRadiusFactor = 1f;
        public const int MaxDistance = 70;
        private readonly Dictionary<int, TrinityActor> _cachedActors = new Dictionary<int, TrinityActor>();
        private readonly HashSet<int> _currentRActorIds = new HashSet<int>();

        public IEnumerable<TrinityActor> ActiveAvoidanceActors => CurrentAvoidances.SelectMany(a => a.Actors);
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

        public List<Structures.Avoidance> CurrentAvoidances = new List<Structures.Avoidance>();
        public List<AvoidanceNode> CurrentNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> AllNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> NearbyNodes = new List<AvoidanceNode>();
        public IOrderedEnumerable<AvoidanceNode> SafeNodesByWeight = new List<AvoidanceNode>().OrderBy(a => 1);
        public IOrderedEnumerable<AvoidanceNode> SafeNodesByDistance = new List<AvoidanceNode>().OrderBy(a => 1);
        public AvoidanceAreaStats NearbyStats = new AvoidanceAreaStats();
        public AvoidanceLayer KiteNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer KiteFromLayer = new AvoidanceLayer();
        public AvoidanceLayer MonsterNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer AvoidanceNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer ObstacleNodeLayer = new AvoidanceLayer();
        public AvoidanceLayer SafeNodeLayer = new AvoidanceLayer();

        public AvoidanceSetting Settings => TrinityPlugin.Settings.Avoidance;

        public int HighestNodeWeight { get; set; }
        public Vector3 MonsterCentroid { get; set; }
        public Vector3 AvoidanceCentroid { get; set; }
        public AvoidanceNode HighestNode { get; set; }

        public AvoidanceGrid Grid => AvoidanceGrid.Instance;

        protected override void OnPulse()
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
            _currentRActorIds.Clear();

            if (!TrinityPlugin.Settings.Avoidance.Avoidances.Any(a => a.IsEnabled))
                return;


            var source = Core.Actors.AllRActors.ToList();

            foreach (var actor in source)
            {
                if (actor == null)
                    continue;

                var rActorId = actor.RActorId;

                TrinityActor existingActor;

                _currentRActorIds.Add(rActorId);

                var isValid = actor.IsValid;

                if (_cachedActors.TryGetValue(rActorId, out existingActor))
                {
                    if (!isValid)
                    {
                        _cachedActors.Remove(rActorId);
                    }
                    else
                    {
                        //Logger.Log($"Updating Existing Actor {actor.Name} OldAnim={existingActor.CurrentAnimation} NewAnim={actor.CurrentAnimation}");
                        existingActor.Position = actor.Position;
                        existingActor.Distance = actor.Distance;
                        existingActor.Animation = actor.Animation;
                    }
                    continue;
                }

                if (!isValid)
                    continue;

                Structures.Avoidance avoidance;
                if (AvoidanceDataFactory.TryCreateAvoidance(source, actor, out avoidance))
                {
                    Logger.Log(LogCategory.Avoidance, $"Created new Avoidance from {actor.InternalName} RActorId={actor.RActorId} ({avoidance.Data.Name}, Immune: {avoidance.IsImmune})");
                    _cachedActors.Add(rActorId, actor);
                    CurrentAvoidances.Add(avoidance);
                }
            }
        }

        private void RemoveExpiredAvoidances()
        {
            foreach (var avoidance in CurrentAvoidances)
            {
                avoidance.Actors.RemoveAll(a => !_currentRActorIds.Contains(a.RActorId));
            }
            CurrentAvoidances.RemoveAll(a => !a.Actors.Any(actor => actor.IsValid));
        }

        public bool IsUpdatingNodes { get; set; }

        public void UpdateGrid()
        {
            Grid.IsUpdatingNodes = true;

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
                var activeAvoidanceSnoIds = new HashSet<int>();
                var kiteFromNodes = new AvoidanceLayer();

                var nodePool = Grid.GetNodesInRadius(TrinityPlugin.Player.Position, node => node.IsWalkable, MaxDistance).Select(n => n.Reset()).ToList();
                //var allNodes = Grid.GetNodesInRadius(TrinityPlugin.Player.Position, node => node != null, MaxDistance).ToList();
                var nearestNodes = Grid.GetNodesInRadius(TrinityPlugin.Player.Position, node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk), TrinityPlugin.Settings.Avoidance.AvoiderLocalRadius);
                var weightSettings = TrinityPlugin.Settings.Avoidance.WeightingOptions;

                try
                {
                    if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
                        return;

                    if (!nodePool.Any())
                        return;

                    foreach (var obj in TrinityPlugin.Targets)
                    {
                        if (obj.IsMe)
                            continue;

                        UpdateGizmoFlags(obj);
                        UpdateGlobeFlags(obj);
                        UpdateMonsterFlags(obj, monsterNodes);
                        UpdateKiteFromFlags(obj, kiteFromNodes);                       
                    }

                    //foreach (var obstacle in CacheData.NavigationObstacles)
                    //{
                    //    UpdateObstacleFlags(obstacle, obstacleNodes);
                    //}

                    //UpdateBacktrackFlags();
                    
                    foreach (var avoidance in Core.Avoidance.CurrentAvoidances)
                    {
                        if (!avoidance.Data.IsEnabled || avoidance.IsImmune)
                            continue;

                        var handler = avoidance.Data.Handler;
                        if (handler == null)
                            continue;

                        if (handler.IsAllowed)
                        {
                            avoidance.Actors.ForEach(a =>
                            {
                                activeAvoidanceSnoIds.Add(a.ActorSnoId);
                                if (Settings.PathAroundAvoidance)
                                {
                                    TrinityPlugin.MainGridProvider.AddCellWeightingObstacle(a.ActorSnoId, a.CollisionRadius);
                                }
                            });

                            try
                            {
                                handler.UpdateNodes(Grid, avoidance);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"Exception in AvoidanceHandler updating nodes. Name={avoidance.Data?.Name} Handler={avoidance.Data.Handler?.GetType()} {ex} {Environment.StackTrace}");
                            }

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
                            if(weightSettings.HasFlag(WeightingOptions.AdjacentSafe))
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

            Grid.IsUpdatingNodes = true;
        }

        private void UpdateGizmoFlags(TrinityActor actor)
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

                    if (actor.Type != TrinityObjectType.Door || !actor.IsUsed)
                    {
                        node.AddNodeFlags(AvoidanceFlags.ClosedDoor);
                    }
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
                node.AddNodeFlags(AvoidanceFlags.Monster);
                layer.Add(node);

                if (node.Center.Distance(actor.Position.ToVector2()) < actor.CollisionRadius)
                    node.AddNodeFlags(AvoidanceFlags.NavigationBlocking);
            }
        }

        public void UpdateKiteFromFlags(TrinityActor actor, AvoidanceLayer layer)
        {
            if (Settings.KiteMode == KiteMode.Never)
                return;

            if (actor.ActorType != ActorType.Monster || actor.IsQuestMonster)
                return;

            var kiteFromBoss = Settings.KiteMode == KiteMode.Elites && actor.IsBoss;
            var kiteFromElites = Settings.KiteMode == KiteMode.Elites && actor.IsElite;
            var kiteAlways = Settings.KiteMode == KiteMode.Always;
            var isKiting = kiteFromBoss || kiteFromElites || kiteAlways;
            var weight = (int)Settings.KiteWeight;

            if (!isKiting)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.CollisionRadius + Settings.KiteDistance))
            {
                if (!node.IsWalkable)
                    continue;

                node.Weight = weight;
                node.AddNodeFlags(AvoidanceFlags.KiteFrom);
                layer.Add(node);
            }
        }

        //public void UpdateObstacleFlags(CacheObstacleObject actor, AvoidanceLayer layer)
        //{
        //    if (!DataDictionary.PathFindingObstacles.ContainsKey(actor.ActorSnoId))
        //        return;

        //    var radius = DataDictionary.PathFindingObstacles[actor.ActorSnoId];

        //    foreach (var node in Grid.GetNodesInRadius(actor.Position, radius))
        //    {
        //        node.NodeFlags &= ~NodeFlags.AllowWalk;
        //        node.IsWalkable = false;
        //        layer.Add(node);
        //    }
        //}

        private void UpdateGlobeFlags(TrinityActor actor)
        {
            if (actor.Type != TrinityObjectType.HealthGlobe)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.Radius*GlobeWeightRadiusFactor))
            {
                if (TrinityPlugin.Settings.Avoidance.WeightingOptions.HasFlag(WeightingOptions.Globes))
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
                var shouldChangeWeight = TrinityPlugin.Settings.Avoidance.WeightingOptions.HasFlag(WeightingOptions.Globes);

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