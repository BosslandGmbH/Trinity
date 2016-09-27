//using System;
//using System.Linq;
//using Buddy.Coroutines;
//using Trinity.Config.Combat;
//using Trinity.DbProvider;
//using Trinity.Framework;
//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Framework.Helpers;
//using Trinity.Objects;
//using Trinity.Reference;
//using Trinity.Technicals;
//using Zeta.Bot;
//using Zeta.Bot.Logic;
//using Zeta.Bot.Navigation;
//using Zeta.Bot.Profile.Common;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Logger = Trinity.Technicals.Logger;

//namespace Trinity.Components.Combat.Abilities
//{
//    public class MonkCombat : CombatBase
//    {
//        private const float MaxDashingStrikeRange = 50f;
//        internal static Vector3 LastTempestRushLocation = Vector3.Zero;
//        private static DateTime _lastTargetChange = DateTime.MinValue;
//        internal static string LastConditionFailureReason = string.Empty;

//        static int _swMinTime = 4000;

//        public static MonkSetting MonkSettings
//        {
//            get { return Core.Settings.Combat.Monk; }
//        }

//        public static bool IsWolMonk
//        {
//            get
//            {
//                return Legendary.TzoKrinsGaze.IsEquipped && Legendary.PintosPride.IsEquipped &&
//                       (Legendary.IncenseTorchOfTheGrandTemple.IsEquipped || Legendary.Ingeom.IsEquipped) &&
//                       Skills.Monk.WaveOfLight.IsActive && Legendary.KyoshirosBlade.IsEquipped;
//            }
//        }

//        public static bool IsInnasEP
//        {
//            get
//            {
//                return Sets.Innas.IsFullyEquipped && Legendary.ConventionOfElements.IsEquipped &&
//                       Legendary.LefebvresSoliloquy.IsEquipped && Legendary.BindingsOfTheLesserGods.IsEquipped &&
//                       !Sets.MonkeyKingsGarb.IsFirstBonusActive && 
//                       (Legendary.TheFistOfAzturrasq.IsEquipped || Legendary.Ingeom.IsEquipped);
//            }
//        }

//        public static bool IsLTK => Sets.MonkeyKingsGarb.IsFullyEquipped 
//                                    && Skills.Monk.LashingTailKick.IsActive 
//                                    && Legendary.GyanaNaKashu.IsEquipped
//                                    && Legendary.KyoshirosSoul.IsEquipped;

//        public static bool IsThousandStormsGenerator
//        {
//            get
//            {
//                return Sets.ThousandStorms.IsFullyEquipped && Sets.ShenlongsSpirit.IsFullyEquipped &&
//                       Legendary.LefebvresSoliloquy.IsEquipped && Legendary.FlyingDragon.IsEquipped && 
//                       Skills.Monk.CycloneStrike.IsActive && Skills.Monk.BreathOfHeaven.IsActive &&
//                       Skills.Monk.DashingStrike.IsActive && Skills.Monk.Epiphany.IsActive &&                       
//                       (Skills.Monk.CripplingWave.IsActive || Skills.Monk.WayOfTheHundredFists.IsActive);
//            }
//        }

//        public static TrinityPower GetWolPower()
//        {
//            // Destructibles
//            if (UseDestructiblePower)
//                return GetMonkDestroyPower();

//            // Spam Blinding Flash
//            if (Settings.Combat.Monk.AlwaysBlindingFlash && CanCast(SNOPower.Monk_BlindingFlash))
//                return new TrinityPower(SNOPower.Monk_BlindingFlash);

//            // Spam Inner Sanctuary
//            if (Settings.Combat.Monk.AlwaysInnerSanctury && CanCast(SNOPower.X1_Monk_InnerSanctuary))
//                return new TrinityPower(SNOPower.X1_Monk_InnerSanctuary);

//            if (UseOOCBuff)
//            {
//                // Sweeping Wind
//                if (CanCast(SNOPower.Monk_SweepingWind) &&
//                    !GetHasBuff(SNOPower.Monk_SweepingWind) && Player.PrimaryResource >= 75 && (!Legendary.KyoshirosSoul.IsEquipped || Skills.Monk.SweepingWind.BuffStacks <= 2))
//                    return new TrinityPower(SNOPower.Monk_SweepingWind);

//                // Breath of Heaven OOC
//                if (CanCast(SNOPower.Monk_BreathOfHeaven) && !GetHasBuff(SNOPower.Monk_BreathOfHeaven) &&
//                    Settings.Combat.Monk.BreathOfHeavenOOC)
//                    return new TrinityPower(SNOPower.Monk_BreathOfHeaven);

//                // Mystic ally
//                if (CanCastMysticAlly())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//                }
//                // Air Ally
//                if (CanCastMysticAirAlly())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//                }
//            }

//            if (CurrentTarget != null)
//            {
//                if (DataDictionary.CorruptGrowthIds.Contains(CurrentTarget.ActorSnoId))
//                {
//                    if (CanCastSevenSidedStrike())
//                    {
//                        return new TrinityPower(SNOPower.Monk_SevenSidedStrike, 16f, CurrentTarget.Position); 
                        
//                    }
//                }

//                if (CanCast(SNOPower.Monk_SweepingWind) && !GetHasBuff(SNOPower.Monk_SweepingWind) &&
//                    Player.PrimaryResource >= 75 && (!Legendary.KyoshirosSoul.IsEquipped || Skills.Monk.SweepingWind.BuffStacks <= 2))
//                    return new TrinityPower(SNOPower.Monk_SweepingWind);

//                if (CanCastEpiphany())
//                    return new TrinityPower(SNOPower.X1_Monk_Epiphany);

//                // Mantra of Salvation
//                if (CanCastMantra(SNOPower.X1_Monk_MantraOfEvasion_v2) && !Settings.Combat.Monk.DisableMantraSpam)
//                    return new TrinityPower(SNOPower.X1_Monk_MantraOfEvasion_v2);

//                // Mantra of Conviction
//                if (CanCastMantra(SNOPower.X1_Monk_MantraOfConviction_v2) && !Settings.Combat.Monk.DisableMantraSpam)
//                    return new TrinityPower(SNOPower.X1_Monk_MantraOfConviction_v2);

//                // Mystic Ally - Air Ally
//                if (CanCastMysticAirAlly())
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);

//                // Blinding Flash
//                if (CanCastBlindingFlash())
//                    return new TrinityPower(SNOPower.Monk_BlindingFlash);

//                // Blinding Flash as a DEFENSE
//                if (CanCastBlindingFlashDefensively())
//                    return new TrinityPower(SNOPower.Monk_BlindingFlash);

//                // Breath of Heaven when needing healing or the buff
//                if (CanCastBreathOfHeavenForHealing())
//                    return new TrinityPower(SNOPower.Monk_BreathOfHeaven);

//                // Breath of Heaven for spirit - Infused with Light
//                if (CanCastBreathOfHeavenInfusedWithLight())
//                    return new TrinityPower(SNOPower.Monk_BreathOfHeaven);

//                // Wave of Light

//                var wolTarget = Player.ParticipatingInTieredLootRun
//                    ? TargetUtil.GetBestRiftValueClusterPoint(60f, .1)
//                    : TargetUtil.GetBestClusterPoint(60f);

//                if (CanCast(SNOPower.Monk_WaveOfLight) && Player.PrimaryResource >= 45 &&
//                    (!Skills.Monk.SweepingWind.IsActive || GetBuffStacks(SNOPower.Monk_SweepingWind) > 1))
//                    return new TrinityPower(SNOPower.Monk_WaveOfLight, 60f, wolTarget);

//                var safePoint = TargetUtil.GetSafeSpotPosition(45f);

//                // We couldn't find a proper safe position. We should auto attack to get some stacks of Sweeping Winds at least
//                // We will be stuck if we don't do anything
//                if ((safePoint == Vector3.Zero || safePoint.Distance(Player.Position) < 10) && CurrentTarget.Distance < 10f)
//                    return DefaultPower;

//                // Dashing Strike
//                if (CanCast(SNOPower.X1_Monk_DashingStrike) && Skills.Monk.DashingStrike.Charges > 0 &&
//                    TimeSincePowerUse(SNOPower.X1_Monk_DashingStrike) >= Settings.Combat.Monk.DashingStrikeDelay)
//                    return new TrinityPower(SNOPower.X1_Monk_DashingStrike, 45f, safePoint);

//                return new TrinityPower(SNOPower.Walk, 40f, safePoint);
//            }

//            return null;
//        }

//        public static TrinityPower GetInnasPower()
//        {
//            TrinityPower power = null;

//            // Destructibles
//            if (UseDestructiblePower)
//                return GetMonkDestroyPower();

//            // Spam Inner Sanctuary
//            if (Settings.Combat.Monk.AlwaysInnerSanctury && CanCast(SNOPower.X1_Monk_InnerSanctuary))
//                return new TrinityPower(SNOPower.X1_Monk_InnerSanctuary);

//            if (CurrentTarget != null)
//            {
//                var mysticAllyTarget = TargetUtil.LowestHealthTarget(15f, Player.Position, Skills.Monk.ExplodingPalm.SNOPower);

//                // Epiphany
//                if (CanCastEpiphany())
//                    return new TrinityPower(SNOPower.X1_Monk_Epiphany);

//                // Cyclone Strike
//                var cycloneRange = Runes.Monk.Implosion.IsActive ? 34 : 24;
//                if (CanCastCycloneStrike(cycloneRange, 50))
//                    return new TrinityPower(SNOPower.Monk_CycloneStrike);


//                // Mystic Ally only during Cold
//                if (CanCast(SNOPower.X1_Monk_MysticAlly_v2) &&
//                    (Core.Buffs.ConventionElement == Element.Cold ||
//                    GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
//                    GetHasBuff(SNOPower.ItemPassive_Unique_Ring_919_x1)) &&
//                    TargetUtil.AnyMobsInRange(25f))
//                {
//                    if (mysticAllyTarget != null)
//                        return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2, 15f, mysticAllyTarget.AcdId);
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2, 15f, CurrentTarget.AcdId);
//                }

//                // InnerSanctuary 
//                if (CanCastInnerSanctuary())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_InnerSanctuary);
//                }

//                // Spread EP 4~7.5 seconds before Cold element (.5 seconds after Lightning starts until Lightning ends)
//                if (!Settings.Combat.Monk.DisableExplodingPalm)
//                {
//                    if (IsInsideCoeTimeSpan(Element.Physical, 3500, 0))
//                    {
//                        // Make a mega-splosion
//                        if (ShouldSpreadExplodingPalm())
//                            ChangeTarget();

//                        // Exploding Palm
//                        if (CanCastExplodingPalm() && CurrentTarget.Distance <= 10f)
//                            return new TrinityPower(SNOPower.Monk_ExplodingPalm, 10f, CurrentTarget.AcdId);
//                    }
//                }

//                // Get your Assimilation stacks right before COLD COE buff
//                if (CanCast(SNOPower.Monk_WayOfTheHundredFists) && IsInsideCoeTimeSpan(Element.Physical))
//                    return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 9f, CurrentTarget.AcdId);

//                // Dashing Strike
//                if (CanCastDashingStrike)
//                    return new TrinityPower(SNOPower.X1_Monk_DashingStrike, 10f, CurrentTarget.AcdId);

//                // Check if the current target is dying so we can make a bomb out of it
//                if (CurrentTarget.HitPointsPct <= 10 &&CanCastExplodingPalm())
//                    return new TrinityPower(SNOPower.Monk_ExplodingPalm, 10f, CurrentTarget.AcdId);

//                // Things to do after casting mystic ally
//                if (TimeSincePowerUse(SNOPower.X1_Monk_MysticAlly_v2) <= 2500 && TargetUtil.AnyMobsInRange(cycloneRange))
//                {
//                    // Cyclone Strike immediately after Mystic Ally
//                    if (TimeSincePowerUse(SNOPower.X1_Monk_MysticAlly_v2) <= 1000 && Player.PrimaryResource >= 50 &&
//                        CanCast(SNOPower.Monk_CycloneStrike, CanCastFlags.NoTimer) &&
//                        SpellHistory.SpellUseCountInTime(SNOPower.Monk_CycloneStrike, TimeSpan.FromMilliseconds(1000)) < 1)
//                        return new TrinityPower(SNOPower.Monk_CycloneStrike);

//                    // EP on lowest health target
//                    if (CanCastExplodingPalm() && !mysticAllyTarget.HasDebuff(SNOPower.Monk_ExplodingPalm) &&
//                        SpellHistory.SpellUseCountInTime(SNOPower.Monk_CycloneStrike, TimeSpan.FromMilliseconds(2500)) < 1)
//                        return new TrinityPower(SNOPower.Monk_ExplodingPalm, 10f, mysticAllyTarget.AcdId);
//                }

//                // if all else fails, just punch
//                if (mysticAllyTarget != null && !CanCastDashingStrike && !CanCastCycloneStrike(cycloneRange, 50))
//                    return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 9f, mysticAllyTarget.AcdId);

//                return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 9f, CurrentTarget.AcdId);
//            }

//            if (IsNull(null) && !Player.IsInTown && TargetUtil.AnyMobsInRange(60f))
//                power = DefaultPower;

//            return power;
//        }

//        public static float MeleeAttackRange = Core.Buffs.HasBuff(SNOPower.X1_Monk_Epiphany) ? 50f : 10f;

//        public static TrinityPower GetThousandStormsGeneratorPower()
//        {
//            var isShenlongsBuffActive = Core.Buffs.HasBuff(SNOPower.P3_ItemPassive_Unique_Ring_026, 1);
//            var endlessWalkOffensiveStacks = Core.Buffs.GetBuffStacks(447541, 1);
//            var endlessWalkDefensiveStacks = Core.Buffs.GetBuffStacks(447541, 2);
//            var shouldConserveSpirit = isShenlongsBuffActive && Player.PrimaryResourcePct < 0.25 && Player.CurrentHealthPct > 0.65;

//            IsIgnoringPackSize = () => isShenlongsBuffActive && Player.CurrentHealthPct > 0.4f && TargetUtil.NumMobsInRange(Settings.Combat.Misc.TrashPackClusterRadius) >= Settings.Combat.Misc.TrashPackSize*0.5;
//            IsFocussingUnits = () => isShenlongsBuffActive && Player.CurrentHealthPct > 0.4f && Core.Targets.Entries.All(i => i.Type != TrinityObjectType.ProgressionGlobe);
//            IsAvoidanceDisabled = () => isShenlongsBuffActive && Player.CurrentHealthPct > 0.4f && !Core.Avoidance.InCriticalAvoidance(Player.Position);
//            IsKitingDisabled = () => isShenlongsBuffActive && Player.CurrentHealthPct > 0.4f && !Core.Avoidance.InCriticalAvoidance(Player.Position);

//            if (UseOOCBuff)
//            {
//                // Breath of Heaven OOC
//                if (CanCast(SNOPower.Monk_BreathOfHeaven) && !GetHasBuff(SNOPower.Monk_BreathOfHeaven) &&
//                    Settings.Combat.Monk.BreathOfHeavenOOC)
//                    return new TrinityPower(SNOPower.Monk_BreathOfHeaven, 0, 0);
//            }

//            if (CurrentTarget != null)
//            {
//                // Dashing Strike - its only for the buff so charge current target.
//                if (CanCast(SNOPower.X1_Monk_DashingStrike) && Skills.Monk.DashingStrike.Charges > 1 &&
//                    TimeSincePowerUse(SNOPower.X1_Monk_DashingStrike) >= 2750)
//                    return new TrinityPower(SNOPower.X1_Monk_DashingStrike, 45f, CurrentTarget.Position, 0, 0);

//                if (CanCastEpiphany())
//                    return new TrinityPower(SNOPower.X1_Monk_Epiphany,0,0);

//                // Breath of Heaven when needing healing or the buff
//                if (CanCastBreathOfHeavenForHealing())
//                    return new TrinityPower(SNOPower.Monk_BreathOfHeaven, 0, 0);

//                // Breath of Heaven for spirit - Infused with Light
//                if (Skills.Monk.BreathOfHeaven.CanCast() && !GetHasBuff(SNOPower.Monk_BreathOfHeaven) && Runes.Monk.InfusedWithLight.IsActive &&
//                    (TargetUtil.AnyMobsInRange(20) || TargetUtil.IsEliteTargetInRange(20)) && Player.PrimaryResourcePct < 0.9)
//                {
//                    return new TrinityPower(SNOPower.Monk_BreathOfHeaven, 0, 0);
//                }

//                var cycloneStrikeRange = Runes.Monk.Implosion.IsActive ? 34f : 24f;
//                var cycloneStrikeSpirit = Runes.Monk.EyeOfTheStorm.IsActive ? 30 : 50;
                
//                if (!isShenlongsBuffActive && !Core.BlockedCheck.IsBlocked && (DateTime.UtcNow - LastWithinBuffedSpot).TotalSeconds > 5)
//                {
//                    TrinityPower power;
//                    if (TryMoveToBuffedSpot(out power, 30f)) {
//                        return power;
//                    }
//                }

//                if (!shouldConserveSpirit && Skills.Monk.CycloneStrike.CanCast() && Player.PrimaryResourcePct < 0.85f
//                    && Skills.Monk.CycloneStrike.TimeSinceUse >= Settings.Combat.Monk.CycloneStrikeDelay
//                    && Player.PrimaryResource > cycloneStrikeSpirit 
//                    && (TargetUtil.IsPercentUnitsWithinBand(10f, cycloneStrikeRange, 0.25) || CurrentTarget.IsElite))
//                {
//                    return new TrinityPower(SNOPower.Monk_CycloneStrike,0,0);
//                }

//                if (CanCast(SNOPower.Monk_CripplingWave))
//                    return new TrinityPower(SNOPower.Monk_CripplingWave, MeleeAttackRange, CurrentTarget.AcdId, 0, 0);

//                if (CanCast(SNOPower.Monk_WayOfTheHundredFists))
//                    return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, MeleeAttackRange, CurrentTarget.AcdId, 0, 0);
//            }

//            return null;
//        }

//        public static TrinityPower GetPower()
//        {
//            TrinityPower power = null;

//            ParameterSetup();

//            // Serenity if health is low
//            if (CanCastSerenityOnLowHealth())
//            {
//                return new TrinityPower(SNOPower.Monk_Serenity);
//            }

//            if (IsThousandStormsGenerator)
//            {
//                return GetThousandStormsGeneratorPower();
//            }

//            // Destructibles
//            if (UseDestructiblePower)
//            {
//                return GetMonkDestroyPower();
//            }

//            if (!IsCurrentlyAvoiding)
//            {  
//                if (IsWolMonk)
//                {
//                    return GetWolPower();
//                }
//                if (IsInnasEP)
//                {
//                    return GetInnasPower();
//                }
//            }

//            if(Settings.Combat.Monk.AlwaysBlindingFlash && CanCast(SNOPower.Monk_BlindingFlash))
//                return new TrinityPower(SNOPower.Monk_BlindingFlash);

//            if (Settings.Combat.Monk.AlwaysInnerSanctury && CanCast(SNOPower.X1_Monk_InnerSanctuary))
//                return new TrinityPower(SNOPower.X1_Monk_InnerSanctuary);

//            if (UseOOCBuff)
//            {
//                // Epiphany: spirit regen
//                if (CanCastEpiphany())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_Epiphany);
//                }

//                // Mystic ally
//                if (CanCastMysticAlly())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//                }
//                // Air Ally
//                if (CanCastMysticAirAlly())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//                }

//                // Breath of Heaven OOC
//                if (CanCast(SNOPower.Monk_BreathOfHeaven) && !GetHasBuff(SNOPower.Monk_BreathOfHeaven) &&
//                    Settings.Combat.Monk.BreathOfHeavenOOC)
//                {
//                    return new TrinityPower(SNOPower.Monk_BreathOfHeaven);
//                }

//                // Sweeping Wind
//                if (CanCast(SNOPower.Monk_SweepingWind) && (!Legendary.KyoshirosSoul.IsEquipped || Skills.Monk.SweepingWind.BuffStacks <= 2) &&
//                    !GetHasBuff(SNOPower.Monk_SweepingWind) && Player.PrimaryResource >= 75)
//                    return new TrinityPower(SNOPower.Monk_SweepingWind);

//                // No buffs, do nothing
//                return new TrinityPower();
//            }

//            if (IsCurrentlyAvoiding)
//            {
//                // Epiphany: spirit regen, dash to targets
//                if (CanCastEpiphany())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_Epiphany);
//                }

//                // SSS to avoid death.
//                if(Legendary.BindingOfTheLost.IsEquipped && CanCast(SNOPower.Monk_SevenSidedStrike, CanCastFlags.NoTimer) && Player.PrimaryResource >= 50 * (1-Player.ResourceCostReductionPct) && TargetUtil.AnyMobsInRange(35f))
//                {
//                    return new TrinityPower(SNOPower.Monk_SevenSidedStrike, 16f, CurrentTarget.Position);
//                }

//                // Serenity if health is low
//                if (CanCastSerenityOnLowHealth())
//                {
//                    return new TrinityPower(SNOPower.Monk_Serenity);
//                }

//                // Mystic ally
//                if (CanCastMysticAlly())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//                }
//                // Air Ally
//                if (CanCastMysticAirAlly())
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//                }

//                // No buffs, do nothing
//                return default(TrinityPower);
//            }

//            // Defensive Salvation.
//            if (!Player.IsIncapacitated && (Player.CurrentHealthPct <= 0.65 || Player.IsFrozen) 
//                && CanCastMantra(SNOPower.X1_Monk_MantraOfEvasion_v2) &&
//                !Core.Buffs.HasBuff(SNOPower.X1_Monk_MantraOfEvasion_v2_Passive, 1) && 
//                Settings.Combat.Monk.DisableMantraSpam)
//                return new TrinityPower(SNOPower.X1_Monk_MantraOfEvasion_v2, 3);            

//            // Combat Section 

//            if (ShouldRefreshBastiansGeneratorBuff || ShouldRefreshSpiritGuardsBuff())
//            {
//                power = GetPrimaryPower();
//                if (power != null)
//                    return power;
//            }

//            // Epiphany: spirit regen, dash to targets
//            if (CanCastEpiphany())
//            {
//                return new TrinityPower(SNOPower.X1_Monk_Epiphany);
//            }

//            // Mystic ally
//            if (CanCastMysticAlly())
//            {
//                return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//            }

//            // Air Ally
//            if (CanCastMysticAirAlly())
//            {
//                return new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);
//            }

//            // InnerSanctuary 
//            if (CanCastInnerSanctuary())
//            {
//                return new TrinityPower(SNOPower.X1_Monk_InnerSanctuary);
//            }

//            // Blinding Flash
//            if (CanCastBlindingFlash())
//            {
//                return new TrinityPower(SNOPower.Monk_BlindingFlash);
//            }

//            // Blinding Flash as a DEFENSE
//            if (CanCastBlindingFlashDefensively())
//            {
//                return new TrinityPower(SNOPower.Monk_BlindingFlash);
//            }

//            // Sweeping Wind
//            if (CanCast(SNOPower.Monk_SweepingWind) && (!Legendary.KyoshirosSoul.IsEquipped || Skills.Monk.SweepingWind.BuffStacks <= 2) && 
//                !GetHasBuff(SNOPower.Monk_SweepingWind) && Player.PrimaryResource >= 75)
//            {
//                return new TrinityPower(SNOPower.Monk_SweepingWind);
//            }

//            // Breath of Heaven Section

//            // Breath of Heaven when needing healing or the buff
//            if (CanCastBreathOfHeavenForHealing())
//            {
//                return new TrinityPower(SNOPower.Monk_BreathOfHeaven);
//            }

//            // Breath of Heaven for spirit - Infused with Light
//            if (CanCastBreathOfHeavenInfusedWithLight())
//            {
//                return new TrinityPower(SNOPower.Monk_BreathOfHeaven);
//            }

//            // Seven Sided Strike
//            if (CanCastSevenSidedStrike())
//            {
//                return new TrinityPower(SNOPower.Monk_SevenSidedStrike, 16f, CurrentTarget.Position);
//            }

//            var cycloneStrikeRange = Runes.Monk.Implosion.IsActive ? 34f : 24f;
//            var cycloneStrikeSpirit = Runes.Monk.EyeOfTheStorm.IsActive ? 30 : 50;

//            // Cyclone Strike
//            if (CanCastCycloneStrike(cycloneStrikeRange, cycloneStrikeSpirit))
//            {
//                return new TrinityPower(SNOPower.Monk_CycloneStrike);
//            }

//            // Dashing Strike
//            if (CanCastDashingStrike)
//            {
//                if (Legendary.Jawbreaker.IsEquipped)
//                {
//                    return JawBreakerDashingStrike();
//                }

//                // Raiment set, dash costs 75 spirit and refunds a charge when it's used
//                if (Sets.ThousandStorms.IsSecondBonusActive)
//                {
//                    if ((Player.PrimaryResource >= 75 && Skills.Monk.DashingStrike.Charges >= 1) || Core.Buffs.HasCastingShrine && Sets.ThousandStorms.IsFullyEquipped) {
//                        if (CurrentTarget.IsElite)
//                            return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, CurrentTarget.Position);

//                        if (!Sets.ThousandStorms.IsFullyEquipped)
//                            return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, TargetUtil.GetBestClusterPoint());

//                        if (!Sets.ThousandStorms.IsSecondBonusActive)
//                            return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, CurrentTarget.AcdId);

//                        return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, TargetUtil.GetBestPierceTarget(50f, true).Position);
//                    }
//                }
//                else if (TargetUtil.ClusterExists(12f, MaxDashingStrikeRange, 3))
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, TargetUtil.GetDashStrikeBestClusterPoint(12f, 50f));
//                }
//            }

//            // Exploding Palm, check settings and override if it's disabled and we have infinite casting
//            if (!Settings.Combat.Monk.DisableExplodingPalm || Core.Buffs.HasCastingShrine)
//            {
//                // Make a mega-splosion
//                if (ShouldSpreadExplodingPalm())
//                {
//                    ChangeTarget();
//                }

//                // Exploding Palm
//                if (CanCastExplodingPalm())
//                {
//                    return new TrinityPower(SNOPower.Monk_ExplodingPalm, 10f, CurrentTarget.AcdId);
//                }
//            }

//            // Wave of light
//            if (CanCastWaveOfLight(WaveOfLightRange))
//            {
//                return new TrinityPower(SNOPower.Monk_WaveOfLight, WaveOfLightRange, TargetUtil.GetBestClusterPoint());
//            }

//            // Lashing Tail Kick
//            if (CanCastLashingTailKick())
//            {
//                return new TrinityPower(SNOPower.Monk_LashingTailKick, 10f, CurrentTarget.AcdId);
//            }

//            // For tempest rush re-use
//            if (CanRecastTempestRush())
//            {
//                GenerateMonkZigZag();
//                Trinity.TrinityPlugin.MaintainTempestRush = true;
//                const string trUse = "Continuing Tempest Rush for Combat";
//                //LogTempestRushStatus(trUse);
//                return new TrinityPower(SNOPower.Monk_TempestRush, 23f, ZigZagPosition, -1, 0, 0);
//            }

//            // Tempest rush at elites or groups of mobs
//            if (CanCastTempestRushAsAttack())
//            {
//                GenerateMonkZigZag();
//                Trinity.TrinityPlugin.MaintainTempestRush = true;
//                const string trUse = "Starting Tempest Rush for Combat";
//                //LogTempestRushStatus(trUse);
//                return new TrinityPower(SNOPower.Monk_TempestRush, 23f, ZigZagPosition, -1, 0, 0);
//            }

//            // 4 Mantra spam for the 4 second buff
//            if (!Settings.Combat.Monk.DisableMantraSpam)
//            {
//                if (CanCastMantra(SNOPower.X1_Monk_MantraOfConviction_v2) && Player.PrimaryResource > 40)
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MantraOfConviction_v2, 3);
//                }
//                if (CanCastMantra(SNOPower.X1_Monk_MantraOfRetribution_v2) && Player.PrimaryResource > 40)
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MantraOfRetribution_v2, 3);
//                }
//                if (CanCastMantra(SNOPower.X1_Monk_MantraOfHealing_v2) && Player.PrimaryResource > 40)
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MantraOfHealing_v2, 3);
//                }
//                if (CanCastMantra(SNOPower.X1_Monk_MantraOfEvasion_v2) && Player.PrimaryResource > 40)
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_MantraOfEvasion_v2, 3);
//                }
//            }

//            // Fists of Thunder: Static Charge and WotHF: Fists of Fury - Static Gen build
//            // Static charge is currently procced by all party members and procced a lot by fists of fury.
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_FistsofThunder) && Runes.Monk.StaticCharge.IsActive &&
//                CanCast(SNOPower.Monk_WayOfTheHundredFists) && Runes.Monk.FistsOfFury.IsActive)
//            {
//                var nearbyEnemyCount = Trinity.TrinityPlugin.Targets.Count(u => u.IsUnit && u.HitPoints > 0 && u.Distance <= 30f);
//                var currentGeneratorStep = GetCurrentComboLevel();

//                // Dashing Strike resets the current generator step, so it's the perfect opportunity to snapshot FoF's 75% proc
//                if (Skills.Monk.DashingStrike.TimeSinceUse < SpellHistory.TimeSinceGeneratorCast)
//                    return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 9f, CurrentTarget.AcdId);

//                // If we have nothing to spread to, just make sure the current enemy is charged
//                if (!CurrentTarget.HasDebuff(SNOPower.Monk_FistsofThunder) || nearbyEnemyCount <= 2)
//                {
//                    //Logger.Log(LogCategory.Behavior, "Putting Static Charge on Current Target {0}", CurrentTarget.InternalName);
//                    return new TrinityPower(SNOPower.Monk_FistsofThunder, 12f, CurrentTarget.AcdId);
//                }

//                // Spread Static Charge among enemies, unless step=0, in which case it's better to apply FoF's debuff first
//                if (ShouldSpreadStaticCharge())
//                {
//                    var target = GetNewStaticChargeTarget() ?? CurrentTarget;
//                    if (target != null && currentGeneratorStep == 0 && !target.HasDebuff(SNOPower.Monk_WayOfTheHundredFists))
//							return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 9f, target.AcdId);

//                        return new TrinityPower(SNOPower.Monk_FistsofThunder, 12f, target.AcdId);
//                }

//                // Spread WotHF among very close enemies, unless step=1 which has a measly 5% proc rate that we don't want to snapshot
//				if (CurrentTarget.HasDebuff(SNOPower.Monk_WayOfTheHundredFists) && ShouldSpreadWotHF())
//				{
//					var changeTarget = GetNewWotHFTarget() ?? CurrentTarget;
//					if(changeTarget != null && currentGeneratorStep == 1 && !changeTarget.HasDebuff(SNOPower.Monk_WayOfTheHundredFists))
//							return new TrinityPower(SNOPower.Monk_FistsofThunder, 9f, changeTarget.AcdId);

//					return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 9f, CurrentTarget.AcdId);
//				}

//                // If we don't need to spread anything, but step=1, cast FoT. Otherwise we cast WotHF
//                if (currentGeneratorStep == 1 && !CurrentTarget.HasDebuff(SNOPower.Monk_WayOfTheHundredFists))
//                    return new TrinityPower(SNOPower.Monk_FistsofThunder, 9f, CurrentTarget.AcdId);

//                return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 9f, CurrentTarget.AcdId);
//            }

//            /*
//             * Dual/Trigen Monk section
//             * 
//             * Cycle through Deadly Reach, Way of the Hundred Fists, and Fists of Thunder every 3 seconds to keep 8% passive buff up if we have Combination Strike
//             *  - or - 
//             * Keep Foresight and Blazing Fists buffs up every 30/5 seconds
//             */
//            bool hasCombinationStrike = Passives.Monk.CombinationStrike.IsActive;
//            bool isDualOrTriGen = Core.Hotbar.ActiveSkills.Count(s =>
//                s.Power == SNOPower.Monk_DeadlyReach ||
//                s.Power == SNOPower.Monk_WayOfTheHundredFists ||
//                s.Power == SNOPower.Monk_FistsofThunder ||
//                s.Power == SNOPower.Monk_CripplingWave) >= 2 && hasCombinationStrike;

//            // interval in milliseconds for Generators
//            int deadlyReachInterval = 0;
//            if (hasCombinationStrike)
//                deadlyReachInterval = 2500;
//            else if (Runes.Monk.Foresight.IsActive)
//                deadlyReachInterval = 29000;

//            int wayOfTheHundredFistsInterval = 0;
//            if (hasCombinationStrike)
//                wayOfTheHundredFistsInterval = 2500;
//            else if (Runes.Monk.BlazingFists.IsActive)
//                wayOfTheHundredFistsInterval = 4500;

//            int cripplingWaveInterval = 0;
//            if (hasCombinationStrike)
//                cripplingWaveInterval = 2500;

//            // Deadly Reach: Foresight, every 27 seconds or 2.7 seconds with combo strike
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_DeadlyReach) && (isDualOrTriGen || Runes.Monk.Foresight.IsActive) &&
//                (SpellHistory.TimeSinceUse(SNOPower.Monk_DeadlyReach) > TimeSpan.FromMilliseconds(deadlyReachInterval) ||
//                (SpellHistory.SpellUseCountInTime(SNOPower.Monk_DeadlyReach, TimeSpan.FromMilliseconds(27000)) < 3) && Runes.Monk.Foresight.IsActive))
//            {
//                return new TrinityPower(SNOPower.Monk_DeadlyReach, 16f, CurrentTarget.AcdId);
//            }

//            // Way of the Hundred Fists: Blazing Fists, every 4-5ish seconds or if we don't have 3 stacks of the buff or or 2.7 seconds with combo strike
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_WayOfTheHundredFists) && (isDualOrTriGen || Runes.Monk.BlazingFists.IsActive) &&
//                (GetBuffStacks(SNOPower.Monk_WayOfTheHundredFists) < 3 ||
//                SpellHistory.TimeSinceUse(SNOPower.Monk_WayOfTheHundredFists) > TimeSpan.FromMilliseconds(wayOfTheHundredFistsInterval)))
//            {
//                return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 16f, CurrentTarget.AcdId);
//            }

//            // Crippling Wave
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_CripplingWave) && isDualOrTriGen &&
//                SpellHistory.TimeSinceUse(SNOPower.Monk_CripplingWave) > TimeSpan.FromMilliseconds(cripplingWaveInterval))
//            {
//                return new TrinityPower(SNOPower.Monk_CripplingWave, 20f, CurrentTarget.AcdId);
//            }

//            power = GetPrimaryPower();
//            if (power != null)
//                return power;

//            // Wave of light as primary 
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_WaveOfLight))
//            {
//                return new TrinityPower(SNOPower.Monk_WaveOfLight, 16f, TargetUtil.GetBestClusterPoint());
//            }

//            // Default attacks
//            return DefaultPower;
//        }

//        private static bool CanRecastTempestRush()
//        {
//            return Player.PrimaryResource >= 15 && CanCast(SNOPower.Monk_TempestRush) &&
//                    SpellHistory.TimeSinceUse(SNOPower.Monk_TempestRush).TotalMilliseconds <= 150 &&
//                    ((Settings.Combat.Monk.TROption != TempestRushOption.MovementOnly) &&
//                    !(Settings.Combat.Monk.TROption == TempestRushOption.TrashOnly && TargetUtil.AnyElitesInRange(40f)));
//        }

//        private static void ParameterSetup()
//        {
//            if (Skills.Monk.SevenSidedStrike.IsActive && Skills.Monk.ExplodingPalm.IsActive && Sets.UlianasStratagem.IsSecondBonusActive)
//                EnergyReserve = 100;
//        }

//        private static TrinityPower GetPrimaryPower()
//        {
//            // Fists of Thunder:Thunder Clap - Fly to Target
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_FistsofThunder) && Runes.Monk.Thunderclap.IsActive && CurrentTarget.Distance > 16f)
//            {
//                return new TrinityPower(SNOPower.Monk_FistsofThunder, 30f, CurrentTarget.AcdId);
//            }

//            // Fists of Thunder
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_FistsofThunder))
//            {
//                return new TrinityPower(SNOPower.Monk_FistsofThunder, 45f, CurrentTarget.AcdId);
//            }
            
//            // Deadly Reach normal
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_DeadlyReach))
//            {
//                return new TrinityPower(SNOPower.Monk_DeadlyReach, 16f, CurrentTarget.AcdId);
//            }

//            // Way of the Hundred Fists normal
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_WayOfTheHundredFists))
//            {
//                return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 16f, CurrentTarget.AcdId);
//            }

//            // Crippling Wave Normal
//            if (!IsCurrentlyAvoiding && CanCast(SNOPower.Monk_CripplingWave))
//            {
//                return new TrinityPower(SNOPower.Monk_CripplingWave, 30f, CurrentTarget.AcdId);
//            }
//            return null;
//        }

//        private static bool CanCastMantra(SNOPower mantraPower, float combatRange = 10f)
//        {
//            return CanCast(mantraPower) && Player.PrimaryResource >= 50 && TimeSincePowerUse(mantraPower) > Settings.Combat.Monk.MantraDelay;
//        }

//        private static bool CanCastTempestRushAsAttack()
//        {
//            return !IsCurrentlyAvoiding && !Player.IsRooted && CanCast(SNOPower.Monk_TempestRush) &&
//                   (Player.PrimaryResource >= Settings.Combat.Monk.TR_MinSpirit || Player.PrimaryResource >= EnergyReserve) &&
//                   (Settings.Combat.Monk.TROption == TempestRushOption.Always ||
//                    Settings.Combat.Monk.TROption == TempestRushOption.CombatOnly ||
//                    (Settings.Combat.Monk.TROption == TempestRushOption.ElitesGroupsOnly && (TargetUtil.AnyElitesInRange(25) || TargetUtil.AnyMobsInRange(25, 2))) ||
//                    (Settings.Combat.Monk.TROption == TempestRushOption.TrashOnly && !TargetUtil.AnyElitesInRange(90f) && TargetUtil.AnyMobsInRange(40f)));
//        }

//        private static bool CanCastLashingTailKick()
//        {
//            if (IsLTK && CanCast(SNOPower.Monk_LashingTailKick) && Player.PrimaryResource >= 50)
//                return true;
                

//            return !IsCurrentlyAvoiding && CanCast(SNOPower.Monk_LashingTailKick) &&
//                   (Player.PrimaryResource >= 50 || Player.PrimaryResource >= EnergyReserve);
//        }

//        private static bool CanCastWaveOfLight(float wolRange)
//        {
//            return !IsCurrentlyAvoiding && CanCast(SNOPower.Monk_WaveOfLight) && !MonkHasNoPrimary &&
//                   (TargetUtil.AnyMobsInRange(wolRange, Settings.Combat.Monk.MinWoLTrashCount) || TargetUtil.IsEliteTargetInRange(wolRange)) &&
//                   (Player.PrimaryResource >= 75 || Player.PrimaryResource >= EnergyReserve);
//        }

//        private static bool CanCastExplodingPalm()
//        {
//            return !IsCurrentlyAvoiding &&
//                   CanCast(SNOPower.Monk_ExplodingPalm, CanCastFlags.NoTimer) &&
//                   (Player.PrimaryResource >= 40 || Player.PrimaryResource >= EnergyReserve) &&
//                   (Runes.Monk.EssenceBurn.IsActive ? !SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.Monk_ExplodingPalm) : !Skills.Monk.ExplodingPalm.IsTrackedOnUnit(CurrentTarget));
//        }

//        private static bool CanCastCycloneStrike(float cycloneStrikeRange, int cycloneStrikeSpirit)
//        {
//            var precondition = !IsCurrentlyAvoiding && CanCast(SNOPower.Monk_CycloneStrike, CanCastFlags.NoTimer);
//            if (!precondition)
//            {
//                LastConditionFailureReason = "PreCondition";
//                return false;
//            }

//            var timerCheck = TimeSincePowerUse(SNOPower.Monk_CycloneStrike) < Settings.Combat.Monk.CycloneStrikeDelay;
//            if (timerCheck)
//            {
//                LastConditionFailureReason = "timerCheck";
//                return false;
//            }

//            var resourceCheck = Player.PrimaryResource >= cycloneStrikeSpirit + EnergyReserve;
//            if (!resourceCheck)
//                return false;

//            var conditionA = ShouldRefreshBastiansSpenderBuff && TargetUtil.AnyMobsInRange(cycloneStrikeRange) ||
//                               TargetUtil.AnyElitesInRange(cycloneStrikeRange) ||
//                               TargetUtil.AnyMobsInRange(cycloneStrikeRange, Settings.Combat.Monk.MinCycloneTrashCount) ||
//                               (CurrentTarget.Distance >= 10f && CurrentTarget.Distance <= cycloneStrikeRange);

//            if (!conditionA)
//            {
//                LastConditionFailureReason = "conditionA";
//                return false;
//            }

//            var conditionB = (TargetUtil.IsPercentUnitsWithinBand(10f, cycloneStrikeRange, 0.25) ||
//                ShouldRefreshBastiansSpenderBuff || Sets.ShenlongsSpirit.IsFullyEquipped || IsInnasEP);
//            if (!conditionB)
//            {
//                LastConditionFailureReason = "conditionB";
//                return false;
//            }

//            return true;
//        }

//        private static bool ShouldRefreshSpiritGuardsBuff()
//        {
//            return Legendary.SpiritGuards.IsEquipped && SpellHistory.TimeSinceGeneratorCast >= 2200;
//        }

//        private static bool CanCastSevenSidedStrike()
//        {
//            var shouldWaitForPrimary = Settings.Combat.Monk.PrimaryBeforeSSS && !Trinity.TrinityPlugin.Targets.Any(u => u.IsUnit && u.Distance < 35f && u.HasDebuff(SNOPower.Monk_ExplodingPalm));

//            if (!shouldWaitForPrimary && Settings.Combat.Monk.SSSOffCD && (Player.PrimaryResource >= 50 || Runes.Monk.Pandemonium.IsActive) && 
//                CanCast(SNOPower.Monk_SevenSidedStrike, CanCastFlags.NoTimer) && TargetUtil.AnyMobsInRange(15))
//                return true;

//            return !IsCurrentlyAvoiding && !shouldWaitForPrimary &&
//                   (TargetUtil.AnyElitesInRange(15, 1) || Player.CurrentHealthPct <= 0.55 || Legendary.Madstone.IsEquipped || Sets.UlianasStratagem.IsMaxBonusActive) &&
//                   CanCast(SNOPower.Monk_SevenSidedStrike, CanCastFlags.NoTimer) &&
//                   (Player.PrimaryResource >= 50 || Player.PrimaryResource >= EnergyReserve);
//        }

//        private static bool CanCastBreathOfHeavenInfusedWithLight()
//        {
//            return CanCast(SNOPower.Monk_BreathOfHeaven, CanCastFlags.NoTimer) &&
//                   !GetHasBuff(SNOPower.Monk_BreathOfHeaven) && Runes.Monk.InfusedWithLight.IsActive &&
//                   (TargetUtil.AnyMobsInRange(20) || TargetUtil.IsEliteTargetInRange(20) || Player.PrimaryResource < 75);
//        }

//        private static bool CanCastBreathOfHeavenForHealing()
//        {
//            return (Player.CurrentHealthPct <= 0.6 || !GetHasBuff(SNOPower.Monk_BreathOfHeaven)) && CanCast(SNOPower.Monk_BreathOfHeaven) &&
//                   (Player.PrimaryResource >= 35 || (!CanCast(SNOPower.Monk_Serenity) && Player.PrimaryResource >= 25));
//        }

//        private static bool CanCastBlindingFlashDefensively()
//        {
//            return Player.PrimaryResource >= 10 && CanCast(SNOPower.Monk_BlindingFlash) &&
//                   Player.CurrentHealthPct <= 0.75 && TargetUtil.AnyMobsInRange(15, 1);
//        }

//        private static bool CanCastBlindingFlash()
//        {
//            return Player.PrimaryResource >= 20 && CanCast(SNOPower.Monk_BlindingFlash) &&
//                   (
//                       TargetUtil.AnyElitesInRange(15, 1) ||
//                       Player.CurrentHealthPct <= 0.4 ||
//                       (TargetUtil.AnyMobsInRange(15, 3)) ||
//                       (CurrentTarget.IsElite && CurrentTarget.RadiusDistance <= 15f));
//        }

//        private static bool CanCastInnerSanctuary()
//        {
//            return (TargetUtil.AnyMobsInRange(16f, 5) || TargetUtil.AnyElitesInRange(16f)) && CanCast(SNOPower.X1_Monk_InnerSanctuary);
//        }

//        private static bool CanCastMysticAirAlly()
//        {
//            return CanCast(SNOPower.X1_Monk_MysticAlly_v2) && Runes.Monk.AirAlly.IsActive && Player.PrimaryResource < 150;
//        }

//        private static bool CanCastMysticAlly()
//        {
//            return CanCast(SNOPower.X1_Monk_MysticAlly_v2) && TargetUtil.AnyMobsInRange(40f) && !Runes.Monk.AirAlly.IsActive && !ShouldWaitForConventionElement(Skills.Monk.MysticAlly);
//        }

//        private static bool CanCastSerenityOnLowHealth()
//        {
//            return (Player.CurrentHealthPct <= Settings.Combat.Monk.SerenityHealthPct || (Player.IsIncapacitated && Player.CurrentHealthPct <= 0.90)) && CanCast(SNOPower.Monk_Serenity);
//        }

//        private static bool CanCastEpiphany()
//        {
//            //Basic checks
//            if (!CanCast(SNOPower.X1_Monk_Epiphany) || GetHasBuff(SNOPower.X1_Monk_Epiphany) || Player.IsInTown)
//                return false;

//            // Epiphany mode is 'Off Cooldown'
//            if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenReady)
//                return true;

//            // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
//            if (CurrentTarget != null && CurrentTarget.IsTreasureGoblin && Settings.Combat.Monk.UseEpiphanyGoblin ||
//                Player.CurrentHealthPct <= 0.39 &&
//                Settings.Combat.Monk.EpiphanyEmergencyHealth || GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
//                ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned)
//                return true;

//            // Epiphany mode is 'Whenever in Combat'
//            if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenInCombat && TargetUtil.AnyMobsInRange(80f))
//                return true;

//            // Epiphany mode is 'Use when Elites are nearby'
//            if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.Normal && TargetUtil.AnyElitesInRange(80f))
//                return true;

//            // Epiphany mode is 'Hard Elites Only'
//            if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.HardElitesOnly && HardElitesPresent)
//                return true;

//            return false;

//        }

//        /// <summary>
//        /// Determines if we should change targets to apply Exploading Palm to another target
//        /// </summary>
//        internal static bool ShouldSpreadExplodingPalm()
//        {
//            if (Sets.UlianasStratagem.IsFullyEquipped)
//                return false;

//            if (CanCast(SNOPower.Monk_CycloneStrike) && Player.PrimaryResource >= 50 &&
//                TimeSincePowerUse(SNOPower.Monk_CycloneStrike) >= Settings.Combat.Monk.CycloneStrikeDelay)
//                return false;

//            if (CanCastDashingStrike)
//                return false;

//            return CurrentTarget != null && Skills.Monk.ExplodingPalm.IsActive &&

//                // Current target is valid
//                CurrentTarget.IsUnit && !CurrentTarget.IsTreasureGoblin &&
//                CurrentTarget.Type != TrinityObjectType.Shrine &&
//                CurrentTarget.Type != TrinityObjectType.Interactable &&
//                CurrentTarget.Type != TrinityObjectType.HealthWell &&
//                CurrentTarget.Type != TrinityObjectType.Door &&
//                CurrentTarget.TrinityItemType != TrinityItemType.HealthGlobe &&
//                CurrentTarget.TrinityItemType != TrinityItemType.ProgressionGlobe &&

//                // Avoid rapidly changing targets
//                DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 500 &&

//                // enough resources and mobs nearby
//                Player.PrimaryResource > 40 && TargetUtil.AnyMobsInRange(15f, 2) &&

//                // Don't bother if X or more targets already have EP
//                !TargetUtil.IsUnitsWithDebuff(15f, TargetUtil.GetBestClusterPoint(), SNOPower.Monk_ExplodingPalm, Settings.Combat.Monk.ExploadingPalmMaxMobCount);

//        }

//        /// <summary>
//        /// Determines if we should change targets to apply Static Charge to another target
//        /// </summary>
//        internal static bool ShouldSpreadStaticCharge()
//        {
//            var mobPct = TargetUtil.AnyMobsInRange(20f, 4) ?
//                Settings.Combat.Monk.StaticChargeMaxMobPct : 1;

//            var result = CurrentTarget != null && Skills.Monk.FistsOfThunder.IsActive &&

//            // Current target is valid
//            CurrentTarget.IsUnit && !CurrentTarget.IsTreasureGoblin && 
//            CurrentTarget.Type != TrinityObjectType.Shrine &&
//            CurrentTarget.Type != TrinityObjectType.Interactable &&
//            CurrentTarget.Type != TrinityObjectType.HealthWell &&
//            CurrentTarget.Type != TrinityObjectType.Door &&
//            CurrentTarget.TrinityItemType != TrinityItemType.HealthGlobe && 
//            CurrentTarget.TrinityItemType != TrinityItemType.ProgressionGlobe &&
          
//            // Avoid rapidly changing targets
//            DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 50 &&

//            // We have something to spread to
//            TargetUtil.AnyMobsInRange(20f, 2) && TargetUtil.IsUnitWithoutDebuffWithinRange(20f, SNOPower.Monk_FistsofThunder) &&

//            // Only spread if less than the settings % of monsters have debuff.
//            TargetUtil.DebuffedPercent(SNOPower.Monk_FistsofThunder, 20f) <= mobPct;            

//            //if (result)
//            //    Logger.Log(LogCategory.Behavior, "Should Spread Static Charge IsUnit={0} !Goblin={1} TargetChange={2} MobsInRange={3} UnitWithoutDebuff={4} %LessThanReq={5} ({6:0.##} / {7:0.##})", CurrentTarget.IsUnit, !CurrentTarget.IsTreasureGoblin, DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 400, TargetUtil.AnyMobsInRange(20f, 2), TargetUtil.IsUnitWithoutDebuffWithinRange(20f, SNOPower.Monk_FistsofThunder), TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f) <= mobPct, TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f), mobPct);
//            //else
//            //    Logger.Log(LogCategory.Behavior, "Should NOT Spread Static Charge IsUnit={0} !Goblin={1} TargetChange={2} MobsInRange={3} UnitWithoutDebuff={4} %LessThanReq={5} ({6:0.##} / {7:0.##})", CurrentTarget.IsUnit, !CurrentTarget.IsTreasureGoblin, DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 400, TargetUtil.AnyMobsInRange(20f, 2), TargetUtil.IsUnitWithoutDebuffWithinRange(20f, SNOPower.Monk_FistsofThunder), TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f) <= mobPct, TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f), mobPct);

//            return result;
//        }

//        internal static bool ShouldSpreadWotHF()
//        {
//            var mobPct = 40;
            
//            var result = CurrentTarget != null && Skills.Monk.WayOfTheHundredFists.IsActive &&

//            // Current target is valid
//            CurrentTarget.IsUnit && !CurrentTarget.IsTreasureGoblin && 
//            CurrentTarget.Type != TrinityObjectType.Shrine &&
//            CurrentTarget.Type != TrinityObjectType.Interactable &&
//            CurrentTarget.Type != TrinityObjectType.HealthWell &&
//            CurrentTarget.Type != TrinityObjectType.Door &&
//            CurrentTarget.TrinityItemType != TrinityItemType.HealthGlobe && 
//            CurrentTarget.TrinityItemType != TrinityItemType.ProgressionGlobe &&
          
//            // Avoid rapidly changing targets
//            DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 50 &&

//            // We have something to spread to
//            TargetUtil.AnyMobsInRange(10f, 2) && TargetUtil.IsUnitWithoutDebuffWithinRange(10f, SNOPower.Monk_WayOfTheHundredFists) &&

//            // Only spread if less than the settings % of monsters have debuff.
//            TargetUtil.DebuffedPercent(SNOPower.Monk_WayOfTheHundredFists, 10f) <= mobPct;            

//            //if (result)
//            //    Logger.Log(LogCategory.Behavior, "Should Spread Static Charge IsUnit={0} !Goblin={1} TargetChange={2} MobsInRange={3} UnitWithoutDebuff={4} %LessThanReq={5} ({6:0.##} / {7:0.##})", CurrentTarget.IsUnit, !CurrentTarget.IsTreasureGoblin, DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 400, TargetUtil.AnyMobsInRange(20f, 2), TargetUtil.IsUnitWithoutDebuffWithinRange(20f, SNOPower.Monk_FistsofThunder), TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f) <= mobPct, TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f), mobPct);
//            //else
//            //    Logger.Log(LogCategory.Behavior, "Should NOT Spread Static Charge IsUnit={0} !Goblin={1} TargetChange={2} MobsInRange={3} UnitWithoutDebuff={4} %LessThanReq={5} ({6:0.##} / {7:0.##})", CurrentTarget.IsUnit, !CurrentTarget.IsTreasureGoblin, DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 400, TargetUtil.AnyMobsInRange(20f, 2), TargetUtil.IsUnitWithoutDebuffWithinRange(20f, SNOPower.Monk_FistsofThunder), TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f) <= mobPct, TargetUtil.PercentOfMobsDebuffed(SNOPower.Monk_FistsofThunder, 20f), mobPct);

//            return result;
//        }		
		
//        /// <summary>
//        /// Blacklist the current target for 3 seconds and attempt to find a new target one
//        /// </summary>
//        internal static void ChangeTarget()
//        {
//            _lastTargetChange = DateTime.UtcNow;

//            var currentTarget = CurrentTarget;
//            var lowestHealthTarget = TargetUtil.LowestHealthTarget(15f, Core.Player.Position, Skills.Monk.ExplodingPalm.SNOPower);

//            //Logger.LogNormal("Blacklisting {0} {1} - Changing Target", CurrentTarget.InternalName, CurrentTarget.CommonData.AcdId);
//            Trinity.TrinityPlugin.Blacklist3Seconds.Add(CurrentTarget.AnnId);

//            // Would like the new target to be different than the one we just blacklisted, or be very close to dead.
//            if (lowestHealthTarget.AcdId == currentTarget.AcdId && lowestHealthTarget.HitPointsPct < 0.2) return;

//            //Combat.Targeting.CurrentTarget = lowestHealthTarget;
//            //Logger.LogNormal("Found lowest health target {0} {1} ({2:0.##}%)", CurrentTarget.InternalName, CurrentTarget.CommonData.AcdId, lowestHealthTarget.HitPointsPct * 100);
//        }

//        internal static TrinityActor GetNewStaticChargeTarget()
//        {          
//            var bossTarget = TargetUtil.ClosestUnit(20f, t => t.AcdId != CurrentTarget.AcdId && t.IsBoss && !t.HasDebuff(SNOPower.Monk_FistsofThunder));
//			if (bossTarget != null)
//			{
//				_lastTargetChange = DateTime.UtcNow;
//				Logger.Log(LogCategory.Behavior, "Blacklisting {0} {1} for 1 second", bossTarget.InternalName, bossTarget.CommonData.AcdId);
//				Trinity.TrinityPlugin.Blacklist1Second.Add(bossTarget.RActorId);
//				return bossTarget;				
//			}
//			var bestTarget = TargetUtil.ClosestUnit(20f, t => t.AcdId != CurrentTarget.AcdId && !t.HasDebuff(SNOPower.Monk_FistsofThunder));            
//            if (bestTarget == null)
//                return CurrentTarget;

//            _lastTargetChange = DateTime.UtcNow;

//            Logger.Log(LogCategory.Behavior, "Blacklisting {0} {1} for 1 second", CurrentTarget.InternalName, CurrentTarget.CommonData.AcdId);
//            Trinity.TrinityPlugin.Blacklist1Second.Add(CurrentTarget.RActorId);

//            Logger.Log(LogCategory.Behavior, "Changing target to {0} {1} (Health={2:0.##}%)", bestTarget.InternalName, bestTarget.CommonData.AcdId, bestTarget.HitPointsPct * 100);
//            return bestTarget;
//        }

//        internal static TrinityActor GetNewWotHFTarget()
//        {          
//			var bestTarget = TargetUtil.ClosestUnit(10f, t => t.AcdId != CurrentTarget.AcdId && !t.HasDebuff(SNOPower.Monk_WayOfTheHundredFists) && t.HasDebuff(SNOPower.Monk_FistsofThunder));            
//            if (bestTarget == null)
//                return CurrentTarget;

//            _lastTargetChange = DateTime.UtcNow;

//            Logger.Log(LogCategory.Behavior, "Blacklisting {0} {1} for 1 second", CurrentTarget.InternalName, CurrentTarget.CommonData.AcdId);
//            Trinity.TrinityPlugin.Blacklist1Second.Add(CurrentTarget.RActorId);

//            Logger.Log(LogCategory.Behavior, "Changing target to {0} {1} (Health={2:0.##}%)", bestTarget.InternalName, bestTarget.CommonData.AcdId, bestTarget.HitPointsPct * 100);
//            return bestTarget;
//        }
//        private static bool CanCastDashingStrike
//        {
//            get
//            {
//                var charges = Skills.Monk.DashingStrike.Charges;

//                if (IsCurrentlyAvoiding ||
//                    TimeSincePowerUse(SNOPower.X1_Monk_DashingStrike) < Settings.Combat.Monk.DashingStrikeDelay ||
//                    charges < 1 || !CanCast(SNOPower.X1_Monk_DashingStrike))
//                    return false;
                
//                if (Sets.ThousandStorms.IsSecondBonusActive && (charges > 1 && Core.Player.PrimaryResource >= 75 || Core.Buffs.HasCastingShrine))
//                    return true;

//                if (!Sets.ThousandStorms.IsSecondBonusActive && (TargetUtil.AnyMobsInRange(60f) || Core.Buffs.HasCastingShrine))
//                    return true;

//                return false;
//            }
//        }

//        /// <summary>
//        /// blahblah999's Dashing Strike with JawBreaker Item
//        /// https://www.thebuddyforum.com/demonbuddy-forum/plugins/trinity/167966-monk-trinity-mod-dash-strike-spam-rots-set-bonus-jawbreaker.html
//        /// </summary>
//        internal static TrinityPower JawBreakerDashingStrike()
//        {
//            const float procDistance = 33f;
//            var farthestTarget = TargetUtil.GetDashStrikeFarthestTarget(49f);

//            // able to cast
//            if (Skills.Monk.DashingStrike.Charges > 1 && TargetUtil.AnyMobsInRange(25f, 3) || TargetUtil.IsEliteTargetInRange(70f)) // surround by mobs or elite engaged.
//            {
//                if (farthestTarget != null) // found a target within 33-49 yards.
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, farthestTarget.Position, -1, 2, 2);
//                }
//                // no free target found, get a nearby cluster point instead.
//                var bestClusterPoint = TargetUtil.GetBestClusterPoint(15f, procDistance);
//                return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, bestClusterPoint, -1, 2, 2);
//            }

//            //usually this trigger after dash to the farthest target. dash a single mobs >30 yards, trying to dash back the cluster
//            if (Skills.Monk.DashingStrike.Charges > 1 && TargetUtil.ClusterExists(20, 50, 3))
//            {
//                var dashStrikeBestClusterPoint = TargetUtil.GetDashStrikeBestClusterPoint(20f, 50f);
//                if (dashStrikeBestClusterPoint != Core.Player.Position)
//                {
//                    return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, dashStrikeBestClusterPoint, -1, 2, 2);
//                }
//            }

//            // dash to anything which is free.               
//            if (Skills.Monk.DashingStrike.Charges > 1 && farthestTarget != null)
//            {
//                return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, farthestTarget.Position, -1, 2, 2);
//            }

//            return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange, CurrentTarget.Position, -1, 2, 2);
//        }

//        private static Vector3 _zigZagPosition = Vector3.Zero;
//        /// <summary>
//        /// The last "ZigZag" position, used with Barb Whirlwind, Monk Tempest Rush, etc.
//        /// </summary>
//        public static Vector3 ZigZagPosition
//        {
//            get { return _zigZagPosition; }
//            internal set { _zigZagPosition = value; }
//        }

//        internal static void GenerateMonkZigZag()
//        {
//            float extraDistance = CurrentTarget.RadiusDistance <= 20f ? 15f : 20f;
//            ZigZagPosition = TargetUtil.GetZigZagTarget(CurrentTarget.Position, extraDistance);
//            double direction = MathUtil.FindDirectionRadian(Player.Position, ZigZagPosition);
//            ZigZagPosition = MathEx.GetPointAt(Player.Position, 40f, (float)direction);
//            Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "Generated ZigZag {0} distance {1:0}", ZigZagPosition, ZigZagPosition.Distance(Player.Position));
//        }

//        internal static TrinityPower GetMonkDestroyPower()
//        {
//            if (Skills.Monk.DashingStrike.Charges > 1 && CanCast(SNOPower.X1_Monk_DashingStrike) && Player.PrimaryResource > 75)
//                return new TrinityPower(SNOPower.X1_Monk_DashingStrike, MaxDashingStrikeRange);

//            if (CanCast(SNOPower.Monk_FistsofThunder))
//                return new TrinityPower(SNOPower.Monk_FistsofThunder, MeleeAttackRange);

//            if (IsTempestRushReady())
//                return new TrinityPower(SNOPower.Monk_TempestRush, MeleeAttackRange);

//            if (CanCast(SNOPower.Monk_DeadlyReach))
//                return new TrinityPower(SNOPower.Monk_DeadlyReach, MeleeAttackRange);

//            if (CanCast(SNOPower.Monk_CripplingWave))
//                return new TrinityPower(SNOPower.Monk_CripplingWave, MeleeAttackRange);

//            if (CanCast(SNOPower.Monk_WayOfTheHundredFists))
//                return new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, MeleeAttackRange);
//            return DefaultPower;
//        }

//        /// <summary>
//        /// Returns true if we have a mantra and it's up, or if we don't have a Mantra at all
//        /// </summary>
//        /// <returns></returns>
//        internal static bool HasMantraAbilityAndBuff()
//        {
//            return
//                (CheckAbilityAndBuff(SNOPower.X1_Monk_MantraOfConviction_v2) ||
//                CheckAbilityAndBuff(SNOPower.X1_Monk_MantraOfEvasion_v2) ||
//                CheckAbilityAndBuff(SNOPower.X1_Monk_MantraOfHealing_v2) ||
//                CheckAbilityAndBuff(SNOPower.X1_Monk_MantraOfRetribution_v2));
//        }

//        internal static bool IsTempestRushReady()
//        {
//            if (!CanCastRecurringPower())
//                return false;

//            if (!Hotbar.Contains(SNOPower.Monk_TempestRush))
//                return false;

//            if (!Hotbar.Contains(SNOPower.Monk_TempestRush))
//                return false;

//            if (!HasMantraAbilityAndBuff())
//                return false;

//            double currentSpirit = ZetaDia.Me.CurrentPrimaryResource;

//            // Minimum 10 spirit to continue channeling tempest rush
//            if (SpellHistory.TimeSinceUse(SNOPower.Monk_TempestRush).TotalMilliseconds < 150 && currentSpirit > 10f)
//                return true;

//            // Minimum 25 Spirit to start Tempest Rush
//            if (PowerManager.CanCast(SNOPower.Monk_TempestRush) && currentSpirit > Settings.Combat.Monk.TR_MinSpirit && SpellHistory.TimeSinceUse(SNOPower.Monk_TempestRush).TotalMilliseconds > 550)
//                return true;

//            return false;
//        }

//        internal static bool CanCastRecurringPower()
//        {
//            if (Player.ActorClass != ActorClass.Monk)
//                return false;

//            if (BrainBehavior.IsVendoring)
//                return false;

//            if (Player.IsInTown)
//                return false;

//            if (ProfileManager.CurrentProfileBehavior != null)
//            {
//                Type profileBehaviorType = ProfileManager.CurrentProfileBehavior.GetType();
//                if (profileBehaviorType == typeof(UseObjectTag) ||
//                    profileBehaviorType == typeof(UsePortalTag) ||
//                    profileBehaviorType == typeof(UseWaypointTag) ||
//                    profileBehaviorType == typeof(UseTownPortalTag))
//                    return false;
//            }

//            try
//            {                
//                if (ZetaDia.Me.LoopingAnimationEndTime > 0)
//                    return false;
//            }
//            catch (Exception ex)
//            {
//                if (ex is CoroutineStoppedException)
//                    throw;
//            }

//            return true;
//        }

//        //internal static void RunOngoingPowers()
//        //{
//        //    MaintainTempestRush();
//        //}

//        //internal static void MaintainTempestRush()
//        //{
//        //    if (!IsTempestRushReady())
//        //        return;

//        //    if (Player.ActorClass != ActorClass.Monk)
//        //        return;

//        //    if (Player.IsInTown || BrainBehavior.IsVendoring)
//        //        return;

//        //    if (TrinityTownRun.IsTryingToTownPortal())
//        //        return;

//        //    if (SpellHistory.TimeSinceUse(SNOPower.Monk_TempestRush) > 150)
//        //        return;

//        //    bool shouldMaintain = false;
//        //    bool nullTarget = CurrentTarget == null;
//        //    if (!nullTarget)
//        //    {
//        //        // maintain for everything except items, doors, interactables... stuff we have to "click" on
//        //        switch (CurrentTarget.Type)
//        //        {
//        //            case TrinityObjectType.Unit:
//        //            case TrinityObjectType.Gold:
//        //            case TrinityObjectType.Avoidance:
//        //            case TrinityObjectType.Barricade:
//        //            case TrinityObjectType.Destructible:
//        //            case TrinityObjectType.HealthGlobe:
//        //            case TrinityObjectType.PowerGlobe:
//        //            case TrinityObjectType.ProgressionGlobe:
//        //                {
//        //                    if (Settings.Combat.Monk.TROption == TempestRushOption.TrashOnly &&
//        //                            (TargetUtil.AnyElitesInRange(40f) || CurrentTarget.IsElite))
//        //                        break;
//        //                    shouldMaintain = true;
//        //                }
//        //                break;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        shouldMaintain = true;
//        //    }

//        //    if (Settings.Combat.Monk.TROption != TempestRushOption.MovementOnly && CanCast(SNOPower.Monk_TempestRush) && shouldMaintain)
//        //    {
//        //        Vector3 target = LastTempestRushLocation;

//        //        const string locationSource = "LastLocation";

//        //        if (target.Distance(ZetaDia.Me.Position) <= 1f)
//        //        {
//        //            // rrrix edit: we can't maintain here
//        //            return;
//        //        }

//        //        if (target == Vector3.Zero)
//        //            return;

//        //        float destinationDistance = target.Distance(ZetaDia.Me.Position);

//        //        target = TargetUtil.FindTempestRushTarget();

//        //        if (destinationDistance > 10f && NavHelper.CanRayCast(ZetaDia.Me.Position, target))
//        //        {
//        //            LogTempestRushStatus(String.Format("Using Tempest Rush to maintain channeling, source={0}, V3={1} dist={2:0}", locationSource, target, destinationDistance));

//        //            var usePowerResult = ZetaDia.Me.UsePower(SNOPower.Monk_TempestRush, target, TrinityPlugin.CurrentWorldDynamicId);
//        //            if (usePowerResult)
//        //            {    
//        //                CacheData.AbilityLastUsed[SNOPower.Monk_TempestRush] = DateTime.UtcNow;
//        //                SpellHistory.RecordSpell(SNOPower.Monk_TempestRush);
//        //            }
//        //            else
//        //            {
//        //                Combat.TargetHandler.LastActionTimes.Add(DateTime.UtcNow);
//        //            }
//        //        }
//        //    }
//        //}

//        //internal static void LogTempestRushStatus(string trUse)
//        //{

//        //    Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "{0}, xyz={4} spirit={1:0} cd={2} lastUse={3:0}",
//        //        trUse,
//        //        Core.Player.PrimaryResource, PowerManager.CanCast(SNOPower.Monk_TempestRush),
//        //        SpellHistory.TimeSinceUse(SNOPower.Monk_TempestRush), ZigZagPosition);
//        //}

//        private static bool MonkHasNoPrimary
//        {
//            get
//            {
//                return !(Hotbar.Contains(SNOPower.Monk_CripplingWave) ||
//                    Hotbar.Contains(SNOPower.Monk_FistsofThunder) ||
//                    Hotbar.Contains(SNOPower.Monk_DeadlyReach) ||
//                    Hotbar.Contains(SNOPower.Monk_WayOfTheHundredFists));
//            }
//        }
//        private static float WaveOfLightRange => Legendary.TzoKrinsGaze.IsEquipped ? 55f : 16f;
//    }
//}
