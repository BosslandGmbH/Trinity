using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Bot;

namespace Trinity.Modules
{
    public class TrinityChangeDetector<T> : ChangeDetector<T>
    {
        public TrinityChangeDetector(Func<T> valueProducer, TimeSpan interval = default(TimeSpan), [CallerMemberName] string name = "") : base(valueProducer, interval, name)
        {
            ChangeMonitor.Store.Add(this);
        }
    }

    public class ChangeMonitor : Module
    {
        public static readonly ChangeDetectorStore Store = new ChangeDetectorStore();

        public void Add(IChangeDetector detector) => Store.Add(detector);

        protected override int UpdateIntervalMs => 100;

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            foreach (var detector in Store.GetDetectors().Where(d => d.IsEnabled && d.HasSubscribers))
            {
                try
                {
                    //Logger.LogDebug($"Updating Detector: {detector.Name} BotMain.BotThread.ThreadState={BotMain.BotThread?.ThreadState} BotMain.BotThread.IsAlive={BotMain.BotThread?.IsAlive} BotMain.IsRunning={BotMain.IsRunning} BotEvents.IsBotRunning={BotEvents.IsBotRunning}");
                    detector.CheckForChanges();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"ChangeMonitor: {detector.Name} {ex}");
                }
            }
        }
    }
}
