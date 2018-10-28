using Buddy.Coroutines;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();
        private static bool _isStartLocationOutOfTown;

        public static bool IsTownRunRequired => (ZetaDia.Me.CanUseTownPortal(out _) || ZetaDia.IsInTown) && (InventoryManager.NumFreeBackpackSlots <= 2 || InventoryManager.Equipped.Any(i => i.DurabilityPercent <= CharacterSettings.Instance.RepairWhenDurabilityBelow));

        // TODO: Make sure that is actually the portal we came from an not an open Rift portal (might cause a lot of empty meters).
        public static DiaGizmo ReturnPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(g => g.IsTownPortal);

        private static readonly Lazy<PropertyInfo> s_vendorProperty = new Lazy<PropertyInfo>(() => typeof(BrainBehavior).GetProperty("IsVendoring"));
        public static bool IsVendoring
        {
            get => BrainBehavior.IsVendoring;
            set => s_vendorProperty.Value.SetValue(null, value);
        }


        public static bool HasMaterialsRequired => Core.Inventory.Currency.HasCurrency(Zeta.Game.TransmuteRecipe.UpgradeRareItem);
        public static bool IsRareToLegendaryTransformationPossible(List<ItemSelectionType> types = null)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown) return false;

            if (TownInfo.ZoltunKulle?.GetActor() is DiaUnit kule)
            {
                if (kule.IsQuestGiver)
                {
                    s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] Cube is not unlocked yet");
                    return false;
                }
            }

            if (types == null && Core.Settings.KanaisCube.RareUpgradeTypes == ItemSelectionType.Unknown)
            {
                s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] No item types selected in settings - (Config => Items => Kanai's Cube)");
                return false;
            }

            if (!HasMaterialsRequired && InventoryManager.NumFreeBackpackSlots < 5)
            {
                s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] Not enough bag space");
                return false;
            }

            if (Core.Inventory.Currency.DeathsBreath < Core.Settings.KanaisCube.DeathsBreathMinimum)
            {
                s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] Not enough deaths breath - Limit is set to {Core.Settings.KanaisCube.DeathsBreathMinimum}, You currently have {Core.Inventory.Currency.DeathsBreath}");
                return false;
            }

            if (!GetBackPackRares(types).Any())
            {
                s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] You need some rares in your backpack for this to work!");
                return false;
            }

            if (!HasMaterialsRequired)
            {
                s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] Unable to find the materials we need, maybe you don't have them!");
                return false;
            }

            return true;
        }

        public static async Task<bool> EnsureIsInTown()
        {
            if (!ZetaDia.IsInTown)
            {
                if (!await CommonCoroutines.UseTownPortal(nameof(TrinityTownRun)))
                    return false;
                _isStartLocationOutOfTown = true;
            }
            return ZetaDia.IsInTown;
        }

        public static async Task<bool> ReturnToStartLocation()
        {
            if (_isStartLocationOutOfTown && ZetaDia.IsInTown)
            {
                if (!await CommonCoroutines.MoveAndInteract(ReturnPortal, () => ZetaDia.IsInTown))
                    return false;
                _isStartLocationOutOfTown = false;
            }
            return true;
        }

        public static async Task<bool> IdentifyItems()
        {
            if (!ZetaDia.IsInTown) return true;

            if (Core.Settings.Items.KeepLegendaryUnid)
            {
                s_logger.Debug($"[{nameof(IdentifyItems)}] Town run setting 'Keep Legendary Unidentified' - Skipping ID");
                return true;
            }

            if (!Core.Inventory.Backpack.Any(i => i.IsUnidentified)) return true;

            var bookActor = TownInfo.BookOfCain;
            if (bookActor == null)
            {
                s_logger.Warn($"[{nameof(IdentifyItems)}] TownInfo.BookOfCain not found Act={ZetaDia.CurrentAct} WorldSnoId={ZetaDia.Globals.WorldSnoId}");
                return true;
            }

            if (!await CommonCoroutines.MoveAndInteract(bookActor.GetActor(), () => CommonCoroutines.IsInteracting))
                return false;

            s_logger.Info($"[{nameof(IdentifyItems)}] Identifying Items");
            await Coroutine.Wait(TimeSpan.FromSeconds(10), () => !CommonCoroutines.IsInteracting);
            return false;
        }

        public static async Task<bool> DoTownRun()
        {
            // We're dead, wait till we're alive again...
            if (!ZetaDia.IsInGame || ZetaDia.Me.IsDead)
                return true;

            if (IsTownRunRequired)
            {
                IsVendoring = true;
            }

            if (!IsVendoring)
                return true;

            // Go to town...
            if (!await EnsureIsInTown())
                return false;

            // Wait for Rift turn in before continue...
            if (TownInfo.Orek?.GetActor() is DiaUnit orek && orek.IsQuestGiver)
                return true;

            // Run specified actions when all of them return true we're good to continue...
            if (!await Any(
                IdentifyItems,
                ExtractLegendaryPowers,
                Gamble,
                TransmuteRareToLegendary,
                TransmuteMaterials,
                DropItems,
                StashItems,
                SellItems,
                SalvageItems,
                StashItems,
                RepairItems
            ))
                return false;

            // Go back where we came from...
            if (!await ReturnToStartLocation()) return false;

            s_logger.Info("Town run finished!");
            IsVendoring = false;
            return true;
        }

        public static async Task<bool> Any(params Func<Task<bool>>[] taskProducers)
        {
            foreach (var task in taskProducers)
            {
                if (!await task())
                    return false;
            }
            return true;
        }

        public static async Task<bool> TakeReturnPortal()
        {
            if (!await CommonCoroutines.MoveAndInteract(ReturnPortal, () => ZetaDia.IsInTown))
                return false;
            return ZetaDia.IsInTown;
        }

        public static async Task<bool> EnsureKanaisCube()
        {
            if (!await CommonCoroutines.MoveAndInteract(TownInfo.KanaisCube.GetActor(), () => UIElements.TransmuteItemsDialog.IsVisible))
                return false;
            return UIElements.TransmuteItemsDialog.IsVisible;
        }

        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<bool> TransmuteRecipe(TrinityItem item, TransmuteRecipe recipe)
        {
            return await TransmuteRecipe(new List<TrinityItem> { item }, recipe);
        }

        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<bool> TransmuteRecipe(List<TrinityItem> transmuteGroup, TransmuteRecipe recipe)
        {
            return await TransmuteRecipe(transmuteGroup.Select(i => i.AnnId).ToList(), recipe);
        }

        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<bool> TransmuteRecipe(IEnumerable<int> transmuteGroupAnnIds, TransmuteRecipe recipe)
        {
            if (!ZetaDia.IsInGame) return true;

            if (!Core.Inventory.Currency.HasCurrency(recipe))
            {
                s_logger.Error($"[{nameof(TransmuteRecipe)}] Not enough currency for {recipe}");
                return true;
            }

            if (!await EnsureKanaisCube())
                return false;

            InventoryManager.TransmuteItems(transmuteGroupAnnIds.ToArray(), recipe);

            if (!UIElements.AcceptTransmutationButton.IsEnabled)
                return false;

            UIElements.AcceptTransmutationButton.Click();
            s_logger.Error($"[{nameof(TransmuteRecipe)}] Zip Zap!");
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
                s_logger.Debug($"[{nameof(GetBackPackRares)}] No items were found in backpack!");

            var rares = Core.Inventory.Backpack.Where(i =>
            {
                if (Core.Inventory.InvalidAnnIds.Contains(i.AnnId))
                    return false;

                if (i.ItemBaseType != ItemBaseType.Armor && i.ItemBaseType != ItemBaseType.Weapon && i.ItemBaseType != ItemBaseType.Jewelry)
                    return false;

                if (i.ItemQualityLevel < ItemQuality.Rare4 && i.ItemQualityLevel >= ItemQuality.Legendary)
                    return false;

                return types == null || types.Contains(GetBackPackItemSelectionType(i));

            }).ToList();

            s_logger.Debug($"[{nameof(GetBackPackRares)}] {rares.Count} Valid Rares in Backpack");
            return rares;
        }

        // TODO: Figure out why that is here...
        public static ItemSelectionType GetBackPackItemSelectionType(TrinityItem item)
        {
            return Enum.TryParse(item.TrinityItemType.ToString(), out ItemSelectionType result) ? result : ItemSelectionType.Unknown;
        }

        public static async Task<bool> TransmuteRareToLegendary()
        {
            return await TransmuteRareToLegendary(null);
        }

        /// <summary>
        /// Convert rares into legendaries with Kanai's cube
        /// </summary>
        /// <param name="types">restrict the rares that can be selected by ItemType</param>
        public static async Task<bool> TransmuteRareToLegendary(List<ItemSelectionType> types)
        {
            if (!IsRareToLegendaryTransformationPossible(types))
                return true;

            var item = GetBackPackRares(types).First();
            if (item == null)
                return true;

            return await TransmuteRecipe(item, Zeta.Game.TransmuteRecipe.UpgradeRareItem);
        }
    }
}
