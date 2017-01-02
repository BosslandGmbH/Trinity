using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Org.BouncyCastle.Asn1.X509;
using Trinity.Components.Combat;
using Trinity.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Framework.Behaviors
{
    public sealed class WaitAfterUnitDeathBehavior : BaseBehavior
    {
        public WaitAfterUnitDeathBehavior()
        {
            ActorEvents.OnUnitKilled += UnitKilled;
        }

        private void UnitKilled(TrinityActor actor)
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
            Logger.LogVerbose(LogCategory.Behavior, $"Waiting after unit death: {LastDiedUnit?.Name}, because {WaitReason}, TimeSinceDeath={TimeSinceDeath:g} RemainingWaitTime={RemainingWaitTime:g}");
            await Coroutine.Sleep(250);
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            LastStartedTime = DateTime.UtcNow;
            Logger.Warn($"Started waiting after unit death: {LastDiedUnit?.Name}, {LastDiedUnit?.AcdId}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            LastStoppedTime = DateTime.UtcNow;
            Logger.Warn($"Finished waiting after unit death: {LastDiedUnit?.Name}, TimeWaited={LastRunTime:g}");
            return true;
        }


    }
}

