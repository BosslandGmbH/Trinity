using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Combat.Abilities;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Technicals;
using TrinityCoroutines;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    public interface IAvoider
    {
        bool IsAvoiding { get; }
        bool ShouldAvoid { get; }
        bool TryGetSafeSpot(out Vector3 position, float minDistance = 10f, float maxDistance = 100f, Func<AvoidanceNode, bool> condition = null);
        TimeSpan TimeSinceLastAvoid { get; }

        Vector3 SafeSpot { get; }
    }

    /// <summary>
    /// Avoider is responsible for detecting if player should avoid and for moving to a safe position.
    /// </summary>
    public class DefaultAvoider : IAvoider
    {
        public bool IsKiting { get; set; }
        public bool IsAvoiding { get; set; }

        public DateTime KiteStutterDelay = DateTime.MinValue;
        public DateTime KiteStutterDuration = DateTime.MinValue;

        public DateTime LastAvoidTime = DateTime.MinValue;

        public TimeSpan TimeSinceLastAvoid => DateTime.UtcNow.Subtract(LastAvoidTime);

        public AvoidanceSetting Settings => TrinityPlugin.Settings.Avoidance;

        public bool ShouldAvoid
        {
            get
            {
                IsKiting = false;
                IsAvoiding = false;

                if (CacheData.BuffsCache.Instance.HasInvulnerableShrine)
                    return false;

                if (CombatBase.IsDoingGoblinKamakazi)
                    return false;

                if (TrinityPlugin.Player.IsInTown)
                    return false;

                if (!TrinityPlugin.Settings.Combat.Misc.AvoidAoEOutOfCombat && !CombatBase.IsInCombat)
                    return false;

                // todo, allow a way of disabling avoidance for monk with serentiy, barb with ironskin, ghost walking WD
                // todo animations
                // todo element immunity

                if (ShouldAvoidCritical)
                {
                    IsAvoiding = true;
                    return true;
                }

                if (CombatBase.IsWaitingForPower())
                {                    
                    Logger.Log(LogCategory.Avoidance, "Not Avoiding because routine needs to cast a power");
                    return false;
                }

                if (ShouldKite)
                {
                    IsKiting = true;
                    IsAvoiding = true;
                    return true;
                }

                if (ShouldAvoidNormal)
                {
                    IsAvoiding = true;
                    return true;
                }
                return false;
            }
        }

        private bool ShouldAvoidNormal
        {
            get
            {
                if (Core.Avoidance.NearbyNodes.Any(n => n.AvoidanceFlags.HasFlag(AvoidanceFlags.Gizmo)) && PlayerMover.IsBlocked)
                {
                    return false;
                }

                if (Settings.DontAvoidWhenBlocked && PlayerMover.IsBlocked && PlayerMover.BlockedTimeMs > 5000)
                {
                    Logger.Log(LogCategory.Avoidance, "Not Avoiding because blocked");
                    return false;                  
                }

                if (Core.Avoidance.HighestNodeWeight >= 2 &&
                    Core.Avoidance.NearbyStats.HighestWeight >= Settings.MinimumHighestNodeWeightTrigger &&
                    Core.Avoidance.NearbyStats.WeightPctTotal >= Settings.MinimumNearbyWeightPctTotalTrigger &&
                    Core.Avoidance.NearbyStats.WeightPctAvg >= Settings.AvoiderNearbyPctAvgTrigger)
                {
                    Logger.Log(LogCategory.Avoidance, "Avoidance Local PctAvg: {0:0.00} / {1:0.00} PctTotal={2:0.00} / {3:0.00} Highest={4} / {5} ({6} Nodes, AbsHighest={7})",
                        Core.Avoidance.NearbyStats.WeightPctAvg,
                        Settings.AvoiderNearbyPctAvgTrigger,
                        Core.Avoidance.NearbyStats.WeightPctTotal,
                        Settings.MinimumNearbyWeightPctTotalTrigger,
                        Core.Avoidance.NearbyStats.HighestWeight,
                        Settings.MinimumHighestNodeWeightTrigger,
                        Core.Avoidance.NearbyStats.NodesTotal,
                        Core.Avoidance.HighestNodeWeight);

                    LastAvoidTime = DateTime.UtcNow;
                    {                       
                        return true;
                    }
                }
                return false;
            }

        }

        private bool ShouldAvoidCritical
        {
            get
            {
                if (TargetZDif < 8 && Settings.Avoidances.Any(a => a.IsEnabled))
                {
                    var standingInCritical = Core.Grids.Avoidance.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance);
                    if (standingInCritical)
                    {
                        Logger.Log(LogCategory.Avoidance, "IsStandingInFlags... CriticalAvoidance");
                        LastAvoidTime = DateTime.UtcNow;
                        return true;
                    }

                    if (CombatBase.CurrentTarget != null && Core.Grids.Avoidance.IsIntersectedByFlags(ZetaDia.Me.Position, CombatBase.CurrentTarget.Position, AvoidanceFlags.CriticalAvoidance))
                    {
                        TargetUtil.ClearCurrentTarget("Current Target Intersects Critical Avoidance.");
                    }

                    if (Core.Grids.Avoidance.IsPathingOverFlags(AvoidanceFlags.CriticalAvoidance))
                    {
                        Logger.Log(LogCategory.Avoidance, "IsPathingOverFlags... CriticalAvoidance");
                        Navigator.Clear();
                        LastAvoidTime = DateTime.UtcNow;
                        return true;
                    }
                }
                return false;
            }
        }

        private bool ShouldKite
        {
            get
            {
                if (DateTime.UtcNow < KiteStutterDuration)
                {
                    Logger.Log(LogCategory.Avoidance, "Kite On Cooldown");
                    return false;
                }

                if (!TrinityPlugin.ObjectCache.Any(o => o.Weight > 0) && TrinityPlugin.Player.CurrentHealthPct > 0.6)
                {
                    return false;
                }

                if (Settings.DontAvoidWhenBlocked && PlayerMover.IsBlocked && PlayerMover.BlockedTimeMs > 5000 && !Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position))
                {
                    Logger.Log(LogCategory.Avoidance, "Not kiting because blocked");
                    return false;
                }

                //if (CombatBase.CurrentTarget?.Type == TrinityObjectType.ProgressionGlobe && CombatBase.CurrentTarget?.Distance < 80f)
                //{
                //    Logger.Log(LogCategory.Avoidance, "Not kiting because current target is a close progression globe");
                //    return false;
                //}

                var isAtKiteHealth = TrinityPlugin.Player.CurrentHealthPct * 100 <= Settings.KiteHealth;
                if (isAtKiteHealth && TargetZDif < 4 && Settings.KiteMode != KiteMode.Never)
                {
                    var canSeeTarget = CombatBase.CurrentTarget == null || Core.Avoidance.Grid.CanRayCast(ZetaDia.Me.Position, CombatBase.CurrentTarget.Position);
                    if (canSeeTarget && Core.Grids.Avoidance.IsStandingInFlags(AvoidanceFlags.KiteFrom))
                    {
                        if (KiteStutterDelay < DateTime.UtcNow)
                        {
                            Logger.Log(LogCategory.Avoidance, "Kite Shutter Triggered");
                            KiteStutterDelay = DateTime.UtcNow.AddMilliseconds(Settings.KiteStutterDelay);
                            KiteStutterDuration = DateTime.UtcNow.AddMilliseconds(Settings.KiteStutterDuration);
                            return true;
                        }

                        Logger.Log(LogCategory.Avoidance, "IsStandingInFlags... KiteFromNode");
                        LastAvoidTime = DateTime.UtcNow;
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private static float TargetZDif
        {
            get { return CombatBase.CurrentTarget == null ? 0 : Math.Abs(CombatBase.CurrentTarget.Position.Z - ZetaDia.Me.Position.Z); }
        }

        private Vector3 _safeSpot;
        public Vector3 SafeSpot 
        {
            get
            {
                if (!Core.Avoidance.Grid.IsLocationInFlags(_safeSpot) && _safeSpot.Distance(ZetaDia.Me.Position) < 5f)
                    return _safeSpot;

                TryGetSafeSpot(out _safeSpot);                
                return _safeSpot;
            }
        }

        public bool TryGetSafeSpot(out Vector3 safeSpot, float minDistance = 0f, float maxDistance = 100f, Func<AvoidanceNode, bool> condition = null)
        {
            var nodes = Core.Avoidance.SafeNodesByDistance.Where(p => p.Distance >= minDistance && p.Distance <= maxDistance);
            var safeSpotNode = condition == null ? nodes.FirstOrDefault() : nodes.FirstOrDefault(condition);
            if (safeSpotNode != null)
            {
                safeSpot = safeSpotNode.NavigableCenter;
                return true;
            }
            safeSpot = Vector3.Zero;
            return false;
        }

    }

}



