using System.Collections.Generic;
using System.Linq; using Trinity.Framework;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Enums;
using Trinity.Modules;

namespace Trinity.Components.Adventurer.Game.Quests
{
    public static class BountyHelpers
    {
        //ActorId: 365020, Type: Item, Name: Khanduran Rune //A1
        //ActorId: 364281, Type: Item, Name: Caldeum Nightshade //A2
        //ActorId: 364290, Type: Item, Name: Arreat War Tapestry //A3
        //ActorId: 364305, Type: Item, Name: Corrupted Angel Flesh //A4
        //ActorId: 364975, Type: Item, Name: Westmarch Holy Water //A5

        //public static readonly int Act1BountyMatSNO = 365020;
        //public static readonly int Act2BountyMatSNO = 364281;
        //public static readonly int Act3BountyMatSNO = 364290;
        //public static readonly int Act4BountyMatSNO = 364305;
        //public static readonly int Act5BountyMatSNO = 364975;

        public static long GetActMatsCount(Act act)
        {
            var playerData = ZetaDia.Storage.PlayerDataManager.ActivePlayerData;
            switch (act)
            {
                case Act.A1:
                    return playerData.GetCurrencyAmount(CurrencyType.KhanduranRune);
                case Act.A2:
                    return playerData.GetCurrencyAmount(CurrencyType.CaldeumNightshade);
                case Act.A3:
                    return playerData.GetCurrencyAmount(CurrencyType.ArreatWarTapestry);
                case Act.A4:
                    return playerData.GetCurrencyAmount(CurrencyType.CorruptedAngelFlesh);
                case Act.A5:
                    return playerData.GetCurrencyAmount(CurrencyType.WestmarchHolyWater);
            }
            return 0;
        }

        public class ObjectiveActor
        {
            public int MarkerNameHash { get; set; }
            public int ActorId { get; set; }
            public int DestWorldId { get; set; }
        }

        public static Dictionary<int, ObjectiveActor> DynamicBountyPortals = new Dictionary<int, ObjectiveActor>
        {
            //texture = 81058

            { -2005510577, new ObjectiveActor { MarkerNameHash = -2005510577, ActorId = 446440, DestWorldId = 443801 } },
            { 498366490, new ObjectiveActor { MarkerNameHash = 498366490, ActorId = 448494, DestWorldId = 448396 } },
            { -816816641, new ObjectiveActor { MarkerNameHash = -816816641, ActorId = 448500, DestWorldId = 448373 } },
            { -728782754, new ObjectiveActor { MarkerNameHash = -728782754, ActorId = 448505, DestWorldId = 448402 } },
            { -2005068563, new ObjectiveActor { MarkerNameHash = -2005068563, ActorId = 446440, DestWorldId = 443705 } },
            { 124027337, new ObjectiveActor { MarkerNameHash = 124027337, ActorId = 446439, DestWorldId = 443678 } },
            { -1454464458, new ObjectiveActor { MarkerNameHash = -1454464458, ActorId = 446440, DestWorldId = 443720 } },
            { 1012772770, new ObjectiveActor { MarkerNameHash = 1012772770, ActorId = 448515, DestWorldId = 448366 } },
            { 467573611, new ObjectiveActor { MarkerNameHash = 467573611, ActorId = 448491, DestWorldId = 448409 } },
            { 1012330756, new ObjectiveActor { MarkerNameHash = 1012330756, ActorId = 448491, DestWorldId = 448381 } },
            { -2002131030, new ObjectiveActor { MarkerNameHash = -2002131030, ActorId = 446559, DestWorldId = 443686 } },
            { 37697317, new ObjectiveActor { MarkerNameHash = 37697317, ActorId = 444404, DestWorldId = 443756 } },
        };

        public static Vector3 ScanForMarkerLocation(int markerNameHash, int searchRadius)
        {
            var marker = Core.Markers.FirstOrDefault(m => m.NameHash == markerNameHash && m.Position.Distance(AdvDia.MyPosition) <= searchRadius && !EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));
            return marker?.Position ?? Vector3.Zero;
        }

        public static Vector3 ScanForMarkerLocation(WorldMarkerType type, int searchRadius)
        {
            var marker = Core.Markers.FirstOrDefault(m => m.MarkerType == type && m.Position.Distance(AdvDia.MyPosition) <= searchRadius && !EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));
            return marker?.Position ?? Vector3.Zero;
        }

        public static Vector3 ScanForMarkerLocation(string name, int searchRadius)
        {
            var marker = Core.Markers.FirstOrDefault(m => m.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && m.Position.Distance(AdvDia.MyPosition) <= searchRadius && !EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));
            return marker?.Position ?? Vector3.Zero;
        }

        public static TrinityMarker ScanForMarker(int markerNameHash, int searchRadius)
        {
            return Core.Markers.FirstOrDefault(m => m.NameHash == markerNameHash && m.Position.Distance(AdvDia.MyPosition) <= searchRadius && !EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));
        }

        public static TrinityMarker ScanForMarker(WorldMarkerType type, int searchRadius)
        {
            return Core.Markers.FirstOrDefault(m => m.MarkerType == type && m.Position.Distance(AdvDia.MyPosition) <= searchRadius && !EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));            
        }

        public static TrinityMarker ScanForMarker(string name, int searchRadius)
        {
            return Core.Markers.FirstOrDefault(m => m.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && m.Position.Distance(AdvDia.MyPosition) <= searchRadius && !EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));
        }

        public static Vector3 TryFindObjectivePosition(IList<ObjectiveActor> objectives, int searchRadius, out ObjectiveActor foundObjective)
        {
            var objectiveMarkers = new HashSet<int>(objectives.Select(o => o.MarkerNameHash));
            var miniMapMarker = AdvDia.CurrentWorldMarkers
                .FirstOrDefault(m => objectiveMarkers.Contains(m.NameHash) && m.Position.Distance(AdvDia.MyPosition) <= searchRadius && !EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));

            if (miniMapMarker != null)
            {
                foundObjective = objectives.FirstOrDefault(o => o.MarkerNameHash == miniMapMarker.Id);
                return miniMapMarker.Position;
            }
            foundObjective = null;
            return Vector3.Zero;
        }

        public static Vector3 ScanForActorLocation(int actorId, int searchRadius)
        {
            var actor = Core.Actors.Actors.FirstOrDefault(a => a.ActorSnoId == actorId && a.Distance <= searchRadius);
            return actor?.Position ?? Vector3.Zero;
        }

        public static bool AreAllActBountiesCompleted(Act act)
        {
            return !ZetaDia.Storage.Quests.Bounties.ToList().Any(b => b.Act == act && b.Info.State != QuestState.Completed);
        }

        public static DiaGizmo GetPortalNearMarkerPosition(Vector3 position)
        {
            if (JustEnteredWorld)
            {
                return null;
            }
            using (new PerformanceLogger("GetPortalNearMarkerPosition"))
            {
                var gizmo = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                    .Where(o => o != null && o.IsFullyValid() && o is GizmoPortal && o.Position.Distance(position) <= 10f)
                    .OrderBy(o => o.Distance)
                    .FirstOrDefault();
                return gizmo;
            }
        }

        public static bool JustEnteredWorld
        {
            get
            {
                if (PluginTime.CurrentMillisecond - PluginEvents.WorldChangeTime < 5000)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool AreAllActBountiesSupported(Act act)
        {
            var result =
                ZetaDia.Storage.Quests.Bounties.Count(
                    b => b.Act == act && BountyDataFactory.GetBountyData((int)b.Quest) != null) == 5;
            if (!result)
            {
                Core.Logger.Debug("[Bounties] Unsupported bounties are detected in {0}", act);
            }
            return result;
        }

        public static bool AnyUnsupportedBounties()
        {
            List<Act> _acts = new List<Act> { Act.A1, Act.A2, Act.A3, Act.A4, Act.A5 };
            for (int i = 0; i < 5; i++)
            {
                if (!AreAllActBountiesSupported(_acts[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsActEnabledOnSettings(Act act)
        {
            switch (act)
            {
                case Act.A1:
                    return PluginSettings.Current.BountyAct1;

                case Act.A2:
                    return PluginSettings.Current.BountyAct2;

                case Act.A3:
                    return PluginSettings.Current.BountyAct3;

                case Act.A4:
                    return PluginSettings.Current.BountyAct4;

                case Act.A5:
                    return PluginSettings.Current.BountyAct5;
            }
            return false;
        }

        public static readonly Dictionary<Act, SNOQuest> ActBountyFinishingQuests = new Dictionary<Act, SNOQuest>
        {
            {Act.A1,SNOQuest.x1_AdventureMode_BountyTurnin_A1},
            {Act.A2,SNOQuest.x1_AdventureMode_BountyTurnin_A2},
            {Act.A3,SNOQuest.x1_AdventureMode_BountyTurnin_A3},
            {Act.A4,SNOQuest.x1_AdventureMode_BountyTurnin_A4},
            {Act.A5,SNOQuest.x1_AdventureMode_BountyTurnin_A5},
        };

        public static bool IsActTurninInProgress(Act act)
        {
            return
                ZetaDia.Storage.Quests.AllQuests.Any(
                    q => q.Quest == ActBountyFinishingQuests[act] && q.State == QuestState.InProgress);
        }

        public static bool IsAnyActTurninInProgress()
        {
            return
                ZetaDia.Storage.Quests.AllQuests.Any(
                    q => ActBountyFinishingQuests.Values.Contains(q.Quest) && q.State == QuestState.InProgress);
        }

        public static bool IsActTurninCompleted(Act act)
        {
            return
                ZetaDia.Storage.Quests.AllQuests.Any(
                    q => q.Quest == ActBountyFinishingQuests[act] && q.State == QuestState.Completed);
        }
    }
}