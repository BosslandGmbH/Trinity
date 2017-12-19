using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.Settings.ItemList;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Combat
{
    public interface ILootProvider
    {
        bool ShouldDrop(TrinityItem item, ItemEvaluationType scheduledAction);

        bool ShouldStash(TrinityItem item);

        bool ShouldSalvage(TrinityItem item);

        bool ShouldPickup(TrinityItem item);

        bool ShouldSell(TrinityItem item);

        bool IsBackpackFull { get; }

        Vector2 FindBackpackSlot(bool twoSlot);
    }

    public class DefaultLootProvider : ILootProvider
    {
        /// <summary>
        ///  智能包裹整理条件
        /// </summary>
        public static bool CanVedonInRift
        {
            get
            {
                switch (Core.Rift.Quest.Step)
                {
                    case Trinity.Components.Adventurer.Game.Rift.RiftStep.UrshiSpawned:
                        return true;
                    case Trinity.Components.Adventurer.Game.Rift.RiftStep.Cleared:
                        return true;
                };
                return false;
            }
        }
        public static int FreeBagSlots => Core.Settings.SenExtend.EnableIntelligentFinishing ? Core.Settings.SenExtend.FreeBagSlots : 4;
        public static int FreeBagSlotsInTown => Core.Settings.SenExtend.EnableIntelligentFinishing ? Core.Settings.SenExtend.FreeBagSlotsInTown : 30;

        public bool ShouldPickup(TrinityItem item)
        {
            if (item == null || !item.IsValid)
            {
                Core.Logger.Debug($"无效物品 {item?.InternalName} Sno={item?.ActorSnoId} GbId={item?.GameBalanceId}");
                return false;
            }

            if (item.RawItemType == RawItemType.CosmeticPet)
            {
                Core.Logger.Log($"发现宠物! - 捡起它 {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }
            if (item.RawItemType == RawItemType.CosmeticWings)
            {
                Core.Logger.Log($"发现翅膀! - 捡起它 {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }
            if (item.RawItemType == RawItemType.CosmeticPennant)
            {
                Core.Logger.Log($"发现翅膀! - 捡起它 {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }
            if (item.RawItemType == RawItemType.CosmeticPortraitFrame)
            {
                Core.Logger.Log($"发现头像框! - 捡起它 {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }

            //if (item.InternalNameLowerCase.Contains("cosmetic"))
            //    return true;

            if (Core.Settings.Items.DisableLootingInCombat && TrinityCombat.IsInCombat && item.Distance > 8f)
                return false;

            if (Core.Settings.Items.DontPickupInTown && Core.Player.IsInTown && !item.IsItemAssigned)
                return false;

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
                return false;

            if (item.IsAncient && Core.Settings.ItemList.AlwaysStashAncients)
                return true;

            if (item.IsPrimalAncient && Core.Settings.ItemList.AlwaysStashPrimalAncients)
                return true;

            if (item.TrinityItemType == TrinityItemType.ConsumableAddSockets)
                return true;

            if (item.RawItemType == RawItemType.Book && Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.Lore))
                return true;

            if (item.RawItemType == RawItemType.Junk && Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.CultistPage))
                return true;
            // 死亡之息
            if (item.GameBalanceId == GameData.ItemGameBalanceIds.DeathsBreath)
                return Core.Rift.IsNephalemRift ? Core.Settings.SenExtend.IsPickDeathsBreath : true; //Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.DeathsBreath);
            // 
            if (item.ActorSnoId == (int)SNOActor.A1_BlackMushroom)
                return Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.RottenMushroom);
            // 奥术之尘
            if (item.GameBalanceId == GameData.ItemGameBalanceIds.ArcaneDust)
                return Core.Rift.IsNephalemRift ? Core.Settings.SenExtend.IsPickArcaneDust : true; //Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.ArcaneDust);
            // 萦雾水晶
            if (item.GameBalanceId == GameData.ItemGameBalanceIds.VeiledCrystal)
                return Core.Rift.IsNephalemRift ? Core.Settings.SenExtend.IsPickVeiledCrystal : true; // Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.VeiledCrystals);
            // 万用材料
            if (item.GameBalanceId == GameData.ItemGameBalanceIds.ReusableParts)
                return Core.Rift.IsNephalemRift ? Core.Settings.SenExtend.IsPickReusableParts : true; //Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.ReusableParts);

            // 遗忘之魂
            if (item.GameBalanceId == GameData.ItemGameBalanceIds.ForgottenSoul)
                return true;

            if (GameData.HerdingMatsSnoIds.Contains(item.ActorSnoId))
                return Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.StaffOfHeardingParts);

            if (GameData.TransmogTable.Contains(item.GameBalanceId) || item.InternalName.StartsWith("Transmog") || item.ActorSnoId == 110952) //Rakanishu's Blade
            {
                Core.Logger.Log($"发现幻化装备! - 拿起它 {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }

            if (item.TrinityItemType == TrinityItemType.TieredLootrunKey || item.TrinityItemType == TrinityItemType.LootRunKey)
                return Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TieredLootrunKey);

            if (item.TrinityItemType == TrinityItemType.HealthPotion && item.ItemQualityLevel >= ItemQuality.Legendary)
                return true;

            if (item.TrinityItemType == TrinityItemType.InfernalKey || item.TrinityItemType == TrinityItemType.PortalDevice)
                return Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.KeywardenIngredients);

            if (item.TrinityItemType == TrinityItemType.UberReagent)
                return true;

            if (item.TrinityItemType == TrinityItemType.HoradricRelic && Core.Player.BloodShards < Core.Player.MaxBloodShards)
                return Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.BloodShards);

            if (item.TrinityItemType == TrinityItemType.ProgressionGlobe)
                return true;

            if (item.TrinityItemType == TrinityItemType.CraftingMaterial)
                return true;

            switch (item.RawItemType)
            {
                case RawItemType.CraftingPlan:
                case RawItemType.CraftingPlan_Jeweler:
                case RawItemType.CraftingPlan_Smith:
                case RawItemType.CraftingPlanLegendary_Smith:
                case RawItemType.CraftingPlan_Mystic:
                case RawItemType.CraftingPlan_MysticTransmog:
                    return Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.CraftingPlans);
            }

            if (item.TrinityItemType == TrinityItemType.CraftingPlan && item.ItemQualityLevel >= ItemQuality.Legendary && Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.CraftingPlans))
            {
                return true;
            }

            if (item.Type == TrinityObjectType.BloodShard)
                return true;

            if (item.ItemType == ItemType.LegendaryGem)
                return true;

            if (item.ItemQualityLevel >= ItemQuality.Legendary)
                return true;

            switch (item.TrinityItemBaseType)
            {
                case TrinityItemBaseType.WeaponTwoHand:
                case TrinityItemBaseType.WeaponOneHand:
                case TrinityItemBaseType.WeaponRange:
                case TrinityItemBaseType.Armor:
                case TrinityItemBaseType.Offhand:
                case TrinityItemBaseType.Jewelry:
                case TrinityItemBaseType.FollowerItem:

                    if (item.TrinityItemQuality == TrinityItemQuality.Inferior)
                        return Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Grey);

                    if (item.TrinityItemQuality == TrinityItemQuality.Common)
                        return Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.White);

                    if (item.TrinityItemQuality == TrinityItemQuality.Magic)
                        return Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Blue);

                    if (item.TrinityItemQuality == TrinityItemQuality.Rare)
                        return Core.Settings.Items.PickupQualities.HasFlag(PickupItemQualities.Yellow);

                    return false;

                case TrinityItemBaseType.Gem:

                    if ((int)item.GemQuality < Core.Settings.Items.GemLevel ||
                        (item.TrinityItemType == TrinityItemType.Ruby && !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Ruby)) ||
                        (item.TrinityItemType == TrinityItemType.Emerald && !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Emerald)) ||
                        (item.TrinityItemType == TrinityItemType.Amethyst && !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Amethyst)) ||
                        (item.TrinityItemType == TrinityItemType.Topaz && !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Topaz)) ||
                        (item.TrinityItemType == TrinityItemType.Diamond && !Core.Settings.Items.GemTypes.HasFlag(TrinityGemType.Diamond)))
                    {
                        return false;
                    }
                    break;

                case TrinityItemBaseType.Misc:
                case TrinityItemBaseType.HealthGlobe:
                    return true;

                case TrinityItemBaseType.ProgressionGlobe:
                    return true;

                default:
                    return false;
            }

            return true;
        }

        public bool ShouldDrop(TrinityItem item, ItemEvaluationType scheduledAction)
        {
            if (item.IsProtected() || item.IsAccountBound)
                return false;

            if (item.IsGem || item.IsCraftingReagent || item.TrinityItemType == TrinityItemType.CraftingPlan)
                return false;

            if (!item.IsUnidentified && (item.IsPotion || item.RawItemType == RawItemType.GeneralUtility || item.IsMiscItem))
                return false;

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

        public bool ShouldStash(TrinityItem item)
        {
            if (Core.ProfileSettings.Options.ShouldKeepInBackpack(item.ActorSnoId))
            {
                Core.Logger.Debug($"Profile Setting Keep in Backpack - Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.RawItemType}");
                return false;
            }

            if (item.IsAncient && Core.Settings.ItemList.AlwaysStashAncients)
            {
                Core.Logger.Debug($"Stashing due to ItemList setting - Always stash ancients. (col={item.InventoryColumn}, row={item.InventoryRow}). Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.RawItemType}");
                return true;
            }

            if (item.IsPrimalAncient && Core.Settings.ItemList.AlwaysStashPrimalAncients)
            {
                Core.Logger.Debug($"Stashing due to ItemList setting - Always stash primal ancients. (col={item.InventoryColumn}, row={item.InventoryRow}). Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.RawItemType}");
                return true;
            }

            // 451002, //sir williams - 451002 (TentacleBear_C_Unique_Cosmetic_02)
            // portrait - 410998 (Cosmetic_Portrait_Frame_1)
            if (item.InternalNameLowerCase.Contains("cosmetic"))
                return true;

            if (item.IsProtected())
            {
                Core.Logger.Debug($"Not stashing due to item being in a protected slot (col={item.InventoryColumn}, row={item.InventoryRow}). Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.RawItemType}");
                return false;
            }
            // 智能包裹整理
            if (Core.Player.IsInventoryLockedForGreaterRift || !Core.Settings.Items.KeepLegendaryUnid && (Core.Player.ParticipatingInTieredLootRun && !CanVedonInRift))
            {
                Core.Logger.Debug($"Not stashing due to inventory locked, keep unidentified setting or participating in loot run. Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.RawItemType}");
                return false;
            }

            if (item.RawItemType == RawItemType.TreasureBag)
                return Core.Settings.Items.StashTreasureBags;

            if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
                return true;

            if (GameData.TransmogTable.Contains(item.GameBalanceId) || item.InternalName.StartsWith("Transmog") || item.ActorSnoId == 110952) //Rakanishu's Blade
            {
                var setting = Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TransmogWhites);
                Core.Logger.Log($"发现幻化装备! - 储存设置={setting} {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return setting;
            }

            if (item.RawItemType == RawItemType.CosmeticPet)
            {
                Core.Logger.Log($"发现宠物! - 储存它 {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }
            if (item.RawItemType == RawItemType.CosmeticWings)
            {
                Core.Logger.Log($"发现翅膀! - 储存它  {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }
            if (item.RawItemType == RawItemType.CosmeticPennant)
            {
                Core.Logger.Log($"发现翅膀! - 储存它  {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }
            if (item.RawItemType == RawItemType.CosmeticPortraitFrame)
            {
                Core.Logger.Log($"发现头像框! - 储存它 {item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId}");
                return true;
            }

            if (Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.RottenMushroom) && item.ActorSnoId == (int)SNOActor.A1_BlackMushroom)
            {
                Core.Logger.Log($"发现腐烂的蘑菇! - 储存设置. 物品={item.Name} 内部名={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} 原始类型={item.RawItemType}");
                return true;
            }

            if (GameData.HerdingMatsSnoIds.Contains(item.ActorSnoId))
            {
                Core.Logger.Log($"发现牧牛杖材料! - 储存设置. 物品={item.Name} 内部名={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} 原始类型={item.RawItemType}");
                return true;
            }

            // Now look for Misc items we might want to keep
            TrinityItemType tItemType = item.TrinityItemType;
            TrinityItemBaseType tBaseType = item.TrinityItemBaseType;

            // Keep any high gems placed in backpack while levelling, so we can socket items with them.
            if (item.IsGem && item.GemQuality >= GemQuality.Marquise && ZetaDia.Me.Level < 70)
            {
                return false;
            }

            var isHandledLegendaryType = item.ItemBaseType == ItemBaseType.Armor || item.ItemBaseType == ItemBaseType.Jewelry || item.ItemBaseType == ItemBaseType.Weapon || item.IsPotion;
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
                    Core.Logger.Log($"分解: 忽略传奇. 物品={item.Name} 内部名称={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} 原始类型={item.RawItemType}");
                    return false;
                }

                if (Core.Settings.Items.LegendaryMode == LegendaryMode.AlwaysStash)
                {
                    Core.Logger.Log($"储存: 总是存储传奇 物品={item.Name} 内部名称={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} 原始类型={item.RawItemType}");
                    return true;
                }

                if (Core.Settings.Items.LegendaryMode == LegendaryMode.StashAncients)
                {
                    if (item.IsAncient)
                    {
                        Core.Logger.Log($"储存: 只存储远古 物品={item.Name} 内部名称={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} 原始类型={item.RawItemType}");
                        return true;
                    }
                    Core.Logger.Log($"分解: 只存储远古 物品={item.Name} 内部名称={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} 原始类型={item.RawItemType}");
                    return false;
                }

                if (Core.Settings.ItemList.AlwaysTrashNonAncients && !item.IsAncient)
                {
                    Core.Logger.Log($"分解: 捡取列表设置 - 总是出售/分解非远古 物品={item.Name} 内部名称={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} 原始类型={item.RawItemType}");
                    return false;
                }
            }

            bool isEquipment = (tBaseType == TrinityItemBaseType.Armor ||
                tBaseType == TrinityItemBaseType.Jewelry ||
                tBaseType == TrinityItemBaseType.Offhand ||
                tBaseType == TrinityItemBaseType.WeaponOneHand ||
                tBaseType == TrinityItemBaseType.WeaponRange ||
                tBaseType == TrinityItemBaseType.WeaponTwoHand);

            if (item.ItemType == ItemType.KeystoneFragment)
            {
                return true;
            }

            if (item.TrinityItemType == TrinityItemType.HoradricCache)
                return false;

            if (item.IsUnidentified)
            {
                Core.Logger.Log("{0} [{1}] = (自动保存不明物品)", item.Name, item.InternalName);
                return true;
            }

            if (tItemType == TrinityItemType.StaffOfHerding)
            {
                Core.Logger.Log(LogCategory.ItemValuation, "{0} [{1}] [{2}] = (总是保存牧牛仗)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (tItemType == TrinityItemType.CraftingMaterial || item.IsCraftingReagent)
            {
                Core.Logger.Log(LogCategory.ItemValuation, "{0} [{1}] [{2}] = (总是保存锻造材料)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (tItemType == TrinityItemType.Emerald || tItemType == TrinityItemType.Amethyst || tItemType == TrinityItemType.Topaz || tItemType == TrinityItemType.Ruby || tItemType == TrinityItemType.Diamond)
            {
                Core.Logger.Log(LogCategory.ItemValuation, "{0} [{1}] [{2}] = (总是保存 宝石)", item.Name, item.InternalName, tItemType);
                return true;
            }
            if (tItemType == TrinityItemType.CraftTome)
            {
                Core.Logger.Log(LogCategory.ItemValuation, "{0} [{1}] [{2}] = (总是保存 书页)", item.Name, item.InternalName, tItemType);
                return true;
            }
            if (tItemType == TrinityItemType.InfernalKey)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (总是保存 炼狱装置)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (tItemType == TrinityItemType.HealthPotion)
            {
                var equippedPotion = Core.Player.EquippedHealthPotion;
                if (equippedPotion == null)
                {
                    Core.Logger.Debug("Potion being stashed because an equipped potion was not found.");
                    return true;
                }
                if (equippedPotion.AnnId == item.AnnId)
                {
                    Core.Logger.Debug($"{item.Name} [{item.InternalName}] [{tItemType}] = (dont stash equipped potion)");
                    return false;
                }
            }

            if (tItemType == TrinityItemType.CraftingPlan && item.ItemQualityLevel >= ItemQuality.Legendary)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (总是保存 传奇设计图)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (tItemType == TrinityItemType.ConsumableAddSockets)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (总是保存 拉玛兰迪的礼物)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (tItemType == TrinityItemType.PortalDevice)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (总是保存 机器)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (tItemType == TrinityItemType.UberReagent)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (总是保存 Uber试剂)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (tItemType == TrinityItemType.TieredLootrunKey)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (忽略有层数的秘境钥匙)", item.Name, item.InternalName, tItemType);
                return false;
            }

            if (tItemType == TrinityItemType.CraftingPlan)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (总是保存 计划书)", item.Name, item.InternalName, tItemType);
                return true;
            }

            if (item.ItemQualityLevel <= ItemQuality.Superior && (isEquipment || item.TrinityItemBaseType == TrinityItemBaseType.FollowerItem))
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (丢弃 白色材料)", item.Name, item.InternalName, tItemType);
                return false;
            }
            if (item.ItemQualityLevel >= ItemQuality.Magic1 && item.ItemQualityLevel <= ItemQuality.Magic3 && (isEquipment || item.TrinityItemBaseType == TrinityItemBaseType.FollowerItem))
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (丢弃 蓝色材料)", item.Name, item.InternalName, tItemType);
                return false;
            }

            if (item.ItemQualityLevel >= ItemQuality.Rare4 && item.ItemQualityLevel <= ItemQuality.Rare6 && (isEquipment || item.TrinityItemBaseType == TrinityItemBaseType.FollowerItem))
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (分解稀有)", item.Name, item.InternalName, tItemType);
                return false;
            }

            if (item.ItemQualityLevel >= ItemQuality.Legendary && Core.Settings.Items.LegendaryMode == LegendaryMode.ItemList && (item.IsEquipment || item.TrinityItemBaseType == TrinityItemBaseType.FollowerItem || item.IsPotion))
            {
                var result = ItemListEvaluator.ShouldStashItem(item);
                Core.Logger.Log("{0} [{1}] [{2}] = {3}", item.Name, item.InternalName, tItemType, "核对捡取列表=" + (result ? "保存" : "丢弃"));

                return result;
            }

            if (item.ItemQualityLevel >= ItemQuality.Legendary)
            {
                Core.Logger.Log("{0} [{1}] [{2}] = (总是保存 传奇)", item.Name, item.InternalName, tItemType);
                return true;
            }

            // If we reached this point, then we found no reason to keep the item!
            return false;
        }

        public bool ShouldSalvage(TrinityItem item)
        {
            var reason = string.Empty;
            try
            {
                if (item.IsProtected())
                {
                    reason = "Protected";
                    return false;
                }

                if (item.IsCosmeticItem)
                {
                    reason = "Cosmetic";
                    return false;
                }

                if (item.IsUnidentified)
                {
                    reason = "Not Identified";
                    return false;
                }

                if (item.IsAncient && Core.Settings.ItemList.AlwaysStashAncients)
                {
                    reason = "ItemList Stash Ancients";
                    return false;
                }

                if (item.IsPrimalAncient && Core.Settings.ItemList.AlwaysStashPrimalAncients)
                {
                    reason = "ItemList Stash Primal Ancients";
                    return false;
                }
                // 智能包裹整理
                if (Core.Player.IsInventoryLockedForGreaterRift || !Core.Settings.Items.KeepLegendaryUnid && (Core.Player.ParticipatingInTieredLootRun && !CanVedonInRift))
                {
                    reason = "Rift Locked Inventory";
                    return false;
                }

                if (Core.ProfileSettings.Options.ShouldKeepInBackpack(item.ActorSnoId))
                {
                    reason = "Profile Setting Keep in Backpack";
                    return false;
                }

                if (!item.IsSalvageable)
                {
                    reason = "Not Salvagable";
                    return false;
                }

                if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
                {
                    reason = "Vanity Item";
                    return false;
                }

                if (item.ItemType == ItemType.KeystoneFragment)
                {
                    reason = "Rift Key";
                    return false;
                }

                if (item.TrinityItemType == TrinityItemType.HealthPotion)
                {
                    var equippedPotion = Core.Player.EquippedHealthPotion;
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

                if (!Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.TransmogWhites) && (GameData.TransmogTable.Contains(item.GameBalanceId) || item.InternalName.StartsWith("Transmog") || item.ActorSnoId == 110952)) //Rakanishu's Blade
                {
                    reason = "Transmog Setting";
                    return true;
                }

 

                reason = "Default";
                switch (item.TrinityItemType)
                {
                    case TrinityItemType.HealthPotion:
                        return true;
                }
                switch (item.TrinityItemBaseType)
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
                Core.Logger.Error($"评估分解中的 {item.Name} ({item.ActorSnoId}) 异常 内部名称={item.InternalName} 品质={item.ItemQualityLevel} 远古={item.IsAncient} 鉴定={!item.IsUnidentified} 原始类型={item.RawItemType} {ex}");
            }
            finally
            {
                Core.Logger.Debug($"分解评估: {item.Name} ({item.ActorSnoId}) 理由={reason} 内部名称={item.InternalName} 品质={item.ItemQualityLevel} 远古={item.IsAncient} 鉴定={!item.IsUnidentified} 原始类型={item.RawItemType}");
            }
            return false;
        }

        public bool ShouldSell(TrinityItem item)
        {
            var reason = string.Empty;
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

                if (item.IsAncient && Core.Settings.ItemList.AlwaysStashAncients)
                {
                    Core.Logger.Debug($"Not Selling due to ItemList setting - Always stash ancients. (col={item.InventoryColumn}, row={item.InventoryRow}). Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.RawItemType}");
                    return false;
                }

                if (item.IsPrimalAncient && Core.Settings.ItemList.AlwaysStashPrimalAncients)
                {
                    Core.Logger.Debug($"Not Selling due to ItemList setting - Always stash primal ancients. (col={item.InventoryColumn}, row={item.InventoryRow}). Item={item.Name} InternalName={item.InternalName} Sno={item.ActorSnoId} GbId={item.GameBalanceId} RawItemType={item.RawItemType}");
                    return false;
                }

                if (item.IsCosmeticItem)
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

                if (item.TrinityItemType == TrinityItemType.HealthPotion)
                {
                    var equippedPotion = Core.Player.EquippedHealthPotion;
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

                if (item.IsEquipment && item.RequiredLevel <= 1)
                {
                    reason = "无法分解 1 级物品";
                    return true;
                }

                if (Core.Player.IsInventoryLockedForGreaterRift || !Core.Settings.Items.KeepLegendaryUnid && Core.Player.ParticipatingInTieredLootRun)
                {
                    reason = "秘境中,仓库锁定!";
                    return false;
                }

                if (item.IsVendorBought)
                {
                    reason = "无法分解商人出售的物品";
                    return false;
                }

                if (item.IsGem && item.GemQuality >= GemQuality.Marquise && ZetaDia.Me.Level < 70)
                {
                    reason = "自动保留高等级宝石";
                    return false;
                }

                if (GameData.VanityItems.Any(i => item.InternalName.StartsWith(i)))
                {
                    reason = "无用物品";
                    return false;
                }

                if (item.ItemType == ItemType.KeystoneFragment)
                {
                    reason = "秘境钥匙";
                    return false;
                }

                if (item.ItemType == ItemType.HoradricCache)
                {
                    reason = "HoradricCache";
                    return false;
                }

                reason = "Default";
                switch (item.TrinityItemBaseType)
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
                        if (item.TrinityItemType == TrinityItemType.CraftingPlan)
                            return true;
                        if (item.TrinityItemType == TrinityItemType.CraftingMaterial)
                            return true;
                        return false;

                    case TrinityItemBaseType.Unknown:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"出售 {item.Name} 异常 ({item.ActorSnoId}) 内部名称={item.InternalName} 品质={item.ItemQualityLevel} 远古={item.IsAncient} 鉴定={!item.IsUnidentified} 原始类型={item.RawItemType} {ex}");
            }
            finally
            {
                Core.Logger.Debug($"出售: {item.Name} ({item.ActorSnoId}) 理由={reason} 内部名称={item.InternalName} 品质={item.ItemQualityLevel} 远古={item.IsAncient} 鉴定={!item.IsUnidentified} 原始类型={item.RawItemType}");
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
                var validLocation = FindBackpackLocation(true, true);
                return validLocation.X >= 0 && validLocation.Y >= 0;
            }
        }

        private static readonly CacheField<bool> _isValidTwoSlotBackpackLocation = new CacheField<bool>(UpdateSpeed.Fast);

        public static bool CachedIsValidTwoSlotBackpackLocation => _isValidTwoSlotBackpackLocation.GetValue(IsValidTwoSlotBackpackLocation);

        public static bool IsValidTwoSlotBackpackLocation()
        {
            var validLocation = FindBackpackLocation(true, false);
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
                    //Core.Logger.Warn($"包裹空余:{InventoryManager.NumFreeBackpackSlots}, FreeBagSlotsInTown: {FreeBagSlotsInTown}, IsInTown: {Core.Player.IsInTown}");
                    // 判断是否需要清理包裹
                    if (InventoryManager.NumFreeBackpackSlots < FreeBagSlotsInTown && Core.Player.IsInTown && !Core.Player.ParticipatingInTieredLootRun)
                    {
                        //Core.Logger.Warn($"在城镇里, 当前空余低于设置值, 需要清理包裹! _lastBackPackLocation: {_lastBackPackLocation}");
                        _lastBackPackLocation = NoFreeSlot;
                    }

					//Core.Logger.Warn($"IsInTown={Core.Player.IsInTown}, ParticipatingInTieredLootRun={Core.Player.ParticipatingInTieredLootRun}, _lastBackPackLocation={_lastBackPackLocation}");
                    if (!forceRefresh && _lastBackPackLocation != new Vector2(-2, -2) && _lastBackPackLocation != new Vector2(-1, -1) &&
                        _lastBackPackCount == Core.Inventory.BackpackItemCount &&
                        _lastProtectedSlotsCount == CharacterSettings.Instance.ProtectedBagSlots.Count)
                    {
                        //Core.Logger.Warn($"不处理,直接返回!");
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
                            Core.Logger.Error("检测到无效的背包项目！标记下来两个空位!");
                            freeBagSlots -= 2;
                            continue;
                        }
                        int row = item.InventoryRow;
                        int col = item.InventoryColumn;

                        if (row < 0 || row > 5)
                        {
                            Core.Logger.Error("无效物品物品 {0} ({1}) 位于背包第 {2} 行!",
                                item.Name, item.InternalName, item.InventoryRow);
                            continue;
                        }

                        if (col < 0 || col > 9)
                        {
                            Core.Logger.Error("无效物品物品 {0} ({1}) 位于背包第 {2} 列!",
                                item.Name, item.InternalName, item.InventoryColumn);
                            continue;
                        }

                        // Slot is already protected, don't double count
                        if (!backpackSlotBlocked[col, row])
                        {
                            backpackSlotBlocked[col, row] = true;
                            freeBagSlots--;
                        }

                        if (!item.IsTwoSquareItem)
                            continue;

                        try
                        {
                            // Slot is already protected, don't double count
                            if (backpackSlotBlocked[col, row + 1])
                                continue;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            if (item.IsValid && !item.IsDisposed)
                            {
                                Core.Logger.Debug("检查下一个空位的道具错误 {0}, 行={1} col={2} IsTwoSquare={3} 物品种类={4}",
                                    item.Name, item.InventoryRow, item.InventoryColumn, item.IsTwoSquareItem, item.ItemType);
                            }
                            else
                            {
                                Core.Logger.Debug("检查下一个空位的道具错误 不再有效");
                            }
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
                        Core.Logger.Debug("空间不足. FreeSlots={0}, FreeBagSlots={1} FreeBagSlotsInTown={2} 是否在城里={3} 受保护的={4} 背包计数={5}",
                            freeBagSlots, 
							FreeBagSlots, 
							FreeBagSlotsInTown, 
							Core.Player.IsInTown,
                            _lastProtectedSlotsCount, 
							_lastBackPackCount);
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
                                continue;

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
                                continue;

                            // Is a Two Slot, check row below
                            if (backpackSlotBlocked[col, row + 1])
                                continue;

                            pos = new Vector2(col, row);
                            if (!forceRefresh)
                            {
                                _lastBackPackLocation = pos;
                            }
                            return pos;
                        }
                    }

                    // no free slot
                    Core.Logger.Debug("没有剩余空间!");

                    pos = NoFreeSlot;
                    if (!forceRefresh)
                    {
                        _lastBackPackLocation = pos;
                    }

                    return pos;
                }
                catch (Exception ex)
                {
                    Core.Logger.Log("获取背包空间错误");
                    Core.Logger.Log("{0}", ex.ToString());
                    return NoFreeSlot;
                }
            }
        }
    }
}