using Trinity.Config.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Monk
{
    partial class Monk
    {
        internal class Innas
        {
            public static TrinityPower PowerSelector()
            {
                if (Player.IsIncapacitated) return null;
                TrinityActor target;

                if (ShouldDashingStrike(out target))
                    return DashingStrike(target);

                if (ShouldInnerSanctuary)
                    return InnerSanctuary;

                if (ShouldEpiphany)
                    return Epiphany;

                if (ShouldBreathOfHeaven)
                    return BreathOfHeaven;

                if (ShouldCycloneStrike)
                    return CycloneStrike;

                if (ShouldWayOfTheHundredFists(out target))
                    return WayOfTheHundredFists(target);

                return null;
            }

            #region Conditions

            private static bool ShouldDashingStrike(out TrinityActor target)
            {
                target = null;

                var charges = Skills.Monk.DashingStrike.Charges;

                if (!Skills.Monk.DashingStrike.CanCast() ||
                    TrinityPlugin.ShouldWaitForLootDrop || charges < 1 ||
                    ZetaDia.Me.AttacksPerSecond < 4) // Testing waiting for flying dragon proc before pulling new mobs
                    return false;

                target = PhelonTargeting.BestAoeUnit(45, true);
                if (target == null || target.Distance > 50 || TimeSincePowerUse(SNOPower.X1_Monk_DashingStrike) < Settings.Combat.Monk.DashingStrikeDelay)
                    return false;

                if (charges > 1 || CacheData.Buffs.HasCastingShrine)
                    return true;

                if (TargetUtil.AnyMobsInRange(60f) || CacheData.Buffs.HasCastingShrine)
                    return true;

                return false;
            }

            private static bool ShouldEpiphany
            {
                get
                {
                    if (!Skills.Monk.Epiphany.CanCast() || GetHasBuff(SNOPower.X1_Monk_Epiphany))
                        return false;

                    // Epiphany mode is 'Off Cooldown'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenReady)
                        return true;

                    // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
                    if (CurrentTarget.IsTreasureGoblin && Settings.Combat.Monk.UseEpiphanyGoblin ||
                        Player.CurrentHealthPct <= 0.39 &&
                        Settings.Combat.Monk.EpiphanyEmergencyHealth || GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
                        ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned)
                        return true;

                    // Epiphany mode is 'Whenever in Combat'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenInCombat &&
                        TargetUtil.AnyMobsInRange(80f))
                        return true;

                    // Epiphany mode is 'Use when Elites are nearby'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.Normal && TargetUtil.AnyElitesInRange(80f))
                        return true;

                    // Epiphany mode is 'Hard Elites Only'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.HardElitesOnly && HardElitesPresent)
                        return true;

                    return false;
                }
            }

            private static bool ShouldWayOfTheHundredFists(out TrinityActor target)
            {
                target = null;

                if (!Skills.Monk.WayOfTheHundredFists.CanCast())
                    return false;

                target = PhelonTargeting.BestAoeUnit(45, true);
                if (target != null && target.Distance < 12)
                    return true;

                target = PhelonUtils.BestAuraUnit(SNOPower.Monk_WayOfTheHundredFists, 12f, true);
                if (target != null && target.Distance < 12)
                    return true;

                target = TargetUtil.GetClosestUnit(12f);
                return target != null;
            }

            private static bool ShouldInnerSanctuary => Skills.Monk.InnerSanctuary.CanCast() &&
                                                        PhelonTargeting.BestAoeUnit(45, true).Distance < 20;

            private static bool ShouldCycloneStrike => Skills.Monk.CycloneStrike.CanCast() &&
                                                       Player.PrimaryResource > cycloneStrikeSpirit &&
                                                       TimeSincePowerUse(SNOPower.Monk_CycloneStrike) >
                                                       Settings.Combat.Monk.CycloneStrikeDelay;

            private static bool ShouldBreathOfHeaven => Skills.Monk.BreathOfHeaven.CanCast() &&
                                                        PhelonTargeting.BestAoeUnit(45, true).Distance < 20;

            #endregion

            #region Expressions

            private static TrinityPower DashingStrike(TrinityActor target)
                => new TrinityPower(SNOPower.X1_Monk_DashingStrike, 65, target.AcdId);

            private static TrinityPower InnerSanctuary => new TrinityPower(SNOPower.X1_Monk_InnerSanctuary);

            private static TrinityPower Epiphany => new TrinityPower(SNOPower.X1_Monk_Epiphany);

            private static TrinityPower CycloneStrike => new TrinityPower(SNOPower.Monk_CycloneStrike);

            private static TrinityPower WayOfTheHundredFists(TrinityActor target)
                => new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, 12f, target.AcdId);

            private static TrinityPower BreathOfHeaven => new TrinityPower(SNOPower.Monk_BreathOfHeaven);

            #endregion

        }
    }
}
