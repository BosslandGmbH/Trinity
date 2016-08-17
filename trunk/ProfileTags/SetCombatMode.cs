using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.RiftCoroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.Adventurer.Tags
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
            CombatBase.CombatMode = Mode;
            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }
    }
}
