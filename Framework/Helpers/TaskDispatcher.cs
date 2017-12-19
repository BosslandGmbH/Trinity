using System;
using System.Threading;
using System.Threading.Tasks;
using Zeta.Bot;
using Zeta.Game;
using Zeta.TreeSharp;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// Runs a Task<T> as a Buddy Coroutine (via TreeSharp Composite) in a new thread.
    /// </summary>
    public class TaskDispatcher
    {
        private static Composite _logic;

        public static int TickDelayMin = 25;
        public static int TickDelayMax = 75;

        public static void Start<T>(Func<object, Task<T>> task, Func<object, bool> stopCondition = null)
        {
            var isStarted = false;

            Worker.Start(() =>
            {
                using (ZetaDia.Memory.AcquireFrame())
                {
                    try
                    {
                        if (!BotMain.IsRunning)
                        {
                            ZetaDia.Actors.Update();
                            Core.Update();
                        }

                        if (!isStarted)
                        {
                            Core.Logger.Log("[任务调度程序] 启动任务, 线程={0}", Thread.CurrentThread.ManagedThreadId);                            
                            _logic = new ActionRunCoroutine(task);
                            _logic.Start(null);
                            isStarted = true;
                        }
                        Tick();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Core.Logger.Debug("[任务调度程序] 异常: {0}", ex);
                    }
                }

                if (stopCondition != null && _logic != null)
                {
                    if (stopCondition.Invoke(_logic?.LastStatus))
                    {
                        Core.Logger.Log("[任务调度程序] 完成任务, 线程={0} (条件)", Thread.CurrentThread.ManagedThreadId);
                        return true;
                    }
                }
                else
                {
                    if (_logic?.LastStatus != RunStatus.Running)
                    {
                        Core.Logger.Log($"[任务调度程序] 完成任务, 线程={Thread.CurrentThread.ManagedThreadId} (最后结果={_logic?.LastStatus})");
                        return true;
                    }
                }

                Thread.Sleep(Randomizer.Random(TickDelayMin, TickDelayMax));
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
                Core.Logger.Debug("[任务调度程序] 异常在滴答: {0}", ex);
                _logic.Stop(null);
                _logic.Start(null);
                throw;
            }
        }
    }
}