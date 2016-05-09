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
                Vector3 portLocation = Vector3.Zero;

                if (ShouldTeleport(out portLocation))
                    return CastTeleport(portLocation);

                if (Player.IsIncapacitated) return null;

                if (ShouldHydra())
                    return CastHydra;

                if (ShouldExplosiveBlast)
                    return CastExplosiveBlast;

                if (ShouldArchon())
                    return CastArchon;
                
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
                    return CanCast(SNOPower.Wizard_ArcaneTorrent) && PhelonTargeting.BestAoeUnit(true).Distance < 45;
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
            {
                get
                {
                    return Skills.Wizard.ExplosiveBlast.CanCast() &&
                           (GetHasBuff(SNOPower.Wizard_ExplosiveBlast) &&
                            GetBuffStacks(SNOPower.Wizard_ExplosiveBlast) < 4 ||
                            TimeSincePowerUse(SNOPower.Wizard_ExplosiveBlast) > 5);
                }
            }

            private static TrinityPower CastArcaneTorrent
            {
                get
                {
                    return new TrinityPower(SNOPower.Wizard_ArcaneTorrent, 55f,
                        PhelonTargeting.BestAoeUnit(true).Position);
                }
            }

            private static bool CanTeleport
            {
                get
                {
                    return Skills.Wizard.Teleport.CanCast() ||
                           Skills.Wizard.ArchonTeleport.CanCast();
                }
            }

            private static bool ShouldTeleport(out Vector3 position)
            {
                if (!CanTeleport)
                {
                    position = Vector3.Zero;
                    return false;
                }
                // Ports to Closest HealthGlobe
                if (TrinityPlugin.Player.CurrentHealthPct < Settings.Combat.Wizard.HealthGlobeLevel)
                {
                    var safePoint = TargetUtil.GetBestHealthGlobeClusterPoint(5, 40);
                    if (safePoint != Vector3.Zero)
                    {
                        position = safePoint;
                        return true;
                    }
                }

                //Ports out Avoidance
                if (Core.Avoidance.InCriticalAvoidance(Player.Position))
                {
                    position = NavHelper.FindSafeZone(false, 1, Player.Position, true);
                    return true;
                }

                var bestDpsPosition = PhelonUtils.BestDpsPosition;
                if (bestDpsPosition != Vector3.Zero &&
                    (bestDpsPosition.Distance(Player.Position) > 7 || Runes.Wizard.Calamity.IsActive ||
                     Runes.Wizard.SafePassage.IsActive && TimeSincePowerUse(SNOPower.Wizard_Teleport) > 4500))
                {
                    position = bestDpsPosition;
                    return true;
                }
                position = Vector3.Zero;
                return false;
            }

            private static TrinityPower CastTeleport(Vector3 position)
            {
                return Skills.Wizard.ArchonStrike.IsActive
                    ? new TrinityPower(SNOPower.Wizard_Archon_Teleport, 40f, position)
                    : new TrinityPower(SNOPower.Wizard_Teleport, 40f, position);
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
                    .ThenBy(p => p.Power.TargetPosition.Distance2DSqr(PhelonTargeting.BestAoeUnit(true).Position))
                    .FirstOrDefault();

                return lastCast != null && lastCast.TargetPosition.Distance2DSqr(PhelonTargeting.BestAoeUnit(true).Position) > 25;
            }

            private static TrinityPower CastHydra
            {
                get
                {
                    return new TrinityPower(SNOPower.Wizard_Hydra, 55f, PhelonTargeting.BestAoeUnit(true).Position);
                }
            }

            #region Archon Skills

            public static bool ShouldArchon()
            {
                if (!Skills.Wizard.Archon.CanCast() || GetHasBuff(SNOPower.Wizard_Archon))
                    return false;

                if (Sets.ChantodosResolve.IsFullyEquipped && Player.PrimaryResourcePct > 0.10)
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

                return PhelonTargeting.BestAoeUnit(true).IsBoss;
            }

            public static TrinityPower CastArchon
            {
                get { return new TrinityPower(SNOPower.Wizard_Archon); }
            }

            #endregion

        }
    }
}