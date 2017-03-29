using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Coroutines.Town
{
    /// <summary>
    /// Convert rares into legendaries with Kanai's cube
    /// </summary>
    public class CubeRaresToLegendary
    {
        public static bool HasUnlockedCube = true;

        public static bool CanRun(List<ItemSelectionType> types = null)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return false;

            var kule = TownInfo.ZultonKule?.GetActor() as DiaUnit;
            if (kule != null)
            {
                if (kule.IsQuestGiver)
                {
                    Core.Logger.Verbose("[CubeRaresToLegendary] Cube is not unlocked yet");
                    HasUnlockedCube = false;
                    return false;
                }
                HasUnlockedCube = true;
            }

            if (!HasUnlockedCube)
                return false;

            if (types == null && Core.Settings.KanaisCube.RareUpgradeTypes == ItemSelectionType.Unknown)
            {
                Core.Logger.Verbose("[CubeRaresToLegendary] No item types selected in settings - (Config => Items => Kanai's Cube)");
                return false;
            }

            if (!HasMaterialsRequired && InventoryManager.NumFreeBackpackSlots < 5)
            {
                Core.Logger.Verbose("[CubeRaresToLegendary] Not enough bag space");
                return false;
            }

            var dbs = Core.Inventory.Currency.DeathsBreath;
            if (dbs < Core.Settings.KanaisCube.DeathsBreathMinimum)
            {
                Core.Logger.Verbose("[CubeRaresToLegendary] Not enough deaths breath - Limit is set to {0}, You currently have {1}", Core.Settings.KanaisCube.DeathsBreathMinimum, dbs);
                return false;
            }

            if (!GetBackPackRares(types).Any())
            {
                Core.Logger.Verbose("[CubeRaresToLegendary] You need some rares in your backpack for this to work!");
                return false;
            }

            if (!HasMaterialsRequired)
            {
                Core.Logger.Verbose("[CubeRaresToLegendary] Unable to find the materials we need, maybe you don't have them!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// A list of conversion candidates from backpack
        /// </summary>
        public static List<TrinityItem> GetBackPackRares(IEnumerable<ItemSelectionType> types = null)
        {
            if (types == null)
                types = Core.Settings.KanaisCube.GetRareUpgradeSettings();

            if (!Core.Inventory.Backpack.Any())
            {
                Core.Logger.Verbose("[CubeRaresToLegendary] No items were found in backpack!");
            }

            var rares = Core.Inventory.Backpack.Where(i =>
            {
                if (Core.Inventory.InvalidAnnIds.Contains(i.AnnId))
                    return false;

                if (i.ItemBaseType != ItemBaseType.Armor && i.ItemBaseType != ItemBaseType.Weapon && i.ItemBaseType != ItemBaseType.Jewelry)
                    return false;

                if (!RareQualities.Contains(i.ItemQualityLevel) || i.ItemQualityLevel == ItemQuality.Legendary)
                    return false;

                return types == null || types.Contains(GetItemSelectionType(i));

            }).ToList();

            Core.Logger.Log(LogCategory.Behavior, "[CubeRaresToLegendary] {0} Valid Rares in Backpack", rares.Count);
            return rares;
        }

        public static ItemSelectionType GetItemSelectionType(TrinityItem item)
        {
            ItemSelectionType result;
            return Enum.TryParse(item.TrinityItemType.ToString(), out result) ? result : ItemSelectionType.Unknown;
        }

        public static HashSet<ItemQuality> RareQualities = new HashSet<ItemQuality>
        {
            ItemQuality.Rare4,
            ItemQuality.Rare5,
            ItemQuality.Rare6,
        };

        public static bool HasMaterialsRequired 
            => Core.Inventory.Currency.HasCurrency(TransmuteRecipe.UpgradeRareItem);

        /// <summary>
        /// Convert rares into legendaries with Kanai's cube
        /// </summary>
        /// <param name="types">restrict the rares that can be selected by ItemType</param>
        public static async Task<bool> Execute(List<ItemSelectionType> types = null)
        {
            while (CanRun(types))
            {
                if (!ZetaDia.IsInTown)
                    break;

                //Core.Logger.Log("[CubeRaresToLegendary] CubeRaresToLegendary Started! Wooo!");

                var backpackGuids = new HashSet<int>(InventoryManager.Backpack.Select(i => i.ACDId));

                if (HasMaterialsRequired)
                {
                    if (TownInfo.KanaisCube.Distance > 10f || !GameUI.KanaisCubeWindow.IsVisible)
                    {
                        if (!await MoveToAndInteract.Execute(TownInfo.KanaisCube))
                        {
                            Core.Logger.Log("Failed to move to the cube, quite unfortunate.");
                            break;
                        }
                        continue;
                    }

                    //Core.Logger.Log("[CubeRaresToLegendary] Ready to go, Lets transmute!");

                    var item = GetBackPackRares(types).First();
                    var itemName = item.Name;
                    var itemAnnId = item.AnnId;
                    var itemInternalName = item.InternalName;
                    await Transmute.Execute(item, TransmuteRecipe.UpgradeRareItem);
                    await Coroutine.Sleep(1500);

                    var newItem = InventoryManager.Backpack.FirstOrDefault(i => !backpackGuids.Contains(i.ACDId));
                    if (newItem != null)
                    {
                        var newLegendaryItem = Legendary.GetItemByACD(newItem);
                        var newTrinityItem = Core.Actors.ItemByAnnId(newItem.AnnId);
                        ItemEvents.FireItemCubed(newTrinityItem);

                        if(newTrinityItem.IsPrimalAncient)
                            Core.Logger.Warn($"[CubeRaresToLegendary] Upgraded Rare '{itemName}' ---> '{newLegendaryItem.Name}' ({newItem.ActorSnoId}) PRIMAL!~");
                        else
                            Core.Logger.Log($"[CubeRaresToLegendary] Upgraded Rare '{itemName}' ---> '{newLegendaryItem.Name}' ({newItem.ActorSnoId})");

                      
                    }
                    else
                    {
                        Core.Logger.Log("[CubeRaresToLegendary] Failed to upgrade Item '{0}' {1} DynId={2} HasBackpackMaterials={3}",
                            itemName, itemInternalName, itemAnnId, HasMaterialsRequired);
                    }

                    Core.Inventory.InvalidAnnIds.Add(itemAnnId);
                }
                else
                {
                    Core.Logger.Log("[CubeRaresToLegendary] Oh no! Out of materials!");
                    return true;
                }

                await Coroutine.Sleep(500);
                await Coroutine.Yield();
            }

            return true;
        }
    }
}