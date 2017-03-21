//using System.ComponentModel;
//using System.Threading.Tasks;
//using Trinity.Framework;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("TrinitySetQuesting")]
//    [XmlElement("SetQuesting")]
//    public class SetQuestingTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public SetQuestingTag() { }
//        private bool _isDone;

//        public override bool IsDone
//        {
//            get { return _isDone; }
//        }

//        [XmlAttribute("mode")]
//        [DefaultValue(true)]
//        public bool Mode { get; set; }

//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ret => SetQuestingTask());
//        }

//        public async Task<bool> SetQuestingTask()
//        {
//            Core.Logger.Error($"{GetType().Name} is deprecated, please remove it from your profile", Mode);
//            _isDone = true;
//            return true;
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
