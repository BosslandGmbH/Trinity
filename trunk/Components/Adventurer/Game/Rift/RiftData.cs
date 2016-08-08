using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.Components.Adventurer.Game.Actors;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Game.Rift
{
    public static class RiftData
    {
        public const int RiftStoneSNO = 364715;
        public const int RiftEntryPortalSNO = 345935;
        public const int GreaterRiftEntryPortalSNO = 396751;
        public const int OrekSNO = 363744;
        public const int UrshiSNO = 398682;
        public const int TownstoneSNO = 135248;
        public const int HolyCowSNO = 209133;
        public const int GreaterRiftKeySNO = 408416;
        public static HashSet<int> DungeonStoneSNOs = new HashSet<int> {135248, 178684};
        public static Vector3 Act1OrekPosition = new Vector3(391, 591, 24);
        public static Vector3 Act1RiftStonePosition = new Vector3(375, 586, 24);


        public static UIElement VendorDialog
        {
            get { return UIElement.FromHash(0x244BD04C84DF92F1); }
        }

        public static UIElement UpgradeKeystoneButton
        {
            get { return UIElement.FromHash(0x4BDE2D63B5C36134); }
        }

        public static UIElement UpgradeGemButton
        {
            get { return UIElement.FromHash(0x826E5716E8D4DD05); }
        }

        public static UIElement ContinueButton
        {
            get { return UIElement.FromHash(0x1A089FAFF3CB6576); }
        }

        public static UIElement UpgradeButton
        {
            get { return UIElement.FromHash(0xD365EA84F587D2FE); }
        }

        public static UIElement VendorCloseButton
        {
            get { return UIElement.FromHash(0xF98A8466DE237BD5); }
        }



        // Cow Rift X1_LR_Level_01 (WorldID: 288454, LevelAreaSnoIdId: 276150)
        // TentacleLord (209133) Distance: 5.491129
        //new InteractWithUnitCoroutine(0, 288454, 209133, 0, 5),
        // 45 secs
        // TentacleLord (209133) Distance: 25.51409
        //new InteractWithUnitCoroutine(0, 288454, 209133, 0, 5),

        public static List<int> RiftWorldIds
        {
            get { return riftWorldIds; }
        }

        private static readonly List<int> riftWorldIds = new List<int>
        {
            288454,
            288685,
            288687,
            288798,
            288800,
            288802,
            288804,
            288810,
            288814,
            288816,
        };

        /// <summary>
        /// Contains all the Exit Name Hashes in Rifts
        /// </summary>
        public static List<int> RiftPortalHashes
        {
            get { return riftPortalHashes; }
        }

        private static readonly List<int> riftPortalHashes = new List<int>
        {
            1938876094,
            1938876095,
            1938876096,
            1938876097,
            1938876098,
            1938876099,
            1938876100,
            1938876101,
            1938876102,
        };

        //Id=4 MinimapTextureSnoId=81058 NameHash=-1944927149 IsPointOfInterest=True IsPortalEntrance = False IsPortalExit=False IsWaypoint = False Location=x="1296" y="941" z="6"  Distance=58
        //[QuestTools][TabUi] Id=10 MinimapTextureSnoId=81058 NameHash=-1944927149 IsPointOfInterest=True IsPortalEntrance=False IsPortalExit=False IsWaypoint=False Location=x="980" y="1088" z="6"  Distance=126
        //ActorId: 353823, Type: Monster, Name: X1_LR_Boss_sniperAngel-33548, Distance2d: 32.87272, CollisionRadius: 0, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 


        public static Dictionary<int, RiftEntryPortal> EntryPortals = new Dictionary<int, RiftEntryPortal>();

        public static void AddEntryPortal()
        {
            ZetaDia.Actors.Update();
            var portal =
                ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                    .Where(g => g.IsFullyValid() && g.IsPortal && g.Distance < 100)
                    .OrderBy(g => g.Position.Distance2DSqr(AdvDia.MyPosition))
                    .FirstOrDefault();
            if (portal == null || EntryPortals.ContainsKey(AdvDia.CurrentWorldDynamicId)) return;
            Util.Logger.Debug("[Rift] Added entry portal {0} ({1})", (SNOActor) portal.ActorSnoId, portal.ActorSnoId);
            EntryPortals.Add(AdvDia.CurrentWorldDynamicId,
                new RiftEntryPortal(portal.ActorSnoId, portal.Position.X, portal.Position.Y));

            var portalMarker =
                AdvDia.CurrentWorldMarkers.FirstOrDefault(m => m.Position.Distance(portal.Position) < 10);
            if (portalMarker != null)
            {
                FileUtils.AppendToLogFile("EntryPortals",
                    string.Format("{0}\t{1}\t{2}\t{3}", portalMarker.Id, portalMarker.NameHash,
                        portalMarker.IsPortalEntrance, portalMarker.IsPortalExit));
            }


        }

        public static bool IsEntryPortal(DiaGizmo portal)
        {
            var sno = portal.ActorSnoId;
            var x = (int) portal.Position.X;
            var y = (int) portal.Position.Y;

            if (!EntryPortals.ContainsKey(AdvDia.CurrentWorldDynamicId))
            {
                return false;
            }

            var worldPortal = EntryPortals[AdvDia.CurrentWorldDynamicId];
            return worldPortal.ActorSNO == sno && worldPortal.X == x && worldPortal.Y == y;
        }

        public static int GetRiftExitPortalHash(int worldId)
        {
            switch (worldId)
            {
                case 288454: //X1_LR_Level_01 = 288454,
                    return 1938876094;
                case 288685: //X1_LR_Level_02 = 288685,
                    return 1938876095;
                case 288687: //X1_LR_Level_03 = 288687,
                    return 1938876096;
                case 288798: //X1_LR_Level_04 = 288798,
                    return 1938876097;
                case 288800: //X1_LR_Level_05 = 288800,
                    return 1938876098;
                case 288802: //X1_LR_Level_06 = 288802,
                    return 1938876099;
                case 288804: //X1_LR_Level_07 = 288804,
                    return 1938876100;
                case 288810: //X1_LR_Level_08 = 288810,
                    return 1938876101;
                case 288814: //X1_LR_Level_09 = 288814,
                    return 1938876102;
                case 288816: //X1_LR_Level_10 = 288816,
                    return int.MinValue;
            }
            return int.MinValue;
        }

        public static int GetGreaterRiftLevel()
        {
            var greaterRiftLevel = PluginSettings.Current.GreaterRiftLevel;
            if (greaterRiftLevel <= 0)
            {
                var maxLevel = PluginSettings.Current.HighestUnlockedRiftLevel;
                greaterRiftLevel = maxLevel + greaterRiftLevel;
            }
            return greaterRiftLevel;
        }

        public static Dictionary<int, long> EmpoweredRiftCost = new Dictionary<int, long>
        {
            {1, 159000},
            {2, 183000},
            {3, 211000},
            {4, 242000},
            {5, 279000},
            {6, 321000},
            {7, 369000},
            {8, 425000},
            {9, 489000},
            {10, 560000},
            {11, 640000},
            {12, 740000},
            {13, 850000},
            {14, 980000},
            {15, 1130000},
            {16, 1300000},
            {17, 1500000},
            {18, 1720000},
            {19, 1980000},
            {20, 2280000},
            {21, 2630000},
            {22, 3020000},
            {23, 3480000},
            {24, 4000000},
            {25, 4460000},
            {26, 5060000},
            {27, 5500000},
            {28, 6100000},
            {29, 6700000},
            {30, 7400000},
            {31, 8100000},
            {32, 8900000},
            {33, 9800000},
            {34, 10800000},
            {35, 11900000},
            {36, 13100000},
            {37, 14400000},
            {38, 15900000},
            {39, 17400000},
            {40, 19200000},
            {41, 21100000},
            {42, 23200000},
            {43, 25600000},
            {44, 28100000},
            {45, 30900000},
            {46, 34000000},
            {47, 37500000},
            {48, 41200000},
            {49, 45300000},
            {50, 49900000},
            {51, 54000000},
            {52, 60000000},
            {53, 66000000},
            {54, 73000000},
            {55, 80000000},
            {56, 88000000},
            {57, 97000000},
            {58, 106000000},
            {59, 117000000},
            {60, 129000000},
            {61, 142000000},
            {62, 156000000},
            {63, 172000000},
            {64, 189000000},
            {65, 208000000},
            {66, 229000000},
            {67, 252000000},
            {68, 1000000000},
            {69, 1000000000},
            {70, 1000000000},
            {71, 1000000000},
            {72, 1000000000},
            {73, 1000000000},
            {74, 2000000000},
            {75, 2000000000},
            {76, 2000000000},
            {77, 2000000000},
            {78, 2000000000},
            {79, 4000000000},
            {80, 4000000000},
            {81, 4000000000},
            {82, 4000000000},
            {83, 4000000000},
            {84, 4000000000},
            {85, 4000000000},
            {86, 5000000000},
            {87, 5000000000},
            {88, 5000000000},
            {89, 5000000000},
            {90, 5000000000},
            {91, 10000000000},
            {92, 10000000000},
            {93, 10000000000},
            {94, 10000000000},
        };

    }

    public class RiftEntryPortal
    {
        public int ActorSNO { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public RiftEntryPortal(int actorSno, float x, float y)
        {
            ActorSNO = actorSno;
            X = (int)x;
            Y = (int)y;
        }
    }


}
