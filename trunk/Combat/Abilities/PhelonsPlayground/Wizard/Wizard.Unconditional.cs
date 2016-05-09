using Trinity.Reference;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    internal partial class Wizard
    {
        public class Unconditional
        {

            public static TrinityPower PowerSelector()
            {
                if (Player.IsIncapacitated) return null;

                TrinityPower buffPower;
                if (CastArmorSpell(out buffPower)) return buffPower;

                if (ShouldMagicWeapon)
                    return CastMagicWeapon;

                if (ShouldFamiliar)
                    return CastFamiliar;
                return null;
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