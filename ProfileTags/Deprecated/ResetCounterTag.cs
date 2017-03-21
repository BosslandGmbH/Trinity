//using Trinity.Components.QuestTools.ProfileTags.Depreciated;
//using Trinity.Framework;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("ResetCounter")]
//    public class ResetCounterTag : ProfileBehavior
//    {
//        private bool _isDone;
//        public override bool IsDone
//        {
//            get { return _isDone; }
//        }

//        [XmlAttribute("name")]
//        public string Name { get; set; }

//        [XmlAttribute("message")]
//        public string Message { get; set; }


//        protected override Composite CreateBehavior()
//        {
//            return new Action(ret => Reset());
//        }

//        private bool Reset()
//        {
//            if (!IncrementCounterTag.Initialized)
//                IncrementCounterTag.Initialize();

//            if (IncrementCounterTag.Counters.ContainsKey(Name))
//                IncrementCounterTag.Counters[Name] = 0;

//            if (!string.IsNullOrWhiteSpace(Message))
//            {
//                Core.Logger.Log(Message, Name);
//            }

//            _isDone = true;
//            return true;
//        }

//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            base.ResetCachedDone();
//        }

//    }
//}



