using System.Collections;
using System.Linq;
using System.Web.UI.WebControls.Expressions;
using System.Windows.Media;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Sno.Helpers;
using Trinity.Framework.Objects.Memory.Sno.Types;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public static class SnoManager
    {
        static SnoManager()
        {
            Core = new SnoCore();
            Groups = new SnoGroups();
            StringList = new StringListHelper();
            GameBalance = new GameBalanceHelper();
        }

        public static SnoCore Core { get; set; }

        public static SnoGroups Groups { get; set; }

        public static StringListHelper StringList { get; set; }

        public static GameBalanceHelper GameBalance { get; set; }

        public class SnoGroups
        {
            public SnoGroups()
            {
                StringList = Core.CreateGroup<SnoStringList>(SnoType.StringList);
                Monster = Core.CreateGroup<NativeMonster>(SnoType.Monster);
                //Hero = Core.CreateGroup<NativeHero>(SnoType.Hero);
                //GameBalance = Core.CreateGroup<NativeGameBalance>(SnoType.GameBalance);
                //Globals = Core.CreateGroup<NativeGlobals>(SnoType.Globals);
                Actor = Core.CreateGroup<NativeActor>(SnoType.Actor);
                //Account = Core.CreateGroup<NativeAccount>(SnoType.Account);
                //Globals = Core.CreateGroup<NativeGlobals>(SnoType.Globals);
                //LevelArea = Core.CreateGroup<NativeLevelArea>(SnoType.LevelArea);
                //Act = Core.CreateGroup<NativeAct>(SnoType.Act);
                //TreasureClass = Core.CreateGroup<NativeTreasureClass>(SnoType.TreasureClass);
                //Power = Core.CreateGroup<NativePower>(SnoType.Power);
                //SkillKit = Core.CreateGroup<NativeSkillKit>(SnoType.SkillKit);
                //Worlds = Core.CreateGroup<NativeWorlds>(SnoType.Worlds);
                //Scene = Core.CreateGroup<NativeScene>(SnoType.Scene);
                //SceneGroup = Core.CreateGroup<NativeSceneGroup>(SnoType.SceneGroup);
            }

            public SnoGroup<SnoStringList> StringList { get; }
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
            public SnoGroup<NativeGameBalance> GameBalance { get; }
            public SnoGroup<NativeGlobals> Globals { get; }
        }
    }
}


