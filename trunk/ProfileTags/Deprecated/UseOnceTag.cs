//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    // * UseOnce ensures a sequence of tags is only ever used once during this profile
//    [XmlElement("UseOnce")]
//    [XmlElement("TrinityUseOnce")]
//    public class UseOnceTag : BaseComplexProfileBehavior
//    {
//        public UseOnceTag() { }
//        // A list of "useonceonly" tags that have been triggered this xml profile
//        public static HashSet<string> UseOnceIDs = new HashSet<string>();
//        public static Dictionary<string, int> UseOnceCounter = new Dictionary<string, int>();

//        protected override Composite CreateBehavior()
//        {
//            return
//             new Decorator(ret => !IsDone,
//                 new PrioritySelector(
//                     GetNodes().Select(b => b.Behavior).ToArray()
//                 )
//             );
//        }

//        public override bool GetConditionExec()
//        {
//            // See if we've EVER hit this ID before
//            if (UseOnceIDs.Contains(ID))
//            {
//                // See if we've hit it more than or equal to the max times before
//                if (UseOnceCounter[ID] >= Max || UseOnceCounter[ID] < 0)
//                    return false;

//                // Add 1 to our hit count, and let it run this time
//                UseOnceCounter[ID]++;
//                return true;
//            }

//            // Never hit this before, so create the entry and let it run

//            // First see if we should disable all other ID's currently hit to prevent them ever being run again this run
//            if (!DisablePrevious)
//            {
//                foreach (string id in UseOnceIDs)
//                {
//                    if (id != ID)
//                    {
//                        UseOnceCounter[id] = -1;
//                    }
//                }
//            }

//            // Now store the fact we have hit this ID and set up the dictionary entry for it
//            UseOnceIDs.Add(ID);
//            UseOnceCounter.Add(ID, 1);
//            return true;
//        }

//        [XmlAttribute("id")]
//        public string ID { get; set; }

//        [XmlAttribute("disableprevious")]
//        public bool DisablePrevious { get; set; }

//        [XmlAttribute("max")]
//        public int Max { get; set; }

//        public Func<bool> Conditional { get; set; }

//    }
//}
