using log4net;
using System;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.Settings.ItemList;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat
{
    public interface ILootProvider
    {
        bool ShouldDrop(ACDItem item, ItemEvaluationType scheduledAction);

        bool ShouldStash(ACDItem item);

        bool ShouldSalvage(ACDItem item);

        bool ShouldPickup(ACDItem item);

        bool ShouldSell(ACDItem item);

        bool IsBackpackFull { get; }

        Vector2 FindBackpackSlot(bool twoSlot);
    }

    public class DefaultLootProvider : ILootProvider
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        public static int FreeBagSlots { get; set; } = 4;
        public static int FreeBagSlotsInTown { get; set; } = 30;

        public bool ShouldPickup(ACDItem item)
        {
            if (item == null ||
                !item.IsValid ||
                item.ActorSnoId == 0 ||
                item.GameBalanceId == 0)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] IGNORE: Not valid - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            RawItemType rit = item.GetRawItemType();
            TrinityItemType tit = item.GetTrinityItemType();
            TrinityItemQuality tiq = item.GetTrinityItemQuality();
            TrinityItemBaseType tib = item.GetTrinityItemBaseType();
            int gbi = item.GameBalanceId;
            ItemQuality iql = item.ItemQualityLevel;

            if (rit == RawItemType.CosmeticPet)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Pet - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (rit == RawItemType.CosmeticWings)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Wings - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (rit == RawItemType.CosmeticPennant)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Pennant - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (rit == RawItemType.CosmeticPortraitFrame)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Portrait - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            //if (item.InternalNameLowerCase.Contains("cosmetic"))
            //    return true;

            if (Core.Settings.Items.DisableLootingInCombat && TrinityCombat.IsInCombat && item.Distance > 8f)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] IGNORE: CombatLooint disabled and IsInCombat and Distance is larger than 8. - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (Core.Settings.Items.DontPickupInTown && Core.Player.IsInTown && !item.Stats.IsItemAssigned)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] IGNORE: Don't pickup in town - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            // 451002, //sir williams - 451002 (TentacleBear_C_Unique_Cosmetic_02)
            // portrait - 410998 (Cosmetic_Portrait_Frame_1)

            //if (Core.Settings.Items.InCombatLooting != SettingMode.Enabled && Combat.IsInCombat)
            //{
            //    if (Core.Settings.Items.InCombatLooting == SettingMode.Disabled)
            //        return false;

            //    if (Core.Settings.Items.InCombatLooting == SettingMode.Selective)
            //    {
            //        var pickupQuality = TypeConversions.GetPickupItemQuality(item.TrinityItemQuality);
            //        if (!Core.Settings.Items.InCombatLootQualities.HasFlag(pickupQuality))
            //            return false;
            //    }
            //}

            if (Core.Settings.Items.DontWalkToLowQuality && item.Distance > 8f && item.IsLowQuality && !item.IsCraftingReagent)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] IGNORE: Don't walk to low quality - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.Stats.IsAncient && Core.Settings.ItemList.AlwaysStashAncients)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: IsAncient && AlwaysStashAncients - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (item.Stats.IsPrimalAncient && Core.Settings.ItemList.AlwaysStashPrimalAncients)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: IsPrimalAncient && AlwaysStashPrimalAncients - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (tit == TrinityItemType.ConsumableAddSockets)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: ConsumeableAddSockets - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (rit == RawItemType.Book && Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.Lore))
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Book && SpecialItemTypes.Lore - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (rit == RawItemType.Junk && Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.CultistPage))
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Junk && SpecialItemTypes.CultistPage - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (gbi == GameData.ItemGameBalanceIds.DeathsBreath)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.DeathsBreath);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: DeathsBreath - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (item.ActorSnoId == (int)SNOActor.A1_BlackMushroom)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.RottenMushroom);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: Rotten Mushroom - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (gbi == GameData.ItemGameBalanceIds.ArcaneDust)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.ArcaneDust);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: ArcaneDust - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (gbi == GameData.ItemGameBalanceIds.VeiledCrystal)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.VeiledCrystals);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: VeiledCrystals - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (gbi == GameData.ItemGameBalanceIds.ReusableParts)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.ReusableParts);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: ReusableParts - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (gbi == GameData.ItemGameBalanceIds.ForgottenSoul)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: ForgottenSoul - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (GameData.HerdingMatsSnoIds.Contains(item.ActorSnoId))
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.StaffOfHeardingParts);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: StaffOfHeardingParts - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (GameData.TransmogTable.Contains(gbi) ||
                item.InternalName.StartsWith("Transmog") ||
                item.ActorSnoId == 110952) //Rakanishu's Blade
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Transmog - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (tit == TrinityItemType.TieredLootrunKey ||
                tit == TrinityItemType.LootRunKey)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TieredLootrunKey);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: TieredLootrunKey - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (tit == TrinityItemType.HealthPotion &&
                item.ItemQualityLevel >= ItemQuality.Legendary)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: HealthPotion && Legendary - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (tit == TrinityItemType.InfernalKey ||
                tit == TrinityItemType.PortalDevice)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.KeywardenIngredients);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: KeywardenIngredients - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (tit == TrinityItemType.UberReagent)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: UberReagent - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (tit == TrinityItemType.HoradricRelic && Core.Player.BloodShards < Core.Player.MaxBloodShards)
            {
                bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.BloodShards);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: BloodShards - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (tit == TrinityItemType.ProgressionGlobe)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: ProgressionGlobe - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (tit == TrinityItemType.CraftingMaterial)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: CraftingMaterial - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            switch (rit)
            {
                case RawItemType.CraftingPlan:
                case RawItemType.CraftingPlan_Jeweler:
                case RawItemType.CraftingPlan_Smith:
                case RawItemType.CraftingPlanLegendary_Smith:
                case RawItemType.CraftingPlan_Mystic:
                case RawItemType.CraftingPlan_MysticTransmog:
                    bool result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.CraftingPlans);
                    s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: CraftingPlans - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return result;
            }

            if (tit == TrinityItemType.CraftingPlan &&
                iql >= ItemQuality.Legendary &&
                Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.CraftingPlans))
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: CraftingPlan && Legendary - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (item.GetObjectType() == TrinityObjectType.BloodShard)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: BloodShard - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (item.GetItemType() == ItemType.LegendaryGem)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: LegendaryGem - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (iql >= ItemQuality.Legendary)
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: ItemQuality >= Legendary - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            switch (tib)
            {
                case TrinityItemBaseType.WeaponTwoHand:
                case TrinityItemBaseType.WeaponOneHand:
                case TrinityItemBaseType.WeaponRange:
                case TrinityItemBaseType.Armor:
                case TrinityItemBaseType.Offhand:
                case TrinityItemBaseType.Jewelry:
                case TrinityItemBaseType.FollowerItem:

                    if (tiq == TrinityItemQuality.Inferior)
                    {
                        bool result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Grey);
                        s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: PickupItemQualities.Grey - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return result;
                    }

                    if (tiq == TrinityItemQuality.Common)
                    {
                        bool result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.White);
                        s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: PickupItemQualities.White - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return result;
                    }

                    if (tiq == TrinityItemQuality.Magic)
                    {
                        bool result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Blue);
                        s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: PickupItemQualities.Blue - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return result;
                    }

                    if (tiq == TrinityItemQuality.Rare)
                    {
                        bool result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Yellow);
                        s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: PickupItemQualities.Yellow - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return result;
                    }

                    return false;

                case TrinityItemBaseType.Gem:

                    if ((int)item.Stats.GemQuality < Core.Settings.Items.GemLevel ||
                        (tit == TrinityItemType.Ruby &&
                         !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Ruby)) ||
                        (tit == TrinityItemType.Emerald &&
                         !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Emerald)) ||
                        (tit == TrinityItemType.Amethyst &&
                         !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Amethyst)) ||
                        (tit == TrinityItemType.Topaz &&
                         !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Topaz)) ||
                        (tit == TrinityItemType.Diamond &&
                         !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Diamond)))
                    {
                        s_logger.Debug($"[{nameof(ShouldPickup)}] IGNORE: GemSettings - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return false;
                    }
                    break;

                case TrinityItemBaseType.Misc:
                case TrinityItemBaseType.HealthGlobe:
                    s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Misc || HealthGlobe - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return true;

                case TrinityItemBaseType.ProgressionGlobe:
                    s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: ProgressionGlobe - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return true;

                default:
                    s_logger.Debug($"[{nameof(ShouldPickup)}] IGNORE: Default - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return false;
            }
            s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Fallback - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
            return true;
        }

        public bool ShouldDrop(ACDItem item, ItemEvaluationType scheduledAction)
        {
            if (item.IsProtected() || item.IsAccountBound)
            {
                return false;
            }

            if (item.IsGem || item.IsCraftingReagent || item.GetTrinityItemType() == TrinityItemType.CraftingPlan)
            {
                return false;
            }

            if (!item.IsUnidentified && (item.IsPotion || item.GetRawItemType() == RawItemType.GeneralUtility || item.IsMiscItem))
            {
                return false;
            }

            if (item.ItemQualityLevel >= ItemQuality.Legendary)
            {
                if (Core.Settings.Items.DropInTownMode == DropInTownOption.All)
                {
                    Core.Logger.Verbose($"Should Drop {item.Name} - Setting='{Core.Settings.Items.DropInTownMode}'");
                    return true;
                }

                switch (scheduledAction)
                {
                    case ItemEvaluationType.Keep:

                        if (Core.Settings.Items.DropInTownMode == DropInTownOption.Keep)
                        {
                            Core.Logger.Verbose($"Should Drop {item.Name} - Setting='{Core.Settings.Items.DropInTownMode}' and item is scheduled for {scheduledAction}");
                            return true;
                        }

                        break;

                    case ItemEvaluationType.Salvage:
                    case ItemEvaluationType.Sell:

                        if (Core.Settings.Items.DropInTownMode == DropInTownOption.Vendor)
                        {
                            Core.Logger.Verbose($"Should Drop {item.Name} - Setting='{Core.Settings.Items.DropInTownMode}' and item is scheduled for {scheduledAction}");
                            return true;
                        }

                        break;
                }
            }

            return false;
        }

        public bool ShouldStash(ACDItem item)
        {
            if (Core.ProfileSettings.Options.ShouldKeepInBackpack(item.ActorSnoId))
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Profile Setting Keep in Backpack - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.Stats.IsAncient && Core.Settings.ItemList.AlwaysStashAncients)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Stashing due to ItemList setting: Always stash ancients - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (item.Stats.IsPrimalAncient && Core.Settings.ItemList.AlwaysStashPrimalAncients)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Stashing due to ItemList setting: Always stash primal ancients - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            // 451002, //sir williams - 451002 (TentacleBear_C_Unique_Cosmetic_02)
            // portrait - 410998 (Cosmetic_Portrait_Frame_1)
            if (item.InternalNameLowerCase().Contains("cosmetic"))
            {
                return true;
            }

            if (item.IsProtected())
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Not stashing due to item being in a protected slot - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (BrainBehavior.GreaterRiftInProgress ||
                !Core.Settings.Items.KeepLegendaryUnid &&
                Core.Player.ParticipatingInTieredLootRun)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Not stashing due to inventory locked, keep unidentified setting or participating in loot run - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.GetRawItemType() == RawItemType.TreasureBag)
            {
                return Core.Settings.Items.StashTreasureBags;
            }

            if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
            {
                return true;
            }

            if (GameData.TransmogTable.Contains(item.GameBalanceId) ||
                item.InternalName.StartsWith("Transmog") ||
                item.ActorSnoId == 110952) //Rakanishu's Blade
            {
                bool setting = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TransmogWhites);
                s_logger.Debug($"[{nameof(ShouldStash)}] Transmog found, ShouldStash: {setting} - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return setting;
            }

            if (item.GetRawItemType() == RawItemType.CosmeticPet)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Pet found, stashing it - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }
            if (item.GetRawItemType() == RawItemType.CosmeticWings)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Wings found, stashing it - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }
            if (item.GetRawItemType() == RawItemType.CosmeticPennant)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Pennant found, stashing it - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }
            if (item.GetRawItemType() == RawItemType.CosmeticPortraitFrame)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Portrait found, stashing it - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.RottenMushroom) &&
                item.ActorSnoId == (int)SNOActor.A1_BlackMushroom)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Rotten Mushroom found, stashing it - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (GameData.HerdingMatsSnoIds.Contains(item.ActorSnoId))
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] Staff of Herding material found, stashing it - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            // Now look for Misc items we might want to keep
            TrinityItemType trinityItemType = item.GetTrinityItemType();
            TrinityItemBaseType trinityBaseType = item.GetTrinityItemBaseType();
            ItemBaseType baseType = item.GetItemBaseType();
            ItemType itemType = item.GetItemType();

            // Keep any high gems placed in backpack while levelling, so we can socket items with them.
            if (item.IsGem &&
                item.Stats.GemQuality >= GemQuality.Marquise &&
                ZetaDia.Me.Level < 70)
            {
                return false;
            }

            bool isHandledLegendaryType = baseType == ItemBaseType.Armor ||
                                          baseType == ItemBaseType.Jewelry ||
                                          baseType == ItemBaseType.Weapon ||
                                          item.IsPotion;
            if (item.ItemQualityLevel >= ItemQuality.Legendary && isHandledLegendaryType)
            {
                /* Stash items that hasn't had their legendary power extracted if the CubeExtractOption is set, if we have no currency for extracting it we can assume it would have been salvaged instead.
                if (Core.Settings.KanaisCube.ExtractLegendaryPowers != CubeExtractOption.None && !Core.Inventory.Currency.HasCurrency(TransmuteRecipe.ExtractLegendaryPower))
                {
                    Item legendaryItem = Legendary.GetItem(item);

                    if (legendaryItem != null && legendaryItem.LegendaryAffix != "" &&
                        Core.Inventory.Stash.ByActorSno(legendaryItem.Id) != null &&
                        !ZetaDia.Storage.PlayerDataManager.ActivePlayerData.KanaisPowersExtractedActorSnoIds.Contains(
                            legendaryItem.Id))
                    {
                        Core.Logger.Debug($"Stashing due to legendary power hasn't been extracted yet.");
                        return true;
                    }
                }*/

                if (Core.Settings.Items.LegendaryMode == LegendaryMode.Ignore)
                {
                    s_logger.Debug($"[{nameof(ShouldStash)}] TRASHING: Ignore Legendary - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return false;
                }

                if (Core.Settings.Items.LegendaryMode == LegendaryMode.AlwaysStash)
                {
                    s_logger.Debug($"[{nameof(ShouldStash)}] STASHING: Always Stash Legendary - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return true;
                }

                if (Core.Settings.Items.LegendaryMode == LegendaryMode.StashAncients)
                {
                    if (item.Stats.IsAncient)
                    {
                        s_logger.Debug($"[{nameof(ShouldStash)}] STASHING: Only Stash Ancients - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return true;
                    }
                    s_logger.Debug($"[{nameof(ShouldStash)}] TRASHING: Only Stash Ancients - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return false;
                }

                if (Core.Settings.ItemList.AlwaysTrashNonAncients && !item.Stats.IsAncient)
                {
                    s_logger.Debug($"[{nameof(ShouldStash)}] TRASHING: ItemList Option - Always Sell/Salvage Non-Ancients - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return false;
                }
            }

            bool isEquipment = trinityBaseType == TrinityItemBaseType.Armor ||
                               trinityBaseType == TrinityItemBaseType.Jewelry ||
                               trinityBaseType == TrinityItemBaseType.Offhand ||
                               trinityBaseType == TrinityItemBaseType.WeaponOneHand ||
                               trinityBaseType == TrinityItemBaseType.WeaponRange ||
                               trinityBaseType == TrinityItemBaseType.WeaponTwoHand;

            if (itemType == ItemType.KeystoneFragment)
            {
                return true;
            }

            if (trinityItemType == TrinityItemType.HoradricCache)
            {
                return false;
            }

            if (item.IsUnidentified)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Unidentified Items - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.StaffOfHerding)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Staff of Herding - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.CraftingMaterial || item.IsCraftingReagent)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Craft Materials - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.Emerald ||
                trinityItemType == TrinityItemType.Amethyst ||
                trinityItemType == TrinityItemType.Topaz ||
                trinityItemType == TrinityItemType.Ruby ||
                trinityItemType == TrinityItemType.Diamond)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Gems - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }
            if (trinityItemType == TrinityItemType.CraftTome)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Tomes - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }
            if (trinityItemType == TrinityItemType.InfernalKey)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Infernal Key - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.HealthPotion)
            {
                ACDItem equippedPotion = Core.Player.EquippedHealthPotion;
                if (equippedPotion == null)
                {
                    s_logger.Debug($"[{nameof(ShouldStash)}] Potion being stashed because it's not equiped - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return true;
                }
                if (equippedPotion.AnnId == item.AnnId)
                {
                    s_logger.Debug($"[{nameof(ShouldStash)}] Don't stash equipped potion - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                    return false;
                }
            }

            if (trinityItemType == TrinityItemType.CraftingPlan &&
                item.ItemQualityLevel >= ItemQuality.Legendary)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Legendary Plans - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.ConsumableAddSockets)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Ramaladni's Gift - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.PortalDevice)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Machines - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.UberReagent)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Uber Reagents - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (trinityItemType == TrinityItemType.TieredLootrunKey)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Tiered Rift Keys - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (trinityItemType == TrinityItemType.CraftingPlan)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Plans - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (item.ItemQualityLevel <= ItemQuality.Superior &&
                (isEquipment ||
                 trinityBaseType == TrinityItemBaseType.FollowerItem))
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] TRASHING: Salvage White - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }
            if (item.ItemQualityLevel >= ItemQuality.Magic1 &&
                item.ItemQualityLevel <= ItemQuality.Magic3 &&
                (isEquipment ||
                 trinityBaseType == TrinityItemBaseType.FollowerItem))
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] TRASHING: Salvage Magic - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.ItemQualityLevel >= ItemQuality.Rare4 &&
                item.ItemQualityLevel <= ItemQuality.Rare6 &&
                (isEquipment ||
                 trinityBaseType == TrinityItemBaseType.FollowerItem))
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] TRASHING: Salvage Rare - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return false;
            }

            if (item.ItemQualityLevel >= ItemQuality.Legendary &&
                Core.Settings.Items.LegendaryMode == LegendaryMode.ItemList &&
                (item.GetIsEquipment() ||
                 trinityBaseType == TrinityItemBaseType.FollowerItem ||
                 item.IsPotion))
            {
                bool result = ItemListEvaluator.ShouldStashItem(item);
                s_logger.Debug($"[{nameof(ShouldStash)}] ItemEvaluator.ShouldStashItem: {result} - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");

                return result;
            }

            if (item.ItemQualityLevel >= ItemQuality.Legendary)
            {
                s_logger.Debug($"[{nameof(ShouldStash)}] AUTOKEEP: Legendaries - Item: \"{item.Name}\", InternalName: \"{item.InternalName}\", Sno: 0x{item.ActorSnoId:x8}, GBId: 0x{item.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            // If we reached this point, then we found no reason to keep the item!
            return false;
        }

        public bool ShouldSalvage(ACDItem item)
        {
            string reason = string.Empty;
            try
            {
                if (item.IsProtected())
                {
                    reason = "Protected";
                    return false;
                }

                if (item.GetIsCosmeticItem())
                {
                    reason = "Cosmetic";
                    return false;
                }

                if (item.IsUnidentified)
                {
                    reason = "Not Identified";
                    return false;
                }

                if (item.Stats.IsAncient &&
                    Core.Settings.ItemList.AlwaysStashAncients)
                {
                    reason = "ItemList Stash Ancients";
                    return false;
                }

                if (item.Stats.IsPrimalAncient &&
                    Core.Settings.ItemList.AlwaysStashPrimalAncients)
                {
                    reason = "ItemList Stash Primal Ancients";
                    return false;
                }

                if (BrainBehavior.GreaterRiftInProgress ||
                    !Core.Settings.Items.KeepLegendaryUnid && Core.Player.ParticipatingInTieredLootRun)
                {
                    reason = "Rift Locked Inventory";
                    return false;
                }

                if (Core.ProfileSettings.Options.ShouldKeepInBackpack(item.ActorSnoId))
                {
                    reason = "Profile Setting Keep in Backpack";
                    return false;
                }

                if (!item.GetIsSalvageable())
                {
                    reason = "Not Salvagable";
                    return false;
                }

                if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
                {
                    reason = "Vanity Item";
                    return false;
                }

                if (item.GetItemType() == ItemType.KeystoneFragment)
                {
                    reason = "Rift Key";
                    return false;
                }

                if (item.GetTrinityItemType() == TrinityItemType.HealthPotion)
                {
                    ACDItem equippedPotion = Core.Player.EquippedHealthPotion;
                    if (equippedPotion == null)
                    {
                        Core.Logger.Debug("Potion being kept because an equipped potion was not found.");
                        return false;
                    }
                    if (equippedPotion.AnnId == item.AnnId)
                    {
                        reason = "Equipped Potion";
                        return false;
                    }
                }

                if (!Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TransmogWhites) &&
                    GameData.TransmogTable.Contains(item.GameBalanceId) ||
                    item.InternalName.StartsWith("Transmog") ||
                    item.ActorSnoId == 110952) //Rakanishu's Blade
                {
                    reason = "Transmog Setting";
                    return true;
                }



                reason = "Default";
                switch (item.GetTrinityItemType())
                {
                    case TrinityItemType.HealthPotion:
                        return true;
                }
                switch (item.GetTrinityItemBaseType())
                {
                    case TrinityItemBaseType.WeaponRange:
                    case TrinityItemBaseType.WeaponOneHand:
                    case TrinityItemBaseType.WeaponTwoHand:
                    case TrinityItemBaseType.Armor:
                    case TrinityItemBaseType.Offhand:
                    case TrinityItemBaseType.Jewelry:
                    case TrinityItemBaseType.FollowerItem:
                        return true;

                    case TrinityItemBaseType.Gem:
                    case TrinityItemBaseType.Misc:
                    case TrinityItemBaseType.Unknown:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"Exception in TrinitySalvage Evaluation for {item.Name} ({item.ActorSnoId}) InternalName={item.InternalName} Quality={item.ItemQualityLevel} Ancient={item.Stats.IsAncient} Identified={!item.IsUnidentified} RawItemType={item.GetRawItemType()} {ex}");
            }
            finally
            {
                Core.Logger.Debug($"Salvage Evaluation for: {item.Name} ({item.ActorSnoId}) Reason={reason} InternalName={item.InternalName} Quality={item.ItemQualityLevel} Ancient={item.Stats.IsAncient} Identified={!item.IsUnidentified} RawItemType={item.GetRawItemType()}");
            }
            return false;
        }

        public bool ShouldSell(ACDItem item)
        {
            string reason = string.Empty;
            try
            {
                if (item.IsProtected())
                {
                    reason = $"Protected Slot [col:{item.InventoryColumn}, row:{item.InventoryRow}]";
                    return false;
                }

                if (Core.ProfileSettings.Options.ShouldKeepInBackpack(item.ActorSnoId))
                {
                    reason = "Profile Setting Keep in Backpack";
                    return false;
                }

                if (item.Stats.IsAncient &&
                    Core.Settings.ItemList.AlwaysStashAncients)
                {
                    Core.Logger.Debug($"Not Selling due to ItemList setting - Always stash ancients. (col={item.InventoryColumn}, row={item.InventoryRow}). Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.GetRawItemType()}");
                    return false;
                }

                if (item.Stats.IsPrimalAncient &&
                    Core.Settings.ItemList.AlwaysStashPrimalAncients)
                {
                    Core.Logger.Debug($"Not Selling due to ItemList setting - Always stash primal ancients. (col={item.InventoryColumn}, row={item.InventoryRow}). Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.GetRawItemType()}");
                    return false;
                }

                if (item.GetIsCosmeticItem())
                {
                    return false;
                }

                //ActorId: 367009, Type: Item, Name: Griswold's Scribblings
                if (item.ActorSnoId == 367009)
                {
                    reason = "Special Case - Griswold's Scribblings";
                    return true;
                }

                if (item.ActorSnoId == 210432)
                {
                    reason = "Special Case - Never sell staff of cow";
                    return false;
                }

                if (item.IsUnidentified)
                {
                    reason = "Not Identified";
                    return false;
                }

                if (item.GetTrinityItemType() == TrinityItemType.HealthPotion)
                {
                    ACDItem equippedPotion = Core.Player.EquippedHealthPotion;
                    if (equippedPotion == null)
                    {
                        Core.Logger.Debug($"Legendary Potion {item.Name} ({item.ActorSnoId}) being kept because an equipped potion was not found.");
                        return false;
                    }
                    if (equippedPotion.AnnId == item.AnnId)
                    {
                        reason = "Equipped Potion";
                        return false;
                    }
                }

                if (item.GetIsEquipment() &&
                    item.RequiredLevel <= 1)
                {
                    reason = "Unable to salvage level 1 items";
                    return true;
                }

                if (BrainBehavior.GreaterRiftInProgress ||
                    !Core.Settings.Items.KeepLegendaryUnid &&
                    Core.Player.ParticipatingInTieredLootRun)
                {
                    reason = "Rift Locked Inventory";
                    return false;
                }

                if (item.IsVendorBought)
                {
                    reason = "Unable to salvage vendor bought items";
                    return false;
                }

                if (item.IsGem && item.Stats.GemQuality >= GemQuality.Marquise &&
                    ZetaDia.Me.Level < 70)
                {
                    reason = "auto-keep high level gems";
                    return false;
                }

                if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
                {
                    reason = "Vantity item";
                    return false;
                }

                if (item.GetItemType() == ItemType.KeystoneFragment)
                {
                    reason = "Rift Key";
                    return false;
                }

                if (item.GetItemType() == ItemType.HoradricCache)
                {
                    reason = "HoradricCache";
                    return false;
                }

                reason = "Default";
                switch (item.GetTrinityItemBaseType())
                {
                    case TrinityItemBaseType.WeaponRange:
                    case TrinityItemBaseType.WeaponOneHand:
                    case TrinityItemBaseType.WeaponTwoHand:
                    case TrinityItemBaseType.Armor:
                    case TrinityItemBaseType.Offhand:
                    case TrinityItemBaseType.Jewelry:
                    case TrinityItemBaseType.FollowerItem:
                        return true;

                    case TrinityItemBaseType.Gem:
                    case TrinityItemBaseType.Misc:
                        if (item.GetTrinityItemType() == TrinityItemType.CraftingPlan)
                        {
                            return true;
                        }

                        if (item.GetTrinityItemType() == TrinityItemType.CraftingMaterial)
                        {
                            return true;
                        }

                        return false;

                    case TrinityItemBaseType.Unknown:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"Exception in TrinitySell Evaluation for {item.Name} ({item.ActorSnoId}) InternalName={item.InternalName} Quality={item.ItemQualityLevel} Ancient={item.Stats.IsAncient} Identified={!item.IsUnidentified} RawItemType={item.GetRawItemType()} {ex}");
            }
            finally
            {
                Core.Logger.Debug($"Sell Evaluation for: {item.Name} ({item.ActorSnoId}) Reason={reason} InternalName={item.InternalName} Quality={item.ItemQualityLevel} Ancient={item.Stats.IsAncient} Identified={!item.IsUnidentified} RawItemType={item.GetRawItemType()}");
            }
            return false;
        }

        private static int _lastBackPackCount;
        private static int _lastProtectedSlotsCount;
        private static Vector2 _lastBackPackLocation = new Vector2(-2, -2);

        public static readonly Vector2 NoFreeSlot = new Vector2(-1, -1);

        internal static void ResetBackPackCheck()
        {
            _lastBackPackCount = -1;
            _lastProtectedSlotsCount = -1;
            _lastBackPackLocation = new Vector2(-2, -2);
            LastCheckBackpackDurability = DateTime.MinValue;
        }

        public static DateTime LastCheckBackpackDurability { get; set; }

        public static bool IsAnyTwoSlotBackpackLocation
        {
            get
            {
                Vector2 validLocation = FindBackpackLocation(true, true);
                return validLocation.X >= 0 && validLocation.Y >= 0;
            }
        }

        private static readonly CacheField<bool> _isValidTwoSlotBackpackLocation = new CacheField<bool>(UpdateSpeed.Fast);

        public static bool CachedIsValidTwoSlotBackpackLocation => _isValidTwoSlotBackpackLocation.GetValue(IsValidTwoSlotBackpackLocation);

        public static bool IsValidTwoSlotBackpackLocation()
        {
            Vector2 validLocation = FindBackpackLocation(true, false);
            return validLocation.X >= 0 && validLocation.Y >= 0;
        }

        public bool IsBackpackFull => !IsValidTwoSlotBackpackLocation();

        public Vector2 FindBackpackSlot(bool twoSlot)
        {
            return FindBackpackLocation(twoSlot, true);
        }

        internal static Vector2 FindBackpackLocation(bool isOriginalTwoSlot, bool forceRefresh = false)
        {
            using (new PerformanceLogger("FindValidBackpackLocation"))
            {
                try
                {
                    if (!forceRefresh &&
                        _lastBackPackLocation != new Vector2(-2, -2) &&
                        _lastBackPackLocation != new Vector2(-1, -1) &&
                        _lastBackPackCount == Core.Inventory.BackpackItemCount &&
                        _lastProtectedSlotsCount == CharacterSettings.Instance.ProtectedBagSlots.Count)
                    {
                        return _lastBackPackLocation;
                    }

                    bool[,] backpackSlotBlocked = new bool[10, 6];

                    int freeBagSlots = 60;

                    if (!forceRefresh)
                    {
                        _lastProtectedSlotsCount = CharacterSettings.Instance.ProtectedBagSlots.Count;
                        _lastBackPackCount = Core.Inventory.BackpackItemCount;
                    }

                    // Block off the entire of any "protected bag slots"
                    foreach (InventorySquare square in CharacterSettings.Instance.ProtectedBagSlots)
                    {
                        backpackSlotBlocked[square.Column, square.Row] = true;
                        freeBagSlots--;
                    }

                    // Map out all the items already in the backpack
                    foreach (ACDItem item in InventoryManager.Backpack)
                    {
                        if (!item.IsValid)
                        {
                            s_logger.Error($"[{nameof(FindBackpackLocation)}] Invalid backpack item detected! Marking down two Slots!");
                            freeBagSlots -= 2;
                            continue;
                        }
                        int row = item.InventoryRow;
                        int col = item.InventoryColumn;

                        if (row < 0 || row > 5)
                        {
                            s_logger.Error($"[{nameof(FindBackpackLocation)}] Item {item.Name} ({item.InternalName}) reports invalid backpack row {item.InventoryRow}!");
                            continue;
                        }

                        if (col < 0 || col > 9)
                        {
                            s_logger.Error($"[{nameof(FindBackpackLocation)}] Item {item.Name} ({item.InternalName}) reports invalid backpack column {item.InventoryColumn}!");
                            continue;
                        }

                        // Slot is already protected, don't double count
                        if (!backpackSlotBlocked[col, row])
                        {
                            backpackSlotBlocked[col, row] = true;
                            freeBagSlots--;
                        }

                        if (!item.IsTwoSquareItem)
                        {
                            continue;
                        }

                        if (row + 1 > 5)
                        {
                            s_logger.Debug($"[{nameof(FindBackpackLocation)}] Two square Item but row is invalid!");
                            continue;
                        }

                        // Slot is already protected, don't double count
                        if (backpackSlotBlocked[col, row + 1])
                        {
                            continue;
                        }

                        freeBagSlots--;
                        backpackSlotBlocked[col, row + 1] = true;
                    }

                    bool noFreeSlots = freeBagSlots < 1;
                    int unprotectedSlots = 60 - _lastProtectedSlotsCount;

                    // Use count of Unprotected slots if FreeBagSlots is higher than unprotected slots
                    int minFreeSlots = Core.Player.IsInTown ?
                        Math.Min(FreeBagSlotsInTown, unprotectedSlots) :
                        Math.Min(FreeBagSlots, unprotectedSlots);

                    // free bag slots is less than required
                    if (noFreeSlots || freeBagSlots < minFreeSlots && !forceRefresh)
                    {
                        Core.Logger.Debug("Free Bag Slots is less than required. FreeSlots={0}, FreeBagSlots={1} FreeBagSlotsInTown={2} IsInTown={3} Protected={4} BackpackCount={5}",
                            freeBagSlots, FreeBagSlots, FreeBagSlotsInTown, Core.Player.IsInTown,
                            _lastProtectedSlotsCount, _lastBackPackCount);
                        _lastBackPackLocation = NoFreeSlot;
                        return _lastBackPackLocation;
                    }

                    // 10 columns
                    Vector2 pos;
                    for (int col = 0; col <= 9; col++)
                    {
                        // 6 rows
                        for (int row = 0; row <= 5; row++)
                        {
                            // Slot is blocked, skip
                            if (backpackSlotBlocked[col, row])
                            {
                                continue;
                            }

                            // Not a two slotitem, slot not blocked, use it!
                            if (!isOriginalTwoSlot)
                            {
                                pos = new Vector2(col, row);
                                if (!forceRefresh)
                                {
                                    _lastBackPackLocation = pos;
                                }
                                return pos;
                            }

                            // Is a Two Slot, Can't check for 2 slot items on last row
                            if (row == 5)
                            {
                                continue;
                            }

                            // Is a Two Slot, check row below
                            if (backpackSlotBlocked[col, row + 1])
                            {
                                continue;
                            }

                            pos = new Vector2(col, row);
                            if (!forceRefresh)
                            {
                                _lastBackPackLocation = pos;
                            }
                            return pos;
                        }
                    }

                    // no free slot
                    s_logger.Debug($"[{nameof(FindBackpackLocation)}] No Free slots!");

                    pos = NoFreeSlot;
                    if (!forceRefresh)
                    {
                        _lastBackPackLocation = pos;
                    }

                    return pos;
                }
                catch (Exception ex)
                {
                    s_logger.Error($"[{nameof(FindBackpackLocation)}] Error:", ex);
                    return NoFreeSlot;
                }
            }
        }
    }
}
