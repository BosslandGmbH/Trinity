using System;
using System.Collections.Generic;
using System.Web.SessionState;
using Trinity.Config.Combat;
using Trinity.Helpers;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    public static class OldAvoidanceManager
    {
        public static AvoidanceType GetAvoidanceType(int actorSno)
        {
            if (Trinity.Settings.Advanced.UseExperimentalAvoidance)
                return AvoidanceType.None;

            AvoidanceType type;
            if (DataDictionary.AvoidanceTypeSNO.TryGetValue(actorSno, out type)) 
                return type;
            
            return default(AvoidanceType);
        }

        public static bool CheckPositionForArcane(Rotator rotator, Vector3 arcanePosition, Vector3 testPosition)
        {
            if (rotator == null) return false;
            var angle = rotator.Angle;
            var arcStartRadian = (float)MathUtil.DegreeToRadian(angle);

            if (rotator.RotateAntiClockwise)
            {
                arcStartRadian = (float)MathUtil.DegreeToRadian(MathUtil.FixAngleTo360(rotator.Angle - 35));        
            }

            return MathUtil.PositionIsInsideArc(testPosition, arcanePosition, 25f, arcStartRadian, 35);
        }

        public static float GetAvoidanceRadius(AvoidanceType type, float defaultValue)
        {
            if (Trinity.Settings.Advanced.UseExperimentalAvoidance)
                return 0;

            switch (type)
            {
                case AvoidanceType.Arcane:
                    return Trinity.Settings.Combat.AvoidanceRadius.Arcane;
                case AvoidanceType.AzmodanBody:
                    return Trinity.Settings.Combat.AvoidanceRadius.AzmoBodies;
                case AvoidanceType.AzmoFireball:
                    return Trinity.Settings.Combat.AvoidanceRadius.AzmoFireBall;
                case AvoidanceType.AzmodanPool:
                    return Trinity.Settings.Combat.AvoidanceRadius.AzmoPools;
                case AvoidanceType.BeastCharge:
                    return 1;
                case AvoidanceType.BeeWasp:
                    return Trinity.Settings.Combat.AvoidanceRadius.BeesWasps;
                case AvoidanceType.Belial:
                    return Trinity.Settings.Combat.AvoidanceRadius.Belial;
                case AvoidanceType.ButcherFloorPanel:
                    return Trinity.Settings.Combat.AvoidanceRadius.ButcherFloorPanel;
                case AvoidanceType.Desecrator:
                    return Trinity.Settings.Combat.AvoidanceRadius.Desecrator;
                case AvoidanceType.DiabloMeteor:
                    return Trinity.Settings.Combat.AvoidanceRadius.DiabloMeteor;
                case AvoidanceType.DiabloPrison:
                    return Trinity.Settings.Combat.AvoidanceRadius.DiabloPrison;
                case AvoidanceType.DiabloRingOfFire:
                    return Trinity.Settings.Combat.AvoidanceRadius.DiabloRingOfFire;
                case AvoidanceType.FireChains:
                    return 1;
                case AvoidanceType.FrozenPulse:
                    return Trinity.Settings.Combat.AvoidanceRadius.FrozenPulse;
                case AvoidanceType.GhomGas:
                    return Trinity.Settings.Combat.AvoidanceRadius.GhomGas;
                case AvoidanceType.Grotesque:
                    return Trinity.Settings.Combat.AvoidanceRadius.Grotesque;
                case AvoidanceType.IceBall:
                    return Trinity.Settings.Combat.AvoidanceRadius.IceBalls;
                case AvoidanceType.IceTrail:
                    return Trinity.Settings.Combat.AvoidanceRadius.IceTrail;
                case AvoidanceType.Orbiter:
                    return Trinity.Settings.Combat.AvoidanceRadius.Orbiter;
                case AvoidanceType.MageFire:
                    return Trinity.Settings.Combat.AvoidanceRadius.MageFire;
                case AvoidanceType.MaghdaProjectille:
                    return Trinity.Settings.Combat.AvoidanceRadius.MaghdaProjectille;
                case AvoidanceType.MoltenCore:
                    return Trinity.Settings.Combat.AvoidanceRadius.MoltenCore;
                case AvoidanceType.MoltenTrail:
                    return Trinity.Settings.Combat.AvoidanceRadius.MoltenTrail;
                case AvoidanceType.Mortar:
                    return defaultValue;
                case AvoidanceType.MoltenBall:
                    return Trinity.Settings.Combat.AvoidanceRadius.MoltenBall;
                case AvoidanceType.PlagueCloud:
                    return Trinity.Settings.Combat.AvoidanceRadius.PlagueCloud;
                case AvoidanceType.PlagueHand:
                    return Trinity.Settings.Combat.AvoidanceRadius.PlagueHands;
                case AvoidanceType.PoisonTree:
                    return Trinity.Settings.Combat.AvoidanceRadius.PoisonTree;
                case AvoidanceType.PoisonEnchanted:
                    return Trinity.Settings.Combat.AvoidanceRadius.PoisonEnchanted;
                case AvoidanceType.ShamanFire:
                    return Trinity.Settings.Combat.AvoidanceRadius.ShamanFire;
                case AvoidanceType.Thunderstorm:
                    return Trinity.Settings.Combat.AvoidanceRadius.Thunderstorm;
                case AvoidanceType.Wormhole:
                    return Trinity.Settings.Combat.AvoidanceRadius.Wormhole;
                case AvoidanceType.ZoltBubble:
                    return Trinity.Settings.Combat.AvoidanceRadius.ZoltBubble;
                case AvoidanceType.ZoltTwister:
                    return Trinity.Settings.Combat.AvoidanceRadius.ZoltTwister;
                default:
                    {
                        //Logger.Log(TrinityLogLevel.Error, LogCategory.Avoidance, "Unknown Avoidance type in Radius Switch! {0}", type.ToString());
                        return defaultValue;
                    }
            }
        }

        public static float GetAvoidanceRadiusBySNO(int snoId, float defaultValue)
        {
            using (new PerformanceLogger("GetAvoidanceRadiusBySNO"))
            {
                if (DataDictionary.AvoidanceTypeSNO.ContainsKey(snoId))
                {
                    float radius = GetAvoidanceRadius(DataDictionary.AvoidanceTypeSNO[snoId], defaultValue);
                    return radius;
                }
                return defaultValue;
            }
        }

        public static float GetAvoidanceHealth(AvoidanceType type, float defaultValue)
        {
            if (Trinity.Settings.Advanced.UseExperimentalAvoidance)
                return 0;

            // Monks with Serenity up ignore all AOE's
            if (Trinity.Player.ActorClass == ActorClass.Monk && Trinity.Hotbar.Contains(SNOPower.Monk_Serenity) && Trinity.GetHasBuff(SNOPower.Monk_Serenity))
            {
                // Monks with serenity are immune
                defaultValue *= 0;
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring avoidance as a Monk with Serenity");
            }
            // Witch doctors with spirit walk available and not currently Spirit Walking will subtly ignore ice balls, arcane, desecrator & plague cloud
            if (Trinity.Player.ActorClass == ActorClass.Witchdoctor && Trinity.Hotbar.Contains(SNOPower.Witchdoctor_SpiritWalk) && Trinity.GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
            {
                if (type == AvoidanceType.IceBall || type == AvoidanceType.Arcane || type == AvoidanceType.Desecrator || type == AvoidanceType.PlagueCloud)
                {
                    // Ignore ICE/Arcane/Desc/PlagueCloud altogether with spirit walk up or available
                    defaultValue *= 0;
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring avoidance as a WitchDoctor with Spirit Walk");
                }
            }

            IAvoidanceHealth avoidanceHealth = null;
            switch (Trinity.Player.ActorClass)
            {
                case ActorClass.Barbarian:
                    avoidanceHealth = Trinity.Settings.Combat.Barbarian;
                    break;
                case ActorClass.Crusader:
                    avoidanceHealth = Trinity.Settings.Combat.Crusader;
                    break;
                case ActorClass.Monk:
                    avoidanceHealth = Trinity.Settings.Combat.Monk;
                    break;
                case ActorClass.Wizard:
                    avoidanceHealth = Trinity.Settings.Combat.Wizard;
                    break;
                case ActorClass.Witchdoctor:
                    avoidanceHealth = Trinity.Settings.Combat.WitchDoctor;
                    break;
                case ActorClass.DemonHunter:
                    avoidanceHealth = Trinity.Settings.Combat.DemonHunter;
                    break;
            }

            if (avoidanceHealth != null)
            {
                switch (type)
                {
                    case AvoidanceType.Arcane:
                        return avoidanceHealth.AvoidArcaneHealth;
                    case AvoidanceType.AzmoFireball:
                        return avoidanceHealth.AvoidAzmoFireBallHealth;
                    case AvoidanceType.AzmodanBody:
                        return avoidanceHealth.AvoidAzmoBodiesHealth;
                    case AvoidanceType.AzmodanPool:
                        return avoidanceHealth.AvoidAzmoPoolsHealth;
                    case AvoidanceType.BeastCharge:
                        return 1;
                    case AvoidanceType.BeeWasp:
                        return avoidanceHealth.AvoidBeesWaspsHealth;
                    case AvoidanceType.Belial:
                        return avoidanceHealth.AvoidBelialHealth;
                    case AvoidanceType.ButcherFloorPanel:
                        return avoidanceHealth.AvoidButcherFloorPanelHealth;
                    case AvoidanceType.Desecrator:
                        return avoidanceHealth.AvoidDesecratorHealth;
                    case AvoidanceType.DiabloMeteor:
                        return avoidanceHealth.AvoidDiabloMeteorHealth;
                    case AvoidanceType.DiabloPrison:
                        return avoidanceHealth.AvoidDiabloPrisonHealth;
                    //case AvoidanceType.DiabloRingOfFire:
                        //return avoidanceHealth.AvoidDiabloRingOfFireHealth;
                    case AvoidanceType.FireChains:
                        return 0.8f;
                    case AvoidanceType.FrozenPulse:
                        return avoidanceHealth.AvoidFrozenPulseHealth;
                    case AvoidanceType.GhomGas:
                        return avoidanceHealth.AvoidGhomGasHealth;
                    case AvoidanceType.Grotesque:
                        return avoidanceHealth.AvoidGrotesqueHealth;
                    case AvoidanceType.IceBall:
                        return avoidanceHealth.AvoidIceBallsHealth;
                    case AvoidanceType.IceTrail:
                        return avoidanceHealth.AvoidIceTrailHealth;
                    case AvoidanceType.Orbiter:
                        return avoidanceHealth.AvoidOrbiterHealth;
                    case AvoidanceType.MageFire:
                        return avoidanceHealth.AvoidMageFireHealth;
                    case AvoidanceType.MaghdaProjectille:
                        return avoidanceHealth.AvoidMaghdaProjectilleHealth;
                    case AvoidanceType.MoltenBall:
                        return avoidanceHealth.AvoidMoltenBallHealth;
                    case AvoidanceType.MoltenCore:
                        return avoidanceHealth.AvoidMoltenCoreHealth;
                    case AvoidanceType.MoltenTrail:
                        return avoidanceHealth.AvoidMoltenTrailHealth;
                    case AvoidanceType.Mortar:
                        return 0.25f;
                    case AvoidanceType.PlagueCloud:
                        return avoidanceHealth.AvoidPlagueCloudHealth;
                    case AvoidanceType.PlagueHand:
                        return avoidanceHealth.AvoidPlagueHandsHealth;
                    case AvoidanceType.PoisonEnchanted:
                        return avoidanceHealth.AvoidPoisonEnchantedHealth;
                    case AvoidanceType.PoisonTree:
                        return avoidanceHealth.AvoidPoisonTreeHealth;
                    case AvoidanceType.ShamanFire:
                        return avoidanceHealth.AvoidShamanFireHealth;
                    case AvoidanceType.Thunderstorm:
                        return avoidanceHealth.AvoidThunderstormHealth;
                    case AvoidanceType.Wormhole:
                        return avoidanceHealth.AvoidWormholeHealth;
                    case AvoidanceType.ZoltBubble:
                        return avoidanceHealth.AvoidZoltBubbleHealth;
                    case AvoidanceType.ZoltTwister:
                        return avoidanceHealth.AvoidZoltTwisterHealth;
                    default:
                        {
                            //Logger.Log(TrinityLogLevel.Error, LogCategory.Avoidance, "Unknown Avoidance type in Health Switch! {0}", type.ToString());
                            return defaultValue;
                        }
                }
            }
            return defaultValue;
        }

        public static bool IsPlayerImmune(AvoidanceType avoidanceType)
        {
            if (CacheData.Buffs.HasInvulnerableShrine)
                return true;

            if (CacheData.BuffsCache.Instance.HasBuff(SNOPower.Barbarian_IgnorePain))
            {
                switch (avoidanceType)
                {
                    case AvoidanceType.IceBall:
                        return true;
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
                        return true;
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
                        return true;
                    }
                    break;

                case AvoidanceType.FrozenPulse:
                case AvoidanceType.IceBall:
                case AvoidanceType.IceTrail:

                    // Ignore if both items are equipped
                    if (Legendary.TalismanOfAranoch.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because TalismanofAranoch is equipped", avoidanceType);
                        return true;
                    }
                    break;

                case AvoidanceType.Orbiter:
                case AvoidanceType.Thunderstorm:

                    if (Legendary.XephirianAmulet.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because XephirianAmulet is equipped", avoidanceType);
                        return true;
                    }
                    break;

                case AvoidanceType.Arcane:
                    if (Legendary.CountessJuliasCameo.IsEquipped)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Avoidance, "Ignoring Avoidance {0} because CountessJuliasCameo is equipped", avoidanceType);
                        return true;
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
                    return true;
                }
            }

            return false;
        }

        public static float GetAvoidanceHealthBySNO(int snoId, float defaultValue)
        {
            using (new PerformanceLogger("GetAvoidanceHealthbySNO"))
            {
                if (DataDictionary.AvoidanceTypeSNO.ContainsKey(snoId))
                {
                    float health = GetAvoidanceHealth(DataDictionary.AvoidanceTypeSNO[snoId], defaultValue);
                    return health;
                }

                return defaultValue;
            }
        }

        public static TrinityCacheObject CurrentSafeSpot { get; set; }

        public static bool IsLockedMovingToSafeSpot { get; set; }
    }
}
