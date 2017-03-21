//using Trinity.Components.QuestTools.Helpers;
//using Zeta.Common;
//using Zeta.XmlEngine;


//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("When")]
//    public class WhenTag : BaseComplexProfileBehavior
//    {
//        [XmlAttribute("condition")]
//        public string Condition { get; set; }

//        [XmlAttribute("name")]
//        public string Name { get; set; }

//        [XmlAttribute("persist")]
//        public bool Persist { get; set; }

//        [XmlAttribute("repeat")]
//        public bool Repeat { get; set; }

//        public override bool GetConditionExec()
//        {
//            BotBehaviorQueue.Queue(new QueueItem
//            {
//                Condition = ret => ScriptManager.GetCondition(Condition).Invoke(),
//                Name = Name,
//                Nodes = Body,
//                Persist = Persist,
//                Repeat = Repeat,
//            });

//            return false;
//        }
//    }
//}

