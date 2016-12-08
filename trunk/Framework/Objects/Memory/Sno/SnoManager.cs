using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls.Expressions;
using System.Windows.Media;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno.Helpers;
using Trinity.Framework.Objects.Memory.Sno.Types;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public static class SnoManager
    {
        static SnoManager()
        {
            Core = new SnoCore();
            Groups = new SnoGroups();
            StringListHelper = new StringListHelper();
            GameBalanceHelper = new GameBalanceHelper();
        }

        public static SnoCore Core { get; set; }
        public static SnoGroups Groups { get; set; }
        public static StringListHelper StringListHelper { get; set; }
        public static GameBalanceHelper GameBalanceHelper { get; set; }

        public class SnoGroups
        {
            private Dictionary<int, NativePower> _d;

            public SnoGroups()
            {
                var sw = Stopwatch.StartNew();
                StringList = Core.CreateGroup<SnoStringList>(SnoType.StringList);
                //Power = Core.CreateGroup<NativePower>(SnoType.Power);
                //GameBalance = Core.CreateGroup<GameBalanceCollection>(SnoType.GameBalance);
                sw.Stop();
                Logger.LogVerbose($"Created SnoGroups in {sw.Elapsed.TotalMilliseconds}ms");

                //Monster = Core.CreateGroup<NativeMonster>(SnoType.Monster);
                //GameBalance = Core.CreateGroup<GameBalanceCollection>(SnoType.GameBalance);
                //Globals = Core.CreateGroup<NativeGlobals>(SnoType.Globals);
                //Actor = Core.CreateGroup<NativeActor>(SnoType.Actor);
                //Account = Core.CreateGroup<NativeAccount>(SnoType.Account);
                //LevelArea = Core.CreateGroup<NativeLevelArea>(SnoType.LevelArea);
                //Act = Core.CreateGroup<NativeAct>(SnoType.Act);
                //TreasureClass = Core.CreateGroup<NativeTreasureClass>(SnoType.TreasureClass);



                //SkillKit = Core.CreateGroup<NativeSkillKit>(SnoType.SkillKit);
                //Worlds = Core.CreateGroup<NativeWorlds>(SnoType.Worlds);
                //Scene = Core.CreateGroup<NativeScene>(SnoType.Scene);
                //SceneGroup = Core.CreateGroup<NativeSceneGroup>(SnoType.SceneGroup);
            }

            public List<NativePower> Powers { get; set; }
            public SnoGroup<SnoStringList> StringList { get; }
            public SnoGroup<GameBalanceCollection> GameBalance { get; }
            public SnoGroup<NativeScene> Scene { get; }
            public SnoGroup<NativeSceneGroup> SceneGroup { get; }
            public SnoGroup<NativeWorlds> Worlds { get; }
            public SnoGroup<NativeSkillKit> SkillKit { get; }
            public SnoGroup<NativePower> Power { get; }
            public SnoGroup<NativeAct> Act { get; }
            public SnoGroup<NativeLevelArea> LevelArea { get; }
            public SnoGroup<NativeTreasureClass> TreasureClass { get; }
            public SnoGroup<NativeActor> Actor { get; }
            public SnoGroup<NativeAccount> Account { get; }
            public SnoGroup<NativeMonster> Monster { get; }
            public SnoGroup<NativeHero> Hero { get; }
            public SnoGroup<NativeGlobals> Globals { get; }

        }
    }
}


