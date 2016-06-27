using System;
using System.Linq;
using Trinity.Cache;
using Trinity.Config.Combat;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities
{
    class BarbarianCombat : CombatBase
    {
        private static bool _allowSprintOoc = true;

        private static TrinityPower QueuedPower { get; set; }

        public static TrinityPower GetPower()
        {
            if (UseDestructiblePower)
                return DestroyObjectPower;

            if (UseOOCBuff)
            {
                // Call of The Ancients
                if (CanUseCallOfTheAncients && Sets.ImmortalKingsCall.IsFullyEquipped)
                    return PowerCallOfTheAncients;
            }
            else
            {
                if (QueuedPower != null && !Player.IsIncapacitated && PowerManager.CanCast(QueuedPower.SNOPower) &&
                    !Player.IsCastingOrLoading)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "Casting Queued Power {0}", QueuedPower);
                    var next = QueuedPower;
                    QueuedPower.MaxFailedCastReTryAttempts = 5;
                    QueuedPower.WaitBeforeUseDelay = 750;
                    QueuedPower = null;
                    return next;
                }
            }

            // Ignore Pain when near Frozen
            if ((ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted) && CanCastIgnorePain)
            {
                Logger.Log("Used Ignore Pain to prevent Frozen");
                return PowerIgnorePain;
            }

            if (!UseOOCBuff)
            {
                // Refresh Frenzy
                if (CanCast(SNOPower.Barbarian_Frenzy) && TimeSincePowerUse(SNOPower.Barbarian_Frenzy) > 3000 &&
                    TimeSincePowerUse(SNOPower.Barbarian_Frenzy) < 4000)
                    return PowerFrenzy;

                // Refresh Bash - Punish
                if (CanCast(SNOPower.Barbarian_Bash) && TimeSincePowerUse(SNOPower.Barbarian_Bash) > 4000 &&
                    TimeSincePowerUse(SNOPower.Barbarian_Bash) < 5000)
                    return PowerBash;
            }

            // Ignore Pain when low on health
            if (CanCastIgnorePain)
                return PowerIgnorePain;

            // WOTB
            if (CanUseWrathOfTheBerserker)
                return PowerWrathOfTheBerserker;

            // Call of the Ancients
            if (CanUseCallOfTheAncients)
                return PowerCallOfTheAncients;

            // Sprint
            if (CanUseSprint)
                return PowerSprint;

            // Ancient Spear
            if (CanUseAncientSpear)
                return PowerAncientSpear;

            // Earthquake
            if (CanUseEarthquake)
                return PowerEarthquake;

            // Avalanche
            if (CanUseAvalanche)
                return PowerAvalanche;

            // War Cry
            if (CanUseWarCry)
                return PowerWarCry;

            // Battle Rage
            if (CanUseBattleRage)
                return PowerBattleRage;

            // Rend
            if (CanUseRend)
                return PowerRend;

            // Overpower
            if (CanUseOverPower)
                return PowerOverpower;

            // Threatening Shout
            if (CanUseThreatingShout)
                return PowerThreateningShout;

            // Leap with Earth Set.
            if (CanUseLeap && Sets.MightOfTheEarth.IsThirdBonusActive)
                return PowerLeap;

            // Ground Stomp
            if (CanUseGroundStomp)
                return PowerGroundStomp;

            // Revenge
            if (CanUseRevenge)
                return PowerRevenge;

            // Furious Charge
            if (CanUseFuriousCharge)
                return PowerFuriousCharge;

            // Leap
            if (CanUseLeap)
                return PowerLeap;

            // Seismic Slam
            if (CanUseSeismicSlam)
                return PowerSeismicSlam;

            // Bash to 3 stacks (Punish)
            if (CanUseBashTo3)
                return PowerBash;

            // Frenzy to 5 stacks (Maniac)
            if (CanUseFrenzyTo5)
                return PowerFrenzy;

            // HOTA Elites
            if (CanUseHammerOfTheAncientsElitesOnly)
                return PowerHammerOfTheAncients;

            // Whirlwind
            if (CanUseWhirlwind)
                return PowerWhirlwind;

            // Hammer of the Ancients
            if (CanUseHammerOfTheAncients)
                return PowerHammerOfTheAncients;

            // Weapon Throw
            if (CanUseWeaponThrow)
                return PowerWeaponThrow;

            // Frenzy Fury Generator
            if (CanUseFrenzy)
                return PowerFrenzy;

            // Bash Fury Generator
            if (CanUseBash)
                return PowerBash;

            // Cleave Fury Generator
            if (CanUseCleave)
                return PowerCleave;

            // Default Attacks
            return DefaultPower;
        }

        public static bool CanCastIgnorePain
        {
            get
            {
                if (UseOOCBuff)
                    return false;

                if (GetHasBuff(SNOPower.Barbarian_IgnorePain))
                    return false;

                if (!CanCast(SNOPower.Barbarian_IgnorePain))
                    return false;

                if (Settings.Combat.Barbarian.IgnorePainOffCooldown)
                    return true;

                if (Player.CurrentHealthPct <= Settings.Combat.Barbarian.IgnorePainMinHealthPct)
                    return true;

                if (Player.IsFrozen || Player.IsRooted || Player.IsJailed)
                    return true;

                return Sets.TheLegacyOfRaekor.IsFullyEquipped && ShouldFuryDump;
            }
        }

        public static bool CanUseCallOfTheAncients
        {
            get
            {
                if (IsCurrentlyAvoiding || !CanCast(SNOPower.Barbarian_CallOfTheAncients) || Player.IsIncapacitated)
                    return false;

                if (!Sets.ImmortalKingsCall.IsFirstBonusActive && CurrentTarget != null &&
                    (CurrentTarget.IsElite || TargetUtil.AnyMobsInRange(25f, 3)))
                    return true;

                if (Sets.ImmortalKingsCall.IsFirstBonusActive && Player.Summons.AncientCount < 3)
                    return true;

                return false;
            }
        }

        public static bool CanUseWrathOfTheBerserker
        {
            get
            {
                /* WOTB should be used when the following conditions are met:
                 * If ignoring elites, when 3 monsters in 25 yards or 10 monsters in 50 yards are present, OR
                 * If using on hard elites only, when an elite with the required affix is present, OR
                 * If normal mode, when any elite is within 20 yards, OR
                 * If we have low health (use potion health)
                 * And not on the Heart of sin
                 */

                var anyTime = Settings.Combat.Barbarian.WOTBMode == BarbarianWOTBMode.WhenReady && !Player.IsInTown;
                var whenInCombat = Settings.Combat.Barbarian.WOTBMode == BarbarianWOTBMode.WhenInCombat &&
                                   TargetUtil.AnyMobsInRange(50) && !UseOOCBuff;
                var hasInfiniteCasting = GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting);

                var emergencyHealth = Player.CurrentHealthPct <= 0.39;


                return CanCast(SNOPower.Barbarian_WrathOfTheBerserker) &&
                       (WOTBGoblins || WOTBIgnoreElites || WOTBElitesPresent || emergencyHealth || hasInfiniteCasting ||
                        anyTime || whenInCombat);
                ;
            }
        }

        /// <summary>
        /// If using WOTB on all elites, or if we should only use on "hard" affixes
        /// </summary>
        public static bool WOTBElitesPresent
        {
            get
            {
                var hardElitesOnly = Settings.Combat.Barbarian.WOTBMode == BarbarianWOTBMode.HardElitesOnly;

                var elitesPresent = TargetUtil.AnyElitesInRange(20f);

                return ((!hardElitesOnly && elitesPresent) || (hardElitesOnly && HardElitesPresent));
            }
        }

        /// <summary>
        /// Make sure we are allowed to use wrath on goblins, else make sure this isn't a goblin
        /// </summary>
        public static bool WOTBGoblins
        {
            get
            {
                if (CurrentTarget == null)
                    return false;
                return CurrentTarget.IsTreasureGoblin && Settings.Combat.Barbarian.UseWOTBGoblin;
            }
        }

        /// <summary>
        /// If ignoring elites completely, trigger on 3 trash within 25 yards, or 10 trash in 50 yards
        /// </summary>
        public static bool WOTBIgnoreElites
        {
            get
            {
                return
                    IgnoringElites &&
                    (TargetUtil.AnyMobsInRange(35f, 8) ||
                     TargetUtil.AnyMobsInRange(50f, 10) ||
                     TargetUtil.AnyMobsInRange(CombatOverrides.EffectiveTrashRadius, CombatOverrides.EffectiveTrashSize));
            }
        }

        public static bool CanUseEarthquake
        {
            get
            {
                var minFury = 50f;
                var range = Runes.Barbarian.Cavein.IsActive ? 24f : 14f;
                var mobsInRange = Sets.MightOfTheEarth.IsFullyEquipped ? 1 : 10;
                return
                    !UseOOCBuff &&
                    !IsCurrentlyAvoiding &&
                    !Player.IsIncapacitated &&
                    CanCast(SNOPower.Barbarian_Earthquake) &&
                    Player.PrimaryResource >= minFury &&
                    (TargetUtil.IsEliteTargetInRange(range) || TargetUtil.AnyMobsInRange(range, mobsInRange));

            }
        }

        public static bool CanUseBattleRage
        {
            get
            {
                var shouldRefreshTaeguk = GetHasBuff(SNOPower.ItemPassive_Unique_Gem_015_x1) &&
                                          !Hotbar.Contains(SNOPower.Barbarian_Whirlwind) &&
                                          Skills.Barbarian.BattleRage.TimeSinceUse >= 2300 &&
                                          Skills.Barbarian.BattleRage.TimeSinceUse <= 3000;

                return !Player.IsIncapacitated && CanCast(SNOPower.Barbarian_BattleRage, CanCastFlags.NoTimer) &&
                       (!GetHasBuff(SNOPower.Barbarian_BattleRage) || ShouldFuryDump || shouldRefreshTaeguk) &&
                       Player.PrimaryResource >= 20;
            }
        }

        public static bool CanUseSprintOOC
        {
            get
            {
                return
                    (Settings.Combat.Barbarian.SprintMode != BarbarianSprintMode.CombatOnly) &&
                    AllowSprintOOC &&
                    !Player.IsIncapacitated &&
                    CanCast(SNOPower.Barbarian_Sprint) &&
                    (Settings.Combat.Misc.AllowOOCMovement || GetHasBuff(SNOPower.Barbarian_WrathOfTheBerserker)) &&
                    !GetHasBuff(SNOPower.Barbarian_Sprint) &&
                    Player.PrimaryResource >= 20;
            }
        }

        public static bool CanUseWarCry
        {
            get
            {
                return
                    CanCast(SNOPower.X1_Barbarian_WarCry_v2, CanCastFlags.NoTimer) && !Player.IsIncapacitated &&
                    (Player.PrimaryResource <= 40 ||
                     Skills.Barbarian.WarCry.TimeSinceUse >= Settings.Combat.Barbarian.WarCryWaitDelay) &&
                    (!Legendary.BladeOfTheTribes.IsEquipped || TargetUtil.AnyMobsInRange(20f));
            }
        }

        public static bool CanUseThreatingShout
        {
            get
            {
                bool inCombat = !UseOOCBuff &&
                                CanCast(SNOPower.Barbarian_ThreateningShout) &&
                                !Player.IsIncapacitated &&
                                Skills.Barbarian.ThreateningShout.TimeSinceUse >
                                Settings.Combat.Barbarian.ThreateningShoutWaitDelay &&
                                ((TargetUtil.AnyMobsInRange(25f, Settings.Combat.Barbarian.MinThreatShoutMobCount, false)) ||
                                 TargetUtil.IsEliteTargetInRange(25f) ||
                                 (Hotbar.Contains(SNOPower.Barbarian_Whirlwind) && Player.PrimaryResource <= 10)
                                    );

                bool outOfCombat = UseOOCBuff &&
                                   !Player.IsIncapacitated &&
                                   Settings.Combat.Barbarian.ThreatShoutOOC &&
                                   CanCast(SNOPower.Barbarian_ThreateningShout) &&
                                   Player.PrimaryResource < 25;

                return inCombat || outOfCombat;

            }
        }

        public static bool CanUseGroundStomp
        {
            get
            {
                return
                    !UseOOCBuff &&
                    !Player.IsIncapacitated &&
                    CanCast(SNOPower.Barbarian_GroundStomp) &&
                    (
                        TargetUtil.AnyElitesInRange(12, 1) ||
                        TargetUtil.AnyMobsInRange(12, 4)
                        );
            }
        }

        public static bool CanUseRevenge
        {
            get
            {
                return
                    !UseOOCBuff &&
                    CanCast(SNOPower.Barbarian_Revenge) &&
                    !Player.IsIncapacitated &&
                    // Don't use revenge on goblins, too slow!
                    (!CurrentTarget.IsTreasureGoblin || TargetUtil.AnyMobsInRange(10f));
            }
        }

        public static bool CanUseFuriousCharge
        {
            get
            {
                if (UseOOCBuff)
                    return false;

                if (Player.IsIncapacitated || !CanCast(SNOPower.Barbarian_FuriousCharge) ||
                    Skills.Barbarian.FuriousCharge.Charges < 1)
                    return false;

                if (Sets.BastionsOfWill.IsFullyEquipped && !ShouldRefreshBastiansGeneratorBuff)
                    return false;

                if (Legendary.StrongarmBracers.IsEquipped && !Sets.TheLegacyOfRaekor.IsFirstBonusActive &&
                    TimeSincePowerUse(SNOPower.Barbarian_FuriousCharge) < 4000)
                    return false;

                var shouldRegenFury = CurrentTarget.NearbyUnitsWithinDistance(10) >= 3 && Player.PrimaryResource <= 40;
                if (!Sets.TheLegacyOfRaekor.IsSecondBonusActive && shouldRegenFury)
                    return true;

                if (Sets.TheLegacyOfRaekor.IsSecondBonusActive && !TargetUtil.AnyMobsInRange(60f))
                    return false;

                return true;
            }
        }

        public static bool CanUseLeap
        {
            get
            {
                bool leapresult = !UseOOCBuff && !Player.IsIncapacitated && CanCast(SNOPower.Barbarian_Leap);
                // This will now cast whenever leap is available and an enemy is around. 
                // Disable Leap OOC option. The last line will prevent you from leaping on destructibles
                if (Legendary.LutSocks.IsEquipped)
                    return leapresult && TargetUtil.AnyMobsInRange(15f, 1);

                return leapresult && (TargetUtil.ClusterExists(15f, 35f, 3) || CurrentTarget.IsElite);
            }
        }

        public static bool CanUseRend
        {
            get
            {
                if (UseOOCBuff || IsCurrentlyAvoiding || Player.IsIncapacitated || !CanCast(SNOPower.Barbarian_Rend))
                    return false;

                if (!CanCast(SNOPower.Barbarian_Rend))
                    return false;

                var mobCountThreshold =
                    TrinityPlugin.Targets.Count(
                        o => o.IsUnit && (!o.HasDebuff(SNOPower.Barbarian_Rend)) && o.RadiusDistance <= 12) >= 3 ||
                    CurrentTarget.IsElite;
                if (!mobCountThreshold)
                    return false;

                // Spam with Bloodlust
                if (Runes.Barbarian.BloodLust.IsActive && Player.CurrentHealthPct <= .25)
                    return true;

                // If lamentation is equipped, cast twice in a row and then wait
                if (Legendary.Lamentation.IsEquipped)
                {
                    var castsWithinTime = SpellHistory.SpellUseCountInTime(SNOPower.Barbarian_Rend,
                        TimeSpan.FromMilliseconds(Settings.Combat.Barbarian.RendWaitDelay));

                    Logger.LogVerbose(LogCategory.Behavior, "Casts within {0}ms = {1}",
                        Settings.Combat.Barbarian.RendWaitDelay, castsWithinTime);

                    if (Player.PrimaryResource >= 20 && QueuedPower != PowerRend && castsWithinTime == 0)
                    {
                        Logger.LogVerbose("Double Rend!");
                        QueuedPower = PowerRend;
                        return true;
                    }

                    return false;
                }

                return Skills.Barbarian.Rend.TimeSinceUse > Settings.Combat.Barbarian.RendWaitDelay &&
                       Player.PrimaryResource >= 20;
            }
        }

        public static bool CanUseOverPower
        {
            get
            {
                if (CurrentTarget == null || Player.IsIncapacitated || Player.IsInTown ||
                    !CanCast(SNOPower.Barbarian_Overpower))
                    return false;

                var overPowerHasBuffEffect = (Runes.Barbarian.KillingSpree.IsActive ||
                                              Runes.Barbarian.CrushingAdvance.IsActive);

                if (!GetHasBuff(SNOPower.Barbarian_Overpower) && overPowerHasBuffEffect)
                    return true;

                return CurrentTarget.RadiusDistance <= 10 && !overPowerHasBuffEffect &&
                       TargetUtil.AnyMobsInRange(10f) &&
                       (CurrentTarget.IsElite || CurrentTarget.IsMinion || CurrentTarget.IsBoss ||
                        TargetUtil.NumMobsInRangeOfPosition(TrinityPlugin.Player.Position, 9) >= 4);
            }
        }

        public static bool CanUseSeismicSlam
        {
            get
            {
                return !UseOOCBuff && CanCast(SNOPower.Barbarian_SeismicSlam) && !Player.IsIncapacitated &&
                       (!Hotbar.Contains(SNOPower.Barbarian_BattleRage) ||
                        (Hotbar.Contains(SNOPower.Barbarian_BattleRage) && GetHasBuff(SNOPower.Barbarian_BattleRage))) &&
                       Player.PrimaryResource >= 15 && CurrentTarget.Distance <= 40 &&
                       (TargetUtil.AnyMobsInRange(20f));
            }
        }

        public static bool CanUseAncientSpear
        {
            get
            {
                if (CurrentTarget == null || !CanCast(SNOPower.X1_Barbarian_AncientSpear) || Player.PrimaryResource < 25)
                    return false;

                if (Skills.Barbarian.AncientSpear.TimeSinceUse < Settings.Combat.Barbarian.AncientSpearWaitDelay)
                    return false;

                if (ShouldRefreshBastiansSpenderBuff)
                    return true;

                if (Runes.Barbarian.BoulderToss.IsActive)
                {
                    // don't interrupt the leaps
                    if (Legendary.LutSocks.IsEquipped &&
                        Skills.Barbarian.Leap.Cooldown < TimeSpan.FromMilliseconds(1000))
                        return false;

                    if (Player.PrimaryResourcePct > 0.9 || ShouldFuryDump)
                        return true;

                    // Throw if we have finished leaping
                    if (Legendary.LutSocks.IsEquipped &&
                        Skills.Barbarian.Leap.Cooldown > TimeSpan.FromMilliseconds(1000))
                        return true;

                    if (Sets.TheLegacyOfRaekor.IsFullyEquipped &&
                        GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_026) >= 5)
                        return true;
                }

                return false;
            }
        }

        public static bool CanUseSprint
        {
            get
            {
                if (TrinityPlugin.Settings.Combat.Barbarian.SprintMode != BarbarianSprintMode.MovementOnly &&
                    !UseOOCBuff && CanCast(SNOPower.Barbarian_Sprint, CanCastFlags.NoTimer) &&
                    !Player.IsIncapacitated && !GetHasBuff(SNOPower.Barbarian_Sprint) && Player.PrimaryResource >= 20)
                    return true;

                return false;
            }
        }

        public static bool CanUseFrenzyTo5
        {
            get
            {
                return !UseOOCBuff && !IsCurrentlyAvoiding && !Player.IsRooted &&
                       Hotbar.Contains(SNOPower.Barbarian_Frenzy) &&
                       !TargetUtil.AnyMobsInRange(15f, 3) && GetBuffStacks(SNOPower.Barbarian_Frenzy) < 5;
            }
        }

        public static bool CanUseBashTo3
        {
            get
            {
                return !UseOOCBuff && !IsCurrentlyAvoiding && !Player.IsRooted &&
                       Hotbar.Contains(SNOPower.Barbarian_Bash) &&
                       Runes.Barbarian.Punish.IsActive && !TargetUtil.AnyMobsInRange(15f, 3) &&
                       GetBuffStacks(SNOPower.Barbarian_Bash) < 3;
            }
        }

        public static bool CanUseWhirlwind
        {
            get
            {
                var alwaysWhirlwind = Sets.BulKathossOath.IsFullyEquipped || Sets.WrathOfTheWastes.IsFullyEquipped;

                if (Player.PrimaryResource < 10 || !CanCast(SNOPower.Barbarian_Whirlwind))
                    return false;

                if (!alwaysWhirlwind && (UseOOCBuff || IsCurrentlyAvoiding || Player.IsIncapacitated || Player.IsRooted))
                    return false;

                return (CurrentTarget != null && CurrentTarget.RadiusDistance <= 25f || TargetUtil.AnyMobsInRange(25f)) &&
                       // Check for energy reservation amounts
                       Player.PrimaryResource >= 10 &&
                       // If they have battle-rage, make sure it's up
                       (!Hotbar.Contains(SNOPower.Barbarian_BattleRage) || GetHasBuff(SNOPower.Barbarian_BattleRage));
            }
        }

        public static bool CanUseHammerOfTheAncients
        {
            get
            {
                bool hotaresult = !UseOOCBuff && !IsCurrentlyAvoiding && !Player.IsIncapacitated &&
                                  CanCast(SNOPower.Barbarian_HammerOfTheAncients) && Player.PrimaryResource >= 20 &&
                                  (!Hotbar.Contains(SNOPower.Barbarian_Whirlwind) ||
                                   (Player.CurrentHealthPct >= Settings.Combat.Barbarian.MinHotaHealth &&
                                    Hotbar.Contains(SNOPower.Barbarian_Whirlwind)));

                if (Legendary.LutSocks.IsEquipped)
                {
                    return !CanUseLeap && hotaresult;
                }

                return hotaresult;

            }
        }

        public static bool CanUseHammerOfTheAncientsElitesOnly
        {
            get
            {
                bool canUseHota = CanUseHammerOfTheAncients;

                if (Legendary.LutSocks.IsEquipped)
                {
                    if (canUseHota)
                    {
                        bool hotaElites = (CurrentTarget.IsElite || CurrentTarget.IsTreasureGoblin) &&
                                          TargetUtil.EliteOrTrashInRange(10f);

                        bool hotaTrash = IgnoringElites && CurrentTarget.IsTrashMob &&
                                         (TargetUtil.EliteOrTrashInRange(6f) ||
                                          CurrentTarget.MonsterSize == MonsterSize.Big);

                        return canUseHota && (hotaElites || hotaTrash);
                    }
                }
                else
                {
                    if (canUseHota && !CanUseLeap)
                    {
                        bool hotaElites = (CurrentTarget.IsElite || CurrentTarget.IsTreasureGoblin) &&
                                          TargetUtil.EliteOrTrashInRange(10f);

                        bool hotaTrash = IgnoringElites && CurrentTarget.IsTrashMob &&
                                         (TargetUtil.EliteOrTrashInRange(6f) ||
                                          CurrentTarget.MonsterSize == MonsterSize.Big);

                        return canUseHota && (hotaElites || hotaTrash);
                    }
                }
                return false;
            }
        }

        public static bool CanUseWeaponThrow
        {
            get { return !UseOOCBuff && !IsCurrentlyAvoiding && CanCast(SNOPower.X1_Barbarian_WeaponThrow); }
        }

        public static bool CanUseFrenzy
        {
            get { return !UseOOCBuff && !IsCurrentlyAvoiding && CanCast(SNOPower.Barbarian_Frenzy); }
        }

        public static bool CanUseBash
        {
            get { return !UseOOCBuff && !IsCurrentlyAvoiding && CanCast(SNOPower.Barbarian_Bash); }
        }

        public static bool CanUseCleave
        {
            get { return !UseOOCBuff && !IsCurrentlyAvoiding && CanCast(SNOPower.Barbarian_Cleave); }
        }

        public static bool CanUseAvalanche
        {
            get
            {
                bool hasBerserker =
                    CacheData.Hotbar.PassiveSkills.Any(p => p == SNOPower.Barbarian_Passive_BerserkerRage);
                double minFury = hasBerserker ? Player.PrimaryResourceMax*0.99 : 0f;

                return !UseOOCBuff && !IsCurrentlyAvoiding &&
                       CanCast(SNOPower.X1_Barbarian_Avalanche_v2, CanCastFlags.NoTimer) &&
                       Player.PrimaryResource >= minFury &&
                       (TargetUtil.AnyMobsInRange(3) || TargetUtil.IsEliteTargetInRange());

            }
        }


        public static TrinityPower PowerAvalanche
        {
            get
            {
                return new TrinityPower(SNOPower.X1_Barbarian_Avalanche_v2, 15f,
                    TargetUtil.GetBestClusterUnit(15f, 45f).Position);
            }
        }

        public static TrinityPower PowerIgnorePain
        {
            get { return new TrinityPower(SNOPower.Barbarian_IgnorePain); }
        }

        public static TrinityPower PowerEarthquake
        {
            get { return new TrinityPower(SNOPower.Barbarian_Earthquake); }
        }

        public static TrinityPower PowerWrathOfTheBerserker
        {
            get { return new TrinityPower(SNOPower.Barbarian_WrathOfTheBerserker); }
        }

        public static TrinityPower PowerCallOfTheAncients
        {
            get { return new TrinityPower(SNOPower.Barbarian_CallOfTheAncients, 4, 4); }
        }

        public static TrinityPower PowerBattleRage
        {
            get { return new TrinityPower(SNOPower.Barbarian_BattleRage); }
        }

        public static TrinityPower PowerSprint
        {
            get { return new TrinityPower(SNOPower.Barbarian_Sprint, 0, 0); }
        }

        public static TrinityPower PowerWarCry
        {
            get { return new TrinityPower(SNOPower.X1_Barbarian_WarCry_v2); }
        }

        public static TrinityPower PowerThreateningShout
        {
            get { return new TrinityPower(SNOPower.Barbarian_ThreateningShout); }
        }

        public static TrinityPower PowerGroundStomp
        {
            get { return new TrinityPower(SNOPower.Barbarian_GroundStomp); }
        }

        public static TrinityPower PowerRevenge
        {
            get { return new TrinityPower(SNOPower.Barbarian_Revenge); }
        }

        public static TrinityPower PowerFuriousCharge
        {
            get
            {
                var bestAoETarget = TargetUtil.GetBestPierceTarget(60f, !Settings.Combat.Misc.KillMonstersInAoE,
                    Settings.Combat.Misc.IgnoreElites);
                var target = CurrentTarget.IsBoss ? CurrentTarget : bestAoETarget;
                if (bestAoETarget != null && Sets.TheLegacyOfRaekor.IsSecondBonusActive)
                    return new TrinityPower(SNOPower.Barbarian_FuriousCharge, 60f, target.Position);
                return new TrinityPower(SNOPower.Barbarian_FuriousCharge, 60f, CurrentTarget.Position);
            }
        }

        public static TrinityPower PowerLeap
        {
            get
            {
                // For Call of Arreat rune. Will do all quakes on top of each other
                if (Legendary.LutSocks.IsEquipped && Runes.Barbarian.CallOfArreat.IsActive)
                {
                    Vector3 aoeTarget = TargetUtil.GetBestClusterPoint(7f, 9f, false);
                    SpellHistory.RecordSpell(SNOPower.Barbarian_Leap);
                    return new TrinityPower(SNOPower.Barbarian_Leap, 35f, aoeTarget);
                }
                else
                {
                    Vector3 aoeTarget = TargetUtil.GetBestClusterPoint(15f, 35f, false);
                    SpellHistory.RecordSpell(SNOPower.Barbarian_Leap);
                    return new TrinityPower(SNOPower.Barbarian_Leap, 35f, aoeTarget);
                }
            }
        }

        public static TrinityPower PowerRend
        {
            get { return new TrinityPower(SNOPower.Barbarian_Rend, 4, 4); }
        }

        public static TrinityPower PowerOverpower
        {
            get { return new TrinityPower(SNOPower.Barbarian_Overpower); }
        }

        public static TrinityPower PowerSeismicSlam
        {
            get
            {
                var target = TargetUtil.UnitsPlayerFacing(20f) >= 6
                    ? TargetUtil.GetClosestUnit(12f)
                    : TargetUtil.GetBestClusterUnit(12f, 12f);

                if (target != null)
                    return new TrinityPower(SNOPower.Barbarian_SeismicSlam, 11f, target.Position);
                return new TrinityPower(SNOPower.Barbarian_SeismicSlam, 11f, CurrentTarget.Position);
            }
        }

        public static TrinityPower PowerAncientSpear
        {
            get
            {
                var clusterTarget = Player.ParticipatingInTieredLootRun && !Sets.MightOfTheEarth.IsFullyEquipped
                    ? TargetUtil.GetBestRiftValueClusterPoint(60f, .1)
                    : TargetUtil.GetBestClusterPoint(60f);

                if (Runes.Barbarian.BoulderToss.IsActive)
                    return new TrinityPower(SNOPower.X1_Barbarian_AncientSpear, 60f, clusterTarget);

                return new TrinityPower(SNOPower.X1_Barbarian_AncientSpear, 60f,
                    TargetUtil.GetBestPierceTarget(40f).Position);
            }
        }

        public static TrinityPower PowerWhirlwind
        {
            get
            {
                return new TrinityPower(SNOPower.Barbarian_Whirlwind, 20f,
                    TargetUtil.GetZigZagTarget(CurrentTarget.Position, 20), TrinityPlugin.CurrentWorldDynamicId, -1, 0,
                    1);
            }
        }

        public static TrinityPower PowerHammerOfTheAncients
        {
            get
            {
                var target = TargetUtil.UnitsPlayerFacing(20f) >= 6
                    ? TargetUtil.GetClosestUnit(15f)
                    : TargetUtil.GetBestClusterUnit(15f, 15f);

                if (target != null)
                    return new TrinityPower(SNOPower.Barbarian_HammerOfTheAncients, 11f, target.Position);
                return new TrinityPower(SNOPower.Barbarian_HammerOfTheAncients, 11f, CurrentTarget.Position);
            }
        }

        public static TrinityPower PowerWeaponThrow
        {
            get { return new TrinityPower(SNOPower.X1_Barbarian_WeaponThrow, 60f, CurrentTarget.AcdId); }
        }

        public static TrinityPower PowerFrenzy
        {
            get { return new TrinityPower(SNOPower.Barbarian_Frenzy, 10f, CurrentTarget.AcdId); }
        }

        public static TrinityPower PowerBash
        {
            get { return new TrinityPower(SNOPower.Barbarian_Bash, 6f, CurrentTarget.AcdId); }
        }

        public static TrinityPower PowerCleave
        {
            get { return new TrinityPower(SNOPower.Barbarian_Cleave, 6f, CurrentTarget.AcdId); }
        }

        private static TrinityPower DestroyObjectPower
        {
            get
            {
                if (CanCast(SNOPower.Barbarian_Frenzy))
                    return new TrinityPower(SNOPower.Barbarian_Frenzy, 4f);

                if (CanCast(SNOPower.Barbarian_Bash))
                    return new TrinityPower(SNOPower.Barbarian_Bash, 4f);

                if (CanCast(SNOPower.Barbarian_Cleave))
                    return new TrinityPower(SNOPower.Barbarian_Cleave, 4f);

                if (CanCast(SNOPower.X1_Barbarian_WeaponThrow))
                    return new TrinityPower(SNOPower.X1_Barbarian_WeaponThrow, 4f);

                if (CanCast(SNOPower.Barbarian_Overpower))
                    return new TrinityPower(SNOPower.Barbarian_Overpower, 9);

                if (CanCast(SNOPower.Barbarian_Whirlwind))
                    return new TrinityPower(SNOPower.Barbarian_Whirlwind, 20f, CurrentTarget.Position);

                if (CanCast(SNOPower.Barbarian_Rend) && Player.PrimaryResourcePct >= 0.65)
                    return new TrinityPower(SNOPower.Barbarian_Rend, 12f);

                return DefaultPower;
            }
        }

        public static bool AllowSprintOOC
        {
            get { return _allowSprintOoc; }
            set { _allowSprintOoc = value; }
        }

        private static bool ShouldFuryDump
        {
            get
            {
                return Settings.Combat.Barbarian.FuryDumpWOTB && Player.PrimaryResourcePct >= 0.8 &&
                       GetHasBuff(SNOPower.Barbarian_WrathOfTheBerserker) ||
                       Settings.Combat.Barbarian.FuryDumpAlways && Player.PrimaryResourcePct >= 0.8;
            }
        }


    }
}