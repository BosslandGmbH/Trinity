using System.Threading;
using Zeta.Bot;

namespace Trinity.Components.Adventurer.Game.Events
{
    public class BotEvents
    {
        internal static void WireUp()
        {
            GameEvents.OnGameJoined += PluginEvents.GameEvents_OnGameJoined;
            GameEvents.OnWorldChanged += PluginEvents.GameEvents_OnWorldChanged;
        }

        internal static void UnWire()
        {
            GameEvents.OnGameJoined -= PluginEvents.GameEvents_OnGameJoined;
            GameEvents.OnWorldChanged -= PluginEvents.GameEvents_OnWorldChanged;
        }

        public static bool IsBotRunning
        {
            get
            {
                if (BotMain.BotThread == null) return false;
                if (BotMain.BotThread.IsAlive) return true;
                if (BotMain.BotThread.ThreadState.HasFlag(ThreadState.AbortRequested)) return true;
                if (BotMain.BotThread.ThreadState.HasFlag(ThreadState.WaitSleepJoin)) return true;
                if (BotMain.BotThread.ThreadState.HasFlag(ThreadState.Aborted)) return false;
                if (BotMain.BotThread.ThreadState.HasFlag(ThreadState.Background)) return true;
                return false;
            }
        }
    }
}