using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using System.Runtime.Serialization;
using System.Threading;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Settings
{
    [DataContract]
    public class AdventurerGems : NotifyBase
    {
        public void UpdateOrder(IList<AdventurerGemSetting> list)
        {
            foreach (var item in list)
            {
                item.Order = list.IndexOf(item);
            }
        }

        private FullyObservableCollection<AdventurerGemSetting> _gemSettings;

        /// <summary>
        /// A list of gem settings and fixed data distict by TYPE of gem
        /// </summary>
        [DataMember]
        public FullyObservableCollection<AdventurerGemSetting> GemSettings
        {
            get { return _gemSettings ?? (_gemSettings = GetDefaultGemSettings()); }
            set { LoadGemSettings(value); } // Called with value deserialized from XML
        }

        /// <summary>
        /// Update gem settings records with partial information from XML Save file.
        /// </summary>
        private void LoadGemSettings(IEnumerable<AdventurerGemSetting> value)
        {
            using (GemSettings.DeferRefresh)
            {
                foreach (var gem in _gemSettings)
                {
                    var setting = value.FirstOrDefault(g => g.Sno == gem.Sno);
                    gem.Order = setting.Order;
                    gem.IsLimited = setting.IsLimited;
                    gem.IsEnabled = setting.IsEnabled;
                    gem.Limit = setting.Limit;
                }
                _gemSettings = new FullyObservableCollection<AdventurerGemSetting>(GemSettings.OrderBy(b => b.Order));
            }
        }

        /// <summary>
        /// Populate gem settings with every possible gem type using the gem reference.
        /// </summary>
        private static FullyObservableCollection<AdventurerGemSetting> GetDefaultGemSettings()
        {
            var gems = Framework.Reference.Gems.ToList().OrderByDescending(o => o.Importance).Select(g => new AdventurerGemSetting(g));
            return new FullyObservableCollection<AdventurerGemSetting>(gems);
        }

        /// <summary>
        /// A list of actual gem instances in the players current game
        /// </summary>
        public List<AdventurerGem> Gems { get; set; } = new IndexedList<AdventurerGem>();

        private static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        /// <summary>
        /// Updates gem list to match the actual gems in players backpack/stash.
        /// </summary>
        public void UpdateGems(int greaterRiftLevel)
        {
            if (ZetaDia.Me == null)
                return;

            if(Thread.CurrentThread.ManagedThreadId == BotMain.BotThread?.ManagedThreadId || !BotMain.IsRunning)
                Core.Actors.Update();

            Gems = Core.Actors.Inventory
                    .Where(i => i.IsValid && i.ItemType == ItemType.LegendaryGem)
                    .Select(i => new AdventurerGem(i, greaterRiftLevel))
                    .Distinct(new AdventurerGemComparer())
                    .ToList();

            UpdateGemSettings(Gems);
        }

        /// <summary>
        /// Populate gem settings records with backpack/stash gems
        /// </summary>
        private void UpdateGemSettings(List<AdventurerGem> freshGemsList)
        {
            if (freshGemsList == null || !freshGemsList.Any())
                return;

            var gemsBySno = freshGemsList.ToLookup(k => k.SNO, v => v);
            foreach (var gemSetting in GemSettings)
            {
                var gems = gemsBySno[gemSetting.Sno];
                if (gems.Any())
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
            var priority = PluginSettings.Current.GemUpgradePriority;
            var equipPriority = PluginSettings.Current.GreaterRiftPrioritizeEquipedGems;
            var chanceReq = PluginSettings.Current.GreaterRiftGemUpgradeChance;

            UpdateGems(level);

            Core.Logger.Log($"[升级宝石] ---- 宝石升级汇总 ----");
            Core.Logger.Log($"[升级宝石] 目前秘境等级: {level}");
            Core.Logger.Log($"[升级宝石] 宝石数量: {Gems.Count}");
            Core.Logger.Log($"[升级宝石] 宝石的最高等级: {Gems.Max(g => g.Rank)}");
            Core.Logger.Log($"[升级宝石] 宝石的最低等级: {Gems.Min(g => g.Rank)}");
            Core.Logger.Log($"[升级宝石] 升级几率设置: {minChance}%");
            Core.Logger.Log($"[升级宝石] 排序优先: {priority}");
            Core.Logger.Log($"[升级宝石] 优先已装备: {equipPriority}");

            var gems = Gems.ToList();

            Core.Logger.Log($"[升级宝石] ---- 排除: 用户禁用类型 ----");

            foreach (var gem in gems.ToList())
            {
                if (!gem.Settings.IsEnabled)
                {
                    Core.Logger.Log($"[升级宝石] {gem.Name} ({gem.SNO}) Id={gem.Guid} 等级={gem.Rank}");
                    gems.Remove(gem);
                }
            }

            Core.Logger.Log($"[升级宝石] ---- 排除: 最高等级 ----");

            foreach (var gem in gems.ToList())
            {
                if (gem.Rank >= gem.Settings.MaxRank)
                {
                    Core.Logger.Log($"[升级宝石] {gem.Name} ({gem.SNO}) Id={gem.Guid} 等级={gem.Rank} 最高等级={gem.Settings.MaxRank}");
                    gems.Remove(gem);
                }
            }

            Core.Logger.Log($"[升级宝石] ---- 排除: 用户等级限制 ----");

            foreach (var gem in gems.ToList())
            {
                if (gem.Settings.IsLimited && gem.Rank >= gem.Settings.Limit)
                {
                    Core.Logger.Log($"[升级宝石] {gem.Name} ({gem.SNO}) Id={gem.Guid} 等级={gem.Rank} 限制={(!gem.Settings.IsLimited ? "None" : gem.Settings.Limit.ToString())}");
                    gems.Remove(gem);
                }
            }

            Core.Logger.Log($"[升级宝石] ---- 排除: 小于几率 ({minChance}%) ----");

            foreach (var gem in gems.ToList())
            {
                if (gem.UpgradeChance < chanceReq)
                {
                    Core.Logger.Log($"[升级宝石] {gem.Name} ({gem.SNO}) Id={gem.Guid} 等级={gem.Rank} 几率={gem.UpgradeChance}");
                    gems.Remove(gem);
                }
            }

            switch (priority)
            {
                case GemPriority.None:
                case GemPriority.Rank:
                    Core.Logger.Log($"[升级宝石] ---- '等级' 有序的候选者 ({gems.Count}), by {(equipPriority ? "Equipped, " : "")}等级 ----");
                    gems = gems.OrderBy(g => equipPriority && g.IsEquiped ? 0 : 1).ThenByDescending(g => g.Rank).ThenBy(g => g.Settings.Order).ToList();
                    break;
                case GemPriority.Order:
                    Core.Logger.Log($"[升级宝石] ---- '顺序' 有序的候选者 ({gems.Count}), by {(equipPriority ? "Equipped, " : "")}顺序 ----");
                    gems = gems.OrderBy(g => equipPriority && g.IsEquiped ? 0 : 1).ThenBy(g => g.Settings.Order).ToList();
                    break;
                case GemPriority.Chance:
                    Core.Logger.Log($"[升级宝石] ---- '几率' 有序的候选者 ({gems.Count}), by {(equipPriority ? "Equipped, " : "")}几率, 然后等级 ----");
                    gems = gems.OrderBy(g => equipPriority && g.IsEquiped ? 0 : 1).ThenByDescending(g => g.UpgradeChance).ThenByDescending(g => g.Rank).ToList();
                    break;
            }

            //if (focus)
            //{
            //    Core.Logger.Log($"[升级宝石] ---- 有序的候选者 ({gems.Count}), by {(equipPriority ? "Equipped, " : "")}顺序, 等级 - 集中模式 ----");
            //    gems = gems.OrderBy(g => equipPriority && g.IsEquiped ? 0 : 1).ThenBy(g => g.Settings.Order).ToList();
            //}
            //else
            //{
            //    Core.Logger.Log($"[升级宝石] ---- 有序的候选者 ({gems.Count}), by {(equipPriority ? "Equipped, " : "")}几率, 顺序, 等级 ----");
            //    gems = gems.OrderBy(g => equipPriority && g.IsEquiped ? 0 : 1).ThenByDescending(g => g.UpgradeChance).ThenBy(g => g.Settings.Order).ThenBy(g => g.Rank).ToList();
            //}

            for (int i = 0; i < gems.Count; i++)
            {
                var gem = gems.ElementAtOrDefault(i);
                Core.Logger.Log($"[升级宝石] #{(i + 1)}: {gem.Name} ({gem.SNO}) Id={gem.Guid} 等级={gem.Rank} 几率={gem.UpgradeChance} @{level} 顺序={gem.Settings.Order} 限制={(gem.Settings.IsLimited ? "None" : gem.Settings.Limit.ToString())} 装备={gem.IsEquiped}");
            }

            if (gems.Count == 0)
            {
                Core.Logger.Log("[升级宝石] 找不到任何达到最小成功几率内的宝石,升级具有最高成功率的宝石");
                gems = Gems.Where(g => !g.IsMaxRank).OrderByDescending(g => g.UpgradeChance).ToList();
            }

            ACDItem acdGem = null;

            var gemToUpgrade = gems.FirstOrDefault();
            if (gemToUpgrade != null)
            {
                Core.Logger.Log($"[升级宝石] ---- 选择 ----");
                Core.Logger.Log($"[升级宝石] 尝试升级 {gemToUpgrade.DisplayName} ({gemToUpgrade.SNO}) 等级={gemToUpgrade.Rank} 几率={gemToUpgrade.UpgradeChance}%");
                acdGem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(i => gemToUpgrade.Guid == i.AnnId);
            }

            if (acdGem == null)
            {
                acdGem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(i => i.ItemType == ItemType.LegendaryGem);
                Core.Logger.Log($"[升级宝石] AcdItem 没有发现 {gemToUpgrade?.DisplayName} - 使用 {acdGem?.Name} 所以任务可以完成");
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