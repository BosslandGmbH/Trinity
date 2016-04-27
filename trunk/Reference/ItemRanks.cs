using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Objects;
using Trinity.Settings.Loot;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Reference
{
    public partial class ItemRanks : FieldCollection<ItemRanks, ItemRank>
    {
        #region Methods
        internal static bool ShouldStashItem(CachedACDItem cItem)
        {
            if (cItem.AcdItem != null && cItem.AcdItem.IsValid)
            {
                bool result = false;
                var item = new Item(cItem.AcdItem);
                var wrappedItem = new ItemWrapper(cItem.AcdItem);

                if (Trinity.Settings.Loot.ItemRank.AncientItemsOnly && wrappedItem.IsEquipment && !cItem.IsAncient)
                {
                    result = false;
                }
                else if (Trinity.Settings.Loot.ItemRank.RequireSocketsOnJewelry && wrappedItem.IsJewelry && cItem.AcdItem.NumSockets != 0)
                {
                    result = false;
                }
                else
                {
                    result = ShouldStashItem(item);
                }
                string action = result ? "KEEP" : "TRASH";
                Logger.Log(LogCategory.ItemValuation, "Ranked Item - {0} - {1}", action, item.Name);

                return result;
            }
            return false;
        }

        internal static bool ShouldStashItem(Item item)
        {
            return GetRankedItemsFromSettings().Any(i => i.Item.Equals(item));
        }

        private static HashSet<int> _itemIds;

        public static HashSet<int> ItemIds
        {
            get
            {
                return _itemIds ?? (_itemIds = new HashSet<int>(ToList()
                    .Select(i => i.Item.Id)));
            }
        }

        public static IEnumerable<ItemRank> GetRankedItems(ActorClass actorClass, double minPercent = 10, int minSampleSize = 10, int betterThanRank = 5)
        {
            var list = ToList().Where(i => i.SoftcoreRank.Any(ird =>
                ird.Rank <= betterThanRank &&
                ird.Class == actorClass &&
                ird.PercentUsed >= minPercent &&
                ird.SampleSize >= minSampleSize));

            List<ItemRank> result = new List<ItemRank>();
            foreach (var ir in list)
            {
                var newIr = new ItemRank { Item = ir.Item };
                foreach (var ird in ir.SoftcoreRank.Where(ird => ird != null && ird.Rank <= betterThanRank && ird.Class == actorClass && ird.PercentUsed >= minPercent && ird.SampleSize >= minSampleSize))
                {
                    newIr.SoftcoreRank.Add(ird);
                }
                foreach (var ird in ir.HardcoreRank.Where(ird => ird != null && ird.Rank <= betterThanRank && ird.Class == actorClass && ird.PercentUsed >= minPercent && ird.SampleSize >= minSampleSize))
                {
                    newIr.HardcoreRank.Add(ird);
                }
                if (newIr.SoftcoreRank.Any())
                    result.Add(newIr);
            }

            return result;
        }

        public static HashSet<int> GetRankedIds(ActorClass actorClass, double minPercent = 10, int minSampleSize = 10, int betterThanRank = 5)
        {
            return new HashSet<int>(GetRankedItems(actorClass, minPercent, minSampleSize, betterThanRank).Select(v => v.Item.Id));
        }

        private static double lastSettingSignature;
        private static List<ItemRank> LastRankedItemsList = new List<ItemRank>();
        public static List<ItemRank> GetRankedItemsFromSettings()
        {
            var irs = Trinity.Settings.Loot.ItemRank;
            var settingSignature = (int)Trinity.Player.ActorClass + (int)irs.ItemRankMode + irs.MinimumRank + irs.MinimumSampleSize + irs.MinimumPercent;
            if (settingSignature == lastSettingSignature)
                return LastRankedItemsList;

            lastSettingSignature = settingSignature;
            LastRankedItemsList = GetRankedItemsFromSettings(Trinity.Settings.Loot.ItemRank);
            return LastRankedItemsList;
        }

        public static List<ItemRank> GetRankedItemsFromSettings(ItemRankSettings irs)
        {
            List<ItemRank> ird = new List<ItemRank>();

            if (irs.ItemRankMode == ItemRankMode.AnyClass)
            {
                foreach (ActorClass actor in Enum.GetValues(typeof(ActorClass)).Cast<ActorClass>())
                {
                    if (actor == ActorClass.Invalid)
                        continue;
                    foreach (ItemRank itemRank in GetRankedItems(actor, irs.MinimumPercent, irs.MinimumSampleSize, irs.MinimumRank))
                    {
                        ird.Add(itemRank);
                    }
                }
            }
            else if (ZetaDia.Me.IsFullyValid())
            {
                ird.AddRange(GetRankedItems(ZetaDia.Me.ActorClass, irs.MinimumPercent, irs.MinimumSampleSize, irs.MinimumRank));
            }
            return ird;
        }

        private static Dictionary<int, ItemRank> _itemRankDictionary = new Dictionary<int, ItemRank>();

        public static ItemRank GetItemRank(ACDItem item)
        {
            if (!_itemRankDictionary.Any())
                _itemRankDictionary = ToList().ToDictionary(k => k.Item.Id, k => k);

            ItemRank itemRank;

            if (_itemRankDictionary.TryGetValue(item.ActorSnoId, out itemRank))
            {
                return itemRank;
            }

            return null;
        }

        private static Dictionary<ActorClass, HashSet<int>> _highRankedIds;

        public static Dictionary<ActorClass, HashSet<int>> HighRankedIds
        {
            get
            {
                if (_highRankedIds != null)
                    return _highRankedIds;

                _highRankedIds = new Dictionary<ActorClass, HashSet<int>>();

                foreach (ActorClass actorClass in (ActorClass[])Enum.GetValues(typeof(ActorClass)))
                {
                    var irs = GetRankedItems(actorClass, 15, 10, 4);

                    var idsHash = new HashSet<int>(irs.Select(v => v.Item.Id));

                    _highRankedIds.Add(actorClass, idsHash);
                }

                return _highRankedIds;
            }
        }

        #endregion
    }
}