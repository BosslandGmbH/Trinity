using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("SetCombatMode")]
    public class SetCombatModeTag : ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("mode")]
        public CombatMode Mode { get; set; }

        public override bool IsDone => _isDone;

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => GetCoroutine());
        }

        private async Task<bool> GetCoroutine()
        {
            Combat.CombatMode = Mode;
            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }
    }
}
