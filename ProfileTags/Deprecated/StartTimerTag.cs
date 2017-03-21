//using System;
//using System.Linq;
//using Zeta.Bot.Profile;
//using Zeta.Game;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{

//    /// <summary>
//    /// XML tag for a profile to START a timer
//    /// </summary>
//    [XmlElement("StartTimer")]
//    public class StartTimerTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public StartTimerTag() { }
//        private bool _isDone;
//        public override bool IsDone { get { return _isDone; } }

//        /// <summary>
//        /// The unique Identifier for this timer, used to identify what the timer is in the reports
//        /// </summary>
//        [XmlAttribute("name")]
//        public string Name { get; set; }

//        /// <summary>
//        /// The group that this timer belongs to, useful for stopping multiple timers at once.
//        /// </summary>
//        [XmlAttribute("group")]
//        public string Group { get; set; }

//        protected override Composite CreateBehavior()
//        {
//            var quest = ZetaDia.Storage.Quests.AllQuests.FirstOrDefault(q => q.QuestSNO == QuestId);

//            return new Sequence(
//                new Action(ret => TimeTracker.Start(new Timing
//                {
//                    Name = Name,
//                    StartTime = DateTime.UtcNow,
//                    Group = Group,
//                })),
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
