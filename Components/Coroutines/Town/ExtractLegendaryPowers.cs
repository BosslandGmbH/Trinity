using Buddy.Coroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    public class ExtractLegendaryPowers
    {
        public static bool HasUnlockedCube = true;
        private static DateTime _disabledUntil = DateTime.MinValue;
        private static readonly TimeSpan s_disableDuration = TimeSpan.FromMinutes(1);
        private static readonly List<int> s_itemsTakenFromStashAnnId = new List<int>();
        private static readonly HashSet<int> s_blacklistedActorSnoIds = new HashSet<int>();

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

            if (TownInfo.ZoltunKulle?.GetActor() is DiaUnit kule)
            {
                if (kule.IsQuestGiver)
                {
                    Core.Logger.Verbose("[ExtractLegendaryPowers] Cube is not unlocked yet");
                    _disabledUntil = DateTime.UtcNow.Add(s_disableDuration);
                    HasUnlockedCube = false;
                    return false;
                }
                HasUnlockedCube = true;
            }

            if (!HasUnlockedCube)
                return false;

            if (!HasCurrencyRequired)
            {
                Core.Logger.Verbose("[ExtractLegendaryPowers] Unable to find the required materials!");
                return false;
            }

            var backpackCandidates = GetLegendaryExtractionCandidates(InventorySlot.BackpackItems);
            var stashCandidates = Core.Settings.KanaisCube.CubeExtractFromStash
                ? GetLegendaryExtractionCandidates(InventorySlot.SharedStash).DistinctBy(i => i.ActorSnoId).ToList()
                : new List<TrinityItem>();

            if (!backpackCandidates.Any() && !stashCandidates.Any())
            {
                Core.Logger.Verbose("[ExtractLegendaryPowers] There are no items that need extraction!");
                _disabledUntil = DateTime.UtcNow.Add(s_disableDuration);
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

                if (s_blacklistedActorSnoIds.Contains(item.ActorSnoId))
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
            if (BrainBehavior.GreaterRiftInProgress)
            {
                Core.Logger.Verbose("Can't extract powers: inventory locked by greater rift");
                return false;
            }

            var result = await Main();

            // Make sure we put back anything we removed from stash. Its possible for example that we ran out of materials
            // and the current backpack contents do no longer match the loot rules. Don't want them to be lost.

            if (!s_itemsTakenFromStashAnnId.Any()) return result;

            await PutItemsInStash.Execute(s_itemsTakenFromStashAnnId);
            s_itemsTakenFromStashAnnId.Clear();
            return result;
        }

        public static async Task<bool> ExtractAllBackpack()
        {
            if (!HasCurrencyRequired)
            {
                Core.Logger.Log("[ExtractLegendaryPowers] Oh no! Out of materials!");
                return false;
            }

            var candidate = GetLegendaryExtractionCandidates(InventorySlot.BackpackItems).FirstOrDefault();
            if (candidate == null)
            {
                Core.Logger.Log("[ExtractLegendaryPowers] Oh no! Out of materials!");
                return false;
            }

            if (!await CubeItemsToMaterials.EnsureKanaisCube())
            {
                Core.Logger.Verbose("Unable to move to the cube.");
                return false;
            }

            if (!await ExtractPower(candidate))
            {
                Core.Logger.Verbose($"Unable to extract power from {candidate}");
                return false;
            }

            return true;
        }


        public static async Task<bool> Main()
        {
            while (CanRun())
            {
                if (!HasCurrencyRequired)
                {
                    Core.Logger.Log("Not enough currency to transmute");
                    return true;
                }

                var backpackCandidate = GetLegendaryExtractionCandidates(InventorySlot.BackpackItems).FirstOrDefault();
                if (backpackCandidate != null)
                {
                    if (!await CubeItemsToMaterials.EnsureKanaisCube())
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

                    Core.Logger.Log("Getting Legendaries from Stash");

                    if (!await TakeItemsFromStash.Execute(stashCandidates))
                        return false;

                    s_itemsTakenFromStashAnnId.AddRange(stashCandidates.Select(i => i.AnnId));
                }
                else
                {
                    Core.Logger.Verbose("Finished");
                    return true;
                }
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
            await Coroutine.Yield();

            var shouldBeDestroyedItem = InventoryManager.Backpack.FirstOrDefault(i => i.AnnId == itemDynamicId);
            if (shouldBeDestroyedItem == null && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.KanaisPowersExtractedActorSnoIds.Contains(itemSnoId))
            {
                Core.Logger.Log($"[ExtractLegendaryPowers] Item Power Extracted! '{itemName}' ({itemSnoId}) Description={affixDescription}");
                Core.Inventory.InvalidAnnIds.Add(itemDynamicId);
                s_itemsTakenFromStashAnnId.Remove(itemDynamicId);
                return true;
            }

            Core.Logger.Log($"[ExtractLegendaryPowers] Failed to Extract Power! '{itemName}' ({itemSnoId}) {itemInternalName} DynId={itemDynamicId}");
            s_blacklistedActorSnoIds.Add(itemSnoId);
            return false;
        }
    }
}