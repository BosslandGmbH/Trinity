using System.Collections.Generic;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public static class ExplorationData
    {
        public static float NavigationNodeBoxSize = 2.5f;
        //{
        //    get { return OpenWorldIds.Contains(AdvDia.CurrentWorldSnoId) ? 8 : 4; }
        //}

        public static float ExplorationNodeBoxSize => OpenWorldIds.Contains(AdvDia.CurrentWorldId) ? 40 : 20;

        public static float ExplorationNodeBoxTolerance => OpenWorldIds.Contains(AdvDia.CurrentWorldId) ? 0.2f : 0.1f;

        public static HashSet<SNOWorld> OpenWorldIds = new HashSet<SNOWorld>
                                                {
                                                    SNOWorld.trOUT_Town,
                                                    SNOWorld.caOUT_Town,
                                                    SNOWorld.A3_Battlefields_02,
                                                    SNOWorld.x1_westm_Graveyard_DeathOrb,
                                                    SNOWorld.x1_Bog_01
                                                };

        public static HashSet<SNOLevelArea> FortressLevelAreaIds = new HashSet<SNOLevelArea>
                                                {
                                                    SNOLevelArea.x1_fortress_level_01_Master,
                                                    SNOLevelArea.x1_fortress_level_02_Master,
                                                    SNOLevelArea.x1_fortress_level_02_Intro,
                                                    SNOLevelArea.X1_fortress_malthael_entrance,
                                                    SNOLevelArea.x1_fortress_level_01_B,
                                                    SNOLevelArea.x1_Fortress_MalthaelArena,
                                                    SNOLevelArea.x1_fortress_level_02,
                                                    SNOLevelArea.x1_fortress_level_01,
                                                    SNOLevelArea.X1_LR_Tileset_Fortress,
                                                };

        public static HashSet<SNOWorld> FortressWorldIds = new HashSet<SNOWorld>
                                                {
                                                    SNOWorld.x1_fortress_level_01,
                                                    SNOWorld.x1_fortress_level_02,
                                                };

        public static readonly HashSet<string> IgnoreScenes = new HashSet<string>
        {
            "caOut_Oasis_Sub240_Water_Money"
        };

        public static readonly Dictionary<Act, SNOWorld> ActHubWorldIds = new Dictionary<Act, SNOWorld>
                                                                     {
                                                                         {Act.A1, SNOWorld.X1_Tristram_Adventure_Mode_Hub},
                                                                         {Act.A2, SNOWorld.caOUT_RefugeeCamp},
                                                                         {Act.A3, SNOWorld.a3dun_hub_keep},
                                                                         {Act.A4, SNOWorld.a3dun_hub_keep},
                                                                         {Act.A5, SNOWorld.X1_Westmarch_Hub},
                                                                     };

        public const int GreedPortalSNO = 393030;
    }
}