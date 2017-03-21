using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.QuestTools
{
    public static class ProfileHistory
    {
        public static Dictionary<DateTime, Profile> LoadedProfiles = new Dictionary<DateTime, Profile>();

        public static void Add(Profile profile)
        {
            LoadedProfiles.Add(DateTime.UtcNow, profile);

            if (LoadedProfiles.Count > 100)
                LoadedProfiles.Remove(LoadedProfiles.First().Key);
        }

        private static int _profileCount;
        private static Profile _lastProfile;        
        public static Profile LastProfile
        {
            get
            {
                if (_profileCount == LoadedProfiles.Count)
                    return _lastProfile;

                _lastProfile = null;
                _profileCount = LoadedProfiles.Count;

                if (LoadedProfiles.Count > 1)
                {

                    Core.Logger.Debug("ProfileCount = {0}", LoadedProfiles.Count);

                    for (int i = LoadedProfiles.Count - 1; i-- > 0;)
                    {
                        var profile = LoadedProfiles.ElementAt(i);

                        if (profile.Value.Path != ProfileManager.CurrentProfile.Path)
                        {
                            Core.Logger.Debug("Processing History Index={0} Name={1} SecondsSinceLoad={2}", i, profile.Value.Name, DateTime.UtcNow.Subtract(profile.Key).TotalSeconds);
                            _lastProfile = profile.Value;
                            break;
                        }
                    }

                }
                else if (LoadedProfiles.Count == 1)
                {
                    _lastProfile = LoadedProfiles.First().Value;
                }

                return _lastProfile;
            }
        }
    }
}
