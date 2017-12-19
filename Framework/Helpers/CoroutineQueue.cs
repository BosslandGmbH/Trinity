using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines;
using Zeta.Bot;
using Zeta.Common;
using Zeta.TreeSharp;

namespace Trinity.Framework.Helpers
{
    public class CoroutineQueue
    {
        private static readonly Composite BotBehaviorHook = new ActionRunCoroutine(ret => BotBehaviorTask());
        private static readonly Queue<ICoroutine> Queue = new Queue<ICoroutine>();
        private static bool HooksInserted { get; set; }

        public static ICoroutine CurrentTask { get; set; }

        public static void Enable()
        {
            Pulsator.OnPulse += Pulse;
            TreeHooks.Instance.OnHooksCleared += Instance_OnHooksCleared;
        }

        private static void Pulse(object sender, EventArgs e)
        {
            UpdateHooks();
        }

        public static void Disable()
        {
            Pulsator.OnPulse -= Pulse;
            TreeHooks.Instance.OnHooksCleared -= Instance_OnHooksCleared;
        }

        private static void Instance_OnHooksCleared(object sender, EventArgs e)
        {
            HooksInserted = false;
        }

        private static void UpdateHooks()
        {
            if (Core.Adventurer.IsEnabled && !HooksInserted)
            {
                TreeHooks.Instance.InsertHook("BotBehavior", 0, BotBehaviorHook);
                HooksInserted = true;
            }
            else if (!Core.Adventurer.IsEnabled && HooksInserted)
            {
                TreeHooks.Instance.RemoveHook("BotBehavior", BotBehaviorHook);
                HooksInserted = false;
            }
        }

        public static bool Enqueue(ICoroutine coroutine)
        {
            if (IsQueued(coroutine))
                return false;

            Queue.Enqueue(coroutine);
            return true;
        }

        private static async Task<bool> BotBehaviorTask()
        {
            while (Queue.Any())
            {
                var coroutine = Queue.Dequeue();
                CurrentTask = coroutine;
                var result = false;
                coroutine.Reset();
                while (!result)
                {
                    result = await coroutine.GetCoroutine();
                    await Coroutine.Yield();
                }
                CurrentTask = null;
                var disposable = coroutine as IDisposable;
                disposable?.Dispose();
            }
            return false;
        }

        public static bool IsQueued(ICoroutine task) 
            => CurrentTask != null && CurrentTask.Id == task.Id || Queue.Any(i => i.Id == task.Id);
    }
}