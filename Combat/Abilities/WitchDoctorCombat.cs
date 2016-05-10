using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Framework;
using Trinity.Movement;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities
{
    public class WitchDoctorCombat : CombatBase
    {
        private static WaitTimer _bastianGeneratorWaitTimer = new WaitTimer(TimeSpan.FromSeconds(3));

        static WitchDoctorCombat()
        {
            _bastianGeneratorWaitTimer.Reset();
        }

        private static readonly HashSet<SNOPower> HarvesterDebuffs = new HashSet<SNOPower>
        {
            SNOPower.Witchdoctor_Haunt,
            SNOPower.Witchdoctor_Locust_Swarm,
            SNOPower.Witchdoctor_Piranhas,
            SNOPower.Witchdoctor_AcidCloud
        };

        private static readonly HashSet<SNOPower> HarvesterCoreDebuffs = new HashSet<SNOPower>
        {
            SNOPower.Witchdoctor_Haunt,
            SNOPower.Witchdoctor_Locust_Swarm,
        };

        public static Stopwatch VisionQuestRefreshTimer = new Stopwatch();
        public static long GetTimeSinceLastVisionQuestRefresh()
        {
            if (!VisionQuestRefreshTimer.IsRunning)
                VisionQuestRefreshTimer.Start();

            return VisionQuestRefreshTimer.ElapsedMilliseconds;
        }

        public static bool IsHellToothPetDoc
        {
            get
            {
                return Sets.HelltoothHarness.IsFullyEquipped && Legendary.TheShortMansFinger.IsEquipped &&
                  !(Runes.WitchDoctor.AngryChicken.IsActive || CacheData.Hotbar.ActivePowers.Contains(SNOPower.Witchdoctor_Hex_ChickenWalk));
            }
        }

        public static TrinityPower GetPetDocPower()
        {
            TrinityPower power = null;

            // Destructible objects
            if (UseDestructiblePower)
            {
                return DestroyObjectPower;
            }

            if (UseOOCBuff)
            {
                var dogCount = Passives.WitchDoctor.MidnightFeast.IsActive ? 4 : 3;
                if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) && TrinityPlugin.PlayerOwnedZombieDogCount < dogCount)
                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
            }

            if (CurrentTarget != null)
            {
                // Zombie Dogs
                var dogCount = Passives.WitchDoctor.MidnightFeast.IsActive ? 4 : 3;
                if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) && (TrinityPlugin.PlayerOwnedZombieDogCount < dogCount ||
                    TargetUtil.AnyElitesInRange(30f)))
                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);

                // Spirit Walk when rooted
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && (Player.IsIncapacitated || Player.IsRooted))
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);

                // Piranhas
                if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
                    (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) &&
                    LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
                    Player.PrimaryResource >= 250)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Enemies.BestCluster.Position);
                }

                // Soul Harvest
                if (CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
                    (TargetUtil.AnyElitesInRange(12) || TargetUtil.AnyMobsInRange(12, 2) || TargetUtil.IsEliteTargetInRange(12f)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
                }

                // Wall of Zombies
                if (CanCast(SNOPower.Witchdoctor_WallOfZombies) &&
                    (TargetUtil.AnyElitesInRange(15, 1) || TargetUtil.AnyMobsInRange(15, 1) ||
                    ((CurrentTarget.IsEliteRareUnique || CurrentTarget.IsTreasureGoblin || CurrentTarget.IsBoss) && CurrentTarget.RadiusDistance <= 25f)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 25f, CurrentTarget.Position);
                }

                // Haunt
                if (CanCast(SNOPower.Witchdoctor_Haunt) && Player.PrimaryResource >= 50)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Haunt, 21f, CurrentTarget.ACDGuid);
                }

                // Acid Cloud
                if (CanCast(SNOPower.Witchdoctor_AcidCloud) && Player.PrimaryResource >= 175)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, 25f, TargetUtil.GetBestClusterPoint(15f, 30f));
                }

                // Spirit walk to a safe spot to cast stuff
                var safeWalkPoint = GetSafeSpotPosition(45f);
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
                {
                    Logger.Log(LogCategory.Routine, "Tryna get to safe point!");
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk, 45f, safeWalkPoint);
                }

                Logger.Log(LogCategory.Routine, "Tryna get to safe point!");
                return new TrinityPower(SNOPower.Walk, 45f, safeWalkPoint);
            }

            // If we can't cast we go to a safe spot and stay there
            if (IsNull(null) && !Player.IsInTown &&
                 !Player.IsCastingTownPortalOrTeleport() && TargetUtil.AnyMobsInRange(60f))
            {
                var safeSpot = GetSafeSpotPosition(45f);
                power = new TrinityPower(SNOPower.Walk, 45f, safeSpot);
            }

            return power;
        }

        public static TrinityPower GetPower()
        {
            TrinityPower power = null;

            //Logger.LogNormal("Gargantuan Count = {0}", TrinityPlugin.PlayerOwnedGargantuanCount);
            //Logger.LogNormal("Fetish Count = {0}", TrinityPlugin.PlayerOwnedFetishArmyCount);

            // Destructible objects
            if (UseDestructiblePower)
            {
                return DestroyObjectPower;
            }

            bool hasRestlessGiant = Runes.WitchDoctor.RestlessGiant.IsActive;
            bool hasWrathfulProtector = Runes.WitchDoctor.WrathfulProtector.IsActive;

            // Gargantuan should be cast ASAP.
            if (CanCast(SNOPower.Witchdoctor_Gargantuan))
            {
                var hasAllGargs = TrinityPlugin.PlayerOwnedGargantuanCount != 0 && (!Legendary.TheShortMansFinger.IsEquipped || TrinityPlugin.PlayerOwnedGargantuanCount > 2);
                if (!hasAllGargs)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Gargantuan);
                }

                if (TargetUtil.AnyElitesInRange(30f) && hasWrathfulProtector) //|| hasRestlessGiant?
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Gargantuan);
                }
            }

            if (IsHellToothPetDoc)
            {
                return GetPetDocPower();
            }

            // Spam Spirit Walk
            if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && Settings.Combat.WitchDoctor.UseSpiritWalkOffCooldown)
            {
                return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
            }

            var hasAngryChicken = (Skills.WitchDoctor.Hex.IsActive && Runes.WitchDoctor.AngryChicken.IsActive) || CacheData.Hotbar.ActivePowers.Contains(SNOPower.Witchdoctor_Hex_ChickenWalk);
            if (hasAngryChicken && Sets.ManajumasWay.IsEquipped && CanCast(SNOPower.Witchdoctor_Hex) && !ZetaDia.IsInTown)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Hex);
            }

            // Zombie Dogs should be cast ASAP.
            if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) &&
            ((Legendary.TheTallMansFinger.IsEquipped && TrinityPlugin.PlayerOwnedZombieDogCount < 1) ||
            (!Legendary.TheTallMansFinger.IsEquipped && TrinityPlugin.PlayerOwnedZombieDogCount <= 2)))
            {
                return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
            }

            // Summon Fetish Army
            var isTikiTorph = Runes.WitchDoctor.TikiTorchers.IsActive;
            var hasEnoughFetishes = isTikiTorph
                ? TrinityPlugin.PlayerOwnedFetishArmyCount >= 7
                : TrinityPlugin.PlayerOwnedFetishArmyCount >= 5;

            var useFetishWithZumiSet = !hasEnoughFetishes && Sets.ZunimassasHaunt.IsFirstBonusActive || Settings.Combat.WitchDoctor.UseFetishArmyOffCooldown;
            var useFetishNormal = !hasEnoughFetishes && !Sets.ZunimassasHaunt.IsFirstBonusActive && (TargetUtil.EliteOrTrashInRange(30f) || TargetUtil.IsEliteTargetInRange(40f)) || Settings.Combat.WitchDoctor.UseFetishArmyOffCooldown;

            if (CanCast(SNOPower.Witchdoctor_FetishArmy) && (useFetishWithZumiSet || useFetishNormal))
            {
                return new TrinityPower(SNOPower.Witchdoctor_FetishArmy);
            }

            // Combat Avoidance Spells
            if (!UseOOCBuff && IsCurrentlyAvoiding)
            {
                // Spirit Walk out of AoE
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
                }

                // Soul harvest at current location while avoiding
                if (Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive && MinimumSoulHarvestCriteria(Enemies.CloseNearby))
                {
                    Skills.WitchDoctor.SoulHarvest.Cast();
                }
            }



            // Incapacitated or Rooted
            if (!UseOOCBuff && (Player.IsIncapacitated || Player.IsRooted))
            {
                // Spirit Walk
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
                }
            }

            // Combat Spells with a Target
            if (!UseOOCBuff && !IsCurrentlyAvoiding && CurrentTarget != null)
            {

                var dartTargetACDGuid = CurrentTarget.ACDGuid;
                if (Legendary.TheDaggerOfDarts.IsEquipped)
                {
                    var target = TargetUtil.GetBestPierceTarget(50f);
                    if (target != null)
                        dartTargetACDGuid = target.ACDGuid;
                }

                if (_bastianGeneratorWaitTimer.IsFinished && ShouldRefreshBastiansGeneratorBuff)
                {
                    if (Hotbar.Contains(SNOPower.Witchdoctor_CorpseSpider) && (!Runes.WitchDoctor.SpiderQueen.IsActive || TrinityPlugin.PlayerOwnedSpiderPetsCount == 0))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, 50f, CurrentTarget.ACDGuid);
                    }
                    if (Hotbar.Contains(SNOPower.Witchdoctor_PoisonDart))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, 50f, dartTargetACDGuid);
                    }
                    if (Hotbar.Contains(SNOPower.Witchdoctor_PlagueOfToads))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, 50f, CurrentTarget.ACDGuid);
                    }
                    if (Hotbar.Contains(SNOPower.Witchdoctor_Firebomb))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Firebomb, 50f, CurrentTarget.ACDGuid);
                    }
                    _bastianGeneratorWaitTimer.Reset();
                }

                // Summon Corpse Spider Queen
                if (CanCast(SNOPower.Witchdoctor_CorpseSpider) && Runes.WitchDoctor.SpiderQueen.IsActive && TrinityPlugin.PlayerOwnedSpiderPetsCount == 0)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, 60f, CurrentTarget.ACDGuid);
                }


                bool hasGraveInjustice = CacheData.Hotbar.PassiveSkills.Contains(SNOPower.Witchdoctor_Passive_GraveInjustice);



                //                Debug.Print(CacheData.Hotbar.GetSkill(SNOPower.Witchdoctor_Hex).RuneIndex.ToString());
                var isChicken = CacheData.Hotbar.ActivePowers.Contains(SNOPower.Witchdoctor_Hex_ChickenWalk);

                bool hasVisionQuest = CacheData.Hotbar.PassiveSkills.Any(s => s == SNOPower.Witchdoctor_Passive_VisionQuest);

                // Set max ranged attack range, based on Grave Injustice, and current target NOT standing in avoidance, and health > 25%
                float rangedAttackMaxRange = 30f;
                if (hasGraveInjustice && !CurrentTarget.IsStandingInAvoidance && Player.CurrentHealthPct > 0.25)
                    rangedAttackMaxRange = Math.Min(Player.GoldPickupRadius + 8f, 30f);

                // Set basic attack range, depending on whether or not we have Bears and whether or not we are a tik tank
                float basicAttackRange = 35f;
                if (hasGraveInjustice)
                    basicAttackRange = rangedAttackMaxRange;
                else if (Hotbar.Contains(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
                    basicAttackRange = 30f;
                else if (Legendary.TiklandianVisage.IsEquipped && !TikHorrifyCriteria(Enemies.BestLargeCluster))
                    basicAttackRange = 25f;
                else if (Legendary.TiklandianVisage.IsEquipped)
                    basicAttackRange = 1f;




                // Summon Pets  -----------------------------------------------------------------------

                // Hex with angry chicken, is chicken, explode!
                if (isChicken && (TargetUtil.AnyMobsInRange(12f, 1, false) || CurrentTarget.RadiusDistance <= 10f || UseDestructiblePower)) // && CanCast(SNOPower.Witchdoctor_Hex_Explode)
                {
                    //Debug.Print("Attempting to cast HEx Explosion {0}", ZetaDia.Me.UsePower(SNOPower.Witchdoctor_Hex_Explode, ZetaDia.Me.Position,
                    //    ZetaDia.WorldId));

                    return new TrinityPower(SNOPower.Witchdoctor_Hex_Explode);
                }

                bool hasJaunt = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 1);
                bool hasHonoredGuest = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 3);
                bool hasUmbralShock = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 2);
                bool hasSeverance = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 0);
                bool hasHealingJourney = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritWalk && s.RuneIndex == 4);

                // Spirit Walk for Goblins chasing
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) &&
                    CurrentTarget.IsTreasureGoblin && CurrentTarget.HitPointsPct < 0.90 && CurrentTarget.RadiusDistance <= 40f)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
                }

                // Spirit Walk < 65% Health: Healing Journey
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && hasHealingJourney && !ZetaDia.IsInTown &&
                    Player.CurrentHealthPct <= 0.65)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
                }

                // Spirit Walk < 50% Mana: Honored Guest
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && hasHonoredGuest && !ZetaDia.IsInTown &&
                    Player.PrimaryResourcePct <= 0.5)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
                }

                // Spirit Walk: Other Runes
                if (CanCast(SNOPower.Witchdoctor_SpiritWalk) && Player.CurrentHealthPct <= .5)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk);
                }

                //bool shouldRefreshVisionQuest = GetTimeSinceLastVisionQuestRefresh() > 4000;
                bool shouldRefreshVisionQuest = !GetHasBuff(SNOPower.Witchdoctor_Passive_VisionQuest) || GetTimeSinceLastVisionQuestRefresh() > 3800;

                // Vision Quest Passive
                if (hasVisionQuest && shouldRefreshVisionQuest)
                {
                    // Poison Darts 
                    if (CanCast(SNOPower.Witchdoctor_PoisonDart))
                    {
                        VisionQuestRefreshTimer.Restart();
                        return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, basicAttackRange, dartTargetACDGuid);
                    }
                    // Corpse Spiders
                    if (CanCast(SNOPower.Witchdoctor_CorpseSpider) && (!Runes.WitchDoctor.SpiderQueen.IsActive || TrinityPlugin.PlayerOwnedSpiderPetsCount == 0))
                    {
                        VisionQuestRefreshTimer.Restart();
                        return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, basicAttackRange, CurrentTarget.ACDGuid);
                    }
                    // Plague Of Toads 
                    if (CanCast(SNOPower.Witchdoctor_PlagueOfToads))
                    {
                        VisionQuestRefreshTimer.Restart();
                        return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, basicAttackRange, CurrentTarget.ACDGuid);
                    }
                    // Fire Bomb 
                    if (CanCast(SNOPower.Witchdoctor_Firebomb))
                    {
                        VisionQuestRefreshTimer.Restart();
                        return new TrinityPower(SNOPower.Witchdoctor_Firebomb, basicAttackRange, CurrentTarget.ACDGuid); ;
                    }
                }

                // Spam Horrify
                if (CanCast(SNOPower.Witchdoctor_Horrify) && Settings.Combat.WitchDoctor.SpamHorrify)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
                }

                bool hasVengefulSpirit = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SoulHarvest && s.RuneIndex == 4);
                bool hasSwallowYourSoul = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SoulHarvest && s.RuneIndex == 3);

                // START Jade Harvester -----------------------------------------------------------------------

                if (Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive)
                {
                    //LogTargetArea("BestLargeCluster", Enemies.BestLargeCluster);
                    //LogTargetArea("BestCluster", Enemies.BestCluster);
                    //LogTargetArea("Nearby", Enemies.Nearby);
                    //LogTargetArea("CloseNearby", Enemies.CloseNearby);

                    // Piranhas
                    if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
                        (TargetUtil.ClusterExists(15f, 45f) || TargetUtil.AnyElitesInRange(45f)) &&
                        LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
                        Player.PrimaryResource >= 250)
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Enemies.BestCluster.Position);
                    }

                    // Should we move to cluster for harvest
                    if (IdealSoulHarvestCriteria(Enemies.BestLargeCluster))
                    {
                        //LogTargetArea("--- Found a good harvest location...", Enemies.BestLargeCluster);
                        MoveToSoulHarvestPoint(Enemies.BestLargeCluster);
                    }

                    // Is there a slightly better position than right here
                    if (MinimumSoulHarvestCriteria(Enemies.BestCluster) && (Enemies.BestCluster.EliteCount >= 2 || Enemies.BestCluster.UnitCount > 4))
                    {
                        //LogTargetArea("--- Found an average harvest location...", Enemies.BestCluster);
                        MoveToSoulHarvestPoint(Enemies.BestCluster);
                    }

                    // Should we harvest right here?
                    if (MinimumSoulHarvestCriteria(Enemies.CloseNearby))
                    {
                        //LogTargetArea("--- Harvesting (CurrentPosition)", Enemies.CloseNearby);
                        return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
                    }

                    // Locust Swarm
                    if (CanCast(SNOPower.Witchdoctor_Locust_Swarm) && Player.PrimaryResource >= 300 &&
                        !CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 20f, CurrentTarget.ACDGuid);
                    }

                    // Acid Cloud
                    if (Skills.WitchDoctor.AcidCloud.CanCast() && Player.PrimaryResource >= 325 &&
                        LastPowerUsed != SNOPower.Witchdoctor_AcidCloud)
                    {
                        Vector3 bestClusterPoint;
                        if (Passives.WitchDoctor.GraveInjustice.IsActive)
                            bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, Math.Min(Player.GoldPickupRadius + 8f, 30f));
                        else
                            bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, 30f);

                        return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, rangedAttackMaxRange, bestClusterPoint);
                    }

                    // Haunt 
                    if (CanCast(SNOPower.Witchdoctor_Haunt) && Player.PrimaryResource >= 50 && !CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Haunt, 45f, CurrentTarget.ACDGuid);
                    }

                    if (CanCast(SNOPower.Witchdoctor_Haunt) && !CurrentTarget.IsTreasureGoblin && CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm) &&
                        CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt) && Player.PrimaryResource >= 350 && !MinimumSoulHarvestCriteria(Enemies.CloseNearby))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Haunt, 45f, CurrentTarget.ACDGuid);
                    }

                    //// Save mana for locust swarm || piranhas
                    //if (!CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm) && Player.PrimaryResource < 300)
                    //{
                    //    //Logger.LogNormal("Saving mana");
                    //    return DefaultPower;
                    //}

                }

                // END Jade Harvester -----------------------------------------------------------------------

                // START LoN Firebats -----------------------------------------------------------------------

                if (Sets.LegacyOfNightmares.IsFullyEquipped && Skills.WitchDoctor.Firebats.IsActive)
                {
                    //LogTargetArea("BestLargeCluster", Enemies.BestLargeCluster);
                    //LogTargetArea("BestCluster", Enemies.BestCluster);
                    //LogTargetArea("Nearby", Enemies.Nearby);
                    //LogTargetArea("CloseNearby", Enemies.CloseNearby);

                    // Always Remember to refresh Taeguk
                    if (Gems.Taeguk.IsEquipped && TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) > 2000)
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Firebats);
                    }

                    // Move to best Firebats point
                    if (CanCast(SNOPower.Witchdoctor_Firebats) && TargetUtil.AnyMobsInRange(40f))
                    {
                        MoveToFirebatsPoint(Enemies.BestCluster);
                    }

                    // Big Bad Voodoo
                    if (CanCast(SNOPower.Witchdoctor_BigBadVoodoo) &&
                        (Settings.Combat.WitchDoctor.UseBigBadVoodooOffCooldown || TargetUtil.AnyMobsInRange(30f)) &&
                        !GetHasBuff(SNOPower.Witchdoctor_BigBadVoodoo))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo, 30f, Enemies.BestCluster.Position);
                    }

                    // Soul Harvest
                    if (CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
                        (TargetUtil.AnyElitesInRange(16) || TargetUtil.AnyMobsInRange(16, 2) || TargetUtil.IsEliteTargetInRange(16f)))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
                    }

                    // Piranhas
                    if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
                        (TargetUtil.ClusterExists(15f, 40f) || TargetUtil.AnyElitesInRange(40f)) &&
                        LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
                        Player.PrimaryResource >= 250)
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Enemies.BestCluster.Position);
                    }

                    var batMana = TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) < 125 ? 75 : 225;
                    // Firebats: Cloud of bats 
                    if (Runes.WitchDoctor.CloudOfBats.IsActive && TargetUtil.AnyMobsInRange(Settings.Combat.WitchDoctor.FirebatsRange) &&
                        CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= batMana)
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Firebats);
                    }

                    // Firebats: Plague Bats
                    if (Runes.WitchDoctor.PlagueBats.IsActive && TargetUtil.AnyMobsInRange(15f) &&
                        CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= batMana)
                    {
                        var bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, 30f);
                        var range = Settings.Combat.WitchDoctor.FirebatsRange > 15f ? 15f : Settings.Combat.WitchDoctor.FirebatsRange;

                        return new TrinityPower(SNOPower.Witchdoctor_Firebats, range, bestClusterPoint);
                    }

                }

                // END LoN Firebats -----------------------------------------------------------------------

                // Tiklandian Visage ----------------------------------------------------------------------
                // Constantly casts Horrify and moves the middle of clusters

                if (Legendary.TiklandianVisage.IsEquipped)
                {
                    // Piranhas
                    if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
                        (TargetUtil.ClusterExists(15f, 45f) || TargetUtil.AnyElitesInRange(45f)) &&
                        LastPowerUsed != SNOPower.Witchdoctor_Piranhas &&
                        Player.PrimaryResource >= 250)
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, Enemies.BestCluster.Position);
                    }

                    //Cast Horrify before we go into the fray
                    if (CanCast(SNOPower.Witchdoctor_Horrify))
                        return new TrinityPower(SNOPower.Witchdoctor_Horrify);

                    // Should we move to a better position to fear people
                    if (TikHorrifyCriteria(Enemies.BestLargeCluster))
                        MoveToHorrifyPoint(Enemies.BestLargeCluster);


                }

                // END Tiklandian Visage ----------------------------------------------------------------------   

                // Sacrifice
                if (CanCast(SNOPower.Witchdoctor_Sacrifice) && TrinityPlugin.PlayerOwnedZombieDogCount > 0 &&
                    (TargetUtil.AnyElitesInRange(15, 1) || (CurrentTarget.IsBossOrEliteRareUnique && CurrentTarget.RadiusDistance <= 9f)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice);
                }

                // Sacrifice for Circle of Life
                bool hasCircleofLife = CacheData.Hotbar.PassiveSkills.Any(s => s == SNOPower.Witchdoctor_Passive_CircleOfLife);
                if (CanCast(SNOPower.Witchdoctor_Sacrifice) && TrinityPlugin.PlayerOwnedZombieDogCount > 0 && hasCircleofLife && TargetUtil.AnyMobsInRange(15f))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice);
                }

                // Wall of Zombies
                if (CanCast(SNOPower.Witchdoctor_WallOfZombies) &&
                    (TargetUtil.AnyElitesInRange(15, 1) || TargetUtil.AnyMobsInRange(15, 1) ||
                    ((CurrentTarget.IsEliteRareUnique || CurrentTarget.IsTreasureGoblin || CurrentTarget.IsBoss) && CurrentTarget.RadiusDistance <= 25f)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 25f, CurrentTarget.Position);
                }

                bool hasSacrifice = Hotbar.Contains(SNOPower.Witchdoctor_Sacrifice);

                // Zombie Dogs for Sacrifice
                if (hasSacrifice && CanCast(SNOPower.Witchdoctor_SummonZombieDog) &&
                    (LastPowerUsed == SNOPower.Witchdoctor_Sacrifice || TrinityPlugin.PlayerOwnedZombieDogCount <= 2) &&
                    LastPowerUsed != SNOPower.Witchdoctor_SummonZombieDog)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
                }

                // Hex with angry chicken, check if we want to shape shift and explode
                if (CanCast(SNOPower.Witchdoctor_Hex) && (TargetUtil.AnyMobsInRange(12f, 1, false) || CurrentTarget.RadiusDistance <= 10f) &&
                    hasAngryChicken)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Hex);
                }

                // Hex Spam Cast without angry chicken
                if (CanCast(SNOPower.Witchdoctor_Hex) && !hasAngryChicken &&
                   (TargetUtil.AnyElitesInRange(12) || TargetUtil.AnyMobsInRange(12, 2) || TargetUtil.IsEliteTargetInRange(18f)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Hex);
                }

                if (CanCast(SNOPower.Witchdoctor_SoulHarvest) && (TargetUtil.AnyElitesInRange(16) || TargetUtil.AnyMobsInRange(16, 2) || TargetUtil.IsEliteTargetInRange(16f)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
                }

                // Mass Confuse, elites only or big mobs or to escape on low health
                if (CanCast(SNOPower.Witchdoctor_MassConfusion) &&
                    (TargetUtil.AnyElitesInRange(12, 1) || TargetUtil.AnyMobsInRange(12, 6) || Player.CurrentHealthPct <= 0.25 || (CurrentTarget.IsBossOrEliteRareUnique && CurrentTarget.RadiusDistance <= 12f)) &&
                    !CurrentTarget.IsTreasureGoblin)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_MassConfusion, 0f, CurrentTarget.ACDGuid);
                }

                if (!Settings.Combat.WitchDoctor.UseBigBadVoodooOffCooldown)
                {
                    // Big Bad Voodoo
                    if (CanCast(SNOPower.Witchdoctor_BigBadVoodoo) &&
                        (TargetUtil.EliteOrTrashInRange(25f) || (CurrentTarget.IsBoss && CurrentTarget.Distance <= 30f) ||
                        Sets.LegacyOfNightmares.IsFullyEquipped && TargetUtil.AnyMobsInRange(40f)))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo);
                    }
                }
                else
                {
                    // Big Bad Voodo, cast whenever available
                    if (!UseOOCBuff && !Player.IsIncapacitated && CanCast(SNOPower.Witchdoctor_BigBadVoodoo))
                    {
                        return new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo);
                    }
                }
                // Grasp of the Dead
                if (CanCast(SNOPower.Witchdoctor_GraspOfTheDead) &&
                    (TargetUtil.AnyMobsInRange(30, 2) || TargetUtil.EliteOrTrashInRange(30f)) &&
                    Player.PrimaryResource >= 150)
                {
                    var bestClusterPoint = TargetUtil.GetBestClusterPoint();

                    return new TrinityPower(SNOPower.Witchdoctor_GraspOfTheDead, 25f, bestClusterPoint);
                }

                // Piranhas
                if (CanCast(SNOPower.Witchdoctor_Piranhas) && Player.PrimaryResource >= 250 &&
                    (TargetUtil.ClusterExists(15f, 45f) || TargetUtil.AnyElitesInRange(45f)) &&
                    Player.PrimaryResource >= 250)
                {
                    var bestClusterPoint = TargetUtil.GetBestClusterPoint();

                    return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 25f, bestClusterPoint);
                }

                bool hasPhobia = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 2);
                bool hasStalker = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 4);
                bool hasFaceOfDeath = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 1);
                bool hasFrighteningAspect = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 0);
                bool hasRuthlessTerror = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 3);

                float horrifyRadius = hasFaceOfDeath ? 24f : 12f;

                // Horrify when low on health
                if (CanCast(SNOPower.Witchdoctor_Horrify) && Player.CurrentHealthPct <= EmergencyHealthPotionLimit && TargetUtil.AnyMobsInRange(horrifyRadius, 3))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
                }

                // Horrify Buff at 35% health -- Freightening Aspect
                if (CanCast(SNOPower.Witchdoctor_Horrify) && Player.CurrentHealthPct <= 0.35 && hasFrighteningAspect)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
                }

                // Fetish Army
                if (CanCast(SNOPower.Witchdoctor_FetishArmy) &&
                    (TargetUtil.EliteOrTrashInRange(30f) || TargetUtil.IsEliteTargetInRange(30f) || Settings.Combat.WitchDoctor.UseFetishArmyOffCooldown))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_FetishArmy);
                }

                bool hasManitou = Runes.WitchDoctor.Manitou.IsActive;

                // Spirit Barrage Manitou
                if (CanCast(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource >= 100 &&
                    TimeSincePowerUse(SNOPower.Witchdoctor_SpiritBarrage) > 18000 && hasManitou)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage);
                }

                bool hasResentfulSpirit = Runes.WitchDoctor.ResentfulSpirits.IsActive;
                // Haunt 
                if (CanCast(SNOPower.Witchdoctor_Haunt) &&
                    Player.PrimaryResource >= 50 &&
                    !SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.Witchdoctor_Haunt) &&
                    LastPowerUsed != SNOPower.Witchdoctor_Haunt)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Haunt, 21f, CurrentTarget.ACDGuid);
                }

                //skillDict.Add("LocustSwarm", SNOPower.Witchdoctor_Locust_Swarm);

                // Locust Swarm
                if (CanCast(SNOPower.Witchdoctor_Locust_Swarm) && Player.PrimaryResource >= 300 && !Legendary.Wormwood.IsEquipped &&
                    !SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.Witchdoctor_Locust_Swarm) && LastPowerUsed != SNOPower.Witchdoctor_Locust_Swarm)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 12f, CurrentTarget.ACDGuid);
                }

                // Sacrifice for 0 Dogs
                if (CanCast(SNOPower.Witchdoctor_Sacrifice) &&
                    (Settings.Combat.WitchDoctor.ZeroDogs || !WitchDoctorHasPrimaryAttack))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice, 9f);
                }

                var zombieChargerRange = hasGraveInjustice ? Math.Min(Player.GoldPickupRadius + 8f, 11f) : 11f;

                // Zombie Charger aka Zombie bears Spams Bears @ Everything from 11feet away
                if (CanCast(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_ZombieCharger, zombieChargerRange, CurrentTarget.Position);
                }

                // Soul Harvest Any Elites or to increase buff stacks
                if (!Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive && CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
                    (TargetUtil.AnyMobsInRange(14f, GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest) + 1, false) || (hasSwallowYourSoul && Player.PrimaryResourcePct <= 0.50) || TargetUtil.IsEliteTargetInRange(14f)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
                }

                // Soul Harvest with VengefulSpirit
                if (!Sets.RaimentOfTheJadeHarvester.IsMaxBonusActive && CanCast(SNOPower.Witchdoctor_SoulHarvest) && hasVengefulSpirit &&
                    TargetUtil.AnyMobsInRange(14, 3))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
                }

                var hasVampireBats = Runes.WitchDoctor.VampireBats.IsActive;
                var hasCloudOfBats = Runes.WitchDoctor.CloudOfBats.IsActive;

                var fireBatsChannelCost = hasVampireBats ? 0 : 75;
                var fireBatsMana = TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) < 125 ? fireBatsChannelCost : 225;

                var firebatsMaintain =
                  TrinityPlugin.ObjectCache.Any(u => u.IsUnit &&
                      u.IsPlayerFacing(70f) && u.Weight > 0 &&
                      u.Distance <= 35 &&
                      SpellHistory.TimeSinceUse(SNOPower.Witchdoctor_Firebats) <= TimeSpan.FromMilliseconds(250d));

                // Fire Bats:Cloud of bats 
                if (hasCloudOfBats && (TargetUtil.AnyMobsInRange(12f)) &&
                    CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= fireBatsMana)
                {
                    var range = Settings.Combat.WitchDoctor.FirebatsRange > 12f ? 12f : Settings.Combat.WitchDoctor.FirebatsRange;

                    return new TrinityPower(SNOPower.Witchdoctor_Firebats, range, CurrentTarget.ACDGuid);
                }

                // Fire Bats fast-attack
                if (CanCast(SNOPower.Witchdoctor_Firebats) && Player.PrimaryResource >= fireBatsMana &&
                     (TargetUtil.AnyMobsInRange(Settings.Combat.WitchDoctor.FirebatsRange) || firebatsMaintain) && !hasCloudOfBats)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Firebats, Settings.Combat.WitchDoctor.FirebatsRange, CurrentTarget.Position);
                }

                // Acid Cloud
                if (CanCast(SNOPower.Witchdoctor_AcidCloud) && Player.PrimaryResource >= 175)
                {
                    Vector3 bestClusterPoint;
                    if (hasGraveInjustice)
                        bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, Math.Min(Player.GoldPickupRadius + 8f, 30f));
                    else
                        bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, 30f);

                    return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, rangedAttackMaxRange, bestClusterPoint);
                }

                bool hasWellOfSouls = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_SpiritBarrage && s.RuneIndex == 1);
                bool hasRushOfEssence = CacheData.Hotbar.PassiveSkills.Any(s => s == SNOPower.Witchdoctor_Passive_RushOfEssence);

                // Spirit Barrage + Rush of Essence
                if (CanCast(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource >= 100 &&
                    hasRushOfEssence && !hasManitou)
                {
                    if (hasWellOfSouls)
                        return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, 21f, CurrentTarget.ACDGuid);

                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, 21f, CurrentTarget.ACDGuid);
                }

                // Zombie Charger backup
                if (CanCast(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_ZombieCharger, zombieChargerRange, CurrentTarget.Position);
                }

                // Regular spirit barage
                if (CanCast(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource >= 100 && !hasManitou)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, basicAttackRange, CurrentTarget.ACDGuid);
                }

                // Poison Darts fast-attack Spams Darts when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
                if (CanCast(SNOPower.Witchdoctor_PoisonDart))
                {
                    VisionQuestRefreshTimer.Restart();
                    return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, basicAttackRange, dartTargetACDGuid);
                }
                // Corpse Spiders fast-attacks Spams Spiders when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
                if (CanCast(SNOPower.Witchdoctor_CorpseSpider) && (!Runes.WitchDoctor.SpiderQueen.IsActive || TrinityPlugin.PlayerOwnedSpiderPetsCount == 0))
                {
                    VisionQuestRefreshTimer.Restart();
                    return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, basicAttackRange, CurrentTarget.ACDGuid);
                }
                // Toads fast-attacks Spams Toads when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
                if (CanCast(SNOPower.Witchdoctor_PlagueOfToads))
                {
                    VisionQuestRefreshTimer.Restart();
                    return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, basicAttackRange, CurrentTarget.ACDGuid);
                }
                // Fire Bomb fast-attacks Spams Bomb when mana is too low (to cast bears) @12yds or @10yds if Bears avialable
                if (CanCast(SNOPower.Witchdoctor_Firebomb))
                {
                    VisionQuestRefreshTimer.Restart();
                    return new TrinityPower(SNOPower.Witchdoctor_Firebomb, basicAttackRange, CurrentTarget.ACDGuid);
                }

                //Hexing Pants Mod
                if (Legendary.HexingPantsOfMrYan.IsEquipped && CurrentTarget.IsUnit &&
                //!CanCast(SNOPower.Witchdoctor_Piranhas) && 
                CurrentTarget.RadiusDistance > 10f)
                {
                    return new TrinityPower(SNOPower.Walk, 10f, CurrentTarget.Position);
                }

                if (Legendary.HexingPantsOfMrYan.IsEquipped && CurrentTarget.IsUnit &&
                //!CanCast(SNOPower.Witchdoctor_Piranhas) && 
                CurrentTarget.RadiusDistance < 10f)
                {
                    Vector3 vNewTarget = MathEx.CalculatePointFrom(CurrentTarget.Position, Player.Position, -10f);
                    return new TrinityPower(SNOPower.Walk, 10f, vNewTarget);
                }

            }

            // Buffs
            if (UseOOCBuff)
            {
                //Spam fear at all times if Tiklandian Visage is ewquipped and fear spam is selected to keep fear buff active
                if (CanCast(SNOPower.Witchdoctor_Horrify) && Settings.Combat.WitchDoctor.SpamHorrify && Legendary.TiklandianVisage.IsEquipped)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
                }

                bool hasStalker = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Witchdoctor_Horrify && s.RuneIndex == 4);
                // Horrify Buff When not in combat for movement speed -- Stalker
                if (CanCast(SNOPower.Witchdoctor_Horrify) && hasStalker)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Horrify);
                }

                // Zombie Dogs non-sacrifice build
                if (CanCast(SNOPower.Witchdoctor_SummonZombieDog) &&
                ((Legendary.TheTallMansFinger.IsEquipped && TrinityPlugin.PlayerOwnedZombieDogCount < 1) ||
                (!Legendary.TheTallMansFinger.IsEquipped && TrinityPlugin.PlayerOwnedZombieDogCount <= 2)))
                {
                    return new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
                }

                if (CanCast(SNOPower.Witchdoctor_Gargantuan) && !hasRestlessGiant && !hasWrathfulProtector && TrinityPlugin.PlayerOwnedGargantuanCount == 0)
                {
                    return new TrinityPower(SNOPower.Witchdoctor_Gargantuan);
                }
            }

            // Default Attacks
            if (IsNull(power))
            {
                // Never use Melee (e.g. Range < 15f), only ranged attacks
                var position = StuckHandler.RandomShuffle(StuckHandler.GetCirclePoints(2, 20f, Player.Position)).FirstOrDefault();
                if (CurrentTarget != null)
                    power = new TrinityPower(SNOPower.Walk, 20f, position);
            }

            return power;
        }

        private static readonly Func<TargetArea, bool> MinimumSoulHarvestCriteria = area =>

             // Harvest is off cooldown AND at least 2 debuffs exists AND at least 40% of the units have a harvestable debuff
             CanCast(SNOPower.Witchdoctor_SoulHarvest) && ((area.TotalDebuffCount(HarvesterCoreDebuffs) >= 2 &&
             area.DebuffedCount(HarvesterCoreDebuffs) >= area.UnitCount * 0.4) ||

             // OR we're gonna die if we don't
             (Player.CurrentHealthPct <= .45 && !Skills.WitchDoctor.SpiritWalk.CanCast())) &&

             // AND there's an elite, boss or more than 3 units or greater 35% of the units within sight are within this cluster
             (area.EliteCount > 0 || area.BossCount > 0 || area.UnitCount >= 3 || area.UnitCount >= (float)Enemies.Nearby.UnitCount * 0.35);


        private static readonly Func<TargetArea, bool> IdealSoulHarvestCriteria = area =>

            // Harvest is off cooldown AND at least 7 debuffs are present (can be more than 1 per unit)
            CanCast(SNOPower.Witchdoctor_SoulHarvest) && area.TotalDebuffCount(HarvesterDebuffs) > 7 &&

            // AND average health accross units in area is more than 30%
            area.AverageHealthPct > 0.3f &&

            // AND at least 2 Elites, a boss or more than 5 units or 80% of the nearby units are within this area
            (area.EliteCount >= 2 || area.BossCount > 0 || area.UnitCount >= 5 || area.UnitCount >= (float)Enemies.Nearby.UnitCount * 0.80);

        private static readonly Func<TargetArea, bool> TikHorrifyCriteria = area =>

            //at least 2 Elites, a boss or more than 5 units or 80% of the nearby units are within this area
            (area.EliteCount >= 2 || area.UnitCount >= 5 || area.UnitCount >= (float)Enemies.Nearby.UnitCount * 0.80);

        private static readonly Func<TargetArea, bool> LonFireBatsCriteria = area =>

            //at least 1 Elites or more than 5 units or mobs inside a piranhado
            (area.EliteCount > 0 || area.UnitCount >= 5 || area.TotalDebuffCount(SNOPower.Witchdoctor_Piranhas) > 0);


        private static readonly Action<string, TargetArea> LogTargetArea = (message, area) =>
        {
            Logger.LogDebug(message + " Units={0} Elites={1} DebuffedUnits={2} TotalDebuffs={4} AvgHealth={3:#.#} ---",
                area.UnitCount,
                area.EliteCount,
                area.DebuffedCount(HarvesterDebuffs),
                area.AverageHealthPct * 100,
                area.TotalDebuffCount(HarvesterDebuffs));
        };

        private static void MoveToSoulHarvestPoint(TargetArea area)
        {
            CombatMovement.Queue(new CombatMovement
            {
                Name = "Jade Harvest Position",
                Destination = area.Position,
                OnUpdate = m =>
                {
                    // Only change destination if the new target is way better
                    if (IdealSoulHarvestCriteria(Enemies.BestLargeCluster) &&
                        Enemies.BestLargeCluster.Position.Distance(m.Destination) > 10f)
                        m.Destination = Enemies.BestLargeCluster.Position;
                },
                OnFinished = m =>
                {
                    if (MinimumSoulHarvestCriteria(Enemies.CloseNearby))
                    {
                        //LogTargetArea("--- Harvesting (CombatMovement)", area);
                        Skills.WitchDoctor.SoulHarvest.Cast();
                    }
                },
                Options = new CombatMovementOptions
                {
                    Logging = LogLevel.Verbose,
                }
            });
        }


        private static void MoveToHorrifyPoint(TargetArea area)
        {
            CombatMovement.Queue(new CombatMovement
            {
                Name = "Horrify Position",
                Destination = area.Position,
                OnUpdate = m =>
                {
                    // Only change destination if the new target is way better
                    if (TikHorrifyCriteria(Enemies.BestLargeCluster) &&
                        Enemies.BestLargeCluster.Position.Distance(m.Destination) > 15f)
                        m.Destination = Enemies.BestLargeCluster.Position;
                },
                Options = new CombatMovementOptions
                {
                    AcceptableDistance = 12f,
                    Logging = LogLevel.Verbose,
                    ChangeInDistanceLimit = 2f,
                    SuccessBlacklistSeconds = 3,
                    FailureBlacklistSeconds = 7,
                    TimeBeforeBlocked = 500
                }

            });
        }

        private static void MoveToFirebatsPoint(TargetArea area)
        {
            CombatMovement.Queue(new CombatMovement
            {
                Name = "Firebats Position",
                Destination = area.Position,
                OnUpdate = m =>
                {
                    if (CanCast(SNOPower.Witchdoctor_SpiritWalk))
                        Skills.WitchDoctor.SpiritWalk.Cast();
                    // Only change destination if the new target is way better
                    if (LonFireBatsCriteria(Enemies.BestCluster) &&
                        Enemies.BestCluster.Position.Distance(m.Destination) > 15f)
                        m.Destination = Enemies.BestCluster.Position;
                },
                Options = new CombatMovementOptions
                {
                    AcceptableDistance = 8f,
                    Logging = LogLevel.Verbose,
                    ChangeInDistanceLimit = 2f,
                    SuccessBlacklistSeconds = 3,
                    FailureBlacklistSeconds = 7,
                    TimeBeforeBlocked = 500
                }

            });
        }

        private static bool WitchDoctorHasPrimaryAttack
        {
            get
            {
                return
                    Hotbar.Contains(SNOPower.Witchdoctor_WallOfZombies) ||
                    Hotbar.Contains(SNOPower.Witchdoctor_Firebats) ||
                    Hotbar.Contains(SNOPower.Witchdoctor_AcidCloud) ||
                    Hotbar.Contains(SNOPower.Witchdoctor_ZombieCharger) ||
                    Hotbar.Contains(SNOPower.Witchdoctor_PoisonDart) ||
                    Hotbar.Contains(SNOPower.Witchdoctor_CorpseSpider) ||
                    Hotbar.Contains(SNOPower.Witchdoctor_PlagueOfToads) ||
                    Hotbar.Contains(SNOPower.Witchdoctor_Firebomb);
            }
        }

        private static TrinityPower DestroyObjectPower
        {
            get
            {

                if (Hotbar.Contains(SNOPower.Witchdoctor_Firebomb))
                    return new TrinityPower(SNOPower.Witchdoctor_Firebomb, 12f, CurrentTarget.Position);
                if (Hotbar.Contains(SNOPower.Witchdoctor_PoisonDart))
                    return new TrinityPower(SNOPower.Witchdoctor_PoisonDart, 15f, CurrentTarget.Position);
                if (Hotbar.Contains(SNOPower.Witchdoctor_ZombieCharger) && Player.PrimaryResource >= 150)
                    return new TrinityPower(SNOPower.Witchdoctor_ZombieCharger, 12f, CurrentTarget.Position);
                if (Hotbar.Contains(SNOPower.Witchdoctor_CorpseSpider))
                    return new TrinityPower(SNOPower.Witchdoctor_CorpseSpider, 12f, CurrentTarget.Position);
                if (Hotbar.Contains(SNOPower.Witchdoctor_PlagueOfToads))
                    return new TrinityPower(SNOPower.Witchdoctor_PlagueOfToads, 12f, CurrentTarget.Position);
                if (Hotbar.Contains(SNOPower.Witchdoctor_AcidCloud) && Player.PrimaryResource >= 175)
                    return new TrinityPower(SNOPower.Witchdoctor_AcidCloud, 12f, CurrentTarget.Position);

                if (Hotbar.Contains(SNOPower.Witchdoctor_Sacrifice) && Hotbar.Contains(SNOPower.Witchdoctor_SummonZombieDog) &&
                    TrinityPlugin.PlayerOwnedZombieDogCount > 0 && Settings.Combat.WitchDoctor.ZeroDogs)
                    return new TrinityPower(SNOPower.Witchdoctor_Sacrifice, 12f, CurrentTarget.Position);

                if (Hotbar.Contains(SNOPower.Witchdoctor_SpiritBarrage) && Player.PrimaryResource > 100)
                    return new TrinityPower(SNOPower.Witchdoctor_SpiritBarrage, 12f, CurrentTarget.ACDGuid);

                return DefaultPower;
            }
        }

        // This is an old, buggy version of GetSafeSportPosition (the new one is in TargetUtil).
        // For some reason this version works better fo WD, so here it is.
        private static Vector3 _lastSafeSpotPosition = Vector3.Zero;
        private static DateTime _lastSafeSpotPositionTime = DateTime.MinValue;
        private static Vector3 GetSafeSpotPosition(float distance)
        {
            // Maximum speed of changing safe spots is every 2s
            if (DateTime.UtcNow.Subtract(_lastSafeSpotPositionTime).TotalSeconds < 2)
                return _lastSafeSpotPosition;

            // If we have a position already and its still within range and still no monster there, keep it.
            var safeSpotIsClose = _lastSafeSpotPosition.Distance(Player.Position) < distance;
            var safeSpotHasNoMonster = !TargetUtil.IsPositionOnMonster(_lastSafeSpotPosition);
            if (_lastSafeSpotPosition != Vector3.Zero && safeSpotIsClose && safeSpotHasNoMonster)
                return _lastSafeSpotPosition;

            Func<Vector3, bool> isValid = p => NavHelper.CanRayCast(p) && !TargetUtil.IsPositionOnMonster(p) && p.Distance(Player.Position) > distance;

            var circlePositions = StuckHandler.GetCirclePoints(8, 10, Player.Position);

            if (distance >= 20)
                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 20, Player.Position));

            if (distance >= 30)
                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 30, Player.Position));

            var closestMonster = TargetUtil.GetClosestUnit();
            var proximityTarget = closestMonster != null ? closestMonster.Position : Player.Position;
            var validPositions = circlePositions.Where(isValid).OrderBy(p => p.Distance(proximityTarget));

            _lastSafeSpotPosition = validPositions.FirstOrDefault();
            _lastSafeSpotPositionTime = DateTime.UtcNow;
            return _lastSafeSpotPosition;
        }

    }
}
