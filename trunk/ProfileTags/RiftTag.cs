using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.RiftCoroutines;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("Rift")]
    public class RiftTag : ProfileBehavior
    {

        [XmlAttribute("level")]
        public virtual int Level { get; set; }

        [XmlAttribute("empowered")]
        public bool IsEmpowered { get; set; }

        [XmlAttribute("getXPShrine")]
        public bool IsGetXPShrine { get; set; }

        [XmlAttribute("riftCount")]
        public int RiftCount { get; set; }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private RiftCoroutine _riftCoroutine;
        private bool _isDone;

        public override bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        public override void OnStart()
        {
            if (Level == 0) Level = -1;
            _stopwatch.Start();

            var riftOptions = new RiftCoroutine.RiftOptions
            {
                RiftCount = RiftCount > 0 ? RiftCount : PluginSettings.Current.RiftCount,
                IsEmpowered = IsEmpowered || PluginSettings.Current.UseEmpoweredRifts,
                NormalRiftForXPShrine = IsGetXPShrine || PluginSettings.Current.NormalRiftForXPShrine,
            };

            _riftCoroutine = new RiftCoroutine(RiftType.Nephalem, riftOptions);
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Coroutine());
        }

        public async Task<bool> Coroutine()
        {
            if (await _riftCoroutine.GetCoroutine())
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
        }
    }
}
