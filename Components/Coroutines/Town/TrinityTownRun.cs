using Buddy.Coroutines;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
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

        public static bool IsTownRunRequired => (ZetaDia.IsInTown ||
                                                 ZetaDia.Me.CanUseTownPortal(out _)) &&
                                                (InventoryManager.NumFreeBackpackSlots <= 2 ||
                                                 InventoryManager.Equipped
                                                     .Any(i => i.DurabilityPercent <= CharacterSettings.Instance.RepairWhenDurabilityBelow));

        // TODO: Make sure that is actually the portal we came from an not an open Rift portal (might cause a lot of empty meters).
        public static DiaGizmo ReturnPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(g => g.IsFullyValid() && g.IsTownPortal);

        private static readonly Lazy<PropertyInfo> s_vendorProperty = new Lazy<PropertyInfo>(() => typeof(BrainBehavior).GetProperty("IsVendoring"));
        public static bool IsVendoring
        {
            get => BrainBehavior.IsVendoring;
            set => s_vendorProperty.Value.SetValue(null, value);
        }

        public static bool HasMaterialsRequired => Core.Inventory.Currency.HasCurrency(Zeta.Game.TransmuteRecipe.UpgradeRareItem);

        public static bool IsRareToLegendaryTransformationPossible(List<ItemSelectionType> types = null)
        {
            if (!ZetaDia.IsInGame ||
                !ZetaDia.IsInTown)
            {
                return false;
            }

            if (TownInfo.ZoltunKulle?.GetActor() is DiaUnit kule)
            {
                if (kule.IsQuestGiver)
                {
                    s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] Cube is not unlocked yet");
                    return false;
                }
            }

            if (types == null &&
                Core.Settings.KanaisCube.RareUpgradeTypes == ItemSelectionType.Unknown)
            {
                s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] No item types selected in settings - (Config => Items => Kanai's Cube)");
                return false;
            }

            if (!HasMaterialsRequired &&
                InventoryManager.NumFreeBackpackSlots < 5)
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

            if (HasMaterialsRequired)
                return true;

            s_logger.Verbose($"[{nameof(IsRareToLegendaryTransformationPossible)}] Unable to find the materials we need, maybe you don't have them!");
            return false;

        }

        public static async Task<CoroutineResult> EnsureIsInTown()
        {
            if (ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            CoroutineResult previousResult;
            if ((previousResult = await CommonCoroutines.UseTownPortal(nameof(TrinityTownRun))) == CoroutineResult.Running)
                return CoroutineResult.Running;

            if (previousResult == CoroutineResult.Done)
                _isStartLocationOutOfTown = true;

            return previousResult;
        }

        public static async Task<CoroutineResult> ReturnToStartLocation()
        {
            if (_isStartLocationOutOfTown &&
                ZetaDia.IsInTown)
            {
                CoroutineResult previousResult;
                if ((previousResult = await CommonCoroutines.MoveAndInteract(
                        ReturnPortal,
                        () => ZetaDia.IsInTown)) == CoroutineResult.Running)
                {
                    return CoroutineResult.Running;
                }

                if (previousResult == CoroutineResult.Failed)
                    return CoroutineResult.Failed;
            }
            _isStartLocationOutOfTown = false;
            return CoroutineResult.Done;
        }

        public static async Task<CoroutineResult> IdentifyItems()
        {
            if (!ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            if (Core.Settings.Items.KeepLegendaryUnid)
            {
                s_logger.Debug($"[{nameof(IdentifyItems)}] Town run setting 'Keep Legendary Unidentified' - Skipping ID");
                return CoroutineResult.NoAction;
            }

            if (!InventoryManager.Backpack.Any(i => i.IsUnidentified))
            {
                s_logger.Debug($"[{nameof(IdentifyItems)}] No unidentified items in Backpack.");
                return CoroutineResult.Done;
            }

            var bookActor = TownInfo.BookOfCain;
            if (bookActor == null)
            {
                s_logger.Warn($"[{nameof(IdentifyItems)}] TownInfo.BookOfCain not found Act={ZetaDia.CurrentAct} WorldSnoId={(SNOWorld)ZetaDia.Globals.WorldSnoId}");
                return CoroutineResult.Failed;
            }

            var actualBook = bookActor.GetActor();

            switch (actualBook)
            {
                case null when await CommonCoroutines.MoveTo(bookActor.Position) != MoveResult.ReachedDestination:
                    return CoroutineResult.Running;
                case null:
                    s_logger.Warn($"[{nameof(IdentifyItems)}] TownInfo.BookOfCain.GetActor() not found Act={ZetaDia.CurrentAct} WorldSnoId={(SNOWorld)ZetaDia.Globals.WorldSnoId}");
                    return CoroutineResult.Failed;
            }

            if (await CommonCoroutines.MoveAndInteract(
                bookActor.GetActor(),
                () => CommonCoroutines.IsInteracting) == CoroutineResult.Running)
            {
                return CoroutineResult.Running;
            }

            s_logger.Info($"[{nameof(IdentifyItems)}] Identifying Items");
            await Coroutine.Wait(TimeSpan.FromSeconds(10), () => !CommonCoroutines.IsInteracting);
            return CoroutineResult.Running;
        }

        public static async Task<CoroutineResult> DoTownRun()
        {
            // We're dead, wait till we're alive again...
            if (!ZetaDia.IsInGame ||
                ZetaDia.Me.IsDead)
            {
                return CoroutineResult.NoAction;
            }

            if (!IsVendoring && IsTownRunRequired)
                IsVendoring = true;

            if (!IsVendoring)
                return CoroutineResult.NoAction;

            CoroutineResult previousResult;
            // Go to town...
            if ((previousResult = await EnsureIsInTown()) == CoroutineResult.Running)
                return CoroutineResult.Running;

            if (previousResult == CoroutineResult.Failed)
                return CoroutineResult.Failed;

            // Wait for Rift turn in before continue...
            if (TownInfo.Orek?.GetActor() is DiaUnit orek &&
                orek.IsQuestGiver)
            {
                return CoroutineResult.NoAction;
            }

            // Run specified actions in sequence...
            if ((previousResult = await CommonCoroutines.Sequence(
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
                    RepairItems)) == CoroutineResult.Running)
            {
                return CoroutineResult.Running;
            }

            if (previousResult == CoroutineResult.Failed)
                return CoroutineResult.Failed;

            // Go back where we came from...
            if ((previousResult = await ReturnToStartLocation()) == CoroutineResult.Running)
                return CoroutineResult.Running;

            if (previousResult == CoroutineResult.Failed)
                return CoroutineResult.Failed;

            s_logger.Info("Town run finished!");
            IsVendoring = false;
            return CoroutineResult.Done;
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
            if (await CommonCoroutines.MoveAndInteract(
                    ReturnPortal,
                    () => ZetaDia.IsInTown) == CoroutineResult.Running)
            {
                return false;
            }

            return ZetaDia.IsInTown;
        }

        public static async Task<bool> EnsureKanaisCube()
        {
            if (await CommonCoroutines.MoveAndInteract(
                    TownInfo.KanaisCube.GetActor(),
                    () => UIElements.TransmuteItemsDialog.IsVisible) == CoroutineResult.Running)
            {
                return false;
            }

            return UIElements.TransmuteItemsDialog.IsVisible;
        }

        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<CoroutineResult> TransmuteRecipe(TransmuteRecipe recipe, params ACDItem[] transmuteGroup)
        {
            return await TransmuteRecipe(recipe, transmuteGroup.Select(i => i.AnnId).ToList());
        }

        /// <summary>
        /// Move to Kanai's cube and transmute.
        /// </summary>
        public static async Task<CoroutineResult> TransmuteRecipe(TransmuteRecipe recipe, IEnumerable<int> transmuteGroupAnnIds)
        {
            if (!ZetaDia.IsInGame)
                return CoroutineResult.NoAction;

            if (!Core.Inventory.Currency.HasCurrency(recipe))
            {
                s_logger.Error($"[{nameof(TransmuteRecipe)}] Not enough currency for {recipe}");
                return CoroutineResult.NoAction;
            }

            if (!await EnsureKanaisCube())
                return CoroutineResult.Running;

            InventoryManager.TransmuteItems(transmuteGroupAnnIds.ToArray(), recipe);

            if (!UIElements.AcceptTransmutationButton.IsEnabled)
                return CoroutineResult.Running;

            UIElements.AcceptTransmutationButton.Click();
            s_logger.Error($"[{nameof(TransmuteRecipe)}] Zip Zap!");
            return CoroutineResult.Done;
        }

        /// <summary>
        /// A list of conversion candidates from backpack
        /// </summary>
        public static List<ACDItem> GetBackPackRares(IEnumerable<ItemSelectionType> types = null)
        {
            if (types == null)
                types = Core.Settings.KanaisCube.GetRareUpgradeSettings();

            if (!InventoryManager.Backpack.Any())
                s_logger.Debug($"[{nameof(GetBackPackRares)}] No items were found in backpack!");

            var rares = InventoryManager.Backpack.Where(i =>
            {
                if (Core.Inventory.InvalidAnnIds.Contains(i.AnnId))
                    return false;

                if (i.ItemBaseType != ItemBaseType.Armor &&
                    i.ItemBaseType != ItemBaseType.Weapon &&
                    i.ItemBaseType != ItemBaseType.Jewelry)
                {
                    return false;
                }

                if (i.ItemQualityLevel < ItemQuality.Rare4 ||
                    i.ItemQualityLevel >= ItemQuality.Legendary)
                {
                    return false;
                }

                return types == null ||
                       types.Contains(GetBackPackItemSelectionType(i));

            }).ToList();

            s_logger.Debug($"[{nameof(GetBackPackRares)}] {rares.Count} Valid Rares in Backpack");
            return rares;
        }

        // TODO: Figure out why that is here...
        public static ItemSelectionType GetBackPackItemSelectionType(ACDItem item)
        {
            return Enum.TryParse(item.GetTrinityItemType().ToString(), out ItemSelectionType result) ?
                result :
                ItemSelectionType.Unknown;
        }

        public static async Task<CoroutineResult> TransmuteRareToLegendary()
        {
            return await TransmuteRareToLegendary(null);
        }

        /// <summary>
        /// Convert rares into legendaries with Kanai's cube
        /// </summary>
        /// <param name="types">restrict the rares that can be selected by ItemType</param>
        public static async Task<CoroutineResult> TransmuteRareToLegendary(List<ItemSelectionType> types)
        {
            if (!IsRareToLegendaryTransformationPossible(types))
                return CoroutineResult.NoAction;

            var item = GetBackPackRares(types).First();
            if (item == null)
                return CoroutineResult.NoAction;

            return await TransmuteRecipe(Zeta.Game.TransmuteRecipe.UpgradeRareItem, item);
        }
    }
}
