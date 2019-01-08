using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Game.Rift
{
    public static class RiftData
    {
        public const SNOActor RiftStoneSNO = SNOActor.x1_OpenWorld_LootRunObelisk_B;
        public const SNOActor RiftEntryPortalSNO = SNOActor.X1_OpenWorld_LootRunPortal;
        public const SNOActor GreaterRiftEntryPortalSNO = SNOActor.X1_OpenWorld_Tiered_Rifts_Portal;
        public const SNOActor OrekSNO = SNOActor.X1_LR_Nephalem;
        public const SNOActor UrshiSNO = SNOActor.P1_LR_TieredRift_Nephalem;
        public const SNOActor TownstoneSNO = SNOActor.Dungeon_Stone_Portal;
        public const SNOActor HolyCowSNO = SNOActor.TentacleLord;
        
        public const SNOActor GreaterRiftKeySNO = SNOActor.TieredLootrunKey_0;
        public static readonly HashSet<SNOActor> PossibleDungeonStoneSNO = new HashSet<SNOActor>
        {
            SNOActor.Dungeon_Stone_Portal,
            SNOActor.Dungeon_Stone_Portal_invis
        };
        public static readonly HashSet<SNOLevelArea> PossibleHolyCowLevelID = new HashSet<SNOLevelArea>
        {
            SNOLevelArea.X1_LR_Tileset_Crypt,
        };
        public static Vector3 Act1OrekPosition = new Vector3(391, 591, 24);
        public static Vector3 Act1RiftStonePosition = new Vector3(375, 586, 24);

        public static UIElement VendorDialog => UIElement.FromHash(0x244BD04C84DF92F1);

        public static UIElement UpgradeKeystoneButton => UIElement.FromHash(0x4BDE2D63B5C36134);

        public static UIElement UpgradeGemButton => UIElement.FromHash(0x826E5716E8D4DD05);

        public static UIElement ContinueButton => UIElement.FromHash(0x1A089FAFF3CB6576);

        public static UIElement UpgradeButton => UIElement.FromHash(0xD365EA84F587D2FE);

        public static UIElement VendorCloseButton => UIElement.FromHash(0xF98A8466DE237BD5);

        // Cow Rift X1_LR_Level_01 (WorldID: 288454, LevelAreaSnoIdId: 276150)
        // TentacleLord (209133) Distance: 5.491129
        //new InteractWithUnitCoroutine(0, 288454, 209133, 0, 5),
        // 45 secs
        // TentacleLord (209133) Distance: 25.51409
        //new InteractWithUnitCoroutine(0, 288454, 209133, 0, 5),

        public static List<SNOWorld> RiftWorldIds { get; } = new List<SNOWorld>
        {
            SNOWorld.X1_LR_Level_01,
            SNOWorld.X1_LR_Level_02,
            SNOWorld.X1_LR_Level_03,
            SNOWorld.X1_LR_Level_04,
            SNOWorld.X1_LR_Level_05,
            SNOWorld.X1_LR_Level_06,
            SNOWorld.X1_LR_Level_07,
            SNOWorld.X1_LR_Level_08,
            SNOWorld.X1_LR_Level_09,
            SNOWorld.X1_LR_Level_10,
        };

        /// <summary>
        /// Contains all the Exit Name Hashes in Rifts
        /// </summary>
        public static List<int> RiftPortalHashes { get; } = new List<int>
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

        public static Dictionary<SNOWorld, RiftEntryPortal> EntryPortals = new Dictionary<SNOWorld, RiftEntryPortal>();

        public static void AddEntryPortal()
        {
            ZetaDia.Actors.Update();
            var portal =
                ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                    .Where(g => g.IsFullyValid() && g.IsPortal && g.Distance < 100)
                    .OrderBy(g => g.Position.Distance2DSqr(AdvDia.MyPosition))
                    .FirstOrDefault();
            if (portal == null || EntryPortals.ContainsKey(AdvDia.CurrentWorldDynamicId)) return;
            Core.Logger.Debug("[Rift] Added entry portal {0} ({1})", (SNOActor)portal.ActorSnoId, portal.ActorSnoId);
            EntryPortals.Add(AdvDia.CurrentWorldDynamicId,
                new RiftEntryPortal(portal.ActorSnoId, portal.Position.X, portal.Position.Y));

            //var portalMarker =
            //    AdvDia.CurrentWorldMarkers.FirstOrDefault(m => m.Position.Distance(portal.Position) < 10);
            //if (portalMarker != null)
            //{
            //    FileUtils.AppendToLogFile("EntryPortals",
            //        string.Format("{0}\t{1}\t{2}\t{3}", portalMarker.Id, portalMarker.NameHash,
            //            portalMarker.IsPortalEntrance, portalMarker.IsPortalExit));
            //}
        }

        public static bool IsEntryPortal(DiaGizmo portal)
        {
            var sno = portal.ActorSnoId;
            var x = (int)portal.Position.X;
            var y = (int)portal.Position.Y;

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
                var maxLevel = ZetaDia.Me != null ? ZetaDia.Me.HighestUnlockedRiftLevel : 110;
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
            {26, 7500000},
            {27, 7500000},
            {28, 7500000},
            {29, 7500000},
            {30, 7500000},
            {31, 7500000},
            {32, 7500000},
            {33, 7500000},
            {34, 7500000},
            {35, 7500000},
            {36, 12200000},
            {37, 12200000},
            {38, 12200000},
            {39, 12200000},
            {40, 12200000},
            {41, 12200000},
            {42, 12200000},
            {43, 12200000},
            {44, 12200000},
            {45, 12200000},
            {46, 19900000},
            {47, 19900000},
            {48, 19900000},
            {49, 19900000},
            {50, 19900000},
            {51, 19900000},
            {52, 19900000},
            {53, 19900000},
            {54, 19900000},
            {55, 19900000},
            {56, 29400000},
            {57, 29400000},
            {58, 29400000},
            {59, 29400000},
            {60, 29400000},
            {61, 29400000},
            {62, 29400000},
            {63, 29400000},
            {64, 29400000},
            {65, 29400000},
            {66, 30300000},
            {67, 31200000},
            {68, 32100000},
            {69, 33100000},
            {70, 34100000},
            {71, 35100000},
            {72, 36200000},
            {73, 37300000},
            {74, 38400000},
            {75, 39500000},
            {76, 40700000},
            {77, 42000000},
            {78, 43200000},
            {79, 44500000},
            {80, 45800000},
            {81, 47200000},
            {82, 48600000},
            {83, 50100000},
            {84, 51600000},
            {85, 53000000},
            {86, 54000000},
            {87, 56000000},
            {88, 58000000},
            {89, 59000000},
            {90, 61000000},
            {91, 63000000},
            {92, 65000000},
            {93, 67000000},
            {94, 69000000},
            {95, 71000000},
            {96, 73000000},
            {97, 75000000},
            {98, 78000000},
            {99, 80000000},
            {100, 82000000},
            {101, 85000000},
            {102, 87000000},
            {103, 90000000},
            {104, 93000000},
            {105, 96000000},
            {106, 98000000},
            {107, 101000000}, /* this is 107 -Seq */
            {108, 105000000},
            {109, 108000000},
            {110, 111000000},
            {111, 114000000} /* removed extra "," -Seq */
        };
    }

    public class RiftEntryPortal
    {
        public SNOActor ActorSNO { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public RiftEntryPortal(SNOActor actorSno, float x, float y)
        {
            ActorSNO = actorSno;
            X = (int)x;
            Y = (int)y;
        }
    }
}