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
            var gbi = item.GameBalanceId;
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
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.DeathsBreath);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: DeathsBreath - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (item.ActorSnoId == SNOActor.A1_BlackMushroom)
            {
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.RottenMushroom);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: Rotten Mushroom - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (gbi == GameData.ItemGameBalanceIds.ArcaneDust)
            {
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.ArcaneDust);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: ArcaneDust - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (gbi == GameData.ItemGameBalanceIds.VeiledCrystal)
            {
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.VeiledCrystals);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: VeiledCrystals - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (gbi == GameData.ItemGameBalanceIds.ReusableParts)
            {
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.ReusableParts);
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
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.StaffOfHeardingParts);
                s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: StaffOfHeardingParts - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return result;
            }

            if (GameData.TransmogTable.Contains(gbi) ||
                item.InternalName.StartsWith("Transmog") ||
                item.ActorSnoId == SNOActor.Sword_norm_unique_03) //Rakanishu's Blade
            {
                s_logger.Debug($"[{nameof(ShouldPickup)}] PICKUP: Transmog - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                return true;
            }

            if (tit == TrinityItemType.TieredLootrunKey ||
                tit == TrinityItemType.LootRunKey)
            {
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TieredLootrunKey);
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
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.KeywardenIngredients);
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
                var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.BloodShards);
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
                    var result = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.CraftingPlans);
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
                        var result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Grey);
                        s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: PickupItemQualities.Grey - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return result;
                    }

                    if (tiq == TrinityItemQuality.Common)
                    {
                        var result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.White);
                        s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: PickupItemQualities.White - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return result;
                    }

                    if (tiq == TrinityItemQuality.Magic)
                    {
                        var result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Blue);
                        s_logger.Debug($"[{nameof(ShouldPickup)}] {(result ? "PICKUP" : "IGNORE")}: PickupItemQualities.Blue - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}, RawItemType: {item.GetRawItemType()}");
                        return result;
                    }

                    if (tiq == TrinityItemQuality.Rare)
                    {
                        var result = Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Yellow);
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
                s_logger.Debug($"[{nameof(ShouldDrop)}] IGNORE: IsProtected || IsAccountBound - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.IsGem || item.IsCraftingReagent || item.GetTrinityItemType() == TrinityItemType.CraftingPlan)
            {
                s_logger.Debug($"[{nameof(ShouldDrop)}] IGNORE: IsGem || IsCraftingReagent || CraftingPlan - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (!item.IsUnidentified && (item.IsPotion || item.GetRawItemType() == RawItemType.GeneralUtility || item.IsMiscItem))
            {
                s_logger.Debug($"[{nameof(ShouldDrop)}] IGNORE: !IsUnidentified && (IsPotion || GeneralUtility || IsMiscItem) - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.ItemQualityLevel >= ItemQuality.Legendary)
            {
                if (Core.Settings.Items.DropInTownMode == DropInTownOption.All)
                {
                    s_logger.Debug($"[{nameof(ShouldDrop)}] DROP: DropInTownMode == All - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return true;
                }

                switch (scheduledAction)
                {
                    case ItemEvaluationType.Keep:

                        if (Core.Settings.Items.DropInTownMode == DropInTownOption.Keep)
                        {
                            s_logger.Debug($"[{nameof(ShouldDrop)}] DROP: DropInTownMode == Keep - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                            return true;
                        }

                        break;

                    case ItemEvaluationType.Salvage:
                    case ItemEvaluationType.Sell:

                        if (Core.Settings.Items.DropInTownMode == DropInTownOption.Vendor)
                        {
                            s_logger.Debug($"[{nameof(ShouldDrop)}] DROP: DropInTownMode == Vendor - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                            return true;
                        }

                        break;
                }
            }
            s_logger.Debug($"[{nameof(ShouldDrop)}] IGNORE: Default - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
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
                item.ActorSnoId == SNOActor.Sword_norm_unique_03) //Rakanishu's Blade
            {
                var setting = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TransmogWhites);
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
                item.ActorSnoId == SNOActor.A1_BlackMushroom)
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

            var isHandledLegendaryType = baseType == ItemBaseType.Armor ||
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

            var isEquipment = trinityBaseType == TrinityItemBaseType.Armor ||
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
                var result = ItemListEvaluator.ShouldStashItem(item);
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
            if (item.IsProtected())
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: IsProtected - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.GetIsCosmeticItem())
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: IsCosmeticItem - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.IsUnidentified)
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: IsUnidentified - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.Stats.IsAncient &&
                Core.Settings.ItemList.AlwaysStashAncients)
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: IsAncient && AlwaysStashAncients - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.Stats.IsPrimalAncient &&
                Core.Settings.ItemList.AlwaysStashPrimalAncients)
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: IsPrimalAncient && AlwaysStashPrimalAncients - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (BrainBehavior.GreaterRiftInProgress ||
                !Core.Settings.Items.KeepLegendaryUnid && Core.Player.ParticipatingInTieredLootRun)
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: GreaterRiftInProgress || !KeepLegendaryUnid && ParticipatingInTieredLootRun - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (Core.ProfileSettings.Options.ShouldKeepInBackpack(item.ActorSnoId))
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: ShouldKeepInBackpack - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (!item.GetIsSalvageable())
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: GetIsSalvageable - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
            {
                s_logger.Debug($"[{nameof(ShouldDrop)}] IGNORE: VanityItems.Any() - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.GetItemType() == ItemType.KeystoneFragment)
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: KeystoneFragment - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.GetTrinityItemType() == TrinityItemType.HealthPotion)
            {
                ACDItem equippedPotion = Core.Player.EquippedHealthPotion;
                if (equippedPotion == null)
                {
                    s_logger.Debug($"[{nameof(ShouldDrop)}] IGNORE: EquippedHealthPotion == null - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return false;
                }
                if (equippedPotion.AnnId == item.AnnId)
                {
                    s_logger.Debug($"[{nameof(ShouldDrop)}] IGNORE: EquippedHealthPotion - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return false;
                }
            }

            if (!Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TransmogWhites) &&
                GameData.TransmogTable.Contains(item.GameBalanceId) ||
                item.InternalName.StartsWith("Transmog") ||
                item.ActorSnoId == SNOActor.Sword_norm_unique_03) //Rakanishu's Blade
            {
                s_logger.Debug($"[{nameof(ShouldSalvage)}] SALVAGE: Transmog - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return true;
            }

            switch (item.GetTrinityItemType())
            {
                case TrinityItemType.HealthPotion:
                    s_logger.Debug($"[{nameof(ShouldSalvage)}] SALVAGE: HealthPotion - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
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
                    s_logger.Debug($"[{nameof(ShouldSalvage)}] SALVAGE: {item.GetTrinityItemBaseType()} - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return true;

                case TrinityItemBaseType.Gem:
                case TrinityItemBaseType.Misc:
                case TrinityItemBaseType.Unknown:
                    s_logger.Debug($"[{nameof(ShouldSalvage)}] IGNORE: {item.GetTrinityItemBaseType()} - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return false;
            }
            s_logger.Debug($"[{nameof(ShouldSalvage)}] SALVAGE: {item.GetTrinityItemBaseType()} - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
            return false;
        }

        public bool ShouldSell(ACDItem item)
        {
            if (item.IsProtected())
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: IsProtected - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (Core.ProfileSettings.Options.ShouldKeepInBackpack(item.ActorSnoId))
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: ShouldKeepInBackpack - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.Stats.IsAncient &&
                Core.Settings.ItemList.AlwaysStashAncients)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: IsAncient && AlwaysStashAncients - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.Stats.IsPrimalAncient &&
                Core.Settings.ItemList.AlwaysStashPrimalAncients)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: IsPrimalAncient && AlwaysStashPrimalAncients - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.GetIsCosmeticItem())
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: IsCosmetic - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            //ActorId: 367009, Type: Item, Name: Griswold's Scribblings
            if (item.ActorSnoId == SNOActor.CraftingReagent_Legendary_Unique_Sword_1H_019_x1)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] SELL: Special Case - Griswold's Scribblings - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return true;
            }

            if (item.ActorSnoId == SNOActor.StaffOfCow)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: Staff of Cow - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.IsUnidentified)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: IsUnidentified - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.GetTrinityItemType() == TrinityItemType.HealthPotion)
            {
                ACDItem equippedPotion = Core.Player.EquippedHealthPotion;
                if (equippedPotion == null)
                {
                    s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: EquippedHealthPotion == null - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return false;
                }
                if (equippedPotion.AnnId == item.AnnId)
                {
                    s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: EquippedHealthPotion - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return false;
                }
            }

            if (item.GetIsEquipment() &&
                item.RequiredLevel <= 1)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] SELL: IsEquipment && RequiredLevel <= 1 - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return true;
            }

            if (BrainBehavior.GreaterRiftInProgress ||
                !Core.Settings.Items.KeepLegendaryUnid && Core.Player.ParticipatingInTieredLootRun)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: GreaterRiftInProgress || !KeepLegendaryUnid && ParticipatingInTieredLootRun - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.IsVendorBought)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: IsVendorBought - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.IsGem && item.Stats.GemQuality >= GemQuality.Marquise &&
                ZetaDia.Me.Level < 70)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: IsGem && GemQuality >= Marquise && Me.Level < 70 - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: VanityItems.Any() - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.GetItemType() == ItemType.KeystoneFragment)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: KeystoneFragment - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
            }

            if (item.GetItemType() == ItemType.HoradricCache)
            {
                s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: HoradricCache - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                return false;
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
                    s_logger.Debug($"[{nameof(ShouldSell)}] SELL: {item.GetTrinityItemBaseType()} - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return true;

                case TrinityItemBaseType.Gem:
                case TrinityItemBaseType.Misc:
                    if (item.GetTrinityItemType() == TrinityItemType.CraftingPlan)
                    {
                        s_logger.Debug($"[{nameof(ShouldSell)}] SELL: {item.GetTrinityItemBaseType()} && CraftingPlan - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                        return true;
                    }

                    if (item.GetTrinityItemType() == TrinityItemType.CraftingMaterial)
                    {
                        s_logger.Debug($"[{nameof(ShouldSell)}] SELL: {item.GetTrinityItemBaseType()} && CrafingMaterial - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                        return true;
                    }

                    s_logger.Debug($"[{nameof(ShouldSell)}] IGNORE: {item.GetTrinityItemBaseType()} - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return false;

                case TrinityItemBaseType.Unknown:
                    s_logger.Debug($"[{nameof(ShouldSell)}] SELL: {item.GetTrinityItemBaseType()} - Item: \"{item?.Name}\", InternalName: \"{item?.InternalName}\", Sno: 0x{item?.ActorSnoId:x8}, GBId: 0x{item?.GameBalanceId:x8}");
                    return false;
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

                    var backpackSlotBlocked = new bool[10, 6];

                    var freeBagSlots = 60;

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
                        var row = item.InventoryRow;
                        var col = item.InventoryColumn;

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

                    var noFreeSlots = freeBagSlots < 1;
                    var unprotectedSlots = 60 - _lastProtectedSlotsCount;

                    // Use count of Unprotected slots if FreeBagSlots is higher than unprotected slots
                    var minFreeSlots = Core.Player.IsInTown ?
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
                    for (var col = 0; col <= 9; col++)
                    {
                        // 6 rows
                        for (var row = 0; row <= 5; row++)
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
