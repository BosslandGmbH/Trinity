using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.UI;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("Test2")]
    public class Test2Tag : ProfileBehavior
    {

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
            Debug.Print("Tag.CreateBehavior");
            return new ActionRunCoroutine(ctx => Routine());
        }

        public override void OnStart()
        {
            Debug.Print("Tag.OnStart");

        }

        //private WaitCoroutine _waitCoroutine = new WaitCoroutine(50000);

        public async Task<bool> Routine()
        {
            return await Task2();
        }

        //public async Task<bool> Task1()
        //{
        //    Logger.Info("Task1");
        //    await Coroutine.Sleep(100000);
        //    return true;
        //}

        public async Task<bool> Task2()
        {
            //Logger.Info("Task2");
            //DeveloperUI.DumpNearbyWaypoint();
            DeveloperUI.CheckForDynamicBounties();
            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }

    }
}
