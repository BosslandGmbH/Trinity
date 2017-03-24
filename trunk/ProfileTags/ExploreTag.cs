using System;
using System.Collections.Generic;
using Trinity.Framework;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.QuestTools;
using Zeta.Common;
using Zeta.Game;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("TrinityExploreDungeon")]
    [XmlElement("ExploreDungeon")]
    [XmlElement("Explore")]
    public class ExploreTag : ExploreTagProfileBehavior { }

    public class ExploreTagProfileBehavior : BaseProfileBehavior
    {
        private ISubroutine _exploreTask;

        #region XmlAttributes

        [XmlAttribute("startReset")]
        [DefaultValue(false)]
        [Description("If explored nodes should be cleared when starting tag")]
        public bool StartReset { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            if (StartReset)
            {
                Core.Scenes.Reset();
            }
            _exploreTask = new ExplorationCoroutine(new HashSet<int> {ZetaDia.CurrentLevelAreaSnoId}, null, null, false);
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!_exploreTask.IsDone && !await _exploreTask.GetCoroutine())
                return false;

            return true;
        }

    }
}