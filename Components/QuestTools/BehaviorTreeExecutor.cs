using System;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Game;
using Zeta.TreeSharp;

namespace Trinity.Components.QuestTools
{
    public static partial class BotBehaviorQueue
    {
        public static class BehaviorTreeExecutor
        {
            private static Composite _logic;

            public static void CreateBotThread()
            {
                Core.Logger.Warn("脱机新的Bot线程");

                var isStarted = false;

                Worker.Start(() =>
                {
                    OnPulse(null, null);

                    if (_active == null)
                    {
                        Core.Logger.Log("Active 为空");

                        if (!Q.Any())
                        {
                            Core.Logger.Log("队列是空的");
                            return true;
                        }

                        var next = Q.First();
                        _active = next;
                        Q.Remove(next);

                        return false;
                    }

                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        if (!isStarted)
                        {
                            _active.Nodes.ForEach(n => n.Run());

                            _logic = new Sequence(_active.Nodes.Select(c => c.Behavior).ToArray());

                            _logic.Start(null);

                            isStarted = true;

                            Core.Logger.Log("启动节点");
                        }

                        Tick();
                    }

                    if (_active.Nodes.All(b => b.IsDone))
                    {
                        _active = null;
                    }

                    if (BotMain.IsRunning || !Q.Any() && _active == null)
                    {
                        Core.Logger.Warn("工作完成，关闭Bot线程");
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
                    _logic.Tick(null);

                    Core.Logger.Log("勾选最后状态={0}", _logic.LastStatus);

                    if (_logic.LastStatus != RunStatus.Running)
                    {
                        _logic.Stop(null);
                        _logic.Start(null);
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Log("Exception in BehaviorTreeExecutor.Logic.Tick() - {0}", ex);
                    _logic.Stop(null);
                    _logic.Start(null);
                    throw;
                }
            }
        }

       

    }




}