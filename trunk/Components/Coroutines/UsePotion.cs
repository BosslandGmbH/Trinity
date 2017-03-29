using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines
{
    public class UsePotion
    {
        public static async Task<bool> Execute()
        {
            if (ShouldUsePotion())
            {
                DrinkPotion();
                return true;
            }
            return false;
        }

        public static bool ShouldUsePotion()
        {
            if (Core.Player == null || Combat.TrinityCombat.Routines.Current == null)
                return false;

            if (Core.Player.CurrentHealthPct > Combat.TrinityCombat.Routines.Current.PotionHealthPct)
                return false;

            if (Core.Player.IsIncapacitated || !(Core.Player.CurrentHealthPct > 0) || Core.Player.IsInTown)
                return false;

            if (SpellHistory.TimeSinceUse(SNOPower.DrinkHealthPotion) <= TimeSpan.FromSeconds(30))
                return false;

            return Core.Player.CurrentHealthPct <= Combat.TrinityCombat.Routines.Current.PotionHealthPct;
        }

        public static bool DrinkPotion()
        {
            var legendaryPotions = Core.Inventory.Backpack.Where(i => i.InternalName.ToLower().Contains("healthpotion_legendary_")).ToList();
            if (legendaryPotions.Any())
            {
                Core.Logger.Verbose(LogCategory.None, "Using Potion", 0);
                var dynamicId = legendaryPotions.First().AnnId;
                InventoryManager.UseItem(dynamicId);
                SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
                SnapShot.Record();
                return true;
            }

            var potion = InventoryManager.BaseHealthPotion;
            if (potion != null)
            {
                Core.Logger.Verbose(LogCategory.None, "Using Potion", 0);
                InventoryManager.UseItem(potion.AnnId);
                SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
                SnapShot.Record();
                return true;
            }

            Core.Logger.Verbose(LogCategory.None, "No Available potions!", 0);
            return false;
        }
    }
}