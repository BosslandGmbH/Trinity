using Trinity.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Coroutines.Town;
using Trinity.Components.QuestTools;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Reference;
using Trinity.ProfileTags.EmbedTags;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("ResumeUseTownPortal")]
    [XmlElement("TakeReturnPortal")]
    public class TakeReturnPortalTag : BaseProfileBehavior
    {
        public override async Task<bool> MainTask()
        {
            if(!await TrinityTownRun.TakeReturnPortal())
            {
                Core.Logger.Log("TakeReturnPortalTag has failed");
            }        
            return true;
        }

    }
}
