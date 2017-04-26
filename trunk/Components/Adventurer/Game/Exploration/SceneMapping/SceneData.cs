using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Game.Rift;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;


namespace Trinity.Components.Adventurer.Game.Exploration.SceneMapping
{
    public class SceneInfo
    {
        public string Name { get; set; }
        public int SnoId { get; set; }
        public Vector2 Size { get; set; }
        public SceneType Type { get; set; }
        public List<Vector3> PointsOfInterest { get; set; }
        public RegionGroup IgnoreRegions { get; set; } = new RegionGroup();
    }

    public enum SceneType
    {
        None = 0,
        Normal,
    }



    public static class SceneData
    {
        public static Dictionary<int, SceneInfo> SceneDefs { get; } = new Dictionary<int, SceneInfo>();

        static SceneData()
        {
            // The top of the d3 minimap is 0,0.
            // X axis increases from top right to bottom left,
            // Y axis increases from top left to bottom right.

            // Cathedral

            SceneDefs.Add(32960, new SceneInfo
            {
                Name = "trDun_Cath_NSE_01",
                SnoId = 32960,
                Size = new Vector2(240, 240),
                Type = SceneType.Normal,
                IgnoreRegions = new RegionGroup
                {
                    new RectangularRegion(5.129883f, 5.093689f, 202.1097f, 78.83484f, CombineType.Add)
                }
            });

            SceneDefs.Add(1884, new SceneInfo
            {
                Name = "trDun_Cath_NSW_01",
                SnoId = 1884,
                Size = new Vector2(240, 240),
                Type = SceneType.Normal,
                IgnoreRegions = new RegionGroup
                {
                    new RectangularRegion(133, 6, 193, 72, CombineType.Add),
                    new RectangularRegion(36, 143, 178, 198, CombineType.Add),
                    new RectangularRegion(4, 2, 63, 75, CombineType.Add)
                }
            });

            //SceneDefs.Add(1884, new SceneInfo
            //{
            //    Name = "trDun_Cath_NSW_01",
            //    SnoId = 1884,
            //    Size = new Vector2(240, 240),
            //    Type = SceneType.Normal,
            //    IgnoreRegions = new RegionGroup
            //    {
            //        new RectangularRegion(141, 48, 189, 170, CombineType.Add),
            //        new RectangularRegion(58, 19, 136, 74, CombineType.Add)
            //    }
            //});


        }

    }
}