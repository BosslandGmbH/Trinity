using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("WaitForCombat")]
    public class WaitForCombatTag : BaseProfileBehavior
    {
        [XmlAttribute("seconds")]
        [XmlAttribute("maxWaitSeconds")]
        [Description("Maximum time to wait ")]
        [DefaultValue(20)]
        public int MaxWaitSeconds { get; set; }

        public override void OnPulse()
        {
            if (ZetaDia.Me.IsInCombat) Done();
        }

        public override async Task<bool> MainTask()
        {            
            await Coroutine.Wait(MaxWaitSeconds * 1000, () => ZetaDia.Me.IsInCombat);
            return true;
        }

    }
}
