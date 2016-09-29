using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Zeta.Bot.Coroutines;
using Zeta.Game;

namespace Trinity.Coroutines
{
    public class EmergencyRepair
    {
        public async static Task<bool> Execute()
        {
            var equippedItems = ZetaDia.Me.Inventory.Equipped.Where(i => i.IsValid);
            bool needEmergencyRepair = false;

            foreach (var item in equippedItems)
            {
                if (item.ACDId == 0) continue;
                float durabilitySum = 0f;
                int itemCount = 0;
                try
                {
                    durabilitySum += item.DurabilityPercent;
                    itemCount++;
                }
                catch
                {
                    // Ring them bells for the chosen few who will judge the many when the game is through
                }
                if (itemCount > 0 && durabilitySum/itemCount < 0.05)
                    needEmergencyRepair = true;
            }

            // We keep dying because we're spawning in AoE and next to 50 elites and we need to just leave the game
            if (DateTime.UtcNow.Subtract(DeathHandler.LastDeathTime).TotalSeconds < 30 && !ZetaDia.IsInTown && needEmergencyRepair && !Core.Settings.Advanced.UseTrinityDeathHandler)
            {
                Logger.Log("Durability is zero, emergency leave game");
                ZetaDia.Service.Party.LeaveGame(true);
                await CommonCoroutines.LeaveGame("Durability is zero");
                Logger.LogDebug(LogCategory.GlobalHandler, "Recently died, durability zero");
                return true;
            }

            return needEmergencyRepair;
        }
    }
}