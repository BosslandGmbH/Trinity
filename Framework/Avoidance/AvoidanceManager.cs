using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adventurer.Game.Exploration;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Trinity.Framework.Grid;
using Trinity.Framework.Utilities;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Technicals;
using Trinity.UI.UIComponents.RadarCanvas;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;
using Zeta.Common;

namespace Trinity.Framework.Avoidance
{
    /// <summary>
    /// Maintains a current list of avoidances and utilities to work with them.
    /// </summary>
    public class AvoidanceManager : Utility
    {
        private readonly HashSet<int> _currentRActorIds = new HashSet<int>();

        private readonly Dictionary<int, IActor> _cachedActors = new Dictionary<int, IActor>();

        public List<Avoidance> Current = new List<Avoidance>();

        public List<AvoidanceNode> NearbyNodes = new List<AvoidanceNode>();

        public AvoidanceAreaStats NearbyStats = new AvoidanceAreaStats();

        public int MaxDistance = 60;

        public int HighestNodeWeight { get; set; }
        public int NearbyWeightSum { get; set; }
        public float NearbyWeightPctSum { get; set; }
        public float FacingWeightPctSum { get; set; }
        public int FacingWeightSum { get; set; }

        public List<AvoidanceNode> CurrentNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> SafeNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> KiteNodes = new List<AvoidanceNode>();

        public List<Vector3> SafeCentroidPositions = new List<Vector3>();
        public List<Vector3> KiteCentroidPositions = new List<Vector3>();

        public List<AvoidanceNode> SafeCentroidNodes = new List<AvoidanceNode>();
        public List<AvoidanceNode> KiteCentroidNodes = new List<AvoidanceNode>();

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            if (!Trinity.IsPluginEnabled || ZetaDia.IsLoadingWorld)
                return;

            if (!Trinity.Settings.Advanced.UseExperimentalAvoidance)
                return;
              
            GetOrUpdateAvoidances();
            RemoveExpiredAvoidances();
            UpdateGrid();
            UpdateStats();
            
            ShouldAvoid = Avoider.ShouldAvoid();
        }               

        private void UpdateStats()
        {
            NearbyStats.Update(NearbyNodes);
        }

        public async Task<bool> Avoid()
        {
            if (ShouldAvoid)
            {
                Avoider.Mode = AvoidanceMode.StayClose;

                var result = await Avoider.MoveToSafeSpot();
                if (!result)
                {
                    IsAvoiding = true;
                    return false;
                }
            }

            IsAvoiding = false;
            return true;
        }

        public bool IsAvoiding { get; set; }
        public bool ShouldAvoid { get; set; }

        private void GetOrUpdateAvoidances()
        {
            _currentRActorIds.Clear();

            var source = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Select(a => new TrinityCacheObject(a) as IActor).ToList();            

            foreach (var actor in source)
            {
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

                Avoidance avoidance;
                if (AvoidanceFactory.TryCreateAvoidance(source, actor, out avoidance))
                {
                    Logger.Log("Created new Avoidance from {0} RActorId={1} ({2})", actor.InternalName, actor.RActorGuid, avoidance.Data.Name);
                    _cachedActors.Add(rActorId, actor);
                    Current.Add(avoidance);
                }
            }  
                      
        }

        private static bool IsValid(IActor actor)
        {
            return !actor.IsDead && !actor.IsDestroyed && (actor.CommonData == null || actor.CommonData.IsValid && !actor.CommonData.IsDisposed);
        }

        private void RemoveExpiredAvoidances()
        {
            foreach (var avoidance in Current)
            {                
                avoidance.Actors.RemoveAll(a => !_currentRActorIds.Contains(a.RActorGuid));
            }
            Current.RemoveAll(a => !a.Actors.Any());
        }

        public AvoidanceGrid Grid
        {
            get { return AvoidanceGrid.Instance; }
        }

        public void UpdateGrid()
        {
            if (!Trinity.Settings.Advanced.UseExperimentalAvoidance)
                return;

            using (new PerformanceLogger("UpdateFlags"))
            {
                HighestNodeWeight = 0;

                if (Grid.NearestNode == null || Grid.NearestNode.DynamicWorldId != ZetaDia.WorldId)
                {
                    return;
                }

                var nodePool = Grid.GetNodesInRadius(Trinity.Player.Position, node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk), MaxDistance);
                var nearestNodes = Grid.GetNodesInRadius(Trinity.Player.Position, node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk), Trinity.Settings.Avoidance.AvoiderLocalRadius);

               

                try
                {                    
                    if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
                        return;

                    if (nodePool == null || !nodePool.Any())
                        return;

                    nodePool.ForEach(n =>
                    {
                        if (n != null)
                            n.Reset();
                    });

                    foreach (var obj in Trinity.ObjectCache)
                    {
                        if (obj.IsMe)
                            continue;

                        UpdateGizmoFlags(obj);
                        UpdateAvoidanceFlags(obj);
                        UpdateGlobeFlags(obj);
                        UpdateMonsterFlags(obj);
                    }

                    UpdateBacktrackFlags();


                    foreach (var avoidance in Core.Avoidance.Current)
                    {
                        if (!avoidance.Data.IsEnabled)
                            continue;

                        var handler = avoidance.Data.Handler;
                        if (handler == null)
                            continue;

                        if (handler.IsAllowed)
                            handler.UpdateNodes(Grid, avoidance);
                    }

                    foreach (var node in nodePool)
                    {
                        if(node == null)
                            continue;

                        if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.Backtrack))
                            node.Weight--;

                        if (IsInKiteDirection(node.NavigableCenter, 70))
                        {
                            node.AddNodeFlags(AvoidanceFlags.Kite);
                            node.Weight--;
                        }
                        else
                        {
                            node.Weight++;
                        }                            

                        if (node.Weight > HighestNodeWeight)
                            HighestNodeWeight = node.Weight;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception in UpdateFlags {0}", ex);

                    if (ex is CoroutineStoppedException)
                        throw;
                }

                CalculateSafeSpots();

                CurrentNodes = nodePool.OrderBy(n => n.Distance).ToList();

                NearbyNodes = nearestNodes;
            }
        }

        private void CalculateSafeSpots()
        {
            using (new PerformanceLogger("CalculateAvoidance"))
            {                
                var searchPositions = StuckHandler.GetCirclePoints(16, 15f, Trinity.Player.Position);
                searchPositions.AddRange(StuckHandler.GetCirclePoints(16, 30f, Trinity.Player.Position));
                searchPositions.AddRange(StuckHandler.GetCirclePoints(16, 45f, Trinity.Player.Position));

                KiteNodes.Clear();
                KiteCentroidNodes.Clear();
                KiteCentroidPositions.Clear();

                SafeNodes.Clear();
                SafeCentroidNodes.Clear();
                SafeCentroidPositions.Clear();

                NearbyWeightPctSum = 0;
                NearbyWeightSum = 0;
                FacingWeightPctSum = 0;
                FacingWeightSum = 0;

                var nearbyNodes = Grid.GetConnectedNodes(Trinity.Player.Position, node => true, 12f);
                foreach (var node in nearbyNodes)
                {
                    NearbyWeightPctSum += node.WeightPct;
                    NearbyWeightSum += node.Weight;

                    if (IsInPlayerFacingDirection(node.NavigableCenter, 90))
                    {
                        FacingWeightSum += node.Weight;
                        FacingWeightPctSum += node.WeightPct;
                    }                        
                }

                foreach (var seed in searchPositions)
                {
                    var nodes = Grid.GetConnectedNodes(seed, node => node.NodeFlags.HasFlag(NodeFlags.AllowWalk) && !node.AdjacentNodes.Any(n => n.WeightPct > 0.1) && node.Weight <= 0, 4f);
                    var totalWeightPct = nodes.Sum(n => n.WeightPct);

                    if (nodes.Count > 4 && totalWeightPct <= NearbyWeightPctSum)
                    {
                        var positions = nodes.Select(n => n.NavigableCenter).ToList();
                        var centroid = MathUtil.Centroid(positions);
                        var centroidNode = Grid.GetNearestNode(centroid);

                        centroidNode.NearbyWeightPct = totalWeightPct;
                        SafeNodes.Add(centroidNode);

                        if (Grid.CanRayWalk(Trinity.Player.Position, centroid))
                        {
                            if (IsInKiteDirection(centroid, 90))
                            {
                                KiteNodes.AddRange(nodes);
                                KiteCentroidNodes.Add(centroidNode);
                                KiteCentroidPositions.Add(centroid);
                            }

                            SafeNodes.AddRange(nodes);
                            SafeCentroidNodes.Add(centroidNode);
                            SafeCentroidPositions.Add(centroid);
                        }
                    }
                }

                SafeNodes = SafeNodes.OrderBy(n => n.NearbyWeightPct).ToList();
                KiteNodes = KiteNodes.OrderBy(n => n.NearbyWeightPct).ToList();
            }
        }

        public const float GlobeWeightRadiusFactor = 1f;
        public const float MonsterWeightRadiusFactor = 1f;
        public const float AvoidanceWeightRadiusFactor = 1f;
        public const float GizmoWeightRadiusFactor = 1f;

        private void UpdateGizmoFlags(IActor actor)
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

        private readonly HashSet<GizmoType> _flaggedGizmoTypes = new HashSet<GizmoType>()
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
        };

        private readonly HashSet<GizmoType> _importantGizmoTypes = new HashSet<GizmoType>()
        {
            GizmoType.Door,
            GizmoType.Switch,
            GizmoType.BreakableDoor,
        };

        private void UpdateAvoidanceFlags(IActor obj)
        {
            if (!DataDictionary.AvoidanceSNO.Contains(obj.ActorSNO))
                return;

            foreach (var node in Grid.GetNodesInRadius(obj.Position, obj.Radius * AvoidanceWeightRadiusFactor))
            {
                node.AddNodeFlags(AvoidanceFlags.Avoidance);
                node.Weight += 4;
            }
        }

        private void UpdateGlobeFlags(IActor actor)
        {
            if (actor.Type != ObjectType.HealthGlobe)
                return;

            foreach (var node in Grid.GetNodesInRadius(actor.Position, actor.Radius * GlobeWeightRadiusFactor))
            {
                node.Weight -= 6;
                node.AddNodeFlags(AvoidanceFlags.Health);
            }         
        }

        public void UpdateMonsterFlags(IActor actor)
        {
            if (actor.ActorType != ActorType.Monster)
                return;

            var nodes = Grid.GetNodesInRadius(actor.Position, actor.CollisionRadius * MonsterWeightRadiusFactor);    
            foreach (var node in nodes)
            {
                node.Weight += 2;
                node.AddNodeFlags(AvoidanceFlags.Monster);

                if (node.Center.Distance(actor.Position.ToVector2()) < actor.CollisionRadius)
                    node.AddNodeFlags(AvoidanceFlags.NavigationBlocking);
            }
        }

        private void UpdateBacktrackFlags()
        {
            foreach (var cachedPos in Core.PlayerHistory.Cache)
            {
                if (cachedPos.WorldId != Trinity.CurrentWorldId)
                    continue;

                var nearestNode = Grid.GetNearestNode(cachedPos.Position);
                if (nearestNode == null)
                    continue;

                var weightChange = IsInKiteDirection(cachedPos.Position, 90) ? -1 : 0;

                foreach (var node in nearestNode.AdjacentNodes)
                {
                    node.Weight += weightChange;
                    node.AddNodeFlags(AvoidanceFlags.Backtrack);
                }

                nearestNode.Weight += weightChange;
                nearestNode.AddNodeFlags(AvoidanceFlags.Backtrack);
            }
        }

        public bool IsInKiteDirection(Vector3 position, double degreesDifferenceAllowed)
        {
            return MathUtil.GetRelativeAngularVariance(Trinity.Player.Position, Core.PlayerHistory.Centroid, position) <= degreesDifferenceAllowed;
        }

        public bool IsInPlayerFacingDirection(Vector3 position, double degreesDifferenceAllowed)
        {
            var pointInFront = MathEx.GetPointAt(Trinity.Player.Position, 10f, Trinity.Player.Rotation);
            return MathUtil.GetRelativeAngularVariance(Trinity.Player.Position, pointInFront, position) <= degreesDifferenceAllowed;
        }

    }
}


