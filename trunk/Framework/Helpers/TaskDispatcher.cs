using System;
using System.Threading;
using System.Threading.Tasks;
using Zeta.Bot;
using Zeta.TreeSharp;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// Runs a Task<T> as a Buddy Coroutine (via TreeSharp Composite) in a new thread.    
    /// </summary>
    public class TaskDispatcher
    {
        private static Composite _logic;

        public static int TickDelayMin = 5;
        public static int TickDelayMax = 25;

        public static void Start<T>(Func<object, Task<T>> task, Func<object,bool> stopCondition = null)
        {
            var isStarted = false;

            Worker.Start(() =>
            {                   
                using (new AquireFrameHelper())
                {
                    try
                    {                        
                        if (!isStarted)
                        {
                            Logger.Log("[TaskDispatcher] Starting Task, thread={0}", Thread.CurrentThread.ManagedThreadId);
                            _logic = new ActionRunCoroutine(task);
                            _logic.Start(null);
                            isStarted = true;
                        }
                        Tick();
                    }
                    catch (InvalidOperationException ex)
                    {                            
                        Logger.LogDebug("[TaskDispatcher] Exception: {0}", ex);
                    }
                }

                if (stopCondition != null && _logic != null)
                {
                    if (stopCondition.Invoke(_logic?.LastStatus))
                    {
                        Logger.Log("[TaskDispatcher] Finished Task, thread={0} (Condition)", Thread.CurrentThread.ManagedThreadId);
                        return true;
                    }
                }
                else
                {
                    if (_logic?.LastStatus != RunStatus.Running)
                    {
                        Logger.Log($"[TaskDispatcher] Finished Task, thread={Thread.CurrentThread.ManagedThreadId} (FinalResult={_logic?.LastStatus})");
                        return true;
                    }
                }
             
                Thread.Sleep(Components.Adventurer.Util.Randomizer.GetRandomNumber(TickDelayMin, TickDelayMax));
                return false;
            });
        }

        private static void Tick()
        {
            try
            {
                _logic.Tick(null);
                if (_logic.LastStatus != RunStatus.Running)
                {
                    _logic.Stop(null);
                    _logic.Start(null);
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug("[TaskDispatcher] Exception in Tick: {0}", ex);
                _logic.Stop(null);
                _logic.Start(null);
                throw;
            }
        }
    }
}
