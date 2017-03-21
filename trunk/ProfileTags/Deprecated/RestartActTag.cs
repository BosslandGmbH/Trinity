//using System.IO;
//using Trinity.Framework;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.Game;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("RestartAct")]
//    public class RestartActTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public RestartActTag() { }
//        private bool _isDone;
//        public override bool IsDone { get { return _isDone; } }

//        public override void OnStart()
//        {
//            Core.Logger.Log("RestartAct initialized");
//        }

//        protected override Composite CreateBehavior()
//        {
//            return
//            new Action(ret => ForceRestartAct());
//        }

//        private static RunStatus ForceRestartAct()
//        {
//            string restartActProfile = GetActName(ZetaDia.CurrentAct) + "_StartNew.xml";
//            Core.Logger.Log("[QuestTools] Restarting Act - loading {0}", restartActProfile);

//            string profilePath = Path.Combine(Path.GetDirectoryName(ProfileManager.CurrentProfile.Path), restartActProfile);
//            ProfileManager.Load(profilePath);

//            return RunStatus.Success;
//        }

//        private static string GetActName(Act act)
//        {
//            switch (act)
//            {
//                case Act.A1: return "Act1";
//                case Act.A2: return "Act2";
//                case Act.A3: return "Act3";
//                case Act.A4: return "Act4";
//                case Act.A5: return "Act5";
//                default:
//                    Core.Logger.Log("Unkown Act passed for ReloadProfileTag: " + act);
//                    return "Act1";
//            }
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
