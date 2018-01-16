using System.ComponentModel;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.QuestTools;
using Trinity.Framework.Objects.Enums;
using Zeta.Game;
using Zeta.XmlEngine;


namespace Trinity.ProfileTags
{
    [XmlElement("TrinityMoveToMarker")]
    [XmlElement("MoveToMarker")]
    [XmlElement("MoveToMapMarker")]
    public class MoveToMapMarkerTag : MoveToMarkerProfileBehavior { }

    public class MoveToMarkerProfileBehavior : BaseProfileBehavior
    {
        private ISubroutine _task;

        #region XmlAttributes

        [XmlAttribute("hash")]
        [XmlAttribute("marker")]
        [XmlAttribute("exitNameHash")]
        [XmlAttribute("mapMarkerNameHash")]
        [XmlAttribute("markerNameHash")]
        [XmlAttribute("portalNameHash")]
        [XmlAttribute("markerHash")]
        [DefaultValue(0)] // -1 is used for objective in MoveToMapMarkerCoroutine
        [Description("Name hash id of the marker")]
        public int MarkerHash { get; set; }

        [XmlAttribute("markerName")]
        [Description("Full or partial name of the marker")]
        public string MarkerName { get; set; }

        [XmlAttribute("markerType")]
        [DefaultValue(default(WorldMarkerType))]
        [Description("Type of the Marker")]
        public WorldMarkerType MarkerType { get; set; }
        //Hidden, None, Waypoint, Entrance, Exit, Objective, Portal, Shrine, LegendaryItem, SetItem, ExitStone

        [XmlAttribute("zerg")]
        [DefaultValue(false)]
        [Description("If combat should be ignored where possible enroute")]
        public bool Zerg { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            if (MarkerHash != 0)
            {
                if (MarkerType != default(WorldMarkerType))
                {
                    _task = new MoveToMapMarkerCoroutine(QuestId, ZetaDia.Globals.WorldSnoId, MarkerHash, MarkerType, Zerg);
                    return false;
                }

                _task = new MoveToMapMarkerCoroutine(QuestId, ZetaDia.Globals.WorldSnoId, MarkerHash, Zerg);
                return false;
            }
            if (!string.IsNullOrEmpty(MarkerName))
            {
                _task = new MoveToMapMarkerCoroutine(QuestId, ZetaDia.Globals.WorldSnoId, MarkerName, Zerg);
                return false;
            }
            if (MarkerType != default(WorldMarkerType))
            {
                _task = new MoveToMapMarkerCoroutine(QuestId, ZetaDia.Globals.WorldSnoId, MarkerType, Zerg);
                return false;
            }
            return true;
        }

        public override async Task<bool> MainTask()
        {
            if (!_task.IsDone && !await _task.GetCoroutine())
                return false;

            return true;
        }

    }
}

