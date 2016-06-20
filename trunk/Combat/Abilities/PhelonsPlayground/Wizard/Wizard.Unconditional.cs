using Trinity.Framework;
using Trinity.Movement;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    internal partial class Wizard
    {
        public class Unconditional
        {

            public static TrinityPower PowerSelector()
            {
                if (CurrentTarget != null && (CurrentTarget.IsUnit || IsFlashfire && CurrentTarget.Distance > 7))
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

                if (IsFlashfire)
                    return TalRasha.Flashfire.PowerSelector();

                if (ShouldArcaneBlast)
                    return CastArcaneBlast;

                return null;
            }

            private static bool ShouldArcaneBlast => Skills.Wizard.ArchonBlast.CanCast();

            private static TrinityPower CastArcaneBlast => new TrinityPower(Skills.Wizard.ArchonBlast.SNOPower);

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

            public static bool CanTeleport
            {
                get
                {
                    return Skills.Wizard.Teleport.CanCast() ||
                           Skills.Wizard.ArchonTeleport.CanCast();
                }
            }

            private static bool ShouldTeleport(out Vector3 position)
            {
                position = Vector3.Zero;
                if (!CanTeleport)
                    return false;
                if (IsFlashfire)
                {
                    Logger.Log("Porting to get closer for Woh");
                    position = CurrentTarget.Position;
                    return true;
                }
                // Ports to Closest HealthGlobe
                if (TrinityPlugin.Player.CurrentHealthPct < Settings.Combat.Wizard.HealthGlobeLevel)
                {
                    var safePoint = TargetUtil.GetBestHealthGlobeClusterPoint(5, 40);
                    if (safePoint != Vector3.Zero)
                    {
                        Logger.Log("Porting to get Health Globe!");
                        position = safePoint;
                        return true;
                    }
                }

                //Ports out Avoidance
                if (Core.Avoidance.InCriticalAvoidance(Player.Position) && !IsInParty)
                {
                    Logger.Log("Porting to get out of Avoidance!");
                    position = Core.Avoidance.Avoider.SafeSpot;
                    return true;
                }

                if (Skills.Wizard.Archon.CooldownRemaining < Skills.Wizard.Teleport.Cooldown.Milliseconds)
                    return false;

                if (Skills.Wizard.Archon.CanCast())
                {
                    Logger.Log("Porting to get closer for Archon");
                    position = PhelonTargeting.BestAoeUnit(45f, true).Position;
                    return true;
                }
                var maxRange = DMOCount > 2 || GetHasBuff(Skills.Wizard.Archon.SNOPower) || IsInParty ? 14 : 40;

                var bestDpsPosition = IsInParty && PhelonGroupSupport.Monk != null
                        ? PhelonGroupSupport.Monk.Position
                        : PhelonUtils.BestDpsPosition(35, maxRange, true);

                if (bestDpsPosition != Vector3.Zero && bestDpsPosition.Distance(Player.Position) > maxRange)
                {
                    Logger.Log("Porting to get to best DPS position!");
                    position = bestDpsPosition;
                    return true;
                }

                if (Skills.Wizard.Teleport.CanCast() &&
                    (Runes.Wizard.Calamity.IsActive && 
                     TimeSincePowerUse(Skills.Wizard.Teleport.SNOPower) > 8000 ||
                     Runes.Wizard.SafePassage.IsActive &&
                     TimeSincePowerUse(SNOPower.Wizard_Teleport) > 4500))
                {
                    Logger.Log("Porting to get to Safe Passage or Calamity buff!");
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