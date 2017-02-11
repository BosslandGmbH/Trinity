using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Trinity.UI.UIComponents;
using System.Collections.Specialized;
using System;

namespace Trinity.Components.Adventurer.Settings
{
    [DataContract]
    public class AdventurerGems : NotifyBase
    {
        //public AdventurerGems()
        //{
        //    GemSettings.CollectionChanged += GemSettings_CollectionChanged;
        //}

        //private void GemSettings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    using (GemSettings.DeferRefresh)
        //    {
        //        var list = sender as IList<AdventurerGemSetting>;
        //        if (list == null)
        //            return;

        //        foreach (var item in e.NewItems.Cast<AdventurerGemSetting>())
        //        {
        //            item.Order = list.IndexOf(item);
        //        }
        //    }
        //}

        public void UpdateOrder(IList<AdventurerGemSetting> list)
        {
            foreach (var item in list)
            {
                item.Order = list.IndexOf(item);
            }
        }

        FullyObservableCollection<AdventurerGemSetting> _gemSettings;

        /// <summary>
        /// A list of gem settings and fixed data distict by TYPE of gem
        /// </summary>
        [DataMember]
        public FullyObservableCollection<AdventurerGemSetting> GemSettings
        {
            get { return _gemSettings ?? (_gemSettings = GetDefaultGemSettings()); }
            set { LoadGemSettings(value); } // Called with value deserialized from from XML
        }

        private void LoadGemSettings(IEnumerable<AdventurerGemSetting> value)
        {
            using (GemSettings.DeferRefresh)
            {
                foreach (var gem in GemSettings)
                {
                    var setting = value.FirstOrDefault(g => g.Sno == gem.Sno);
                    gem.Order = setting.Order;
                    gem.IsLimited = setting.IsLimited;
                    gem.IsEnabled = setting.IsEnabled;
                    gem.Limit = setting.Limit;
                }
                GemSettings.OrderBy(b => b.Order);
            }
        }

        private static FullyObservableCollection<AdventurerGemSetting> GetDefaultGemSettings()
        {
            var gems = Reference.Gems.ToList().OrderByDescending(o => o.Importance).Select(g => new AdventurerGemSetting(g));
            return new FullyObservableCollection<AdventurerGemSetting>(gems);
        }

        /// <summary>
        /// A list of actual gem instances in the players current game
        /// </summary>
        public List<AdventurerGem> Gems { get; set; }

        private static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        /// <summary>
        /// References gem list to match the actual gems in players backpack/stash.
        /// </summary>
        public void UpdateGems(int greaterRiftLevel)
        {
            if (ZetaDia.Me == null)
                return;

            Core.Actors.Update();

            Gems = Core.Actors.Inventory
                    .Where(i => i.IsValid && i.ItemType == ItemType.LegendaryGem)
                    .Select(i => new AdventurerGem(i, greaterRiftLevel))
                    .Distinct(new AdventurerGemComparer())
                    .ToList();

            UpdateGemSettings(Gems);
        }

        private void UpdateGemSettings(List<AdventurerGem> freshGemsList)
        {
            if (freshGemsList == null || !freshGemsList.Any())
                return;

            var gemsBySno = freshGemsList.ToLookup(k => k.SNO, v => v);
            foreach (var gemSetting in GemSettings)
            {
                var gems = gemsBySno[gemSetting.Sno];
                if(gems.Any())
                {
                    gemSetting.HighestRank = gems.Max(g => g.Rank);
                }
                gemSetting.GemCount = gems.Count();
                gemSetting.IsEquipped = gems.Any(g => g.IsEquiped);
                gemSetting.Gems = gems;
            }
        }

        public ACDItem GetUpgradeTarget()
        {
            ZetaDia.Actors.Update();

            var minChance = PluginSettings.Current.GreaterRiftGemUpgradeChance;
            var level = ZetaDia.Me.InTieredLootRunLevel + 1;
            var focus = PluginSettings.Current.GemUpgradeFocusMode;
            var equipPriority = PluginSettings.Current.GreaterRiftPrioritizeEquipedGems;
            var chanceReq = PluginSettings.Current.GreaterRiftGemUpgradeChance;

            UpdateGems(level);

            Logger.Log($"[UpgradeGems] ---- Gem Upgrade Summary ----");
            Logger.Log($"[UpgradeGems] Current Rift Level: {level}");
            Logger.Log($"[UpgradeGems] Gem Count: {Gems.Count}");
            Logger.Log($"[UpgradeGems] Highest Ranked Gem: {Gems.Max(g => g.Rank)}");
            Logger.Log($"[UpgradeGems] Lowest Ranked Gem: {Gems.Min(g => g.Rank)}");
            Logger.Log($"[UpgradeGems] Upgrade Chance Setting: {minChance}%");
            Logger.Log($"[UpgradeGems] Focus Mode: {focus}");
            Logger.Log($"[UpgradeGems] Prioritize Equipped: {equipPriority}");

            var gems = Gems.ToList();

            Logger.Log($"[UpgradeGems] ---- Excluded: User Disabled Type ----");

            foreach (var gem in gems.ToList())
            {
                if (!gem.Settings.IsEnabled)
                {
                    Logger.Log($"[UpgradeGems] {gem.Name} ({gem.SNO}) Id={gem.Guid} Rank={gem.Rank}");
                    gems.Remove(gem);
                }
            }

            Logger.Log($"[UpgradeGems] ---- Excluded: By Max Rank ----");

            foreach (var gem in gems.ToList())
            {
                if (gem.Rank >= gem.Settings.MaxRank)
                {
                    Logger.Log($"[UpgradeGems] {gem.Name} ({gem.SNO}) Id={gem.Guid} Rank={gem.Rank} MaxRank={gem.Settings.MaxRank}");
                    gems.Remove(gem);
                }
            }

            Logger.Log($"[UpgradeGems] ---- Excluded: User Rank Limit ----");

            foreach (var gem in gems.ToList())
            {
                if (gem.Settings.IsLimited && gem.Rank >= gem.Settings.Limit)
                {
                    Logger.Log($"[UpgradeGems] {gem.Name} ({gem.SNO}) Id={gem.Guid} Rank={gem.Rank} Limit={(!gem.Settings.IsLimited ? "None" : gem.Settings.Limit.ToString())}");
                    gems.Remove(gem);
                }
            }

            Logger.Log($"[UpgradeGems] ---- Excluded: Below Chance ({minChance}%) ----");

            foreach (var gem in gems.ToList())
            {
                if (gem.UpgradeChance < chanceReq)
                {
                    Logger.Log($"[UpgradeGems] {gem.Name} ({gem.SNO}) Id={gem.Guid} Rank={gem.Rank} Chance={gem.UpgradeChance}");
                    gems.Remove(gem);
                }
            }

            if (focus)
            {
                Logger.Log($"[UpgradeGems] ---- Ordered Candidates ({gems.Count}), by {(equipPriority ? "Equipped, " : "")}Order, Rank - Focus Mode ----");
                gems = gems.OrderBy(g => equipPriority && g.IsEquiped ? 0 : 1).ThenBy(g => g.Settings.Order).ToList(); ;
            }
            else
            {
                Logger.Log($"[UpgradeGems] ---- Ordered Candidates ({gems.Count}), by {(equipPriority ? "Equipped, " : "")}Chance, Order, Rank ----");
                gems = gems.OrderBy(g => equipPriority && g.IsEquiped ? 0 : 1).ThenByDescending(g => g.UpgradeChance).ThenBy(g => g.Settings.Order).ThenBy(g => g.Rank).ToList();
            }

            for (int i = 0; i < gems.Count; i++)
            {
                var gem = gems.ElementAtOrDefault(i);
                Logger.Log($"[UpgradeGems] #{(i + 1)}: {gem.Name} ({gem.SNO}) Id={gem.Guid} Rank={gem.Rank} Chance={gem.UpgradeChance} @{level} Order={gem.Settings.Order} Limit={(gem.Settings.IsLimited ? "None" : gem.Settings.Limit.ToString())} Equipped={gem.IsEquiped}");
            }

            if (gems.Count == 0)
            {
                Util.Logger.Info("[UpgradeGems] Couldn't find any gems over the minimum upgrade chance, upgrading the gem with highest upgrade chance");
                gems = Gems.Where(g => !g.IsMaxRank).OrderByDescending(g => g.UpgradeChance).ToList();
            }

            ACDItem acdGem = null;

            var gemToUpgrade = gems.FirstOrDefault();
            if (gemToUpgrade != null)
            {
                Logger.Log($"[UpgradeGems] ---- Selection ----");
                Logger.Log($"[UpgradeGems] Attempting to upgrade {gemToUpgrade.DisplayName} ({gemToUpgrade.SNO}) Rank={gemToUpgrade.Rank} Chance={gemToUpgrade.UpgradeChance}%");
                acdGem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(i => gemToUpgrade.Guid == i.AnnId);
            }

            if (acdGem == null)
            {
                acdGem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(i => i.ItemType == ItemType.LegendaryGem);
                Logger.Log($"[UpgradeGems] AcdItem Not Found {gemToUpgrade?.DisplayName} - Using {acdGem?.Name} so the quest can be completed");
            }

            return acdGem;
        }

        private AdventurerGem GetMatchingInventoryGem(AdventurerGem gem, List<AdventurerGem> inventoryGems)
        {
            return inventoryGems.FirstOrDefault(g => g.SNO == gem.SNO && g.Rank >= gem.Rank);
        }

        public class AdventurerGemComparer : IEqualityComparer<AdventurerGem>
        {
            public bool Equals(AdventurerGem x, AdventurerGem y)
            {
                return x.Guid == y.Guid;
            }

            public int GetHashCode(AdventurerGem obj)
            {
                return obj.Guid;
            }
        }
    }
}