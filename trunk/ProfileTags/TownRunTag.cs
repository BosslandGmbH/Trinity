using Trinity.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
    [XmlElement("TownRun")]
    public class TownRunTag : BaseProfileBehavior
    {
        public override async Task<bool> StartTask()
        {
            TrinityTownRun.IsWantingTownRun = true;
            return true;
        }
    }
}
