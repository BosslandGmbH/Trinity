//using Trinity.Components.QuestTools.Helpers;
//using Trinity.Framework;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    // TrinityMaxDeaths tells Trinity to handle deaths and exit game after X deaths
//    [XmlElement("TrinityMaxDeaths")]
//    [XmlElement("MaxDeaths")]
//    public class MaxDeathsTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public MaxDeathsTag() { }
//        private bool _isDone;

//        public override bool IsDone
//        {
//            get { return _isDone; }
//        }

//        protected override Composite CreateBehavior()
//        {
//            return
//            new Sequence(
//                new DecoratorContinue(ret => MaxDeaths != Death.MaxDeathsAllowed,
//                    new Action(ret => Core.Logger.Log("Max deaths set by profile. Will restart the game after {0}", MaxDeaths))
//                ),
//                new Action(ret => Death.MaxDeathsAllowed = MaxDeaths),
//                new DecoratorContinue(ret => Reset,
//                    new Action(ret => Death.DeathCount = 0)
//                ),
//                new Action(ret => _isDone = true)
//            );
//        }

//        [XmlAttribute("reset")]
//        public bool Reset { get; set; }

//        [XmlAttribute("max")]
//        public int MaxDeaths { get; set; }

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
