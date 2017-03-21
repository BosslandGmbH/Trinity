//using System.Collections.Generic;
//using Trinity.Framework;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("ProfileSetting")]
//    public class ProfileSettingTag : ProfileBehavior, IEnhancedProfileBehavior
//    { 
//        private bool _isDone; 
//        public override bool IsDone 
//        { 
//            get { return _isDone; } 
//        } 

//        [XmlAttribute("name")]	
//        public string Name { get; set; }

//        [XmlAttribute("value")]
//        public string Value { get; set; }

//        public static Dictionary<string,string> ProfileSettings = new Dictionary<string, string>();
//        public static bool Initialized;
//        public static void Initialize()
//        {
//            BotMain.OnStart += bot => ProfileSettings.Clear();
//            Initialized = true;
//        }

//        protected override Composite CreateBehavior() 
//        { 
//            return new Action(ret =>
//            {
//                if (!Initialized)
//                    Initialize();

//                if (ProfileSettings.ContainsKey(Name))
//                    ProfileSettings[Name] = Value;
//                else
//                    ProfileSettings.Add(Name,Value);

//                Core.Logger.Log("Setting Condition={0} to {1}", Name, Value);

//				_isDone = true;
//                return RunStatus.Failure;
//            }); 
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



