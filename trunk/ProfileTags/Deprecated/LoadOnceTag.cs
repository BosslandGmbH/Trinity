//using System;
//using Trinity.Framework;
//using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    // Thanks Nesox for the XML loading tricks!
//    [XmlElement("TrinityLoadOnce")]
//    [XmlElement("LoadOnce")]
//    public class LoadOnceTag : ProfileBehavior
//    {
//        internal static int LastUpdate = DateTime.UtcNow.Ticks.GetHashCode();
//        internal static List<string> UsedProfiles = new List<string>();
//        internal string[] AvailableProfiles
//        {
//            get
//            {
//                return
//                    (from p in Profiles
//                     where !UsedProfiles.Contains(p.FileName) && !p.FileName.Contains(_currentProfileName)
//                     select p.FileName).ToArray();
//            }

//        }
//        internal int UsedProfileCount
//        {
//            get
//            {
//                return (from p in Profiles
//                        where UsedProfiles.Contains(Path.GetFileName(p.FileName))
//                        select p).Count();
//            }
//        }

//        internal int UnusedProfileCount
//        {
//            get
//            {
//                return AvailableProfiles.Count();
//            }
//        }

//        string _nextProfileName = String.Empty;
//        string _nextProfilePath = String.Empty;
//        string _currentProfilePath = String.Empty;
//        string _currentProfileName = String.Empty;
//        readonly Random _rand = new Random();
//        bool _initialized;
//        private bool _isDone;

//        public override bool IsDone
//        {
//            get { return !IsActiveQuestStep || _isDone; }
//        }

//        [XmlElement("ProfileList")]
//        public List<LoadProfileOnce> Profiles { get; set; }

//        [XmlAttribute("noRandom")]
//        public bool NoRandom { get; set; }

//        public LoadOnceTag()
//        {
//            if (Profiles == null)
//                Profiles = new List<LoadProfileOnce>();
//            else if (!Profiles.Any())
//                Profiles = new List<LoadProfileOnce>();

//            GameEvents.OnGameJoined += TrinityLoadOnce_OnGameJoined;

//        }

//        ~LoadOnceTag()
//        {
//            GameEvents.OnGameJoined -= TrinityLoadOnce_OnGameJoined;
//        }

//        static void TrinityLoadOnce_OnGameJoined(object sender, EventArgs e)
//        {
//            UsedProfiles = new List<string>();
//        }

//        public override void OnStart()
//        {
//            Initialize();
//        }

//        private void Initialize()
//        {
//            if (_initialized)
//                return;

//            if (Profiles == null)
//                Profiles = new List<LoadProfileOnce>();

//            RealignFileNames();

//            _currentProfilePath = Path.GetDirectoryName(ProfileManager.CurrentProfile.Path);
//            _currentProfileName = Path.GetFileName(ProfileManager.CurrentProfile.Path);

//            _initialized = true;
//        }

//        // Re-align filenames with A, B, n, etc
//        public void RealignFileNames()
//        {
//            // Seed random generator with same INT until new game
//            Random rand2 = new Random(LastUpdate);

//            foreach (LoadProfileOnce p in Profiles)
//            {
//                if (!p.FileName.Contains(';'))
//                    continue;
//                string[] profilesArray = p.FileName.Split(';');
//                p.FileName = profilesArray[rand2.Next(0, profilesArray.Length)];
//            }
//        }

//        internal static void RecordLoadOnceProfile()
//        {
//            string currentProfileFileName = Path.GetFileName(ProfileManager.CurrentProfile.Path);
//            if (!UsedProfiles.Contains(currentProfileFileName))
//            {
//                UsedProfiles.Add(currentProfileFileName);
//            }
//        }

//        protected override Composite CreateBehavior()
//        {
//            return new Sequence(
//                new Action(ret => Initialize()),
//                new Action(ret => Core.Logger.Log("LoadOnce: Found {0} Total Profiles, {1} Used Profiles, {2} Unused Profiles",
//                    Profiles.Count(), UsedProfileCount, UnusedProfileCount)),
//                new PrioritySelector(
//                    new Decorator(ret => AvailableProfiles.Length == 0,
//                        new Sequence(
//                            new Action(ret => Core.Logger.Log("LoadOnce: All available profiles have been used!", true)),
//                            new Action(ret => LastUpdate = DateTime.UtcNow.Ticks.GetHashCode()),
//                            new Action(ret => _isDone = true)
//                        )
//                    ),
//                    new Decorator(ret => AvailableProfiles.Length > 0,
//                        new Sequence(
//                            new PrioritySelector(
//                                new Decorator(ret => NoRandom,
//                                    new Action(ret => _nextProfileName = AvailableProfiles[0])
//                                ),
//                                new Action(ret => _nextProfileName = AvailableProfiles[_rand.Next(0, AvailableProfiles.Length)])
//                            ),
//                            new Action(ret => _nextProfilePath = Path.Combine(_currentProfilePath, _nextProfileName)),
//                            new PrioritySelector(
//                                new Decorator(ret => File.Exists(_nextProfilePath),
//                                    new Sequence(
//                                        new Action(ret => Core.Logger.Log("LoadOnce: Loading next profile: {0}", _nextProfileName)),
//                                        new Action(ret => UsedProfiles.Add(_nextProfileName)),
//                                        new Action(ret => ProfileManager.Load(_nextProfilePath))
//                                    )
//                                ),
//                                new Action(ret => Core.Logger.Log("LoadOnce: ERROR: Profile {0} does not exist!", _nextProfilePath))
//                            )
//                        )
//                    ),
//                    new Action(ret => Core.Logger.Log("LoadOnce: Unkown error", true))
//                )
//           );
//        }
//        public override void ResetCachedDone()
//        {
//            _initialized = false;
//            _isDone = false;
//            base.ResetCachedDone();
//        }
//    }

//    [XmlElement("LoadProfileOnce")]
//    public class LoadProfileOnce
//    {
//        [XmlAttribute("filename")]
//        [XmlAttribute("Filename")]
//        [XmlAttribute("FileName")]
//        [XmlAttribute("fileName")]
//        [XmlAttribute("profile")]
//        public string FileName { get; set; }

//        public LoadProfileOnce(string filename)
//        {
//            FileName = filename;
//        }

//        public LoadProfileOnce()
//        {

//        }

//        public override string ToString()
//        {
//            return FileName;
//        }
//    }

//}
