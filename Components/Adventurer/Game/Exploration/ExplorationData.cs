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

        public static float ExplorationNodeBoxSize
        {
            get
            {
                return OpenWorldIds.Contains(AdvDia.CurrentWorldId) ? 40 : 20;
                //return OpenWorldIds.Contains(AdvDia.CurrentWorldSnoId) ? 30 : 15;
            }
        }

        public static float ExplorationNodeBoxTolerance
        {
            get
            {
                return OpenWorldIds.Contains(AdvDia.CurrentWorldId) ? 0.2f : 0.1f;
                //return OpenWorldIds.Contains(AdvDia.CurrentWorldSnoId) ? 0.1f : 0.03f;
            }
        }

        public static HashSet<int> OpenWorldIds = new HashSet<int>
                                                {
                                                    71150,
                                                    70885,
                                                    95804,
                                                    338944,
                                                    267412
                                                };

        public static HashSet<int> FortressLevelAreaIds = new HashSet<int>
                                                {
                                                    370512,
                                                    366169,
                                                    360494,
                                                    349787,
                                                    340533,
                                                    276361,
                                                    271271,
                                                    271234,
                                                    333758,
                                                };

        public static HashSet<int> FortressWorldIds = new HashSet<int>
                                                {
                                                    271233,
                                                    271235,
                                                };

        public static readonly HashSet<string> IgnoreScenes = new HashSet<string>
                                                              {
            "caOut_Oasis_Sub240_Water_Money"
        };

        public static readonly Dictionary<Act, int> ActHubWorldIds = new Dictionary<Act, int>
                                                                     {
                                                                         {Act.A1, 332336},
                                                                         {Act.A2, 161472},
                                                                         {Act.A3, 172909},
                                                                         {Act.A4, 172909},
                                                                         {Act.A5, 304235},
                                                                     };

        public const int GreedPortalSNO = 393030;
    }
}