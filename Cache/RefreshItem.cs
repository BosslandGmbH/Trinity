using System;
using System.IO;
using System.Linq;
using Trinity.Cache;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Settings.Loot;
using Trinity.Technicals;
using TrinityCoroutines.Resources;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public partial class TrinityPlugin
    {
        private static bool RefreshItem()
        {
            using (new PerformanceLogger("RefreshItem"))
            {
                bool logNewItem;
                bool AddToCache;

                var diaItem = c_diaObject as DiaItem;

                if (diaItem == null)
                    return false;

                if (diaItem.CommonData == null)
                    return false;

                if (!diaItem.IsValid)
                    return false;

                if (!diaItem.CommonData.IsValid)
                    return false;

                c_ItemQuality = diaItem.CommonData.ItemQualityLevel;
                ((DiaItem)c_diaObject).CommonData.GetAttribute<int>(ActorAttributeType.ItemQualityLevelIdentified);
                c_ItemDisplayName = diaItem.CommonData.Name;

                CurrentCacheObject.DynamicID = c_diaObject.CommonData.AnnId;
                CurrentCacheObject.GameBalanceID = c_diaObject.CommonData.GameBalanceId;

                c_ItemLevel = diaItem.CommonData.Level;
                c_DBItemBaseType = diaItem.CommonData.ItemBaseType;
                c_DBItemType = diaItem.CommonData.ItemType;
                c_IsOneHandedItem = diaItem.CommonData.IsOneHand;
                c_IsTwoHandedItem = diaItem.CommonData.IsTwoHand;
                c_item_tFollowerType = diaItem.CommonData.FollowerSpecialType;

                // Calculate item type
                if(_cItemTinityItemType == TrinityItemType.Unknown)
                    _cItemTinityItemType = TrinityItemManager.DetermineItemType(CurrentCacheObject.InternalName, c_DBItemType, c_item_tFollowerType);

                // And temporarily store the base type
                TrinityItemBaseType itemBaseType = TrinityItemManager.DetermineBaseType(_cItemTinityItemType);

                // Compute item quality from item link 
                if (!CacheData.ItemLinkQuality.TryGetValue(CurrentCacheObject.ACDGuid, out c_ItemQuality))
                {
                    c_ItemQuality = diaItem.CommonData.GetItemQuality();
                    CacheData.ItemLinkQuality.Add(CurrentCacheObject.ACDGuid, c_ItemQuality);
                }

                if (itemBaseType == TrinityItemBaseType.Gem)
                    c_ItemLevel = (int)diaItem.CommonData.GemQuality;

                CurrentCacheObject.ObjectHash = HashGenerator.GenerateItemHash(
                    CurrentCacheObject.Position,
                    CurrentCacheObject.ActorSNO,
                    CurrentCacheObject.InternalName,
                    Player.WorldID,
                    c_ItemQuality,
                    c_ItemLevel);

                try
                {
                    c_IsAncient = c_ItemQuality == ItemQuality.Legendary && diaItem.CommonData.GetAttribute<int>(ActorAttributeType.AncientRank) > 0;
                }
                catch {}

                float range = 0f;

                // Always include progression globes in cache.
                if (_cItemTinityItemType == TrinityItemType.ProgressionGlobe)
                {
                    CurrentCacheObject.Type = TrinityObjectType.ProgressionGlobe;
                    return true;
                }

                // Always include deaths breath in cache.
                var craftMaterialType = TrinityItemManager.GetCraftingMaterialType(CurrentCacheObject.ActorSNO);
                if (craftMaterialType == InventoryItemType.DeathsBreath)
                {
                    CurrentCacheObject.Type = TrinityObjectType.Item;
                    return true;
                }

                // Always include blood shards unless we're at max already.
                if (_cItemTinityItemType == TrinityItemType.HoradricRelic)
                {
                    return ZetaDia.PlayerData.BloodshardCount < Player.MaxBloodShards;
                }

                // no range check on Legendaries
                if (c_ItemQuality < ItemQuality.Legendary)
                {
                    if (c_ItemQuality >= ItemQuality.Rare4)
                        range = CurrentBotLootRange;

                    if (_keepLootRadiusExtendedForSeconds > 0)
                        range += 90f;

                    if (CurrentCacheObject.Distance > (CurrentBotLootRange + range))
                    {
                        c_IgnoreSubStep = "OutOfRange";
                        // return here to save CPU on reading unncessary attributes for out of range items;
                        return false;
                    }
                }

                var pickupItem = new PickupItem
                {
                    Name = c_ItemDisplayName,
                    InternalName = CurrentCacheObject.InternalName,
                    Level = c_ItemLevel,
                    Quality = c_ItemQuality,
                    BalanceID = CurrentCacheObject.GameBalanceID,
                    DBBaseType = c_DBItemBaseType,
                    DBItemType = c_DBItemType,
                    TBaseType = itemBaseType,
                    TType = _cItemTinityItemType,
                    IsOneHand = c_IsOneHandedItem,
                    IsTwoHand = c_IsTwoHandedItem,
                    ItemFollowerType = c_item_tFollowerType,
                    DynamicID = CurrentCacheObject.DynamicID,
                    Position = CurrentCacheObject.Position,
                    ActorSNO = CurrentCacheObject.ActorSNO,
                    ACDGuid = CurrentCacheObject.ACDGuid,
                    RActorGUID = CurrentCacheObject.RActorGuid,
                };

                // Treat all globes as a yes
                if (_cItemTinityItemType == TrinityItemType.HealthGlobe)
                {
                    CurrentCacheObject.Type = TrinityObjectType.HealthGlobe;
                    return true;
                }

                // Treat all globes as a yes
                if (_cItemTinityItemType == TrinityItemType.PowerGlobe)
                {
                    CurrentCacheObject.Type = TrinityObjectType.PowerGlobe;
                    return true;
                }

                // Item stats
                logNewItem = RefreshItemStats(itemBaseType);

                // Get whether or not we want this item, cached if possible
                if (!CacheData.PickupItem.TryGetValue(CurrentCacheObject.RActorGuid, out AddToCache))
                {
                    if (pickupItem.IsTwoHand && Settings.Loot.Pickup.IgnoreTwoHandedWeapons && c_ItemQuality < ItemQuality.Legendary)
                    {
                        AddToCache = false;
                    }
                    else if (Settings.Loot.ItemFilterMode == ItemFilterMode.DemonBuddy)
                    {
                        AddToCache = ItemManager.Current.ShouldPickUpItem((ACDItem)c_diaObject.CommonData);
                    }
                    else if (Settings.Loot.ItemFilterMode == ItemFilterMode.TrinityWithItemRules)
                    {
                        AddToCache = TrinityItemManager.ItemRulesPickupValidation(pickupItem);
                    }
                    else // TrinityPlugin Scoring Only
                    {
                        AddToCache = TrinityItemManager.PickupItemValidation(pickupItem);
                    }

                    // Pickup low level enabled, and we're a low level
                    if (!AddToCache && Settings.Loot.Pickup.PickupLowLevel && Player.Level <= 10)
                    {
                        AddToCache = TrinityItemManager.PickupItemValidation(pickupItem);
                    }

                    // Ignore if item has existed before in this location
                    if (Settings.Loot.TownRun.DropLegendaryInTown)
                    {
                        if (!CacheData.DroppedItems.Any(i => i.Equals(pickupItem)))
                        {
                            CacheData.DroppedItems.Add(pickupItem);
                            AddToCache = true;
                        }
                        else
                        {
                            Logger.LogDebug("Ignoring Dropped Item = ItemPosition={0} Hashcode={1} DynId={2}", pickupItem.Position, pickupItem.GetHashCode(), pickupItem.DynamicID);
                            AddToCache = false;
                        }
                            
                    }

                    CacheData.PickupItem.Add(CurrentCacheObject.RActorGuid, AddToCache);
                }

                if (AddToCache && ForceVendorRunASAP)
                    c_IgnoreSubStep = "ForcedVendoring";

                if (_cItemTinityItemType == TrinityItemType.PortalDevice || _cItemTinityItemType == TrinityItemType.UberReagent)
                {
                    AddToCache = true;
                }

                var acdItem = CurrentCacheObject.CommonData as ACDItem;
                if (acdItem != null && acdItem.ItemType == ItemType.LegendaryGem)
                {
                    AddToCache = true;
                }

                // Didn't pass pickup rules, so ignore it
                if (!AddToCache && c_IgnoreSubStep == String.Empty)
                    c_IgnoreSubStep = "NoMatchingRule";

                if (Settings.Advanced.LogDroppedItems && logNewItem && _cItemTinityItemType != TrinityItemType.HealthGlobe && _cItemTinityItemType != TrinityItemType.HealthPotion && _cItemTinityItemType != TrinityItemType.PowerGlobe && _cItemTinityItemType != TrinityItemType.ProgressionGlobe)
                    //LogDroppedItem();
                    ItemDroppedAppender.Instance.AppendDroppedItem(pickupItem);

                return AddToCache;
            }
        }

        private static bool RefreshGold()
        {
            if (!Settings.Loot.Pickup.PickupGold)
            {
                c_IgnoreSubStep = "PickupDisabled";
                return false;
            }

            if (Player.ActorClass == ActorClass.Barbarian && Settings.Combat.Barbarian.IgnoreGoldInWOTB && Hotbar.Contains(SNOPower.Barbarian_WrathOfTheBerserker) &&
                GetHasBuff(SNOPower.Barbarian_WrathOfTheBerserker))
            {
                c_IgnoreSubStep = "IgnoreGoldInWOTB";
                return false;
            }

            // Get the gold amount of this pile, cached if possible
            if (!CacheData.GoldStack.TryGetValue(CurrentCacheObject.RActorGuid, out c_GoldStackSize))
            {
                try
                {
                    c_GoldStackSize = ((ACDItem)c_diaObject.CommonData).Gold;
                }
                catch
                {
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception getting gold pile amount for item {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
                    c_IgnoreSubStep = "GetAttributeException";
                    return false;
                }
                CacheData.GoldStack.Add(CurrentCacheObject.RActorGuid, c_GoldStackSize);
            }

            if (c_GoldStackSize < Settings.Loot.Pickup.MinimumGoldStack)
            {
                c_IgnoreSubStep = "NotEnoughGold";
                return false;
            }

            return true;
        }
        private static bool RefreshItemStats(TrinityItemBaseType baseType)
        {
            bool isNewLogItem = false;

            c_ItemMd5Hash = HashGenerator.GenerateItemHash(CurrentCacheObject.Position, CurrentCacheObject.ActorSNO, CurrentCacheObject.InternalName, CurrentWorldDynamicId, c_ItemQuality, c_ItemLevel);

            if (!GenericCache.ContainsKey(c_ItemMd5Hash))
            {
                GenericCache.AddToCache(new GenericCacheObject(c_ItemMd5Hash, null, new TimeSpan(1, 0, 0)));

                try
                {
                    isNewLogItem = true;
                    if (baseType == TrinityItemBaseType.Armor || baseType == TrinityItemBaseType.WeaponOneHand || baseType == TrinityItemBaseType.WeaponTwoHand ||
                        baseType == TrinityItemBaseType.WeaponRange || baseType == TrinityItemBaseType.Jewelry || baseType == TrinityItemBaseType.FollowerItem ||
                        baseType == TrinityItemBaseType.Offhand)
                    {
                        try
                        {
                            int iThisQuality;
                            ItemDropStats.ItemsDroppedStats.Total++;
                            if (c_ItemQuality >= ItemQuality.Legendary)
                                iThisQuality = ItemDropStats.QUALITYORANGE;
                            else if (c_ItemQuality >= ItemQuality.Rare4)
                                iThisQuality = ItemDropStats.QUALITYYELLOW;
                            else if (c_ItemQuality >= ItemQuality.Magic1)
                                iThisQuality = ItemDropStats.QUALITYBLUE;
                            else
                                iThisQuality = ItemDropStats.QUALITYWHITE;
                            ItemDropStats.ItemsDroppedStats.TotalPerQuality[iThisQuality]++;
                            ItemDropStats.ItemsDroppedStats.TotalPerLevel[c_ItemLevel]++;
                            ItemDropStats.ItemsDroppedStats.TotalPerQPerL[iThisQuality, c_ItemLevel]++;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error Refreshing Item Stats for Equippable Item: " + ex.ToString());
                        }
                    }
                    else if (baseType == TrinityItemBaseType.Gem)
                    {
                        try
                        {
                            int iThisGemType = 0;
                            ItemDropStats.ItemsDroppedStats.TotalGems++;
                            if (_cItemTinityItemType == TrinityItemType.Topaz)
                                iThisGemType = ItemDropStats.GEMTOPAZ;
                            if (_cItemTinityItemType == TrinityItemType.Ruby)
                                iThisGemType = ItemDropStats.GEMRUBY;
                            if (_cItemTinityItemType == TrinityItemType.Emerald)
                                iThisGemType = ItemDropStats.GEMEMERALD;
                            if (_cItemTinityItemType == TrinityItemType.Amethyst)
                                iThisGemType = ItemDropStats.GEMAMETHYST;
                            if (_cItemTinityItemType == TrinityItemType.Diamond)
                                iThisGemType = ItemDropStats.GEMDIAMOND;
                            ItemDropStats.ItemsDroppedStats.GemsPerType[iThisGemType]++;
                            ItemDropStats.ItemsDroppedStats.GemsPerLevel[c_ItemLevel]++;
                            ItemDropStats.ItemsDroppedStats.GemsPerTPerL[iThisGemType, c_ItemLevel]++;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error refreshing item stats for Gem: " + ex.ToString());
                        }
                    }
                    else if (_cItemTinityItemType == TrinityItemType.InfernalKey)
                    {
                        try
                        {
                            ItemDropStats.ItemsDroppedStats.TotalInfernalKeys++;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error refreshing item stats for InfernalKey: " + ex.ToString());
                        }
                    }
                    // See if we should update the stats file
                    if (DateTime.UtcNow.Subtract(ItemDropStats.ItemStatsLastPostedReport).TotalSeconds > 10)
                    {
                        try
                        {
                            ItemDropStats.ItemStatsLastPostedReport = DateTime.UtcNow;
                            ItemDropStats.OutputReport();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error Calling OutputReport from RefreshItemStats " + ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Couldn't do Item Stats" + ex.ToString());
                }
            }
            return isNewLogItem;
        }

        private static void LogSkippedGold()
        {
            string skippedItemsPath = Path.Combine(FileManager.LoggingPath, String.Format("SkippedGoldStacks_{0}_{1}.csv", Player.ActorClass, DateTime.UtcNow.ToString("yyyy-MM-dd")));

            bool writeHeader = !File.Exists(skippedItemsPath);
            using (var LogWriter = new StreamWriter(skippedItemsPath, true))
            {
                if (writeHeader)
                {
                    LogWriter.WriteLine("ActorSnoId,RActorGUID,DyanmicID,ACDId,Name,GoldStackSize,IgnoreItemSubStep,Distance");
                }
                LogWriter.Write(FormatCSVField(CurrentCacheObject.ActorSNO));
                LogWriter.Write(FormatCSVField(CurrentCacheObject.RActorGuid));
                LogWriter.Write(FormatCSVField(CurrentCacheObject.DynamicID));
                LogWriter.Write(FormatCSVField(CurrentCacheObject.ACDGuid));
                LogWriter.Write(FormatCSVField(CurrentCacheObject.InternalName));
                LogWriter.Write(FormatCSVField(c_GoldStackSize));
                LogWriter.Write(FormatCSVField(c_IgnoreSubStep));
                LogWriter.Write(FormatCSVField(CurrentCacheObject.Distance));
                LogWriter.Write("\n");
            }
        }

        private static string FormatCSVField(DateTime time)
        {
            return String.Format("\"{0:yyyy-MM-ddTHH:mm:ss.ffff}\",", time.ToLocalTime());
        }

        private static string FormatCSVField(string text)
        {
            return String.Format("\"{0}\",", text);
        }

        private static string FormatCSVField(int number)
        {
            return String.Format("\"{0}\",", number);
        }

        private static string FormatCSVField(double number)
        {
            return String.Format("\"{0:0}\",", number);
        }

        private static string FormatCSVField(bool value)
        {
            return String.Format("\"{0}\",", value);
        }
    }
}