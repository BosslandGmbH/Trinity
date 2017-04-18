using System.ComponentModel;
using System.Linq;
using Trinity.Framework;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.QuestTools;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;

namespace Trinity.ProfileTags
{
    [XmlElement("ClearLevelArea")]
    public class ClearLevelAreaTag : BaseProfileBehavior
    {
        private ISubroutine _task;

        public override async Task<bool> StartTask()
        {
            _task = new ClearLevelAreaCoroutine(QuestId, true);
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!_task.IsDone && !await _task.GetCoroutine())
                return false;

            return true;
        }

    }
}
