using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Linq;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Modules
{
    public class BuildLogger : Module
    {
        private bool _hasLoggedCurrentBuild;

        protected override int UpdateIntervalMs => 1000;

        protected override void OnPulse()
        {
            if (!_hasLoggedCurrentBuild && BotMain.IsRunning && Core.Inventory.PlayerEquippedIds.Any())
            {
                DebugUtil.LogBuildAndItems();
                _hasLoggedCurrentBuild = true;
            }
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

                Core.Logger.Log("------ 被动 ------");

                Action<Passive> logPassive = p => Core.Logger.Log("被动: {0}", p.Name);

                PassiveUtils.Active.ForEach(logPassive);
            }
            catch (Exception ex)
            {
                Core.Logger.Log("调试 > 登录构建和项目 异常: {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        internal static void LogNewItems()
        {
            //var knownIds = Legendary.ItemIds;

            //using (new AquireFrameHelper())
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
            //        Core.Logger.Log(string.Format("Item: {0}: {1} ({2})", i.ItemType, i.Name, i.ActorSnoId));
            //    });
            //}
        }

    }
}
