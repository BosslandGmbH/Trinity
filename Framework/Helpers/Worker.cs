using System;
using System.Diagnostics;
using System.Threading;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// A worker thread. Use Worker.Start() with code to be executed.
    /// </summary>
    public class Worker
    {
        public Worker()
        {
        }

        private static Thread _thread;
        private static Func<bool> _worker;
        private static bool _working;
        public static int WaitTime;

        public delegate void WorkerEvent();

        public static event WorkerEvent OnStopped = () => { };

        public static event WorkerEvent OnStarted = () => { };

        public static bool IsRunning
        {
            get { return _thread != null && _thread.IsAlive; }
        }

        /// <summary>
        /// Run code in a new worker thread. WorkerDelegate should return true to end, false to repeat.
        /// </summary>
        /// <param name="worker">Delegate to be run</param>
        /// <param name="waitTime"></param>
        public static void Start(Func<bool> worker, int waitTime = 25)
        {
            if (IsRunning)
                return;

            WaitTime = waitTime;

            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            var ns = type != null ? type.Namespace : string.Empty;

            _worker = worker;
            _thread = new Thread(SafeWorkerDelegate)
            {
                Name = $"Worker: {ns}.{type}",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };

            Core.Logger.Debug("开始 {0} 线程 Id={1}", _thread.Name, _thread.ManagedThreadId);

            _working = true;
            _thread.Start();

            OnStarted.Invoke();
        }

        public static void Stop()
        {
            try
            {
                if (!IsRunning)
                    return;

                Core.Logger.Debug("关闭线程");

                _thread.Abort(new { RequestingThreadId = Thread.CurrentThread.ManagedThreadId });
            }
            catch (Exception)
            {
                _working = false;
            }
        }

        public static void SafeWorkerDelegate()
        {
            if (_thread == null)
                return;

            Core.Logger.Debug("线程 {0}: {1} 已经启动", _thread.ManagedThreadId, _thread.Name);

            while (_working)
            {
                try
                {
                    Thread.Sleep(Math.Max(1, WaitTime));

                    if (_worker == null)
                        continue;

                    if (_worker.Invoke())
                        _working = false;
                }
                catch (ThreadAbortException ex)
                {
                    _working = false;
                    Core.Logger.Debug("中止 线程: {0}, StateInfo={1}", _thread.ManagedThreadId, ex.ExceptionState);
                    Thread.ResetAbort();
                }
                catch (Exception ex)
                {
                    Core.Logger.Log("线程中的错误 {0}: {1} {2}", _thread.ManagedThreadId, _thread.Name, ex);
                }
            }

            Core.Logger.Debug("线程 {0}: {1} 完成", _thread.ManagedThreadId, _thread.Name);

            OnStopped.Invoke();
        }
    }
}