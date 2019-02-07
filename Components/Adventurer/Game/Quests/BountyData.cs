using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.Adventurer.Game.Quests
{
    public class BountyData
    {
        private QuestData _questData;
        private HashSet<SNOLevelArea> _levelAreaIds;

        public SNOQuest QuestId { get; set; }
        public Act Act { get; set; }
        public SNOWorld WorldId { get; set; }
        public BountyQuestType QuestType { get; set; }
        public List<ISubroutine> Coroutines { get; set; }
        public SNOLevelArea WaypointLevelAreaId { get; set; }

        public HashSet<SNOLevelArea> LevelAreaIds
        {
            get
            {
                if (_levelAreaIds == null)
                {
                    _levelAreaIds = QuestData.LevelAreaIds;
                }
                return _levelAreaIds;
            }
            set => _levelAreaIds = value;
        }

        public QuestData QuestData => _questData ?? (_questData = QuestData.GetQuestData(QuestId));

        public bool IsAvailable
        {
            get { return ZetaDia.Storage.Quests.AllQuests.Any(q => q.QuestSNO == QuestId && q.State != QuestState.Completed); }
        }

        private int _waypointNumber;

        public int WaypointNumber
        {
            get
            {
                if (_waypointNumber != 0)
                    return _waypointNumber;

                if (WaypointLevelAreaId != 0)
                    QuestData.Waypoint = WaypointFactory.GetWaypointByLevelAreaId(WaypointLevelAreaId);

                if (QuestData.Waypoint != null)
                    return QuestData.Waypoint.Number;

                if (BountyDataFactory.QuestWaypointNumbers.ContainsKey(QuestId))
                {
                    var wpnr = BountyDataFactory.QuestWaypointNumbers[QuestId];
                    if (wpnr >= 40) wpnr++;
                    return wpnr;
                }

                return 0;
            }
            set => _waypointNumber = value;
        }

        public virtual void Reset()
        {
            foreach (var coroutine in Coroutines)
            {
                coroutine.Reset();
            }

            if (WaypointLevelAreaId == (SNOLevelArea)13)
            {
                var firstCoroutine = Coroutines.FirstOrDefault();
                if (firstCoroutine != null && !(firstCoroutine is MoveToPositionCoroutine))
                {
                    //<SafeMoveTo questId="312429" stepId="2" x="2401" y="4537" z="-2" pathPrecision="5" pathPointLimit="250" scene="trOut_Highlands_Roads_EW_03" statusText="" />
                    Coroutines.Insert(0, new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2401, 4537, -2)));
                }
            }
            //if (WaypointNumber == 14)
            //{
            //    var firstCoroutine = Coroutines.FirstOrDefault();
            //    if (firstCoroutine != null && !(firstCoroutine is MoveToPositionCoroutine))
            //    {
            //        //<SafeMoveTo questId="312429" stepId="2" x="2401" y="4537" z="-2" pathPrecision="5" pathPointLimit="250" scene="trOut_Highlands_Roads_EW_03" statusText="" />
            //        Coroutines.Insert(0, new MoveToPositionCoroutine(new Vector3(1472, 4027, 38)));
            //    }

            //}
        }

        public override string ToString()
        {
            return
                $"Act={Act} QuestId={QuestId} IsAvailable={IsAvailable} Coroutines.Count={Coroutines.Count} Waypoint.Number={QuestData.Waypoint.Number} StartWorldSnoId={QuestData.Waypoint.WorldSnoId} StartLevelArea={QuestData.Waypoint.LevelAreaSnoId} WorldId={WorldId}";
        }
    }
}
