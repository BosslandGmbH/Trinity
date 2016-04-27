using System;
using System.Diagnostics;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.TreeSharp;

namespace Trinity.Helpers
{
    public class GoldInactivity
    {
        private long _lastGoldAmount;
        private DateTime _lastCheckBag = DateTime.MinValue;
        private DateTime _lastFoundGold = DateTime.MinValue;

        private static GoldInactivity _instance;
        public static GoldInactivity Instance { get { return _instance ?? (_instance = new GoldInactivity()); } }

        /// <summary>
        /// Resets the gold inactivity timer
        /// </summary>
        internal void ResetCheckGold()
        {
            Logger.LogDebug(LogCategory.GlobalHandler, "Resetting Gold Timer, Last gold changed from {0} to {1}", _lastGoldAmount, Trinity.Player.Coinage);

            _lastCheckBag = DateTime.UtcNow;
            _lastFoundGold = DateTime.UtcNow;
            _lastGoldAmount = 0;
        }

        private const int CheckGoldSeconds = 5;

        /// <summary>
        /// Determines whether or not to leave the game based on the gold inactivity timer
        /// </summary>
        /// <returns></returns>
        internal bool GoldInactive()
        {
            if (Trinity.Player.ParticipatingInTieredLootRun)
                return false;

            if (Trinity.Settings.Advanced.DisableAllMovement)
                return false;

            if (!Trinity.Settings.Advanced.GoldInactivityEnabled)
            {
                // timer isn't enabled so move along!
                //ResetCheckGold();
                return false;
            }
            try
            {
                if (!ZetaDia.IsInGame)
                {
                    //Logger.Log("Not in game, gold inactivity reset", 0);
                    ResetCheckGold(); //If not in game, reset the timer
                    return false;
                }
                if (ZetaDia.IsLoadingWorld)
                {
                    Logger.Log("Loading world, gold inactivity reset");
                    return false;
                }

                if ((DateTime.UtcNow.Subtract(_lastCheckBag).TotalSeconds < CheckGoldSeconds))
                {
                    return false;
                }
                _lastCheckBag = DateTime.UtcNow;

                // sometimes bosses take a LONG time
                if (Trinity.CurrentTarget != null && Trinity.CurrentTarget.IsBoss)
                {
                    Logger.Log("Current target is boss, gold inactivity reset");
                    ResetCheckGold();
                    return false;
                }

                if (Trinity.Player.Coinage != _lastGoldAmount && Trinity.Player.Coinage != 0)
                {
                    Logger.LogVerbose(LogCategory.GlobalHandler, "Gold Changed from {0} to {1}", _lastGoldAmount, Trinity.Player.Coinage);
                    _lastFoundGold = DateTime.UtcNow;
                    _lastGoldAmount = Trinity.Player.Coinage;
                }

                int goldUnchangedSeconds = Convert.ToInt32(DateTime.UtcNow.Subtract(_lastFoundGold).TotalSeconds);
                if (goldUnchangedSeconds >= Trinity.Settings.Advanced.InactivityTimer)
                {
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Gold inactivity after {0}s. (Setting={1}) Sending abort. ", goldUnchangedSeconds, Trinity.Settings.Advanced.InactivityTimer);
                    _lastFoundGold = DateTime.UtcNow;
                    _lastGoldAmount = Trinity.Player.Coinage;
                    return true;
                }
                if (goldUnchangedSeconds > 0)
                {
                    Logger.Log(LogCategory.GlobalHandler, "Gold unchanged for {0}s", goldUnchangedSeconds);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogCategory.GlobalHandler, "Error in GoldInactivity: " + e.Message);
            }

            return false;
        }

        private readonly Stopwatch _leaveGameTimer = new Stopwatch();

        internal static Composite CreateGoldInactiveLeaveGame()
        {
            return new Decorator(ret => Instance.GoldInactive(), CommonBehaviors.LeaveGame(ret => "Gold Inactivity Tripped"));
        }

    }
}
