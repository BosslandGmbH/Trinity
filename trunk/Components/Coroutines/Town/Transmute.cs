using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.Coroutines.Town
{
    public static class Transmute
    {
        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<bool> Execute(TrinityItem item, TransmuteRecipe recipe)
        {
            return await Execute(new List<TrinityItem> { item }, recipe);
        }

        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<bool> Execute(List<TrinityItem> transmuteGroup, TransmuteRecipe recipe)
        {
            return await Execute(transmuteGroup.Select(i => i.AnnId).ToList(), recipe);
        }

        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<bool> Execute(IEnumerable<int> transmuteGroupAnnIds, TransmuteRecipe recipe)
        {
            if (!ZetaDia.IsInGame)
                return false;

            Core.Logger.Log("Transmuting...");

            if (!Core.Inventory.Currency.HasCurrency(recipe))
            {
                Core.Logger.Error($"--> Not enough currency for {recipe}");
                return false;
            }

            if (!UIElements.TransmuteItemsDialog.IsVisible)
            {
                await MoveToAndInteract.Execute(TownInfo.KanaisCube);
                await Coroutine.Sleep(1000);
            }

            if (!UIElements.TransmuteItemsDialog.IsVisible)
            {
                Core.Logger.Log(" --> Can't transmute without the vendor window open!");
                return false;
            }

            Core.Logger.Log("Zip Zap!");
            InventoryManager.TransmuteItems(transmuteGroupAnnIds.ToArray(), recipe);
            await Coroutine.Sleep(Randomizer.Fudge(500));
            UIElement.FromHash(TransmuteButtonHash)?.Click();
            return true;
        }

        private const long TransmuteButtonHash = 0x7BD4F1CE7188C0D7;

    }
}
 