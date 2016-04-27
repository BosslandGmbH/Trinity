using System;
using System.Diagnostics;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.TreeSharp;

namespace Trinity.Helpers
{
    public class XpInactivity
    {
        private Int64 _lastXpAmount;
        private DateTime _lastCheckBag = DateTime.MinValue;
        private DateTime _lastFoundXp = DateTime.MinValue;

        private static XpInactivity _instance;
        public static XpInactivity Instance { get { return _instance ?? (_instance = new XpInactivity()); } }

        /// <summary>
        /// Resets the Experience inactivity timer
        /// </summary>
        internal void ResetCheckXp()
        {
            if (Trinity.Settings.Advanced.XpInactivityEnabled)
                Logger.LogDebug(LogCategory.GlobalHandler, "Resetting Experience Timer, Last Experience changed from {0} to {1}", _lastXpAmount, Trinity.Player.Coinage);

            _lastCheckBag = DateTime.UtcNow;
            _lastFoundXp = DateTime.UtcNow;
            _lastXpAmount = 0;
        }

        private const int CheckXpSeconds = 5;

        private bool _inBossFight;

        /// <summary>
        /// Determines whether or not to leave the game based on the Experience inactivity timer
        /// </summary>
        /// <returns></returns>
        internal bool XpInactive()
        {
            if (!Trinity.Settings.Advanced.XpInactivityEnabled)
            {
                // these are not the inactivity timers we're looking for
                return false;
            }

            if (Trinity.Settings.Advanced.DisableAllMovement)
                return false;

            try
            {
                if (!ZetaDia.IsInGame)
                {
                    //Logger.Log("Not in game, XP inactivity reset", 0);
                    ResetCheckXp(); //If not in game, reset the timer
                    return false;
                }
                if (ZetaDia.IsLoadingWorld)
                {
                    Logger.Log("Loading world, XP inactivity reset");
                    return false;
                }

                if ((DateTime.UtcNow.Subtract(_lastCheckBag).TotalSeconds < CheckXpSeconds))
                {
                    return false;
                }
                _lastCheckBag = DateTime.UtcNow;
         
                var inBossFight = ZetaDia.Me.IsInBossEncounter;
                if (inBossFight != _inBossFight)
                {
                    _inBossFight = inBossFight;

                    if (inBossFight)
                    {
                        Logger.Log("Started a boss encounter, XP inactivity reset");
                        ResetCheckXp();
                        return false;
                    }
                }

                Int64 exp;
                if (Trinity.Player.Level < 70)
                    exp = ZetaDia.Me.CurrentExperience;
                else
                    exp = ZetaDia.Me.ParagonCurrentExperience;

                if (exp != _lastXpAmount && exp != 0)
                {
                    Logger.LogVerbose(LogCategory.GlobalHandler, "Experience Changed from {0} to {1}", _lastXpAmount, exp);
                    _lastFoundXp = DateTime.UtcNow;
                    _lastXpAmount = exp;
                }

                int xpUnchangedSeconds = Convert.ToInt32(DateTime.UtcNow.Subtract(_lastFoundXp).TotalSeconds);
                if (xpUnchangedSeconds >= Trinity.Settings.Advanced.InactivityTimer)
                {
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Experience inactivity after {0}s. Sending abort.", xpUnchangedSeconds);
                    _lastFoundXp = DateTime.UtcNow;
                    _lastXpAmount = Trinity.Player.Coinage;
                    return true;
                }
                if (xpUnchangedSeconds > 0)
                {
                    Logger.Log(LogCategory.GlobalHandler, "Experience unchanged for {0}s", xpUnchangedSeconds);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogCategory.GlobalHandler, "Error in XpInactivity: " + e.Message);
            }

            return false;
        }

        private readonly Stopwatch _leaveGameTimer = new Stopwatch();

        internal static Composite CreateXpInactiveLeaveGame()
        {
            return new Decorator(ret => Instance.XpInactive(), CommonBehaviors.LeaveGame(ret => "Experience Inactivity Tripped"));
        }

    }
}
