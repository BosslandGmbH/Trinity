using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines.Resources
{
    /// <summary>
    /// Enum InventoryItemType - this is not finalized, i don't have all the items
    /// </summary>
    public enum InventoryItemType
    {
        None = 0,
        CommonDebris = 1,
        ReusableParts = 361984,
        ArcaneDust = 361985,
        ExquisiteEssence = 3,
        ShimmeringEssence = 4,
        SubtleEssence = 5,
        WishfulEssence = 6,
        DeathsBreath = 361989,
        DemonicEssence = 8,
        EncrustedHoof = 9,
        FallenTooth = 10,
        IridescentTear = 11,
        LizardEye = 12,
        VeiledCrystal = 361986,
        FieryBrimstone = 189863,
        ForgottenSoul = 361988,
        KeyOfBones = 364694,
        KeyOfEvil = 364697,
        KeyOfGluttony = 364695,
        KeyOfWar = 364696,
        KeyOfDestruction = 255880,
        KeyOfHate = 255881,
        KeyOfTerror = 255882,
		CaldeumNightshade = 364281,
		WestmarchHolyWater = 364975,
		ArreatWarTapestry = 364290,
		CorruptedAngelFlesh = 364305,
		KhanduranRune = 365020,
        BlackSmithPlan = 192598,
        JewelerPlan = 192600,
    }

    public enum ItemLocation
    {
        Unknown = 0,
        Backpack,
        Stash,
        Ground,
        Equipped,
    }

    public static class Inventory
    {
        static Inventory()
        {
            Materials = new MaterialStore(CraftingMaterialIds);
            Pulsator.OnPulse += OnPulse;
            GameEvents.OnWorldChanged += OnWorldChanged;
        }

        private static void OnWorldChanged(object sender, EventArgs eventArgs)
        {
            InvalidItemDynamicIds.Clear();
        }

        private static void OnPulse(object sender, EventArgs e)
        {
            if (ZetaDia.IsInGame && ZetaDia.IsInTown && Materials.TimeSinceUpdate.TotalMilliseconds > 1000)
                Materials.Update();
        }

        public static MaterialStore Materials { get; set; }

        public class MaterialStore
        {
            public MaterialStore(IEnumerable<int> actorIds)
            {
                Types = actorIds.Select(i => (InventoryItemType)i).ToList();
            }

            public MaterialStore(IList<InventoryItemType> types)
            {
                Types = types;
            }

            public IList<InventoryItemType> Types { get; set; }

            public Dictionary<InventoryItemType, MaterialRecord> Source = new Dictionary<InventoryItemType, MaterialRecord>();

            public void Update(bool updateAllProperties = false)
            {
                Source = GetMaterials(Types);
                LastUpdated = DateTime.UtcNow;
            }

            public DateTime LastUpdated = DateTime.MinValue;

            public TimeSpan TimeSinceUpdate
            {
                get { return DateTime.UtcNow.Subtract(LastUpdated);  }
            }

            public MaterialRecord this[InventoryItemType i]
            {
                get { return Source[i]; }
                set { Source[i] = value; }
            }

            public void LogCounts(string msg = "", TrinityLogLevel level = TrinityLogLevel.Info)
            {
                Logger.Log(level, msg + " " + GetCountsString(Source));
            }

            public MaterialRecord HighestCountMaterial(IEnumerable<InventoryItemType> types)
            {
                return Source.Where(m => types.Contains(m.Key)).OrderByDescending(pair => pair.Value.TotalStackQuantity).FirstOrDefault().Value;
            }

            public IEnumerable<KeyValuePair<InventoryItemType, MaterialRecord>> OfTypes(IEnumerable<InventoryItemType> types)
            {
                return Source.Where(m => types.Contains(m.Key));
            }

            public List<TrinityItem> GetStacksOfType(InventoryItemType type, InventorySlot location)
            {
                return Source[type].GetItemsByInventorySlot(location);
            }

            public List<TrinityItem> GetStacksOfType(InventoryItemType type, InventorySlot location, int quantity)
            {
                return GetStacksUpToQuantity(Source[type].GetItemsByInventorySlot(location),quantity).ToList();
            }

            public bool HasStackQuantityOfType(InventoryItemType type, InventorySlot location, int quantity)
            {
                return Source.Any() && Source[type].StackQuantityByInventorySlot(location) > quantity;
            }

            public bool HasStackQuantityOfTypes(IEnumerable<InventoryItemType> types, InventorySlot location, int quantity)
            {
                return Source.Where(pair => types.Contains(pair.Key)).All(pair => HasStackQuantityOfType(pair.Key, location, quantity));
            }

            public bool HasRecipe(Dictionary<InventoryItemType, int> recipe, InventorySlot location)
            {
                return recipe.All(entry => HasStackQuantityOfType(entry.Key, location, entry.Value));
            }

            public List<TrinityItem> GetRecipeItems(Dictionary<InventoryItemType, int> recipe, InventorySlot location)
            {
                return recipe.SelectMany(i => GetStacksOfType(i.Key, location, i.Value)).ToList();
            }
        }


        public static IEnumerable<TrinityItem> GetStacksUpToQuantity(List<TrinityItem> materialsStacks, int maxStackQuantity)
        {
            if (materialsStacks == null || !materialsStacks.Any() || materialsStacks.Count == 1)
                return materialsStacks;

            long dbQuantity = 0;
            var overlimit = 0;            

            // First of Non-Stackable Items
            var first = materialsStacks.First();
            if (first.ItemStackQuantity == 0 && maxStackQuantity == 1 && materialsStacks.All(i => !i.IsCraftingReagent))
                return new List<TrinityItem> { first };

            // Position in the cube matters; it looks like it will fail if
            // stacks are added after the required amount of ingredient is met, 
            // as the cube encounters them from top left to bottom right.

            var toBeAdded = materialsStacks.TakeWhile(db =>
            {            
                var thisStackQuantity = db.ItemStackQuantity;

                if (dbQuantity + thisStackQuantity < maxStackQuantity)
                {
                    dbQuantity += thisStackQuantity;
                    return true;
                }
                overlimit++;
                return overlimit == 1;
            });

            return toBeAdded.ToList();
        }

        public class MaterialRecord
        {
            public int ActorId { get; set; }
            public InventoryItemType Type { get; set; }

            public List<TrinityItem> StashItems = new List<TrinityItem>();
            public List<TrinityItem> BackpackItems = new List<TrinityItem>();

            public long Total
            {
                get { return StashItemCount + BackpackItemCount; }
            }

            public long StashItemCount
            {
                get { return StashItems.Count; }
            }

            public long BackpackItemCount
            {
                get { return BackpackItems.Count; }
            }

            private long? _backpackStackQuantity;
            public long BackpackStackQuantity
            {
                get { return _backpackStackQuantity ?? (_backpackStackQuantity = BackpackItems.Where(i => i.IsValid).Select(i => i.ItemStackQuantity).Sum()).Value; }
            }

            private long? _stashStackQuantity;
            public long StashStackQuantity
            {
                get { return _stashStackQuantity ?? (_stashStackQuantity = StashItems.Where(i => i.IsValid).Select(i => i.ItemStackQuantity).Sum()).Value; }
            }

            private long? _totalStackQuantity;
            public long TotalStackQuantity
            {
                get { return _totalStackQuantity ?? (_totalStackQuantity = StashStackQuantity + BackpackStackQuantity).Value; }
            }

            public long StackQuantityByInventorySlot(InventorySlot slot)
            {
                switch (slot)
                {
                    case InventorySlot.BackpackItems: return BackpackStackQuantity;
                    case InventorySlot.SharedStash: return StashStackQuantity;
                    default: return TotalStackQuantity;
                }
            }

            public List<TrinityItem> GetItemsByInventorySlot(params InventorySlot[] slots)
            {
                var items = new List<TrinityItem>();
                foreach (var slot in slots)
                {
                    switch (slot)
                    {
                        case InventorySlot.BackpackItems:
                            items.AddRange(BackpackItems);
                            break;
                        case InventorySlot.SharedStash:
                            items.AddRange(StashItems);
                            break;
                    }
                }
                return items;
            }
        }

        public static string GetCountsString(Dictionary<InventoryItemType, MaterialRecord> materials)
        {
            var backpack = string.Empty;
            var stash = string.Empty;
            var total = string.Empty;

            foreach (var item in materials)
            {
                backpack += $"{item.Key}={item.Value.BackpackStackQuantity} ";
                stash += $"{item.Key}={item.Value.StashStackQuantity} ";
                total += $"{item.Key}={item.Value.TotalStackQuantity} ";
            }
            return $"Backpack: [{backpack.Trim()}] \r\nStash: [{stash.Trim()}]\r\n Total: [{total.Trim()}]\r\n";
        }

        public static Dictionary<InventoryItemType, MaterialRecord> GetMaterials(IList<InventoryItemType> types)
        {
            var materials = types.ToDictionary(t => t, v => new MaterialRecord());
            var materialSNOs = new HashSet<int>(types.Select(m => (int)m));

            foreach (var item in Backpack.Items)
            {
                if (InvalidItemDynamicIds.Contains(item.AnnId))
                    continue;

                if (!item.IsValid || item.IsCraftingReagent && item.ItemStackQuantity == 0)
                {
                    Logger.LogVerbose("Invalid item skipped: {0}", item.InternalName);
                    //InvalidItemDynamicIds.Add(item.AnnId);
                    continue;
                }
                    
                var itemSNO = item.ActorSnoId;
                if (materialSNOs.Contains(itemSNO))
                {
                    var type = (InventoryItemType)itemSNO;
                    var materialRecord = materials[type];
                    materialRecord.BackpackItems.Add(item);
                    materialRecord.Type = type;
                    materialRecord.ActorId = itemSNO;
                }
            }

            foreach (var item in Stash.Items)
            {
                if (InvalidItemDynamicIds.Contains(item.AnnId))
                    continue;

                if (!item.IsValid || item.IsCraftingReagent && item.ItemStackQuantity == 0)
                {
                    Logger.LogVerbose("Invalid item skipped: {0}", item.InternalName);
                    //InvalidItemDynamicIds.Add(item.AnnId);
                    continue;
                }

                var itemSNO = item.ActorSnoId;
                if (materialSNOs.Contains(itemSNO))
                {
                    var type = (InventoryItemType) itemSNO;
                    var materialRecord = materials[type];
                    materialRecord.StashItems.Add(item);
                    materialRecord.Type = type;
                    materialRecord.ActorId = itemSNO;
                }
            }

            return materials;
        }

        private static HashSet<int> _blacklistedDynamicIds;
        public static HashSet<int> InvalidItemDynamicIds
        {
            get { return _blacklistedDynamicIds ?? (_blacklistedDynamicIds = new HashSet<int>()); }
            set { _blacklistedDynamicIds = value; }
        }

        public static HashSet<InventoryItemType> MaterialConversionTypes = new HashSet<InventoryItemType>
        {
            InventoryItemType.ArcaneDust,
            InventoryItemType.ReusableParts,
            InventoryItemType.VeiledCrystal,
        };

        public static HashSet<InventoryItemType> CraftingMaterialTypes = new HashSet<InventoryItemType>
        {
            InventoryItemType.ArcaneDust,
            InventoryItemType.ReusableParts,
            InventoryItemType.VeiledCrystal,
            InventoryItemType.CaldeumNightshade,
            InventoryItemType.DeathsBreath,
            InventoryItemType.WestmarchHolyWater,
            InventoryItemType.ArreatWarTapestry,
            InventoryItemType.CorruptedAngelFlesh,
            InventoryItemType.KhanduranRune,
            InventoryItemType.ForgottenSoul,
        };

        public static HashSet<int> CraftingMaterialIds = new HashSet<int>
        {
            361985, //Type: Item, Name: Arcane Dust
            361984, //Type: Item, Name: Reusable Parts
            361986, //Type: Item, Name: Veiled Crystal
            364281, //Type: Item, Name: Caldeum Nightshade
            361989, //Type: Item, Name: Death's Breath
            364975, //Type: Item, Name: Westmarch Holy Water
            364290, //Type: Item, Name: Arreat War Tapestry
            364305, //Type: Item, Name: Corrupted Angel Flesh
            365020, //Type: Item, Name: Khanduran Rune
            361988, //Type: Item, Name: Forgotten Soul
        };

        public static HashSet<int> MaterialConversionIds = new HashSet<int>
        {
            361985, //Type: Item, Name: Arcane Dust
            361984, //Type: Item, Name: Reusable Parts
            361986, //Type: Item, Name: Veiled Crystal
            361989, //Type: Item, Name: Death's Breath
        };

        public static HashSet<int> RareUpgradeIds = new HashSet<int>
        {
            361985, //Type: Item, Name: Arcane Dust
            361984, //Type: Item, Name: Reusable Parts
            361986, //Type: Item, Name: Veiled Crystal
            361989, //Type: Item, Name: Death's Breath
        };

        public static HashSet<int> SetRollingIds = new HashSet<int>
        {
            361989, //Type: Item, Name: Death's Breath
            361988, //Type: Item, Name: Forgotten Soul
        };

        public static HashSet<int> PowerExtractionIds = new HashSet<int>
        {
            364281, //Type: Item, Name: Caldeum Nightshade
            361989, //Type: Item, Name: Death's Breath
            364975, //Type: Item, Name: Westmarch Holy Water
            364290, //Type: Item, Name: Arreat War Tapestry
            364305, //Type: Item, Name: Corrupted Angel Flesh
            365020, //Type: Item, Name: Khanduran Rune
            361988, //Type: Item, Name: Forgotten Soul
        };



        public static List<TrinityItem> OfType(IEnumerable<InventoryItemType> types)
        {
            var typesHash = new HashSet<int>(types.Select(t => (int)t));
            return AllItems.Where(i => typesHash.Contains(i.ActorSnoId)).ToList();
        }

        public static List<TrinityItem> OfType(params InventoryItemType[] types)
        {
            var typesHash = new HashSet<int>(types.Select(t => (int)t));
            return AllItems.Where(i => typesHash.Contains(i.ActorSnoId)).ToList();
        }

        public static List<TrinityItem> OfType(InventoryItemType type)
        {
            return AllItems.Where(i => i.ActorSnoId == (int)type).ToList();
        }

        public static List<TrinityItem> ByItemType(ItemType type)
        {
            return AllItems.Where(i => i.ItemType == type).ToList();
        }

        public static List<TrinityItem> ByActorSNO(int ActorSNO)
        {
            return AllItems.Where(i => i.ActorSnoId == ActorSNO).ToList();
        }

        public static string LastLogInfo;

        public static IEnumerable<TrinityItem> AllItems
        {
            get
            {
                //var items = ZetaDia.Actors.GetActorsOfType<ACDItem>(true).Where(i =>
                //var items = Core.Actors.Inventory.Where(i =>
                //{
                //    //if (!i.IsValid || i.IsDisposed)
                //    //{
                //    //    Logger.LogVerbose($"Inventory Skipping: {i.InternalName} ({i.ActorSnoId}) due to not valid or disposed");
                //    //    return false;
                //    //}

                //    if (i.InventorySlot != InventorySlot.BackpackItems && i.InventorySlot != InventorySlot.SharedStash)
                //    {
                //        return false;
                //    }

                //    //if (InvalidItemDynamicIds.Contains(i.AnnId))
                //    //{
                //    //    Logger.LogVerbose($"Inventory Skipping: {i.Name} ({i.ActorSnoId}) due to InvalidItemDynamicId ({i.AnnId})");
                //    //    return false;
                //    //}

                //    if (!i.IsValid)
                //    {
                //        Logger.LogVerbose($"Inventory Skipping: {i.Name} ({i.ActorSnoId}) is invalid or disposed");
                //    }

                //    //todo: perf optimize
                //    var stackQuantity = i.ItemStackQuantity;
                //    var isEquipment = !i.IsCraftingReagent && stackQuantity == 0;
                //    var isCraftingMaterial = i.IsCraftingReagent && stackQuantity > 0;
                //    var isGem = i.IsGem && stackQuantity > 0;
                //    var isPotion = i.IsPotion && stackQuantity == 0;
                //    var isMisc = i.IsMiscItem || i.ItemBaseType == ItemBaseType.Misc;
                //    var isCraftingBook = i.ItemType == ItemType.CraftingPage || i.ItemType == ItemType.CraftingPlan;
                //    var isCraftingPlan = i.ItemType == ItemType.CraftingPlan && stackQuantity == 1;

                //    var isValid = isEquipment || isCraftingMaterial || isCraftingPlan || isPotion || isGem || isMisc || isCraftingBook;

                //    if (!isValid)                 
                //        Logger.LogVerbose($"Inventory Skipping: {i.Name} ({i.ActorSnoId}), unknown item");

                //    return isValid;

                //}).ToList();

                return Core.Actors.Inventory.Where(i => !InvalidItemDynamicIds.Contains(i.AnnId));
            }
        }

        public static bool IsBadData(ACDItem i)
        {
            var stackQuantity = i.ItemStackQuantity;
            var isCraftingReagent = i.IsCraftingReagent;

            if (!i.IsValid || i.IsDisposed)
                return true;

            if (i.IsUnidentified && Core.Settings.Loot.TownRun.KeepLegendaryUnid)
                return false;

            return i.Name == i.ItemType.ToString() || isCraftingReagent && stackQuantity == 0 || !isCraftingReagent && !i.IsGem && i.ItemBaseType != ItemBaseType.Misc && stackQuantity > 0;
        }

        public static void LogBadData(ACDItem i, string prefix)
        {
            var stackQuantity = i.ItemStackQuantity;
            var isCraftingReagent = i.IsCraftingReagent;

            if (!i.IsValid)
            {
                Logger.LogVerbose($"{prefix} Bad Data Found: {i.Name} ({i.ActorSnoId}), is memory address not valid");
                return;
            }

            if (i.IsDisposed)
            {
                Logger.LogVerbose($"{prefix} Bad Data Found: {i.Name} ({i.ActorSnoId}), is disposed");
                return;
            }

            if (i.IsUnidentified)
                return;

            if (i.Name == i.ItemType.ToString())
            {
                Logger.LogVerbose($"{prefix} Bad Data Found: {i.Name} ({i.ActorSnoId}), name matches the item type {i.ItemType}");
                return;
            }

            if (stackQuantity == 0 && i.MaxStackCount > 0 || stackQuantity > 0 && i.MaxStackCount == 0)
            {
                Logger.LogVerbose($"{prefix} Bad Data Found: {i.Name} ({i.ActorSnoId}), stack count mismatch current={stackQuantity}  max={i.MaxStackCount}");
                return;
            }

            if (i.ItemQualityLevel == ItemQuality.Invalid)
            {
                Logger.LogVerbose($"{prefix} Bad Data Found: {i.Name} ({i.ActorSnoId}), item quality level is 'Invalid' LinkColorQuality={i.GetItemQuality()}");
                return;
            }
        }

        public static class Backpack
        {
            public static List<TrinityItem> Items
            {
                get { return AllItems.Where(i => i.InventorySlot == InventorySlot.BackpackItems).ToList(); }
            }

            public static List<TrinityItem> OfType(IEnumerable<InventoryItemType> types)
            {
                var typesHash = new HashSet<int>(types.Select(t => (int)t));
                return Items.Where(i => i.IsValid && typesHash.Contains(i.ActorSnoId)).ToList();
            }

            public static List<TrinityItem> OfType(params InventoryItemType[] types)
            {
                var typesHash = new HashSet<int>(types.Select(t => (int)t));
                return Items.Where(i => typesHash.Contains(i.ActorSnoId)).ToList();
            }

            public static List<TrinityItem> OfType(InventoryItemType type)
            {
                return Items.Where(i => i.ActorSnoId == (int)type).ToList();
            }

            public static List<TrinityItem> ByItemType(ItemType type)
            {
                return Items.Where(i => i.ItemType == type).ToList();
            }

            public static List<TrinityItem> ByActorSNO(int ActorSNO)
            {
                return Items.Where(i => i.ActorSnoId == ActorSNO).ToList();
            }

            public static List<TrinityItem> ArcaneDust
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ArcaneDust).ToList(); }
            }

            public static List<TrinityItem> ReusableParts
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ReusableParts).ToList(); }
            }

            public static List<TrinityItem> VeiledCrystals
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.VeiledCrystal).ToList(); }
            }

            public static List<TrinityItem> DeathsBreath
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.DeathsBreath).ToList(); }
            }
			
            public static List<TrinityItem> ForgottenSoul
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ForgottenSoul).ToList(); }
            }			
			
            public static List<TrinityItem> CaldeumNightshade
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.CaldeumNightshade).ToList(); }
            }			

			public static List<TrinityItem> WestmarchHolyWater
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.WestmarchHolyWater).ToList(); }
            }		
			
			public static List<TrinityItem> ArreatWarTapestry
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ArreatWarTapestry).ToList(); }
            }			

			public static List<TrinityItem> CorruptedAngelFlesh
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.CorruptedAngelFlesh).ToList(); }
            }	
			
			public static List<TrinityItem> KhanduranRune
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.KhanduranRune).ToList(); }
            }

            public static void Update()
            {
                foreach (var item in Items)
                {
                    item.OnUpdated();
                }
            }
        }

        public static class Stash
        {
            public static List<TrinityItem> Items
            {
                get { return AllItems.Where(i => i.InventorySlot == InventorySlot.SharedStash).ToList(); }
            }

            public static List<TrinityItem> OfType(IEnumerable<InventoryItemType> types)
            {
                var typesHash = new HashSet<int>(types.Select(t => (int)t));
                return Items.Where(i => typesHash.Contains(i.ActorSnoId)).ToList();
            }

            public static List<TrinityItem> OfType(params InventoryItemType[] types)
            {
                var typesHash = new HashSet<int>(types.Select(t => (int)t));
                return Items.Where(i => typesHash.Contains(i.ActorSnoId)).ToList();
            }

            public static List<TrinityItem> OfType(InventoryItemType type)
            {
                return Items.Where(i => i.ActorSnoId == (int)type).ToList();
            }

            public static List<TrinityItem> ByItemType(ItemType type)
            {
                return Items.Where(i => i.ItemType == type).ToList();
            }

            public static List<TrinityItem> ByActorSNO(int ActorSNO)
            {
                return Items.Where(i => i.ActorSnoId == ActorSNO).ToList();
            }

            public static List<TrinityItem> ArcaneDust
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ArcaneDust).ToList(); }
            }

            public static List<TrinityItem> ReusableParts
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ReusableParts).ToList(); }
            }

            public static List<TrinityItem> VeiledCrystals
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.VeiledCrystal).ToList(); }
            }

            public static List<TrinityItem> DeathsBreath
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.DeathsBreath).ToList(); }
            }
			
            public static List<TrinityItem> ForgottenSoul
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ForgottenSoul).ToList(); }
            }			
			
            public static List<TrinityItem> CaldeumNightshade
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.CaldeumNightshade).ToList(); }
            }			

			public static List<TrinityItem> WestmarchHolyWater
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.WestmarchHolyWater).ToList(); }
            }		
			
			public static List<TrinityItem> ArreatWarTapestry
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.ArreatWarTapestry).ToList(); }
            }			

			public static List<TrinityItem> CorruptedAngelFlesh
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.CorruptedAngelFlesh).ToList(); }
            }	
			
			public static List<TrinityItem> KhanduranRune
            {
                get { return Items.Where(i => i.ActorSnoId == (int)InventoryItemType.KhanduranRune).ToList(); }
            }	            			
        }


    }
}

