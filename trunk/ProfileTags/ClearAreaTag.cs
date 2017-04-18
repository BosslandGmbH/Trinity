﻿using System.ComponentModel;
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
    [XmlElement("ClearArea")]
    public class ClearAreaTag : ClearAreaProfileBehavior { }

    public class ClearAreaProfileBehavior : BaseProfileBehavior
    {
        private ISubroutine _task;

        [XmlAttribute("radius")]
        [Description("Size of area to clear")]
        [DefaultValue(30)]
        public float Radius { get; set; }

        [XmlAttribute("seconds")]
        [Description("How long to clear area for")]
        [DefaultValue(15)]
        public int Seconds { get; set; }

        [XmlAttribute("marker")]
        [XmlAttribute("markerHash")]
        [Description("the id of a marker to find within area cleared")]
        [DefaultValue(0)]
        public int MarkerHash { get; set; }

        [XmlAttribute("actor")]
        [XmlAttribute("actorSnoId")]
        [Description("Actors to find within the area cleared")]
        [DefaultValue(0)]
        public int ActorId { get; set; }

        public override async Task<bool> StartTask()
        {
            _task = new ClearAreaForNSecondsCoroutine(QuestId, Seconds, ActorId, MarkerHash, (int)Radius);
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
