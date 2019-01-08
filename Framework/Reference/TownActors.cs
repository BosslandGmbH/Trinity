using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Helpers;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Framework.Reference
{
    public class TownActors : FieldCollection<TownActors, TownActor>
    {
        public static TownActor A5HearthPortal = new TownActor
        {
            Name = "A5 HearthPortal",
            InternalName = SNOActor.hearthPortal.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.hearthPortal,
            InteractPosition = new Vector3(546.3218f, 753.0034f, 2.706141f),
            Position = new Vector3(545.243f, 753.3441f, 3.20088f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ReturnPortal,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub,
        };

        public static TownActor A5BlacksmithSalvage = new TownActor
        {
            Name = "A5 Blacksmith Salvage",
            InternalName = SNOActor.PT_Blacksmith_RepairShortcut.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.PT_Blacksmith_RepairShortcut,
            InteractPosition = new Vector3(539.8195f, 707.8679f, 2.620764f),
            Position = new Vector3(533.1066f, 713.524f, 2.52075f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Salvage,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub,
        };

        public static TownActor A5KanaiCube = new TownActor
        {
            Name = "A5 Kanai's Cube",
            InternalName = SNOActor.KanaiCube_Stand.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.KanaiCube_Stand,
            InteractPosition = new Vector3(573.6256f, 806.0084f, 2.620763f),
            Position = new Vector3(576.6396f, 810.533f, 2.52077f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Transmute,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5ZoltunKulle = new TownActor
        {
            Name = "A5 Zoltun Kulle",
            InternalName = SNOActor.p2_HQ_ZoltunKulle.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.p2_HQ_ZoltunKulle,
            InteractPosition = new Vector3(566.0811f, 810.5207f, 2.659653f),
            Position = new Vector3(570.3144f, 813.2427f, 2.5863f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Zoltun,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Mystic = new TownActor
        {
            Name = "A5 Mystic",
            InternalName = SNOActor.PT_Mystic.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.PT_Mystic,
            InteractPosition = new Vector3(532.0173f, 803.4391f, 2.620764f),
            Position = new Vector3(531.2024f, 806.9097f, 2.52075f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Enchanting,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5QuaterMaster = new TownActor
        {
            Name = "A5 QuaterMaster",
            InternalName = SNOActor.X1_A5_UniqueVendor_InnKeeper.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.X1_A5_UniqueVendor_InnKeeper,
            InteractPosition = new Vector3(546.7432f, 775.9437f, 2.782241f),
            Position = new Vector3(541.8223f, 777.1297f, 3.11829f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.QuarterMaster,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Stash = new TownActor
        {
            Name = "A5 Stash",
            InternalName = SNOActor.Player_Shared_Stash.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.Player_Shared_Stash,
            InteractPosition = new Vector3(506.2032f, 741.4248f, 2.620764f),
            Position = new Vector3(502.8296f, 739.7472f, 2.59863f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Stash,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5BookOfCain = new TownActor
        {
            Name = "A5 Book of Cain",
            InternalName = SNOActor.a5_Id_All_Book_Of_Cain_B.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.a5_Id_All_Book_Of_Cain_B,
            InteractPosition = new Vector3(499.2372f, 753.682f, 2.640297f),
            Position = new Vector3(493.0586f, 751.1307f, 2.84739f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Identify,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Healer = new TownActor
        {
            Name = "A5 Healer",
            InternalName = SNOActor.X1_A5_WestmHub_Healer.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.X1_A5_WestmHub_Healer,
            InteractPosition = new Vector3(423.4814f, 752.5201f, 7.415271f),
            Position = new Vector3(417.8354f, 747.932f, 6.552297f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Healing,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Miner = new TownActor
        {
            Name = "A5 Miner",
            InternalName = SNOActor.X1_A5_UniqueVendor_Miner.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.X1_A5_UniqueVendor_Miner,
            InteractPosition = new Vector3(424.886f, 820.4818f, 7.406786f),
            Position = new Vector3(422.2853f, 821.3752f, 7.37405f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Miner,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5BlacksmithForgeWeapons = new TownActor
        {
            Name = "A5 Blacksmith Forge Weapons",
            InternalName = SNOActor.PT_Blacksmith_ForgeWeaponShortcut.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.PT_Blacksmith_ForgeWeaponShortcut,
            InteractPosition = new Vector3(535.4509f, 733.5281f, 2.620764f),
            Position = new Vector3(530.8178f, 725.8815f, 2.52075f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ForgeWeapons,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        //public static TownActor A5Blacksmith = new TownActor
        //{
        //    Name = "A5 Blacksmith",
        //    InternalName = SNOActor.PT_Blacksmith.ToString(),
        //    Act = Act.A5,
        //    ActorId = SNOActor.PT_Blacksmith,
        //    InteractPosition = new Vector3(545.5338f, 717.6472f, 2.634673f),
        //    Position = new Vector3(541.959f, 716.487f, 2.52075f),
        //    IsAdventurerMode = true,
        //    ServiceType = ServiceType.Repair,
        //    IsGizmo = false,
        //    IsUnit = true,
        //    WorldSnoId = SNOWorld.X1_Westmarch_Hub,
        //    LevelAreaId = SNOLevelArea.x1_Westm_Hub
        //};

        public static TownActor A5Tyrael = new TownActor
        {
            Name = "A5 Tyrael",
            InternalName = SNOActor.Tyrael_Heaven.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.Tyrael_Heaven,
            InteractPosition = new Vector3(568.2975f, 744.4036f, 2.665126f),
            Position = new Vector3(572.043f, 738.9701f, 2.52087f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Tyrael,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5NephalemObelisk = new TownActor
        {
            Name = "A5 Nephalem Obelisk",
            InternalName = SNOActor.x1_OpenWorld_LootRunObelisk_B.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.x1_OpenWorld_LootRunObelisk_B,
            InteractPosition = new Vector3(600.1259f, 749.2164f, 2.620764f),
            Position = new Vector3(606.84f, 750.39f, 2.53f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftStart,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Kadala = new TownActor
        {
            Name = "A5 Kadala",
            InternalName = SNOActor.X1_RandomItemNPC.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.X1_RandomItemNPC,
            InteractPosition = new Vector3(606.0291f, 772.7044f, 2.745291f),
            Position = new Vector3(609.95f, 775.33f, 2.62f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Gambling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Orek = new TownActor
        {
            Name = "A5 Orek",
            InternalName = SNOActor.X1_LR_Nephalem.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.X1_LR_Nephalem,
            InteractPosition = new Vector3(587.8287f, 757.9866f, 2.745319f),
            Position = new Vector3(589.13f, 762.11f, 2.75f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftFinish,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Jeweler = new TownActor
        {
            Name = "A5 Jeweler",
            InternalName = SNOActor.Start_Location_0.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.Start_Location_0,
            InteractPosition = new Vector3(610.7127f, 727.1551f, 2.620764f),
            Position = new Vector3(607.704f, 727.27f, 2.62076f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemTraining,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Fence = new TownActor
        {
            Name = "A5 Fence",
            InternalName = SNOActor.X1_A5_UniqueVendor_Fence.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.X1_A5_UniqueVendor_Fence,
            InteractPosition = new Vector3(618.9899f, 793.6663f, 2.620764f),
            Position = new Vector3(624.09f, 796.71f, 2.52075f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Fence,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Collector = new TownActor
        {
            Name = "A5 Collector",
            InternalName = SNOActor.X1_A5_UniqueVendor_Collector.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.X1_A5_UniqueVendor_Collector,
            InteractPosition = new Vector3(626.1296f, 817.1263f, 2.620764f),
            Position = new Vector3(628.941f, 817.6201f, 2.52077f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Collector,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        public static TownActor A5Waypoint = new TownActor
        {
            Name = "A5 Waypoint",
            InternalName = SNOActor.Waypoint.ToString(),
            Act = Act.A5,
            ActorId = SNOActor.Waypoint,
            InteractPosition = new Vector3(558.4846f, 766.1548f, 2.796464f),
            Position = new Vector3(557.5455f, 765.3394f, 2.69647f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Waypoint,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Westmarch_Hub,
            LevelAreaId = SNOLevelArea.x1_Westm_Hub
        };

        // A4 Campaign

        public static TownActor A4CampaignHearthPortal = new TownActor
        {
            Name = "A4 HearthPortal",
            InternalName = SNOActor.hearthPortal.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.hearthPortal,
            InteractPosition = new Vector3(373.9471f, 414.2664f, 0.3321428f),
            Position = new Vector3(374.071f, 417.005f, 0.614487f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.ReturnPortal,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignBookOfCain = new TownActor
        {
            Name = "A4 Book of Cain",
            InternalName = SNOActor.a3_Id_All_Book_Of_Cain.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.a3_Id_All_Book_Of_Cain,
            InteractPosition = new Vector3(378.305f, 388.9206f, 0.4707598f),
            Position = new Vector3(373.871f, 388.59f, -0.306229f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Identify,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignStash = new TownActor
        {
            Name = "A4 Stash",
            InternalName = SNOActor.Player_Shared_Stash.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.Player_Shared_Stash,
            InteractPosition = new Vector3(388.1244f, 385.259f, 0.332143f),
            Position = new Vector3(387.683f, 382.029f, 0f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Stash,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignTownWaypoint = new TownActor
        {
            Name = "A4 Waypoint",
            InternalName = SNOActor.Waypoint.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.Waypoint,
            InteractPosition = new Vector3(401.6938f, 414.5268f, 1.5338f),
            Position = new Vector3(402.54f, 414.342f, 0.7017f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Waypoint,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignMiner = new TownActor
        {
            Name = "A4 The Miner",
            InternalName = SNOActor.A4_UniqueVendor_Miner_InTown_01.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.A4_UniqueVendor_Miner_InTown_01,
            InteractPosition = new Vector3(391.9044f, 513.2457f, 0.1f),
            Position = new Vector3(388.982f, 519.2976f, 0.948341f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Miner,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignBlacksmithSalvage = new TownActor
        {
            Name = "A4 Blacksmith Salvage",
            InternalName = SNOActor.PT_Blacksmith_RepairShortcut.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.PT_Blacksmith_RepairShortcut,
            InteractPosition = new Vector3(328.9132f, 424.1724f, 0.3253784f),
            Position = new Vector3(320.9509f, 423.937f, -0.11f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Salvage,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignFence = new TownActor
        {
            Name = "A4 The Fence",
            InternalName = SNOActor.A4_UniqueVendor_Fence_InTown_01.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.A4_UniqueVendor_Fence_InTown_01,
            InteractPosition = new Vector3(442.6061f, 517.278f, 0.1000001f),
            Position = new Vector3(440.712f, 518.9634f, -0.0501633f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Fence,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignCollector = new TownActor
        {
            Name = "A4 The Collector",
            InternalName = SNOActor.A3_UniqueVendor_Collector_InTown_01.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.A3_UniqueVendor_Collector_InTown_01,
            InteractPosition = new Vector3(441.8826f, 322.1977f, 0.1000005f),
            Position = new Vector3(444.577f, 318.0379f, 0f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Collector,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        public static TownActor A4CampaignJeweler = new TownActor
        {
            Name = "A4 Jeweler",
            InternalName = SNOActor.PT_Jeweler.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.PT_Jeweler,
            InteractPosition = new Vector3(406.0522f, 320.8097f, 0.1000005f),
            Position = new Vector3(404.894f, 317.7354f, 0.0188384f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemTraining,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a4dun_heaven_hub_keep,
            LevelAreaId = SNOLevelArea.A4_Dun_Keep_Hub,
        };

        // A4

        public static TownActor A4HearthPortal = new TownActor
        {
            Name = "A3 HearthPortal",
            InternalName = SNOActor.hearthPortal.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.hearthPortal,
            InteractPosition = new Vector3(373.9471f, 414.2664f, 0.3321428f),
            Position = new Vector3(374.071f, 417.005f, 0.614487f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ReturnPortal,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub,
        };

        public static TownActor A4BookOfCain = new TownActor
        {
            Name = "A4 Book of Cain",
            InternalName = SNOActor.a3_Id_All_Book_Of_Cain.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.a3_Id_All_Book_Of_Cain,
            InteractPosition = new Vector3(378.305f, 388.9206f, 0.4707598f),
            Position = new Vector3(373.871f, 388.59f, -0.306229f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Identify,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Stash = new TownActor
        {
            Name = "A4 Stash",
            InternalName = SNOActor.Player_Shared_Stash.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.Player_Shared_Stash,
            InteractPosition = new Vector3(388.1244f, 385.259f, 0.332143f),
            Position = new Vector3(387.683f, 382.029f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Stash,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Tyrael = new TownActor
        {
            Name = "A4 Tyrael",
            InternalName = SNOActor.Tyrael_Heaven.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.Tyrael_Heaven,
            InteractPosition = new Vector3(379.3534f, 419.4077f, 0.3321424f),
            Position = new Vector3(378.94f, 422.09f, 0.47f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Tyrael,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4TownWaypoint = new TownActor
        {
            Name = "A4 Waypoint",
            InternalName = SNOActor.Waypoint.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.Waypoint,
            InteractPosition = new Vector3(401.6938f, 414.5268f, 1.5338f),
            Position = new Vector3(402.54f, 414.342f, 0.7017f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Waypoint,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4KanaiCube = new TownActor
        {
            Name = "A4 Kanai's Cube",
            InternalName = SNOActor.KanaiCube_Stand.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.KanaiCube_Stand,
            InteractPosition = new Vector3(430.4615f, 494.1607f, 0.2338486f),
            Position = new Vector3(429.265f, 498.6068f, 0.0613022f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Transmute,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4ZoltunKulle = new TownActor
        {
            Name = "A4 Zoltun Kulle",
            InternalName = SNOActor.p2_HQ_ZoltunKulle.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.p2_HQ_ZoltunKulle,
            InteractPosition = new Vector3(418.2447f, 494.3743f, 0.199793f),
            Position = new Vector3(418.616f, 498.2578f, -3.61718f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Zoltun,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Miner = new TownActor
        {
            Name = "A4 The Miner",
            InternalName = SNOActor.A4_UniqueVendor_Miner_InTown_01.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.A4_UniqueVendor_Miner_InTown_01,
            InteractPosition = new Vector3(391.9044f, 513.2457f, 0.1f),
            Position = new Vector3(388.982f, 519.2976f, 0.948341f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Miner,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Fence = new TownActor
        {
            Name = "A4 The Fence",
            InternalName = SNOActor.A4_UniqueVendor_Fence_InTown_01.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.A4_UniqueVendor_Fence_InTown_01,
            InteractPosition = new Vector3(442.6061f, 517.278f, 0.1000001f),
            Position = new Vector3(440.712f, 518.9634f, -0.0501633f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Fence,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Orek = new TownActor
        {
            Name = "A4 Orek",
            InternalName = SNOActor.X1_LR_Nephalem.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.X1_LR_Nephalem,
            InteractPosition = new Vector3(451.2573f, 404.9175f, 0.1000005f),
            Position = new Vector3(453.18f, 401.51f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftFinish,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4NephalemObelisk = new TownActor
        {
            Name = "A4 Nephalem Obelisk",
            InternalName = SNOActor.x1_OpenWorld_LootRunObelisk_B.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.x1_OpenWorld_LootRunObelisk_B,
            InteractPosition = new Vector3(463.455f, 386.0683f, 0.4986931f),
            Position = new Vector3(467.59f, 387.45f, 0.78f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftStart,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Kadala = new TownActor
        {
            Name = "A4 Kadala",
            InternalName = SNOActor.X1_RandomItemNPC.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.X1_RandomItemNPC,
            InteractPosition = new Vector3(472.0305f, 411.4961f, 0.4986931f),
            Position = new Vector3(475.989f, 414.056f, 0.498693f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Gambling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Collector = new TownActor
        {
            Name = "A4 The Collector",
            InternalName = SNOActor.A3_UniqueVendor_Collector_InTown_01.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.A3_UniqueVendor_Collector_InTown_01,
            InteractPosition = new Vector3(441.8826f, 322.1977f, 0.1000005f),
            Position = new Vector3(444.577f, 318.0379f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Collector,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Jeweler = new TownActor
        {
            Name = "A4 Jeweler",
            InternalName = SNOActor.PT_Jeweler.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.PT_Jeweler,
            InteractPosition = new Vector3(406.0522f, 320.8097f, 0.1000005f),
            Position = new Vector3(404.894f, 317.7354f, 0.0188384f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemTraining,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A4Mystic = new TownActor
        {
            Name = "PT_Mystic",
            InternalName = SNOActor.PT_Mystic.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.PT_Mystic,
            InteractPosition = new Vector3(313.8778f, 307.9454f, 0.1000005f),
            Position = new Vector3(310.1808f, 307.4399f, 6.10352E-05f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Enchanting,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        // Cannot repair at blacksmith in campaign mode he just has talk options.
        //public static TownActor A4Blacksmith = new TownActor
        //{
        //    Name = "A4 Blacksmith",
        //    InternalName = SNOActor.PT_Blacksmith.ToString(),
        //    Act = Act.A4,
        //    ActorId = SNOActor.PT_Blacksmith,
        //    InteractPosition = new Vector3(329.887f, 415.2654f, 0.4806792f),
        //    Position = new Vector3(326.6459f, 414.904f, -0.0621901f),
        //    IsAdventurerMode = true,
        //    ServiceType = ServiceType.Repair,
        //    IsGizmo = false,
        //    IsUnit = true,
        //    WorldSnoId = SNOWorld.a3dun_hub_keep,
        //    LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        //};

        public static TownActor A4BlacksmithSalvage = new TownActor
        {
            Name = "A4 Blacksmith Salvage",
            InternalName = SNOActor.PT_Blacksmith_RepairShortcut.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.PT_Blacksmith_RepairShortcut,
            InteractPosition = new Vector3(328.9132f, 424.1724f, 0.3253784f),
            Position = new Vector3(320.9509f, 423.937f, -0.11f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Salvage,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        //      <!-- A3_UniqueVendor_InnKeeper-529 (181473) A3_UniqueVendor_InnKeeper Distance = 6.338559 Type=Unit Anim = OmniNPC_Male_HTH_town_idle_01-- >
        //< MoveToActor questId="93595" stepId="8" actorId="181473" markerHash="0" explore="false" maxRange="300" startAnimation="OmniNPC_Male_HTH_town_idle_01" x="308.35" y="499.54" z="-5.78" sceneX="68.35" sceneY="19.54" sceneZ="-5.78" sceneSnoId="172872" sceneName="a3dun_vendors_A_E02_S02" distance="5" useNavigation="true"  worldSnoId="SNOWorld.a3dun_hub_keep" levelAreaSnoId="SNOLevelArea.A3_Dun_Keep_Hub" />
        // Different in campaign mode...
        //

        //public static TownActor A4QuaterMaster = new TownActor
        //{
        //    Name = "A4 QuarterMaster",
        //    InternalName = SNOActor.A4_UniqueVendor_InnKeeper_08.ToString(),
        //    Act = Act.A4,
        //    ActorId = SNOActor.A4_UniqueVendor_InnKeeper_08,
        //    InteractPosition = new Vector3(311.422f, 496.7398f, -5.678425f),
        //    Position = new Vector3(308.39f, 499.479f, -5.77843f),
        //    IsAdventurerMode = true,
        //    ServiceType = ServiceType.QuarterMaster,
        //    IsGizmo = false,
        //    IsUnit = true,
        //    WorldSnoId = SNOWorld.a3dun_hub_keep,
        //    LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        //};

        public static TownActor A4Healer = new TownActor
        {
            Name = "Priest_BastionsKeep_Healer",
            InternalName = SNOActor.Priest_BastionsKeep_Healer.ToString(),
            Act = Act.A4,
            ActorId = SNOActor.Priest_BastionsKeep_Healer,
            InteractPosition = new Vector3(310.1947f, 510.1656f, -5.678425f),
            Position = new Vector3(307.1355f, 509.5115f, -5.77843f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Healing,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        // A3

        public static TownActor A3HearthPortal = new TownActor
        {
            Name = "A3 HearthPortal",
            InternalName = SNOActor.hearthPortal.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.hearthPortal,
            InteractPosition = new Vector3(373.9471f, 414.2664f, 0.3321428f),
            Position = new Vector3(374.071f, 417.005f, 0.614487f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ReturnPortal,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub,
        };


        public static TownActor A3BookOfCain = new TownActor
        {
            Name = "A3 Book of Cain",
            InternalName = SNOActor.a3_Id_All_Book_Of_Cain.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.a3_Id_All_Book_Of_Cain,
            InteractPosition = new Vector3(378.305f, 388.9206f, 0.4707598f),
            Position = new Vector3(373.871f, 388.59f, -0.306229f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Identify,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Stash = new TownActor
        {
            Name = "A3 Stash",
            InternalName = SNOActor.Player_Shared_Stash.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.Player_Shared_Stash,
            InteractPosition = new Vector3(388.1244f, 385.259f, 0.332143f),
            Position = new Vector3(387.683f, 382.029f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Stash,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Tyrael = new TownActor
        {
            Name = "A3 Tyrael",
            InternalName = SNOActor.Tyrael_Heaven.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.Tyrael_Heaven,
            InteractPosition = new Vector3(379.3534f, 419.4077f, 0.3321424f),
            Position = new Vector3(378.94f, 422.09f, 0.47f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Tyrael,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3TownWaypoint = new TownActor
        {
            Name = "A3 Waypoint",
            InternalName = SNOActor.Waypoint.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.Waypoint,
            InteractPosition = new Vector3(401.6938f, 414.5268f, 1.5338f),
            Position = new Vector3(402.54f, 414.342f, 0.7017f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Waypoint,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3KanaiCube = new TownActor
        {
            Name = "A3 Kanai's Cube",
            InternalName = SNOActor.KanaiCube_Stand.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.KanaiCube_Stand,
            InteractPosition = new Vector3(430.4615f, 494.1607f, 0.2338486f),
            Position = new Vector3(429.265f, 498.6068f, 0.0613022f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Transmute,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3ZoltunKulle = new TownActor
        {
            Name = "A3 Zoltun Kulle",
            InternalName = SNOActor.p2_HQ_ZoltunKulle.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.p2_HQ_ZoltunKulle,
            InteractPosition = new Vector3(418.2447f, 494.3743f, 0.199793f),
            Position = new Vector3(418.616f, 498.2578f, -3.61718f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Zoltun,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Miner = new TownActor
        {
            Name = "A3 The Miner",
            InternalName = SNOActor.A4_UniqueVendor_Miner_InTown_01.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.A4_UniqueVendor_Miner_InTown_01,
            InteractPosition = new Vector3(391.9044f, 513.2457f, 0.1f),
            Position = new Vector3(388.982f, 519.2976f, 0.948341f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Miner,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Fence = new TownActor
        {
            Name = "A3 The Fence",
            InternalName = SNOActor.A4_UniqueVendor_Fence_InTown_01.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.A4_UniqueVendor_Fence_InTown_01,
            InteractPosition = new Vector3(442.6061f, 517.278f, 0.1000001f),
            Position = new Vector3(440.712f, 518.9634f, -0.0501633f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Fence,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Orek = new TownActor
        {
            Name = "A3 Orek",
            InternalName = SNOActor.X1_LR_Nephalem.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.X1_LR_Nephalem,
            InteractPosition = new Vector3(451.2573f, 404.9175f, 0.1000005f),
            Position = new Vector3(453.18f, 401.51f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftFinish,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3NephalemObelisk = new TownActor
        {
            Name = "A3 Nephalem Obelisk",
            InternalName = SNOActor.x1_OpenWorld_LootRunObelisk_B.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.x1_OpenWorld_LootRunObelisk_B,
            InteractPosition = new Vector3(463.455f, 386.0683f, 0.4986931f),
            Position = new Vector3(467.59f, 387.45f, 0.78f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftStart,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Kadala = new TownActor
        {
            Name = "A3 Kadala",
            InternalName = SNOActor.X1_RandomItemNPC.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.X1_RandomItemNPC,
            InteractPosition = new Vector3(472.0305f, 411.4961f, 0.4986931f),
            Position = new Vector3(475.989f, 414.056f, 0.498693f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Gambling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Collector = new TownActor
        {
            Name = "A3 The Collector",
            InternalName = SNOActor.A3_UniqueVendor_Collector_InTown_01.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.A3_UniqueVendor_Collector_InTown_01,
            InteractPosition = new Vector3(441.8826f, 322.1977f, 0.1000005f),
            Position = new Vector3(444.577f, 318.0379f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Collector,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Jeweler = new TownActor
        {
            Name = "A3 Jeweler",
            InternalName = SNOActor.PT_Jeweler.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.PT_Jeweler,
            InteractPosition = new Vector3(406.0522f, 320.8097f, 0.1000005f),
            Position = new Vector3(404.894f, 317.7354f, 0.0188384f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemTraining,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        public static TownActor A3Mystic = new TownActor
        {
            Name = "PT_Mystic",
            InternalName = SNOActor.PT_Mystic.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.PT_Mystic,
            InteractPosition = new Vector3(313.8778f, 307.9454f, 0.1000005f),
            Position = new Vector3(310.1808f, 307.4399f, 6.10352E-05f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Enchanting,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        //public static TownActor A3Blacksmith = new TownActor
        //{
        //    Name = "A3 Blacksmith",
        //    InternalName = SNOActor.PT_Blacksmith.ToString(),
        //    Act = Act.A3,
        //    ActorId = SNOActor.PT_Blacksmith,
        //    InteractPosition = new Vector3(329.887f, 415.2654f, 0.4806792f),
        //    Position = new Vector3(326.6459f, 414.904f, -0.0621901f),
        //    IsAdventurerMode = true,
        //    ServiceType = ServiceType.Repair,
        //    IsGizmo = false,
        //    IsUnit = true,
        //    WorldSnoId = SNOWorld.a3dun_hub_keep,
        //    LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        //};

        public static TownActor A3BlacksmithSalvage = new TownActor
        {
            Name = "A3 Blacksmith Salvage",
            InternalName = SNOActor.PT_Blacksmith_RepairShortcut.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.PT_Blacksmith_RepairShortcut,
            InteractPosition = new Vector3(328.9132f, 424.1724f, 0.3253784f),
            Position = new Vector3(320.9509f, 423.937f, -0.11f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Salvage,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        // diff actor in campaign mode
        //public static TownActor A3QuaterMaster = new TownActor
        //{
        //    Name = "A3 QuarterMaster",
        //    InternalName = SNOActor.A4_UniqueVendor_InnKeeper_08.ToString(),
        //    Act = Act.A3,
        //    ActorId = SNOActor.A4_UniqueVendor_InnKeeper_08,
        //    InteractPosition = new Vector3(311.422f, 496.7398f, -5.678425f),
        //    Position = new Vector3(308.39f, 499.479f, -5.77843f),
        //    IsAdventurerMode = true,
        //    ServiceType = ServiceType.QuarterMaster,
        //    IsGizmo = false,
        //    IsUnit = true,
        //    WorldSnoId = SNOWorld.a3dun_hub_keep,
        //    LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        //};

        public static TownActor A3Healer = new TownActor
        {
            Name = "Priest_BastionsKeep_Healer",
            InternalName = SNOActor.Priest_BastionsKeep_Healer.ToString(),
            Act = Act.A3,
            ActorId = SNOActor.Priest_BastionsKeep_Healer,
            InteractPosition = new Vector3(310.1947f, 510.1656f, -5.678425f),
            Position = new Vector3(307.1355f, 509.5115f, -5.77843f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Healing,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.a3dun_hub_keep,
            LevelAreaId = SNOLevelArea.A3_Dun_Keep_Hub
        };

        // A2

        public static TownActor A2HearthPortal = new TownActor
        {
            Name = "A2 HearthPortal",
            InternalName = SNOActor.hearthPortal.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.hearthPortal,
            InteractPosition = new Vector3(309.7593f, 272.6019f, 0.09999999f),
            Position = new Vector3(308.2124f, 273.393f, -1.14215f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ReturnPortal,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub,
        };

        public static TownActor A2Waypoint = new TownActor
        {
            Name = "A2 Waypoint",
            InternalName = SNOActor.Waypoint.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.Waypoint,
            InteractPosition = new Vector3(324.8699f, 291.0312f, 1.645629f),
            Position = new Vector3(324.8699f, 291.0312f, 0.753804f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Waypoint,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Kadala = new TownActor
        {
            Name = "A2 Kadala",
            InternalName = SNOActor.X1_RandomItemNPC.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.X1_RandomItemNPC,
            InteractPosition = new Vector3(338.0048f, 257.9352f, -0.07518402f),
            Position = new Vector3(340.88f, 258.63f, -0.12f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Gambling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2NephalemObelisk = new TownActor
        {
            Name = "A2 Nephalem Obelisk",
            InternalName = SNOActor.x1_OpenWorld_LootRunObelisk_B.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.x1_OpenWorld_LootRunObelisk_B,
            InteractPosition = new Vector3(355.4316f, 262.7514f, -0.3242287f),
            Position = new Vector3(359.9f, 262.766f, -0.0996094f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftStart,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Stash = new TownActor
        {
            Name = "A2 Stash",
            InternalName = SNOActor.Player_Shared_Stash.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.Player_Shared_Stash,
            InteractPosition = new Vector3(323.8755f, 226.2005f, 0.1f),
            Position = new Vector3(323.0558f, 222.7048f, 6.103516E-05f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Stash,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2BookOfCain = new TownActor
        {
            Name = "A2 Book of Cain",
            InternalName = SNOActor.a2_Id_All_Book_Of_Cain.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.a2_Id_All_Book_Of_Cain,
            InteractPosition = new Vector3(330.7662f, 227.0237f, 0.1f),
            Position = new Vector3(333.1537f, 222.927f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Identify,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2JewelerGemCombine = new TownActor
        {
            Name = "PT_Jeweler_AddSocketShortcut",
            InternalName = SNOActor.PT_Jeweler_AddSocketShortcut.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.PT_Jeweler_AddSocketShortcut,
            InteractPosition = new Vector3(369.4004f, 230.2056f, 0.1f),
            Position = new Vector3(366.331f, 226.058f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemCombine,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Jeweler = new TownActor
        {
            Name = "A2 Jeweler",
            InternalName = SNOActor.PT_Jeweler.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.PT_Jeweler,
            InteractPosition = new Vector3(377.3954f, 221.7746f, 0.1000003f),
            Position = new Vector3(374.482f, 218.019f, 6.10352E-05f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemTraining,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2KanaiCube = new TownActor
        {
            Name = "A2 Kanai's Cube",
            InternalName = SNOActor.KanaiCube_Stand.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.KanaiCube_Stand,
            InteractPosition = new Vector3(291.9481f, 325.5482f, 0.1000038f),
            Position = new Vector3(288.7741f, 327.0323f, 0.100003f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Transmute,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2ZoltunKulle = new TownActor
        {
            Name = "A2 Zoltun Kulle",
            InternalName = SNOActor.p2_HQ_ZoltunKulle.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.p2_HQ_ZoltunKulle,
            InteractPosition = new Vector3(295.8192f, 332.9078f, 0.1000038f),
            Position = new Vector3(295.6881f, 339.6401f, -6.10352E-05f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Zoltun,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Enchantress = new TownActor
        {
            Name = "A2 Enchantress",
            InternalName = SNOActor.Enchantress.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.Enchantress,
            InteractPosition = new Vector3(320.9602f, 327.7493f, 0.1000038f),
            Position = new Vector3(325.2468f, 329.7658f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Hireling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Scoundrel = new TownActor
        {
            Name = "A2 Scoundrel",
            InternalName = SNOActor.Scoundrel.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.Scoundrel,
            InteractPosition = new Vector3(327.7908f, 323.8778f, 0.1000038f),
            Position = new Vector3(331.5732f, 327.2547f, 6.10348E-05f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Hireling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Templar = new TownActor
        {
            Name = "A2 Templar",
            InternalName = SNOActor.Templar.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.Templar,
            InteractPosition = new Vector3(323.8937f, 339.4733f, 0.1000009f),
            Position = new Vector3(329.1279f, 337.8493f, 0.00012207f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Hireling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Mystic = new TownActor
        {
            Name = "A2 Mystic",
            InternalName = SNOActor.PT_Mystic.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.PT_Mystic,
            InteractPosition = new Vector3(257.2221f, 301.137f, 0.1000034f),
            Position = new Vector3(254.0293f, 299.3363f, 6.10352E-05f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Enchanting,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Peddler = new TownActor
        {
            Name = "A2Peddler",
            InternalName = SNOActor.A2_UniqueVendor_Peddler_InTown_01.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.A2_UniqueVendor_Peddler_InTown_01,
            InteractPosition = new Vector3(289.8295f, 276.1737f, 0.1000036f),
            Position = new Vector3(286.4183f, 276.1614f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Peddler,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2BlacksmithForgeWeapons = new TownActor
        {
            Name = "A2 Blacksmith Forge Weapons",
            InternalName = SNOActor.PT_Blacksmith_ForgeWeaponShortcut.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.PT_Blacksmith_ForgeWeaponShortcut,
            InteractPosition = new Vector3(276.2955f, 239.1677f, 0.1f),
            Position = new Vector3(267.9745f, 238.17f, 7.62939E-06f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ForgeWeapons,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2BlacksmithSalvage = new TownActor
        {
            Name = "A2BlacksmithSalvage",
            InternalName = SNOActor.PT_Blacksmith_RepairShortcut.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.PT_Blacksmith_RepairShortcut,
            InteractPosition = new Vector3(278.9544f, 225.4655f, 0.1000046f),
            Position = new Vector3(270.0626f, 225.621f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Salvage,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };
        
        // Smith does not allow repair in campaign mode has dialog options
        //public static TownActor A2Blacksmith = new TownActor
        //{
        //    Name = "PT_Blacksmith",
        //    InternalName = SNOActor.PT_Blacksmith.ToString(),
        //    Act = Act.A2,
        //    ActorId = SNOActor.PT_Blacksmith,
        //    InteractPosition = new Vector3(274.3154f, 217.5824f, 0.1f),
        //    Position = new Vector3(269f, 218f, 0.0341768f),
        //    IsAdventurerMode = true,
        //    ServiceType = ServiceType.Repair,
        //    IsGizmo = false,
        //    IsUnit = true,
        //    WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
        //    LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        //};

        public static TownActor A2BlacksmithForgeArmor = new TownActor
        {
            Name = "A2 Blacksmith Forge Armor",
            InternalName = SNOActor.PT_Blacksmith_ForgeArmorShortcut.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.PT_Blacksmith_ForgeArmorShortcut,
            InteractPosition = new Vector3(278.7855f, 204.1124f, 0.1f),
            Position = new Vector3(272.2524f, 203.623f, 0f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ForgeArmor,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Fence = new TownActor
        {
            Name = "A2 Fence",
            InternalName = SNOActor.A2_UniqueVendor_Fence_InTown_01.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.A2_UniqueVendor_Fence_InTown_01,
            InteractPosition = new Vector3(334.2357f, 135.1849f, -16.39508f),
            Position = new Vector3(336.9657f, 131.983f, -16.3549f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Fence,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Miner = new TownActor
        {
            Name = "A2 Miner",
            InternalName = SNOActor.A2_UniqueVendor_Miner_InTown_01.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.A2_UniqueVendor_Miner_InTown_01,
            InteractPosition = new Vector3(348.2905f, 147.9847f, -16.39508f),
            Position = new Vector3(349.917f, 155.382f, -16.4951f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Miner,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Healer = new TownActor
        {
            Name = "A2 Healer",
            InternalName = SNOActor.Priest_Caldeum.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.Priest_Caldeum,
            InteractPosition = new Vector3(367.9383f, 130.638f, -16.39508f),
            Position = new Vector3(372.229f, 127.948f, -16.4951f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Healing,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        public static TownActor A2Collector = new TownActor
        {
            Name = "A2 Collector",
            InternalName = SNOActor.A2_UniqueVendor_Collector_InTown_01.ToString(),
            Act = Act.A2,
            ActorId = SNOActor.A2_UniqueVendor_Collector_InTown_01,
            InteractPosition = new Vector3(356.8491f, 135.2607f, -16.39508f),
            Position = new Vector3(355.707f, 127.784f, -16.4951f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Collector,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.caOUT_RefugeeCamp,
            LevelAreaId = SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub
        };

        // A1

        public static TownActor A1Kadala = new TownActor
        {
            Name = "A1 Kadala",
            InternalName = SNOActor.X1_RandomItemNPC.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.X1_RandomItemNPC,
            InteractPosition = new Vector3(382.3804f, 572.0419f, 24.04533f),
            Position = new Vector3(378.829f, 575.168f, 24.0453f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Gambling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Orek = new TownActor
        {
            Name = "A1 Orek",
            InternalName = SNOActor.X1_LR_Nephalem.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.X1_LR_Nephalem,
            InteractPosition = new Vector3(388.3909f, 589.8685f, 24.04533f),
            Position = new Vector3(392.414f, 592.662f, 23.8687f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftFinish,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1ZoltunKulle = new TownActor
        {
            Name = "A1 Zoltun Kulle",
            InternalName = SNOActor.p2_HQ_ZoltunKulle.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.p2_HQ_ZoltunKulle,
            InteractPosition = new Vector3(406.7583f, 588.0729f, 24.02334f),
            Position = new Vector3(412.616f, 592.977f, 23.8746f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Zoltun,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1KanaiCube = new TownActor
        {
            Name = "A1 Kanai's Cube",
            InternalName = SNOActor.KanaiCube_Stand.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.KanaiCube_Stand,
            InteractPosition = new Vector3(418.5141f, 583.8591f, 24.04533f),
            Position = new Vector3(419.253f, 587.147f, 23.9453f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Transmute,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Waypoint = new TownActor
        {
            Name = "A1 Waypoint",
            InternalName = SNOActor.Waypoint_Town.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Waypoint_Town,
            InteractPosition = new Vector3(402.1251f, 556.044f, 24.89966f),
            Position = new Vector3(401.7305f, 555.0093f, 24.66344f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Waypoint,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Templar = new TownActor
        {
            Name = "A1 Templar",
            InternalName = SNOActor.Templar.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Templar,
            InteractPosition = new Vector3(429.9375f, 555.3445f, 24.04532f),
            Position = new Vector3(434.86f, 557.99f, 23.95f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Hireling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Scoundrel = new TownActor
        {
            Name = "A1 Scoundrel",
            InternalName = SNOActor.Scoundrel.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Scoundrel,
            InteractPosition = new Vector3(431.3742f, 567.5134f, 24.04533f),
            Position = new Vector3(436.72f, 568.1f, 23.86f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Hireling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Enchantress = new TownActor
        {
            Name = "A1 Enchantress",
            InternalName = SNOActor.Enchantress.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Enchantress,
            InteractPosition = new Vector3(441.0632f, 559.6582f, 24.04533f),
            Position = new Vector3(444.65f, 556.75f, 23.95f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Hireling,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Tyrael = new TownActor
        {
            Name = "A1 Tyrael",
            InternalName = SNOActor.Tyrael_Heaven.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Tyrael_Heaven,
            InteractPosition = new Vector3(413.5652f, 532.0673f, 24.04533f),
            Position = new Vector3(418.598f, 531.309f, 24.0453f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Tyrael,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Stash = new TownActor
        {
            Name = "A1 Stash",
            InternalName = SNOActor.Player_Shared_Stash.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Player_Shared_Stash,
            InteractPosition = new Vector3(387.6853f, 513.0157f, 24.04533f),
            Position = new Vector3(388.16f, 509.63f, 23.9453f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Stash,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1BookOfCain = new TownActor
        {
            Name = "A1 Book of Cain",
            InternalName = SNOActor.a1_Id_All_Book_Of_Cain.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.a1_Id_All_Book_Of_Cain,
            InteractPosition = new Vector3(368.7403f, 497.8855f, 24.04532f),
            Position = new Vector3(369.77f, 493.56f, 23.95f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Identify,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1BlacksmithForgeArmor = new TownActor
        {
            Name = "A1 Blacksmith Forge Armor",
            InternalName = SNOActor.PT_Blacksmith_ForgeArmorShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Blacksmith_ForgeArmorShortcut,
            InteractPosition = new Vector3(361.3269f, 551.1993f, 23.99268f),
            Position = new Vector3(354.231f, 550.2668f, 23.8432f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ForgeArmor,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Blacksmith = new TownActor
        {
            Name = "A1 Blacksmith",
            InternalName = SNOActor.PT_Blacksmith.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Blacksmith,
            InteractPosition = new Vector3(360.5066f, 562.293f, 24.01392f),
            Position = new Vector3(356.421f, 563.415f, 23.8317f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Repair,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Salvage = new TownActor
        {
            Name = "A1 Salvage",
            InternalName = SNOActor.PT_Blacksmith_RepairShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Blacksmith_RepairShortcut,
            InteractPosition = new Vector3(361.3072f, 577.0943f, 24.04533f),//new Vector3(362.2981f, 573.9589f, 24.04533f),//new Vector3(361.1635f, 571.1841f, 24.04533f),
            Position = new Vector3(352.126f, 572.0295f, 23.8371f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Salvage,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1BlacksmithForgeWeapons = new TownActor
        {
            Name = "A1 Blacksmith Forge Weapons",
            InternalName = SNOActor.PT_Blacksmith_ForgeWeaponShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Blacksmith_ForgeWeaponShortcut,
            InteractPosition = new Vector3(361.1142f, 585.2828f, 24.04532f),
            Position = new Vector3(349.847f, 584.722f, 23.7396f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ForgeWeapons,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1NephalemObelisk = new TownActor
        {
            Name = "A1 Nephalem Obelisk",
            InternalName = SNOActor.x1_OpenWorld_LootRunObelisk_B.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.x1_OpenWorld_LootRunObelisk_B,
            InteractPosition = new Vector3(377.035f, 588.3451f, 24.04533f),
            Position = new Vector3(374.652f, 594.721f, 23.7971f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.RiftStart,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Miner = new TownActor
        {
            Name = "A1 Miner",
            InternalName = SNOActor.A1_UniqueVendor_Miner_InTown_01.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.A1_UniqueVendor_Miner_InTown_01,
            InteractPosition = new Vector3(430.1573f, 577.0442f, 24.04533f),
            Position = new Vector3(434.69f, 580.03f, 23.95f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Miner,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Collector = new TownActor
        {
            Name = "A1 Collector",
            InternalName = SNOActor.A1_UniqueVendor_Collector_InTown_01.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.A1_UniqueVendor_Collector_InTown_01,
            InteractPosition = new Vector3(454.2323f, 451.8214f, 24.04532f),
            Position = new Vector3(453.3335f, 445.1592f, 23.94533f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Collector,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Fence = new TownActor
        {
            Name = "A1 Fence",
            InternalName = SNOActor.A1_UniqueVendor_Fence_InTown_01.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.A1_UniqueVendor_Fence_InTown_01,
            InteractPosition = new Vector3(335.3153f, 302.097f, 0.5997883f),
            Position = new Vector3(333.1045f, 298.124f, 0.4997864f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Fence,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1JewelerGemCombiner = new TownActor
        {
            Name = "A1 Jeweler Gem Combiner",
            InternalName = SNOActor.PT_Jeweler_AddSocketShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Jeweler_AddSocketShortcut,
            InteractPosition = new Vector3(334.4247f, 515.1616f, 24.04532f),
            Position = new Vector3(330.81f, 508.62f, 23.88f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemCombine,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Jeweler = new TownActor
        {
            Name = "A1 Jeweler",
            InternalName = SNOActor.PT_Jeweler.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Jeweler,
            InteractPosition = new Vector3(341.5923f, 504.1067f, 24.04532f),
            Position = new Vector3(339.46f, 501.03f, 23.95f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemTraining,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1JewelerGemRemove = new TownActor
        {
            Name = "PT_Jeweler_RemoveGemShortcut",
            InternalName = SNOActor.PT_Jeweler_RemoveGemShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Jeweler_RemoveGemShortcut,
            InteractPosition = new Vector3(354.6265f, 499.6074f, 24.04532f),
            Position = new Vector3(353.19f, 493.54f, 23.96f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.GemRemove,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Mystic = new TownActor
        {
            Name = "A1Mystic",
            InternalName = SNOActor.PT_Mystic.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Mystic,
            InteractPosition = new Vector3(279.0273f, 512.7984f, 24.04532f),
            Position = new Vector3(277.72f, 515.28f, 23.95f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Enchanting,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1Healer = new TownActor
        {
            Name = "A1 Healer",
            InternalName = SNOActor.Priest_Male_B_NoLook.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Priest_Male_B_NoLook,
            InteractPosition = new Vector3(272.5094f, 565.4769f, 24.04532f),
            Position = new Vector3(269.38f, 564.31f, 23.95f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.Healing,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub
        };

        public static TownActor A1HearthPortal = new TownActor
        {
            Name = "A1 HearthPortal",
            InternalName = SNOActor.hearthPortal.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.hearthPortal,
            InteractPosition = new Vector3(402.354f, 518.5125f, 24.04533f),
            Position = new Vector3(405.959f, 515.3994f, 23.9453f),
            IsAdventurerMode = true,
            ServiceType = ServiceType.ReturnPortal,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            LevelAreaId = SNOLevelArea.A1_Tristram_Adventure_Mode_Hub,
        };

        // A1 Campaign


        public static TownActor A1CampaignWaypoint = new TownActor
        {
            Name = "A1 Waypoint (Campaign)",
            InternalName = SNOActor.Waypoint_Town.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Waypoint_Town,
            InteractPosition = new Vector3(2981.588f, 2833.883f, 24.9007f),
            Position = new Vector3(2981.73f, 2835.009f, 24.66344f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Waypoint,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram,
        };

        public static TownActor A1CampaignHearthPortal = new TownActor
        {
            Name = "A1 HearthPortal (Campaign)",
            InternalName = SNOActor.hearthPortal.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.hearthPortal,
            InteractPosition = new Vector3(2982.349f, 2798.655f, 24.04533f),
            Position = new Vector3(2985.959f, 2795.399f, 23.9453f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.ReturnPortal,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram,
        };

        public static TownActor A1CampaignStash = new TownActor
        {
            Name = "A1 Stash (Campaign)",
            InternalName = SNOActor.Player_Shared_Stash.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Player_Shared_Stash,
            InteractPosition = new Vector3(2968.377f, 2796.289f, 24.04533f),
            Position = new Vector3(2968.16f, 2789.63f, 23.9453f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Stash,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        public static TownActor A1CampaignBookOfCain = new TownActor
        {
            Name = "A1 Book Of Cain (Campaign)",
            InternalName = SNOActor.a1_Id_All_Book_Of_Cain.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.a1_Id_All_Book_Of_Cain,
            InteractPosition = new Vector3(2952.935f, 2776.312f, 24.04533f),
            Position = new Vector3(2950.485f, 2767.289f, 23.9453f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Identify,
            IsGizmo = true,
            IsUnit = false,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        public static TownActor A1CampaignBlacksmithForgeArmor = new TownActor
        {
            Name = "A1 Blacksmith Forge Armor (Campaign)",
            InternalName = SNOActor.PT_Blacksmith_ForgeArmorShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Blacksmith_ForgeArmorShortcut,
            InteractPosition = new Vector3(2941.265f, 2832.725f, 23.98976f),
            Position = new Vector3(2934.231f, 2830.267f, 23.8432f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.ForgeArmor,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        public static TownActor A1CampaignBlacksmithSalvage = new TownActor
        {
            Name = "A1 Blacksmith Salvage (Campaign)",
            InternalName = SNOActor.PT_Blacksmith_RepairShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Blacksmith_RepairShortcut,
            InteractPosition = new Vector3(2941.394f, 2853.283f, 24.04533f),
            Position = new Vector3(2932.126f, 2852.03f, 23.8371f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Salvage,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        public static TownActor A1CampaignBlacksmithForgeWeapons = new TownActor
        {
            Name = "A1 Blacksmith Forge Weapons (Campaign)",
            InternalName = SNOActor.PT_Blacksmith_ForgeWeaponShortcut.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.PT_Blacksmith_ForgeWeaponShortcut,
            InteractPosition = new Vector3(2941.198f, 2862.261f, 24.04532f),
            Position = new Vector3(2929.847f, 2864.722f, 23.7396f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.ForgeWeapons,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        // Disabled because haedrig eamon opens a talk dialog rather than a vendor window in campaign mode.
        //public static TownActor A1CampaignBlacksmith = new TownActor
        //{
        //    Name = "A1 Blacksmith (Campaign)",
        //    InternalName = SNOActor.PT_Blacksmith.ToString(),
        //    Act = Act.A1,
        //    ActorId = SNOActor.PT_Blacksmith,
        //    InteractPosition = new Vector3(2939.816f, 2844.584f, 24.04531f),
        //    Position = new Vector3(2936.421f, 2843.415f, 23.8317f),
        //    IsAdventurerMode = false,
        //    IsTownVendor = true,
        //    ServiceType = ServiceType.Repair,
        //    IsGizmo = false,
        //    IsUnit = true,
        //    WorldSnoId = SNOWorld.trOUT_Town,
        //    LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram,
        //};

        public static TownActor A1CampaignMiner = new TownActor
        {
            Name = "A1 Miner (Campaign)",
            InternalName = SNOActor.A1_UniqueVendor_Miner_InTown_01.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.A1_UniqueVendor_Miner_InTown_01,
            InteractPosition = new Vector3(2896.259f, 2785.079f, 24.04532f),
            Position = new Vector3(2893.607f, 2779.572f, 23.9009f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Miner,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        public static TownActor A1CampaignHealer = new TownActor
        {
            Name = "A1 Healer (Campaign)",
            InternalName = SNOActor.Priest_Male_B_NoLook.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.Priest_Male_B_NoLook,
            InteractPosition = new Vector3(2888.466f, 2804.673f, 24.04532f),
            Position = new Vector3(2884.539f, 2799.291f, 23.9453f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Healing,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        public static TownActor A1CampaignCollector = new TownActor
        {
            Name = "A1 Collector (Campaign)",
            InternalName = SNOActor.A1_UniqueVendor_Collector_InTown_01.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.A1_UniqueVendor_Collector_InTown_01,
            InteractPosition = new Vector3(3033.752f, 2728.269f, 24.04532f),
            Position = new Vector3(3033.333f, 2725.159f, 23.9453f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Collector,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };

        public static TownActor A1CampaignFence = new TownActor
        {
            Name = "A1 Fence (Campaign)",
            InternalName = SNOActor.A1_UniqueVendor_Fence_InTown_01.ToString(),
            Act = Act.A1,
            ActorId = SNOActor.A1_UniqueVendor_Fence_InTown_01,
            InteractPosition = new Vector3(2914.041f, 2585.553f, 0.5997883f),
            Position = new Vector3(2913.104f, 2578.124f, 0.499786f),
            IsAdventurerMode = false,
            ServiceType = ServiceType.Fence,
            IsGizmo = false,
            IsUnit = true,
            WorldSnoId = SNOWorld.trOUT_Town,
            LevelAreaId = SNOLevelArea.A1_trOUT_NewTristram
        };
    }
}
