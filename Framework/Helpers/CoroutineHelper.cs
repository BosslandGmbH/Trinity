using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Buddy.Coroutines;
using log4net;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.TreeSharp;
using Action = System.Action;

namespace Trinity.Framework.Helpers
{
    public static class CoroutineHelper
    {
        private static readonly ILog Logger = Zeta.Common.Logger.GetLoggerInstanceForType();
        private static Coroutine _updateCoroutine;
        public static List<Task> Tasks = new List<Task>();
        private static CancellationToken _token;
        private static CancellationTokenSource _tokenSource;

        static CoroutineHelper()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            BotMain.OnStop += ibot => CancelRunningTasks();
        }

        public static bool IsRunning
        {
            get
            {
                Tasks.RemoveAll(t => t.IsCompleted);
                return Tasks.Any(t => !t.IsCompleted);
            }
        }

        /// <summary>
        /// Runs a task as a coroutine in a new thread (if not running) or ticks current task as a coroutine.
        /// </summary>
        public static void RunCoroutine<T>(Func<Task<T>> taskProducer, Func<T, bool> stopCondition = null, int tickMilliseconds = 50)
        {
            if (BotMain.IsPaused)
                return;

            if (!BotMain.IsRunning || BotMain.IsPausedForStateExecution)
            {
                StartNew(taskProducer, stopCondition, null, tickMilliseconds);
            }
            else
            {
                var result = ToCoroutine(taskProducer, stopCondition).Result;
            }
        }

        /// <summary>
        /// Pauses the bot and runs a task as a coroutine in a new thread.
        /// </summary>
        public static void ForceRunCoroutine<T>(Func<Task<T>> task, Func<T, bool> stopCondition = null, int tickMilliseconds = 50)
        {
            if (BotMain.IsPaused || !ZetaDia.IsInGame)
                return;

            BotMain.IsPausedForStateExecution = true;
            StartNew(task, stopCondition, () =>
            {
                Core.Logger.Log("强制运行协程完成");
                BotMain.IsPausedForStateExecution = false;

            }, tickMilliseconds);
        }

        /// <summary>
        /// Starts a new Thread and ticks the provided taskProducer as a Coroutine until it is finished.
        /// </summary>
        private static Task StartNew<T>(Func<Task<T>> taskProducer, Func<T, bool> stopCondition, Action onFinished = null, int tickDelay = 50)
        {
            CancelRunningTasks();

            var task = Task.Run(async () =>
            {
                try
                {
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        if (!ZetaDia.IsInGame || !ZetaDia.Service.IsValid)
                            return;

                        ZetaDia.Actors.Update();
                        Core.Update();
                    }

                    var result = true;
                    while (result)
                    {
                        if (BotMain.IsPaused)
                            continue;

                        using (ZetaDia.Memory.AcquireFrame())
                        {
                            if (!ZetaDia.IsInGame || !ZetaDia.Service.IsValid)
                                return;

                            ZetaDia.Actors.Update();
                            Core.Update();

                            if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                                return;

                            result = await ToCoroutine(taskProducer, stopCondition);
                        }

                        await Task.Delay(tickDelay);

                        if (_token.IsCancellationRequested)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("CoroutineHelper.StartNew threw exception {0}", ex);
                }
                finally
                {
                    onFinished?.Invoke();
                }

            }, _token);

            RecordTask(task);
            return task;
        }

        private static void RecordTask(Task task)
        {
            Tasks.Add(task);
            Tasks.RemoveAll(t => t.IsCompleted);
        }

        /// <summary>
        /// Instructs all running tasks to cancel themselves.
        /// </summary>
        public static void CancelRunningTasks()
        {
            if (!Tasks.Any()) return;

            Core.Logger.Log($"发送取消请求 {Tasks.Count(t => t.Status == TaskStatus.Running)} 运行和 {Tasks.Count(t => t.Status != TaskStatus.Running)} 已完成的任务...");

            if (_tokenSource.Token.CanBeCanceled)
                _tokenSource.Cancel();

            Tasks.Clear();

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
        }

        /// <summary>
        /// Converts a task into an ActionRunCoroutine composite
        /// </summary>
        public static Composite ToComposite<T>(this Task<T> task)
        {
            return new ActionRunCoroutine(ret => task);
        }

        /// <summary>
        /// Converts a task producer to a coroutine, returns true if it should be repeated, false if its done.
        /// </summary>
        public static async Task<bool> ToCoroutine<T>(this Func<Task<T>> taskProducer, Func<T, bool> stopCondition = null)
        {
            if (taskProducer == null)
            {
                Logger.ErrorFormat("CoroutineHelper.ToCoroutine task cannot be null");
                return true;
            }

            if (_updateCoroutine == null || _updateCoroutine.IsFinished)
            {
                _updateCoroutine = new Coroutine(async () =>
                {
                    var result = await taskProducer();

                    if (stopCondition != null)
                        return stopCondition(result);

                    if (result is bool)
                        return result;

                    if (result is MoveResult && (MoveResult)(object)result == MoveResult.ReachedDestination)
                        return true;

                    return false;
                });
            }

            try
            {
                _updateCoroutine.Resume();
            }
            catch (CoroutineException ex)
            {
                Logger.ErrorFormat("CoroutineHelper.ToCoroutine update coroutine threw exception {0}", ex);
                return true;
            }

            if (!_updateCoroutine.IsFinished)
                return true; // Still updating

            if (_updateCoroutine.Status != CoroutineStatus.RanToCompletion)
            {
                Logger.ErrorFormat("CoroutineHelper.ToCoroutine update coroutine went into status {0}", _updateCoroutine.Status);
                return true;
            }

            if (!(bool)_updateCoroutine.Result)
                return true; // failed, so retry

            return false;
        }
    }
}