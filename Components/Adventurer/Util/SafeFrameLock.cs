using System;
using System.Threading;
using GreyMagic;
using Trinity.Components.Adventurer.Game.Events;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Util
{
    public static class SafeFrameLock
    {
        public static SafeFrameLockExecutionResult ExecuteWithinFrameLock(Action action, bool updateActors = false)
        {
            var result = new SafeFrameLockExecutionResult { Success = true };
            FrameLock frameLock = null;
            var frameLockAcquired = false;

            // If UI thread (settings window) or others try to read memory while bot is running 
            // it can freeze the application, so they need to acquire soft framelock.
            var isNotMainThread = Thread.CurrentThread != BotMain.BotThread;
            if (isNotMainThread && ZetaDia.Memory.Executor.IsExecutingContinuously)
            {
                result.Success = false;   
                return result;
            }

            if (!BotEvents.IsBotRunning || isNotMainThread)
            {
                Logger.Verbose("Acquiring Framelock");
                frameLock = ZetaDia.Memory.AcquireFrame();
                if (updateActors) ZetaDia.Actors.Update();
                frameLockAcquired = true;
            }

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }
            finally
            {
                if (frameLockAcquired)
                {
                    Logger.Verbose("Releasing Framelock");
                    frameLock.Dispose();
                }
            }
            return result;
        }


    }

    public class SafeFrameLockExecutionResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }

    }
}
