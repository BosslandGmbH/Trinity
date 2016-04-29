using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Utilities;
using Trinity.Helpers;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;
using Trinity.Objects;
using Trinity.UI.UIComponents.RadarCanvas;

namespace Trinity.Combat.Abilities
{
    public class CrusaderCombat : CombatBase
    {

        private static bool _alternator;
        private static DateTime _coeElementTime;
        private static Element _coeCurrentElement;


        public static CrusaderSetting CrusaderSettings
        {
            get { return TrinityPlugin.Settings.Combat.Crusader; }
        }

        public static bool IsBombardmentBuild
        {
            get
            {
                return Skills.Crusader.Bombardment.IsActive && Legendary.BeltOfTheTrove.IsEquipped &&
                       !Skills.Crusader.Punish.IsActive &&
                       // Including LoN Bombardment build that uses Mortal Drama,
                       (Legendary.TheMortalDrama.IsEquipped ||
                        // Pony builds that use Norvald's Fervor
                        Sets.NorvaldsFavor.IsFullyEquipped ||
                        // Phalanx LoN build that uses Phalanx boosting items
                        Legendary.WarhelmOfKassar.IsEquipped && Legendary.UnrelentingPhalanx.IsEquipped &&
                        Legendary.EternalUnion.IsEquipped ||
                       // or just some generic variation that uses Ingeom
                       Legendary.Ingeom.IsEquipped ||
                       Legendary.BloodBrother.IsEquipped && !Sets.ThornsoftheInvoker.IsFullyEquipped);
            }
        }

        private static bool EveryOtherTick
        {
            get { return _alternator = !_alternator; }
        }

        public static bool IsInvokerPunishBuild
        {
            get { return Sets.ThornsoftheInvoker.IsFullyEquipped && Skills.Crusader.Punish.IsActive; }
        }

        public static float _invPunSteedTargetDist = 25f;

        private static readonly Func<bool> BigClusterOrElitesInRange = () => TargetUtil.AnyElitesInRange(20f) || TargetUtil.NumMobsInRange(30f) >= 10;

        /// <summary>
        /// DarkFiend's Invoker Behavior
        /// </summary>
        public static TrinityPower GetInvokerPower()
        {
            //Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "InvokerPunish call ...", true);

            TrinityPower power = null;

            if (Player.IsCastingPortal)
                return null;

            // Destructibles
            if (UseDestructiblePower)
                return DestroyObjectPower;

            if (CurrentTarget == null)
            {
                // LawsOfJustice
                if (CanCast(SNOPower.X1_Crusader_LawsOfJustice2) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice2, 6) &&
                    (TargetUtil.EliteOrTrashInRange(16f) ||
                     Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct ||
                     CrusaderSettings.SpamLaws))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);
                }
            }

            else if (CurrentTarget.Type != TrinityObjectType.Avoidance)
            {
                // 8 Seconds of cluster finding while on the wrong elements.
                if (CacheData.Buffs.ConventionElement == Element.Fire && Legendary.ConventionOfElements.IsEquipped)
                {
                    CombatOverrides.ModifyTrashSizeForDuration(2d, TimeSpan.FromSeconds(8), 4, 10, BigClusterOrElitesInRange);
                }

                // Steed Charge
                if (Settings.Combat.Crusader.SpamSteedCharge && Skills.Crusader.SteedCharge.IsActive)
                {
                    if (CanCastSteedCharge())
                    {
                        return new TrinityPower(SNOPower.X1_Crusader_SteedCharge);
                    }
                }

                // LawsOfJustice
                if (CanCast(SNOPower.X1_Crusader_LawsOfJustice2) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice_Passive2, 6) &&
                    (TargetUtil.EliteOrTrashInRange(16f) ||
                     Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct ||
                     TargetUtil.AnyMobsInRange(15f, 5) ||
                     CrusaderSettings.SpamLaws))
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);

                // Akarats when off Cooldown
                if (CanCastAkaratsChampion())
                    return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);

                // Iron Skin
                if (CanCast(SNOPower.X1_Crusader_IronSkin) && !ShouldWaitForConventionofElements(Skills.Crusader.IronSkin, Element.Physical))
                    return new TrinityPower(SNOPower.X1_Crusader_IronSkin);

                // Consecration
                if (CanCast(SNOPower.X1_Crusader_Consecration) && !ShouldWaitForConventionofElements(Skills.Crusader.Consecration, Element.Physical))
                    return new TrinityPower(SNOPower.X1_Crusader_Consecration);

                // Provoke
                if (CanCast(SNOPower.X1_Crusader_Provoke) && !ShouldWaitForConventionofElements(Skills.Crusader.Provoke, Element.Physical))
                    return new TrinityPower(SNOPower.X1_Crusader_Provoke);

                // Bombardment
                if (CanCastBombardment())
                {
                    Vector3 bestPoint = CurrentTarget.IsEliteRareUnique ?
                            CurrentTarget.Position :
                            TargetUtil.GetBestClusterPoint();

                    return new TrinityPower(SNOPower.X1_Crusader_Bombardment, 16f, bestPoint);
                }

                // Ensure steed charge isn't interrupted by Punish.
                if (IsSteedCharging && CurrentTarget != null && !IsDoingGoblinKamakazi)
                {
                    return new TrinityPower(SNOPower.Walk, 100f, TargetUtil.GetZigZagTarget(CurrentTarget.Position, 16f));
                }

                // Try to punch our way out if blocked by mobs with no Steed Charge
                if (CanCast(SNOPower.X1_Crusader_Punish) && !Skills.Crusader.SteedCharge.IsActive && PlayerMover.IsBlocked)
                {
                    var closestMonster = TargetUtil.GetClosestUnit(12f);
                    if (closestMonster != null)
                    {
                        return new TrinityPower(SNOPower.X1_Crusader_Punish, 12f, closestMonster.ACDGuid);
                    }
                }

                // Primary generators
                power = GetPrimaryPower();
                if (power != null)
                    return power;
            }

            // Default Attacks
            if (IsNull(null))
                power = DefaultPower;

            return power;
        }

        ///// <summary>
        ///// Newk's Bombard Behavior
        ///// </summary>
        //private static TrinityPower GetBombardPower()
        //{
        //    TrinityPower power = null;

        //    //Logger.Log(
        //    //    "cd={0} RemainingMsToElement(Phys/1500)={1} Elapsed={2} ShouldWait(Bomb)={3}",
        //    //    Skills.Crusader.Bombardment.CooldownRemaining,
        //    //    TimeToElementStart(Element.Physical),
        //    //    TimeFromElementStart(Element.Physical),
        //    //    ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 3000)
        //    //    );

        //    if (Player.IsCastingTownPortalOrTeleport())
        //    {
        //        return null;
        //    }

        //    if (CurrentTarget == null)
        //    {
        //        if (!IsSteedCharging)
        //        {
        //            // LawsOfHope
        //            if (CanCast(SNOPower.X1_Crusader_LawsOfHope2) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope2, 6) && ZetaDia.Me.Movement.SpeedXY > 0 &&
        //                (TargetUtil.EliteOrTrashInRange(16f) ||
        //                 Player.CurrentHealthPct <= CrusaderSettings.LawsOfHopeHpPct ||
        //                 CrusaderSettings.SpamLaws))
        //            {
        //                return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
        //            }

        //            // AkaratsChampion 100% uptime setup
        //            if (CanCastAkaratsChampion() && !Runes.Crusader.Rally.IsActive && Legendary.Ingeom.IsEquipped && ZetaDia.Me.Movement.SpeedXY > 0 && !Player.IsInTown)
        //            {
        //                return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
        //            }

        //            // Condemn
        //            if (CanCast(SNOPower.X1_Crusader_Condemn) && ZetaDia.Me.Movement.SpeedXY > 0 && !Player.IsInTown)
        //            {
        //                return new TrinityPower(SNOPower.X1_Crusader_Condemn);
        //            }
        //        }

        //        // dont waste coe phase
        //        if (Settings.Combat.Misc.UseConventionElementOnly && Legendary.ConventionOfElements.IsEquipped && !GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) &&
        //            ZetaDia.Me.Movement.SpeedXY > 0 && !Player.IsInTown)
        //        {
        //            // AkaratsChampion 100% uptime setup - force
        //            if (CanCastIronSkin() && CanCastAkaratsChampion() && !Runes.Crusader.Rally.IsActive && Legendary.Ingeom.IsEquipped)
        //            {
        //                return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
        //            }

        //            // Iron Skin
        //            if (CanCastIronSkin())
        //            {
        //                return new TrinityPower(SNOPower.X1_Crusader_IronSkin);
        //            }

        //            // Bombardment
        //            if (CanCastBombardment())
        //            {
        //                return new TrinityPower(SNOPower.X1_Crusader_Bombardment);
        //            }

        //            // AkaratsChampion if rally
        //            if (CanCastAkaratsChampion() && Runes.Crusader.Rally.IsActive)
        //            {
        //                if (GetHasBuff(SNOPower.X1_Crusader_IronSkin) && Skills.Crusader.Bombardment.CooldownRemaining > 6000)
        //                    return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
        //            }
        //        }

        //        // SteedCharge
        //        if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_SteedCharge) && ZetaDia.Me.Movement.SpeedXY > 0 && !Player.IsInTown && CrusaderSettings.SteedChargeOOC)
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_SteedCharge);
        //        }
        //    }

        //    // Has Target
        //    if (CurrentTarget != null)
        //    {
        //        // AkaratsChampion
        //        if ((!IsSteedCharging || CanCastIronSkin()) && CanCastAkaratsChampion() && !Runes.Crusader.Rally.IsActive)
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
        //        }

        //        // Iron Skin
        //        if (CanCastIronSkin())
        //        {
        //            //Logger.Log("IronSkin");
        //            return new TrinityPower(SNOPower.X1_Crusader_IronSkin);
        //        }

        //        // Bombardment
        //        if (CanCastBombardment())
        //        {
        //            if (IsBombardmentBuild && Runes.Crusader.Critical.IsActive && CanCast(SNOPower.X1_Crusader_LawsOfValor2))
        //                return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);

        //            var bestPoint = CurrentTarget.IsEliteRareUnique
        //                ? CurrentTarget.Position
        //                : TargetUtil.GetBestClusterPoint();

        //            //Logger.Log("Bombardment");
        //            return new TrinityPower(SNOPower.X1_Crusader_Bombardment, 16f, bestPoint);
        //        }

        //        // Consecration
        //        if (CanCastConsecration() && GetHasBuff(SNOPower.X1_Crusader_IronSkin))
        //        {
        //            //Logger.Log("Consecration");
        //            return new TrinityPower(SNOPower.X1_Crusader_Consecration);
        //        }

        //        // Provoke
        //        if (!IsSteedCharging && CanCastProvoke())
        //        {
        //            //Logger.Log("Provoke");
        //            return new TrinityPower(SNOPower.X1_Crusader_Provoke);
        //        }

        //        // Shield Glare
        //        if (CanCastShieldGlare() && !IsSteedCharging)
        //        {
        //            var arcTarget = TargetUtil.GetBestArcTarget(45f, 70f);
        //            if (arcTarget != null && arcTarget.Position != Vector3.Zero)
        //            {
        //                //Logger.Log("Shield Glare");
        //                return new TrinityPower(SNOPower.X1_Crusader_ShieldGlare, 15f, arcTarget.Position);
        //            }
        //        }

        //        // Condemn
        //        if (CanCastCondemn() && !IsSteedCharging)
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_Condemn);
        //        }

        //        // LawsOfJustice
        //        if (CanCast(SNOPower.X1_Crusader_LawsOfJustice2) &&
        //            !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice_Passive2, 6) &&
        //            (!IsSteedCharging && TargetUtil.EliteOrTrashInRange(16f) ||
        //             Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct ||
        //             !IsSteedCharging && TargetUtil.AnyMobsInRange(15f, 5) ||
        //             !IsSteedCharging && Settings.Combat.Crusader.SpamLaws))
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);
        //        }

        //        // Laws of Hope
        //        if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfHope2) &&
        //            (TargetUtil.EliteOrTrashInRange(16f) ||
        //             TargetUtil.AnyMobsInRange(15f, 5) || Settings.Combat.Crusader.SpamLaws) &&
        //            !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope_Passive2, 6))
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
        //        }

        //        // LawsOfValor
        //        if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfValor2) &&
        //            !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfValor_Passive2, 6) &&
        //            (TargetUtil.EliteOrTrashInRange(16f) || TargetUtil.AnyMobsInRange(15f, 5) ||
        //             Settings.Combat.Crusader.SpamLaws))
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);
        //        }

        //        // Phalanx
        //        if (CanCastPhalanx())
        //        {
        //            var bestPierceTarget = TargetUtil.GetBestPierceTarget(45f);
        //            if (!Runes.Crusader.Bowmen.IsActive && bestPierceTarget != null)
        //                return new TrinityPower(SNOPower.x1_Crusader_Phalanx3, 45f, bestPierceTarget.ACDGuid);
        //            return new TrinityPower(SNOPower.x1_Crusader_Phalanx3);
        //        }

        //        // AkaratsChampion if rally
        //        if (CanCastAkaratsChampion() && Runes.Crusader.Rally.IsActive)
        //        {
        //            if (GetHasBuff(SNOPower.X1_Crusader_IronSkin) && Skills.Crusader.Bombardment.CooldownRemaining > 6000)
        //                return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
        //        }

        //        // SteedSpam
        //        if (Settings.Combat.Crusader.SpamSteedCharge && !IsSteedCharging && CanCastSteedCharge())
        //        {
        //            //Logger.Log("Steed Charge");
        //            return new TrinityPower(SNOPower.X1_Crusader_SteedCharge);
        //        }

        //        // SteedSpam DMG MOVE
        //        if (IsSteedCharging && !IsCurrentlyAvoiding)
        //        {
        //            //Logger.Log("Steed Charge Damage");
        //            return new TrinityPower(SNOPower.Walk, 20f, TargetUtil.GetZigZagTarget(CurrentTarget.Position, 35f, true), TrinityPlugin.CurrentWorldDynamicId, -1, 0, 1);
        //        }
        //    }

        //    // Buffs
        //    if (UseOOCBuff)
        //    {
        //        if (CanCastSteedChargeOutOfCombat() && !IsSteedCharging)
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_SteedCharge);
        //        }

        //        // Laws of Hope2
        //        if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfHope2) &&
        //            Player.CurrentHealthPct <= CrusaderSettings.LawsOfHopeHpPct
        //            && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope_Passive2, 6))
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
        //        }

        //        // LawsOfJustice2
        //        if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfJustice2) &&
        //            !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice2, 6) &&
        //            (Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct))
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);
        //        }

        //        // LawsOfValor2
        //        if (CanCast(SNOPower.X1_Crusader_LawsOfValor2) && TargetUtil.EliteOrTrashInRange(16f) &&
        //            !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfValor2, 6) &&
        //            !IsSteedCharging)
        //        {
        //            return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);
        //        }

        //        // Phalanx
        //        if (IsBombardmentBuild && Runes.Crusader.Bowmen.IsActive && CanCastPhalanx() &&
        //            !IsSteedCharging && !Player.IsInTown)
        //        {
        //            return new TrinityPower(SNOPower.x1_Crusader_Phalanx3);
        //        }
        //    }

        //    // Prevent Default Attack
        //    if (CurrentTarget != null && CurrentTarget.Type != TrinityObjectType.Destructible)
        //    {
        //        //Logger.Log("Prevent Primary Attack ");
        //        var targetPosition = TargetUtil.GetLoiterPosition(CurrentTarget, 5f);
        //        return new TrinityPower(SNOPower.Walk, 12f, targetPosition);
        //    }

        //    // Destructibles
        //    if (UseDestructiblePower)
        //        return DestroyObjectPower;

        //    // Primary generators
        //    //Logger.Log("Default / Generator");
        //    power = GetPrimaryPower();
        //    return power ?? DefaultPower;
        //}

        /// <summary>
        /// Newk's Bombard Behavior
        /// </summary>
        private static TrinityPower GetBombardPower()
        {
            TrinityPower power = null;

            //Logger.Log(
            //    "cd={0} RemainingMsToElement(Phys/1500)={1} Elapsed={2} ShouldWait(Bomb)={3}",
            //    Skills.Crusader.Bombardment.CooldownRemaining,
            //    TimeToElementStart(Element.Physical),
            //    TimeFromElementStart(Element.Physical),
            //    ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 3000)
            //    );

            // AkaratsChampion
            if (CanCastAkaratsChampion() && !IsSteedCharging)
            {
                //Logger.Log("Akarats Champion");
                return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
            }

            if (Player.IsCastingTownPortalOrTeleport())
            {
                return null;
            }

            if (CurrentTarget == null)
            {
                // Iron Skin
                if (CanCast(SNOPower.X1_Crusader_IronSkin) && (Player.CurrentHealthPct <= CrusaderSettings.IronSkinHpPct) ||
                    !ShouldWaitForConventionofElements(Skills.Crusader.IronSkin, Element.Physical, 1600, 0) && TargetUtil.AnyMobsInRange(60f))
                {
                    //Logger.Log("IronSkin");
                    return new TrinityPower(SNOPower.X1_Crusader_IronSkin);
                }

                // Bombardment
                if (CanCast(SNOPower.X1_Crusader_Bombardment) && GetHasBuff(SNOPower.X1_Crusader_IronSkin) &&
                    !ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 1000) &&
                    ZetaDia.Me.Movement.SpeedXY != 0 && (TargetUtil.AnyMobsInRange(60f, CrusaderSettings.BombardmentAoECount) ||
                    TargetUtil.AnyElitesInRange(60f)))
                {
                    if (IsBombardmentBuild && Runes.Crusader.Critical.IsActive && CanCast(SNOPower.X1_Crusader_LawsOfValor2))
                        return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);

                    //Logger.Log("Bombardment");
                    return new TrinityPower(SNOPower.X1_Crusader_Bombardment);
                }

                // Shield Glare
                if (CanCast(SNOPower.X1_Crusader_ShieldGlare) && !IsSteedCharging &&
                    !ShouldWaitForConventionofElements(Skills.Crusader.ShieldGlare, Element.Physical, 2500, 0) &&
                    TargetUtil.UnitsPlayerFacing(30f) >= CrusaderSettings.ShieldGlareAoECount)
                {
                    var arcTarget = TargetUtil.GetBestArcTarget(45f, 70f);
                    if (arcTarget != null && arcTarget.Position != Vector3.Zero)
                    {
                        //Logger.Log("Shield Glare");
                        return new TrinityPower(SNOPower.X1_Crusader_ShieldGlare, 15f, arcTarget.Position);
                    }
                }

                // LawsOfHope
                if (CanCast(SNOPower.X1_Crusader_LawsOfHope2) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope2, 6) &&
                    !IsSteedCharging && (TargetUtil.EliteOrTrashInRange(16f) ||
                     Player.CurrentHealthPct <= CrusaderSettings.LawsOfHopeHpPct ||
                     CrusaderSettings.SpamLaws))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
                }
            }

            // Has Target
            if (CurrentTarget != null)
            {
                // Walk Spam
                //if (!IsCurrentlyAvoiding && EveryOtherTick)
                //{
                //    return new TrinityPower(SNOPower.Walk, 24f, TargetUtil.GetZigZagTarget(CurrentTarget.Position, 35f, true), TrinityPlugin.CurrentWorldDynamicId, -1, 0, 1);
                //}

                //if (!IsCurrentlyAvoiding && !isSteedCharging && EveryOtherTick)
                //{
                //    Logger.Log("Loiter");
                //    var targetPosition = GetLoiterPosition(CurrentTarget, 5f);
                //    return new TrinityPower(SNOPower.Walk, 12f, targetPosition);
                //}

                // Iron Skin
                if (CanCastIronSkin())
                {
                    //Logger.Log("IronSkin");
                    return new TrinityPower(SNOPower.X1_Crusader_IronSkin);
                }

                // Consecration
                if (CanCastConsecration() && GetHasBuff(SNOPower.X1_Crusader_IronSkin))
                {
                    //Logger.Log("Consecration");
                    return new TrinityPower(SNOPower.X1_Crusader_Consecration);
                }

                // Bombardment
                if (CanCastBombardment())
                {
                    if (IsBombardmentBuild && Runes.Crusader.Critical.IsActive && CanCast(SNOPower.X1_Crusader_LawsOfValor2))
                        return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);

                    var bestPoint = CurrentTarget.IsEliteRareUnique
                        ? CurrentTarget.Position
                        : TargetUtil.GetBestClusterPoint();

                    //Logger.Log("Bombardment");
                    return new TrinityPower(SNOPower.X1_Crusader_Bombardment, 16f, bestPoint);
                }

                if (PlayerMover.IsBlocked && !IsSteedCharging && CanCast(SNOPower.X1_Crusader_SteedCharge))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_SteedCharge);
                }

                // Provoke
                if (!IsSteedCharging && CanCastProvoke())
                {
                    //Logger.Log("Provoke");
                    return new TrinityPower(SNOPower.X1_Crusader_Provoke);
                }

                // Shield Glare
                if (CanCastShieldGlare() && !IsSteedCharging)
                {
                    var arcTarget = TargetUtil.GetBestArcTarget(45f, 70f);
                    if (arcTarget != null && arcTarget.Position != Vector3.Zero)
                    {
                        //Logger.Log("Shield Glare");
                        return new TrinityPower(SNOPower.X1_Crusader_ShieldGlare, 15f, arcTarget.Position);
                    }
                }

                // Condemn
                if (CanCastCondemn())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Condemn);
                }

                // Judgement
                if (CanCastJudgement())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Judgment, 20f, TargetUtil.GetBestClusterPoint(20f));
                }

                // AkaratsChampion
                if (CanCastAkaratsChampion() && !IsSteedCharging)
                {
                    //Logger.Log("Akarats Champion");
                    return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
                }

                // LawsOfJustice
                if (CanCast(SNOPower.X1_Crusader_LawsOfJustice2) &&
                    !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice_Passive2, 6) &&
                    (!IsSteedCharging && TargetUtil.EliteOrTrashInRange(16f) ||
                     Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct ||
                     TargetUtil.AnyMobsInRange(15f, 5) || Settings.Combat.Crusader.SpamLaws))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);
                }

                // Laws of Hope
                if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfHope2) &&
                    (TargetUtil.EliteOrTrashInRange(16f) ||
                     TargetUtil.AnyMobsInRange(15f, 5) || Settings.Combat.Crusader.SpamLaws) &&
                    !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope_Passive2, 6))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
                }

                // LawsOfValor
                if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfValor2) &&
                    !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfValor_Passive2, 6) &&
                    (TargetUtil.EliteOrTrashInRange(16f) || TargetUtil.AnyMobsInRange(15f, 5) ||
                     Settings.Combat.Crusader.SpamLaws))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);
                }

                // Phalanx
                if (CanCastPhalanx() && !IsSteedCharging)
                {
                    var bestPierceTarget = TargetUtil.GetBestPierceTarget(45f);
                    if (!Runes.Crusader.Bowmen.IsActive && bestPierceTarget != null)
                        return new TrinityPower(SNOPower.x1_Crusader_Phalanx3, 45f, bestPierceTarget.ACDGuid);
                    return new TrinityPower(SNOPower.x1_Crusader_Phalanx3);
                }

                // SteedSpam
                if (Settings.Combat.Crusader.SpamSteedCharge && CanCastSteedCharge() && !IsSteedCharging)
                {
                    //Logger.Log("Steed Charge");
                    return new TrinityPower(SNOPower.X1_Crusader_SteedCharge);
                }

                // SteedSpam DMG MOVE
                if (IsSteedCharging && !IsCurrentlyAvoiding)
                {
                    //Logger.Log("Steed Charge Damage");
                    return new TrinityPower(SNOPower.Walk, 20f, TargetUtil.GetZigZagTarget(CurrentTarget.Position, 35f, true), TrinityPlugin.CurrentWorldDynamicId, -1, 0, 1);
                }
            }

            // Buffs
            if (UseOOCBuff)
            {
                // Laws of Hope2
                if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfHope2) &&
                    Player.CurrentHealthPct <= CrusaderSettings.LawsOfHopeHpPct
                    && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope_Passive2, 6))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
                }

                // LawsOfJustice2
                if (!IsSteedCharging && CanCast(SNOPower.X1_Crusader_LawsOfJustice2) &&
                    !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice2, 6) &&
                    (Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);
                }

                // LawsOfValor2
                if (CanCast(SNOPower.X1_Crusader_LawsOfValor2) && TargetUtil.EliteOrTrashInRange(16f) &&
                    !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfValor2, 6) &&
                    !IsSteedCharging)
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);
                }

                // Phalanx
                if (IsBombardmentBuild && Runes.Crusader.Bowmen.IsActive && CanCastPhalanx() &&
                    !IsSteedCharging && !Player.IsInTown)
                {
                    return new TrinityPower(SNOPower.x1_Crusader_Phalanx3);
                }
            }

            // Prevent Default Attack
            if (CurrentTarget != null && CurrentTarget.Type != TrinityObjectType.Destructible)
            {
                //Logger.Log("Prevent Primary Attack ");
                var targetPosition = TargetUtil.GetLoiterPosition(CurrentTarget, 5f);
                return new TrinityPower(SNOPower.Walk, 12f, targetPosition);
            }

            // Destructibles
            if (UseDestructiblePower)
                return DestroyObjectPower;

            // Primary generators
            //Logger.Log("Default / Generator");
            power = GetPrimaryPower();
            return power ?? DefaultPower;
        }

        public static TrinityPower GetPower()
        {
            if (IsBombardmentBuild)
            {
                return GetBombardPower();
            }

            if (IsInvokerPunishBuild)
            {
                return GetInvokerPower();
            }

            TrinityPower power = null;

            if (Player.IsCastingPortal)
                return null;
            
            if (UseOOCBuff)
            {
                if (Gems.Taeguk.IsEquipped && CanCast(SNOPower.X1_Crusader_BlessedHammer) && !Player.IsIncapacitated &&
                    !Player.IsInTown &&
                    Player.PrimaryResource >= 10 && TimeSincePowerUse(SNOPower.X1_Crusader_BlessedHammer) >= 2500)
                {
                    return new TrinityPower(SNOPower.X1_Crusader_BlessedHammer);
                }
            }

            // Destructibles
            if (UseDestructiblePower)
                return DestroyObjectPower;

            if (!UseOOCBuff && !IsCurrentlyAvoiding && CurrentTarget != null)
            {

                //There are doubles?? not sure which is correct yet
                // Laws of Hope
                // Laws of Hope2
                if (CanCast(SNOPower.X1_Crusader_LawsOfHope2) && (TargetUtil.EliteOrTrashInRange(16f) ||
                    TargetUtil.AnyMobsInRange(15f, 5) || Player.CurrentHealthPct <= CrusaderSettings.LawsOfHopeHpPct || Settings.Combat.Crusader.SpamLaws)
                    && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope_Passive2, 6))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
                }

                // LawsOfJustice
                // LawsOfJustice2
                if (CanCast(SNOPower.X1_Crusader_LawsOfJustice2) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice_Passive2, 6) &&
                    (TargetUtil.EliteOrTrashInRange(16f) || Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct ||
                    TargetUtil.AnyMobsInRange(15f, 5) || Settings.Combat.Crusader.SpamLaws))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);
                }

                // LawsOfValor
                // LawsOfValor2
                if (CanCast(SNOPower.X1_Crusader_LawsOfValor2) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfValor_Passive2, 6) &&
                    (TargetUtil.EliteOrTrashInRange(16f) || TargetUtil.AnyMobsInRange(15f, 5) || Settings.Combat.Crusader.SpamLaws))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);
                }

                if (ShouldRefreshBastiansGeneratorBuff)
                {
                    power = GetPrimaryPower();
                    if (power != null)
                        return power;
                }

                // Judgement
                if (CanCastJudgement())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Judgment, 20f, TargetUtil.GetBestClusterPoint(20f));
                }

                // Shield Glare
                if (CanCastShieldGlare())
                {
                    var arcTarget = TargetUtil.GetBestArcTarget(45f, 70f);
                    if (arcTarget != null && arcTarget.Position != Vector3.Zero)
                        return new TrinityPower(SNOPower.X1_Crusader_ShieldGlare, 15f, arcTarget.Position);
                }

                // Iron Skin
                if (CanCastIronSkin())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_IronSkin);
                }

                // Provoke
                if (CanCast(SNOPower.X1_Crusader_Provoke) && (TargetUtil.EliteOrTrashInRange(15f) ||
                    TargetUtil.AnyMobsInRange(15f, CrusaderSettings.ProvokeAoECount) || Sets.SeekerOfTheLight.IsFullyEquipped && Player.PrimaryResourcePct <= 0.25))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Provoke);
                }

                // Consecration
                if (CanCastConsecration() && (!Runes.Crusader.ShatteredGround.IsActive ||
                   TargetUtil.AnyMobsInRange(15f, 5) || TargetUtil.IsEliteTargetInRange(15f)))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Consecration);
                }

                // AkaratsChampion
                if (CanCastAkaratsChampion())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_AkaratsChampion);
                }

                // Bombardment
                if (CanCastBombardment())
                {
                    if (IsBombardmentBuild && Runes.Crusader.Critical.IsActive && CanCast(SNOPower.X1_Crusader_LawsOfValor2))
                        return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);

                    if (IsBombardmentBuild && Runes.Crusader.DecayingStrength.IsActive && CanCast(SNOPower.X1_Crusader_LawsOfJustice2))
                        return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);

                    Vector3 bestPoint = CurrentTarget.IsEliteRareUnique ?
                        CurrentTarget.Position :
                        TargetUtil.GetBestClusterPoint();

                    return new TrinityPower(SNOPower.X1_Crusader_Bombardment, 16f, bestPoint);
                }

                // FallingSword
                if (CanCastFallingSword())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_FallingSword, 16f, TargetUtil.GetBestClusterPoint(15f, 65f, false));
                }

                // HeavensFury
                bool hasAkkhan = (Sets.ArmorOfAkkhan.IsThirdBonusActive);
                if (CanCastHeavensFury() && !hasAkkhan)
                {
                    return new TrinityPower(SNOPower.X1_Crusader_HeavensFury3, 65f, TargetUtil.GetBestClusterPoint());
                }

                if (CanCastHeavensFuryHolyShotgun() && hasAkkhan)
                {
                    return new TrinityPower(SNOPower.X1_Crusader_HeavensFury3, 15f, CurrentTarget.Position);
                }

                // Condemn
                if (CanCastCondemn())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Condemn);
                }

                // Phalanx
                if (CanCastPhalanx())
                {
                    var bestPierceTarget = TargetUtil.GetBestPierceTarget(45f);
                    if (!Runes.Crusader.Bowmen.IsActive && bestPierceTarget != null)
                        return new TrinityPower(SNOPower.x1_Crusader_Phalanx3, 45f, bestPierceTarget.ACDGuid);
                    return new TrinityPower(SNOPower.x1_Crusader_Phalanx3);
                }
                if (CanCastPhalanxStampede())
                {
                    var bestPierceTarget = TargetUtil.GetBestPierceTarget(45f);
                    if (bestPierceTarget != null)
                        return new TrinityPower(SNOPower.x1_Crusader_Phalanx3, 45f, bestPierceTarget.ACDGuid);
                }

                // Blessed Shield : Piercing Shield
                bool hasPiercingShield = CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.X1_Crusader_BlessedShield && s.RuneIndex == 5);
                if (CanCastBlessedShieldPiercingShield(hasPiercingShield))
                {
                    var bestPierceTarget = TargetUtil.GetBestPierceTarget(45f);
                    if (bestPierceTarget != null)
                        return new TrinityPower(SNOPower.X1_Crusader_BlessedShield, 14f, bestPierceTarget.ACDGuid);
                }

                // Blessed Shield
                if (CanCastBlessedShield() && !hasPiercingShield)
                {
                    return new TrinityPower(SNOPower.X1_Crusader_BlessedShield, 14f, TargetUtil.GetBestClusterUnit().ACDGuid);
                }

                // Fist of Heavens
                if (CanCastFistOfHeavens())
                {
                    float range = Settings.Combat.Crusader.FistOfHeavensDist;
                    float clusterRange = 8f;
                    if (Runes.Crusader.DivineWell.IsActive)
                        clusterRange = 18f;
                    return new TrinityPower(SNOPower.X1_Crusader_FistOfTheHeavens, range, TargetUtil.GetBestClusterUnit(clusterRange).Position);
                }

                // Blessed Hammer
                if (CanCastBlessedHammer())
                {
                    return new TrinityPower(SNOPower.X1_Crusader_BlessedHammer, CrusaderSettings.BlessedHammerRange);
                }

                // Provoke
                if (CanCast(SNOPower.X1_Crusader_Provoke) && TargetUtil.AnyMobsInRange(15f, 4))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Provoke);
                }

                // Shield Bash
                var piroReduction = Legendary.PiroMarella.IsEquipped ? 0.6 : 1;
                if (CanCast(SNOPower.X1_Crusader_ShieldBash2, CanCastFlags.NoTimer) && Player.PrimaryResource >= 30 * (1 - Player.ResourceCostReductionPct) * piroReduction)
                {
                    var bestTarget = CurrentTarget.IsBoss ? CurrentTarget : TargetUtil.GetBestClusterUnit(15, 50f, 1, true, false);
                    if (bestTarget != null)
                        return new TrinityPower(SNOPower.X1_Crusader_ShieldBash2, 65f, bestTarget.ACDGuid);
                }

                // Sweep Attack
                if (CanCastSweepAttack())
                {
                    var arcTarget = TargetUtil.GetBestArcTarget(8f, 60f);
                    if (arcTarget != null && arcTarget.Position != Vector3.Zero)
                        return new TrinityPower(SNOPower.X1_Crusader_SweepAttack, 8f, arcTarget.Position);
                }

                /*
                 *  Basic Attacks
                 */


                // Blessed Shield
                if (CanCast(SNOPower.X1_Crusader_BlessedShield))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_BlessedShield, 14f, CurrentTarget.ACDGuid);
                }

                // Ensure steed charge isn't interrupted by Punish.
                if (IsSteedCharging && CurrentTarget != null && !IsDoingGoblinKamakazi)
                {
                    Logger.Log("Steed Charging X");

                    if (IsBombardmentBuild && Loiter(out power, 20f))
                        return power;

                    return new TrinityPower(SNOPower.Walk, 100f, TargetUtil.GetZigZagTarget(CurrentTarget.Position, 16f));
                }

                // Prevent Default Attack
                if (IsBombardmentBuild && (CurrentTarget == null || CurrentTarget.Type != TrinityObjectType.Destructible))
                {
                    Logger.Log("Prevent Primary Attack ");
                    var targetPosition = TargetUtil.GetLoiterPosition(CurrentTarget, 5f);
                    return new TrinityPower(SNOPower.Walk, 12f, targetPosition);
                }

                // Primary generators
                power = GetPrimaryPower();
                if (power != null)
                    return power;

                return DefaultPower;
            }

            // Buffs
            if (UseOOCBuff)
            {
                /*
                 *  Laws
                 */

                // Laws of Hope2
                if (CanCast(SNOPower.X1_Crusader_LawsOfHope2) && Player.CurrentHealthPct <= CrusaderSettings.LawsOfHopeHpPct
                    && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope_Passive2, 6))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfHope2);
                }

                // LawsOfJustice2
                if (CanCast(SNOPower.X1_Crusader_LawsOfJustice2) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice2, 6)
                    && (TargetUtil.EliteOrTrashInRange(16f) || Player.CurrentHealthPct <= CrusaderSettings.LawsOfJusticeHpPct))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfJustice2);
                }

                // LawsOfValor2
                if (CanCast(SNOPower.X1_Crusader_LawsOfValor2) && TargetUtil.EliteOrTrashInRange(16f) && !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfValor2, 6))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_LawsOfValor2);
                }
            }

            // Default Attacks
            if (IsNull(null))
                power = DefaultPower;

            return power;
        }

        private Vector3 _currentLoiterTarget;
        private static bool Loiter(out TrinityPower power, float targetDistance = 14f, bool forceMove = false)
        {
            var monsterTooClose = CurrentTarget != null && CurrentTarget.Distance < targetDistance || CurrentTarget == null && TargetUtil.AnyMobsInRange(targetDistance);
            if (monsterTooClose || forceMove)
            {
                var maxDistance = targetDistance * 1.5f;

                // Pick a safe spot near current target
                var safeSpot = CurrentTarget != null ?
                    NavHelper.KitePoint(CurrentTarget.Position, targetDistance, maxDistance) :
                    NavHelper.KitePoint(Player.Position, targetDistance, maxDistance);

                if (safeSpot.Distance(Player.Position) < 8f)
                {
                    safeSpot = NavHelper.KitePoint(TargetUtil.GetBestClusterPoint(16f, maxDistance), targetDistance, maxDistance);
                }

                CombatMovement.Queue(new CombatMovement
                {
                    Options = new CombatMovementOptions()
                    {
                        AcceptableDistance = 2f,
                        FailureBlacklistSeconds = 1,
                        MaxDistance = 60f,
                        SuccessBlacklistSeconds = 1,
                        Logging = LogLevel.All
                    },
                    Destination = safeSpot,
                    Name = "CrusaderLoiterPoint",
                    OnUpdate = m =>
                    {
                        if (CanCastSteedCharge())
                            Skills.Crusader.SteedCharge.Cast();

                        // Dynamically change the destination.
                        if (m != null && m.Status != null && m.Status.LastPosition != Vector3.Zero)
                        {
                            var target = TargetUtil.GetZigZagTarget(m.Status.LastPosition, 10f);
                            if (target != Vector3.Zero && target.Distance(m.Destination) > 10f)
                                m.Destination = target;
                        }
                    }
                });

                power = new TrinityPower(SNOPower.Walk);
                return true;
            }

            power = null;
            return false;
        }

        public static TrinityPower GetPrimaryPower()
        {
            var targetAcdGuid = CurrentTarget?.ACDGuid ?? -1;

            // Justice
            if (CanCast(SNOPower.X1_Crusader_Justice))
            {
                return new TrinityPower(SNOPower.X1_Crusader_Justice, 12f, targetAcdGuid);
            }

            // Smite
            if (CanCast(SNOPower.X1_Crusader_Smite))
            {
                var smiteTarget = TargetUtil.GetBestClusterUnit(15f, 15f);
                if (smiteTarget != null)
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Smite, 15f, TargetUtil.GetBestClusterUnit(15f, 15f).ACDGuid);
                }

                return new TrinityPower(SNOPower.X1_Crusader_Smite, 15f, targetAcdGuid);
            }

            // Slash
            if (CanCast(SNOPower.X1_Crusader_Slash))
            {
                var slashTarget = TargetUtil.GetBestClusterUnit(5f, 8f);
                if (slashTarget != null)
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Slash, 15f, TargetUtil.GetBestClusterUnit(5f, 8f).ACDGuid);
                }
                return new TrinityPower(SNOPower.X1_Crusader_Slash, 15f, targetAcdGuid);
            }

            // Punish
            if (CanCast(SNOPower.X1_Crusader_Punish))
            {
                return new TrinityPower(SNOPower.X1_Crusader_Punish, 12f, targetAcdGuid);
            }

            return null;
        }

        private static bool CanCastSweepAttack()
        {
            return CanCast(SNOPower.X1_Crusader_SweepAttack) && Player.PrimaryResource >= 30 * (1 - Player.ResourceCostReductionPct) &&
                (TargetUtil.UnitsPlayerFacing(18f) > CrusaderSettings.SweepAttackAoECount || TargetUtil.EliteOrTrashInRange(18f) || Player.PrimaryResource > 60 || CurrentTarget.CountUnitsBehind(12f) > 1);
        }

        private static bool CanCastFistOfHeavens()
        {
            return CanCast(SNOPower.X1_Crusader_FistOfTheHeavens, CanCastFlags.NoTimer) && Player.PrimaryResource >= 30 * (1 - Player.ResourceCostReductionPct);
        }

        private static bool CanCastBlessedHammer()
        {
            var range = CrusaderSettings.BlessedHammerRange;
            return CanCast(SNOPower.X1_Crusader_BlessedHammer) &&
                (TargetUtil.ClusterExists(CombatBase.CombatOverrides.EffectiveTrashRadius, range) ||
                TargetUtil.EliteOrTrashInRange(range) || Player.PrimaryResourcePct > 0.5 || Sets.SeekerOfTheLight.IsFullyEquipped && Player.PrimaryResource > 10);
        }

        private static bool CanCastBlessedShield()
        {
            return CanCast(SNOPower.X1_Crusader_BlessedShield) && (TargetUtil.ClusterExists(14f, 3) || TargetUtil.EliteOrTrashInRange(14f)) && (Player.PrimaryResource >= 20 * (1 - Player.ResourceCostReductionPct) || Legendary.GyrfalconsFoote.IsEquipped);
        }

        private static bool CanCastBlessedShieldPiercingShield(bool hasPiercingShield)
        {
            return CanCast(SNOPower.X1_Crusader_BlessedShield) && hasPiercingShield && (TargetUtil.ClusterExists(14f, 3) || TargetUtil.EliteOrTrashInRange(14f)) && Player.PrimaryResource >= 20 * (1 - Player.ResourceCostReductionPct);
        }

        private static bool CanCastPhalanx()
        {
            return CanCast(SNOPower.x1_Crusader_Phalanx3) && (TargetUtil.ClusterExists(45f, 3) ||
                TargetUtil.EliteOrTrashInRange(45f) || Legendary.WarhelmOfKassar.IsEquipped) &&
                Player.PrimaryResource >= 30 * (1 - Player.ResourceCostReductionPct);
        }

        private static bool CanCastPhalanxStampede()
        {
            return (Legendary.UnrelentingPhalanx.IsEquipped && CanCast(SNOPower.x1_Crusader_Phalanx3) && TargetUtil.AnyMobsInRange(45f, 1) && Runes.Crusader.Stampede.IsActive) && Player.PrimaryResource >= 30 * (1 - Player.ResourceCostReductionPct);
        }

        private static bool CanCastSteedCharge()
        {
            if (IsBombardmentBuild)
            {
                if (Settings.Combat.Misc.UseConventionElementOnly && ShouldWaitForConventionofElements(Skills.Crusader.SteedCharge, Element.Physical))
                    return false;
            }

            return CanCast(SNOPower.X1_Crusader_SteedCharge) && !Player.IsInTown && ZetaDia.Me.Movement.SpeedXY != 0;
        }

        private static bool CanCastCondemn()
        {
            if (!CanCast(SNOPower.X1_Crusader_Condemn))
                return false;

            if (!TargetUtil.AnyMobsInRange(CrusaderSettings.CondemnRange, CrusaderSettings.CondemnAoECount))
                return false;

            if (Legendary.FrydehrsWrath.IsEquipped && Player.PrimaryResource < 40)
                return false;

            if (IsBombardmentBuild && !ShouldWaitForConventionofElements(Skills.Crusader.IronSkin, Element.Physical, 2500, 0))
                return true;

            return true;
        }

        private static bool CanCastHeavensFury()
        {
            return (CanCast(SNOPower.X1_Crusader_HeavensFury3) && (TargetUtil.EliteOrTrashInRange(65f) || TargetUtil.ClusterExists(15f, 65f, CrusaderSettings.HeavensFuryAoECount)));
        }

        private static bool CanCastHeavensFuryHolyShotgun()
        {
            return (CanCast(SNOPower.X1_Crusader_HeavensFury3) && TargetUtil.AnyMobsInRange(15f, 1) && Runes.Crusader.FiresOfHeaven.IsActive);
        }

        private static bool CanCastFallingSword()
        {
            if (!CanCast(SNOPower.X1_Crusader_FallingSword))
                return false;

            if (Sets.SeekerOfTheLight.IsFullyEquipped && !GetHasBuff(SNOPower.X1_Crusader_FallingSword) && Player.PrimaryResource >= 25 &&
                (CacheData.Buffs.ConventionElement != Element.Holy || Player.CurrentHealthPct <= 0.5))
                return true;

            return !Sets.SeekerOfTheLight.IsFullyEquipped && (CurrentTarget.IsEliteRareUnique || TargetUtil.ClusterExists(15f, 65f, CrusaderSettings.FallingSwordAoECount)) &&
                Player.PrimaryResource >= 25 * (1 - Player.ResourceCostReductionPct);
        }

        private static bool CanCastBombardment()
        {
            if (!CanCast(SNOPower.X1_Crusader_Bombardment))
                return false;

            if (IsBombardmentBuild)
            {
                if (ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 1000))
                    return false;

                if (!GetHasBuff(SNOPower.X1_Crusader_IronSkin))
                    return false;

                if (ZetaDia.Me.Movement.SpeedXY == 0)
                    return false;

                return true;
            }

            return (TargetUtil.AnyMobsInRange(60f, CrusaderSettings.BombardmentAoECount) || TargetUtil.AnyElitesInRange(60f)) &&
                !(Settings.Combat.Misc.UseConventionElementOnly && ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 1000));
        }

        private static bool CanCastAkaratsChampion()
        {
            //Basic checks
            if (!CanCast(SNOPower.X1_Crusader_AkaratsChampion) || Player.IsInTown)
                return false;

            // Akarat's mode is 'Off Cooldown'
            if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenReady)
                return true;

            // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
            if (CurrentTarget != null && CurrentTarget.IsTreasureGoblin && Settings.Combat.Monk.UseEpiphanyGoblin ||
                Player.CurrentHealthPct <= 0.39 &&
                Settings.Combat.Crusader.AkaratsEmergencyHealth || GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
                ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned)
                return true;

            // Akarat's mode is 'Whenever in Combat'
            if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenInCombat && TargetUtil.AnyMobsInRange(80f))
                return true;

            // Akarat's mode is 'Use when Elites are nearby'
            if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.Normal && TargetUtil.AnyElitesInRange(80f))
                return true;

            // Akarat's mode is 'Hard Elites Only'
            if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.HardElitesOnly && HardElitesPresent)
                return true;

            return false;
        }

        private static bool CanCastConsecration()
        {
            if (!CanCast(SNOPower.X1_Crusader_Consecration))
                return false;

            if (Player.CurrentHealthPct <= CrusaderSettings.ConsecrationHpPct)
                return true;

            if (IsBombardmentBuild && !ShouldWaitForConventionofElements(Skills.Crusader.Consecration, Element.Physical))
                return true;

            return false;
        }

        private static bool CanCastIronSkin()
        {
            if (!CanCast(SNOPower.X1_Crusader_IronSkin))
                return false;

            if (Player.CurrentHealthPct <= CrusaderSettings.IronSkinHpPct)
                return true;

            if (!IsBombardmentBuild && CurrentTarget.IsBossOrEliteRareUnique &&
                CurrentTarget.RadiusDistance <= 10f && !Settings.Combat.Misc.UseConventionElementOnly)
                return true;

            if (IsBombardmentBuild && !ShouldWaitForConventionofElements(Skills.Crusader.IronSkin, Element.Physical, 1600, 0))
                return true;

            return false;
        }

        private static bool CanCastShieldGlare()
        {
            if (!CanCast(SNOPower.X1_Crusader_ShieldGlare))
                return false;

            if (!IsBombardmentBuild &&
                ((CurrentTarget.IsBossOrEliteRareUnique && CurrentTarget.RadiusDistance <= 15f) ||
                 TargetUtil.UnitsPlayerFacing(16f) >= CrusaderSettings.ShieldGlareAoECount))
                return true;

            if (IsBombardmentBuild && !ShouldWaitForConventionofElements(Skills.Crusader.ShieldGlare, Element.Physical, 2500, 0) &&
                ((CurrentTarget.IsBossOrEliteRareUnique && CurrentTarget.RadiusDistance <= 15f) ||
                 TargetUtil.UnitsPlayerFacing(16f) >= CrusaderSettings.ShieldGlareAoECount))
                return true;

            return false;
        }

        private static bool CanCastProvoke()
        {
            if (!CanCast(SNOPower.X1_Crusader_Provoke))
                return false;

            if (IsBombardmentBuild)
            {
                if (Settings.Combat.Misc.UseConventionElementOnly &&
                    (ShouldWaitForConventionofElements(Skills.Crusader.Provoke, Element.Physical)))
                    return false;
            }

            if ((TargetUtil.EliteOrTrashInRange(15f) || TargetUtil.AnyMobsInRange(15f, CrusaderSettings.ProvokeAoECount)) ||
               (Sets.SeekerOfTheLight.IsFullyEquipped && Player.PrimaryResourcePct <= 0.25))
                return true;

            return false;
        }


        private static bool CanCastJudgement()
        {
            return CanCast(SNOPower.X1_Crusader_Judgment) && (TargetUtil.EliteOrTrashInRange(16f) || TargetUtil.ClusterExists(15f, CrusaderSettings.JudgmentAoECount));
        }

        private static TrinityPower DestroyObjectPower
        {
            get
            {
                // Sweep Attack
                if (CanCast(SNOPower.X1_Crusader_SweepAttack))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_SweepAttack, 15f, CurrentTarget.ACDGuid);
                }

                //Blessed Shield
                if (CanCast(SNOPower.X1_Crusader_BlessedShield))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_BlessedShield, 14f, CurrentTarget.ACDGuid);
                }

                // Justice
                if (CanCast(SNOPower.X1_Crusader_Justice))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Justice, 45f, CurrentTarget.ACDGuid);
                }

                // Smite
                if (CanCast(SNOPower.X1_Crusader_Smite))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Smite, 15f, CurrentTarget.ACDGuid);
                }

                // Slash
                if (CanCast(SNOPower.X1_Crusader_Slash))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Slash, 5f, CurrentTarget.ACDGuid);
                }

                // Punish
                if (CanCast(SNOPower.X1_Crusader_Punish))
                {
                    return new TrinityPower(SNOPower.X1_Crusader_Punish, 5f, CurrentTarget.ACDGuid);
                }
                return DefaultPower;
            }
        }

        public static bool IsSteedCharging
        {
            get { return DataDictionary.SteedChargeAnimations.Contains(Player.CurrentAnimation); }
        }

    }
}
