using System;
using System.Collections.Generic;
using System.IO;
using Trinity.Combat.Abilities;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Profile;

namespace Trinity
{
    public class UsedProfileManager
    {
        private static List<string> UsedProfiles = new List<string>();

        internal static void RecordProfile()
        {
            using (new PerformanceLogger("RecordProfile"))
            {
                try
                {
                    SetProfileInWindowTitle();

                    string currentProfile = ProfileManager.CurrentProfile.Path;

                    if (!UsedProfiles.Contains(currentProfile))
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.UserInformation, "New profile found - updating TargetBlacklists");
                        RefreshProfileBlacklists();
                        UsedProfiles.Add(currentProfile);
                    }

                    if (currentProfile != TrinityPlugin.CurrentProfile)
                    {
                        CombatBase.IsQuestingMode = false;

                        // See if we appear to have started a new game
                        if (TrinityPlugin.FirstProfile != "" && currentProfile == TrinityPlugin.FirstProfile)
                        {
                            TrinityPlugin.TotalProfileRecycles++;
                        }

                        TrinityPlugin.ProfileHistory.Add(currentProfile);
                        TrinityPlugin.CurrentProfile = currentProfile;
                        TrinityPlugin.CurrentProfileName = ProfileManager.CurrentProfile.Name;

                        if (TrinityPlugin.FirstProfile == "")
                            TrinityPlugin.FirstProfile = currentProfile;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error recording new profile: " + ex.ToString());
                }
            }
        }

        private static DateTime _lastProfileCheck = DateTime.MinValue;
        internal static void SetProfileInWindowTitle()
        {
            if (DateTime.UtcNow.Subtract(_lastProfileCheck).TotalMilliseconds < 1000)
                return;

            _lastProfileCheck = DateTime.UtcNow;
            
            if (ProfileManager.CurrentProfile != null)
            {
                string fileName = Path.GetFileName(ProfileManager.CurrentProfile.Path);

                if (ProfileManager.CurrentProfile != null && ProfileManager.CurrentProfile.Name != null)
                {
                    TrinityPlugin.SetWindowTitle(TrinityPlugin.CurrentProfileName + " " + fileName);
                }
                else if (ProfileManager.CurrentProfile != null && string.IsNullOrWhiteSpace(ProfileManager.CurrentProfile.Name))
                {
                    TrinityPlugin.SetWindowTitle(fileName);
                }
            }
        }
        
        /// <summary>
        /// Adds profile blacklist entries to the TrinityPlugin Blacklist
        /// </summary>
        internal static void RefreshProfileBlacklists()
        {
            try
            {
                if (ProfileManager.CurrentProfile != null && ProfileManager.CurrentProfile.TargetBlacklists != null)
                {
                    foreach (TargetBlacklist b in ProfileManager.CurrentProfile.TargetBlacklists)
                    {
                        if (!DataDictionary.BlackListIds.Contains(b.ActorId))
                        {
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.UserInformation, "Adding Profile TargetBlacklist {0} to TrinityPlugin Blacklist", b.ActorId);
                            DataDictionary.AddToBlacklist(b.ActorId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in Refreshing Profile Blacklists: " + ex.ToString());
            }
        }
    }
}
