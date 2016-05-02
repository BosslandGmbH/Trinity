using Trinity.Config.Combat;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial class Barbarian
    {

        public static TrinityPower UnconditionalPower()
        {
            if (ShouldIgnorePain)
                return CastIgnorePain;

            if (ShouldWrathOfTheBerserker)
                return CastWrathOfTheBerserker;

            if (Player.IsIncapacitated) return null;

            if (ShouldUseWarCry)
                return CastWarCry;

            if (ShouldCallOfTheAncients)
                return CastCallOfTheAncients;
            return null;
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

                if (GetHasBuff(SNOPower.Barbarian_IgnorePain))
                    return false;

                return Player.IsFrozen || Player.IsRooted || Player.IsJailed;
            }
        }

        public static TrinityPower CastIgnorePain => new TrinityPower(SNOPower.Barbarian_IgnorePain);

        public static bool ShouldCallOfTheAncients
        {
            get
            {
                if (!CanCast(SNOPower.Barbarian_CallOfTheAncients))
                    return false;

                return !Sets.ImmortalKingsCall.IsFirstBonusActive && CurrentTarget != null &&
                       (CurrentTarget.IsEliteRareUnique || TargetUtil.AnyMobsInRange(25f, 3)) ||
                       Sets.ImmortalKingsCall.IsFirstBonusActive && TrinityPlugin.PlayerOwnedAncientCount < 3;
            }
        }

        public static TrinityPower CastCallOfTheAncients => new TrinityPower(SNOPower.Barbarian_CallOfTheAncients);

        public static bool ShouldUseWarCry
        {
            get
            {
                if (!CanCast(SNOPower.X1_Barbarian_WarCry_v2, CanCastFlags.NoTimer))
                    return false;

                if (Player.PrimaryResource <= 40 || Skills.Barbarian.WarCry.TimeSinceUse >= Settings.Combat.Barbarian.WarCryWaitDelay)
                    return true;

                if (CurrentTarget != null)
                    return !Legendary.BladeOfTheTribes.IsEquipped || TargetUtil.AnyMobsInRange(20f);

                return !GetHasBuff(SNOPower.X1_Barbarian_WarCry_v2);
            }
        }
        
        public static TrinityPower CastWarCry => new TrinityPower(SNOPower.X1_Barbarian_WarCry_v2);
        public static bool ShouldUseBattleRage
        {
            get
            {
                if (!CanCast(SNOPower.Barbarian_BattleRage, CanCastFlags.NoTimer) || Player.PrimaryResource < 20)
                    return false;

                var shouldRefreshTaeguk = GetHasBuff(SNOPower.ItemPassive_Unique_Gem_015_x1) && !Hotbar.Contains(SNOPower.Barbarian_Whirlwind) &&
                    Skills.Barbarian.BattleRage.TimeSinceUse >= 2300 && Skills.Barbarian.BattleRage.TimeSinceUse <= 3000;

                return !GetHasBuff(SNOPower.Barbarian_BattleRage) || shouldRefreshTaeguk;
            }
        }
        public static TrinityPower PowerBattleRage => new TrinityPower(SNOPower.Barbarian_BattleRage);

        public static bool ShouldWrathOfTheBerserker
        {
            get
            {
                if (!CanCast(SNOPower.Barbarian_WrathOfTheBerserker))
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

        public static TrinityPower CastWrathOfTheBerserker { get { return new TrinityPower(SNOPower.Barbarian_WrathOfTheBerserker); } }
    }
}