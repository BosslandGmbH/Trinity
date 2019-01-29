using Serilog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Coroutines.Town;
using Trinity.Components.QuestTools;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Reference;
using Trinity.ProfileTags.EmbedTags;
using Trinity.Settings;
using Zeta.Bot.Coroutines;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    // TODO: Fix that stuff...
    [XmlElement("Transmute")]
    public class TransmuteTag : BaseProfileBehavior
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();
        [XmlElement("Items")]
        [Description("Items to be transmuted")]
        public List<ItemTag> Items { get; set; }

        /*
        public enum TransmuteRecipe
        {
            ConvertCraftingMaterialsFromRare = -1849120465,
            AugmentAncientItem = -1846982033,
            ConvertGems = -1112415923,
            RemoveLevelRequirement = -1020554217,
            ConvertCraftingMaterialsFromMagic = -897374554,
            ExtractLegendaryPower = -114611389,
            OpenPortalToCow = -1,
            OpenPortalToGreed = -1,
            ConvertCraftingMaterialsFromNormal = 507548782,
            ConvertSetItem = 955478940,
            ReforgeLegendary = 1697574309,
            UpgradeRareItem = 1946622401,
        }     
        */

        [XmlAttribute("recipe")]
        [Description("Cube recipe to use")]
        public TransmuteRecipe Recipe { get; set; }

        public override async Task<bool> MainTask()
        {
            if (Items == null || !Items.Any())
            {
                s_logger.Error($"[{nameof(MainTask)}] No items were specified. Use: <Transmute recipe=\"UpgradeRareItem\"><Items><Item id=\"0\" quantity =\"0\" /></Items></Transmute>");
                return true;
            }

            if (!GameUI.KanaisCubeWindow.IsVisible)
            {
                s_logger.Error($"[{nameof(MainTask)}] Kanai's Cube window must be visible");
                return true;
            }

            if (Recipe == 0)
            {
                s_logger.Error($"[{nameof(MainTask)}] You must specifiy a recipe to use: <Transmute recipe=\"UpgradeRareItem\"... valid values are: ConvertCraftingMaterialsFromRare, AugmentAncientItem, ConvertGems, RemoveLevelRequirement, ConvertCraftingMaterialsFromMagic, ExtractLegendaryPower, OpenPortalToCow, OpenPortalToGreed, ConvertCraftingMaterialsFromNormal, ConvertSetItem, ReforgeLegendary, UpgradeRareItem");
                return true;
            }

            var transmuteGroup = new List<ACDItem>();
            foreach (var item in Items)
            {
                var backpackItems = InventoryManager.Backpack.ByActorSno(item.Id);
                var acdItems = backpackItems as ACDItem[] ?? backpackItems.ToArray();
                if (acdItems.Length == 0)
                {
                    if (item.Quality != TrinityItemQuality.Invalid)
                    {
                        backpackItems = InventoryManager.Backpack.ByQuality(item.Quality);
                        acdItems = backpackItems as ACDItem[] ?? backpackItems.ToArray();
                    }

                    if (acdItems.Length == 0)
                    {
                        s_logger.Error($"[{nameof(MainTask)}] {item} was not found in backpack");
                        return true;
                    }
                }
                
                var stacks = acdItems.GetStacksUpToQuantity(item.Quantity);
                transmuteGroup.AddRange(stacks);
            }

            return await TrinityTownRun.TransmuteRecipe(Recipe, transmuteGroup.ToArray()) == CoroutineResult.Done;
        }
    }
}
