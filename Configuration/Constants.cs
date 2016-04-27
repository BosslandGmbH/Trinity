namespace Trinity
{
    public static class Constants
    {
        // These constants are used for item scoring and stashing
        internal const int DEXTERITY = 0;
        internal const int INTELLIGENCE = 1;
        internal const int STRENGTH = 2;
        internal const int VITALITY = 3;
        internal const int LIFEPERCENT = 4;
        internal const int LIFEONHIT = 5;
        internal const int LIFESTEAL = 6;
        internal const int LIFEREGEN = 7;
        internal const int MAGICFIND = 8;
        internal const int GOLDFIND = 9;
        internal const int MOVEMENTSPEED = 10;
        internal const int PICKUPRADIUS = 11;
        internal const int SOCKETS = 12;
        internal const int CRITCHANCE = 13;
        internal const int CRITDAMAGE = 14;
        internal const int ATTACKSPEED = 15;
        internal const int MINDAMAGE = 16;
        internal const int MAXDAMAGE = 17;
        internal const int BLOCKCHANCE = 18;
        internal const int THORNS = 19;
        internal const int ALLRESIST = 20;
        internal const int RANDOMRESIST = 21;
        internal const int TOTALDPS = 22;
        internal const int ARMOR = 23;
        internal const int MAXDISCIPLINE = 24;
        internal const int MAXMANA = 25;
        internal const int ARCANECRIT = 26;
        internal const int MANAREGEN = 27;
        internal const int GLOBEBONUS = 28;
        /// <summary>
        /// The number of stats we have to work with
        /// </summary>
        internal const int TOTALSTATS = 29;
        // starts at 0, remember... 0-26 = 1-27!
        // Readable names of the above stats that get output into the trash/stash log files
        internal static readonly string[] StatNames = new string[29] { 
            "Dexterity",
            "Intelligence",
            "Strength",
            "Vitality",
            "Life %",
            "Life On Hit",
            "Life Steal %",
            "Life Regen",
            "Magic Find %",
            "Gold Find   %",
            "Movement Speed %",
            "Pickup Radius",
            "Sockets",
            "Crit Chance %",
            "Crit Damage %",
            "Attack Speed %",
            "+Min Damage",
            "+Max Damage",
            "Total Block %",
            "Thorns",
            "+All Resist",
            "+Highest Single Resist",
            "DPS",
            "Armor",
            "Max Disc.",
            "Max Mana",
            "Arcane-On-Crit",
            "Mana Regen",
            "Globe Bonus"
        };
        // Stores the apparent maximums of each stat for each item slot
        // Note that while these SHOULD be *actual* maximums for most stats - for things like DPS, these can just be more sort of "what a best-in-slot DPS would be"
        // Variable name                                                  Dex, Int, Str, Vit, Life%,  LOH,Steal%, LPS, MF, GF, MS, Rad, Sox,  Crit%, CDam%, ASPD,  Min+, Max+, Blk, Thrn, AR, SR,  DPS, ARMOR, Disc, Mana, Arc.,  Regen, Globes (Values were updated to 85% of max based on d3maxstats.com and experience by DarkTitan)
        internal static double[] MaxPointsWeaponTwoHand = new double[29] { 956, 956, 956, 956, 0, 4000, 5.8, 0, 0, 0, 0, 0, 1, 0, 170, 0, 0, 0, 0, 0, 0, 0, 2285, 0, 10, 119, 10, 14, 0 };
        internal static double[] MaxPointsWeaponOneHand = new double[29] { 956, 956, 1125, 956, 0, 2800, 2.8, 0, 0, 0, 0, 0, 1, 0, 85, 0, 0, 0, 0, 0, 0, 0, 1870, 0, 10, 150, 10, 14, 0 };
        internal static double[] MaxPointsWeaponRanged = new double[29] { 562, 562, 562, 562, 0, 2800, 2.8, 0, 0, 0, 0, 0, 1, 0, 85, 0, 0, 0, 0, 0, 0, 0, 2040, 0, 10, 0, 0, 14, 0 };
        internal static double[] MaxPointsOffHand = new double[29] { 562, 562, 562, 562, 9, 0, 0, 2000, 18, 20, 0, 0, 1, 8.5, 0, 15, 210, 402, 0, 979, 0, 0, 0, 0, 10, 119, 10, 11, 25256 };
        internal static double[] MaxPointsShield = new double[29] { 562, 562, 562, 562, 16, 0, 0, 2000, 20, 25, 0, 0, 1, 10, 0, 0, 0, 0, 30, 2544, 80, 136, 0, 397, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsRing = new double[29] { 425, 425, 425, 425, 12, 1600, 0, 1834, 20, 25, 0, 0, 1, 6, 50, 9, 36, 100, 0, 979, 80, 136, 0, 240, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsAmulet = new double[29] { 562, 562, 562, 562, 16, 2500, 0, 2000, 45, 50, 0, 0, 1, 10, 100, 9, 36, 100, 0, 1712, 80, 136, 0, 360, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsShoulders = new double[29] { 425, 425, 425, 425, 12, 0, 0, 2000, 20, 25, 0, 7, 0, 0, 0, 0, 0, 0, 0, 2544, 80, 136, 0, 265, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsHelm = new double[29] { 562, 562, 562, 562, 12, 0, 0, 2000, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 80, 136, 0, 397, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsPants = new double[29] { 425, 425, 425, 425, 0, 0, 0, 2000, 20, 25, 0, 7, 2, 0, 0, 0, 0, 0, 0, 1454, 80, 136, 0, 397, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsGloves = new double[29] { 562, 562, 562, 562, 0, 0, 0, 2000, 20, 25, 0, 7, 0, 10, 50, 9, 0, 0, 0, 1454, 80, 136, 0, 265, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsChest = new double[29] { 425, 425, 425, 425, 12, 0, 0, 2000, 20, 25, 0, 7, 3, 0, 0, 0, 0, 0, 0, 2544, 80, 136, 0, 397, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsBracer = new double[29] { 425, 425, 425, 425, 0, 0, 0, 2000, 20, 25, 0, 7, 0, 6, 0, 0, 0, 0, 0, 1454, 80, 136, 0, 265, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsBoots = new double[29] { 425, 425, 425, 425, 0, 0, 0, 2000, 20, 25, 12, 7, 0, 0, 0, 0, 0, 0, 0, 1454, 80, 136, 0, 265, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsBelt = new double[29] { 425, 425, 425, 425, 12, 0, 0, 2000, 20, 25, 0, 7, 0, 0, 0, 0, 0, 0, 0, 2544, 80, 136, 0, 265, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsCloak = new double[29] { 425, 425, 425, 425, 12, 0, 0, 2000, 20, 25, 0, 7, 3, 0, 0, 0, 0, 0, 0, 2544, 80, 136, 0, 397, 10, 0, 0, 0, 25256 };
        internal static double[] MaxPointsMightyBelt = new double[29] { 425, 425, 425, 425, 12, 0, 2000, 485, 20, 25, 0, 7, 0, 0, 0, 0, 0, 0, 0, 2544, 80, 136, 0, 265, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsSpiritStone = new double[29] { 562, 562, 562, 562, 12, 0, 2000, 2104, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 80, 136, 0, 397, 0, 0, 0, 0, 25256 };
        internal static double[] MaxPointsVoodooMask = new double[29] { 562, 562, 562, 562, 12, 0, 2000, 485, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 80, 136, 0, 397, 0, 119, 0, 11, 25256 };
        internal static double[] MaxPointsWizardHat = new double[29] { 562, 562, 562, 562, 12, 0, 2000, 485, 20, 25, 0, 7, 1, 6, 0, 0, 0, 0, 0, 1454, 80, 136, 0, 397, 0, 0, 10, 0, 25256 };
        internal static double[] MaxPointsFollower = new double[29] { 425, 425, 425, 425, 0, 300, 0, 234, 0, 0, 0, 0, 0, 0, 55, 0, 0, 0, 0, 0, 50, 40, 0, 0, 0, 0, 0, 0, 0 };

        /* These are all missing from the valuation calculations: 

        Life Per Spirit Spent
        Bonus Experience
        Reduced Level Requirement
        Weapon % Damange modifier
        Bonus Elemental Dmg
        Bonus Cold Damage
        Reduced Dmg from Melee %
        Reduced Dmg from Ranged %
        Reduced Dmg from Elites %
        Crowd Control Reduction %

        (All Chance % to crowd control)
        Chance to Stun %
        Chance to Knockback %
        Chance to Slow %
        Chance to Immobilize %
        Chance to Freeze %
        Chance to Fear %
        Chance to Chill %
        Chance to Blind %

        Spirit Regen per second
        Hatred regen per second
        Extra max fury
        Extra max arcane

        */

        // Stores the total points this stat is worth at the above % point of maximum
        // Note that these values get all sorts of bonuses+ multipliers+ and extra things applied in the actual scoring routine. These values are more of a base value.
        // Variable name                                                Dex,   Int,   Str,   Vit,    L%,   LOH, Stl%, LPS,    MF,   GF,   MS,  Rad,   Sox, Crit%, CDam%, ASPD, Min+, Max+,  Blk,  Thn,    AR,   SR,   DPS, ARMR,  Disc, Mana, Arc.,  Regn, Globe
        internal static double[] WeaponPointsAtMax = new double[29] { 4000, 4000, 4000, 2000, 2000, 7000, 7000, 1000, 2000, 2000, 10000, 425, 15000, 10000, 10000, 0, 0, 0, 0, 0, 13000, 0, 60000, 0, 10000, 8500, 8500, 2000, 1000 };
        internal static double[] ArmorPointsAtMax = new double[29] { 9000, 9000, 9000, 6000, 6000, 10000, 8000, 1200, 2000, 0, 8000, 1000, 9000, 9000, 8000, 8000, 3000, 3000, 0, 0, 10000, 3000, 0, 3000, 4000, 3000, 3000, 2000, 1000 };
        internal static double[] JewelryPointsAtMax = new double[29] { 6000, 6000, 6000, 4000, 6000, 6000, 8000, 1200, 2000, 0, 3500, 1000, 3000, 8000, 8000, 8000, 4000, 4000, 0, 0, 10000, 3000, 0, 3000, 4000, 3000, 3000, 2000, 1000 };
        // Some special values for score calculations 
        // BonusThreshold is a percentage of the max-stat possible that the stat starts to get a multiplier on it's score. 1 means it has to be above 100% of the max-stat to get a multiplier (so only possible if the max-stat isn't ACTUALLY the max possible)
        // MinimumThreshold is a percentage of the max-stat possible that the stat will simply be ignored for being too low. eg if set to .5 - then anything less than 50% of the max-stat will be ignored.
        // MinimumPrimary is used for some stats only - and means that at least ONE primary stat has to be above that level  to get score. Eg magic-find has .5 - meaning any item without at least 50% of a max-stat primary  will ignore magic-find scoring.
        // Variable name                                               Dex,  Int,  Str,  Vit,   L%,  LOH, Stl%,  LPS,   MF,   GF,   MS,  Rad,  Sox, Crit, CDam, ASPD, Min+, Max+,  Blk, Thn,    AR,   SR,  DPS, ARMR, Disc, Mana, Arc., Regn, Globe
        internal static double[] BonusThreshold = new double[29] { .90, .90, .90, .90, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, .90, 1, 0.9, 0.9, 0.9, .9, 1 };
        internal static double[] MinimumThreshold = new double[29] { .40, .40, .40, .30, .30, .30, .55, .7, .7, .7, .70, .8, 0, .30, .50, .40, .2, .2, .65, .6, .40, .60, .70, .60, .7, .7, .7, .7, .8 };
        internal static double[] StatMinimumPrimary = new double[29] { 0, 0, 0, 0.3, 0.3, 0, 0, .2, .50, .50, .50, 0, 0, 0, 0, 30, .40, .40, .40, .40, .30, .60, 0, .40, .40, .40, .40, .4, .4 };

        ////												                  Dex  Int  Str  Vit  Life% LOH Steal%  LPS Magic% Gold% MSPD Rad. Sox Crit% CDam% ASPD Min+ Max+ Block% Thorn Allres Res   DPS  ARMOR Disc.Mana Arc. Regen  Globes
        //internal static double[] MaxPointsWeaponOneHand = new double[29] { 400, 400, 400, 400, 0,   1000, 3,      0,    0,    0,   0,   0,   1,   0,   85,   0,   0,   0,    0,     0,    0,    0,  1500, 0,     10, 150, 10,   14, 0 };
        //internal static double[] MaxPointsWeaponTwoHand = new double[29] { 530, 530, 530, 530, 0,   1800, 5.8,    0,    0,    0,   0,   0,   1,   0,  170,  0,   0,   0,    0,     0,    0,    0,  1700, 0,     10, 119, 10,   14, 0 };
        //internal static double[] MaxPointsWeaponRanged = new double[29]  { 320, 320, 320, 320, 0,    850, 2.8,    0,    0,    0,   0,   0,   1,   0,   85, 0, 0, 0, 0, 0, 0, 0, 1410, 0, 0, 0, 0, 14, 0 };
        //internal static double[] MaxPointsOffHand = new double[29]       { 300, 300, 300, 300, 9,      0, 0,    234,   18,   20,   0,   0,   1, 8.5,    0, 15, 110, 402, 0, 979, 0, 0, 0, 0, 10, 119, 10, 11, 5977 };
        //internal static double[] MaxPointsShield = new double[29]        { 330, 330, 330, 330, 16,     0, 0,    485,   20,   25,   0,   0,   1,  10,    0, 0, 0, 0, 30, 2544, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsRing = new double[29]          { 200, 200, 200, 200, 12,   479, 0,    234,   20,   25,   0,   0,   1,   6,   50, 9, 36, 100, 0, 979, 80, 60, 0, 240, 0, 0, 0, 0, 5977 };
        //internal static double[] MaxPointsAmulet = new double[29]        { 350, 350, 350, 350, 16,   959, 0,    410,   45,   50,   0,   0,   1,  10,  100, 9, 36, 100, 0, 1712, 80, 60, 0, 360, 0, 0, 0, 0, 5977 };
        //internal static double[] MaxPointsShoulders = new double[29]     { 200, 200, 300, 200, 12,      0, 0,   485,   20,   25,   0,   7,   0,   0,    0, 0, 0, 0, 0, 2544, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsHelm = new double[29]          { 200, 300, 200, 200, 12,      0, 0,   485,   20,   25,   0,   7,   1,   6,    0, 0, 0, 0, 0, 1454, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsPants = new double[29]         { 200, 200, 200, 300, 0,       0, 0,   485,   20,   25,   0,   7,   2,   0,    0, 0, 0, 0, 0, 1454, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsGloves = new double[29]        { 300, 300, 200, 200, 0,       0, 0,   485,   20,   25,   0,   7,   0,  10,   50, 9, 0, 0, 0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsChest = new double[29]         { 200, 200, 200, 300, 12,      0, 0,   599,   20,   25,   0,   7,   3,   0,    0, 0, 0, 0, 0, 2544, 80, 60, 0, 397, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsBracer = new double[29]        { 200, 200, 200, 200, 0,       0, 0,   485,   20,   25,   0,   7,   0,   6,    0, 0, 0, 0, 0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsBoots = new double[29]         { 300, 200, 200, 200, 0,       0, 0,   485,   20,   25,   12,  7,   0,   0,    0, 0, 0, 0, 0, 1454, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsBelt = new double[29]          { 200, 200, 300, 200, 12,      0, 0,   485,   20,   25,   0,   7,   0,   0,    0, 0, 0, 0, 0, 2544, 80, 60, 0, 265, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsCloak = new double[29]         { 200, 200, 200, 300, 12,      0, 0,   410,   20,   25,   0,   7,   3,   0,    0, 0, 0, 0, 0, 2544, 70, 50, 0, 397, 10, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsMightyBelt = new double[29]    { 200, 200, 300, 200, 12,      0, 3,   485,   20,   25,   0,   7,   0,   0,    0, 0, 0, 0, 0, 2544, 70, 50, 0, 265, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsSpiritStone = new double[29]   { 200, 300, 200, 200, 12,      0, 0,   485,   20,   25,   0,   7,   1,   6,    0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 0, 0, 0, 12794 };
        //internal static double[] MaxPointsVoodooMask = new double[29]    { 200, 300, 200, 200, 12,      0, 0,   485,   20,   25,   0,   7,   1,   6,    0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 119, 0, 11, 12794 };
        //internal static double[] MaxPointsWizardHat = new double[29]     { 200, 300, 200, 200, 12,      0, 0,   485,   20,   25,   0,   7,   1,   6,    0, 0, 0, 0, 0, 1454, 70, 50, 0, 397, 0, 0, 10, 0, 12794 };
        //internal static double[] MaxPointsFollower = new double[29]      { 300, 300, 300, 200, 0,     300, 0,   234,   0,     0,   0,   0,   0,   0, 55, 0, 0, 0, 0, 0, 50, 40, 0, 0, 0, 0, 0, 0, 0 };
        //// Stores the total points this stat is worth at the above % point of maximum
        //// Note that these values get all sorts of bonuses, multipliers, and extra things applied in the actual scoring routine. These values are more of a "base" value.
        ////                                                              Dex    Int    Str    Vit    Life%  LOH    Steal% LPS   Magic%  Gold%  MSPD   Rad  Sox    Crit%  CDam%  ASPD   Min+  Max+ Block% Thorn Allres Res   DPS    ARMOR  Disc.  Mana  Arc.  Regen  Globes
        //internal static double[] WeaponPointsAtMax = new double[29] { 14000, 14000, 14000, 14000, 13000, 20000, 7000, 1000, 6000, 6000, 6000, 425, 16000, 15000, 15000, 0, 0, 0, 0, 1000, 11000, 0, 64000, 0, 10000, 8500, 8500, 10000, 8000 };
        ////                                                              Dex    Int    Str    Vit    Life%  LOH    Steal% LPS   Magic%  Gold%  MSPD   Rad. Sox    Crit%  CDam%  ASPD   Min+  Max+ Block% Thorn Allres Res   DPS    ARMOR  Disc.  Mana  Arc.  Regen  Globes
        //internal static double[] ArmorPointsAtMax = new double[29] { 11000, 11000, 11000, 9500, 9000, 10000, 4000, 1200, 3000, 3000, 3500, 1000, 4300, 9000, 6100, 7000, 3000, 3000, 5000, 1200, 7500, 1500, 0, 5000, 4000, 3000, 3000, 6000, 5000 };
        //internal static double[] JewelryPointsAtMax = new double[29] { 11500, 11000, 11000, 10000, 8000, 11000, 4000, 1200, 4500, 4500, 3500, 1000, 3500, 7500, 6300, 6800, 800, 800, 5000, 1200, 7500, 1500, 0, 4500, 4000, 3000, 3000, 6000, 5000 };
        //// Some special values for score calculations
        //// BonusThreshold is a percentage of the "max-stat possible", that the stat starts to get a multiplier on it's score. 1 means it has to be above 100% of the "max-stat" to get a multiplier (so only possible if the max-stat isn't ACTUALLY the max possible)
        //// MinimumThreshold is a percentage of the "max-stat possible", that the stat will simply be ignored for being too low. eg if set to .5 - then anything less than 50% of the max-stat will be ignored.
        //// MinimumPrimary is used for some stats only - and means that at least ONE primary stat has to be above that level, to get score. Eg magic-find has .5 - meaning any item without at least 50% of a max-stat primary, will ignore magic-find scoring.
        ////                                                             Dex  Int  Str  Vit  Life%  LOH  Steal%   LPS Magic% Gold% MSPD Radi  Sox  Crit% CDam% ASPD  Min+  Max+  Block%  Thorn  Allres  Res   DPS  ARMOR   Disc. Mana  Arc. Regen  Globes
        //internal static double[] BonusThreshold = new double[29] { .75, .75, .75, .75, .80, .70, .8, 1, 1, 1, .95, 1, 1, .70, .90, 1, .9, .9, .83, 1, .85, .95, .80, .90, 1, 1, 1, .9, 1.0 };
        //internal static double[] MinimumThreshold = new double[29] { .40, .40, .40, .30, .60, .35, .6, .7, .40, .40, .75, .8, .4, .40, .60, .40, .2, .2, .65, .6, .40, .55, .80, .80, .7, .7, .7, .7, .40 };
        //internal static double[] StatMinimumPrimary = new double[29] { 0, 0, 0, 0, 0, 0, 0, .2, .40, .40, .30, 0, 0, 0, 0, 0, .40, .40, .40, .40, .40, .40, 0, .40, .40, .40, .40, .4, .40 };
    }
}
