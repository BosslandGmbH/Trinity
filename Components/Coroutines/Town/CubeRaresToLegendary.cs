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
                    Core.Logger.Verbose("[魔盒稀有升级传奇] 魔盒还没有解锁");
                    HasUnlockedCube = false;
                    return false;
                }
                HasUnlockedCube = true;
            }

            if (!HasUnlockedCube)
                return false;

            if (types == null && Core.Settings.KanaisCube.RareUpgradeTypes == ItemSelectionType.Unknown)
            {
                Core.Logger.Verbose("[魔盒稀有升级传奇] 在设置中没有选择道具类型 - (设置 => 物品 => 卡奈魔盒)");
                return false;
            }

            if (!HasMaterialsRequired && InventoryManager.NumFreeBackpackSlots < 5)
            {
                Core.Logger.Verbose("[魔盒稀有升级传奇] 没有足够的背包空间");
                return false;
            }

            var dbs = Core.Inventory.Currency.DeathsBreath;
            if (dbs < Core.Settings.KanaisCube.DeathsBreathMinimum)
            {
                Core.Logger.Verbose("[魔盒稀有升级传奇] 没有足够的死亡之息 - 限制设置为 {0}, 你目前只有 {1}", Core.Settings.KanaisCube.DeathsBreathMinimum, dbs);
                return false;
            }

            if (!GetBackPackRares(types).Any())
            {
                Core.Logger.Verbose("[魔盒稀有升级传奇] 你需要在背包中有一些稀有道具才能使魔盒工作!");
                return false;
            }

            if (!HasMaterialsRequired)
            {
                Core.Logger.Verbose("[魔盒稀有升级传奇] 无法找到我们所需要的材料, 也许你没有他们!");
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
                Core.Logger.Verbose("[魔盒稀有升级传奇] 没有在背包中找到道具!");
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

            Core.Logger.Log(LogCategory.Behavior, "[魔盒稀有升级传奇] {0} 有效的稀有道具在背包", rares.Count);
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

                //Core.Logger.Log("[魔盒稀有升级传奇] 魔盒稀有升级传奇 开始! 喔喔喔!");

                var backpackGuids = new HashSet<int>(InventoryManager.Backpack.Select(i => i.ACDId));

                if (HasMaterialsRequired)
                {
                    if (TownInfo.KanaisCube.Distance > 10f || !GameUI.KanaisCubeWindow.IsVisible)
                    {
                        if (!await MoveToAndInteract.Execute(TownInfo.KanaisCube))
                        {
                            Core.Logger.Log("无法移动到魔盒, 很不幸.");
                            break;
                        }
                        continue;
                    }

                    //Core.Logger.Log("[魔盒稀有升级传奇] 准备好了让我们来转换!");

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
                            Core.Logger.Warn($"[魔盒稀有升级传奇] 升级稀有 '{itemName}' ---> '{newLegendaryItem.Name}' ({newItem.ActorSnoId}) 最初的 !~");
                        else
                            Core.Logger.Log($"[魔盒稀有升级传奇] 升级稀有 '{itemName}' ---> '{newLegendaryItem.Name}' ({newItem.ActorSnoId})");

                      
                    }
                    else
                    {
                        Core.Logger.Log("[魔盒稀有升级传奇] 升级道具失败'{0}' {1} DynId={2} 背包有材料={3}",
                            itemName, itemInternalName, itemAnnId, HasMaterialsRequired);
                    }

                    Core.Inventory.InvalidAnnIds.Add(itemAnnId);
                }
                else
                {
                    Core.Logger.Log("[魔盒稀有升级传奇] Oh 不! 没有材料了!");
                    return true;
                }

                await Coroutine.Sleep(500);
                await Coroutine.Yield();
            }

            return true;
        }
    }
}