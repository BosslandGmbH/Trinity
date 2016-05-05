using System;
using System.Linq;
using Trinity.Framework;
using Trinity.Reference;
using Zeta.Common;
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

            private static TrinityPower CastArcaneTorrent
            {
                get
                {
                    return new TrinityPower(SNOPower.Wizard_ArcaneTorrent, 45f,
                        PhelonTargeting.BestAoeUnit(true).Position);
                }
            }

            private static bool CanTeleport
            {
                get
                {
                    return CanCast(SNOPower.Wizard_Teleport, CanCastFlags.NoTimer) ||
                           CanCast(SNOPower.Wizard_Archon_Teleport);
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
                //Ports to Closest Occ Buff
                var closestOc = PhelonUtils.ClosestOcculous;
                if (closestOc != Vector3.Zero && closestOc.Distance(PhelonTargeting.BestAoeUnit().Position) < 10 &&
                    (CanCast(SNOPower.Wizard_Archon) || GetHasBuff(SNOPower.Wizard_Archon)))
                {
                    position = closestOc;
                    return true;
                }
                //Ports out Avoidance
                if (Core.Avoidance.InCriticalAvoidance(Player.Position) ||
                    !CanCast(SNOPower.Wizard_Archon) && !GetHasBuff(SNOPower.Wizard_Archon) &&
                    Core.Avoidance.InAvoidance(Player.Position))
                {
                    position = NavHelper.FindSafeZone(false, 1, Player.Position, true);
                    return true;
                }

                if (Runes.Wizard.Calamity.IsActive ||
                    Runes.Wizard.SafePassage.IsActive && TimeSincePowerUse(SNOPower.Wizard_Teleport) > 4500)
                {
                    if (CanCast(SNOPower.Wizard_Archon) || GetHasBuff(SNOPower.Wizard_Archon) &&
                        PhelonTargeting.BestAoeUnit() != null)
                    {
                        position = PhelonTargeting.BestAoeUnit().Position;
                        return true;
                    }
                    position = NavHelper.FindSafeZone(false, 1, Player.Position, true);
                    return true;
                }
                position = Vector3.Zero;
                return false;
            }

            private static TrinityPower CastTeleport(Vector3 position)
            {
                return GetHasBuff(SNOPower.Wizard_Archon)
                    ? new TrinityPower(SNOPower.Wizard_Archon_Teleport, 40f, position)
                    : new TrinityPower(SNOPower.Wizard_Teleport, 40f, position);
            }

            private static bool ShouldHydra()
            {
                if (!CanCast(SNOPower.Wizard_Hydra, CanCastFlags.NoTimer))
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
                if (!CanCast(SNOPower.Wizard_Archon) || GetHasBuff(SNOPower.Wizard_Archon))
                    return false;

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