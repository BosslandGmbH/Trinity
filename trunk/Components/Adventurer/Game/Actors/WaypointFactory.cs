using System.Collections.Generic;
using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals.Actors.Gizmos;

namespace Trinity.Components.Adventurer.Game.Actors
{
    public class WaypointData
    {
        public WaypointData(int number, int levelAreaSnoId, int worldSnoId, string name)
        {
            Name = name;
            Number = number;
            WorldSno = (SNOWorld)worldSnoId;
            WorldSnoId = worldSnoId;
            LevelAreaSno = (SNOLevelArea)levelAreaSnoId;
            LevelAreaSnoId = levelAreaSnoId;
        }

        public string Name { get; set; }
        public int Number { get; set; }
        public SNOWorld WorldSno { get; set; }
        public int WorldSnoId { get; set; }
        public SNOLevelArea LevelAreaSno { get; set; }
        public int LevelAreaSnoId { get; set; }
    }

    public static class WaypointFactory
    {
        public static readonly Dictionary<Act, int> ActHubs = new Dictionary<Act, int>
        {
                {Act.A1, 0},
                {Act.A2, 19},
                {Act.A3, 28},
                {Act.A4, 42},
                {Act.A5, 50}
            };

        public static int GetWaypointNumber(int levelAreaId)
        {
            //return ZetaDia.Memory.CallInjected<int>(new IntPtr(0x0112DBC0),
            //    CallingConvention.Cdecl, levelAreaId);

            var wp = GetWaypointByLevelAreaId(levelAreaId);
            if (wp != null)
                return wp.Number;

            return -1;
        }

        public static WaypointData GetWaypointByLevelAreaId(int levelAreaId)
        {
            return Waypoints.FirstOrDefault(o => o.Value.LevelAreaSnoId == levelAreaId).Value;
        }

        public static WaypointData GetWaypointByWorldId(int worldId)
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
            { 0, new WaypointData(0, 332339, 332336, "New Tristram") },
            { 1, new WaypointData(1, 101351, 71150, "The Old Ruins") },
            { 2, new WaypointData(2, 19780, 50579, "Cathedral Level 1") },
            { 3, new WaypointData(3, 19783, 50582, "Cathedral Level 2") },
            { 4, new WaypointData(4, 19785, 50584, "Cathedral Level 4") },
            { 5, new WaypointData(5, 19787, 50585, "The Royal Crypts") },
            { 6, new WaypointData(6, 19954, 71150, "The Weeping Hollow") },
            { 7, new WaypointData(7, 72712, 71150, "Cemetery of the Forsaken") },
            { 8, new WaypointData(8, 19952, 71150, "Fields of Misery") },
            { 9, new WaypointData(9, 19952, 71150, "Drowned Temple") },
            { 10, new WaypointData(10, 19953, 71150, "The Festering Woods") },
            { 11, new WaypointData(11, 119870, 167721, "Wortham Chapel Cellar") },
            { 12, new WaypointData(12, 78572, 180550, "Caverns of Araneae") },
            { 13, new WaypointData(13, 93632, 71150, "Southern Highlands") },
            { 14, new WaypointData(14, 19941, 71150, "Northern Highlands") },
            { 15, new WaypointData(15, 19774, 2826, "Halls of Agony Level 1") },
            { 16, new WaypointData(16, 19775, 58982, "Halls of Agony Level 2") },
            { 17, new WaypointData(17, 19776, 58983, "Halls of Agony Level 3") },
            { 18, new WaypointData(18, 19943, 71150, "Leoric's Manor Courtyard") },
            { 19, new WaypointData(19, 168314, 161472, "Hidden Camp") },
            { 20, new WaypointData(20, 19836, 70885, "Howling Plateau") },
            { 21, new WaypointData(21, 210451, 109894, "City of Caldeum") },
            { 22, new WaypointData(22, 63666, 70885, "Stinging Winds") },
            { 23, new WaypointData(23, 19839, 70885, "Road to Alcarnus") },
            { 24, new WaypointData(24, 57425, 70885, "Dahlgur Oasis") },
            { 25, new WaypointData(25, 53834, 70885, "Desolate Sands") },
            { 26, new WaypointData(26, 19800, 50613, "Archives of Zoltun Kulle") },
            { 27, new WaypointData(27, 62752, 59486, "Ancient Waterway") },
            { 28, new WaypointData(28, 456638, 456634, "Temple of the Firstborn Level 1") },
            { 29, new WaypointData(29, 460671, 460372, "Shrouded Moors") },
            { 30, new WaypointData(30, 92945, 172909, "Bastion's Keep Stronghold") },
            { 31, new WaypointData(31, 93173, 93099, "Stonefort") },
            { 32, new WaypointData(32, 75436, 93104, "The Keep Depths Level 1") },
            { 33, new WaypointData(33, 93103, 75434, "The Keep Depths Level 2") },
            { 34, new WaypointData(34, 136448, 136415, "The Keep Depths Level 3") },
            { 35, new WaypointData(35, 154644, 95804, "The Battlefields") },
            { 36, new WaypointData(36, 155048, 95804, "The Bridge of Korsikk") },
            { 37, new WaypointData(37, 112565, 95804, "Rakkis Crossing") },
            { 38, new WaypointData(38, 86080, 81049, "Arreat Crater Level 1") },
            { 39, new WaypointData(39, 80791, 79401, "Tower of the Damned Level 1") },
            { 40, new WaypointData(40, 119305, 81934, "Arreat Crater Level 2") },
            { 41, new WaypointData(41, 119653, 119641, "Tower of the Cursed Level 1") },
            { 42, new WaypointData(42, 119306, 119290, "The Core of Arreat") },
            { 43, new WaypointData(43, 428494, 428493, "The Ruins of Sescheron") },
            { 44, new WaypointData(44, 92945, 172909, "Bastion's Keep Stronghold") },
            { 45, new WaypointData(45, 109514, 109513, "Gardens of Hope 1st Tier") },
            { 46, new WaypointData(46, 409512, 409510, "Gardens of Hope 2nd Tier") },
            { 47, new WaypointData(47, 409517, 409511, "Gardens of Hope 3rd Tier") },
            { 48, new WaypointData(48, 109538, 121579, "The Silver Spire Level 1") },
            { 49, new WaypointData(49, 109540, 129305, "The Silver Spire Level 2") },
            { 50, new WaypointData(50, 109526, 109525, "Hell Rift Level 1") },
            { 51, new WaypointData(51, 409001, 409000, "Besieged Tower Level 1") },
            { 52, new WaypointData(52, 464063, 456029, "Upper Realm of Infernal Fate") },
            { 53, new WaypointData(53, 464065, 457461, "Realm of Unbending Fate") },
            { 54, new WaypointData(54, 464857, 458965, "Realm of Fractured Fate") },
            { 55, new WaypointData(55, 475856, 460587, "Upper Realm of Cursed Fate") },
            { 56, new WaypointData(56, 475854, 456029, "Lower Realm of Infernal Fate") },
            { 57, new WaypointData(57, 464066, 460587, "Lower Realm of Cursed Fate") },
            { 58, new WaypointData(58, 270011, 304235, "The Survivors' Enclave") },
            { 59, new WaypointData(59, 261758, 261712, "Westmarch Commons") },
            { 60, new WaypointData(60, 338946, 338944, "Briarthorn Cemetery") },
            { 61, new WaypointData(61, 263493, 263494, "Westmarch Heights") },
            { 62, new WaypointData(62, 258142, 267412, "Paths of the Drowned") },
            { 63, new WaypointData(63, 283553, 283552, "Passage to Corvus") },
            { 64, new WaypointData(64, 283567, 283566, "Ruins of Corvus") },
            { 65, new WaypointData(65, 271234, 271233, "Pandemonium Fortress Level 1") },
            { 66, new WaypointData(66, 338602, 338600, "Battlefields of Eternity") },
            { 67, new WaypointData(67, 427763, 408254, "Greyhollow Island") },
            { 68, new WaypointData(68, 459863, 271235, "Pandemonium Fortress Level 2") },
        };

        public static readonly Dictionary<int, WaypointData> CampaignWaypoints = new Dictionary<int, WaypointData>
        {
        };
    }
}