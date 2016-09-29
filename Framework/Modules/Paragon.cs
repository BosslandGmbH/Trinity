using System;
using Trinity.Framework.Objects;
using Trinity.Settings.Paragon;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;
using UIElement = Zeta.Game.Internals.UIElement;

namespace Trinity.Framework.Modules
{
    /// <summary>
    /// Spend paragon points
    /// </summary>
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

            if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || !ZetaDia.Me.IsValid)
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
                    Logger.Log("You have {0} points in the category: {1} available", pointsAvailable, category);

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

        private static void ApplyPoints(ParagonSetting.ParagonCollection source, int pointsAvailable)
        {
            var pointsSpent = 0;

            foreach (var item in source)
            {
                if (pointsSpent >= pointsAvailable)
                {
                    Logger.LogVerbose("Spent all available points for the category '{0}'", item.Category);
                    return;
                }

                var currentlySpent = ZetaDia.Me.GetParagonBonus(item.DynamicType);

                var amount = Math.Min(item.MaxLimit - currentlySpent, pointsAvailable - pointsSpent);

                if (currentlySpent >= item.MaxLimit)
                {
                    Logger.LogVerbose("Skipping {1} ({2}) max limit has been reached (Spent={3} IsLimited={4} Limit={5} LimitMax={6}",
                        amount, item.DisplayName, item.DynamicType, currentlySpent, item.IsLimited, item.Limit, item.MaxLimit);

                    continue;
                }

                if (item.IsLimited)
                {
                    if (currentlySpent >= item.Limit)
                    {
                        Logger.LogVerbose("Skipping {1} ({2}) limit has been reached (Spent={3} IsLimited={4} Limit={5} LimitMax={6}",
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
                        Logger.LogVerbose("Adjusting MovementSpeed based on {0}% contributed by items.", pctBonus);
                    }
                }

                if (amount <= 0)
                {
                    Logger.LogVerbose("Skipping {1} ({2}) spend amount = {0} (Spent={3} IsLimited={4} Limit={5} LimitMax={6}",
                        amount, item.DisplayName, item.DynamicType, currentlySpent, item.IsLimited, item.Limit, item.MaxLimit);

                    continue;
                }

                Logger.LogVerbose("Assigning {0} points to {1} ({2}) (Spent={3} IsLimited={4} Limit={5} LimitMax={6}", 
                    amount, item.DisplayName, item.DynamicType, currentlySpent, item.IsLimited, item.Limit, item.MaxLimit);

                ZetaDia.Me.SpendParagonPoints(item.DynamicType, amount);
                pointsSpent += amount;
            }
        }

    }
}

