//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Trinity.Components.QuestTools.Helpers;
//using Zeta.Bot;
//using Zeta.Bot.Navigation;
//using Zeta.Bot.Profile;
//using Zeta.Game;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    /// <summary>
//    /// Reloads the current profile, and optionally restarts the act quest if the profile has been reloaded too many times.
//    /// </summary>
//    [XmlElement("ReloadProfile")]
//    public class ReloadProfileTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public ReloadProfileTag() { }
//        private bool _isDone;
//        public override bool IsDone
//        {
//            get { return _isDone; }
//        }

//        [XmlAttribute("force")]
//        public bool Force { get; set; }

//        public Zeta.Game.Internals.Quest CurrentQuest { get { return ZetaDia.CurrentQuest; } }

//        private static string _lastReloadLoopQuestStep = "";

//        /// <summary>
//        /// Gets or sets the last reload loop quest step.
//        /// </summary>
//        /// <value>
//        /// The last reload loop quest step.
//        /// </value>
//        internal static string LastReloadLoopQuestStep
//        {
//            get { return _lastReloadLoopQuestStep; }
//            set { _lastReloadLoopQuestStep = value; }
//        }

//        internal static int QuestStepReloadLoops { get; set; }

//        string _currProfile = "";

//        /// <summary>
//        /// Initializes the <see cref="ReloadProfileTag"/> class.
//        /// </summary>
//        static ReloadProfileTag()
//        {
//            QuestStepReloadLoops = 0;
//        }

//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ret => MainCoroutine());
//        }

//        /// <summary>
//        /// The main Coroutine
//        /// </summary>
//        /// <returns></returns>
//        private async Task<bool> MainCoroutine()
//        {
//            if (DateTime.UtcNow.Subtract(BotEvents.LastProfileReload).TotalSeconds < 2)
//            {
//                Core.Logger.Log("Profile loading loop detected, counted {0} reloads", QuestStepReloadLoops);
//                _isDone = true;
//                return true;
//            }

//            if (ZetaDia.IsInGame && ZetaDia.Me.IsValid)
//            {
//                _currProfile = ProfileManager.CurrentProfile.Path;
//                Core.Logger.Log("Reloading profile {0} {1}", _currProfile, QuestInfo());
//                CountReloads();
//                BotEvents.LastProfileReload = DateTime.UtcNow;
//                ProfileManager.Load(_currProfile);
//                Navigator.Clear();

//                return true;
//            }

//            return true;
//        }

//        /// <summary>
//        /// Reloads the ActX_Start.xml profile
//        /// </summary>
//        /// <returns></returns>
//        private void ForceRestartAct()
//        {
//            Regex questingProfileName = new Regex(@"Act \d by rrrix");

//            if (!questingProfileName.IsMatch(ProfileManager.CurrentProfile.Name))
//                return;

//            string restartActProfile = String.Format("{0}_StartNew.xml", GetActName(ZetaDia.CurrentAct));
//            Core.Logger.Log("[QuestTools] Max Profile reloads reached, restarting Act! Loading Profile {0} - {1}", restartActProfile, QuestInfo());

//            string profilePath = Path.Combine(Path.GetDirectoryName(ProfileManager.CurrentProfile.Path), restartActProfile);
//            ProfileManager.Load(profilePath);
//        }

//        private string GetActName(Act act)
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

//        /// <summary>
//        /// Counts the reloads.
//        /// </summary>
//        private void CountReloads()
//        {
//            // if this is the first time reloading this quest and step, set reload loops to zero
//            string questId = QuestId + "_" + StepId;
//            if (questId != LastReloadLoopQuestStep)
//            {
//                QuestStepReloadLoops = 0;
//            }

//            // increment ReloadLoops 
//            QuestStepReloadLoops++;

//            // record this quest Id and step Id
//            LastReloadLoopQuestStep = questId;
//        }

//        /// <summary>
//        /// Quests the information.
//        /// </summary>
//        /// <returns></returns>
//        private string QuestInfo()
//        {
//            return String.Format(
//                "Act=\"{0}\" questId=\"{1}\" stepId=\"{2}\" levelAreaId=\"{3}\" worldId={4}",
//                ZetaDia.CurrentAct,
//                CurrentQuest.QuestSnoId,
//                CurrentQuest.StepId,
//                ZetaDia.CurrentLevelAreaSnoId,
//                ZetaDia.Globals.WorldSnoId
//                );
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
