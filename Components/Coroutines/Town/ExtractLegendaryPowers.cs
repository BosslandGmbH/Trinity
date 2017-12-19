using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Coroutines.Town
{
    public class ExtractLegendaryPowers
    {
        public static bool HasUnlockedCube = true;
        private static DateTime _disabledUntil = DateTime.MinValue;
        private static readonly TimeSpan DisableDuration = TimeSpan.FromMinutes(1);


        public static HashSet<RawItemType> DoNotExtractRawItemTypes = new HashSet<RawItemType>
        {
            RawItemType.EnchantressSpecial,
            RawItemType.ScoundrelSpecial,
            RawItemType.FollowerSpecial,
            RawItemType.TemplarSpecial,
        };

        public static HashSet<int> DoNotExtractItemIds = new HashSet<int>()
        {
            Legendary.PigSticker.Id,
            Legendary.CorruptedAshbringer.Id
        };

        private static List<int> _itemsTakenFromStashAnnId = new List<int>();
        private static HashSet<int> _blacklistedActorSnoIds = new HashSet<int>();

        public static bool HasCurrencyRequired
            => Core.Inventory.Currency.HasCurrency(TransmuteRecipe.ExtractLegendaryPower);

        public static bool CanRun()
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown || ZetaDia.Storage.CurrentWorldType != Act.OpenWorld)
                return false;

            if (Core.Settings.KanaisCube.ExtractLegendaryPowers == CubeExtractOption.None)
                return false;

            if (DateTime.UtcNow < _disabledUntil)
                return false;

            var kule = TownInfo.ZultonKule?.GetActor() as DiaUnit;
            if (kule != null)
            {
                if (kule.IsQuestGiver)
                {
                    Core.Logger.Verbose("[提取传奇威能] 魔盒还没有解锁");
                    _disabledUntil = DateTime.UtcNow.Add(DisableDuration);
                    HasUnlockedCube = false;
                    return false;
                }
                HasUnlockedCube = true;
            }

            if (!HasUnlockedCube)
                return false;

            if (!HasCurrencyRequired)
            {
                Core.Logger.Verbose("[提取传奇威能] 无法找到需求的材料！");
                return false;
            }

            var backpackCandidates = GetLegendaryExtractionCandidates(InventorySlot.BackpackItems);
            var stashCandidates = Core.Settings.KanaisCube.CubeExtractFromStash
                ? GetLegendaryExtractionCandidates(InventorySlot.SharedStash).DistinctBy(i => i.ActorSnoId).ToList()
                : new List<TrinityItem>();

            if (!backpackCandidates.Any() && !stashCandidates.Any())
            {
                Core.Logger.Verbose("[提取传奇威能] 没有可提取的物品！");
                _disabledUntil = DateTime.UtcNow.Add(DisableDuration);
                return false;
            }

            return true;
        }

        private static IEnumerable<TrinityItem> GetLegendaryExtractionCandidates(InventorySlot slot)
        {
            var alreadyCubedIds = new HashSet<int>(ZetaDia.Storage.PlayerDataManager.ActivePlayerData.KanaisPowersExtractedActorSnoIds);
            var usedIds = new HashSet<int>();

            foreach (var item in Core.Inventory.Where(i => i.InventorySlot == slot))
            {
                if (!item.IsValid)
                    continue;

                if (item.TrinityItemType == TrinityItemType.HealthPotion)
                    continue;

                if (item.FollowerType != FollowerType.None)
                    continue;

                if (usedIds.Contains(item.ActorSnoId))
                    continue;

                if (DoNotExtractRawItemTypes.Contains(item.RawItemType))
                    continue;

                if (DoNotExtractItemIds.Contains(item.ActorSnoId))
                    continue;

                if (alreadyCubedIds.Contains(item.ActorSnoId))
                    continue;

                if (_blacklistedActorSnoIds.Contains(item.ActorSnoId))
                    continue;

                if (Core.Settings.KanaisCube.ExtractLegendaryPowers == CubeExtractOption.OnlyTrashed && Combat.TrinityCombat.Loot.ShouldStash(item))
                    continue;

                if (Core.Settings.KanaisCube.ExtractLegendaryPowers == CubeExtractOption.OnlyNonAncient && !item.IsAncient)
                    continue;

                if (string.IsNullOrEmpty(Legendary.GetItem(item)?.LegendaryAffix))
                    continue;

                usedIds.Add(item.ActorSnoId);
                yield return item;
            }
        }

        public static async Task<bool> Execute()
        {
            if (Core.Player.IsInventoryLockedForGreaterRift)
            {
                Core.Logger.Verbose("无法提取威能: 库存被大秘境锁定");
                return false;
            }

            var result = await Main();

            // Make sure we put back anything we removed from stash. Its possible for example that we ran out of materials
            // and the current backpack contents do no longer match the loot rules. Don't want them to be lost.

            if (_itemsTakenFromStashAnnId.Any())
            {
                await PutItemsInStash.Execute(_itemsTakenFromStashAnnId);
                _itemsTakenFromStashAnnId.Clear();
            }
            return result;
        }

        public static async Task<bool> ExtractAllBackpack()
        {
            if (!HasCurrencyRequired)
            {
                Core.Logger.Log("[提取传奇威能] Oh!不！没有材料了！");
                return true;
            }

            var candidate = GetLegendaryExtractionCandidates(InventorySlot.BackpackItems).FirstOrDefault();
            if (candidate == null)
            {
                Core.Logger.Log("[提取传奇威能] Oh!不！没有材料了！");
                return true;
            }

            if (!await MoveToCube())
            {
                Core.Logger.Verbose("Unable to move to the cube.");
                return true;
            }

            if (!await ExtractPower(candidate))
            {
                Core.Logger.Verbose($"Unable to extract power from {candidate}");
                return true;
            }

            return false;
        }


        public static async Task<bool> Main()
        {
            var started = false;
            while (CanRun())
            {
                if (!started)
                {
                    Core.Logger.Log("提取模式设置为: {0}", Core.Settings.KanaisCube.ExtractLegendaryPowers);
                    started = true;
                }

                if (!HasCurrencyRequired)
                {
                    Core.Logger.Log("没有足够的金币来提取");
                    return false;
                }

                var backpackCandidate = GetLegendaryExtractionCandidates(InventorySlot.BackpackItems).FirstOrDefault();
                if (backpackCandidate != null)
                {
                    if (!await MoveToCube())
                    {
                        Core.Logger.Verbose("Unable to move to cube");
                        return false;
                    }

                    if (!await ExtractPower(backpackCandidate))
                        return false;
              
                }
                else if (Core.Settings.KanaisCube.CubeExtractFromStash)
                {
                    var stashCandidates = GetLegendaryExtractionCandidates(InventorySlot.SharedStash).ToList();
                    if (!stashCandidates.Any())
                        return false;

                    Core.Logger.Log("提取仓库中传奇物品的威能");

                    if (!await TakeItemsFromStash.Execute(stashCandidates))
                        return false;

                    _itemsTakenFromStashAnnId.AddRange(stashCandidates.Select(i => i.AnnId));
                }
                else
                {
                    Core.Logger.Verbose("Finished");
                    return false;
                }

                await Coroutine.Sleep(500);
                await Coroutine.Yield();
            }
            return true;
        }

        private static async Task<bool> ExtractPower(TrinityItem item)
        {
            if (item == null)
                return false;

            var itemName = item.Name;
            var itemDynamicId = item.AnnId;
            var itemInternalName = item.InternalName;
            var itemSnoId = item.ActorSnoId;
            var affixDescription = item.Reference.LegendaryAffix;

            await Transmute.Execute(item, TransmuteRecipe.ExtractLegendaryPower);
            await Coroutine.Sleep(1500);

            var shouldBeDestroyedItem = InventoryManager.Backpack.FirstOrDefault(i => i.AnnId == itemDynamicId);
            if (shouldBeDestroyedItem == null && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.KanaisPowersExtractedActorSnoIds.Contains(itemSnoId))
            {
                Core.Logger.Log($"[提取传奇威能] 获得物品威能! '{itemName}' ({itemSnoId}) 描述={affixDescription}");
                Core.Inventory.InvalidAnnIds.Add(itemDynamicId);
                _itemsTakenFromStashAnnId.Remove(itemDynamicId);
                return true;
            }

            Core.Logger.Log($"[提取传奇威能] 无法提取威能! '{itemName}' ({itemSnoId}) {itemInternalName} DynId={itemDynamicId}");
            _blacklistedActorSnoIds.Add(itemSnoId);
            return false;
        }

        private static async Task<bool> MoveToCube()
        {
            if (GameUI.KanaisCubeWindow.IsVisible)
                return true;

            if (TownInfo.KanaisCube.Distance < 10f)
                return true;

            if (TownInfo.KanaisCube.Distance > 350f || TownInfo.KanaisCube.Position == Vector3.Zero)
                return false;

            if (!await MoveToAndInteract.Execute(TownInfo.KanaisCube))
                return false;

            return true;
        }
    }
}