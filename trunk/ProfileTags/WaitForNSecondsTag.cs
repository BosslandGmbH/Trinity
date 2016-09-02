using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("WaitForNSeconds")]
    public class WaitForNSecondsTag : ProfileBehavior
    {

        [XmlAttribute("waitTime")]
        [DefaultValue(5)]
        public int WaitTime { get; set; }

        private bool _isDone;
        public override bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }

        public override void OnStart()
        {
            Debug.Print("Tag.OnStart");

        }

        private WaitCoroutine _waitCoroutine;

        public async Task<bool> Routine()
        {
            if (_waitCoroutine == null)
            {
                _waitCoroutine=new WaitCoroutine(WaitTime * 1000);
            }
            if (!await _waitCoroutine.GetCoroutine()) return true;
            _isDone = true;
            _waitCoroutine = null;
            return true;
        }



        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
            _waitCoroutine = null;
        }

    }
}
