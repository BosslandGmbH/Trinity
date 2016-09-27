using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Framework;
using Trinity.Reference;
using Trinity.Technicals;
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

        public static bool ShouldSnapshot()
        {
            if (!Core.Settings.Combat.Misc.TryToSnapshot || !Gems.BaneOfTheStricken.IsEquipped ||
                ZetaDia.Me.AttacksPerSecond < Core.Settings.Combat.Misc.SnapshotAttackSpeed || Core.Player.CurrentHealthPct >= 1)
                return false;

            if (SnapShot.Last.AttacksPerSecond >= Core.Settings.Combat.Misc.SnapshotAttackSpeed)
                return false;

            return true;
        }

        public static bool ShouldUsePotion()
        {
            if (Core.Player.CurrentHealthPct > Combat.Routines.Current.EmergencyHealthPct)
                return false;

            if (Core.Player.IsIncapacitated || !(Core.Player.CurrentHealthPct > 0) || Core.Player.IsInTown)
                return false;

            if (SpellHistory.TimeSinceUse(SNOPower.DrinkHealthPotion) <= TimeSpan.FromSeconds(30))
                return false;

            return Core.Player.CurrentHealthPct <= Combat.Routines.Current.EmergencyHealthPct || ShouldSnapshot();
        }

        public static bool DrinkPotion()
        {
            var logEntry = ShouldSnapshot() ? "Using Potion to Snapshot Bane of the Stricken!" : "Using Potion";

            var legendaryPotions = Core.Inventory.Backpack.Where(i => i.InternalName.ToLower().Contains("healthpotion_legendary_")).ToList();
            if (legendaryPotions.Any())
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, logEntry, 0);
                var dynamicId = legendaryPotions.First().AnnId;
                ZetaDia.Me.Inventory.UseItem(dynamicId);
                SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
                SnapShot.Record();
                return true;
            }

            var potion = ZetaDia.Me.Inventory.BaseHealthPotion;
            if (potion != null)
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, logEntry, 0);
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