using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("ClearArea")]
    public class ClearAreaTag : ProfileBehavior
    {

        private Vector3 _center;
        private ClearAreaForNSecondsCoroutine _clearAreaForNSecondsCoroutine;
        private ClearLevelAreaCoroutine _clearLevelAreaCoroutine;

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
            //AdvDia.Update(true);
            _center = AdvDia.MyPosition;
            _clearAreaForNSecondsCoroutine = new ClearAreaForNSecondsCoroutine(0, 60, 0, 0, 60, false);
            _clearLevelAreaCoroutine=new ClearLevelAreaCoroutine(312429);
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }


        public async Task<bool> Routine()
        {
            if (!await _clearLevelAreaCoroutine.GetCoroutine()) return true;
            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }

    }
}
