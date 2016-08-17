using System.Threading.Tasks;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("StopBot")]
    public class StopBotTag : ProfileBehavior
    {
        private bool _isDone;
        public override bool IsDone => _isDone;
        protected override Composite CreateBehavior() => new ActionRunCoroutine(ctx => StopBot());
        public override void ResetCachedDone(bool force = false) => _isDone = false;

        private async Task<bool> StopBot()
        {
            BotMain.Stop();
            _isDone = true;
            return true;
        }
    }
}
