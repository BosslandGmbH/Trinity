using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Adventurer.Game.Exploration;
using Trinity.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    class NavHelper
    {
        #region Fields
        private static DateTime lastFoundSafeSpot = DateTime.MinValue;
        private static Vector3 lastSafeZonePosition = Vector3.Zero;
        private static bool hasEmergencyTeleportUp = false;
        #endregion

        #region Helper fields
        private static List<TrinityCacheObject> ObjectCache
        {
            get
            {
                return Trinity.ObjectCache;
            }
        }
        private static CacheData.PlayerCache Player
        {
            get
            {
                return CacheData.Player;
            }
        }
        private static bool AnyTreasureGoblinsPresent
        {
            get
            {
                if (ObjectCache != null)
                    return ObjectCache.Any(u => u.IsTreasureGoblin);
                else
                    return false;
            }
        }
        private static TrinityCacheObject CurrentTarget
        {
            get
            {
                return Trinity.CurrentTarget;
            }
        }
        private static HashSet<SNOPower> Hotbar
        {
            get
            {
                return CacheData.Hotbar.ActivePowers;
            }
        }
        private static Zeta.Bot.Navigation.MainGridProvider MainGridProvider
        {
            get
            {
                return Trinity.MainGridProvider;
            }
        }
        #endregion


        internal static string PrettyPrintVector3(Vector3 pos)
        {
            return string.Format("x=\"{0:0}\" y=\"{1:0}\" z=\"{2:0}\"", pos.X, pos.Y, pos.Z);
        }

        internal static bool CanRayCast(Vector3 destination)
        {
            return CanRayCast(Player.Position, destination);
        }

        /// <summary>
        /// Checks the Navigator to see if the destination is in LoS (walkable) and also checks for any navigation obstacles
        /// </summary>
        /// <param name="vStartLocation"></param>
        /// <param name="vDestination"></param>
        /// <param name="ZDiff"></param>
        /// <returns></returns>
        internal static bool CanRayCast(Vector3 vStartLocation, Vector3 vDestination)
        {
            // Navigator.Raycast is REVERSE Of ZetaDia.Physics.Raycast
            // Navigator.Raycast returns True if it "hits" an edge
            // ZetaDia.Physics.Raycast returns False if it "hits" an edge
            // So ZetaDia.Physics.Raycast() == !Navigator.Raycast()
            // We're using Navigator.Raycast now because it's "faster" (per Nesox)

            using (new PerformanceLogger("CanRayCast"))
            {
                if (DataDictionary.NeverRaycastLevelAreaIds.Contains(Trinity.Player.LevelAreaId))
                    return true;

                bool rayCastHit = Navigator.Raycast(vStartLocation, vDestination);

                if (rayCastHit)
                    return false;

                return !CacheData.NavigationObstacles.Any(o => MathEx.IntersectsPath(o.Position, o.Radius, vStartLocation, vDestination));
            }
        }

        /// <summary>
        /// This will find a safe place to stand in both Kiting and Avoidance situations
        /// </summary>
        /// <param name="isStuck"></param>
        /// <param name="stuckAttempts"></param>
        /// <param name="dangerPoint"></param>
        /// <param name="shouldKite"></param>
        /// <param name="avoidDeath"></param>
        /// <returns></returns>
        internal static Vector3 FindSafeZone(bool isStuck, int stuckAttempts, Vector3 dangerPoint, bool shouldKite = false, IEnumerable<TrinityCacheObject> monsterList = null, bool avoidDeath = false)
        {
            // Handle The Butcher's Lair
            var butcherFloorPanels = CacheData.TimeBoundAvoidance.Where(aoe => DataDictionary.ButcherFloorPanels.Contains(aoe.ActorSNO)).ToList();
            if (butcherFloorPanels.Any())
            {
                foreach (var safePoint in DataDictionary.ButcherPanelPositions.OrderBy(p => p.Value.Distance2DSqr(Trinity.Player.Position)))
                {
                    // Floor panel with fire animation was added to cache
                    if (butcherFloorPanels.Any(p => p.ActorSNO == safePoint.Key && p.Position.Distance2DSqr(safePoint.Value) <= 15f * 15f))
                    {
                        continue;
                    }

                    // floor panel position is in Butcher animation avoidance (charging, chain hook)
                    if (CacheData.TimeBoundAvoidance.Any(aoe => aoe.Position.Distance(safePoint.Value) < aoe.Radius))
                        continue;

                    // no avoidance object in cache, this point is safe
                    return safePoint.Value;
                }

                // Don't fall back to regular avoidance
                return Vector3.Zero;
            }

            if (!isStuck)
            {
                if (shouldKite && DateTime.UtcNow.Subtract(lastFoundSafeSpot).TotalMilliseconds <= 1500 && lastSafeZonePosition != Vector3.Zero)
                {
                    return lastSafeZonePosition;
                }
                if (DateTime.UtcNow.Subtract(lastFoundSafeSpot).TotalMilliseconds <= 800 && lastSafeZonePosition != Vector3.Zero)
                {
                    return lastSafeZonePosition;
                }
                hasEmergencyTeleportUp = (!Player.IsIncapacitated && (
                    // Leap is available
                    (CombatBase.CanCast(SNOPower.Barbarian_Leap)) ||
                    // Whirlwind is available
                    (CombatBase.CanCast(SNOPower.Barbarian_Whirlwind) &&
                        ((Player.PrimaryResource >= 10))) ||
                    // Tempest rush is available
                    (CombatBase.CanCast(SNOPower.Monk_TempestRush) &&
                        ((Player.PrimaryResource >= 20))) ||
                    // Teleport is available
                    (CombatBase.CanCast(SNOPower.Wizard_Teleport) && Player.PrimaryResource >= 15) ||
                    // Archon Teleport is available
                    (CombatBase.CanCast(SNOPower.Wizard_Archon_Teleport))
                    ));
                // Wizards can look for bee stings in range and try a wave of force to dispel them
                if (!shouldKite && Player.ActorClass == ActorClass.Wizard && CombatBase.CanCast(SNOPower.Wizard_WaveOfForce) &&
                    !Player.IsIncapacitated && CacheData.TimeBoundAvoidance.Count(u => u.ActorSNO == 5212 && u.Position.Distance(Player.Position) <= 15f) >= 2 &&
                    (
                    //HotbarSkills.PassiveSkills.Contains(SNOPower.Wizard_Passive_CriticalMass) || 
                    PowerManager.CanCast(SNOPower.Wizard_WaveOfForce)))
                {
                    ZetaDia.Me.UsePower(SNOPower.Wizard_WaveOfForce, Vector3.Zero, Player.WorldDynamicID, -1);
                }
            }


            float highestWeight = 0f;

            if (monsterList == null)
                monsterList = new List<TrinityCacheObject>();

            //Vector3 vBestLocation = FindSafeZone(dangerPoint, shouldKite, isStuck, monsterList, avoidDeath);            
            Vector3 vBestLocation = MainFindSafeZone(dangerPoint, shouldKite, isStuck, monsterList, avoidDeath);
            highestWeight = 1;

            // Loop through distance-range steps
            if (highestWeight <= 0)
                return vBestLocation;

            lastFoundSafeSpot = DateTime.UtcNow;
            lastSafeZonePosition = vBestLocation;
            return vBestLocation;
        }

        const double BoxSize = 2.5;
        public static Point WorldToGrid(Vector2 worldVector)
        {
            // MainGridProvider.WorldToGrid() reverses X and Y
            
            return new Point((int)Math.Round(worldVector.Y / BoxSize, 0, MidpointRounding.AwayFromZero), (int)Math.Round(worldVector.Y / BoxSize, 0, MidpointRounding.AwayFromZero));
        }

        internal static Vector3 KitePoint(Vector3 origin, float minDistance, float maxDistance)
        {            
            return MainFindSafeZone(origin, true, false, new List<TrinityCacheObject> { TargetUtil.GetBestClusterUnit() }, false, minDistance, maxDistance);
        }

        // thanks to Main for the super fast can-stand-at code
        internal static Vector3 MainFindSafeZone(Vector3 origin, bool shouldKite = false, bool isStuck = false, IEnumerable<TrinityCacheObject> monsterList = null, bool avoidDeath = false, float minDistance = 0f, float maxDistance = 0f)
        {
            const float gridSquareSize = 5f;
            
            if (maxDistance <= 0 || maxDistance <= minDistance)
            {
                var dhMaxDistance = Math.Max(Trinity.Settings.Combat.DemonHunter.KiteMaxDistance, Trinity.Settings.Combat.DemonHunter.KiteLimit + 5);
                maxDistance = Trinity.Player.ActorClass == ActorClass.DemonHunter ? dhMaxDistance : 100f;
            }

            const int maxWeight = 100;

            double gridSquareRadius = Math.Sqrt((Math.Pow(gridSquareSize / 2, 2) + Math.Pow(gridSquareSize / 2, 2)));

            GridPoint3 bestPoint = new GridPoint3(Vector3.Zero, 0, 0);

            if (MainGridProvider.Width == 0)
            {
                // Do not remove nav server logging, we need to differentiate between legitmate trinity issues and nav server issues.
                // In this case, if MainGridProvider is empty, then the bot cannot kite.
                Logger.Log("NavServer Issue: MainGridProvider is empty, unable to avoidance/kite position");
                return Vector3.Zero;
            }

            int totalNodes = 0;
            int nodesCantStand = 0;
            int nodesZDiff = 0;
            int nodesGT45Raycast = 0;
            int nodesAvoidance = 0;
            int nodesMonsters = 0;
            int nodesObjects = 0;
            int pathFailures = 0;
            int navRaycast = 0;
            int pointsFound = 0;

            int worldId = Trinity.Player.WorldID;
            Stopwatch[] timers = Enumerable.Range(0, 12).Select(i => new Stopwatch()).ToArray();

            Vector2 minWorld;
            minWorld.X = origin.X - maxDistance;
            minWorld.Y = origin.Y - maxDistance;

            //Point minPoint = WorldToGrid(minWorld);
            Point minPoint = MainGridProvider.WorldToGrid(minWorld);
            minPoint.X = Math.Max(minPoint.X, 0);
            minPoint.Y = Math.Max(minPoint.Y, 0);

            Vector2 maxWorld;
            maxWorld.X = origin.X + maxDistance;
            maxWorld.Y = origin.Y + maxDistance;

            // MainGridProvider will be empty/clear we start receiving navServer data
            //Point maxPoint = WorldToGrid(maxWorld);
            Point maxPoint = MainGridProvider.WorldToGrid(maxWorld);
            maxPoint.X = Math.Min(maxPoint.X, MainGridProvider.Width - 1);
            maxPoint.Y = Math.Min(maxPoint.Y, MainGridProvider.Height - 1);

            Point originPos = MainGridProvider.WorldToGrid(origin.ToVector2());             

            //var monsters = monsterList.ToList();

            using (new PerformanceLogger("MainFindSafeZoneLoop"))
            {
                for (int y = minPoint.Y; y <= maxPoint.Y; y++)
                {
                    int searchAreaBasis = y * MainGridProvider.Width;
                    for (int x = minPoint.X; x <= maxPoint.X; x++)
                    {
                        totalNodes++;

                        timers[0].Start();

                        int dx = originPos.X - x;
                        int dy = originPos.Y - y;

                        // Ignore out of range
                        if (dx * dx + dy * dy > (maxDistance / 2.5f) * (maxDistance / 2.5f))
                        {
                            continue;
                        }

                        // extremely efficient CanStandAt
                        if (!MainGridProvider.SearchArea[searchAreaBasis + x])
                        {
                            nodesCantStand++;
                            continue;
                        }

                        Vector2 xy = MainGridProvider.GridToWorld(new Point(x, y));
                        Vector3 xyz = Vector3.Zero;

                        if (Trinity.Settings.Combat.Misc.UseNavMeshTargeting)
                        {
                            xyz = new Vector3(xy.X, xy.Y, MainGridProvider.GetHeight(xy));
                        }
                        else
                        {
                            xyz = new Vector3(xy.X, xy.Y, origin.Z + 4);
                        }

                        GridPoint3 gridPoint = new GridPoint3(xyz, 0, origin.Distance(xyz));

                        timers[0].Stop();

                        if (isStuck && gridPoint.Distance > (PlayerMover.TotalAntiStuckAttempts + 2) * 5)
                        {
                            continue;
                        }

                        if (minDistance > 0 && gridPoint.Distance < minDistance)
                        {
                            continue;
                        }

                        /*
                         * Check if a square is occupied already
                         */
                        // Avoidance
                        timers[2].Start();
                        if (CacheData.TimeBoundAvoidance.Any(aoe => CheckPointForAvoidance(aoe, xyz, gridSquareRadius)))
                        {
                            nodesAvoidance++;
                            continue;
                        }
                        timers[2].Stop();

                        if (monsterList != null && monsterList.Any(m => xyz.Distance(m.Position) - m.Radius - 2 <= minDistance))
                        {
                            nodesMonsters++;
                            continue;
                        }

                        // Monsters
                        if (shouldKite)
                        {
                            timers[3].Start();
                            double checkRadius = gridSquareRadius;

                            if (CombatBase.KiteDistance > 0)
                            {
                                checkRadius = gridSquareSize + 10f;
                            }

                            // Any monster standing in this GridPoint
                            if (CacheData.MonsterObstacles.Any(monster => monster.Position.Distance(xyz) - monster.Radius <= checkRadius))
                            {
                                nodesMonsters++;
                                continue;
                            }



                            timers[3].Stop();

                        }

                        timers[4].Start();
                        if (isStuck && UsedStuckSpots.Any(p => Vector3.Distance(p.Position, gridPoint.Position) <= gridSquareRadius))
                        {
                            continue;
                        }
                        timers[4].Stop();

                        // set base weight
                        if (!isStuck && !avoidDeath)
                        {
                            // e.g. ((100 - 15) / 100) * 100) = 85
                            // e.g. ((100 - 35) / 100) * 100) = 65
                            // e.g. ((100 - 75) / 100) * 100) = 25
                            gridPoint.Weight = ((maxDistance - gridPoint.Distance) / maxDistance) * maxWeight;

                            // Low weight for close range grid points
                            if (shouldKite && gridPoint.Distance < CombatBase.KiteDistance)
                            {
                                gridPoint.Weight = (int)gridPoint.Distance;
                            }

                        }
                        else
                        {
                            gridPoint.Weight = gridPoint.Distance;
                        }

                        // Boss Areas
                        timers[5].Start();
                        if (UnSafeZone.UnsafeKiteAreas.Any(a => a.WorldId == Trinity.Player.WorldID && a.Position.Distance2DSqr(gridPoint.Position) <= (a.Radius * a.Radius)))
                        {
                            continue;
                        }
                        timers[5].Stop();

                        if (shouldKite)
                        {
                            /*
                            * We want to down-weight any grid points where monsters are closer to it than we are
                            */
                            timers[7].Start();
                            foreach (CacheObstacleObject monster in CacheData.MonsterObstacles)
                            {
                                float distFromPointToMonster = gridPoint.Position.Distance(monster.Position);
                                float distFromPointToOrigin = gridPoint.Position.Distance(origin);

                                // No Kite Distance Setting
                                if (CombatBase.KiteDistance <= 0)
                                {
                                    // higher weight closer to monster
                                    if (distFromPointToMonster < distFromPointToOrigin)
                                    {
                                        gridPoint.Weight += distFromPointToOrigin;
                                    }
                                    else if (distFromPointToMonster > distFromPointToOrigin)
                                    {
                                        gridPoint.Weight -= distFromPointToMonster;
                                    }
                                }
                                else // Kite Distance is Set
                                {
                                    // higher weight further from monster
                                    if (distFromPointToMonster < distFromPointToOrigin)
                                    {
                                        gridPoint.Weight -= distFromPointToOrigin;
                                    }
                                    else if (distFromPointToMonster > distFromPointToOrigin)
                                    {
                                        gridPoint.Weight += distFromPointToMonster;
                                    }
                                    if (PositionCache.Cache.Any(cachePoint => gridPoint.Position.Distance(cachePoint.Position) <= gridSquareRadius))
                                    {
                                        gridPoint.Weight += distFromPointToOrigin; // always <= max distance, 0-150ish
                                    }
                                }
                            }
                            timers[7].Stop();

                            timers[8].Start();
                            foreach (CacheObstacleObject avoidance in CacheData.TimeBoundAvoidance)
                            {
                                float distSqrFromPointToAvoidance = gridPoint.Position.Distance2DSqr(avoidance.Position);

                                // position is inside avoidance
                                if (distSqrFromPointToAvoidance < (avoidance.Radius * avoidance.Radius))
                                    continue;

                                float distSqrFromPointToOrigin = gridPoint.Position.Distance2DSqr(origin);
                                if (distSqrFromPointToAvoidance < distSqrFromPointToOrigin)
                                {
                                    gridPoint.Weight -= distSqrFromPointToOrigin;
                                }
                                else if (distSqrFromPointToAvoidance > distSqrFromPointToOrigin)
                                {
                                    gridPoint.Weight += distSqrFromPointToAvoidance;
                                }
                            }
                            timers[8].Stop();
                        }
                        else if (isStuck)
                        {
                            // give weight to points nearer to our destination
                            gridPoint.Weight *= (maxDistance - PlayerMover.LastMoveToTarget.Distance(gridPoint.Position)) / maxDistance * maxWeight;
                        }
                        else if (!avoidDeath) // melee avoidance use only
                        {
                            timers[9].Start();
                            var monsterCount = Trinity.ObjectCache.Count(u => u.IsUnit && u.Position.Distance(gridPoint.Position) <= 2.5f);
                            if (monsterCount > 0)
                                gridPoint.Weight *= monsterCount;
                            timers[9].Stop();
                        }

                        pointsFound++;

                        if (gridPoint.Weight > bestPoint.Weight && gridPoint.Distance > 1)
                        {
                            bestPoint = gridPoint;
                        }
                    }
                }
            }


            if (isStuck)
            {
                UsedStuckSpots.Add(bestPoint);
            }

            string times = "";
            for (int t = 0; t < timers.Length; t++)
            {
                if (timers[t].IsRunning) timers[t].Stop();
                times += string.Format("{0}/{1:0.0} ", t, timers[t].ElapsedMilliseconds);
            }

            Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Kiting grid found {0}, distance: {1:0}, weight: {2:0}", bestPoint.Position, bestPoint.Distance, bestPoint.Weight);
            Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance,
            "Kiting grid stats Total={0} CantStand={1} ZDiff {2} GT45Raycast {3} Avoidance {4} Monsters {5} Obstacles {14} pathFailures {6} navRaycast {7} "
            + "pointsFound {8} shouldKite={9} isStuck={10} avoidDeath={11} monsters={12} timers={13}",
                totalNodes,
                nodesCantStand,
                nodesZDiff,
                nodesGT45Raycast,
                nodesAvoidance,
                nodesMonsters,
                pathFailures,
                navRaycast,
                pointsFound,
                shouldKite,
                isStuck,
                avoidDeath,
                monsterList == null ? 0 : monsterList.Count(),
                times,
                nodesObjects
                );

            if (double.IsNaN(bestPoint.Position.X))
            {
                Logger.LogVerbose("Somethign went wrong, NaN value for MainFindSafeZone result");
                return Vector3.Zero;
            }

            return bestPoint.Position;
        }

        private static bool CheckPointForAvoidance(CacheObstacleObject aoe, Vector3 xyz, double gridSquareRadius)
        {
            //switch (aoe.AvoidanceType)
            //{
            //    case AvoidanceType.Arcane:
            //        //return AvoidanceManager.CheckPositionForArcane(aoe.Rotator, aoe.Position, xyz);
            //        return false;
            //}

            return xyz.Distance(aoe.Position) <= gridSquareRadius;
        }




        internal static Vector3 SimpleUnstucker()
        {
            var myPos = Trinity.Player.Position;
            float rotation = Trinity.Player.Rotation;

            const double totalPoints = 2 * Math.PI;
            const double start = 0;
            const double step = Math.PI / 4;

            const float minDistance = 10f;
            const float maxDistance = 25f;
            const float stepDistance = 5f;

            HashSet<GridPoint3> gridPoints = new HashSet<GridPoint3>();

            int navigationObstacleFail = 0;

            for (double r = start; r <= totalPoints; r += step)
            {
                for (float d = minDistance; d <= maxDistance; d += stepDistance)
                {
                    float newDirection = (float)(rotation + r);
                    Vector3 newPos = MathEx.GetPointAt(myPos, d, newDirection);

                    if (!MainGridProvider.CanStandAt(MainGridProvider.WorldToGrid(newPos.ToVector2())))
                    {
                        continue;
                    }

                    // If this hits a known navigation obstacle, skip it
                    if (CacheData.NavigationObstacles.Any(o => MathEx.IntersectsPath(o.Position, o.Radius, myPos, newPos)))
                    {
                        navigationObstacleFail++;
                        continue;
                    }

                    // use distance as weight
                    gridPoints.Add(new GridPoint3(newPos, (int)d, d));
                }
            }

            if (!gridPoints.Any())
            {
                Logger.LogDebug(LogCategory.UserInformation, "Unable to generate new unstucker position! navObsticle={0} - trying RANDOM point!", navigationObstacleFail);

                Random random = new Random();
                int distance = random.Next(5, 30);
                float direction = (float)random.NextDouble();

                return MathEx.GetPointAt(myPos, distance, direction);
            }
            Navigator.Clear();

            var bestPoint = gridPoints.OrderByDescending(p => p.Weight).FirstOrDefault();
            Logger.LogDebug(LogCategory.UserInformation, "Generated Unstuck position {0} distance={1:0.0} navObsticle={2}",
                NavHelper.PrettyPrintVector3(bestPoint.Position), bestPoint.Distance, navigationObstacleFail);
            return bestPoint.Position;
        }

        internal static List<GridPoint3> UsedStuckSpots = new List<GridPoint3>();


    }
    internal class UnSafeZone
    {
        public int WorldId { get; set; }
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Spots where we should not kite to (used during boss fights)
        /// </summary>
        internal static HashSet<UnSafeZone> UnsafeKiteAreas = new HashSet<UnSafeZone>
        {
            new UnSafeZone
            {
                WorldId = 182976, 
                Position = (new Vector3(281.0147f,361.5885f,20.86533f)),
                Name = "Chamber of Queen Araneae",
                Radius = 90f
            },
            new UnSafeZone
            {
                WorldId = 78839,
                Position = (new Vector3(54.07843f, 55.02061f, 0.100002f)),
                Name = "Chamber of Suffering (Butcher)",
                Radius = 120f
            },
            new UnSafeZone
            {
                WorldId = 109143,
                Position = (new Vector3(355.8749f,424.0184f,-14.9f)),
                Name = "Izual",
                Radius = 120f
            },
            new UnSafeZone
            {
                WorldId = 121214,
                Position = new Vector3(579, 582, 21),
                Name = "Azmodan",
                Radius = 120f
            },
            new UnSafeZone
            {
                WorldId = 308446, 
                Position = new Vector3(469.9994f, 355.01f, -15.85094f),
                Name = "Urzael",
                Radius = (new Vector3(375.144f, 359.9929f, 0.1f)).Distance(new Vector3(469.9994f, 355.01f, -15.85094f)),
            }
        };
    }

    internal class GridPoint3 : IEquatable<GridPoint3>
    {
        public Vector3 Position { get; set; }
        public double Weight { get; set; }
        public float Distance { get; set; }

        /// <summary>
        /// Creates a new gridpoint
        /// </summary>
        /// <param name="position">Vector3 Position of the GridPoint</param>
        /// <param name="weight">Weight of the GridPoint</param>
        /// <param name="distance">Distance to the Position</param>
        public GridPoint3(Vector3 position, int weight, float distance)
        {
            Position = position;
            Weight = weight;
            Distance = distance;
        }

        public bool Equals(GridPoint3 other)
        {
            return Equals(Position, other.Position);
        }
    }

}
