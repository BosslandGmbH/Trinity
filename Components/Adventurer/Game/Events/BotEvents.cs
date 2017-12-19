using System.Threading;
using Zeta.Bot;

namespace Trinity.Components.Adventurer.Game.Events
{
    public class BotEvents
    {
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