using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Coroutines.Town
{
    public class TownInfo
    {
        public static HashSet<ServiceType> BlacksmithServiceTypes = new HashSet<ServiceType>
        {
            ServiceType.ForgeArmor,
            ServiceType.ForgeWeapons,
            ServiceType.Repair,
            ServiceType.Salvage,
        };

        public static HashSet<ServiceType> JewelerServiceTypes = new HashSet<ServiceType>
        {
            ServiceType.GemTraining,
            ServiceType.GemCombine,
            ServiceType.GemRemove,
        };

        public static HashSet<ServiceType> VendorServiceTypes = new HashSet<ServiceType>
        {
            ServiceType.Collector,
            ServiceType.QuarterMaster,
            ServiceType.Fence,
            ServiceType.Miner,
            ServiceType.Peddler,
        };

        public static IEnumerable<TownActor> CurrentAct => TownActors.Where(i => i.LevelAreaId == ZetaDia.CurrentLevelAreaSnoId);

        public static TownActor NearestMerchant => CurrentAct.Where(i => VendorServiceTypes.Contains(i.ServiceType)).OrderBy(i => i.Distance).FirstOrDefault();

        public static TownActor Stash => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Stash);

        public static TownActor Tyrael => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Tyrael);

        public static TownActor RiftObelisk => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.RiftStart);

        public static TownActor Kadala => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Gambling);

        public static TownActor Healer => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Healing);

        public static TownActor KanaisCube => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Transmute);

        public static TownActor BookOfCain => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Identify);

        public static TownActor ZultonKule => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Zoltun);

        public static TownActor Jeweler => CurrentAct.FirstOrDefault(i => JewelerServiceTypes.Contains(i.ServiceType));

        public static TownActor BlacksmithSalvage => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Salvage);

        public static TownActor Blacksmith => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.Repair);

        public static TownActor BlacksmithAny => CurrentAct.FirstOrDefault(i => BlacksmithServiceTypes.Contains(i.ServiceType));

        public static TownActor ReturnPortal => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.ReturnPortal);

        public static TownActor Orek => CurrentAct.FirstOrDefault(i => i.ServiceType == ServiceType.RiftFinish);

        /// <summary>
        /// Positions out in the open that can be used to avoid stucks before pathing to an actor.
        /// </summary>
        public static Vector3 NearestSafeSpot
        {
            get
            {
                var vectors = new List<Vector3>();
                var levelAreaId = ZetaDia.CurrentLevelAreaSnoId;
                switch (levelAreaId)
                {
                    case 19947: // Campaign A1 Hub
                        vectors.Add(new Vector3(2984.255f, 2819.564f, 24.04533f));
                        vectors.Add(new Vector3(2961.75f, 2814.551f, 24.04533f));
                        vectors.Add(new Vector3(2999.527f, 2840.646f, 24.04532f));
                        vectors.Add(new Vector3(2962.378f, 2852.275f, 24.04533f));
                        break;

                    case 332339: // OpenWorld A1 Hub
                        vectors.Add(new Vector3(361.1273f, 520.1647f, 24.04533f));
                        vectors.Add(new Vector3(391.9926f, 528.0787f, 24.04533f));
                        vectors.Add(new Vector3(373.4539f, 561.2624f, 24.01137f));
                        vectors.Add(new Vector3(420.4648f, 566.5033f, 24.04533f));
                        vectors.Add(new Vector3(403.9575f, 525.1089f, 24.04532f));
                        vectors.Add(new Vector3(366.5748f, 568.8141f, 24.04532f));
                        break;

                    case 168314: // A2 Hub
                        vectors.Add(new Vector3(297.4033f, 299.373f, 0.1000002f));
                        vectors.Add(new Vector3(301.3916f, 238.0036f, 0.1f));
                        vectors.Add(new Vector3(342.6914f, 245.5927f, -0.08884031f));
                        vectors.Add(new Vector3(339.4829f, 269.1677f, 0.1000038f));
                        break;

                    case 92945: // A3/A4 Hub
                        vectors.Add(new Vector3(404.9528f, 376.1343f, 0.3325971f));
                        vectors.Add(new Vector3(375.729f, 346.2118f, 0.100001f));
                        vectors.Add(new Vector3(344.1237f, 382.0998f, 0.4986931f));
                        vectors.Add(new Vector3(368.1393f, 409.2282f, 0.4353126f));
                        vectors.Add(new Vector3(350.4086f, 441.0667f, 0.1f));
                        vectors.Add(new Vector3(386.2637f, 474.5234f, 0.1000001f));
                        vectors.Add(new Vector3(401.9803f, 455.5027f, 0.4707586f));
                        vectors.Add(new Vector3(401.9803f, 455.5027f, 0.4707586f));
                        vectors.Add(new Vector3(461.1328f, 437.4017f, 0.4986926f));
                        vectors.Add(new Vector3(432.9738f, 413.0385f, 0.3321429f));
                        vectors.Add(new Vector3(446.3181f, 351.8981f, 0.1000005f));
                        break;

                    case 270011: // A5 Hub
                        vectors.Add(new Vector3(569.0485f, 795.1511f, 2.659489f));
                        vectors.Add(new Vector3(551.3872f, 804.597f, 2.659653f));
                        vectors.Add(new Vector3(597.4126f, 765.3945f, 2.745319f));
                        vectors.Add(new Vector3(586.5446f, 737.8735f, 2.620764f));
                        vectors.Add(new Vector3(547.4657f, 740.6374f, 2.685001f));
                        vectors.Add(new Vector3(514.8192f, 758.8961f, 2.662077f));
                        break;
                }

                if (!vectors.Any())
                    return ZetaDia.Me.Position;

                var orderedVectors = vectors.OrderByDescending(v => v.Distance(ZetaDia.Me.Position));

                return orderedVectors.FirstOrDefault();
            }
        }

        public static Dictionary<GambleSlotTypes, int> MysterySlotTypeAndId = new Dictionary<GambleSlotTypes, int>
        {
            {GambleSlotTypes.OneHanded,377355},
            {GambleSlotTypes.TwoHanded,377356},
            {GambleSlotTypes.Quiver,377360},
            {GambleSlotTypes.Orb,377358},
            {GambleSlotTypes.Mojo,377359},
            {GambleSlotTypes.Helm,377344},
            {GambleSlotTypes.Gloves,377346},
            {GambleSlotTypes.Boots,377347},
            {GambleSlotTypes.Chest,377345},
            {GambleSlotTypes.Belt,377349},
            {GambleSlotTypes.Pants,377350},
            {GambleSlotTypes.Bracers,377351},
            {GambleSlotTypes.Shield,377357},
            {GambleSlotTypes.Ring,377352},
            {GambleSlotTypes.Amulet,377353},
            {GambleSlotTypes.Shoulder,377348}
        };

        public static Dictionary<GambleSlotTypes, int> MysterySlotTypeAndPrice = new Dictionary<GambleSlotTypes, int>
        {
            {GambleSlotTypes.OneHanded,75},
            {GambleSlotTypes.TwoHanded,75},
            {GambleSlotTypes.Quiver,25},
            {GambleSlotTypes.Orb,25},
            {GambleSlotTypes.Mojo,25},
            {GambleSlotTypes.Helm,25},
            {GambleSlotTypes.Gloves,25},
            {GambleSlotTypes.Boots,25},
            {GambleSlotTypes.Chest,25},
            {GambleSlotTypes.Belt,25},
            {GambleSlotTypes.Pants,25},
            {GambleSlotTypes.Bracers,25},
            {GambleSlotTypes.Shield,25},
            {GambleSlotTypes.Ring,50},
            {GambleSlotTypes.Amulet,100},
            {GambleSlotTypes.Shoulder,25}
        };
    }
}