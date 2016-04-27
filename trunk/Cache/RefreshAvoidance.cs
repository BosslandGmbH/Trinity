using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Helpers;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public partial class Trinity
    {
        private static bool RefreshAvoidance()
        {
            if (Settings.Advanced.UseExperimentalAvoidance)
                return false;

            try
            {
                CurrentCacheObject.Animation = c_diaObject.CommonData.CurrentAnimation;
            }
            catch (Exception ex)
            {
                Logger.LogDebug(LogCategory.CacheManagement, "Error reading CurrentAnimation for AoE sno:{0} raGuid:{1} name:{2} ex:{3}",
                  CurrentCacheObject.ActorSNO, CurrentCacheObject.RActorGuid, CurrentCacheObject.InternalName, ex.Message);
            }

            float customRadius;
            if (DataDictionary.DefaultAvoidanceCustomRadius.TryGetValue(CurrentCacheObject.ActorSNO, out customRadius) ||
                DataDictionary.DefaultAvoidanceAnimationCustomRadius.TryGetValue((int)CurrentCacheObject.Animation, out customRadius))
            {
                CurrentCacheObject.Radius = customRadius;
            }

            double minAvoidanceHealth = GetAvoidanceHealth(CurrentCacheObject.ActorSNO);
            double minAvoidanceRadius = GetAvoidanceRadius(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius+5);

            // Add Navigation cell weights to path around avoidance
            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, (float)minAvoidanceRadius);

            AvoidanceType avoidanceType = OldAvoidanceManager.GetAvoidanceType(CurrentCacheObject.ActorSNO);

            // Beast Charge should set aoe position as players current position!
            var avoidAtPlayerPosition = DataDictionary.AvoidAnimationAtPlayer.Contains((int)CurrentCacheObject.Animation);
            if (avoidAtPlayerPosition)
            {
                CurrentCacheObject.Position = Player.Position;
            }

            // Monks with Serenity up ignore all AOE's
            if (Player.ActorClass == ActorClass.Monk && Hotbar.Contains(SNOPower.Monk_Serenity) && GetHasBuff(SNOPower.Monk_Serenity))
            {
                // Monks with serenity are immune
                minAvoidanceHealth *= 0;
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring avoidance as a Monk with Serenity");
            }
            // Witch doctors with spirit walk available and not currently Spirit Walking will subtly ignore ice balls, arcane, desecrator & plague cloud
            if (Player.ActorClass == ActorClass.Witchdoctor && Hotbar.Contains(SNOPower.Witchdoctor_SpiritWalk) && GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
            {
                if (avoidanceType == AvoidanceType.IceBall || avoidanceType == AvoidanceType.Arcane || avoidanceType == AvoidanceType.Desecrator || avoidanceType == AvoidanceType.PlagueCloud)
                {
                    // Ignore ICE/Arcane/Desc/PlagueCloud altogether with spirit walk up or available
                    minAvoidanceHealth *= 0;
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring avoidance as a WitchDoctor with Spirit Walk");
                }
            }

            // Item based immunity
            switch (avoidanceType)
            {
                case AvoidanceType.PoisonTree:
                case AvoidanceType.PlagueCloud:
                case AvoidanceType.PoisonEnchanted:
                case AvoidanceType.PlagueHand:

                    if (Legendary.MarasKaleidoscope.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because MarasKaleidoscope is equipped", avoidanceType);
                        minAvoidanceHealth = 0;
                    }
                    break;

                case AvoidanceType.AzmoFireball:
                case AvoidanceType.DiabloRingOfFire:
                case AvoidanceType.DiabloMeteor:
                case AvoidanceType.ButcherFloorPanel:
                case AvoidanceType.Mortar:
                case AvoidanceType.MageFire:
                case AvoidanceType.MoltenTrail:
                case AvoidanceType.MoltenBall:
                case AvoidanceType.ShamanFire:

                    if (Legendary.TheStarOfAzkaranth.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because TheStarofAzkaranth is equipped", avoidanceType);
                        minAvoidanceHealth = 0;
                    }
                    break;

                case AvoidanceType.FrozenPulse:
                case AvoidanceType.IceBall:
                case AvoidanceType.IceTrail:

                    // Ignore if both items are equipped
                    if (Legendary.TalismanOfAranoch.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because TalismanofAranoch is equipped", avoidanceType);
                        minAvoidanceHealth = 0;
                    }
                    break;

                case AvoidanceType.Orbiter:
                case AvoidanceType.Thunderstorm:

                    if (Legendary.XephirianAmulet.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because XephirianAmulet is equipped", avoidanceType);
                        minAvoidanceHealth = 0;
                    }
                    break;

                case AvoidanceType.Arcane:
                    if (Legendary.CountessJuliasCameo.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because CountessJuliasCameo is equipped", avoidanceType);
                        minAvoidanceHealth = 0;
                    }
                    break;
            }

            // Set based immunity
            if (Sets.BlackthornesBattlegear.IsMaxBonusActive)
            {
                var blackthornsImmunity = new HashSet<AvoidanceType>
                {
                    AvoidanceType.Desecrator,
                    AvoidanceType.MoltenBall,
                    AvoidanceType.MoltenCore,
                    AvoidanceType.MoltenTrail,
                    AvoidanceType.PlagueHand
                };

                if (blackthornsImmunity.Contains(avoidanceType))
                {
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because BlackthornesBattlegear is equipped", avoidanceType);
                    minAvoidanceHealth = 0;
                }
            }

            if (minAvoidanceHealth == 0)
            {
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance! Name={0} SNO={1} radius={2:0} health={3:0.00} dist={4:0}",
                       CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO, minAvoidanceRadius, minAvoidanceHealth, CurrentCacheObject.Distance);
                return false;
            }

            //if (avoidanceType == AvoidanceType.Arcane)
            //{
            //    var rotator = new Rotator
            //    {
            //        RotateDuration = TimeSpan.FromSeconds(10),
            //        RotateAmount = 360,
            //        RotateAntiClockwise = CurrentCacheObject.InternalNameLowerCase.Contains("_reverse"),
            //        DebugLogging = true,
            //        StartAngleDegrees = CurrentCacheObject.Object.Movement.RotationDegrees
            //    };

            //    TrinityCacheObject.CreateRotator(CurrentCacheObject.ACDId, rotator);
            //}

            //Logger.LogDebug(LogCategory.Avoidance, "{0} Distance={1:0} {2}! {3} ({4})",
            //    (avoidanceType == AvoidanceType.None) ? CurrentCacheObject.Animation.ToString() : avoidanceType.ToString(),
            //    CurrentCacheObject.Distance,
            //    minAvoidanceHealth >= Player.CurrentHealthPct ? "Adding" : "Ignoring",
            //    CurrentCacheObject.InternalName, CurrentCacheObject.ActorSnoId);

            // Add it to the list of known avoidance objects, *IF* our health is lower than this avoidance health limit
            if (minAvoidanceHealth >= Player.CurrentHealthPct)
            {
                float avoidanceRadius = (float)GetAvoidanceRadius(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);

                TimeSpan aoeExpiration;
                DataDictionary.AvoidanceSpawnerDuration.TryGetValue(CurrentCacheObject.ActorSNO, out aoeExpiration);

                var cacheObstacle = new CacheObstacleObject(CurrentCacheObject.Position, avoidanceRadius, CurrentCacheObject.ActorSNO, CurrentCacheObject.InternalName)
                {
                    Expires = DateTime.UtcNow.Add(aoeExpiration),
                    ObjectType = TrinityObjectType.Avoidance,
                    Rotation = CurrentCacheObject.Rotation,
                    RActorGUID = CurrentCacheObject.RActorGuid,
                    ACDGuid = CurrentCacheObject.ACDGuid,
                };

                if (CurrentCacheObject.AvoidanceType == AvoidanceType.Arcane)
                {
                    var rotator = new Rotator
                    {
                        RotateDuration = TimeSpan.FromSeconds(10),
                        RotateAmount = 360,
                        RotateAntiClockwise = cacheObstacle.Name.ToLower().Contains("_reverse"),
                        DebugLogging = false,
                        StartAngleDegrees = cacheObstacle.Rotation
                    };

                    CacheObstacleObject.CreateRotator(cacheObstacle.RActorGUID, rotator);
                }

                CacheData.TimeBoundAvoidance.Add(cacheObstacle);

                // Is this one under our feet? If so flag it up so we can find an avoidance spot
                if (CurrentCacheObject.Distance <= minAvoidanceRadius)
                {
                    _currentAvoidance = CurrentCacheObject;
                    _currentAvoidance.AvoidanceRadius = avoidanceRadius;
                    _currentAvoidance.AvoidanceHealth = minAvoidanceHealth;
                    _currentAvoidanceName = CurrentCacheObject.InternalName;
                    _standingInAvoidance = true;

                    // Note if this is a travelling projectile or not so we can constantly update our safe points
                    if (DataDictionary.AvoidanceProjectiles.Contains(CurrentCacheObject.ActorSNO))
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.Avoidance, "Is standing in avoidance for projectile Name={0} SNO={1} radius={2:0} health={3:0.00} dist={4:0}",
                           CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO, minAvoidanceRadius, minAvoidanceHealth, CurrentCacheObject.Distance);
                    }
                    else
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.Avoidance, "Is standing in avoidance Name={0} SNO={1} radius={2:0} health={3:0.00} dist={4:0}",
                            CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO, minAvoidanceRadius, minAvoidanceHealth, CurrentCacheObject.Distance);
                    }
                }
            }

            return true;
        }

        private static double GetAvoidanceHealth(int actorSNO = -1)
        {
            // snag our SNO from cache variable if not provided
            if (actorSNO == -1)
                actorSNO = CurrentCacheObject.ActorSNO;
            try
            {
                if (actorSNO != -1)
                    return OldAvoidanceManager.GetAvoidanceHealthBySNO(CurrentCacheObject.ActorSNO, 1);
                return OldAvoidanceManager.GetAvoidanceHealthBySNO(actorSNO, 1);
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.Avoidance, "Exception getting avoidance radius for sno={0}", actorSNO);
                Logger.Log(TrinityLogLevel.Info, LogCategory.Avoidance, ex.ToString());
                // 100% unless specified
                return 1;
            }
        }
        private static double GetAvoidanceRadius(int actorSNO = -1, float radius = -1f)
        {
            if (actorSNO == -1)
                actorSNO = CurrentCacheObject.ActorSNO;

            if (radius == -1f)
                radius = 20f;

            try
            {
                return OldAvoidanceManager.GetAvoidanceRadiusBySNO(actorSNO, radius);
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.Avoidance, "Exception getting avoidance radius for sno={0} radius={1}", actorSNO, radius);
                Logger.Log(TrinityLogLevel.Info, LogCategory.Avoidance, ex.ToString());
                return radius;
            }

        }
    }
}
