using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Coroutines.Town;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.DbProvider
{
    public static class VendorHook
    {
        /// <summary>
        /// Injected to TownRun Composite at Step3 (just after identifying legendaries).
        /// </summary>
        public async static Task<bool> ExecutePreVendor()
        {
            if (!ZetaDia.IsInTown)
                return false;

            //Logger.LogVerbose("PreVendor Hook Started");            

            try
            {
                bool? result = null;

                // Can't manage inventory while participating in a greater rift.
                if (TrinityPlugin.Player.IsInventoryLockedForGreaterRift || !TrinityPlugin.Settings.Loot.TownRun.KeepLegendaryUnid && TrinityPlugin.Player.ParticipatingInTieredLootRun)
                    result = false;

                // Learn some recipies.
                if (!result.HasValue && !await UseCraftingRecipes.Execute())
                    result = true;

                // Destroy white/blue/yellow items to convert crafting materials.
                if (!result.HasValue && !await CubeItemsToMaterials.Execute())
                    result = true;

                // Gamble first for cube legendary rares, bag space permitting.
                if (!result.HasValue && !await Gamble.Execute())
                    result = true;

                // Run this before vendoring to use the rares we picked up.
                if (!result.HasValue && !await CubeRaresToLegendary.Execute())
                    result = true;

                if(!result.HasValue)
                    await ExtractLegendaryPowers.Execute();

                if (result.HasValue)
                {
                    //Logger.LogVerbose(LogCategory.Behavior, "PreVendor Hook Result = {0}", result);
                    //ItemManager.Current.Refresh(); // Massive performance issue.
                    return result.Value;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in VendorHook {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;
            }

            return false;
        }

        /// <summary>
        /// Injected to TownRun Composite after Stash/Sell/Salvage
        /// </summary>
        public async static Task<bool> ExecutePostVendor()
        {
            if (!ZetaDia.IsInTown)
                return false;

            //Logger.LogVerbose("PostVendor Hook Started");

            try
            {
                bool? result = null;

                // Can't manage inventory while participating in a greater rift.
                if (TrinityPlugin.Player.IsInventoryLockedForGreaterRift || !TrinityPlugin.Settings.Loot.TownRun.KeepLegendaryUnid && TrinityPlugin.Player.ParticipatingInTieredLootRun)
                    result = false;

                // Run again in case we missed first time due to full bags.
                if (!result.HasValue && !await Gamble.Execute())
                    result = true;

                // Destroy white/blue/yellow items to convert crafting materials.
                if (!result.HasValue && !await CubeItemsToMaterials.Execute())
                    result = true;

                // Run again in case we just gambled
                if (!result.HasValue && !await CubeRaresToLegendary.Execute())
                    result = true;

                if (result.HasValue)
                {
                    //Logger.LogVerbose(LogCategory.Behavior, "PostVendor Hook Result = {0}", result);
                    //ItemManager.Current.Refresh(); // Massive performance issue.
                    return result.Value;
                }
                    
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in VendorHook {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;
            }

            return false;
        }
    }
}
