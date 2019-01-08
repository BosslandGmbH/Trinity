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
        public static HashSet<SNOActor> KeyIds = new HashSet<SNOActor>
        {
            SNOActor.InfernalMachine_SkeletonKing_x1,
            SNOActor.InfernalMachine_Ghom_x1,
            SNOActor.InfernalMachine_SiegeBreaker_x1,
            SNOActor.InfernalMachine_Diablo_x1
        };

        public static HashSet<SNOActor> A4CorruptionSNOs = new HashSet<SNOActor>
        {
            SNOActor.a4dun_Garden_Corruption_HellRift_Monster,
            SNOActor.a4dun_Garden_Corruption_Monster
        };

        public static HashSet<SNOActor> GoblinSNOs = new HashSet<SNOActor>
        {
            SNOActor.treasureGoblin_A,
            SNOActor.treasureGoblin_B,
            SNOActor.treasureGoblin_backpack,
            SNOActor.treasureGoblin_C,
            SNOActor.treasureGoblin_C_Unique_DevilsHand,
            SNOActor.p1_treasureGobin_A_Unique_GreedMinion,
            SNOActor.treasureGoblin_G,
            SNOActor.p1_treasureGoblin_inBackpack_A,
            SNOActor.p1_treasureGoblin_tentacle_A,
            SNOActor.p1_treasureGoblin_tentacle_backpack,
            SNOActor.treasureGoblin_D_Splitter,
            SNOActor.treasureGoblin_E,
            SNOActor.treasureGoblin_F,
            SNOActor.treasureGoblin_D_Splitter_02,
            SNOActor.treasureGoblin_D_Splitter_03,
            SNOActor.treasureGoblin_H,
            SNOActor.p1_treasureGoblin_teleport_shell,
            SNOActor.p1_treasureGoblin_backpack_B,
            SNOActor.p1_treasureGoblin_backpack_F,
            SNOActor.p1_treasureGoblin_backpack_C,
            SNOActor.p1_treasureGoblin_backpack_H,
            SNOActor.p1_treasureGoblin_backpack_D,
            SNOActor.treasureGoblin_I,
            SNOActor.treasureGoblin_J,
            SNOActor.p1_treasureGoblin_backpack_J,
            SNOActor.treasureGoblin_B_WhatsNew,
            SNOActor.treasureGoblin_F_WhatsNew,
            SNOActor.treasureGoblin_C_WhatsNew,
            SNOActor.treasureGoblin_B_FX_WhatsNew,
            SNOActor.p1_treasureGoblin_backpack_E,
            SNOActor.p1_treasureGoblin_backpack_I,
            SNOActor.treasureGoblin_J_WhatsNew,
            SNOActor.treasureGoblin_J_FX_WhatsNew,
            SNOActor.treasureGoblin_E_WhatsNew,
            SNOActor.treasureGoblin_D_WhatsNew,
            SNOActor.treasureGoblin_Anniversary_Event,
         };

        public static Dictionary<Act, KeywardenData> Items;

        static KeywardenDataFactory()
        {
            Items = new Dictionary<Act, KeywardenData>();
            var act1 = new KeywardenData
            {
                Act = Act.A1,
                KeywardenSNO = SNOActor.GoatMutant_Ranged_A_Unique_Uber,
                KeySNO = SNOActor.InfernalMachine_SkeletonKing_x1,
                WorldId = SNOWorld.trOUT_Town,
                LevelAreaId = SNOLevelArea.A1_trOut_TristramFields_A,
                WaypointLevelAreaId = SNOLevelArea.A1_trOut_TristramFields_A,
                BossEncounter = SNOBossEncounter.A1_KeywardenPlaceholder
            };
            Items.Add(Act.A1, act1);

            var act2 = new KeywardenData
            {
                Act = Act.A2,
                KeywardenSNO = SNOActor.DuneDervish_B_Unique_Uber,
                KeySNO = SNOActor.InfernalMachine_Ghom_x1,
                WorldId = SNOWorld.caOUT_Town,
                LevelAreaId = SNOLevelArea.A2_caOut_Oasis,
                WaypointLevelAreaId = SNOLevelArea.A2_caOut_Oasis,
                BossEncounter = SNOBossEncounter.A2_KeywardenPlaceholder
            };
            Items.Add(Act.A2, act2);

            var act3 = new KeywardenData
            {
                Act = Act.A3,
                KeywardenSNO = SNOActor.morluSpellcaster_A_Unique_Uber,
                KeySNO = SNOActor.InfernalMachine_SiegeBreaker_x1,
                WorldId = SNOWorld.a3Dun_rmpt_Level02,
                LevelAreaId = SNOLevelArea.A3_dun_rmpt_Level02,
                WaypointLevelAreaId = SNOLevelArea.A3_dun_rmpt_Level02,
                BossEncounter = SNOBossEncounter.A3_KeywardenPlaceholder
            };
            Items.Add(Act.A3, act3);

            var act4 = new KeywardenData
            {
                Act = Act.A4,
                KeywardenSNO = SNOActor.TerrorDemon_A_Unique_Uber,
                KeySNO = SNOActor.InfernalMachine_Diablo_x1,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_A,
                LevelAreaId = SNOLevelArea.A4_dun_Garden_of_Hope_A,
                WaypointLevelAreaId = SNOLevelArea.A4_dun_Garden_of_Hope_A,
                BossEncounter = SNOBossEncounter.A4_KeywardenPlaceholder
            };
            Items.Add(Act.A4, act4);
        }
    }
}
