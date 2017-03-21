//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// Stops all timers.
//    /// </summary>
//    [XmlElement("StopAllTimers")]
//    public class StopAllTimersTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        /// <summary>
//        /// Flags the timer as successfull in finding an objective
//        /// </summary>
//        [XmlAttribute("objectiveFound")]
//        public bool ObjectiveFound { get; set; }

//        private bool _isDone;
//        public override bool IsDone { get { return _isDone; } }

//        protected override Composite CreateBehavior()
//        {
//            return new Sequence(
//                new Action(ret => TimeTracker.StopAll(ObjectiveFound)),
//                new Action(ret => _isDone = true)
//            );
//        }

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
