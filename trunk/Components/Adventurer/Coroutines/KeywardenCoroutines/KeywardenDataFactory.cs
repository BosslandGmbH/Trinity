using System.Collections.Generic;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines.KeywardenCoroutines
{
    //act1 bones
    //act2 gluttony
    //act3 war
    //act4 evil

    //A1: ActorId: 366946, Type: Item, Name: Infernal Machine of Bones
    //A2: ActorId: 366947, Type: Item, Name: Infernal Machine of Gluttony
    //A3: ActorId: 366948, Type: Item, Name: Infernal Machine of War
    //A4: ActorId: 366949, Type: Item, Name: Infernal Machine of Evil

    //A1: WorldId: 71150 LevelAreaSnoIdId: 19952 WaypointNr: 8
    //A2: WorldId: 70885 LevelAreaSnoIdId: 57425 WaypointNr: 23
    //A3: WorldId: 93099 LevelAreaSnoIdId: 93173 WaypointNr: 28
    //A4: WorldId: 409510 LevelAreaSnoIdId: 409512 WaypointNr: 43

    public static class KeywardenDataFactory
    {
        public static HashSet<int> A4CorruptionSNOs = new HashSet<int> { 210268, 210120 };

        public static HashSet<int> GoblinSNOs = new HashSet<int>
        {
            //5984, 5985, 5987, 5988, 405186, 380657

        // treasureGoblin_A
        5984,
        // treasureGoblin_B
        5985,
        // treasureGoblin_backpack
        5986,
        // treasureGoblin_C
        5987,
        // treasureGoblin_Portal
        //54862,
        // treasureGoblin_Portal_Proxy
        //54887,
        // treasureGoblin_portal_emitter
        //59948,
        // treasureGoblin_portal_opening
        //60549,
        // treasureGoblin_portal_closing
        //60558,
        // treasureGoblin_portal_summon_trailActor
        //108403,
        // treasureGoblin_stunImmune_trailActor
        //129286,
        // Lore_Bestiary_TreasureGoblin
        //147316,
        // treasureGoblin_A_Slave
        //326803,
        // treasureGoblin_C_Unique_DevilsHand
        343046,
        // p1_treasureGobin_A_Unique_GreedMinion
        380657,
        // treasureGoblin_G
        391593,
        // p1_treasureGoblin_inBackpack_A
        394196,
        // p1_treasureGoblin_jump_trailActor
        //403549,
        // p1_treasureGoblin_tentacle_A
        405186,
        // p1_treasureGoblin_tentacle_backpack
        405189,
        // treasureGoblin_D_Splitter
        408354,
        // treasureGoblin_E
        408655,
        // treasureGoblin_F
        408989,
        // treasureGoblin_Portal_Open
        //410460,
        // treasureGoblin_D_Splitter_02
        410572,
        // treasureGoblin_D_Splitter_03
        410574,
        // treasureGoblin_H
        413289,
        // p1_treasureGoblin_teleport_shell
        //428094,
        // p1_treasureGoblin_backpack_B
        428205,
        // p1_treasureGoblin_backpack_F
        428206,
        // p1_treasureGoblin_backpack_C
        428211,
        // p1_treasureGoblin_backpack_H
        428213,
        // p1_treasureGoblin_backpack_D
        428247,
        // treasureGoblin_I
        428663,
        // treasureGoblin_J
        429161,
        // p1_treasureGoblin_backpack_J
        429526,
        // treasureGoblin_B_WhatsNew
        429615,
        // treasureGoblin_F_WhatsNew
        429619,
        // treasureGoblin_C_WhatsNew
        429620,
        // treasureGoblin_B_FX_WhatsNew
        429624,
        // p1_treasureGoblin_backpack_E
        429660,
        // treasureGoblin_A_LegacyPuzzleRing
        //429689,
        // p1_treasureGoblin_backpack_I
        433905,
        // treasureGoblin_J_WhatsNew
        434630,
        // treasureGoblin_J_FX_WhatsNew
        434631,
        // treasureGoblin_E_WhatsNew
        434632,
        // treasureGoblin_D_WhatsNew
        434633,
        // treasureGoblin_Anniversary_Event
        434745,
         };

        public static Dictionary<Act, KeywardenData> Items;

        static KeywardenDataFactory()
        {
            Items = new Dictionary<Act, KeywardenData>();
            var act1 = new KeywardenData
            {
                Act = Act.A1,
                KeywardenSNO = 255704,
                KeySNO = 366946,
                WorldId = 71150,
                LevelAreaId = 19952,
                WaypointNumber = 8,
                BossEncounter = SNOBossEncounter.A1_KeywardenPlaceholder
            };
            Items.Add(Act.A1, act1);

            var act2 = new KeywardenData
            {
                Act = Act.A2,
                KeywardenSNO = 256022,
                KeySNO = 366947,
                WorldId = 70885,
                LevelAreaId = 57425,
                WaypointNumber = 24,
                BossEncounter = SNOBossEncounter.A2_KeywardenPlaceholder
            };
            Items.Add(Act.A2, act2);

            var act3 = new KeywardenData
            {
                Act = Act.A3,
                KeywardenSNO = 256040,
                KeySNO = 366948,
                WorldId = 93099,
                LevelAreaId = 93173,
                WaypointNumber = 29,
                BossEncounter = SNOBossEncounter.A3_KeywardenPlaceholder
            };
            Items.Add(Act.A3, act3);

            var act4 = new KeywardenData
            {
                Act = Act.A4,
                KeywardenSNO = 256054,
                KeySNO = 366949,
                WorldId = 409510,
                LevelAreaId = 409512,
                WaypointNumber = 44,
                BossEncounter = SNOBossEncounter.A4_KeywardenPlaceholder
            };
            Items.Add(Act.A4, act4);
        }
    }
}