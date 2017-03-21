//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// Prevents a useonce tag ID ever being used again
//    /// </summary>                    
//    [XmlElement("UseStop")]
//    [XmlElement("TrinityUseStop")]
//    public class UseStopTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public UseStopTag() { }
//        private bool _isDone = false;

//        public override bool IsDone
//        {
//            get { return _isDone; }
//        }

//        protected override Composite CreateBehavior()
//        {
//            return
//            new Sequence(
//                new PrioritySelector(
//                    new Decorator(ret => UseOnceTag.UseOnceIDs.Contains(ID),
//                        new Action(ret => UseOnceTag.UseOnceCounter[ID] = -1)
//                    ),
//                    new Sequence(
//                        new Action(ret => UseOnceTag.UseOnceIDs.Add(ID)),
//                        new Action(ret => UseOnceTag.UseOnceCounter.Add(ID, -1))
//                    )
//                ),
//                new Action(ret => _isDone = true)
//            );
//        }

//        [XmlAttribute("id")]
//        public string ID{ get; set; }

//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            base.ResetCachedDone();
//        }

//        #region IEnhancedProfileBehavior

//        public void Update()
//        {
//            UpdateBehavior();
//        }

//        public void Start()
//        {
//            OnStart();
//        }

//        public void Done()
//        {
//            _isDone = true;
//        }

//        #endregion
//    }
//}
