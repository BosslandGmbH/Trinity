using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines
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
            if (Core.Player == null || Combat.Routines.Current == null)
                return false;

            if (Core.Player.CurrentHealthPct > Combat.Routines.Current.PotionHealthPct)
                return false;

            if (Core.Player.IsIncapacitated || !(Core.Player.CurrentHealthPct > 0) || Core.Player.IsInTown)
                return false;

            if (SpellHistory.TimeSinceUse(SNOPower.DrinkHealthPotion) <= TimeSpan.FromSeconds(30))
                return false;

            return Core.Player.CurrentHealthPct <= Combat.Routines.Current.PotionHealthPct;
        }

        public static bool DrinkPotion()
        {

            var legendaryPotions = Core.Inventory.Backpack.Where(i => i.InternalName.ToLower().Contains("healthpotion_legendary_")).ToList();
            if (legendaryPotions.Any())
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Using Potion", 0);
                var dynamicId = legendaryPotions.First().AnnId;
                ZetaDia.Me.Inventory.UseItem(dynamicId);
                SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
                SnapShot.Record();
                return true;
            }

            var potion = ZetaDia.Me.Inventory.BaseHealthPotion;
            if (potion != null)
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Using Potion", 0);
                ZetaDia.Me.Inventory.UseItem(potion.AnnId);
                SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
                SnapShot.Record();
                return true;
            }

            Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "No Available potions!", 0);
            return false;
        }

    }
}