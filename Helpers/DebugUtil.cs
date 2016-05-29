using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Trinity;
using Trinity.Framework.Actors;
using Trinity.Framework.Objects.Memory;
using Trinity.Items;
using Trinity.Items.ItemList;
using Trinity.Objects;
using Trinity.Objects.Native;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Helpers
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
                sb.AppendLine(string.Format("\r{0}: {1} => {2} ", item.Name, item.valA, item.valB));
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

    class DebugUtil
    {
        private static DateTime _lastCacheClear = DateTime.MinValue;
        private static Dictionary<int, CachedBuff> _lastBuffs = new Dictionary<int, CachedBuff>();
        private static Dictionary<string, DateTime> _seenAnimationCache = new Dictionary<string, DateTime>();
        private static Dictionary<int, DateTime> _seenUnknownCache = new Dictionary<int, DateTime>();


        public static void LogAnimation(TrinityCacheObject cacheObject)
        {
            if (!LogCategoryEnabled(LogCategory.Animation) || cacheObject.CommonData == null || !cacheObject.CommonData.IsValid || !cacheObject.CommonData.AnimationInfo.IsValid)
                return;

            var state = cacheObject.CommonData.AnimationState.ToString();
            var name = cacheObject.CommonData.CurrentAnimation.ToString();

            // Log Animation
            if (!_seenAnimationCache.ContainsKey(name))
            {
                Logger.Log(LogCategory.Animation, "{0} State={1} By: {2} ({3})", name, state, cacheObject.InternalName, cacheObject.ActorSNO);
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
                Logger.Log(LogCategory.UnknownObjects, "{0} ({1}) Type={2}", diaObject.Name, diaObject.ActorSnoId, diaObject.ActorType);
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
            return TrinityPlugin.Settings != null && TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(category);
        }



        internal static void LogOnPulse()
        {
            LogBuffs();
        }

        public static void LogBuffs()
        {
            if (CacheData.Buffs != null && CacheData.Buffs.ActiveBuffs != null)
            {
                _lastBuffs.ForEach(b =>
                {
                    if (CacheData.Buffs.ActiveBuffs.Any(o => o.Id + o.BuffAttributeSlot == b.Key))
                        return;

                    Logger.Log(LogCategory.ActiveBuffs, "Buff lost '{0}' ({1}) Stacks={2} VariantId={3} VariantName={4}", b.Value.InternalName, b.Value.Id, b.Value.StackCount, b.Value.BuffAttributeSlot, b.Value.VariantName);
                });

                CacheData.Buffs.ActiveBuffs.ForEach(b =>
                {
                    CachedBuff lastBuff;
                    if (_lastBuffs.TryGetValue(b.Id + b.BuffAttributeSlot, out lastBuff))
                    {
                        if (b.StackCount != lastBuff.StackCount)
                        {
                            Logger.Log(LogCategory.ActiveBuffs, "Buff Stack Changed: '{0}' ({1}) Stacks={2}", b.InternalName, b.Id, b.StackCount);
                        }
                    }
                    else
                    {
                        Logger.Log(LogCategory.ActiveBuffs, "Buff Gained '{0}' ({1}) Stacks={2} VariantId={3} VariantName={4}", b.InternalName, b.Id, b.StackCount, b.BuffAttributeSlot, b.VariantName);
                    }
                });

                _lastBuffs = CacheData.Buffs.ActiveBuffs.DistinctBy(k => k.Id + k.BuffAttributeSlot).ToDictionary(k => k.Id + k.BuffAttributeSlot, v => v);
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
                using (new AquireFrameHelper())
                {
                    Action<Item, TrinityLogLevel> logItem = (i, l) =>
                    {
                        Logger.Log(l, LogCategory.UserInformation, string.Format("Item: {0}: {1} ({2}) is Equipped",
                            i.ItemType, i.Name, i.Id));
                    };

                    Action<ACDItem, TrinityLogLevel> logACDItem = (i, l) =>
                    {
                        Logger.Log(l, LogCategory.UserInformation, string.Format("Item: {0}: {1} ({2}) is Equipped",
                            i.ItemType, i.Name, i.ActorSnoId));
                    };

                    if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                    {
                        Logger.Log("Error: Not in game");
                        return;
                    }

                    var equipped = ZetaDia.Me.Inventory.Equipped;
                    if (!equipped.Any())
                    {
                        Logger.Log("Error: No equipped items detected");
                        return;
                    }

                    LogNewItems();

                    var equippedItems = Legendary.Equipped.Where(c => (!c.IsSetItem || !c.Set.IsEquipped) && !c.IsEquippedInCube).ToList();
                    Logger.Log(level, LogCategory.UserInformation, "------ Equipped Non-Set Legendaries: Items={0}, Sets={1} ------", equippedItems.Count, Sets.Equipped.Count);
                    equippedItems.ForEach(i => logItem(i, level));

                    var cubeItems = Legendary.Equipped.Where(c => c.IsEquippedInCube).ToList();
                    Logger.Log(level, LogCategory.UserInformation, "------ Equipped in Kanai's Cube: Items={0} ------", cubeItems.Count, Sets.Equipped.Count);
                    cubeItems.ForEach(i => logItem(i, level));

                    Sets.Equipped.ForEach(s =>
                    {
                        Logger.Log(level, LogCategory.UserInformation, "------ Set: {0} {1}: {2}/{3} Equipped. ActiveBonuses={4}/{5} ------",
                            s.Name,
                            s.IsClassRestricted ? "(" + s.ClassRestriction + ")" : string.Empty,
                            s.EquippedItems.Count,
                            s.Items.Count,
                            s.CurrentBonuses,
                            s.MaxBonuses);

                        s.Items.Where(i => i.IsEquipped).ForEach(i => logItem(i, level));
                    });

                    //Logger.Log(level, LogCategory.UserInformation, "------ Gems ------", SkillUtils.Active.Count, SkillUtils.Active.Count);

                    //var gems = ZetaDia.Actors.ACDList.OfType<ACDItem>().Where(i => i.InventorySlot == InventorySlot.Socket && i.ItemType == ItemType.LegendaryGem);

                    //foreach (var gem in gems)
                    //{
                    //    logACDItem(gem, level);                    }

                    //var backpackRing = ZetaDia.Me.Inventory.Backpack.FirstOrDefault(i => i.ItemType == ItemType.Ring && i.ActorSnoId == Legendary.AshnagarrsBloodRing.Id);
                    //var backpackRingNative = ZetaDia.Memory.Read<ItemRecord>(SNORecordGameBalance.GetGameBalanceRecord(backpackRing.GameBalanceId, backpackRing.GameBalanceType));

                    //var backpackGem = ZetaDia.Actors.ACDList.OfType<ACDItem>().FirstOrDefault(i => i.InventorySlot == InventorySlot.Socket && i.ItemType == ItemType.LegendaryGem && i.ActorSnoId == Gems.Taeguk.Id);
                    //var backpackGemNative= ZetaDia.Memory.Read<ItemRecord>(SNORecordGameBalance.GetGameBalanceRecord(backpackGem.GameBalanceId, backpackGem.GameBalanceType));

                    //var equippedRing = ZetaDia.Me.Inventory.Equipped.FirstOrDefault(i => i.ItemType == ItemType.Ring && i.ActorSnoId == Legendary.AshnagarrsBloodRing.Id);
                    //var equippedRingNative = ZetaDia.Memory.Read<ItemRecord>(SNORecordGameBalance.GetGameBalanceRecord(equippedRing.GameBalanceId, equippedRing.GameBalanceType));

                    //var equippedGem = ZetaDia.Actors.ACDList.OfType<ACDItem>().LastOrDefault(i => i.ItemType == ItemType.LegendaryGem && i.ActorSnoId == Gems.Taeguk.Id);
                    //var equippedGemNative = ZetaDia.Memory.Read<ItemRecord>(SNORecordGameBalance.GetGameBalanceRecord(equippedGem.GameBalanceId, equippedGem.GameBalanceType));

                    //var ringDiff = backpackRing.DetailedCompare(equippedRing);
                    //var gemDiff = backpackGem.DetailedCompare(equippedGem);
                    //var ringNativeDiff = backpackRingNative.DetailedCompare(equippedRingNative);
                    //var gemNativeDiff = backpackGemNative.DetailedCompare(equippedGemNative);

                    //Logger.Log("\n\r ringDiff \n\r {0}", ringDiff);
                    //Logger.Log("\n\r gemDiff \n\r {0}", gemDiff);
                    //Logger.Log("\n\r ringNativeDiff \n\r {0}", ringNativeDiff);
                    //Logger.Log("\n\r gemNativeDiff \n\r {0}", gemNativeDiff);


                    Logger.Log(level, LogCategory.UserInformation, "------ Active Skills / Runes ------", SkillUtils.Active.Count, SkillUtils.Active.Count);

                    Action<Skill> logSkill = s =>
                    {
                        Logger.Log(level, LogCategory.UserInformation, "Skill: {0} Rune={1} Type={2}",
                            s.Name,
                            s.CurrentRune.Name,
                            (s.IsAttackSpender) ? "Spender" : (s.IsGeneratorOrPrimary) ? "Generator" : "Other"
                            );
                    };



                    SkillUtils.Active.ForEach(logSkill);

                    Logger.Log(level, LogCategory.UserInformation, "------ Passives ------", SkillUtils.Active.Count, SkillUtils.Active.Count);

                    Action<Passive> logPassive = p => Logger.Log(level, LogCategory.UserInformation, "Passive: {0}", p.Name);

                    PassiveUtils.Active.ForEach(logPassive);

                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in DebugUtil > LogBuildAndItems: {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        public static void LogSystemInformation(TrinityLogLevel level = TrinityLogLevel.Debug)
        {
            Logger.Log(level, LogCategory.UserInformation, "------ System Information ------");
            Logger.Log(level, LogCategory.UserInformation, "Processor: " + SystemInformation.Processor);
            Logger.Log(level, LogCategory.UserInformation, "Current Speed: " + SystemInformation.ActualProcessorSpeed);
            Logger.Log(level, LogCategory.UserInformation, "Operating System: " + SystemInformation.OperatingSystem);
            Logger.Log(level, LogCategory.UserInformation, "Motherboard: " + SystemInformation.MotherBoard);
            Logger.Log(level, LogCategory.UserInformation, "System Type: " + SystemInformation.SystemType);
            Logger.Log(level, LogCategory.UserInformation, "Free Physical Memory: " + SystemInformation.FreeMemory);
            Logger.Log(level, LogCategory.UserInformation, "Hard Drive: " + SystemInformation.HardDisk);
            Logger.Log(level, LogCategory.UserInformation, "Video Card: " + SystemInformation.VideoCard);
            Logger.Log(level, LogCategory.UserInformation, "Resolution: " + SystemInformation.Resolution);
        }


        internal static void DumpReferenceItems(TrinityLogLevel level = TrinityLogLevel.Debug)
        {

            var path = Path.Combine(FileManager.DemonBuddyPath, "Plugins\\TrinityPlugin\\Resources\\JS Class Generator\\ItemReference.js");

            if (File.Exists(path))
                File.Delete(path);

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine("var itemLookup = {");

                foreach (var item in Legendary.ToList())
                {
                    var key = !string.IsNullOrEmpty(item.Slug) ? item.Slug : RemoveApostophes(item.Name).ToLower();

                    if (item.Id != 0)
                        w.WriteLine(string.Format("     \"{0}\": [\"{1}\", {2}, \"{3}\"],", key, item.Name, item.Id, item.InternalName));
                }

                w.WriteLine("}");
            }

            Logger.Log("Dumped Reference Items to: {0}", path);
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
            var p = Logger.Prefix;
            Logger.Prefix = "";

            var dropItems = Legendary.ToList().Where(i => !i.IsCrafted && i.Id == 0).OrderBy(i => i.TrinityItemType).ToList();
            var craftedItems = Legendary.ToList().Where(i => i.IsCrafted && i.Id == 0).OrderBy(i => i.TrinityItemType).ToList();

            Logger.Log("Dropped Items: {0}", dropItems.Count);
            foreach (var item in dropItems)
            {
                Logger.Log("{0} - {1} = 0", item.TrinityItemType, item.Name);
            }

            Logger.Log(" ");
            Logger.Log("Crafted Items: {0}", craftedItems.Count);
            foreach (var item in craftedItems)
            {
                Logger.Log("{0} - {1} = 0", item.TrinityItemType, item.Name);
            }

            Logger.Prefix = p;
        }

        internal static void LogNewItems()
        {
            var knownIds = Legendary.ItemIds;

            using (new AquireFrameHelper())
            {
                if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                {
                    Logger.Log("Not in game");
                    return;
                }

                var allItems = new List<ACDItem>();
                allItems.AddRange(ZetaDia.Me.Inventory.StashItems);
                allItems.AddRange(ZetaDia.Me.Inventory.Equipped);
                allItems.AddRange(ZetaDia.Me.Inventory.Backpack);

                if (!allItems.Any())
                    return;

                var newItems = allItems.Where(i => i != null && i.IsValid && i.ItemQualityLevel == ItemQuality.Legendary && (i.ItemBaseType == ItemBaseType.Jewelry || i.ItemBaseType == ItemBaseType.Armor || i.ItemBaseType == ItemBaseType.Weapon) && !knownIds.Contains(i.ActorSnoId)).DistinctBy(p => p.ActorSnoId).OrderBy(i => i.ItemType).ToList();

                if (!newItems.Any())
                    return;

                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "------ New/Unknown Items {0} ------", newItems.Count);

                newItems.ForEach(i =>
                {
                    Logger.Log(string.Format("Item: {0}: {1} ({2})", i.ItemType, i.Name, i.ActorSnoId));
                });
            }
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
                var type = TrinityItemManager.DetermineItemType(name, ItemType.Unknown);
                if (type != TrinityItemType.Unknown || DataDictionary.GoldSNO.Contains(sno) ||
                    DataDictionary.ForceToItemOverrideIds.Contains(sno) || DataDictionary.HealthGlobeSNO.Contains(sno) || Legendary.ItemIds.Contains(sno))
                {
                    toLog.Add(string.Format("{{ {0}, TrinityItemType.{1} }}, // {2}", sno, type, name));
                }
            }

            var path = WriteLinesToLog("ItemSNOReference.log", toLog, true);
            Logger.Log("Finished Dumping Item SNO Reference to {0}", path);
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
            Logger.Log("Starting ItemList Backpack Test");

            var backpackItems = ZetaDia.Me.Inventory.Backpack.ToList();
            var total = backpackItems.Count;
            var stashCount = 0;

            foreach (var acdItem in backpackItems)
            {
                Logger.Log($"{acdItem.Name} ActorSnoId={acdItem.ActorSnoId} GameBalanceId={acdItem.GameBalanceId} ACDId={acdItem.ACDId} AnnId={acdItem.AnnId}");

                var cItem = new CachedItem(acdItem.BaseAddress);
                Logger.LogVerbose(cItem.Attributes.ToString());

                if (ItemListEvaluator.ShouldStashItem(cItem, true))
                    stashCount++;
            }

            Logger.Log("Finished ItemList Backpack Test");

            Logger.Log("Finished - Stash {0} / {1}", stashCount, total);
        }
    }
}
