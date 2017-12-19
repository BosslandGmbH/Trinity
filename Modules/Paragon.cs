using System;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Trinity.Settings.Paragon;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

using UIElement = Zeta.Game.Internals.UIElement;

namespace Trinity.Modules
{
    public class Paragon : Module
    {
        public DateTime LastAssignedParagon = DateTime.MinValue;

        protected override void OnPulse()
        {
            SpendPoints();    
        }

        public void SpendPoints()
        {
            if (ZetaDia.Me == null)
                return;

            if (!Core.Settings.Paragon.IsEnabled)
                return;

            if (DateTime.UtcNow.Subtract(LastAssignedParagon).TotalSeconds < 30)
                return;

            if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || !ZetaDia.Me.IsValid)
                return;

            if (IsParagonWindowOpen()) // Dont spend when the user might be clicking buttons, things could get confused.
                return;

            LastAssignedParagon = DateTime.UtcNow;

            for (int i = 0; i < 4; i++)
            {
                ParagonCategory category = (ParagonCategory) i;
                int pointsAvailable = ZetaDia.Me.GetParagonPointsAvailable((ParagonCategory) i);
                if (pointsAvailable != 0)
                {
                    Core.Logger.Log("你有 {0} 点巅峰: {1} 可用", pointsAvailable, category);

                    switch (category)
                    {
                        case ParagonCategory.PrimaryAttributes:
                            ApplyPoints(Core.Settings.Paragon.CoreParagonPriority, pointsAvailable);
                            break;

                        case ParagonCategory.Defense:
                            ApplyPoints(Core.Settings.Paragon.DefenseParagonPriority, pointsAvailable);
                            break;

                        case ParagonCategory.Offense:
                            ApplyPoints(Core.Settings.Paragon.OffenseParagonPriority, pointsAvailable);
                            break;

                        case ParagonCategory.Utility:
                            ApplyPoints(Core.Settings.Paragon.UtilityParagonPriority, pointsAvailable);
                            break;
                    }
                }
            }
        }

        public bool IsParagonWindowOpen()
        {
            //[1F2D2530] Mouseover: 0x20098D9B0B396156, Name: Root.NormalLayer.Paragon_main.LayoutRoot.ParagonPointSelect.tab_1
            var paragonCoreTab = UIElement.FromHash(0x20098D9B0B396156);
            return paragonCoreTab != null && paragonCoreTab.IsVisible;
        }

        private static void ApplyPoints(ParagonSettings.ParagonCollection source, int pointsAvailable)
        {
            var pointsSpent = 0;

            foreach (var item in source)
            {
                if (pointsSpent >= pointsAvailable)
                {
                    Core.Logger.Verbose("花费了所有类别的所有可用点 '{0}'", item.Category);
                    return;
                }

                var currentlySpent = ZetaDia.Me.GetParagonBonus(item.DynamicType);

                var amount = Math.Min(item.MaxLimit - currentlySpent, pointsAvailable - pointsSpent);

                if (currentlySpent >= item.MaxLimit)
                {
                    Core.Logger.Verbose("跳过 {1} ({2}) 的最大限制已经达到 (花费={3} 有限={4} 限制={5} 限制最大值={6}",
                        amount, item.DisplayName, item.DynamicType, currentlySpent, item.IsLimited, item.Limit, item.MaxLimit);

                    continue;
                }

                if (item.IsLimited)
                {
                    if (currentlySpent >= item.Limit)
                    {
                        Core.Logger.Verbose("跳过 {1} ({2}) 限制已达到 (花费={3} 有限={4} 限制={5} 限制最大值={6}",
                            amount, item.DisplayName, item.DynamicType, currentlySpent, item.IsLimited, item.Limit, item.MaxLimit);

                        continue;
                    }

                    amount = Math.Min(item.Limit, amount);
                }

                if (item.DynamicType == ParagonBonusType.MovementBonusRunSpeed)
                {
                    var bonusSpeedFromItems = ZetaDia.Me.CommonData.MovementScalarTotal;
                    if (bonusSpeedFromItems > 1)
                    {
                        var pctBonus = (int)Math.Round((bonusSpeedFromItems - 1)*100,0, MidpointRounding.AwayFromZero);
                        var pointsNegatedByItems = pctBonus * 2;
                        var effectiveMaxPoints = item.MaxLimit - pointsNegatedByItems; 
                        amount = Math.Min(effectiveMaxPoints, amount);
                        Core.Logger.Verbose("基于项目贡献的 {0}% 调整移动速度.", pctBonus);
                    }
                }

                if (amount <= 0)
                {
                    Core.Logger.Verbose("跳过 {1} ({2}) 花费数量 = {0} (花费={3} 有限={4} 限制={5} 限制最大值={6}",
                        amount, item.DisplayName, item.DynamicType, currentlySpent, item.IsLimited, item.Limit, item.MaxLimit);

                    continue;
                }

                Core.Logger.Verbose("将 {0} 点分配给 {1} ({2}) (花费={3} 有限={4} 限制={5} 限制最大值={6}", 
                    amount, item.DisplayName, item.DynamicType, currentlySpent, item.IsLimited, item.Limit, item.MaxLimit);

                ZetaDia.Me.SpendParagonPoints(item.DynamicType, amount);
                pointsSpent += amount;
            }
        }

    }
}

