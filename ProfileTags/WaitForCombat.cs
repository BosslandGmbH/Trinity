using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines;
using Zeta.Bot;
using Zeta.Bot.Profile.Common;
using Zeta.Bot.Profile.Composites;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("WaitForCombat")]
    public class WaitForCombatTag : TrinityProfileBehavior
    {
        public WaitForCombatTag()
        {
            QuestId = QuestId == 0 ? 1 : QuestId;
        }

        [XmlAttribute("maxWaitSeconds")]
        [DefaultValue(60)]
        public int MaxWaitSeconds { get; set; }

        private bool _isDone;
        public override bool IsDone => _isDone;

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }

        public async Task<bool> Routine()
        {
            await Coroutine.Wait(10000, () => ZetaDia.Me.IsInCombat);
            await Coroutine.Wait(MaxWaitSeconds*1000, () => !ZetaDia.Me.IsInCombat);
            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }

    }
}
