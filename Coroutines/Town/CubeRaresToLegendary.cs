using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors;
using Trinity.Helpers;
using Trinity.Reference;
using Trinity.Technicals;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Coroutines.Town
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
                    Logger.LogVerbose("[CubeRaresToLegendary] Cube is not unlocked yet");
                    HasUnlockedCube = false;
                    return false;
                }
                HasUnlockedCube = true;
            }

            if (!HasUnlockedCube)
                return false;

            if (types == null && Trinity.Settings.KanaisCube.RareUpgradeTypes == ItemSelectionType.Unknown)
            {
                Logger.LogVerbose("[CubeRaresToLegendary] No item types selected in settings - (Config => Items => Kanai's Cube)");
                return false;
            }

            if (!BackpackHasMaterials && ZetaDia.Me.Inventory.NumFreeBackpackSlots < 5)
            {
                Logger.LogVerbose("[CubeRaresToLegendary] Not enough bag space");
                return false;
            }

            var dbs = Inventory.OfType(InventoryItemType.DeathsBreath).StackQuantity();
            if (dbs < Trinity.Settings.KanaisCube.DeathsBreathMinimum)
            {
                Logger.LogVerbose("[CubeRaresToLegendary] Not enough deaths breath - Limit is set to {0}, You currently have {1}", Trinity.Settings.KanaisCube.DeathsBreathMinimum, dbs);
                return false;
            }

            if (!GetBackPackRares(types).Any())
            {
                Logger.LogVerbose("[CubeRaresToLegendary] You need some rares in your backpack for this to work!");
                return false;
            }

            if (!BackpackHasMaterials && !StashHasMaterials)
            {
                Logger.LogVerbose("[CubeRaresToLegendary] Unable to find the materials we need, maybe you don't have them!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// A list of conversion candidates from backpack
        /// </summary>
        public static List<CachedItem> GetBackPackRares(IEnumerable<ItemSelectionType> types = null)
        {
            if (types == null)
                types = Trinity.Settings.KanaisCube.GetRareUpgradeSettings();

            var rares = Inventory.Backpack.Items.Where(i =>
            {
                if (Inventory.InvalidItemDynamicIds.Contains(i.AnnId))
                    return false;

                if (i.ItemBaseType != ItemBaseType.Armor && i.ItemBaseType != ItemBaseType.Weapon && i.ItemBaseType != ItemBaseType.Jewelry)
                    return false;

                if (!RareQualities.Contains(i.ItemQualityLevel) || i.ItemQualityLevel == ItemQuality.Legendary)
                    return false;

                //if (i.ItemStackQuantity != 0 || !i.IsValid || i.ItemLevel < 70)
                //{
                //    Logger.LogVerbose($"Skipping Item - Invalid {i.InternalName} IsValid={i.IsValid} InvalidStackQuantity={i.ItemStackQuantity != 0} Level={i.ItemLevel}");
                //    return false;
                //}

                return types == null || types.Contains(GetItemSelectionType(i));

            }).ToList();

            Logger.Log(LogCategory.Behavior, "[CubeRaresToLegendary] {0} Valid Rares in Backpack", rares.Count);
            return rares;
        }

        public static ItemSelectionType GetItemSelectionType(CachedItem item)
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

        /// <summary>
        /// If backpack has enough materials to convert a rare to a legendary
        /// </summary>
        public static bool BackpackHasMaterials
        {
            get
            {
                var dust = Inventory.Backpack.ArcaneDust.Select(i => i.ItemStackQuantity).Sum();
                var crystals = Inventory.Backpack.VeiledCrystals.Select(i => i.ItemStackQuantity).Sum();
                var deaths = Inventory.Backpack.DeathsBreath.Select(i => i.ItemStackQuantity).Sum();
                var parts = Inventory.Backpack.ReusableParts.Select(i => i.ItemStackQuantity).Sum();

                Logger.Log("[CubeRaresToLegendary] Backpack Crafting Materials: Dust={0} Crystals={1} Deaths={2} Parts={3}",
                    dust, crystals, deaths, parts);

                return dust >= 50 && crystals >= 50 && deaths >= 25 && parts >= 50;
            }
        }

        /// <summary>
        /// If stash has enough materials to convert a rare to a legendary
        /// </summary>
        public static bool StashHasMaterials
        {
            get
            {
                var dust = Inventory.Stash.ArcaneDust.Select(i => i.ItemStackQuantity).Sum();
                var crystals = Inventory.Stash.VeiledCrystals.Select(i => i.ItemStackQuantity).Sum();
                var deaths = Inventory.Stash.DeathsBreath.Select(i => i.ItemStackQuantity).Sum();
                var parts = Inventory.Stash.ReusableParts.Select(i => i.ItemStackQuantity).Sum();

                Logger.Log("[CubeRaresToLegendary] Stash Crafting Materials: Dust={0} Crystals={1} Deaths={2} Parts={3}",
                    dust, crystals, deaths, parts);

                return dust >= 50 && crystals >= 50 && deaths >= 25 && parts >= 50;
            }
        }

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

                Logger.Log("[CubeRaresToLegendary] CubeRaresToLegendary Started! Wooo!");

                var backpackGuids = new HashSet<int>(ZetaDia.Me.Inventory.Backpack.Select(i => i.ACDId));

                if (BackpackHasMaterials)
                {
                    if (TownInfo.KanaisCube.Distance > 10f || !GameUI.KanaisCubeWindow.IsVisible)
                    {
                        if (!await MoveToAndInteract.Execute(TownInfo.KanaisCube))
                        {
                            Logger.Log("Failed to move to the cube, quite unfortunate.");
                            break;
                        }
                        continue;
                    }

                    Logger.Log("[CubeRaresToLegendary] Ready to go, Lets transmute!");

                    var item = GetBackPackRares(types).First();
                    var itemName = item.Name;
                    var itemDynamicId = item.AnnId;
                    var itemInternalName = item.InternalName;
                    var transmuteGroup = new List<CachedItem>
                    {
                        item,
                    };

                    transmuteGroup.AddRange(Inventory.GetStacksUpToQuantity(Inventory.Backpack.ArcaneDust, 50));
                    transmuteGroup.AddRange(Inventory.GetStacksUpToQuantity(Inventory.Backpack.VeiledCrystals, 50));
                    transmuteGroup.AddRange(Inventory.GetStacksUpToQuantity(Inventory.Backpack.ReusableParts, 50));
                    transmuteGroup.AddRange(Inventory.GetStacksUpToQuantity(Inventory.Backpack.DeathsBreath, 25));

                    await Transmute.Execute(transmuteGroup);
                    await Coroutine.Sleep(1500);

                    var newItem = ZetaDia.Me.Inventory.Backpack.FirstOrDefault(i => !backpackGuids.Contains(i.ACDId));
                    if (newItem != null)
                    {
                        var newLegendaryItem = Legendary.GetItemByACD(newItem);

                        Logger.Log("[CubeRaresToLegendary] Upgraded Rare '{0}' ---> '{1}' ({2})",
                            itemName, newLegendaryItem.Name, newItem.ActorSnoId);
                    }
                    else
                    {
                        Logger.Log("[CubeRaresToLegendary] Failed to upgrade Item '{0}' {1} DynId={2} HasBackpackMaterials={3}",
                            itemName, itemInternalName, itemDynamicId, BackpackHasMaterials);
                    }

                    Inventory.InvalidItemDynamicIds.Add(itemDynamicId);
                }
                else if (StashHasMaterials)
                {
                    Logger.Log("[CubeRaresToLegendary] Getting Materials from Stash");
                    if (!await TakeItemsFromStash.Execute(Inventory.RareUpgradeIds, 5000))
                        return true;
                }
                else
                {
                    Logger.Log("[CubeRaresToLegendary] Oh no! Out of materials!");
                    return true;
                }

                await Coroutine.Sleep(500);
                await Coroutine.Yield();
            }

            return true;
        }



    }
}
