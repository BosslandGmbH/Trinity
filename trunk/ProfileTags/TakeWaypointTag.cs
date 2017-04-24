using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("Waypoint")]
    [XmlElement("UseWaypoint")]
    [XmlElement("TakeWaypoint")]
    public class TakeWaypointTag : BaseProfileBehavior
    {
        [XmlAttribute("number")]
        [XmlAttribute("waypointNumber")]
        [Description("Number of waypoint to arrive at")]
        [DefaultValue(-1)]
        public int WaypointNumber { get; set; }

        [XmlAttribute("levelAreaId")]
        [XmlAttribute("levelAreaSnoId")]
        [XmlAttribute("destinationLevelAreaSnoId")]
        [Description("Id of level area to arrive at")]
        public int DestinationLevelAreaSnoId { get; set; }

        public override async Task<bool> MainTask()
        {
            if (WaypointNumber == -1 && DestinationLevelAreaSnoId != 0)
                WaypointNumber = WaypointCoroutine.GetWaypointNumber(DestinationLevelAreaSnoId);

            if (!await WaypointCoroutine.UseWaypoint(WaypointNumber))
                return false;

            Done();
            await Coroutine.Sleep(1000);
            return true;
        }

    }
}

