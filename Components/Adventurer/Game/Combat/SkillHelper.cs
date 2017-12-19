using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Game.Combat
{
    public static class SkillHelper
    {
        /// <summary>
        /// Gets the default weapon power based on the current equipped primary weapon
        /// </summary>
        public static SNOPower DefaultWeaponPower
        {
            get
            {
                var lhItem = InventoryManager.Equipped.FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand);

                if (lhItem == null)
                    return SNOPower.None;

                switch (lhItem.ItemType)
                {
                    default:
                        return SNOPower.Weapon_Melee_Instant;

                    case ItemType.Axe:
                    case ItemType.CeremonialDagger:
                    case ItemType.Dagger:
                    case ItemType.Daibo:
                    case ItemType.FistWeapon:
                    case ItemType.Mace:
                    case ItemType.Polearm:
                    case ItemType.Spear:
                    case ItemType.Staff:
                    case ItemType.Sword:
                    case ItemType.MightyWeapon:
                        return SNOPower.Weapon_Melee_Instant;

                    case ItemType.Wand:
                        return SNOPower.Weapon_Ranged_Wand;

                    case ItemType.Bow:
                    case ItemType.Crossbow:
                    case ItemType.HandCrossbow:
                        return SNOPower.Weapon_Ranged_Projectile;
                }
            }
        }

        /// <summary>
        /// Gets the default weapon distance based on the current equipped primary weapon
        /// </summary>
        public static float DefaultWeaponDistance
        {
            get
            {
                switch (DefaultWeaponPower)
                {
                    case SNOPower.Weapon_Ranged_Instant:
                    case SNOPower.Weapon_Ranged_Projectile:
                        return 65f;

                    case SNOPower.Weapon_Ranged_Wand:
                        return 55f;

                    default:
                        return 12f;
                }
            }
        }
    }
}