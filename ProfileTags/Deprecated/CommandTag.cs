//using System;
//using Trinity.Framework;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.Bot.Settings;
//using Zeta.Game;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    /// <summary>
//    /// Give commands for the bot to do unusual stuff
//    /// </summary>
//    [XmlElement("Command")]
//    public class CommandTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        private bool _isDone;

//        public enum CommandType
//        {
//            None = 0,
//            StopBot,
//            SetDifficulty,
//            LogSpecial
//        }

//        [XmlAttribute("value")]
//        public string Value { get; set; }
       
//        [XmlAttribute("name")]
//        public CommandType Name { get; set; }

//        [XmlAttribute("reason")]
//        public string Reason { get; set; }

//        public override bool IsDone
//        {
//            get { return QuestId > 1 && !IsActiveQuestStep || _isDone; }
//        }

//        protected override Composite CreateBehavior()
//        {
//            return new Action(ret =>
//            {
//                Core.Logger.Debug("Performing action {0}", Name);

//                var profileName = ProfileManager.CurrentProfile.Path;

//                switch (Name)
//                {
//                    case CommandType.SetDifficulty:

//                        GameDifficulty difficulty;
//                        if (Enum.TryParse(Value, true, out difficulty))
//                        {
//                            Core.Logger.Warn("Profile '{0}' changed difficulty to {1}. {2}", profileName, difficulty, Reason);
//                            CharacterSettings.Instance.GameDifficulty = difficulty;
//                        }
//                        break;

//                    case CommandType.StopBot:

//                        Core.Logger.Warn("Profile '{0}' requested the bot be stopped. {1}", profileName, Reason);
//                        BotMain.Stop();
//                        break;

//                    case CommandType.LogSpecial:
//                        break;

//                }

//                _isDone = true;
//                return RunStatus.Success;
//            });
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