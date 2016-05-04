using System;
using Trinity.DbProvider;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public class ItemValuation
    {
        private static double[] ItemMaxStats = new double[Constants.TOTALSTATS];
        private static double[] ItemMaxPoints = new double[Constants.TOTALSTATS];
        private static bool IsInvalidItem = true;
        private static double TotalItemPoints = 0;
        private static TrinityItemBaseType baseItemType = TrinityItemBaseType.Unknown;

        /// <summary>
        /// This is a bonus applied at the end of valuation 
        /// </summary>
        private static double BestFinalBonus = 1d;

        // Constants for convenient stat names
        private static double[] HadStat = new double[Constants.TOTALSTATS] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static double[] HadPoints = new double[Constants.TOTALSTATS] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static double SafeLifePercentage = 0;
        private static bool SocketsCanReplacePrimaries = false;
        private static double HighestScoringPrimary = 0;
        private static int WhichPrimaryIsHighest = 0;
        private static double AmountHighestScoringPrimary = 0;
        // End of main 0-Constants.TOTALSTATS stat loop
        private static int TotalRequirements;
        private static double GlobalMultiplier = 1;

        /// <summary>
        /// The bizarre mystery function to score your lovely items!
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        internal static double ValueThisItem(CachedACDItem item, TrinityItemType itemType)
        {
            // Reset static variables
            TotalItemPoints = 0;
            IsInvalidItem = true;
            ItemMaxStats = new double[Constants.TOTALSTATS];
            ItemMaxPoints = new double[Constants.TOTALSTATS];
            baseItemType = TrinityItemManager.DetermineBaseType(itemType);
            BestFinalBonus = 1d;
            HadStat = new double[Constants.TOTALSTATS] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            HadPoints = new double[Constants.TOTALSTATS] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            SafeLifePercentage = 0;
            SocketsCanReplacePrimaries = false;
            HighestScoringPrimary = 0;
            WhichPrimaryIsHighest = 0;
            AmountHighestScoringPrimary = 0;
            TotalRequirements = 0;
            GlobalMultiplier = 1;
            JunkItemStatString = "";
            ValueItemStatString = "";

            // Checks for Invalid Item Types
            CheckForInvalidItemType(itemType);

            // Double safety check for unidentified items
            if (item.IsUnidentified)
                IsInvalidItem = true;

            // Make sure we got a valid item here, otherwise score it a big fat 0
            if (IsInvalidItem)
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, $"-- Invalid Item Type={itemType}, IsUnidentified={item.IsUnidentified}");
                return 0;
            }

            Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "NEXT ITEM= " + item.RealName + " - " + item.InternalName + " [" + baseItemType.ToString() + " - " + itemType.ToString() + "]");

            ResetValuationStatStrings();

            // We loop through all of the stats, in a particular order. The order *IS* important, because it pulls up primary stats first, BEFORE other stats
            for (int i = 0; i <= (Constants.TOTALSTATS - 1); i++)
            {
                double TempStatistic = 0;

                // Now we lookup each stat on this item we are scoring, and store it in the variable "iTempStatistic" - which is used for calculations further down
                switch (i)
                {
                    case Constants.DEXTERITY: TempStatistic = item.Dexterity; break;
                    case Constants.INTELLIGENCE: TempStatistic = item.Intelligence; break;
                    case Constants.STRENGTH: TempStatistic = item.Strength; break;
                    case Constants.VITALITY: TempStatistic = item.Vitality; break;
                    case Constants.LIFEPERCENT: TempStatistic = item.LifePercent; break;
                    case Constants.LIFEONHIT: TempStatistic = item.LifeOnHit; break;
                    case Constants.LIFESTEAL: TempStatistic = item.LifeSteal; break;
                    case Constants.LIFEREGEN: TempStatistic = item.HealthPerSecond; break;
                    case Constants.MAGICFIND: TempStatistic = item.MagicFind; break;
                    case Constants.GOLDFIND: TempStatistic = item.GoldFind; break;
                    case Constants.MOVEMENTSPEED: TempStatistic = item.MovementSpeed; break;
                    case Constants.PICKUPRADIUS: TempStatistic = item.PickUpRadius; break;
                    case Constants.SOCKETS: TempStatistic = item.Sockets; break;
                    case Constants.CRITCHANCE: TempStatistic = item.CritPercent; break;
                    case Constants.CRITDAMAGE: TempStatistic = item.CritDamagePercent; break;
                    case Constants.ATTACKSPEED: TempStatistic = item.AttackSpeedPercent; break;
                    case Constants.MINDAMAGE: TempStatistic = item.MinDamage; break;
                    case Constants.MAXDAMAGE: TempStatistic = item.MaxDamage; break;
                    case Constants.BLOCKCHANCE: TempStatistic = item.BlockChance; break;
                    case Constants.THORNS: TempStatistic = item.Thorns; break;
                    case Constants.ALLRESIST: TempStatistic = item.ResistAll; break;
                    case Constants.RANDOMRESIST:
                        if (item.ResistArcane > TempStatistic) TempStatistic = item.ResistArcane;
                        if (item.ResistCold > TempStatistic) TempStatistic = 0;
                        //thisitem.ResistCold;
                        if (item.ResistFire > TempStatistic) TempStatistic = item.ResistFire;
                        if (item.ResistHoly > TempStatistic) TempStatistic = item.ResistHoly;
                        if (item.ResistLightning > TempStatistic) TempStatistic = 0;
                        //thisitem.ResistLightning;
                        if (item.ResistPhysical > TempStatistic) TempStatistic = item.ResistPhysical;
                        if (item.ResistPoison > TempStatistic) TempStatistic = 0;
                        //thisitem.ResistPoison;
                        break;
                    case Constants.TOTALDPS: TempStatistic = item.WeaponDamagePerSecond; break;
                    case Constants.ARMOR: TempStatistic = item.ArmorBonus; break;
                    case Constants.MAXDISCIPLINE: TempStatistic = item.MaxDiscipline; break;
                    case Constants.MAXMANA: TempStatistic = item.MaxMana; break;
                    case Constants.ARCANECRIT: TempStatistic = item.ArcaneOnCrit; break;
                    case Constants.MANAREGEN: TempStatistic = item.ManaRegen; break;
                    case Constants.GLOBEBONUS: TempStatistic = item.GlobeBonus; break;
                }
                HadStat[i] = TempStatistic;
                HadPoints[i] = 0;

                // Now we check that the current statistic in the "for" loop, actually exists on this item, and is a stat we are measuring (has >0 in the "max stats" array)
                if (ItemMaxStats[i] > 0 && TempStatistic > 0)
                {

                    // Final bonus granted is an end-of-score multiplier. 1 = 100%, so all items start off with 100%, of course!
                    double FinalBonusGranted = 1;

                    // Item Stat Ratio is what PERCENTAGE of the *MAXIMUM POSSIBLE STAT*, this stat is at.
                    // Note that stats OVER the max will get a natural score boost, since this value will be over 1!
                    double itemStatRatio = TempStatistic / ItemMaxStats[i];

                    // Now multiply the "max points" value, by that percentage, as the start/basis of the scoring for this statistic
                    double iTempPoints = ItemMaxPoints[i] * itemStatRatio;

                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "--- " + Constants.StatNames[i] + ": " + TempStatistic.ToString("0") + " out of " + ItemMaxStats[i].ToString() + " (" + ItemMaxPoints[i].ToString() + " * " + itemStatRatio.ToString("0.000") + " = " + iTempPoints.ToString("0") + ")");

                    // Check if this statistic is over the "bonus threshold" array value for this stat - if it is, then it gets a score bonus when over a certain % of max-stat
                    if (itemStatRatio > Constants.BonusThreshold[i] && Constants.BonusThreshold[i] > 0f)
                    {
                        FinalBonusGranted += ((itemStatRatio - Constants.BonusThreshold[i]) * 0.9);
                    }

                    // We're going to store the life % stat here for quick-calculations against other stats. Don't edit this bit!
                    if (i == Constants.LIFEPERCENT)
                    {
                        if (ItemMaxStats[Constants.LIFEPERCENT] > 0)
                        {
                            SafeLifePercentage = (TempStatistic / ItemMaxStats[Constants.LIFEPERCENT]);
                        }
                        else
                        {
                            SafeLifePercentage = 0;
                        }
                    }

                    // This *REMOVES* score from follower items for stats that followers don't care about
                    if (baseItemType == TrinityItemBaseType.FollowerItem && (i == Constants.CRITDAMAGE || i == Constants.LIFEONHIT || i == Constants.ALLRESIST))
                        FinalBonusGranted -= 0.9;

                    // Bonus 15% for being *at* the stat cap (ie - completely maxed out, or very very close to), but not for the socket stat (since sockets are usually 0 or 1!)
                    if (i != Constants.SOCKETS)
                    {
                        if ((TempStatistic / ItemMaxStats[i]) >= 0.99)
                            FinalBonusGranted += 0.15;

                        // Else bonus 10% for being in final 95%
                        else if ((TempStatistic / ItemMaxStats[i]) >= 0.95)
                            FinalBonusGranted += 0.10;
                    }

                    // Socket handling

                    // Sockets give special bonuses for certain items, depending how close to the max-socket-count it is for that item

                    // It also enables bonus scoring for stats which usually rely on a high primary stat - since a socket can make up for a lack of a high primary (you can socket a +primary stat!)
                    if (i == Constants.SOCKETS)
                    {

                        // Off-handers get less value from sockets
                        if (baseItemType == TrinityItemBaseType.Offhand)
                        {
                            FinalBonusGranted -= 0.35;
                        }

                        // Chest
                        if (itemType == TrinityItemType.Chest || itemType == TrinityItemType.Cloak)
                        {
                            if (TempStatistic >= 2)
                            {
                                SocketsCanReplacePrimaries = true;
                                if (TempStatistic >= 3)
                                    FinalBonusGranted += 0.25;
                            }
                        }

                        // Pants
                        if (itemType == TrinityItemType.Legs)
                        {
                            if (TempStatistic >= 2)
                            {
                                SocketsCanReplacePrimaries = true;
                                FinalBonusGranted += 0.25;
                            }
                        }

                        // Helmets can have a bonus for a socket since it gives amazing MF/GF
                        if (TempStatistic >= 1 && (itemType == TrinityItemType.Helm || itemType == TrinityItemType.WizardHat || itemType == TrinityItemType.VoodooMask ||
                            itemType == TrinityItemType.SpiritStone))
                        {
                            SocketsCanReplacePrimaries = true;
                        }

                        // And rings and amulets too
                        if (TempStatistic >= 1 && (itemType == TrinityItemType.Ring || itemType == TrinityItemType.Amulet))
                        {
                            SocketsCanReplacePrimaries = true;
                        }
                    }

                    // Right, here's quite a long bit of code, but this is basically all about granting all sorts of bonuses based on primary stat values of all different ranges

                    // For all item types *EXCEPT* weapons
                    if (baseItemType != TrinityItemBaseType.WeaponRange && baseItemType != TrinityItemBaseType.WeaponOneHand && baseItemType != TrinityItemBaseType.WeaponTwoHand)
                    {
                        double SpecialBonus = 0;
                        if (i > Constants.LIFEPERCENT)
                        {

                            // Knock off points for being particularly low
                            if ((TempStatistic / ItemMaxStats[i]) < 0.2 && (Constants.BonusThreshold[i] <= 0f || Constants.BonusThreshold[i] >= 0.2))
                                FinalBonusGranted -= 0.35;
                            else if ((TempStatistic / ItemMaxStats[i]) < 0.4 && (Constants.BonusThreshold[i] <= 0f || Constants.BonusThreshold[i] >= 0.4))
                                FinalBonusGranted -= 0.15;

                            // Remove 80% if below minimum threshold
                            if ((TempStatistic / ItemMaxStats[i]) < Constants.MinimumThreshold[i] && Constants.MinimumThreshold[i] > 0f)
                                FinalBonusGranted -= 0.8;

                            // Primary stat/vitality minimums or zero-check reductions on other stats
                            if (Constants.StatMinimumPrimary[i] > 0)
                            {

                                // Remove 40% from all stats if there is no prime stat present or vitality/life present and this is below 90% of max
                                if (((TempStatistic / ItemMaxStats[i]) < .90) && ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) < Constants.StatMinimumPrimary[i]) &&
                                    ((HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) < (Constants.StatMinimumPrimary[i] + 0.1)) && ((HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) < Constants.StatMinimumPrimary[i]) &&
                                    ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) < Constants.StatMinimumPrimary[i]) && (SafeLifePercentage < (Constants.StatMinimumPrimary[i] * 2.5)) && !SocketsCanReplacePrimaries)
                                {
                                    if (itemType != TrinityItemType.Ring && itemType != TrinityItemType.Amulet)
                                        FinalBonusGranted -= 0.4;
                                    else
                                        FinalBonusGranted -= 0.3;

                                    // And another 25% off for armor and all resist which are more useful with primaries, as long as not jewelry
                                    if ((i == Constants.ARMOR || i == Constants.ALLRESIST || i == Constants.RANDOMRESIST) && itemType != TrinityItemType.Ring && itemType != TrinityItemType.Amulet && !SocketsCanReplacePrimaries)
                                        FinalBonusGranted -= 0.15;
                                }
                            }
                            else
                            {

                                // Almost no primary stats or health at all
                                if (HadStat[Constants.DEXTERITY] <= 60 && HadStat[Constants.STRENGTH] <= 60 && HadStat[Constants.INTELLIGENCE] <= 60 && HadStat[Constants.VITALITY] <= 60 && SafeLifePercentage < 0.9 && !SocketsCanReplacePrimaries)
                                {

                                    // So 35% off for all items except jewelry which is 20% off
                                    if (itemType != TrinityItemType.Ring && itemType != TrinityItemType.Amulet)
                                    {
                                        FinalBonusGranted -= 0.35;

                                        // And another 25% off for armor and all resist which are more useful with primaries
                                        if (i == Constants.ARMOR || i == Constants.ALLRESIST)
                                            FinalBonusGranted -= 0.15;
                                    }
                                    else
                                    {
                                        FinalBonusGranted -= 0.20;
                                    }
                                }
                            }
                            if (baseItemType == TrinityItemBaseType.Armor || baseItemType == TrinityItemBaseType.Jewelry)
                            {

                                // Grant a 50% bonus to stats if a primary is above 200 AND (vitality above 200 or life% within 90% max)
                                if ((HadStat[Constants.DEXTERITY] > 200 || HadStat[Constants.STRENGTH] > 200 || HadStat[Constants.INTELLIGENCE] > 200) && (HadStat[Constants.VITALITY] > 200 || SafeLifePercentage > .97))
                                {
                                    if (0.5 > SpecialBonus) SpecialBonus = 0.5;
                                }

                                // Else grant a 40% bonus to stats if a primary is above 200
                                if (HadStat[Constants.DEXTERITY] > 200 || HadStat[Constants.STRENGTH] > 200 || HadStat[Constants.INTELLIGENCE] > 200)
                                {
                                    if (0.4 > SpecialBonus) SpecialBonus = 0.4;
                                }

                                // Grant a 30% bonus if vitality > 200 or life percent within 90% of max
                                if (HadStat[Constants.VITALITY] > 200 || SafeLifePercentage > .97)
                                {
                                    if (0.3 > SpecialBonus) SpecialBonus = 0.3;
                                }
                            }

                            // Checks for various primary & health levels
                            if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > .85 || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > .85 || (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > .85)
                            {
                                if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .6 || SafeLifePercentage > .90)
                                {
                                    if (0.5 > SpecialBonus) SpecialBonus = 0.5;
                                }
                                else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .35 || SafeLifePercentage > .85)
                                {
                                    if (0.4 > SpecialBonus) SpecialBonus = 0.4;
                                }
                                else
                                {
                                    if (0.2 > SpecialBonus) SpecialBonus = 0.2;
                                }
                            }
                            if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > .75 || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > .75 || (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > .75)
                            {
                                if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .6 || SafeLifePercentage > .90)
                                {
                                    if (0.35 > SpecialBonus) SpecialBonus = 0.35;
                                }
                                else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .35 || SafeLifePercentage > .85)
                                {
                                    if (0.30 > SpecialBonus) SpecialBonus = 0.30;
                                }
                                else
                                {
                                    if (0.15 > SpecialBonus) SpecialBonus = 0.15;
                                }
                            }
                            if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > .65 || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > .65 || (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > .65)
                            {
                                if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .6 || SafeLifePercentage > .90)
                                {
                                    if (0.26 > SpecialBonus) SpecialBonus = 0.26;
                                }
                                else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .35 || SafeLifePercentage > .85)
                                {
                                    if (0.22 > SpecialBonus) SpecialBonus = 0.22;
                                }
                                else
                                {
                                    if (0.11 > SpecialBonus) SpecialBonus = 0.11;
                                }
                            }
                            if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > .55 || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > .55 || (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > .55)
                            {
                                if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .6 || SafeLifePercentage > .90)
                                {
                                    if (0.18 > SpecialBonus) SpecialBonus = 0.18;
                                }
                                else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .35 || SafeLifePercentage > .85)
                                {
                                    if (0.14 > SpecialBonus) SpecialBonus = 0.14;
                                }
                                else
                                {
                                    if (0.08 > SpecialBonus) SpecialBonus = 0.08;
                                }
                            }
                            if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > .5 || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > .5 || (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > .5)
                            {
                                if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .6 || SafeLifePercentage > .90)
                                {
                                    if (0.12 > SpecialBonus) SpecialBonus = 0.12;
                                }
                                else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .35 || SafeLifePercentage > .85)
                                {
                                    if (0.05 > SpecialBonus) SpecialBonus = 0.05;
                                }
                                else
                                {
                                    if (0.03 > SpecialBonus) SpecialBonus = 0.03;
                                }
                            }
                            if (itemType == TrinityItemType.Ring || itemType == TrinityItemType.Amulet)
                            {
                                if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > .4 || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > .4 || (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > .4)
                                {
                                    if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .6 || SafeLifePercentage > .90)
                                    {
                                        if (0.10 > SpecialBonus) SpecialBonus = 0.10;
                                    }
                                    else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .35 || SafeLifePercentage > .85)
                                    {
                                        if (0.08 > SpecialBonus) SpecialBonus = 0.08;
                                    }
                                    else
                                    {
                                        if (0.05 > SpecialBonus) SpecialBonus = 0.05;
                                    }
                                }
                            }
                            if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .8 || SafeLifePercentage > .98)
                            {
                                if (0.20 > SpecialBonus) SpecialBonus = 0.20;
                            }
                            if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .7 || SafeLifePercentage > .95)
                            {
                                if (0.16 > SpecialBonus) SpecialBonus = 0.16;
                            }
                            if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .6 || SafeLifePercentage > .92)
                            {
                                if (0.12 > SpecialBonus) SpecialBonus = 0.12;
                            }
                            if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .55 || SafeLifePercentage > .89)
                            {
                                if (0.07 > SpecialBonus) SpecialBonus = 0.07;
                            }
                            else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .5 || SafeLifePercentage > .87)
                            {
                                if (0.05 > SpecialBonus) SpecialBonus = 0.05;
                            }
                            else if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > .45 || SafeLifePercentage > .86)
                            {
                                if (0.02 > SpecialBonus) SpecialBonus = 0.02;
                            }
                        }

                        // This stat is one after life percent stat

                        // Shields get less of a special bonus from high prime stats
                        if (itemType == TrinityItemType.Shield)
                            SpecialBonus *= 0.7;

                        if (SpecialBonus > 0)
                            Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "------- special bonus =" + SpecialBonus.ToString(), true);

                        FinalBonusGranted += SpecialBonus;
                    }

                    if (i == Constants.LIFESTEAL && itemType == TrinityItemType.MightyBelt)
                        FinalBonusGranted += 0.3;

                    if (i == Constants.TOTALDPS)
                    {
                        // Knock off points for being particularly low
                        if ((TempStatistic / ItemMaxStats[i]) < Constants.MinimumThreshold[i] && Constants.MinimumThreshold[i] > 0f)
                            FinalBonusGranted -= 0.5;
                    }
                    else
                    {
                        // Knock off points for being particularly low
                        if ((TempStatistic / ItemMaxStats[i]) < Constants.MinimumThreshold[i] && Constants.MinimumThreshold[i] > 0f)
                            FinalBonusGranted -= 0.35;
                    }
                    // Grant a 20% bonus to vitality or Life%, for being paired with any prime stat above minimum threshold +.1
                    if (((i == Constants.VITALITY && (TempStatistic / ItemMaxStats[Constants.VITALITY]) > Constants.MinimumThreshold[Constants.VITALITY]) ||
                          i == Constants.LIFEPERCENT && (TempStatistic / ItemMaxStats[Constants.LIFEPERCENT]) > Constants.MinimumThreshold[Constants.LIFEPERCENT]) &&
                        ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > (Constants.MinimumThreshold[Constants.DEXTERITY] + 0.1) || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > (Constants.MinimumThreshold[Constants.STRENGTH] + 0.1) ||
                         (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > (Constants.MinimumThreshold[Constants.INTELLIGENCE] + 0.1)))
                        FinalBonusGranted += 0.2;

                    // Blue item point reduction for non-weapons
                    if (item.Quality < ItemQuality.Rare4 && (baseItemType == TrinityItemBaseType.Armor || baseItemType == TrinityItemBaseType.Offhand ||
                        baseItemType == TrinityItemBaseType.Jewelry || baseItemType == TrinityItemBaseType.FollowerItem) && ((TempStatistic / ItemMaxStats[i]) < 0.88))
                        FinalBonusGranted -= 0.9;

                    // Special all-resist bonuses
                    if (i == Constants.ALLRESIST)
                    {

                        // Shields with < 60% max all resist, lost some all resist score
                        if (itemType == TrinityItemType.Shield && (TempStatistic / ItemMaxStats[i]) <= 0.6)
                            FinalBonusGranted -= 0.30;
                        double iSpecialBonus = 0;

                        // All resist gets a special bonus if paired with good strength and some vitality
                        if ((HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > 0.7 && (HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > 0.3)
                            if (0.45 > iSpecialBonus) iSpecialBonus = 0.45;

                        // All resist gets a smaller special bonus if paired with good dexterity and some vitality
                        if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > 0.7 && (HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > 0.3)
                            if (0.35 > iSpecialBonus) iSpecialBonus = 0.35;

                        // All resist gets a slight special bonus if paired with good intelligence and some vitality
                        if ((HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > 0.7 && (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > 0.3)
                            if (0.25 > iSpecialBonus) iSpecialBonus = 0.25;

                        // Smaller bonuses for smaller stats

                        // All resist gets a special bonus if paired with good strength and some vitality
                        if ((HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > 0.55 && (HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > 0.3)
                            if (0.45 > iSpecialBonus) iSpecialBonus = 0.20;

                        // All resist gets a smaller special bonus if paired with good dexterity and some vitality
                        if ((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > 0.55 && (HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > 0.3)
                            if (0.35 > iSpecialBonus) iSpecialBonus = 0.15;

                        // All resist gets a slight special bonus if paired with good intelligence and some vitality
                        if ((HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > 0.55 && (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > 0.3)
                            if (0.25 > iSpecialBonus) iSpecialBonus = 0.10;

                        // This stat is one after life percent stat
                        FinalBonusGranted += iSpecialBonus;

                        // Global bonus to everything
                        //if ((ItemMaxStats[i] - TempStatistic) < 10.2f)
                        //    GlobalMultiplier += 0.05;
                    }

                    // All resist special bonuses
                    if (itemType != TrinityItemType.Ring && itemType != TrinityItemType.Amulet)
                    {

                        // Shields get 10% less on everything
                        if (itemType == TrinityItemType.Shield)
                            FinalBonusGranted -= 0.10;

                        // Prime stat gets a 20% bonus if 50 from max possible
                        if ((i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE || i == Constants.VITALITY) && (ItemMaxStats[i] - TempStatistic) < 50.5f)
                            FinalBonusGranted += 0.25;

                        // Reduce a prime stat by 75% if less than 100 *OR* less than 50% max
                        if ((i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE) && (TempStatistic < 100 || ((TempStatistic / ItemMaxStats[i]) < 0.5)))
                            FinalBonusGranted -= 0.75;

                        // Reduce a vitality/life% stat by 60% if less than 80 vitality/less than 60% max possible life%
                        if ((i == Constants.VITALITY && TempStatistic < 80) || (i == Constants.LIFEPERCENT && ((TempStatistic / ItemMaxStats[Constants.LIFEPERCENT]) < 0.6)))
                            FinalBonusGranted -= 0.6;

                        // Grant 10% to any 4 main stat above 200
                        if ((i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE || i == Constants.VITALITY) && TempStatistic > 200)
                            FinalBonusGranted += 0.1;

                        // Special stat handling stuff for non-jewelry types

                        // Within 2 block chance
                        if (i == Constants.BLOCKCHANCE && (ItemMaxStats[i] - TempStatistic) < 2.3f)
                            FinalBonusGranted += 1;

                        // Within final 5 gold find
                        if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) < 5.3f)
                        {
                            FinalBonusGranted += 0.04;

                            // Even bigger bonus if got prime stat & vit
                            if (((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > Constants.MinimumThreshold[Constants.DEXTERITY] || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > Constants.MinimumThreshold[Constants.STRENGTH] ||
                                (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > Constants.MinimumThreshold[Constants.INTELLIGENCE]) && (HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > Constants.MinimumThreshold[Constants.VITALITY])
                                FinalBonusGranted += 0.02;
                        }

                        // Within final 3 gold find
                        if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) < 3.3f)
                        {
                            FinalBonusGranted += 0.04;
                        }

                        // Within final 2 gold find
                        if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) < 2.3f)
                        {
                            FinalBonusGranted += 0.05;
                        }

                        // Within final 3 magic find
                        if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) < 3.3f)
                            FinalBonusGranted += 0.08;

                        // Within final 2 magic find
                        if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) < 2.3f)
                        {
                            FinalBonusGranted += 0.04;

                            // Even bigger bonus if got prime stat & vit
                            if (((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > Constants.MinimumThreshold[Constants.DEXTERITY] || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > Constants.MinimumThreshold[Constants.STRENGTH] ||
                                (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > Constants.MinimumThreshold[Constants.INTELLIGENCE]) && (HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > Constants.MinimumThreshold[Constants.VITALITY])
                                FinalBonusGranted += 0.03;
                        }

                        // Within final magic find
                        if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) < 1.3f)
                        {
                            FinalBonusGranted += 0.05;
                        }

                        // Within final 10 all resist
                        if (i == Constants.ALLRESIST && (ItemMaxStats[i] - TempStatistic) < 10.2f)
                        {
                            FinalBonusGranted += 0.05;

                            // Even bigger bonus if got prime stat & vit
                            if (((HadStat[Constants.DEXTERITY] / ItemMaxStats[Constants.DEXTERITY]) > Constants.MinimumThreshold[Constants.DEXTERITY] || (HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > Constants.MinimumThreshold[Constants.STRENGTH] ||
                                (HadStat[Constants.INTELLIGENCE] / ItemMaxStats[Constants.INTELLIGENCE]) > Constants.MinimumThreshold[Constants.INTELLIGENCE]) && (HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY]) > Constants.MinimumThreshold[Constants.VITALITY])
                                FinalBonusGranted += 0.20;
                        }

                        // Within final 50 armor
                        if (i == Constants.ARMOR && (ItemMaxStats[i] - TempStatistic) < 50.2f)
                        {
                            FinalBonusGranted += 0.10;
                            if ((HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > Constants.MinimumThreshold[Constants.STRENGTH])
                                FinalBonusGranted += 0.10;
                        }

                        // Within final 15 armor
                        if (i == Constants.ARMOR && (ItemMaxStats[i] - TempStatistic) < 15.2f)
                            FinalBonusGranted += 0.15;

                        // Within final 5 critical hit damage
                        if (i == Constants.CRITDAMAGE && (ItemMaxStats[i] - TempStatistic) < 5.2f)
                            FinalBonusGranted += 0.25;

                        // More than 2.5 crit chance out
                        if (i == Constants.CRITCHANCE && (ItemMaxStats[i] - TempStatistic) > 2.45f)
                            FinalBonusGranted -= 0.35;

                        // More than 20 crit damage out
                        if (i == Constants.CRITDAMAGE && (ItemMaxStats[i] - TempStatistic) > 19.95f)
                            FinalBonusGranted -= 0.35;

                        // More than 2 attack speed out
                        if (i == Constants.ATTACKSPEED && (ItemMaxStats[i] - TempStatistic) > 1.95f)
                            FinalBonusGranted -= 0.35;

                        // More than 2 move speed
                        if (i == Constants.MOVEMENTSPEED && (ItemMaxStats[i] - TempStatistic) > 1.95f)
                            FinalBonusGranted -= 0.35;

                        // More than 5 gold find out
                        if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) > 5.2f)
                            FinalBonusGranted -= 0.40;

                        // More than 8 gold find out
                        if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) > 8.2f)
                            FinalBonusGranted -= 0.1;

                        // More than 5 magic find out
                        if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) > 5.2f)
                            FinalBonusGranted -= 0.40;

                        // More than 7 magic find out
                        if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) > 7.2f)
                            FinalBonusGranted -= 0.1;

                        // More than 20 all resist out
                        if (i == Constants.ALLRESIST && (ItemMaxStats[i] - TempStatistic) > 20.2f)
                            FinalBonusGranted -= 0.50;

                        // More than 30 all resist out
                        if (i == Constants.ALLRESIST && (ItemMaxStats[i] - TempStatistic) > 30.2f)
                            FinalBonusGranted -= 0.20;
                    }

                    // And now for jewelry checks...
                    if (itemType == TrinityItemType.Ring || itemType == TrinityItemType.Amulet)
                    {

                        // Global bonus to everything if jewelry has an all resist above 50%
                        if (i == Constants.ALLRESIST && (TempStatistic / ItemMaxStats[i]) > 0.5)
                            GlobalMultiplier += 0.08;

                        // Within final 10 all resist
                        if (i == Constants.ALLRESIST && (ItemMaxStats[i] - TempStatistic) < 10.2f)
                            FinalBonusGranted += 0.10;

                        // Within final 5 critical hit damage
                        if (i == Constants.CRITDAMAGE && (ItemMaxStats[i] - TempStatistic) < 5.2f)
                            FinalBonusGranted += 0.25;

                        // Within 3 block chance
                        if (i == Constants.BLOCKCHANCE && (ItemMaxStats[i] - TempStatistic) < 3.3f)
                            FinalBonusGranted += 0.15;

                        // Reduce a prime stat by 60% if less than 60
                        if ((i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE) && (TempStatistic < 60 || ((TempStatistic / ItemMaxStats[i]) < 0.3)))
                            FinalBonusGranted -= 0.6;

                        // Reduce a vitality/life% stat by 50% if less than 50 vitality/less than 40% max possible life%
                        if ((i == Constants.VITALITY && TempStatistic < 50) || (i == Constants.LIFEPERCENT && ((TempStatistic / ItemMaxStats[Constants.LIFEPERCENT]) < 0.4)))
                            FinalBonusGranted -= 0.5;

                        // Grant 20% to any 4 main stat above 150
                        if ((i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE || i == Constants.VITALITY) && TempStatistic > 150)
                            FinalBonusGranted += 0.2;

                        // Special stat handling stuff for jewelry
                        if (itemType == TrinityItemType.Ring)
                        {

                            // Prime stat gets a 25% bonus if 30 from max possible
                            if ((i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE || i == Constants.VITALITY) && (ItemMaxStats[i] - TempStatistic) < 30.5f)
                                FinalBonusGranted += 0.25;

                            // Within final 5 magic find
                            if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) < 5.2f)
                                FinalBonusGranted += 0.4;

                            // Within final 5 gold find
                            if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) < 5.2f)
                                FinalBonusGranted += 0.35;

                            // Within final 45 life on hit
                            if (i == Constants.LIFEONHIT && (ItemMaxStats[i] - TempStatistic) < 45.2f)
                                FinalBonusGranted += 1.2;

                        }
                        else
                        {

                            // Prime stat gets a 25% bonus if 60 from max possible
                            if ((i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE || i == Constants.VITALITY) && (ItemMaxStats[i] - TempStatistic) < 60.5f)
                                FinalBonusGranted += 0.25;

                            // Within final 10 magic find
                            if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) < 10.2f)
                                FinalBonusGranted += 0.4;

                            // Within final 10 gold find
                            if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) < 10.2f)
                                FinalBonusGranted += 0.35;

                            // Within final 40 life on hit
                            if (i == Constants.LIFEONHIT && (ItemMaxStats[i] - TempStatistic) < 40.2f)
                                FinalBonusGranted += 1.2;

                        }

                        // Within final 50 armor
                        if (i == Constants.ARMOR && (ItemMaxStats[i] - TempStatistic) < 50.2f)
                        {
                            FinalBonusGranted += 0.30;
                            if ((HadStat[Constants.STRENGTH] / ItemMaxStats[Constants.STRENGTH]) > Constants.MinimumThreshold[Constants.STRENGTH])
                                FinalBonusGranted += 0.30;
                        }

                        // Within final 15 armor
                        if (i == Constants.ARMOR && (ItemMaxStats[i] - TempStatistic) < 15.2f)
                            FinalBonusGranted += 0.20;

                        // More than 2.5 crit chance out
                        if (i == Constants.CRITCHANCE && (ItemMaxStats[i] - TempStatistic) > 5.55f)
                            FinalBonusGranted -= 0.20;

                        // More than 20 crit damage out
                        if (i == Constants.CRITDAMAGE && (ItemMaxStats[i] - TempStatistic) > 19.95f)
                            FinalBonusGranted -= 0.20;

                        // More than 2 attack speed out
                        if (i == Constants.ATTACKSPEED && (ItemMaxStats[i] - TempStatistic) > 1.95f)
                            FinalBonusGranted -= 0.20;

                        // More than 15 gold find out
                        if (i == Constants.GOLDFIND && (ItemMaxStats[i] - TempStatistic) > 15.2f)
                            FinalBonusGranted -= 0.1;

                        // More than 15 magic find out
                        if (i == Constants.MAGICFIND && (ItemMaxStats[i] - TempStatistic) > 15.2f)
                            FinalBonusGranted -= 0.1;

                        // More than 30 all resist out
                        if (i == Constants.ALLRESIST && (ItemMaxStats[i] - TempStatistic) > 20.2f)
                            FinalBonusGranted -= 0.1;

                        // More than 40 all resist out
                        if (i == Constants.ALLRESIST && (ItemMaxStats[i] - TempStatistic) > 30.2f)
                            FinalBonusGranted -= 0.1;
                    }

                    // All the "set to 0" checks now

                    // Disable specific primary stat scoring for certain class-specific item types
                    if ((itemType == TrinityItemType.VoodooMask || itemType == TrinityItemType.WizardHat || itemType == TrinityItemType.Wand ||
                        itemType == TrinityItemType.CeremonialKnife || itemType == TrinityItemType.Mojo || itemType == TrinityItemType.Orb)
                        && (i == Constants.STRENGTH || i == Constants.DEXTERITY))
                        FinalBonusGranted = 0;
                    if ((itemType == TrinityItemType.Quiver || itemType == TrinityItemType.HandCrossbow || itemType == TrinityItemType.Cloak ||
                        itemType == TrinityItemType.SpiritStone || itemType == TrinityItemType.TwoHandDaibo || itemType == TrinityItemType.FistWeapon)
                        && (i == Constants.STRENGTH || i == Constants.INTELLIGENCE))
                        FinalBonusGranted = 0;
                    if ((itemType == TrinityItemType.MightyBelt || itemType == TrinityItemType.MightyWeapon || itemType == TrinityItemType.TwoHandMighty)
                        && (i == Constants.DEXTERITY || i == Constants.INTELLIGENCE))
                        FinalBonusGranted = 0;

                    // Remove unwanted follower stats for specific follower types
                    if (itemType == TrinityItemType.FollowerEnchantress && (i == Constants.STRENGTH || i == Constants.DEXTERITY))
                        FinalBonusGranted = 0;
                    if (itemType == TrinityItemType.FollowerEnchantress && (i == Constants.INTELLIGENCE || i == Constants.VITALITY))
                        FinalBonusGranted -= 0.4;
                    if (itemType == TrinityItemType.FollowerScoundrel && (i == Constants.STRENGTH || i == Constants.INTELLIGENCE))
                        FinalBonusGranted = 0;
                    if (itemType == TrinityItemType.FollowerScoundrel && (i == Constants.DEXTERITY || i == Constants.VITALITY))
                        FinalBonusGranted -= 0.4;
                    if (itemType == TrinityItemType.FollowerTemplar && (i == Constants.DEXTERITY || i == Constants.INTELLIGENCE))
                        FinalBonusGranted = 0;
                    if (itemType == TrinityItemType.FollowerTemplar && (i == Constants.STRENGTH || i == Constants.VITALITY))
                        FinalBonusGranted -= 0.4;

                    // Attack speed is always on a quiver so forget it
                    if ((itemType == TrinityItemType.Quiver) && (i == Constants.ATTACKSPEED))
                        FinalBonusGranted = 0;

                    // Single resists worth nothing without all-resist
                    if (i == Constants.RANDOMRESIST && (HadStat[Constants.ALLRESIST] / ItemMaxStats[Constants.ALLRESIST]) < Constants.MinimumThreshold[Constants.ALLRESIST])
                        FinalBonusGranted = 0;
                    if (FinalBonusGranted < 0)
                        FinalBonusGranted = 0;

                    // Grant the final bonus total
                    iTempPoints *= FinalBonusGranted;

                    // If it's a primary stat, log the highest scoring primary... else add these points to the running total
                    if (i == Constants.DEXTERITY || i == Constants.STRENGTH || i == Constants.INTELLIGENCE)
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "---- +" + iTempPoints.ToString("0") + " (*" + FinalBonusGranted.ToString("0.00") + " multiplier) [MUST BE MAX STAT SCORE TO COUNT]", true);

                        if (iTempPoints > HighestScoringPrimary)
                        {
                            HighestScoringPrimary = iTempPoints;
                            WhichPrimaryIsHighest = i;
                            AmountHighestScoringPrimary = TempStatistic;
                        }
                    }
                    else
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "---- +" + iTempPoints.ToString("0") + " score (*" + FinalBonusGranted.ToString("0.00") + " multiplier)", true);

                        TotalItemPoints += iTempPoints;
                    }
                    HadPoints[i] = iTempPoints;

                    // For item logs
                    if (i != Constants.DEXTERITY && i != Constants.STRENGTH && i != Constants.INTELLIGENCE)
                    {
                        if (ValueItemStatString != "")
                            ValueItemStatString += ". ";
                        ValueItemStatString += Constants.StatNames[i] + "=" + Math.Round(TempStatistic).ToString();
                        if (JunkItemStatString != "")
                            JunkItemStatString += ". ";
                        JunkItemStatString += Constants.StatNames[i] + "=" + Math.Round(TempStatistic).ToString();
                    }
                }
            }

            // Now add on one of the three primary stat scores, whichever was higher
            if (HighestScoringPrimary > 0)
            {

                // Give a 30% of primary-stat-score-possible bonus to the primary scoring if paired with a good amount of life % or vitality
                if ((HadStat[Constants.VITALITY] / ItemMaxStats[Constants.VITALITY] > (Constants.MinimumThreshold[Constants.VITALITY] + 0.1)) || SafeLifePercentage > 0.85)
                    HighestScoringPrimary += ItemMaxPoints[WhichPrimaryIsHighest] * 0.3;

                // Reduce a primary a little if there is no vitality or life
                if ((HadStat[Constants.VITALITY] < 40) || SafeLifePercentage < 0.7)
                    HighestScoringPrimary *= 0.8;
                TotalItemPoints += HighestScoringPrimary;

                ValueItemStatString = Constants.StatNames[WhichPrimaryIsHighest] + "=" + Math.Round(AmountHighestScoringPrimary).ToString() + ". " + ValueItemStatString;
                JunkItemStatString = Constants.StatNames[WhichPrimaryIsHighest] + "=" + Math.Round(AmountHighestScoringPrimary).ToString() + ". " + JunkItemStatString;

            }
            Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "--- +" + TotalItemPoints.ToString("0") + " total score pre-special reductions. (GlobalMultiplier=" + GlobalMultiplier.ToString("0.000") + ")", true);

            // Global multiplier
            TotalItemPoints *= GlobalMultiplier;

            // 2 handed weapons and ranged weapons lose a large score for low DPS
            if (baseItemType == TrinityItemBaseType.WeaponRange || baseItemType == TrinityItemBaseType.WeaponTwoHand)
            {
                if ((HadStat[Constants.TOTALDPS] / ItemMaxStats[Constants.TOTALDPS]) <= 0.7)
                    TotalItemPoints *= 0.75;
            }

            // Weapons should get a nice 15% bonus score for having very high primaries
            if (baseItemType == TrinityItemBaseType.WeaponRange || baseItemType == TrinityItemBaseType.WeaponOneHand || baseItemType == TrinityItemBaseType.WeaponTwoHand)
            {
                if (HighestScoringPrimary > 0 && (HighestScoringPrimary >= ItemMaxPoints[WhichPrimaryIsHighest] * 0.9))
                {
                    TotalItemPoints *= 1.15;
                }

                // And an extra 15% for a very high vitality
                if (HadStat[Constants.VITALITY] > 0 && (HadStat[Constants.VITALITY] >= ItemMaxPoints[Constants.VITALITY] * 0.9))
                {
                    TotalItemPoints *= 1.15;
                }

                // And an extra 15% for a very high life-on-hit
                if (HadStat[Constants.LIFEONHIT] > 0 && (HadStat[Constants.LIFEONHIT] >= ItemMaxPoints[Constants.LIFEONHIT] * 0.9))
                {
                    TotalItemPoints *= 1.15;
                }
            }

            // Shields 
            if (itemType == TrinityItemType.Shield)
            {

                // Strength/Dex based shield calculations
                if (WhichPrimaryIsHighest == Constants.STRENGTH || WhichPrimaryIsHighest == Constants.DEXTERITY)
                {
                    if (HadStat[Constants.BLOCKCHANCE] < 20)
                    {
                        TotalItemPoints *= 0.7;
                    }
                    else if (HadStat[Constants.BLOCKCHANCE] < 25)
                    {
                        TotalItemPoints *= 0.9;
                    }
                }

                // Intelligence/no primary based shields
                else
                {
                    if (HadStat[Constants.BLOCKCHANCE] < 28)
                        TotalItemPoints -= HadPoints[Constants.BLOCKCHANCE];
                }
            }

            // Quivers
            if (itemType == TrinityItemType.Quiver)
            {
                TotalRequirements = 0;
                if (HadStat[Constants.DEXTERITY] >= 100)
                    TotalRequirements++;
                else
                    TotalRequirements -= 3;
                if (HadStat[Constants.DEXTERITY] >= 160)
                    TotalRequirements++;
                if (HadStat[Constants.DEXTERITY] >= 250)
                    TotalRequirements++;
                if (HadStat[Constants.ATTACKSPEED] < 14)
                    TotalRequirements -= 2;
                if (HadStat[Constants.VITALITY] >= 70 || SafeLifePercentage >= 0.85)
                    TotalRequirements++;
                else
                    TotalRequirements--;
                if (HadStat[Constants.VITALITY] >= 260)
                    TotalRequirements++;
                if (HadStat[Constants.MAXDISCIPLINE] >= 8)
                    TotalRequirements++;
                if (HadStat[Constants.MAXDISCIPLINE] >= 10)
                    TotalRequirements++;
                if (HadStat[Constants.SOCKETS] >= 1)
                    TotalRequirements++;
                if (HadStat[Constants.CRITCHANCE] >= 6)
                    TotalRequirements++;
                if (HadStat[Constants.CRITCHANCE] >= 8)
                    TotalRequirements++;
                if (HadStat[Constants.LIFEPERCENT] >= 8)
                    TotalRequirements++;
                if (HadStat[Constants.MAGICFIND] >= 18)
                    TotalRequirements++;
                if (TotalRequirements < 4)
                    TotalItemPoints *= 0.4;
                else if (TotalRequirements < 5)
                    TotalItemPoints *= 0.5;
                if (TotalRequirements >= 7)
                    TotalItemPoints *= 1.2;
            }

            // Mojos and Sources
            if (itemType == TrinityItemType.Orb || itemType == TrinityItemType.Mojo)
            {
                TotalRequirements = 0;
                if (HadStat[Constants.INTELLIGENCE] >= 100)
                    TotalRequirements++;
                else if (HadStat[Constants.INTELLIGENCE] < 80)
                    TotalRequirements -= 3;
                else if (HadStat[Constants.INTELLIGENCE] < 100)
                    TotalRequirements -= 1;
                if (HadStat[Constants.INTELLIGENCE] >= 160)
                    TotalRequirements++;
                if (HadStat[Constants.MAXDAMAGE] >= 250)
                    TotalRequirements++;
                else
                    TotalRequirements -= 2;
                if (HadStat[Constants.MAXDAMAGE] >= 340)
                    TotalRequirements++;
                if (HadStat[Constants.MINDAMAGE] >= 50)
                    TotalRequirements++;
                else
                    TotalRequirements--;
                if (HadStat[Constants.MINDAMAGE] >= 85)
                    TotalRequirements++;
                if (HadStat[Constants.VITALITY] >= 70)
                    TotalRequirements++;
                if (HadStat[Constants.SOCKETS] >= 1)
                    TotalRequirements++;
                if (HadStat[Constants.CRITCHANCE] >= 6)
                    TotalRequirements++;
                if (HadStat[Constants.CRITCHANCE] >= 8)
                    TotalRequirements++;
                if (HadStat[Constants.LIFEPERCENT] >= 8)
                    TotalRequirements++;
                if (HadStat[Constants.MAGICFIND] >= 15)
                    TotalRequirements++;
                if (HadStat[Constants.MAXMANA] >= 60)
                    TotalRequirements++;
                if (HadStat[Constants.ARCANECRIT] >= 8)
                    TotalRequirements++;
                if (HadStat[Constants.ARCANECRIT] >= 10)
                    TotalRequirements++;
                if (TotalRequirements < 4)
                    TotalItemPoints *= 0.4;
                else if (TotalRequirements < 5)
                    TotalItemPoints *= 0.5;
                if (TotalRequirements >= 8)
                    TotalItemPoints *= 1.2;
            }

            // Chests/cloaks/pants without a socket lose 17% of total score
            if ((itemType == TrinityItemType.Chest || itemType == TrinityItemType.Cloak || itemType == TrinityItemType.Legs) && HadStat[Constants.SOCKETS] == 0)
                TotalItemPoints *= 0.83;

            // Boots with no movement speed get reduced score
            if ((itemType == TrinityItemType.Boots) && HadStat[Constants.MOVEMENTSPEED] <= 6)
                TotalItemPoints *= 0.75;

            // Helmets
            if (itemType == TrinityItemType.Helm || itemType == TrinityItemType.WizardHat || itemType == TrinityItemType.VoodooMask || itemType == TrinityItemType.SpiritStone)
            {
                // Helmets without a socket lose 20% of total score, and most of any MF/GF bonus
                if (HadStat[Constants.SOCKETS] == 0)
                {
                    TotalItemPoints *= 0.8;
                    if (HadStat[Constants.MAGICFIND] > 0 || HadStat[Constants.GOLDFIND] > 0)
                    {
                        if (HadStat[Constants.MAGICFIND] > 0 && HadStat[Constants.GOLDFIND] > 0)
                            TotalItemPoints -= ((HadPoints[Constants.MAGICFIND] * 0.25) + (HadPoints[Constants.GOLDFIND] * 0.25));
                        else
                            TotalItemPoints -= ((HadPoints[Constants.MAGICFIND] * 0.65) + (HadPoints[Constants.GOLDFIND] * 0.65));
                    }
                }
            }

            Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "--- +" + TotalItemPoints.ToString("0") + " total score after special reductions. (TotalRequirements=" + TotalRequirements + ")", true);

            GetBestFinalPoints(itemType);

            TotalItemPoints *= BestFinalBonus;

            Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "TOTAL: " + TotalItemPoints.ToString("0") + "(Final Bonus=" + BestFinalBonus.ToString("0.00") + ")");

            Logger.Log(TrinityLogLevel.Verbose, LogCategory.ItemValuation, "");

            return Math.Round(TotalItemPoints);
        }

        public static string ValueItemStatString { get; set; }

        internal static void ResetValuationStatStrings()
        {
            ValueItemStatString = "";
            JunkItemStatString = "";
        }

        public static string JunkItemStatString { get; set; }

        private static void CheckForInvalidItemType(TrinityItemType itemType)
        {
            // One Handed Weapons 
            if (itemType == TrinityItemType.Axe || itemType == TrinityItemType.CeremonialKnife || itemType == TrinityItemType.Dagger ||
                 itemType == TrinityItemType.FistWeapon || itemType == TrinityItemType.Mace || itemType == TrinityItemType.MightyWeapon ||
                 itemType == TrinityItemType.Spear || itemType == TrinityItemType.Sword || itemType == TrinityItemType.Wand ||
                 itemType == TrinityItemType.HandCrossbow)
            {
                Array.Copy(Constants.MaxPointsWeaponOneHand, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.WeaponPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Two Handed Weapons
            if (itemType == TrinityItemType.TwoHandAxe || itemType == TrinityItemType.TwoHandDaibo || itemType == TrinityItemType.TwoHandMace ||
                itemType == TrinityItemType.TwoHandMighty || itemType == TrinityItemType.TwoHandPolearm || itemType == TrinityItemType.TwoHandStaff ||
                itemType == TrinityItemType.TwoHandSword ||
                itemType == TrinityItemType.TwoHandCrossbow || itemType == TrinityItemType.TwoHandBow)
            {
                Array.Copy(Constants.MaxPointsWeaponTwoHand, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.WeaponPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }
            // Off-handed stuff

            // Mojo, Source, Quiver
            if (itemType == TrinityItemType.Mojo || itemType == TrinityItemType.Orb || itemType == TrinityItemType.Quiver)
            {
                Array.Copy(Constants.MaxPointsOffHand, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Shields
            if (itemType == TrinityItemType.Shield)
            {
                Array.Copy(Constants.MaxPointsShield, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Jewelry

            // Ring
            if (itemType == TrinityItemType.Amulet)
            {
                Array.Copy(Constants.MaxPointsAmulet, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.JewelryPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Ring
            if (itemType == TrinityItemType.Ring)
            {
                Array.Copy(Constants.MaxPointsRing, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.JewelryPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Armor

            // Belt
            if (itemType == TrinityItemType.Belt)
            {
                Array.Copy(Constants.MaxPointsBelt, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Boots
            if (itemType == TrinityItemType.Boots)
            {
                Array.Copy(Constants.MaxPointsBoots, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Bracers
            if (itemType == TrinityItemType.Bracer)
            {
                Array.Copy(Constants.MaxPointsBracer, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Chest
            if (itemType == TrinityItemType.Chest)
            {
                Array.Copy(Constants.MaxPointsChest, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }
            if (itemType == TrinityItemType.Cloak)
            {
                Array.Copy(Constants.MaxPointsCloak, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Gloves
            if (itemType == TrinityItemType.Gloves)
            {
                Array.Copy(Constants.MaxPointsGloves, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Helm
            if (itemType == TrinityItemType.Helm)
            {
                Array.Copy(Constants.MaxPointsHelm, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Pants
            if (itemType == TrinityItemType.Legs)
            {
                Array.Copy(Constants.MaxPointsPants, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }
            if (itemType == TrinityItemType.MightyBelt)
            {
                Array.Copy(Constants.MaxPointsMightyBelt, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Shoulders
            if (itemType == TrinityItemType.Shoulder)
            {
                Array.Copy(Constants.MaxPointsShoulders, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }
            if (itemType == TrinityItemType.SpiritStone)
            {
                Array.Copy(Constants.MaxPointsSpiritStone, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }
            if (itemType == TrinityItemType.VoodooMask)
            {
                Array.Copy(Constants.MaxPointsVoodooMask, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Wizard Hat
            if (itemType == TrinityItemType.WizardHat)
            {
                Array.Copy(Constants.MaxPointsWizardHat, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.ArmorPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }

            // Follower Items
            if (itemType == TrinityItemType.FollowerEnchantress || itemType == TrinityItemType.FollowerScoundrel || itemType == TrinityItemType.FollowerTemplar)
            {
                Array.Copy(Constants.MaxPointsFollower, ItemMaxStats, Constants.TOTALSTATS);
                Array.Copy(Constants.JewelryPointsAtMax, ItemMaxPoints, Constants.TOTALSTATS);
                IsInvalidItem = false;
            }
        }

        /// <summary>
        /// Define Special Reductions
        /// </summary>
        /// <param name="itemType"></param>
        private static void GetBestFinalPoints(TrinityItemType itemType)
        {
            // Gold-find and pickup radius combined
            if ((HadStat[Constants.GOLDFIND] / ItemMaxStats[Constants.GOLDFIND] > 0.55) && (HadStat[Constants.PICKUPRADIUS] / ItemMaxStats[Constants.PICKUPRADIUS] > 0.5))
                TotalItemPoints += (((ItemMaxPoints[Constants.PICKUPRADIUS] + ItemMaxPoints[Constants.GOLDFIND]) / 2) * 0.25);

            // All-resist and pickup radius combined
            if ((HadStat[Constants.ALLRESIST] / ItemMaxStats[Constants.ALLRESIST] > 0.55) && (HadStat[Constants.PICKUPRADIUS] > 0))
                TotalItemPoints += (((ItemMaxPoints[Constants.PICKUPRADIUS] + ItemMaxPoints[Constants.ALLRESIST]) / 2) * 0.65);

            // Special crit hit/crit chance/attack speed combos a.k.a Trifecta!
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.8)) && (HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.8)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.8)))
            {
                if (BestFinalBonus < 3.2 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 3.2;
            }

            /*
             *  2.3 Bonus for 80% 2 of 3 CritDmg/Crit%/AttackSpd Combo
             */
            // 80% of crit chance, 80% crit damage of max for item
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.8)) && (HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.8)))
            {
                if (BestFinalBonus < 2.3)
                    BestFinalBonus = 2.3;
            }
            // 80% of crit chance, 80% of attack speed of max for item
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.8)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.8)))
            {
                if (BestFinalBonus < 2.1 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 2.1;
            }
            // 80% of crit damage, 80% of attack speed of max for item
            if ((HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.8)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.8)))
            {
                if (BestFinalBonus < 1.8 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 1.8;
            }
            /*
             *  2.1 Bonus for 65% 2 of 3 CritDmg/Crit%/AttackSpd Combo
             */
            // 65% crit chance, 65% crit damage, 65% attack speed of max for item
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.65)) && (HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.65)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.65)))
            {
                if (BestFinalBonus < 2.1 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 2.1;
            }
            // 65% crit chance, 65% crit damage
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.65)) && (HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.65)))
            {
                if (BestFinalBonus < 1.9) BestFinalBonus = 1.9;
            }
            // 65% crit chance, 65% attack speed of max for item
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.65)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.65)))
            {
                if (BestFinalBonus < 1.7 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 1.7;
            }
            // 65% crit damage, 65% attack speed of max for item
            if ((HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.65)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.65)))
            {
                if (BestFinalBonus < 1.5 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 1.5;
            }
            /*
            *  1.7 Bonus for 45% 2 of 3 CritDmg/Crit%/AttackSpd Combo
            */
            // 45% crit chance, 45% crit damage, 45% attack speed of max for item
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.45)) && (HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.45)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.45)))
            {
                if (BestFinalBonus < 1.7 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 1.7;
            }
            // 45% crit chance, 45% crit damage, 45% attack speed of max for item
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.45)) && (HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.45)))
            {
                if (BestFinalBonus < 1.4) BestFinalBonus = 1.4;
            }
            // 45% crit chance, 45% attack speed of max for item
            if ((HadStat[Constants.CRITCHANCE] > (ItemMaxStats[Constants.CRITCHANCE] * 0.45)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.45)))
            {
                if (BestFinalBonus < 1.3 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 1.3;
            }
            // 45% crit damage, 45% attack speed of max for item
            if ((HadStat[Constants.CRITDAMAGE] > (ItemMaxStats[Constants.CRITDAMAGE] * 0.45)) && (HadStat[Constants.ATTACKSPEED] > (ItemMaxStats[Constants.ATTACKSPEED] * 0.45)))
            {
                if (BestFinalBonus < 1.1 && itemType != TrinityItemType.Quiver)
                    BestFinalBonus = 1.1;
            }
        }
        /// <summary>
        ///     Output test scores for everything in the backpack
        /// </summary>
        internal static void TestScoring()
        {
            //using (new PerformanceLogger("TestScoring"))
            //{
            //    using (new ZetaCacheHelper())
            //    {
            //        try
            //        {
            //            if (TownRun.TestingBackpack)
            //                return;
            //            TownRun.TestingBackpack = true;
            //            //ZetaDia.Actors.Update();
            //            if (ZetaDia.Actors.Me == null)
            //            {
            //                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Error testing scores - not in game world?");
            //                return;
            //            }
            //            if (ZetaDia.IsInGame && !ZetaDia.IsLoadingWorld)
            //            {
            //                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "===== Outputting Test Scores =====");
            //                foreach (ACDItem item in ZetaDia.Me.Inventory.Backpack)
            //                {
            //                    if (item.BaseAddress == IntPtr.Zero)
            //                    {
            //                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "GSError: Diablo 3 memory read error, or item became invalid [TestScore-1]");
            //                    }
            //                    else
            //                    {
            //                        bool shouldStash = ItemManager.Current.ShouldStashItem(item);
            //                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, shouldStash ? "* KEEP *" : "-- TRASH --");
            //                    }
            //                }
            //                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "===== Finished Test Score Outputs =====");
            //            }
            //            else
            //            {
            //                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Error testing scores - not in game world?");
            //            }
            //            TownRun.TestingBackpack = false;
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger.LogNormal("Exception in TestScoring(): {0}", ex);
            //            TownRun.TestingBackpack = false;
            //        }
            //    }
            //}
        }

        /// <summary>Return the score needed to keep something by the item type</summary>
        internal static double ScoreNeeded(ItemBaseType itemBaseType)
        {
            switch (itemBaseType)
            {
                case ItemBaseType.Weapon:
                    return Math.Round((double)TrinityPlugin.Settings.Loot.TownRun.WeaponScore);
                case ItemBaseType.Armor:
                    return Math.Round((double)TrinityPlugin.Settings.Loot.TownRun.ArmorScore);
                case ItemBaseType.Jewelry:
                    return Math.Round((double)TrinityPlugin.Settings.Loot.TownRun.JewelryScore);
                default:
                    return 0;
            }
        }

        /// <summary>
        ///     Checks if score of item is suffisant for throw notification.
        /// </summary>
        public static bool CheckScoreForNotification(TrinityItemBaseType itemBaseType, double itemValue)
        {
            switch (itemBaseType)
            {
                case TrinityItemBaseType.WeaponOneHand:
                case TrinityItemBaseType.WeaponRange:
                case TrinityItemBaseType.WeaponTwoHand:
                    return (itemValue >= TrinityPlugin.Settings.Notification.WeaponScore);
                case TrinityItemBaseType.Armor:
                case TrinityItemBaseType.Offhand:
                    return (itemValue >= TrinityPlugin.Settings.Notification.ArmorScore);
                case TrinityItemBaseType.Jewelry:
                    return (itemValue >= TrinityPlugin.Settings.Notification.JewelryScore);
            }
            return false;
        }

    }
}
