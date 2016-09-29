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
        public Worker() { }

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
        public static void Start(Func<bool> worker, int waitTime = 50)
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
                Name = string.Format("Worker: {0}.{1}", ns, type),
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };

            Logger.LogDebug("Starting {0} Thread Id={1}", _thread.Name, _thread.ManagedThreadId);

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

                Logger.LogDebug("Shutting down thread");
               
                _thread.Abort(new { RequestingThreadId = Thread.CurrentThread.ManagedThreadId});
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

            Logger.LogDebug("Thread {0}: {1} Started", _thread.ManagedThreadId, _thread.Name);

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
                    Logger.LogDebug("Aborting Thread: {0}, StateInfo={1}", _thread.ManagedThreadId, ex.ExceptionState);
                    Thread.ResetAbort();                    
                }
                catch (Exception ex)
                {
                    Logger.Log("Error in Thread {0}: {1} {2}", _thread.ManagedThreadId, _thread.Name, ex);
                }
            }

            Logger.LogDebug("Thread {0}: {1} Finished", _thread.ManagedThreadId, _thread.Name);

            OnStopped.Invoke();
        }

    }
}
