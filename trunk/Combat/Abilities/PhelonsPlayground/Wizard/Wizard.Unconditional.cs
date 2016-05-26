using Trinity.Framework;
using Trinity.Movement;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    internal partial class Wizard
    {
        public class Unconditional
        {

            public static TrinityPower PowerSelector()
            {
                if (CurrentTarget != null && CurrentTarget.IsUnit)
                {
                    Vector3 portLocation;
                    if (ShouldTeleport(out portLocation))
                        return CastTeleport(portLocation);
                }
                if (Player.IsIncapacitated) return null;

                if (ShouldDiamondSkin)
                    return CastDiamondSkin;

                TrinityPower buffPower;
                if (CastArmorSpell(out buffPower)) return buffPower;

                if (ShouldMagicWeapon)
                    return CastMagicWeapon;

                if (ShouldFamiliar)
                    return CastFamiliar;

                if (TalRashasCount == 3 && Legendary.WandOfWoh.IsEquipped)
                    return TalRasha.Flashfire.PowerSelector();

                return null;
            }

            private static bool ShouldDiamondSkin
            {
                get
                {
                    return Skills.Wizard.DiamondSkin.CanCast();
                }
            }

            private static TrinityPower CastDiamondSkin
            {
                get { return new TrinityPower(Skills.Wizard.DiamondSkin.SNOPower); }
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
                if (Core.Avoidance.InCriticalAvoidance(Player.Position) && !IsInParty)
                {
                    position = NavHelper.FindSafeZone(false, 1, Player.Position, true);
                    return true;
                }
                var bestDpsPosition = PhelonUtils.BestDpsPosition;
                if (bestDpsPosition != Vector3.Zero &&
                    (bestDpsPosition.Distance(Player.Position) > 12 || Skills.Wizard.Teleport.CanCast() && (Runes.Wizard.Calamity.IsActive ||
                     Runes.Wizard.SafePassage.IsActive) && TimeSincePowerUse(SNOPower.Wizard_Teleport) > 4500))
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

            private static bool ShouldMagicWeapon
            {
                get
                {
                    return
                        Player.PrimaryResource >= 25 &&
                        Skills.Wizard.MagicWeapon.CanCast() &&
                        !GetHasBuff(SNOPower.Wizard_MagicWeapon);
                }
            }

            private static TrinityPower CastMagicWeapon
            {
                get { return new TrinityPower(SNOPower.Wizard_MagicWeapon); }
            }

            private static bool ShouldFamiliar
            {
                get
                {
                    if (!Skills.Wizard.Familiar.CanCast() || Player.PrimaryResource < 25)
                        return false;

                    return !GetHasBuff(SNOPower.Wizard_Familiar);
                }
            }

            private static TrinityPower CastFamiliar
            {
                get { return new TrinityPower(SNOPower.Wizard_Familiar); }
            }

            private static bool CastArmorSpell(out TrinityPower buffPower)
            {
                if (Player.PrimaryResource >= 25)
                {
                    // Energy armor as priority cast if available and not buffed
                    if (Skills.Wizard.EnergyArmor.CanCast() && !GetHasBuff(SNOPower.Wizard_EnergyArmor))
                    {
                        buffPower = new TrinityPower(SNOPower.Wizard_EnergyArmor);
                        return true;
                    }
                    // Ice Armor
                    if (Skills.Wizard.IceArmor.CanCast() && !GetHasBuff(SNOPower.Wizard_IceArmor))
                    {
                        buffPower = new TrinityPower(SNOPower.Wizard_IceArmor);
                        return true;
                    }
                    // Storm Armor
                    if (Skills.Wizard.StormArmor.CanCast() && !GetHasBuff(SNOPower.Wizard_StormArmor))
                    {
                        buffPower = new TrinityPower(SNOPower.Wizard_StormArmor);
                        return true;
                    }
                }

                buffPower = null;
                return false;
            }
        }
    }
}

//private static bool CanTeleport => CanCast(SNOPower.Wizard_Teleport, CanCastFlags.NoTimer) ||
//                                   CanCast(SNOPower.Wizard_Archon_Teleport);

//private static bool ShouldTeleport(out Vector3 position)
//{
//    if (!CanTeleport)
//    {
//        position = Vector3.Zero;
//        return false;
//    }
//    // Ports to Closest HealthGlobe
//    if (TrinityPlugin.Player.CurrentHealthPct < Settings.Combat.Wizard.HealthGlobeLevel)
//    {
//        var safePoint = TargetUtil.GetBestHealthGlobeClusterPoint();
//        if (safePoint != Vector3.Zero)
//        {
//            position = TargetUtil.GetBestHealthGlobeClusterPoint();
//            return true;
//        }
//    }
//    position = Vector3.Zero;
//    return false;
//}

//private static TrinityPower CastTeleport(Vector3 position)
//{
//    return GetHasBuff(SNOPower.Wizard_Archon)
//        ? new TrinityPower(SNOPower.Wizard_Archon_Teleport, 45f, position)
//        : new TrinityPower(SNOPower.Wizard_Teleport, 45f, position);
//}