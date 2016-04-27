using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Trinity.Technicals;
using Trinity.Helpers;
using Trinity.Items;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Game;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace Trinity.Helpers
{
    class TaskDispatcher
    {
            private static Composite _logic;

            public static void Start(Func<Object,Task<bool>> task, Func<object,bool> stopCondition = null)
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
                                Logger.Log("Starting new Bot Thread Id={0}", Thread.CurrentThread.ManagedThreadId);
                                _logic = new ActionRunCoroutine(task);
                                _logic.Start(null);
                                isStarted = true;
                            }
                            Tick();
                        }
                        catch (InvalidOperationException ex)
                        {                            
                            Logger.LogDebug("CheckInCoroutine() Derp {0}", ex);
                        }

                    }

                    if (stopCondition != null && _logic != null && stopCondition.Invoke(_logic?.LastStatus))
                    {
                        Logger.Log("Bot Thread Finished Id={0}", Thread.CurrentThread.ManagedThreadId);
                        return true;
                    }
                        
                    return false;
                });
            }

            private static void Tick()
            {
                try
                {
                    _logic.Tick(null);

                    Logger.Log("Tick LastStatus={0}", _logic.LastStatus);

                    if (_logic.LastStatus != RunStatus.Running)
                    {
                        _logic.Stop(null);
                        _logic.Start(null);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception in TaskDispatcher.Logic.Tick() - {0}", ex);
                    _logic.Stop(null);
                    _logic.Start(null);
                    throw;
                }
            }
        }
}
