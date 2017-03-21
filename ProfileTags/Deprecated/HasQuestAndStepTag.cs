//using System.Linq;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("HasQuestAndStep")]
//    public class HasQuestAndStep : BaseComplexProfileBehavior
//    {
//        public HasQuestAndStep() { }
//        protected override Composite CreateBehavior()
//        {
//            return
//            new Decorator(ret => !IsDone,
//                new PrioritySelector(
//                    GetNodes().Select(b => b.Behavior).ToArray()
//                )
//            );
//        }

//        public override bool GetConditionExec()
//        {
//            return ZetaDia.Storage.Quests.AllQuests
//                .Where(quest => quest.QuestSNO == QuestId && quest.State != QuestState.Completed && quest.QuestStep == StepId).FirstOrDefault() != null;
//        }

//        private bool CheckNotAlreadyDone(object obj)
//        {
//            return !IsDone;
//        }
//    }
//}
