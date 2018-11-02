using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Settings;
using Zeta.Bot.Coroutines;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    /// <summary>
    /// Convert common/rare/magic into crafting materials with Kanai's cube
    /// </summary>
    public static partial class TrinityTownRun
    {
        public static HashSet<CurrencyType> CurrencyConversionTypes { get; } = new HashSet<CurrencyType>
        {
            CurrencyType.ArcaneDust,
            CurrencyType.VeiledCrystal,
            CurrencyType.ReusableParts
        };

        // TODO: Move logging somewhere else.
        public static bool IsItemsToMaterialTransformationPossible
        {
            get
            {
                if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                    return false;

                if (TownInfo.ZoltunKulle?.GetActor() is DiaUnit kule && kule.IsQuestGiver)
                {
                    s_logger.Verbose($"[{nameof(IsItemsToMaterialTransformationPossible)}] Cube is not unlocked yet");
                    return false;
                }

                var settingsTypes = Core.Settings.KanaisCube.GetCraftingMaterialTypes();
                if (!settingsTypes.Any())
                {
                    s_logger.Verbose(
                        $"[{nameof(IsItemsToMaterialTransformationPossible)}] No materials have been selected in settings");
                    return false;
                }

                // * Never create more of the material you have most of.
                // * Always use the material you have most of to create others.
                // * Only allow creation of a material if it has been selected in settings.

                var allTypes = GetAllConversionTypes();
                var orderedTypes = OrderByAmount(allTypes);
                if (orderedTypes == null) return false;

                var highestCountType = orderedTypes.First();
                var allowedTypes = settingsTypes.Select(GetCurrencyType);
                var otherTypes = orderedTypes.Skip(1).Where(t => allowedTypes.Contains(t)).ToList();
                var result = (int)highestCountType != -1 &&
                             otherTypes.Any(t => IsMaterialTransmutationPossible(highestCountType, t, true));

                if (result)
                {
                    s_logger.Verbose(
                        $"[{nameof(IsItemsToMaterialTransformationPossible)}] Selected {highestCountType} as the material with highest count - {Core.Inventory.Currency.GetCurrency(highestCountType)}");
                }

                return result;
            }
        }

        public static bool IsMaterialTransmutationPossible(CurrencyType from, CurrencyType to, bool excludeLegendaryUpgradeRares = false)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return false;

            if (!GetSacraficialItems(to, excludeLegendaryUpgradeRares).Any())
            {
                s_logger.Warn($"[{nameof(IsMaterialTransmutationPossible)}] You don't have enough valid weapon/armor/jewellery in backpack to convert '{from} ({GetCurrency(from)})' to '{to}'.");
                return false;
            }

            if (!HasCurrency(from))
            {
                s_logger.Warn($"[{nameof(IsMaterialTransmutationPossible)}] You don't have enough backpack materials to convert '{from} ({GetCurrency(from)})' to '{to}', Deaths={Core.Inventory.Currency.DeathsBreath}");
                return false;
            }

            s_logger.Info($"[{nameof(IsMaterialTransmutationPossible)}] You have enough Materials to convert '{from} ({GetCurrency(from)})' to '{to}', Deaths={Core.Inventory.Currency.DeathsBreath}");
            return true;
        }

        public static async Task<CoroutineResult> TransmuteMaterials()
        {
            return await TransmuteMaterials(null);
        }

        public static async Task<CoroutineResult> TransmuteMaterials(List<ItemSelectionType> types)
        {
            if (!IsItemsToMaterialTransformationPossible)
                return CoroutineResult.NoAction;

            // * Never create more of the material you have most of.
            // * Always use the material you have most of to create others.
            // * Only allow creation of a material if it has been selected in settings.

            var settingsTypes = Core.Settings.KanaisCube.GetCraftingMaterialTypes();
            var allTypes = GetAllConversionTypes();
            var orderedTypes = OrderByAmount(allTypes).ToArray();

            var highestCount = orderedTypes.First();
            var allowedTypes = settingsTypes.Select(GetCurrencyType);
            var otherType = orderedTypes.Skip(1).FirstOrDefault(t => allowedTypes.Contains(t));

            if (await TransmuteMaterials(highestCount, otherType) == CoroutineResult.Running)
                return CoroutineResult.Running;

            s_logger.Verbose($"[{nameof(TransmuteMaterials)}] CubeItemsToMaterials Finished!");
            return CoroutineResult.Done;
        }

        /// <summary>
        /// Converts crafting materials into other types of crafting materials
        /// </summary>
        /// <param name="to">the type of material you will get more of</param>
        public static async Task<CoroutineResult> TransmuteMaterials(CurrencyType to)
        {
            foreach (var currency in GetOtherCurrency(to).OrderByDescending(c => Core.Inventory.Currency.GetCurrency(c)))
            {
                if (await TransmuteMaterials(currency, to) == CoroutineResult.Running)
                    return CoroutineResult.Running;
            }
            return CoroutineResult.Done;
        }

        public static async Task<CoroutineResult> TransmuteMaterials(CurrencyType from, CurrencyType to)
        {
            if (!ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            if (!CurrencyConversionTypes.Contains(to) || !CurrencyConversionTypes.Contains(from))
            {
                Core.Logger.Log($"[{nameof(TransmuteMaterials)}] Unable to convert from '{from}' to '{to}'");
                return CoroutineResult.NoAction;
            }

            if (!IsMaterialTransmutationPossible(from, to))
                return CoroutineResult.NoAction;

            var item = GetSacraficialItems(to).First();
            var recipe = GetRecipeFromCurrency(from);

            if (await TransmuteRecipe(recipe, item) == CoroutineResult.Running)
                return CoroutineResult.Running;

            Core.Logger.Log($"[{nameof(TransmuteMaterials)}] Converted from '{from}' to '{to}'");
            return CoroutineResult.Done;
        }

        public static bool HasCurrency(CurrencyType from)
        {
            var recipe = GetRecipeFromCurrency(from);
            return (int)recipe != -1 && Core.Inventory.Currency.HasCurrency(recipe);
        }

        public static IOrderedEnumerable<CurrencyType> OrderByAmount(IEnumerable<InventoryItemType> settingsTypes)
        {
            return settingsTypes.Select(GetCurrencyType).OrderByDescending(t => Core.Inventory.Currency.GetCurrency(t));
        }

        public static IOrderedEnumerable<CurrencyType> OrderByAmount(IEnumerable<CurrencyType> currencyTypes)
        {
            return currencyTypes.OrderByDescending(t => Core.Inventory.Currency.GetCurrency(t));
        }

        public static List<CurrencyType> GetAllConversionTypes()
        {
            return new List<CurrencyType>
            {
                CurrencyType.ArcaneDust,
                CurrencyType.VeiledCrystal,
                CurrencyType.ReusableParts,
            };
        }

        public static CurrencyType GetCurrencyType(InventoryItemType type)
        {
            switch (type)
            {
                case InventoryItemType.ArcaneDust:
                    return CurrencyType.ArcaneDust;
                case InventoryItemType.VeiledCrystal:
                    return CurrencyType.VeiledCrystal;
                case InventoryItemType.ReusableParts:
                    return CurrencyType.ReusableParts;
                default:
                    return (CurrencyType)(-1);
            }
        }

        public static TransmuteRecipe GetRecipeFromCurrency(CurrencyType from)
        {
            switch (from)
            {
                case CurrencyType.ArcaneDust:
                    return Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromMagic;
                case CurrencyType.VeiledCrystal:
                    return Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromRare;
                case CurrencyType.ReusableParts:
                    return Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromNormal;
                default:
                    return (TransmuteRecipe)(-1);
            }
        }

        public static TransmuteRecipe GetRecipeToCurrency(CurrencyType to, TrinityItem item)
        {
            var quality = item.TrinityItemQuality;
            switch (to)
            {
                case CurrencyType.ArcaneDust:
                    return quality == TrinityItemQuality.Rare
                        ? Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromRare
                        : Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromNormal;

                case CurrencyType.VeiledCrystal:
                    return quality == TrinityItemQuality.Magic
                        ? Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromMagic
                        : Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromNormal;

                case CurrencyType.ReusableParts:
                    return quality == TrinityItemQuality.Rare
                        ? Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromRare
                        : Zeta.Game.TransmuteRecipe.ConvertCraftingMaterialsFromMagic;
                default:
                    return (TransmuteRecipe)(-1);
            }
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
                default:
                    return 0;
            }
        }

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
                var upgradeRares = GetBackPackRares();
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
