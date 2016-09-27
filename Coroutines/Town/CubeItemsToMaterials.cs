using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Coroutines.Resources;
using Trinity.Framework;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Coroutines.Town
{
    /// <summary>
    /// Convert common/rare/magic into crafting materials with Kanai's cube
    /// </summary>
    public class CubeItemsToMaterials
    {
        private static IEnumerable<KeyValuePair<InventoryItemType, Inventory.MaterialRecord>> _materials;
        private static Inventory.MaterialRecord _highest;

        public static bool HasUnlockedCube { get; set; }
        public static DateTime LastCanRunCheck = DateTime.MinValue;
        public static bool LastCanRunResult;

        public static bool CanRun()
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return false;

            if (!LastCanRunResult && DateTime.UtcNow.Subtract(LastCanRunCheck).TotalSeconds < 5)
                return LastCanRunResult;

            var kule = TownInfo.ZultonKule?.GetActor() as DiaUnit;
            if (kule != null)
            {
                if (kule.IsQuestGiver)
                {
                    Logger.LogVerbose("[CubeRaresToLegendary] Cube is not unlocked yet");
                    return false;
                }
            }

            Inventory.Materials.Update();

            _highest = Inventory.Materials.HighestCountMaterial(Inventory.MaterialConversionTypes);

            var settingsTypes = Core.Settings.KanaisCube.GetCraftingMaterialTypes();
            if (!settingsTypes.Any())
            {
                Logger.LogVerbose("[CubeItemsToMaterials] No materials have been selected in settings", _highest.Type, _highest.TotalStackQuantity);
            }        

            Logger.LogVerbose("[CubeItemsToMaterials] Selected {0} as the material with highest count - {1}", _highest.Type, _highest.TotalStackQuantity);

            var _validTypes = settingsTypes.Where( t => t != _highest.Type);
            _materials = Inventory.Materials.OfTypes(_validTypes);

            bool result = false;
            foreach (var material in _materials)
            {
                if (ConvertMaterials.CanRun(_highest.Type, material.Key, true, true))
                {
                    Logger.LogVerbose("[CubeItemsToMaterials] YES - {0} -> {1}", _highest.Type, material.Key);
                    result = true;
                }
                else
                {
                    Logger.LogVerbose("[CubeItemsToMaterials] NO - {0} -> {1}", _highest.Type, material.Key);
                }                    
            }

            LastCanRunCheck = DateTime.UtcNow;
            LastCanRunResult = result;
            return result;
        }

        public static async Task<bool> Execute(List<ItemSelectionType> types = null)
        {
            if (!CanRun())
                return true;

            Logger.LogVerbose("[CubeItemsToMaterials] Getting Materials from Stash");

            if (!Inventory.Materials.HasStackQuantityOfTypes(Inventory.MaterialConversionTypes, InventorySlot.BackpackItems, 100) && !await TakeItemsFromStash.Execute(Inventory.RareUpgradeIds, 5000))
                return true;
                
            Logger.LogVerbose("[CubeItemsToMaterials] Time to Convert some junk into delicious crafting materials.");

            if (!await MoveToAndInteract.Execute(TownInfo.KanaisCube))
            {
                Logger.Log("[CubeItemsToMaterials] Failed to move to the cube, quite unfortunate.");
                return true;
            }

            if (_highest.Type == InventoryItemType.None)
            {
                Logger.Log("[CubeItemsToMaterials] Error: Highest material count is unknown.");
                return true;
            }

            foreach (var material in _materials)
            {
                if (!await ConvertMaterials.Execute(_highest.Type, material.Key))
                {
                    Logger.Log("[Cube] Failed! Finished!");
                    return true;
                }

                await Coroutine.Sleep(100);
                await Coroutine.Yield();
            }
          
            Logger.LogVerbose("[Cube] CubeItemsToMaterials Finished!");
            return true;
        }

    }
}
