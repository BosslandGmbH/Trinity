using Trinity.Config.Combat;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial class Barbarian
    {
        public class Unconditional
        {
            public static TrinityPower PowerSelector()
            {

                if (ShouldIgnorePain)
                    return CastIgnorePain;

                if (ShouldWrathOfTheBerserker)
                    return CastWrathOfTheBerserker;

                if (Player.IsIncapacitated) return null;

                if (ShouldThreateningShout)
                    return CastThreateningShout;

                if (ShouldBattleRage)
                    return CastBattleRage;

                if (ShouldUseWarCry)
                    return CastWarCry;

                if (ShouldCallOfTheAncients)
                    return CastCallOfTheAncients;
                return null;
            }

            public static bool ShouldBattleRage
            {
                get
                {

                    return !Player.IsIncapacitated && CanCast(SNOPower.Barbarian_BattleRage, CanCastFlags.NoTimer) &&
                           !GetHasBuff(SNOPower.Barbarian_BattleRage) && Player.PrimaryResource >= 20;
                }
            }

            public static TrinityPower CastBattleRage
            {
                get { return new TrinityPower(SNOPower.Barbarian_BattleRage); }
            }

            public static bool ShouldThreateningShout
            {
                get
                {
                    return Skills.Barbarian.ThreateningShout.CanCast() &&
                           (Skills.Barbarian.ThreateningShout.TimeSinceUse > 2000 || Player.PrimaryResourcePct < 0.25 || 
                           Legendary.BladeOfTheTribes.IsEquipped || CurrentTarget != null && zDPSEquipped);
                }
            }

            public static TrinityPower CastThreateningShout
            {
                get { return new TrinityPower(Skills.Barbarian.ThreateningShout.SNOPower); }
            }

            public static bool ShouldIgnorePain
            {
                get
                {
                    if (!CanCast(SNOPower.Barbarian_IgnorePain))
                        return false;

                    if (Settings.Combat.Barbarian.IgnorePainOffCooldown)
                        return true;

                    if (Player.CurrentHealthPct <= Settings.Combat.Barbarian.IgnorePainMinHealthPct)
                        return true;

                    if (GetHasBuff(SNOPower.Barbarian_IgnorePain) && Skills.Barbarian.IgnorePain.TimeSinceUse < 4500)
                        return false;

                    return Player.IsFrozen || Player.IsRooted || Player.IsJailed;
                }
            }

            public static TrinityPower CastIgnorePain
            {
                get { return new TrinityPower(SNOPower.Barbarian_IgnorePain); }
            }

            public static bool ShouldCallOfTheAncients
            {
                get
                {
                    if (!CanCast(SNOPower.Barbarian_CallOfTheAncients))
                        return false;

                    return !Sets.ImmortalKingsCall.IsFirstBonusActive && CurrentTarget != null &&
                           (CurrentTarget.IsElite || TargetUtil.AnyMobsInRange(25f, 3)) ||
                           Sets.ImmortalKingsCall.IsFirstBonusActive && TrinityPlugin.PlayerOwnedAncientCount < 3;
                }
            }

            public static TrinityPower CastCallOfTheAncients
            {
                get { return new TrinityPower(SNOPower.Barbarian_CallOfTheAncients); }
            }

            public static bool ShouldUseWarCry
            {
                get
                {
                    if (!CanCast(SNOPower.X1_Barbarian_WarCry_v2, CanCastFlags.NoTimer))
                        return false;

                    if (Player.PrimaryResource <= 40 || CurrentTarget != null && zDPSEquipped ||
                        Legendary.BladeOfTheTribes.IsEquipped || 
                        Skills.Barbarian.WarCry.TimeSinceUse >= Settings.Combat.Barbarian.WarCryWaitDelay)
                        return true;

                    if (CurrentTarget != null)
                        return Legendary.BladeOfTheTribes.IsEquipped && TargetUtil.AnyMobsInRange(20f) && Barbarian.zDPSEquipped;

                    return !GetHasBuff(SNOPower.X1_Barbarian_WarCry_v2);
                }
            }

            public static TrinityPower CastWarCry
            {
                get { return new TrinityPower(SNOPower.X1_Barbarian_WarCry_v2); }
            }

            public static bool ShouldUseBattleRage
            {
                get
                {
                    if (!CanCast(SNOPower.Barbarian_BattleRage, CanCastFlags.NoTimer) || Player.PrimaryResource < 20)
                        return false;

                    var shouldRefreshTaeguk = GetHasBuff(SNOPower.ItemPassive_Unique_Gem_015_x1) &&
                                              !Hotbar.Contains(SNOPower.Barbarian_Whirlwind) &&
                                              Skills.Barbarian.BattleRage.TimeSinceUse >= 3000;

                    return !GetHasBuff(SNOPower.Barbarian_BattleRage) || shouldRefreshTaeguk;
                }
            }

            public static TrinityPower PowerBattleRage
            {
                get { return new TrinityPower(SNOPower.Barbarian_BattleRage); }
            }

            public static bool ShouldWrathOfTheBerserker
            {
                get
                {
                    if (!CanCast(SNOPower.Barbarian_WrathOfTheBerserker) || GetHasBuff(SNOPower.Barbarian_WrathOfTheBerserker))
                        return false;

                    if (Settings.Combat.Barbarian.WOTBMode == BarbarianWOTBMode.WhenReady ||
                        GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) || Player.CurrentHealthPct <= 0.30 ||
                        Player.IsFrozen || Player.IsRooted || Player.IsJailed)
                        return true;

                    if (CurrentTarget == null) return false;

                    if (TargetUtil.AnyMobsInRange(25) &&
                        Settings.Combat.Barbarian.WOTBMode == BarbarianWOTBMode.WhenInCombat ||
                        //Goblin
                        CurrentTarget.IsTreasureGoblin && Settings.Combat.Barbarian.UseWOTBGoblin ||
                        //Ignore Elites
                        IgnoringElites && (TargetUtil.AnyMobsInRange(35f, 8) || TargetUtil.AnyMobsInRange(50f, 10) ||
                                           TargetUtil.AnyMobsInRange(CombatOverrides.EffectiveTrashRadius,
                                               CombatOverrides.EffectiveTrashSize)) ||
                        //Elite Area
                        (Settings.Combat.Barbarian.WOTBMode != BarbarianWOTBMode.HardElitesOnly &&
                         TargetUtil.AnyElitesInRange(20f)) ||
                        (Settings.Combat.Barbarian.WOTBMode == BarbarianWOTBMode.HardElitesOnly && HardElitesPresent))
                    {
                        return true;
                    }
                    return false;
                }
            }

            public static TrinityPower CastWrathOfTheBerserker
            {
                get { return new TrinityPower(SNOPower.Barbarian_WrathOfTheBerserker); }
            }
        }
    }
}