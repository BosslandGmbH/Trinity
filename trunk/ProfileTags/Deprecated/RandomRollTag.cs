//using System;
//using Trinity.Framework;
//using System.Collections.Generic;
//using System.Globalization;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("TrinityRandomRoll")]
//    [XmlElement("RandomRoll")]
//    public class RandomRollTag : ProfileBehavior
//    {
//        public RandomRollTag() { }
//        private bool _isDone;

//        public override bool IsDone
//        {
//            get { return _isDone; }
//        }

//        protected override Composite CreateBehavior()
//        {
//            return new Action(ret => Roll());
//        }

//        private void Roll()
//        {
//            // Generate a random value between the selected min-max range, and assign it to our dictionary of random values
//            int oldValue;
//            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
//            int randval = (rndNum.Next((Max - Min) + 1)) + Min;

//            Core.Logger.Log("Generating RNG for profile between {0} and {1}, result={2}", Min, Max, randval);
//            if (!RandomIds.TryGetValue(Id, out oldValue))
//            {
//                RandomIds.Add(Id, randval);
//            }
//            else
//            {
//                RandomIds[Id] = randval;
//            }
//            _isDone = true;
//        }

//        [XmlAttribute("id")]
//        public int Id { get; set; }

//        [XmlAttribute("min")]
//        public int Min { get; set; }

//        [XmlAttribute("max")]
//        public int Max { get; set; }

//        internal static Dictionary<int, int> RandomIds = new Dictionary<int, int>();

//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            base.ResetCachedDone();
//        }
//    }
//}
