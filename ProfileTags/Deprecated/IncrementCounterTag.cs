//using System.Collections.Generic;
//using Trinity.Framework;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("IncrementCounter")]
//    public class IncrementCounterTag : ProfileBehavior
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


//        public static Dictionary<string, int> Counters = new Dictionary<string, int>();
//        public static bool Initialized;
//        public static void Initialize()
//        {
//            BotMain.OnStart += bot => Counters.Clear();
//            Initialized = true;
//        }

//        protected override Composite CreateBehavior() 
//        { 
//            return new Action(ret => Increment());
//        }

//        private bool Increment()
//        {
//            if (!Initialized)
//                Initialize();

//            if (Counters.ContainsKey(Name))
//                Counters[Name]++;
//            else
//                Counters.Add(Name, 1);

//            if (!string.IsNullOrWhiteSpace(Message))
//            {
//                if (Message.Contains("{0}"))
//                {
//                    Core.Logger.Log(Message, Counters[Name]);
//                }
//                else
//                {
//                    Core.Logger.Log(Message);
//                }
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



