//using System.Threading.Tasks;
//using Trinity.Framework;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("CombatSetting")]
//    class CombatSettingTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        private bool _isDone;
//        public override bool IsDone => _isDone;

//        [XmlAttribute("trashPackSize")]
//        public int TrashPackSize { get; set; }

//        [XmlAttribute("nonEliteRange")]
//        [XmlAttribute("killRadius")]
//        public int NonEliteRange { get; set; }

//        [XmlAttribute("trashPackClusterRadius")]
//        public float TrashPackClusterRadius { get; set; }

//        [XmlAttribute("combat")]
//        public string Combat { get; set; }

//        [XmlAttribute("looting")]
//        public string Looting { get; set; }


//        [XmlAttribute("lootRadius")]
//        public int LootRadius { get; set; }

//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ret => CombatSettingTask());
//        }

//        private async Task<bool> CombatSettingTask()
//        {
//            Core.Logger.Error("CombatSettingTag is deprecated, please remove it from your profile");
//            _isDone = true;
//            return true;
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

