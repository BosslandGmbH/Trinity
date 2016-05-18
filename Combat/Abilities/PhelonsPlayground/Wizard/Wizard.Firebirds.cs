using System;
using System.Linq;
using Trinity.Framework;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    internal partial class Wizard
    {
        public class Firebirds
        {
            public static TrinityPower PowerSelector()
            {

                if (ShouldArchon())
                    return CastArchon;

                if (Player.IsIncapacitated) return null;

                if (ShouldHydra())
                    return CastHydra;

                if (ShouldExplosiveBlast)
                    return CastExplosiveBlast;
                
                var power = Archon.PowerSelector();
                if (power != null)
                    return power;

                if (ShouldArcaneTorrent)
                    return CastArcaneTorrent;

                return null;
            }

            private static bool ShouldArcaneTorrent
            {
                get
                {
                    return CanCast(SNOPower.Wizard_ArcaneTorrent) && PhelonTargeting.BestAoeUnit(45, true) != null;
                }
            }

            private static TrinityPower CastExplosiveBlast
            {
                get
                {
                    return new TrinityPower(SNOPower.Wizard_ExplosiveBlast);
                }
            }

            private static bool ShouldExplosiveBlast
                =>
                    Skills.Wizard.ExplosiveBlast.CanCast() && TargetUtil.AnyMobsInRange(12f) &&
                    TimeSincePowerUse(SNOPower.Wizard_ExplosiveBlast) > 4;

            //(GetHasBuff(SNOPower.Wizard_ExplosiveBlast) && GetBuffStacks(SNOPower.Wizard_ExplosiveBlast) < 4 ||
            //TimeSincePowerUse(SNOPower.Wizard_ExplosiveBlast) > 5);

            private static TrinityPower CastArcaneTorrent
            {
                get
                {
                    return new TrinityPower(SNOPower.Wizard_ArcaneTorrent, 55f,
                        PhelonTargeting.BestAoeUnit(45, true).Position);
                }
            }

            private static bool ShouldHydra()
            {
                if (!Skills.Wizard.Hydra.CanCast())
                    return false;

                if (Legendary.SerpentsSparker.IsEquipped && LastPowerUsed == SNOPower.Wizard_Hydra &&
                    SpellHistory.SpellUseCountInTime(SNOPower.Wizard_Hydra, TimeSpan.FromSeconds(2)) < 2)
                    return true;
                var lastCast = SpellHistory.History
                    .Where(p => p.Power.SNOPower == SNOPower.Wizard_Hydra && p.TimeSinceUse < TimeSpan.FromSeconds(14))
                    .OrderBy(s => s.TimeSinceUse)
                    .ThenBy(p => p.Power.TargetPosition.Distance2DSqr(PhelonTargeting.BestAoeUnit(45,true).Position))
                    .FirstOrDefault();

                return lastCast != null && lastCast.TargetPosition.Distance2DSqr(PhelonTargeting.BestAoeUnit(45, true).Position) > 25;
            }

            private static TrinityPower CastHydra
            {
                get
                {
                    return new TrinityPower(SNOPower.Wizard_Hydra, 55f, PhelonTargeting.BestAoeUnit(45, true).Position);
                }
            }

            #region Archon Skills

            public static bool ShouldArchon()
            {
                if (!Skills.Wizard.Archon.CanCast() || Skills.Wizard.ArchonBlast.CanCast())
                    return false;

                if (Player.PrimaryResourcePct < 0.20)
                    return true;

                if (Sets.ChantodosResolve.IsFullyEquipped)
                {
                    return GetHasBuff(SNOPower.P3_ItemPassive_Unique_Ring_021) &&
                           GetBuffStacks(SNOPower.P3_ItemPassive_Unique_Ring_021) > 19;
                }

                if (Settings.Combat.Wizard.ArchonElitesOnly &&
                    TargetUtil.AnyElitesInRange(Settings.Combat.Wizard.ArchonEliteDistance))
                    return true;

                if (!Settings.Combat.Wizard.ArchonElitesOnly &&
                    TargetUtil.AnyMobsInRange(Settings.Combat.Wizard.ArchonMobDistance,
                        Settings.Combat.Wizard.ArchonMobCount))
                    return true;

                return PhelonTargeting.BestAoeUnit(45, true).IsBoss;
            }

            public static TrinityPower CastArchon
            {
                get { return new TrinityPower(SNOPower.Wizard_Archon); }
            }

            #endregion

        }
    }
}