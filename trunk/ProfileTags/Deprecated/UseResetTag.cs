//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// Resets a UseOnce tag as if it has never been used
//    /// </summary>
//    [XmlElement("UseReset")]
//    [XmlElement("TrinityUseReset")]
//    public class UseResetTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public UseResetTag() { }
//        private bool _isDone = false;

//        public override bool IsDone { get { return _isDone; } }

//        protected override Composite CreateBehavior()
//        {
//            return new Action(ret => DoReset());
//        }

//        private void DoReset()
//        {
//            if (UseOnceTag.UseOnceIDs.Contains(ID))
//            {
//                UseOnceTag.UseOnceIDs.Remove(ID);
//                UseOnceTag.UseOnceCounter.Remove(ID);
//            }
//            _isDone = true;
//        }

//        [XmlAttribute("id")]
//        public string ID { get; set; }

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
