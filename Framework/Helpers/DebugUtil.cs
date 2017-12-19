using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Trinity.Components.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings.ItemList;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Helpers
{
    public static class DebugExtentions
    {
        public static VarianceResult DetailedCompare<T>(this T val1, T val2)
        {
            if (val1 == null || val2 == null)
                return new VarianceResult();

            var type = val1.GetType();
            var variances = new IndexedList<Variance>();

            foreach (var f in type.GetFields())
            {
                CompareValue(f.GetValue(val1), f.GetValue(val2), f.Name, variances);
            }

            foreach (var f in type.GetProperties())
            {
                CompareValue(f.GetValue(val1), f.GetValue(val2), f.Name, variances);
            }

            return new VarianceResult(variances);
        }

        public static void CompareValue<T>(T val1, T val2, string propName, IndexedList<Variance> newVariances)
        {
            var v = new Variance
            {
                Name = propName,
                valA = val1,
                valB = val2
            };

            if (v.valA == null && v.valB == null)
                return;

            if (v.valA == null || v.valB == null)
                newVariances.Add(v);
            else if (!v.valA.Equals(v.valB))
                newVariances.Add(v);
        }
    }

    public class VarianceResult
    {
        public VarianceResult()
        {
        }

        public VarianceResult(List<Variance> variances)
        {
            Variances = variances;
        }

        public List<Variance> Variances { get; set; } = new List<Variance>();

        public override string ToString()
        {
            if (!Variances.Any())
                return "No Variances";

            var sb = new StringBuilder();
            sb.AppendLine("");
            foreach (var item in Variances)
            {
                sb.AppendLine($"\r{item.Name}: {item.valA} => {item.valB} ");
            }
            return sb.ToString();
        }
    }

    public class Variance
    {
        public string Name { get; set; }
        public object valA { get; set; }
        public object valB { get; set; }
    }

    internal class DebugUtil
    {
        private static DateTime _lastCacheClear = DateTime.MinValue;
        private static Dictionary<int, CachedBuff> _lastBuffs = new Dictionary<int, CachedBuff>();
        private static Dictionary<string, DateTime> _seenAnimationCache = new Dictionary<string, DateTime>();
        private static Dictionary<int, DateTime> _seenUnknownCache = new Dictionary<int, DateTime>();

        public static void LogAnimation(TrinityActor cacheObject)
        {
            if (!LogCategoryEnabled(LogCategory.Animation) || cacheObject.CommonData == null || !cacheObject.CommonData.IsValid)
                return;

            var state = cacheObject.CommonData.AnimationState.ToString();
            var name = cacheObject.CommonData.AnimationInfo.Current.ToString();

            // Log Animation
            if (!_seenAnimationCache.ContainsKey(name))
            {
                Core.Logger.Log(LogCategory.Animation, "{0} 状态={1} By: {2} ({3})", name, state, cacheObject.InternalName, cacheObject.ActorSnoId);
                _seenAnimationCache.Add(name, DateTime.UtcNow);
            }

            CacheMaintenance();
        }

        internal static void LogUnknown(DiaObject diaObject)
        {
            if (!LogCategoryEnabled(LogCategory.UnknownObjects) || !diaObject.IsValid || !diaObject.CommonData.IsValid)
                return;

            // Log Object
            if (!_seenUnknownCache.ContainsKey(diaObject.ActorSnoId))
            {
                Core.Logger.Log(LogCategory.UnknownObjects, "{0} ({1}) 类型={2}", diaObject.Name, diaObject.ActorSnoId, diaObject.ActorType);
                _seenUnknownCache.Add(diaObject.ActorSnoId, DateTime.UtcNow);
            }

            CacheMaintenance();
        }

        internal static void LogInFile(string file, string msg, string filename = "")
        {
            FileStream logStream = null;

            string filePath = Path.Combine(FileManager.LoggingPath, file + ".log");
            logStream = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);

            //TODO : Change File Log writing
            using (var logWriter = new StreamWriter(logStream))
            {
                logWriter.WriteLine(msg);
            }
        }

        private static void CacheMaintenance()
        {
            var age = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(15));
            if (DateTime.UtcNow.Subtract(_lastCacheClear) > TimeSpan.FromSeconds(15))
            {
                if (_seenAnimationCache.Any())
                    _seenAnimationCache = _seenAnimationCache.Where(p => p.Value < age).ToDictionary(p => p.Key, p => p.Value);

                if (_seenUnknownCache.Any())
                    _seenUnknownCache = _seenUnknownCache.Where(p => p.Value < age).ToDictionary(p => p.Key, p => p.Value);
            }
            _lastCacheClear = DateTime.UtcNow;
        }

        public static bool LogCategoryEnabled(LogCategory category)
        {
            return Core.Settings != null && Core.Settings.Advanced.LogCategories.HasFlag(category);
        }

        internal static void LogOnPulse()
        {
            LogBuffs();
        }

        public static void LogBuffs()
        {
            if (Core.Buffs != null && Core.Buffs.ActiveBuffs != null)
            {
                _lastBuffs.ForEach(b =>
                {
                    if (Core.Buffs.ActiveBuffs.Any(o => o.Id + o.BuffAttributeSlot == b.Key))
                        return;

                    Core.Logger.Log(LogCategory.ActiveBuffs, "状态小时 '{0}' ({1}) 建立堆栈={2} VariantId={3} VariantName={4}", b.Value.InternalName, b.Value.Id, b.Value.StackCount, b.Value.BuffAttributeSlot, b.Value.VariantName);
                });

                Core.Buffs.ActiveBuffs.ForEach(b =>
                {
                    CachedBuff lastBuff;
                    if (_lastBuffs.TryGetValue(b.Id + b.BuffAttributeSlot, out lastBuff))
                    {
                        if (b.StackCount != lastBuff.StackCount)
                        {
                            Core.Logger.Log(LogCategory.ActiveBuffs, "状态改变: '{0}' ({1}) 建立堆栈={2}", b.InternalName, b.Id, b.StackCount);
                        }
                    }
                    else
                    {
                        Core.Logger.Log(LogCategory.ActiveBuffs, "获得状态 '{0}' ({1}) 建立堆栈={2} VariantId={3} VariantName={4}", b.InternalName, b.Id, b.StackCount, b.BuffAttributeSlot, b.VariantName);
                    }
                });

                _lastBuffs = Core.Buffs.ActiveBuffs.DistinctBy(k => k.Id + k.BuffAttributeSlot).ToDictionary(k => k.Id + k.BuffAttributeSlot, v => v);
            }
        }

        public string StructFieldsToString(object o)
        {
            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();
            PropertyInfo[] properties = type.GetProperties();

            Dictionary<string, object> values = new Dictionary<string, object>();
            Array.ForEach(fields, (field) => values.Add(field.Name, field.GetValue(o)));
            Array.ForEach(properties, (property) =>
            {
                if (property.CanRead)
                    values.Add(property.Name, property.GetValue(o, null));
            });

            return String.Join(", ", values);
        }

        public static void LogBuildAndItems(TrinityLogLevel level = TrinityLogLevel.Info)
        {
            try
            {
                Action<Item, TrinityLogLevel> logItem = (i, l) =>
                {
                    Core.Logger.Log($"道具: {i.ItemType}: {i.Name} ({i.Id}) 被装备");
                };

                Action<ACDItem, TrinityLogLevel> logACDItem = (i, l) =>
                {
                    Core.Logger.Log($"道具: {i.ItemType}: {i.Name} ({i.ActorSnoId}) 被装备");
                };

                if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                {
                    Core.Logger.Log("错误: 不在游戏");
                    return;
                }

                var equipped = InventoryManager.Equipped;
                if (!equipped.Any())
                {
                    Core.Logger.Log("错误: 检查到没有装备道具");
                    return;
                }

                LogNewItems();

                var equippedItems = Legendary.Equipped.Where(c => (!c.IsSetItem || !c.Set.IsEquipped) && !c.IsEquippedInCube).ToList();
                Core.Logger.Log("------ 装备非套装传奇: 物品={0}, 套装={1} ------", equippedItems.Count, Sets.Equipped.Count);
                equippedItems.ForEach(i => logItem(i, level));

                var cubeItems = Legendary.Equipped.Where(c => c.IsEquippedInCube).ToList();
                Core.Logger.Log("------ 装备卡奈魔盒威能: 物品={0} ------", cubeItems.Count, Sets.Equipped.Count);
                cubeItems.ForEach(i => logItem(i, level));

                Sets.Equipped.ForEach(s =>
                {
                    Core.Logger.Log("------ 套装: {0} {1}: {2}/{3} 装备. 套装属性={4}/{5} ------",
                        s.Name,
                        s.IsClassRestricted ? "(" + s.ClassRestriction + ")" : string.Empty,
                        s.EquippedItems.Count,
                        s.Items.Count,
                        s.CurrentBonuses,
                        s.MaxBonuses);

                    s.Items.Where(i => i.IsEquipped).ForEach(i => logItem(i, level));
                });

                Core.Logger.Log("------ 激活技能 / 符文 ------", SkillUtils.Active.Count, SkillUtils.Active.Count);

                Action<Skill> logSkill = s =>
                {
                    Core.Logger.Log("技能: {0} 符文={1} 类型={2}",
                        s.Name,
                        s.CurrentRune.Name,
                        (s.IsAttackSpender) ? "消耗" : (s.IsGeneratorOrPrimary) ? "生成" : "其他"
                        );
                };

                SkillUtils.Active.ForEach(logSkill);

                Core.Logger.Log("------ 被动 ------", SkillUtils.Active.Count, SkillUtils.Active.Count);

                Action<Passive> logPassive = p => Core.Logger.Log("被动: {0}", p.Name);

                PassiveUtils.Active.ForEach(logPassive);
            }
            catch (Exception ex)
            {
                Core.Logger.Log("调试 > 登录构建和项目 异常: {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        public static void LogSystemInformation(TrinityLogLevel level = TrinityLogLevel.Debug)
        {
            //try
            //{
            //    Core.Logger.Log("------ 系统信息 ------");
            //    Core.Logger.Log("处理器: " + SystemInformation.Processor);
            //    Core.Logger.Log("当前速度: " + SystemInformation.ActualProcessorSpeed);
            //    Core.Logger.Log("操作系统: " + SystemInformation.OperatingSystem);
            //    Core.Logger.Log("主板: " + SystemInformation.MotherBoard);
            //    Core.Logger.Log("系统类型: " + SystemInformation.SystemType);
            //    Core.Logger.Log("可用物理内存: " + SystemInformation.FreeMemory);
            //    Core.Logger.Log("硬盘: " + SystemInformation.HardDisk);
            //    Core.Logger.Log("显卡: " + SystemInformation.VideoCard);
            //    Core.Logger.Log("解析度: " + SystemInformation.Resolution);
            //}
            //catch (Exception)
            //{
            //}
        }

        internal static void DumpReferenceItems(TrinityLogLevel level = TrinityLogLevel.Debug)
        {
            var path = Path.Combine(FileManager.DemonBuddyPath, "Plugins\\Trinity\\Resources\\JS Class Generator\\ItemReference.js");

            if (File.Exists(path))
                File.Delete(path);

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("var itemLookup = {");

                foreach (var item in Legendary.ToList())
                {
                    var key = !string.IsNullOrEmpty(item.Slug) ? item.Slug : RemoveApostophes(item.Name).ToLower();

                    if (item.Id != 0)
                        w.WriteLine($"     \"{key}\": [\"{item.Name}\", {item.Id}, \"{item.InternalName}\"],");
                }

                w.WriteLine("}");
            }

            Core.Logger.Log("转储道具信息到: {0}", path);
        }

        public static string RemoveApostophes(string input)
        {
            input = input.Replace("'", String.Empty);
            input = input.Replace("`", String.Empty);
            input = input.Replace("’", String.Empty);
            return input;
        }

        internal static void LogInvalidItems(TrinityLogLevel level = TrinityLogLevel.Debug)
        {
            var dropItems = Legendary.ToList().Where(i => !i.IsCrafted && i.Id == 0).OrderBy(i => i.TrinityItemType).ToList();
            var craftedItems = Legendary.ToList().Where(i => i.IsCrafted && i.Id == 0).OrderBy(i => i.TrinityItemType).ToList();

            Core.Logger.Log("掉落的物品: {0}", dropItems.Count);
            foreach (var item in dropItems)
            {
                Core.Logger.Log("{0} - {1} = 0", item.TrinityItemType, item.Name);
            }

            Core.Logger.Log(" ");
            Core.Logger.Log("制作的物品: {0}", craftedItems.Count);
            foreach (var item in craftedItems)
            {
                Core.Logger.Log("{0} - {1} = 0", item.TrinityItemType, item.Name);
            }
        }

        internal static void LogNewItems()
        {
            //var knownIds = Legendary.ItemIds;

            //using (ZetaDia.Memory.AcquireFrame())
            //{
            //    if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
            //    {
            //        Core.Logger.Log("不在游戏");
            //        return;
            //    }

            //    var allItems = new List<ACDItem>();
            //    allItems.AddRange(InventoryManager.StashItems);
            //    allItems.AddRange(InventoryManager.Equipped);
            //    allItems.AddRange(InventoryManager.Backpack);

            //    if (!allItems.Any())
            //        return;

            //    var newItems = allItems.Where(i => i != null && i.IsValid && i.ItemQualityLevel == ItemQuality.Legendary && (i.ItemBaseType == ItemBaseType.Jewelry || i.ItemBaseType == ItemBaseType.Armor || i.ItemBaseType == ItemBaseType.Weapon) && !knownIds.Contains(i.ActorSnoId)).DistinctBy(p => p.ActorSnoId).OrderBy(i => i.ItemType).ToList();

            //    if (!newItems.Any())
            //        return;

            //    Core.Logger.Log("------ 新/未知物品 {0} ------", newItems.Count);

            //    newItems.ForEach(i =>
            //    {
            //        Core.Logger.Log($"道具: {i.ItemType}: {i.Name} ({i.ActorSnoId})");
            //    });
            //}
        }

        internal static void DumpItemSNOReference()
        {
            string[] names = Enum.GetNames(typeof(SNOActor));
            int[] values = (int[])Enum.GetValues(typeof(SNOActor));
            var toLog = new List<string>();
            for (int i = 0; i < names.Length; i++)
            {
                var sno = values[i];
                var name = names[i];
                var type = TypeConversions.DetermineItemType(name, ItemType.Unknown);
                if (type != TrinityItemType.Unknown || GameData.GoldSNO.Contains(sno) ||
                    GameData.ForceToItemOverrideIds.Contains(sno) || GameData.HealthGlobeSNO.Contains(sno) || Legendary.ItemIds.Contains(sno))
                {
                    toLog.Add($"{{ {sno}, TrinityItemType.{type} }}, // {name}");
                }
            }

            var path = WriteLinesToLog("ItemSNOReference.log", toLog, true);
            Core.Logger.Log("完成掉落物品SNO刷新到 {0}", path);
        }

        public static string WriteLinesToLog(string logFileName, string lines, bool deleteFirst = false)
        {
            return WriteLinesToLog(logFileName, new List<string> { lines }, deleteFirst);
        }

        /// <summary>
        /// Writes an ActorMeta record to the log file in the format for a Dictionary collection initializer
        /// </summary>
        public static string WriteLinesToLog(string logFileName, IEnumerable<string> msgs, bool deleteFirst = false)
        {
            var fullFilePath = Path.Combine(FileManager.LoggingPath, logFileName);

            if (deleteFirst && File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            var logStream = File.Open(fullFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);

            using (var logWriter = new StreamWriter(logStream))
            {
                foreach (var msg in msgs.Where(msg => !string.IsNullOrEmpty(msg)))
                {
                    logWriter.WriteLine(msg);
                }
            }

            logStream.Close();

            return fullFilePath;
        }

        public static void ItemListTest()
        {
            Core.Logger.Log("开始捡取列表背包测试");

            var backpackItems = Core.Inventory.Backpack;
            var total = backpackItems.Count();
            var toBeStashed = 0;

            foreach (var acdItem in backpackItems)
            {
                Core.Logger.Log($"{acdItem.Name} ActorSnoId={acdItem.ActorSnoId} GameBalanceId={acdItem.GameBalanceId} ACDId={acdItem.AcdId} AnnId={acdItem.AnnId}");
                Core.Logger.Verbose(acdItem.ToString());
                Core.Logger.Verbose(acdItem.Attributes.ToString());

                if (ItemListEvaluator.ShouldStashItem(acdItem, true))
                    toBeStashed++;
            }

            Core.Logger.Log("完成捡取列表背包测试");

            Core.Logger.Log("完成 - 储存 {0} / {1}", toBeStashed, total);
        }

        public enum DumpItemLocation
        {
            All,
            Equipped,
            Backpack,
            Ground,
            Stash,
            Merchant,
        }

        public static void DumpQuickItems()
        {
            List<ACDItem> itemList;
            try
            {
                itemList = ZetaDia.Actors.GetActorsOfType<ACDItem>(true).OrderBy(i => i.InventorySlot).ThenBy(i => i.Name).ToList();
            }
            catch
            {
                Core.Logger.Error("快速储存: 检测到物品错误!");
                itemList = ZetaDia.Actors.GetActorsOfType<ACDItem>(true).ToList();
            }
            StringBuilder sbTopList = new StringBuilder();
            foreach (var item in itemList)
            {
                try
                {
                    sbTopList.AppendFormat("\nName={0} InternalName={1} ActorSnoId={2} DynamicID={3} InventorySlot={4}",
                        item.Name, item.InternalName, item.ActorSnoId, item.AnnId, item.InventorySlot);
                }
                catch (Exception)
                {
                    sbTopList.AppendFormat("Exception reading data from ACDItem ACDId={0}", item.ACDId);
                }
            }
            Core.Logger.Log(sbTopList.ToString());
        }

        public static void DumpItems(DumpItemLocation location)
        {
            using (ZetaDia.Memory.SaveCacheState())
            {
                ZetaDia.Memory.TemporaryCacheState(false);

                List<ItemWrapper> itemList = new List<ItemWrapper>();

                switch (location)
                {
                    case DumpItemLocation.All:
                        itemList = ZetaDia.Actors.GetActorsOfType<ACDItem>(true).Select(i => new ItemWrapper(i)).OrderBy(i => i.InventorySlot).ThenBy(i => i.Name).ToList();
                        break;

                    case DumpItemLocation.Backpack:
                        itemList = InventoryManager.Backpack.Select(i => new ItemWrapper(i)).ToList();
                        break;

                    case DumpItemLocation.Merchant:
                        itemList = InventoryManager.MerchantItems.Select(i => new ItemWrapper(i)).ToList();
                        break;

                    case DumpItemLocation.Ground:
                        itemList = ZetaDia.Actors.GetActorsOfType<DiaItem>(true).Select(i => new ItemWrapper(i.CommonData)).ToList();
                        break;

                    case DumpItemLocation.Equipped:
                        itemList = InventoryManager.Equipped.Select(i => new ItemWrapper(i)).ToList();
                        break;

                    case DumpItemLocation.Stash:
                        if (UIElements.StashWindow.IsVisible)
                        {
                            itemList = InventoryManager.StashItems.Select(i => new ItemWrapper(i)).ToList();
                        }
                        else
                        {
                            Core.Logger.Log("储物箱窗口无法打开!");
                        }
                        break;
                }

                itemList.RemoveAll(i => i == null);
                //itemList.RemoveAll(i => !i.IsValid);

                foreach (var item in itemList.OrderBy(i => i.InventorySlot).ThenBy(i => i.Name))
                {
                    try
                    {
                        string itemName = $"\nName={item.Name} InternalName={item.InternalName} ActorSnoId={item.ActorSnoId} DynamicID={item.DynamicId} InventorySlot={item.InventorySlot}";

                        Core.Logger.Log(itemName);
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Log("Exception reading Basic Item Info\n{0}", ex.ToString());
                    }
                    try
                    {
                        foreach (object val in Enum.GetValues(typeof(ActorAttributeType)))
                        {
                            int iVal = item.Item.GetAttribute<int>((ActorAttributeType)val);
                            float fVal = item.Item.GetAttribute<float>((ActorAttributeType)val);

                            if (iVal > 0 || fVal > 0)
                                Core.Logger.Log("Attribute: {0}, iVal: {1}, fVal: {2}", val, iVal, (fVal != fVal) ? "" : fVal.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Log("Exception reading attributes for {0}\n{1}", item.Name, ex.ToString());
                    }

                    try
                    {
                        foreach (var stat in Enum.GetValues(typeof(ItemStats.Stat)).Cast<ItemStats.Stat>())
                        {
                            float fStatVal = item.Stats.GetStat<float>(stat);
                            int iStatVal = item.Stats.GetStat<int>(stat);
                            if (fStatVal > 0 || iStatVal > 0)
                                Core.Logger.Log("属性 {0}={1}f ({2})", stat, fStatVal, iStatVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Log("Exception reading Item Stats\n{0}", ex.ToString());
                    }

                    try
                    {
                        Core.Logger.Log("Link Color ItemQuality=" + item.Item.GetItemQuality());
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Log("Exception reading Item Link\n{0}", ex.ToString());
                    }

                    try
                    {
                        PrintObjectProperties(item);
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Log("Exception reading Item PropertyLoader\n{0}", ex.ToString());
                    }
                }
            }
        }

        private static void PrintObjectProperties<T>(T item)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties.ToList().OrderBy(p => p.Name))
            {
                try
                {
                    object val = property.GetValue(item, null);
                    if (val != null)
                    {
                        Core.Logger.Log(typeof(T).Name + "." + property.Name + "=" + val);

                        // Special cases!
                        if (property.Name == "ValidInventorySlots")
                        {
                            foreach (var slot in ((InventorySlot[])val))
                            {
                                Core.Logger.Log(slot.ToString());
                            }
                        }
                    }
                }
                catch
                {
                    Core.Logger.Log("从对象读取 {0} 异常", property.Name);
                }
            }
        }
    }
}