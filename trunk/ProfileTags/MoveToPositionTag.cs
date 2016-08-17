using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Trinity.Technicals;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("MoveToPosition")]
    public class MoveToPositionTag : ProfileBehavior
    {

        [XmlAttribute("x")]
        [DefaultValue(0)]
        public float X { get; set; }

        [XmlAttribute("y")]
        [DefaultValue(0)]
        public float Y { get; set; }

        [XmlAttribute("z")]
        [DefaultValue(0)]
        public float Z { get; set; }

        private bool _isDone;
        public override bool IsDone => _isDone;

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }

        public override void OnStart() => Logger.LogVerbose($"Started Tag: {GetType().Name}");
        public override void OnDone() => Logger.LogVerbose($"Finished Tag: {GetType().Name}");

        private ISubroutine _task;

        public async Task<bool> Routine()
        {
            if(_task == null)
                _task = new MoveToPositionCoroutine(AdvDia.CurrentWorldId, new Vector3(X, Y, Z), 3);

            if (!await _task.GetCoroutine()) return true;
            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _task?.Reset();
            _isDone = false;
        }

    }
}

