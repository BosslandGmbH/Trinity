using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    /// <summary>
    /// Converts crafting materials into other types of crafting materials
    /// </summary>
    public class ConvertMaterials
    {
        public static bool CanRun(CurrencyType from, CurrencyType to, bool excludeLegendaryUpgradeRares = false)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return false;

            if (!GetSacraficialItems(to, excludeLegendaryUpgradeRares).Any())
            {
                Core.Logger.Verbose(LogCategory.Behavior, "[ConvertMaterials] You don't have enough valid weapon/armor/jewellery in backpack", from, to);
                return false;
            }

            if (HasCurrency(from))
            {
                Core.Logger.Verbose(LogCategory.Behavior, $"[ConvertMaterials] We have enough Materials to convert from {from} ({GetCurrency(from)}) to {to}, Deaths={Core.Inventory.Currency.DeathsBreath}");
                return true;
            }

            Core.Logger.Verbose(LogCategory.Behavior, $"[ConvertMaterials] Not Enough Backpack materials to convert from {from} ({GetCurrency(from)}) to {to}, Deaths={Core.Inventory.Currency.DeathsBreath}");
            return false;
        }

        /// <summary>
        /// Converts crafting materials into other types of crafting materials
        /// </summary>
        /// <param name="to">the type of material you will get more of</param>
        public static async Task<bool> Execute(CurrencyType to)
        {
            if (!UIElements.TransmuteItemsDialog.IsVisible)
            {
                Core.Logger.Log("Transmute dialog must be open");
                return false;
            }

            foreach (var currency in GetOtherCurrency(to).OrderByDescending(c => Core.Inventory.Currency.GetCurrency(c)))
            {
                if (CanRun(currency, to))
                {
                    await Execute(currency, to);
                }
            }
            return true;
        }

        /// <summary>
        /// Converts crafting materials into other types of crafting materials
        /// </summary>
        /// <param name="from">the type of material you will consume</param>
        /// <param name="to">the type of material you will get more of</param>
        public static async Task<bool> Execute(CurrencyType from, CurrencyType to)
        {
            Core.Logger.Log("[ConvertMaterials] Wooo! Lets convert some {0} to {1}", from, to);

            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return false;

            if (!CurrencyConversionTypes.Contains(to) || !CurrencyConversionTypes.Contains(from))
            {
                Core.Logger.Log("[Cube] Unable to convert from {0} to {1}", from, to);
                return false;
            }

            var fromAmount = GetCurrency(from);
            var toAmount = GetCurrency(to);
            var sacraficialItems = GetSacraficialItems(to);

            Core.Logger.Verbose($"[ConvertMaterials] Starting Material Counts DeathsBreath={Core.Inventory.Currency.DeathsBreath} {from}={fromAmount} {to}={toAmount} SacraficialItems={sacraficialItems.Count}");

            while (CanRun(from, to))
            {
                var item = GetSacraficialItems(to).First();
                var recipe = GetRecipeFromCurrency(from);

                await Transmute.Execute(item, recipe);
                await Coroutine.Yield();
                Core.Update();

                var newToAmount = GetCurrency(to);
                if (newToAmount > toAmount)
                {
                    Core.Logger.Log("[ConvertMaterials] Converted materials '{0}' ---> '{1}'", from, to);
                    toAmount = newToAmount;
                    fromAmount = GetCurrency(from);
                    ConsecutiveFailures = 0;
                    Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                }
                else
                {
                    ConsecutiveFailures++;
                    if (ConsecutiveFailures > 3)
                    {
                        Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                    }
                    Core.Logger.Error("[ConvertMaterials] Failed to convert materials");
                    return false;
                }

                await Coroutine.Yield();
            }

            Core.Logger.Verbose($"[ConvertMaterials] Finishing Material Counts DeathsBreath={Core.Inventory.Currency.DeathsBreath} {from}={fromAmount} {to}={toAmount} SacraficialItems={sacraficialItems.Count}");

            return true;
        }

        public static int ConsecutiveFailures { get; set; }

        public static bool HasCurrency(CurrencyType from)
        {
            var recipe = GetRecipeFromCurrency(from);
            return (int)recipe != -1 && Core.Inventory.Currency.HasCurrency(recipe);
        }

        public static TransmuteRecipe GetRecipeFromCurrency(CurrencyType from)
        {
            switch (from)
            {
                case CurrencyType.ArcaneDust:
                    return TransmuteRecipe.ConvertCraftingMaterialsFromMagic;
                case CurrencyType.VeiledCrystal:
                    return TransmuteRecipe.ConvertCraftingMaterialsFromRare;
                case CurrencyType.ReusableParts:
                    return TransmuteRecipe.ConvertCraftingMaterialsFromNormal;
            }
            return (TransmuteRecipe)(-1);
        }

        public static TransmuteRecipe GetRecipeToCurrency(CurrencyType to, TrinityItem item)
        {
            var quality = item.TrinityItemQuality;
            switch (to)
            {
                case CurrencyType.ArcaneDust:
                    return quality == TrinityItemQuality.Rare
                        ? TransmuteRecipe.ConvertCraftingMaterialsFromRare
                        : TransmuteRecipe.ConvertCraftingMaterialsFromNormal;

                case CurrencyType.VeiledCrystal:
                    return quality == TrinityItemQuality.Magic
                        ? TransmuteRecipe.ConvertCraftingMaterialsFromMagic
                        : TransmuteRecipe.ConvertCraftingMaterialsFromNormal;

                case CurrencyType.ReusableParts:
                    return quality == TrinityItemQuality.Rare
                        ? TransmuteRecipe.ConvertCraftingMaterialsFromRare
                        : TransmuteRecipe.ConvertCraftingMaterialsFromMagic;
            }
            return (TransmuteRecipe)(-1);
        }

        public static IEnumerable<CurrencyType> GetOtherCurrency(CurrencyType to)
        {
            switch (to)
            {
                case CurrencyType.ArcaneDust:
                    yield return CurrencyType.ReusableParts;
                    yield return CurrencyType.VeiledCrystal;
                    break;
                case CurrencyType.ReusableParts:
                    yield return CurrencyType.VeiledCrystal;
                    yield return CurrencyType.ArcaneDust;
                    break;
                case CurrencyType.VeiledCrystal:
                    yield return CurrencyType.ReusableParts;
                    yield return CurrencyType.ArcaneDust;
                    break;
            }
        }

        public static long GetCurrency(CurrencyType from)
        {
            switch (from)
            {
                case CurrencyType.ArcaneDust:
                    return Core.Inventory.Currency.ArcaneDust;
                case CurrencyType.VeiledCrystal:
                    return Core.Inventory.Currency.VeiledCrystals;
                case CurrencyType.ReusableParts:
                    return Core.Inventory.Currency.ReusableParts;
            }
            return 0;
        }

        public static HashSet<CurrencyType> CurrencyConversionTypes = new HashSet<CurrencyType>
        {
            CurrencyType.ArcaneDust,
            CurrencyType.VeiledCrystal,
            CurrencyType.ReusableParts
        };

        public static List<TrinityItem> GetSacraficialItems(CurrencyType to, bool excludeLegendaryUpgradeRares = false)
        {
            List<TrinityItem> sacraficialItems = new List<TrinityItem>();

            switch (to)
            {
                case CurrencyType.ReusableParts:
                    sacraficialItems = GetBackpackItemsOfQuality(new List<ItemQuality>
                    {
                        ItemQuality.Inferior,
                        ItemQuality.Normal,
                        ItemQuality.Superior
                    });
                    break;

                case CurrencyType.ArcaneDust:
                    sacraficialItems = GetBackpackItemsOfQuality(new List<ItemQuality>
                    {
                        ItemQuality.Magic1,
                        ItemQuality.Magic2,
                        ItemQuality.Magic3
                    });
                    break;

                case CurrencyType.VeiledCrystal:
                    sacraficialItems = GetBackpackItemsOfQuality(new List<ItemQuality>
                    {
                        ItemQuality.Rare4,
                        ItemQuality.Rare5,
                        ItemQuality.Rare6
                    });
                    break;
            }

            if (excludeLegendaryUpgradeRares)
            {
                var upgradeRares = CubeRaresToLegendary.GetBackPackRares();
                sacraficialItems.RemoveAll(i => upgradeRares.Contains(i));
            }

            sacraficialItems.RemoveAll(i => Core.Inventory.InvalidAnnIds.Contains(i.AnnId));
            return sacraficialItems;
        }

        public static List<TrinityItem> GetBackpackItemsOfQuality(List<ItemQuality> qualities)
        {
            return Core.Inventory.Backpack.Where(i =>
            {
                if (!i.IsValid)
                    return false;

                if (!i.IsCraftingReagent && i.RequiredLevel < 70)
                    return false;

                if (i.ItemBaseType != ItemBaseType.Armor && i.ItemBaseType != ItemBaseType.Weapon && i.ItemBaseType != ItemBaseType.Jewelry)
                    return false;

                var stackQuantity = i.ItemStackQuantity;
                var isVendor = i.IsVendorBought;
                if (!isVendor && stackQuantity != 0 || isVendor && stackQuantity > 1)
                    return false;

                return qualities.Contains(i.ItemQualityLevel);
            }).ToList();
        }
    }
}
