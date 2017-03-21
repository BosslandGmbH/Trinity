//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// XML tag for a profile to STOP a timer
//    /// </summary>
//    [XmlElement("StopTimer")]
//    public class StopTimerTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        private bool _isDone;
//        public override bool IsDone { get { return _isDone; } }

//        /// <summary>
//        /// Specifying a value for name="" will the timer with that name to be stopped.
//        /// </summary>
//        [XmlAttribute("name")]
//        public string Name { get; set; }

//        /// <summary>
//        /// Specifying a value for group="" will cause all timers with that group name to be stopped.
//        /// </summary>
//        [XmlAttribute("group")]
//        public string Group { get; set; }

//        /// <summary>
//        /// Flags the timer as successfull in finding an objective
//        /// </summary>
//        [XmlAttribute("objectiveFound")]
//        public bool ObjectiveFound { get; set; }

//        protected override Composite CreateBehavior()
//        {
//            return new Sequence(
//                new PrioritySelector(

//                    new Decorator(ret => Name != null,
//                        new Action(ret => TimeTracker.StopTimer(Name, ObjectiveFound))),

//                    new Decorator(ret => Group != null,
//                        new Action(ret => TimeTracker.StopGroup(Group, ObjectiveFound)))

//                ),
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
