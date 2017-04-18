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
using Trinity.Framework.Objects.Enums;
using Trinity.ProfileTags.EmbedTags;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("ExploreLevelArea")]
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

        [XmlElement("IgnoreScenes")]
        [Description("Scenes that will be flagged as visited immediately")]
        public List<SceneTag> IgnoreScenes { get; set; }

        [XmlAttribute("actorId")]
        [Description("Legacy support for stopping exploration when actorId id found nearby. You should use 'stopCondition'.")]
        public int ActorId { get; set; }

        [XmlAttribute("markerHash")]
        [XmlAttribute("exitNameHash")]
        [Description("Legacy support for stopping exploration when exit found with specific markerHash. You should use 'stopCondition'.")]
        public int ExitNameHash { get; set; }

        [XmlAttribute("until")]
        [Description("Legacy support for stopping exploration when exit found. You should use 'stopCondition'.")]
        public string ExploreUntil { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            if (StartReset)
            {
                Core.Scenes.Reset();
            }
            var ignoreSceneNames = IgnoreScenes?.Select(s => s.Name).ToList();
            var levelAreaIds = new HashSet<int> {ZetaDia.CurrentLevelAreaSnoId};

            _exploreTask = new ExplorationCoroutine(levelAreaIds, ignoreSceneNames, null, false);
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (await CheckPreDefinedStopConditions())
                return true;

            if (!_exploreTask.IsDone && !await _exploreTask.GetCoroutine())
                return false;

            return true;
        }

        private async Task<bool> CheckPreDefinedStopConditions()
        {
            if (ExitNameHash != 0 && ProfileConditions.MarkerExistsNearMe(ExitNameHash, 80f))
                return true;

            if (ActorId > 0 && ProfileConditions.ActorExistsNearMe(ActorId, 80f))
                return true;

            if (ExploreUntil == "ExitFound" && ProfileConditions.MarkerTypeWithinRange("Exit", 80f))
                return true;

            return false;
        }
    }
}