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
                Core.Logger.Warn("Spooling up new Bot thread");

                var isStarted = false;

                Worker.Start(() =>
                {
                    OnPulse(null, null);

                    if (_active == null)
                    {
                        Core.Logger.Log("Active is null");

                        if (!Q.Any())
                        {
                            Core.Logger.Log("Queue is empty");
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

                            Core.Logger.Log("Starting Nodes");
                        }

                        Tick();
                    }

                    if (_active.Nodes.All(b => b.IsDone))
                    {
                        _active = null;
                    }

                    if (BotMain.IsRunning || !Q.Any() && _active == null)
                    {
                        Core.Logger.Warn("Work is finished, Shutting down Bot thread");
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

                    Core.Logger.Log("Tick LastStatus={0}", _logic.LastStatus);

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