using System.Collections.Generic;
using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals.Actors.Gizmos;

namespace Trinity.Components.Adventurer.Game.Actors
{
    public class WaypointData
    {
        public WaypointData(int number, SNOLevelArea levelAreaSnoId, SNOWorld worldSnoId, string name)
        {
            Name = name;
            Number = number;
            WorldSnoId = worldSnoId;
            LevelAreaSnoId = levelAreaSnoId;
        }

        public string Name { get; set; }
        public int Number { get; set; }
        public SNOWorld WorldSnoId { get; set; }
        public SNOLevelArea LevelAreaSnoId { get; set; }
    }

    public static class WaypointFactory
    {
        public static readonly Dictionary<Act, int> ActHubs = new Dictionary<Act, int>
        {
                {Act.A1, 0},
                {Act.A2, 19},
                {Act.A3, 30},
                {Act.A4, 30},
                {Act.A5, 58}
            };

        public static int GetWaypointNumber(SNOLevelArea levelAreaId)
        {
            //return ZetaDia.Memory.CallInjected<int>(new IntPtr(0x0112DBC0),
            //    CallingConvention.Cdecl, levelAreaId);

            var wp = GetWaypointByLevelAreaId(levelAreaId);
            if (wp != null)
                return wp.Number;

            return -1;
        }

        public static WaypointData GetWaypointByLevelAreaId(SNOLevelArea levelAreaId)
        {
            return Waypoints.FirstOrDefault(o => o.Value.LevelAreaSnoId == levelAreaId).Value;
        }

        public static WaypointData GetWaypointByWorldId(SNOWorld worldId)
        {
            return Waypoints.FirstOrDefault(o => o.Value.WorldSnoId == worldId).Value;
        }

        public static Dictionary<int, WaypointData> Waypoints
        {
            get
            {
                if (ZetaDia.Storage.CurrentWorldType == Act.OpenWorld)
                    return OpenWorldWaypoints;

                return CampaignWaypoints;
            }
        }

        public static bool NearWaypoint(int waypointNumber)
        {
            var gizmoWaypoint = ZetaDia.Actors.GetActorsOfType<GizmoWaypoint>().OrderBy(g => g.Distance).FirstOrDefault();
            if (gizmoWaypoint != null && gizmoWaypoint.IsFullyValid())
            {
                if (gizmoWaypoint.WaypointNumber == waypointNumber && gizmoWaypoint.Distance <= 500)
                {
                    return true;
                }
            }
            return false;
        }

        public static readonly Dictionary<int, WaypointData> OpenWorldWaypoints = new Dictionary<int, WaypointData>
        {
            { 0, new WaypointData(0, SNOLevelArea.A1_Tristram_Adventure_Mode_Hub, SNOWorld.X1_Tristram_Adventure_Mode_Hub, "New Tristram") },
            { 1, new WaypointData(1, SNOLevelArea.A1_trOut_Old_Tristram_Road_Cath, SNOWorld.trOUT_Town, "The Old Ruins") },
            { 2, new WaypointData(2, SNOLevelArea.A1_trDun_Level01, SNOWorld.a1trDun_Level01, "Cathedral Level 1") },
            { 3, new WaypointData(3, SNOLevelArea.A1_trDun_Level04, SNOWorld.a1trDun_Level04, "Cathedral Level 2") },
            { 4, new WaypointData(4, SNOLevelArea.A1_trDun_Level06, SNOWorld.a1trDun_Level06, "Cathedral Level 4") },
            { 5, new WaypointData(5, SNOLevelArea.A1_trDun_Level07B, SNOWorld.a1trDun_Level07, "The Royal Crypts") },
            { 6, new WaypointData(6, SNOLevelArea.A1_trOut_TristramWilderness, SNOWorld.trOUT_Town, "The Weeping Hollow") },
            { 7, new WaypointData(7, SNOLevelArea.A1_trOut_Wilderness_BurialGrounds, SNOWorld.trOUT_Town, "Cemetery of the Forsaken") },
            { 8, new WaypointData(8, SNOLevelArea.A1_trOut_TristramFields_A, SNOWorld.trOUT_Town, "Fields of Misery") },
            { 9, new WaypointData(9, SNOLevelArea.A1_trOut_TristramFields_A, SNOWorld.trOUT_Town, "Drowned Temple") },
            { 10, new WaypointData(10, SNOLevelArea.A1_trOut_TristramFields_B, SNOWorld.trOUT_Town, "The Festering Woods") },
            { 11, new WaypointData(11, SNOLevelArea.A1_trOut_TownAttack_ChapelCellar, SNOWorld.trOut_TownAttack_ChapelCellar_A, "Wortham Chapel Cellar") },
            { 12, new WaypointData(12, SNOLevelArea.A1_C6_SpiderCave_01_Main, SNOWorld.a1Dun_SpiderCave_01, "Caverns of Araneae") },
            { 13, new WaypointData(13, SNOLevelArea.A1_trOUT_Highlands_Bridge, SNOWorld.trOUT_Town, "Southern Highlands") },
            { 14, new WaypointData(14, SNOLevelArea.A1_trOUT_Highlands2, SNOWorld.trOUT_Town, "Northern Highlands") },
            { 15, new WaypointData(15, SNOLevelArea.A1_trDun_Leoric01, SNOWorld.trDun_Leoric_Level01, "Halls of Agony Level 1") },
            { 16, new WaypointData(16, SNOLevelArea.A1_trDun_Leoric02, SNOWorld.trDun_Leoric_Level02, "Halls of Agony Level 2") },
            { 17, new WaypointData(17, SNOLevelArea.A1_trDun_Leoric03, SNOWorld.trDun_Leoric_Level03, "Halls of Agony Level 3") },
            { 18, new WaypointData(18, SNOLevelArea.A1_trOUT_LeoricsManor, SNOWorld.trOUT_Town, "Leoric's Manor Courtyard") },
            { 19, new WaypointData(19, SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub, SNOWorld.caOUT_RefugeeCamp, "Hidden Camp") },
            { 20, new WaypointData(20, SNOLevelArea.A2_caOUT_StingingWinds_Canyon, SNOWorld.caOUT_Town, "Howling Plateau") },
            { 21, new WaypointData(21, SNOLevelArea.A2_Caldeum_Uprising, SNOWorld.a2dun_Cald_Uprising, "City of Caldeum") },
            { 22, new WaypointData(22, SNOLevelArea.A2_caOUT_BorderlandsKhamsin, SNOWorld.caOUT_Town, "Stinging Winds") },
            { 23, new WaypointData(23, SNOLevelArea.A2_caOUT_StingingWinds, SNOWorld.caOUT_Town, "Road to Alcarnus") },
            { 24, new WaypointData(24, SNOLevelArea.A2_caOut_Oasis, SNOWorld.caOUT_Town, "Dahlgur Oasis") },
            { 25, new WaypointData(25, SNOLevelArea.A2_caOUT_Boneyard_01, SNOWorld.caOUT_Town, "Desolate Sands") },
            { 26, new WaypointData(26, SNOLevelArea.A2_Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Lobby, "Archives of Zoltun Kulle") },
            { 27, new WaypointData(27, SNOLevelArea.A2_dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_01, "Ancient Waterway") },
            { 28, new WaypointData(28, SNOLevelArea.A2_p6_Church_Level_01, SNOWorld.p6_Church_Level_01, "Temple of the Firstborn Level 1") },
            { 29, new WaypointData(29, SNOLevelArea.A2_p6_Moor_01, SNOWorld.p6_Moor_01, "Shrouded Moors") },
            { 30, new WaypointData(30, SNOLevelArea.A3_Dun_Keep_Hub, SNOWorld.a3dun_hub_keep, "Bastion's Keep Stronghold") },
            { 31, new WaypointData(31, SNOLevelArea.A3_dun_rmpt_Level02, SNOWorld.a3Dun_rmpt_Level02, "Stonefort") },
            { 32, new WaypointData(32, SNOLevelArea.A3_Dun_Keep_Level03, SNOWorld.a3Dun_Keep_Level03, "The Keep Depths Level 1") },
            { 33, new WaypointData(33, SNOLevelArea.A3_Dun_Keep_Level04, SNOWorld.a3Dun_Keep_Level04, "The Keep Depths Level 2") },
            { 34, new WaypointData(34, SNOLevelArea.A3_Dun_Keep_Level05, SNOWorld.a3Dun_Keep_Level05, "The Keep Depths Level 3") },
            { 35, new WaypointData(35, SNOLevelArea.A3_Dun_Battlefield_Gate, SNOWorld.A3_Battlefields_02, "The Battlefields") },
            { 36, new WaypointData(36, SNOLevelArea.A3_Bridge_Choke_A, SNOWorld.A3_Battlefields_02, "The Bridge of Korsikk") },
            { 37, new WaypointData(37, SNOLevelArea.A3_Battlefield_B, SNOWorld.A3_Battlefields_02, "Rakkis Crossing") },
            { 38, new WaypointData(38, SNOLevelArea.A3_Dun_Crater_Level_01, SNOWorld.a3Dun_Crater_Level_01, "Arreat Crater Level 1") },
            { 39, new WaypointData(39, SNOLevelArea.A3_dun_Crater_ST_Level01, SNOWorld.a3dun_Crater_ST_Level01, "Tower of the Damned Level 1") },
            { 40, new WaypointData(40, SNOLevelArea.A3_Dun_Crater_Level_02, SNOWorld.a3Dun_Crater_Level_02, "Arreat Crater Level 2") },
            { 41, new WaypointData(41, SNOLevelArea.A3_dun_Crater_ST_Level01B, SNOWorld.a3dun_Crater_ST_Level01B, "Tower of the Cursed Level 1") },
            { 42, new WaypointData(42, SNOLevelArea.A3_Dun_Crater_Level_03, SNOWorld.a3Dun_Crater_Level_03, "The Core of Arreat") },
            { 43, new WaypointData(43, SNOLevelArea.A3_dun_ruins_frost_city_A_01, SNOWorld.a3dun_ruins_frost_city_A_01, "The Ruins of Sescheron") },
            { 44, new WaypointData(44, SNOLevelArea.A3_Dun_Keep_Hub, SNOWorld.a3dun_hub_keep, "Bastion's Keep Stronghold") },
            { 45, new WaypointData(45, SNOLevelArea.A4_dun_Garden_of_Hope_01, SNOWorld.a4dun_Garden_of_Hope_01, "Gardens of Hope 1st Tier") },
            { 46, new WaypointData(46, SNOLevelArea.A4_dun_Garden_of_Hope_A, SNOWorld.a4dun_Garden_of_Hope_Random_A, "Gardens of Hope 2nd Tier") },
            { 47, new WaypointData(47, SNOLevelArea.A4_dun_Garden_of_Hope_B, SNOWorld.a4dun_Garden_of_Hope_Random_B, "Gardens of Hope 3rd Tier") },
            { 48, new WaypointData(48, SNOLevelArea.A4_dun_Spire_01, SNOWorld.a4dun_Spire_Level_01, "The Silver Spire Level 1") },
            { 49, new WaypointData(49, SNOLevelArea.A4_dun_Spire_02, SNOWorld.a4dun_Spire_Level_02, "The Silver Spire Level 2") },
            { 50, new WaypointData(50, SNOLevelArea.A4_dun_Hell_Portal_01, SNOWorld.a4dun_Hell_Portal_01, "Hell Rift Level 1") },
            { 51, new WaypointData(51, SNOLevelArea.A4_dun_CorruptSpire_SideDungeon_A_Level1, SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1, "Besieged Tower Level 1") },
            { 52, new WaypointData(52, SNOLevelArea.A4_P6_RoF_02_Section_01, SNOWorld.Lost_Souls_Prototype_V2, "Upper Realm of Infernal Fate") },
            { 53, new WaypointData(53, SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_03, SNOWorld.Lost_Souls_Prototype_V3, "Realm of Unbending Fate") },
            { 54, new WaypointData(54, SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_01, SNOWorld.Lost_Souls_Prototype_V4, "Realm of Fractured Fate") },
            { 55, new WaypointData(55, SNOLevelArea.A4_P6_RoF_Labyrinth_01b, SNOWorld.Lost_Souls_Prototype_V5, "Upper Realm of Cursed Fate") },
            { 56, new WaypointData(56, SNOLevelArea.A4_P6_RoF_trDun_Cath_EW_Hall_01b, SNOWorld.Lost_Souls_Prototype_V2, "Lower Realm of Infernal Fate") },
            { 57, new WaypointData(57, SNOLevelArea.A4_P6_RoF_05_Section_01, SNOWorld.Lost_Souls_Prototype_V5, "Lower Realm of Cursed Fate") },
            { 58, new WaypointData(58, SNOLevelArea.x1_Westm_Hub, SNOWorld.X1_Westmarch_Hub, "The Survivors' Enclave") },
            { 59, new WaypointData(59, SNOLevelArea.X1_WESTM_ZONE_01, SNOWorld.X1_WESTM_ZONE_01, "Westmarch Commons") },
            { 60, new WaypointData(60, SNOLevelArea.X1_Westm_Graveyard_DeathOrb, SNOWorld.x1_westm_Graveyard_DeathOrb, "Briarthorn Cemetery") },
            { 61, new WaypointData(61, SNOLevelArea.X1_WESTM_ZONE_03, SNOWorld.X1_WESTM_ZONE_03, "Westmarch Heights") },
            { 62, new WaypointData(62, SNOLevelArea.x1_Bog_01_Part2, SNOWorld.x1_Bog_01, "Paths of the Drowned") },
            { 63, new WaypointData(63, SNOLevelArea.x1_Catacombs_Level01, SNOWorld.x1_Catacombs_Level01, "Passage to Corvus") },
            { 64, new WaypointData(64, SNOLevelArea.x1_Catacombs_Level02, SNOWorld.x1_Catacombs_Level02, "Ruins of Corvus") },
            { 65, new WaypointData(65, SNOLevelArea.x1_fortress_level_01, SNOWorld.x1_fortress_level_01, "Pandemonium Fortress Level 1") },
            { 66, new WaypointData(66, SNOLevelArea.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_Ext_2_Battlefields, "Battlefields of Eternity") },
            { 67, new WaypointData(67, SNOLevelArea.x1_P2_Forest_Coast_Level_01, SNOWorld.x1_p4_Forest_Coast_01, "Greyhollow Island") },
            { 68, new WaypointData(68, SNOLevelArea.x1_fortress_level_02_islands, SNOWorld.x1_fortress_level_02, "Pandemonium Fortress Level 2") },
        };

        public static readonly Dictionary<int, WaypointData> CampaignWaypoints = new Dictionary<int, WaypointData>
        {
        };
    }
}
