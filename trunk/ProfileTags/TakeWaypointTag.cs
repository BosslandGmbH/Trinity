using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("TakeWaypoint")]
    public class TakeWaypointTag : ProfileBehavior
    {
        [XmlAttribute("waypointNumber")]
        public int WaypointNumber { get; set; }

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

            return new ActionRunCoroutine(ctx => Coroutine());
        }

        private async Task<bool> Coroutine()
        {
            if (await WaypointCoroutine.UseWaypoint(WaypointNumber))
            {
                _isDone = true;
                return true;
            }
            return false;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }

    }
}
