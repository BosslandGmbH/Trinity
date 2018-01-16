using System;
using System.ComponentModel;
using System.IO;
using Trinity.Framework;
using Trinity.Components.QuestTools;
using Zeta.Bot;
using Zeta.Game;
using Zeta.XmlEngine;
using System.Threading.Tasks;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Game.Internals.Service;

namespace Trinity.ProfileTags
{
    [XmlElement("ReloadProfile")]
    public class ReloadProfileTag : BaseProfileBehavior
    {
        #region XmlAttributes

        [XmlAttribute("max")]
        [XmlAttribute("maxReloads")]
        [Description("Bot will restart game after this many reloads")]
        [DefaultValue(30)]
        public int MaxReloads { get; set; }

        [XmlAttribute("stop")]
        [XmlAttribute("stopAtMax")]
        [Description("Whether bot should be stopped instead of restarting")]
        [DefaultValue(false)]
        public bool StopAtMax { get; set; }

        [XmlAttribute("restart")]
        [XmlAttribute("restartGame")]
        [Description("Whether game should be restarted")]
        [DefaultValue(false)]
        public bool ShouldRestartGame { get; set; }

        #endregion

        public DateTime LastProfileReload { get; set; }
        internal static int QuestStepReloadLoops { get; set; }
        internal static string LastReloadLoopQuestStep { get; set; } = string.Empty;
        public static GameId LastGameId { get; set; }
        public string CurrentProfile { get; set; } = string.Empty;

        public override async Task<bool> MainTask()
        {
            ResetOnGameChange();

            if (ZetaDia.IsInGame && ZetaDia.Me.IsValid && QuestStepReloadLoops >= MaxReloads)
            {
                if (StopAtMax)
                {
                    Core.Logger.Log("*** Max Profile Reloads Threshold Breached *** ");
                    Core.Logger.Log("*** Profile restarts DISABLED *** ");
                    Core.Logger.Log("*** STOPPING BOT *** ");
                    BotMain.Stop();
                    return true;
                }
                await ReloadProfile(true);
                return true;
            }

            if (DateTime.UtcNow.Subtract(LastProfileReload).TotalSeconds < 2)
            {
                Core.Logger.Log("Profile loading loop detected, counted {0} reloads", QuestStepReloadLoops);
                return true;
            }

            if (ZetaDia.IsInGame && ZetaDia.Me.IsValid)
            {
                await ReloadProfile(ShouldRestartGame);
                return false;
            }

            return false;
        }

        private void ResetOnGameChange()
        {
            var gameId = ZetaDia.Service.CurrentGameId;
            if (!gameId.Equals(LastGameId))
            {
                LastGameId = gameId;
                OnGameChanged();
            }
        }

        public void OnGameChanged()
        {
            QuestStepReloadLoops = 0;
        }

        private async Task<bool> ReloadProfile(bool restart)
        {
            var path = Path.GetDirectoryName(ProfileManager.CurrentProfile.Path);
            if (path != null)
            {
                CurrentProfile = ProfileManager.CurrentProfile.Path;
                Core.Logger.Log($"Reloading profile {CurrentProfile} {QuestInfo()} Restart#={QuestStepReloadLoops}");
                var profilePath = Path.Combine(path, CurrentProfile);
                ProfileManager.Load(profilePath);
                Navigator.Clear();
                CountReloads();
                LastProfileReload = DateTime.UtcNow;

                if (restart)
                {
                    await CommonCoroutines.LeaveGame("ReloadProfileTag");
                }

                return true;
            }
            return false;
        }

        private void CountReloads()
        {
            var questId = QuestId + "_" + StepId;
            if (questId != LastReloadLoopQuestStep)
                QuestStepReloadLoops = 0;

            QuestStepReloadLoops++;
            LastReloadLoopQuestStep = questId;
        }

        private string QuestInfo()
        {
            return $"Act=\"{ZetaDia.CurrentAct}\" questId=\"{ZetaDia.CurrentQuest.QuestSnoId}\" stepId=\"{ZetaDia.CurrentQuest.StepId}\" levelAreaId=\"{ZetaDia.CurrentLevelAreaSnoId}\" worldId={ZetaDia.Globals.WorldSnoId}";
        }

    }
}
