using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    /// <summary>
    /// Convert common/rare/magic into crafting materials with Kanai's cube
    /// </summary>
    public class CubeItemsToMaterials
    {
        public static bool HasUnlockedCube { get; set; }
        public static DateTime LastCanRunCheck = DateTime.MinValue;
        public static bool LastCanRunResult;

        public static bool CanRun()
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return false;

            if (!LastCanRunResult && DateTime.UtcNow.Subtract(LastCanRunCheck).TotalSeconds < 5)
                return LastCanRunResult;

            var kule = TownInfo.ZoltunKulle?.GetActor() as DiaUnit;
            if (kule != null)
            {
                if (kule.IsQuestGiver)
                {
                    Core.Logger.Verbose("[CubeRaresToLegendary] Cube is not unlocked yet");
                    return false;
                }
            }

            var settingsTypes = Core.Settings.KanaisCube.GetCraftingMaterialTypes();
            if (!settingsTypes.Any())
            {
                Core.Logger.Verbose("[CubeItemsToMaterials] No materials have been selected in settings");
                return false;
            }

            // * Never create more of the material you have most of.
            // * Always use the material you have most of to create others.
            // * Only allow creation of a material if it has been selected in settings.

            var allTypes = GetAllConversionTypes();
            var orderedTypes = OrderByAmount(allTypes);
            var highestCountType = orderedTypes.First();
            var allowedTypes = settingsTypes.Select(GetCurrencyType);
            var otherTypes = orderedTypes.Skip(1).Where(t => allowedTypes.Contains(t)).ToList();
            var result = (int)highestCountType != -1 && otherTypes.Any(t => ConvertMaterials.CanRun(highestCountType, t, true));

            if (result)
            {
                Core.Logger.Verbose($"[CubeItemsToMaterials] Selected {highestCountType} as the material with highest count - {Core.Inventory.Currency.GetCurrency(highestCountType)}");
            }

            LastCanRunCheck = DateTime.UtcNow;
            LastCanRunResult = result;
            return result;
        }

        public static async Task<bool> Execute(List<ItemSelectionType> types = null)
        {
            if (!CanRun())
                return true;

            Core.Logger.Verbose("[CubeItemsToMaterials] Time to Convert some junk into delicious crafting materials.");

            if (!await MoveToAndInteract.Execute(TownInfo.KanaisCube))
            {
                Core.Logger.Log("[CubeItemsToMaterials] Failed to move to the cube, quite unfortunate.");
                return true;
            }

            // * Never create more of the material you have most of.
            // * Always use the material you have most of to create others.
            // * Only allow creation of a material if it has been selected in settings.

            var settingsTypes = Core.Settings.KanaisCube.GetCraftingMaterialTypes();
            var allTypes = GetAllConversionTypes();
            var orderedTypes = OrderByAmount(allTypes).ToArray();

            var highestCount = orderedTypes.First();
            var allowedTypes = settingsTypes.Select(GetCurrencyType);
            var otherTypes = orderedTypes.Skip(1).Where(t => allowedTypes.Contains(t)).ToList();

            foreach (var toType in otherTypes)
            {
                if (!await ConvertMaterials.Execute(highestCount, toType))
                {
                    Core.Logger.Log("[Cube] Failed! Finished!");
                    return true;
                }
                await Coroutine.Yield();
            }

            Core.Logger.Verbose("[Cube] CubeItemsToMaterials Finished!");
            return true;
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
            }
            return (CurrencyType)(-1);
        }
    }
}
