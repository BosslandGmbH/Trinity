using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Framework.Behaviors
{
    public class BaseBehavior
    {
        public string Name => GetType().Name;
        public bool IsRunning;
        public DateTime Timeout;

        public static BaseBehavior CurrentBehavior { get; private set; }

        protected async Task<bool> Run(Func<Task<bool>> conditionProducer, Func<Task<bool>> actionProducer, int timeoutMs)
        {
            var action = actionProducer?.Invoke();
            if (action == null) return false;

            var condition = conditionProducer?.Invoke();
            if (condition == null) return false;

            if (!await condition)
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
            await action;
            return true;
        }

        protected virtual async Task<bool> OnStopped() => true;
        protected virtual async Task<bool> OnStarted() => true;

        public async Task<bool> Stop()
        {
            await OnStopped();
            IsRunning = false;
            Timeout = DateTime.MinValue;
            return true;
        }

        public async Task<bool> Start(int timeoutMs)
        {
            IsRunning = true;
            Timeout = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            await OnStarted();
            return true;
        }

        public override string ToString() => $"{Name} ";
    }
}
