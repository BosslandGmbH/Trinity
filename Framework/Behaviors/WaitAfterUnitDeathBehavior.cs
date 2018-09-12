using Buddy.Coroutines;
using System;
using Trinity.Framework.Helpers;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;


namespace Trinity.Framework.Behaviors
{
    public sealed class WaitAfterUnitDeathBehavior : BaseBehavior
    {
        public WaitAfterUnitDeathBehavior()
        {
            ActorEvents.OnUnitKilled += UnitKilled;
        }

        private void UnitKilled(object sender, TrinityActor actor)
        {
            if (!IsRunning)
            {
                LastDiedUnit = actor;
                WaitStartTime = DateTime.UtcNow;
            }
        }

        public DateTime WaitStartTime { get; private set; } = DateTime.MinValue;
        public TrinityActor LastDiedUnit { get; private set; }
        public TimeSpan WaitTimeMs { get; private set; }
        public string WaitReason { get; private set; }
        public Predicate<TrinityActor> WaitCondition { get; private set; }
        public DateTime LastStartedTime { get; set; }
        public DateTime LastStoppedTime { get; set; }
        public TimeSpan TimeSinceDeath => DateTime.UtcNow - WaitStartTime;
        public TimeSpan RemainingWaitTime => (WaitStartTime + WaitTimeMs) - DateTime.UtcNow;
        public TimeSpan LastRunTime => LastStoppedTime - LastStartedTime;

        public async Task<bool> While(Predicate<TrinityActor> waitCondition, string reason = "", int waitMs = 5000)
        {
            if (!IsRunning)
            {
                WaitTimeMs = TimeSpan.FromMilliseconds(waitMs);
                WaitCondition = waitCondition;
                WaitReason = reason;
            }
            return await Run(async () => LastDiedUnit != null && await WaitCheck(), WaitAction, waitMs * 2);
        }

        public void Clear()
        {
            LastDiedUnit = null;
            WaitStartTime = DateTime.MinValue;
        }

        private async Task<bool> WaitCheck()
        {
            return LastDiedUnit != null && WaitCondition(LastDiedUnit) && TimeSinceDeath < WaitTimeMs;
        }

        private async Task<bool> WaitAction()
        {
            Core.Logger.Verbose(LogCategory.Behavior, $"Waiting after unit death: {LastDiedUnit?.Name}, because {WaitReason}, TimeSinceDeath={TimeSinceDeath:g} RemainingWaitTime={RemainingWaitTime:g}");
            await Coroutine.Sleep(250);
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            LastStartedTime = DateTime.UtcNow;
            Core.Logger.Warn($"Started waiting after unit death: {LastDiedUnit?.Name}, {LastDiedUnit?.AcdId}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            LastStoppedTime = DateTime.UtcNow;
            Core.Logger.Warn($"Finished waiting after unit death: {LastDiedUnit?.Name}, TimeWaited={LastRunTime:g}");
            return true;
        }
    }
}