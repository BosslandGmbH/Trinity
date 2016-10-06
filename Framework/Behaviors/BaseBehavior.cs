using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Framework.Behaviors
{
    public class BaseBehavior
    {
        /*
            Once condition is true, the start task runs.
            While condition remains true, the work task runs.
            Once condition is false or Timeout, the stop task runs.
        */

        public string Name => GetType().Name;
        public bool IsRunning;
        public DateTime Timeout;

        public static BaseBehavior CurrentBehavior { get; private set; }

        protected async Task<bool> Run(Func<Task<bool>> conditionProducer, Func<Task<bool>> actionProducer, int timeoutMs)
        {
            try
            {
                if (conditionProducer == null) return false;
                if (actionProducer == null) return false;

                if (!await conditionProducer())
                {
                    if (IsRunning)
                        await Stop();

                    return false;
                }

                if (!IsRunning)
                {
                    await Start(timeoutMs);
                }
                else if (DateTime.UtcNow > Timeout)
                {
                    await Stop();
                    return false;
                }

                CurrentBehavior = this;
                await actionProducer();
                return true;
            }
            catch (CoroutineUnhandledException ex)
            {
                Logger.Error($"{Name} Exception {ex} {Environment.StackTrace}");
                throw;
            }
        }

        protected virtual async Task<bool> OnStopped() => true;
        protected virtual async Task<bool> OnStarted() => true;

        protected async Task<bool> Stop()
        {
            await OnStopped();
            IsRunning = false;
            Timeout = DateTime.MinValue;
            return true;
        }

        protected async Task<bool> Start(int timeoutMs)
        {
            IsRunning = true;
            Timeout = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            await OnStarted();
            return true;
        }

        public override string ToString() => $"{Name} ";
    }
}
