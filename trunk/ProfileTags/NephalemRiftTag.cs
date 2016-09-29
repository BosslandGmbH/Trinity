using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.RiftCoroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("NephalemRift")]
    public class NephalemRiftTag : ProfileBehavior
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private RiftCoroutine _riftCoroutine;
        private bool _isDone;

        [XmlAttribute("riftCount")]
        public int RiftCount { get; set; }

        public override bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        public override void OnStart()
        {
            if (!Core.Adventurer.IsEnabled)
            {
                Logger.Error("Plugin is not enabled. Please enable Adventurer and try again.");
                _isDone = true;
                return;
            }
                   
            Combat.CombatMode = CombatMode.Normal;                
            var riftOptions = new RiftCoroutine.RiftOptions
            {
                RiftCount = RiftCount > 0 ? RiftCount : PluginSettings.Current.RiftCount,
            };

            PluginEvents.CurrentProfileType = ProfileType.Rift;

            _stopwatch.Start();
            //AdvDia.Update(true);
            _riftCoroutine = new RiftCoroutine(RiftType.Nephalem, riftOptions);
        }



        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Coroutine());
        }

        public async Task<bool> Coroutine()
        {
            if (_isDone)
            {
                return true;
            }

            PluginEvents.PulseUpdates();
            if (_riftCoroutine == null || await _riftCoroutine.GetCoroutine())
            {
                _isDone = true;
            }
            return true;
        }

        public override void OnDone()
        {
            Logger.Info("[Rift] It took {0} ms to finish the rift", _stopwatch.ElapsedMilliseconds);
            base.OnDone();
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
            _riftCoroutine = null;
            base.ResetCachedDone(force);
        }
    }
}
