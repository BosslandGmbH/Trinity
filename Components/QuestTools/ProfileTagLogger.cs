using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Modules;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;
using Zeta.XmlEngine;

namespace Trinity.Components.QuestTools
{
    public class ProfileTagLogger
    {
        public class StateSnapshot
        {
            public static StateSnapshot Create()
            {
                ZetaDia.Actors.Update();
                if (!ZetaDia.IsInGame || ZetaDia.Me == null)
                    return null;

                var s = new StateSnapshot();
                using (ZetaDia.Memory.AcquireFrame())
                {
                    Core.Scenes.Update();
                    Core.Update();

                    var position = ZetaDia.Me.Position;
                    var currentQuest = ZetaDia.CurrentQuest;


                    s.QuestId = currentQuest?.QuestSnoId ?? (SNOQuest)1;
                    s.QuestStep = currentQuest?.StepId ?? 1;
                    s.SceneSnoId = ZetaDia.Me.CurrentScene.SceneInfo.GetSNOId<SNOScene>();
                    s.SceneName = ZetaDia.Me.CurrentScene.Name;
                    s.WorldSnoId = ZetaDia.Globals.WorldSnoId;
                    s.LevelAreaSnoId = ZetaDia.CurrentLevelAreaSnoId;

                    var waypoint = ZetaDia.Storage.ActManager.GetWaypointByLevelAreaSnoId(s.LevelAreaSnoId);
                    s.WaypointNumber = waypoint?.Number ?? 0;

                    s.UpdateForActor(Core.Actors.Me);

                }
                return s;
            }

            public void UpdateForActor(TrinityActor actor)
            {
                if (actor == null)
                {
                    ActorId = 0;
                    ActorSno = 0;
                    ActorName = string.Empty;
                    StartAnimation = string.Empty;
                    SetPosition(Vector3.Zero);
                    return;
                }

                ActorId = actor.ActorSnoId;
                ActorSno = ActorId;
                ActorName = actor.Name;
                StartAnimation = actor.Animation != 0 ? actor.Animation.ToString() : string.Empty;
                SetPosition(actor.Position);

                var marker = BountyHelpers.GetMarkerNearActor(actor);
                if (marker != null)
                {
                    MarkerHash = marker.NameHash;
                    MarkerType = marker.MarkerType;
                }
                else
                {
                    MarkerHash = 0;
                    MarkerName = string.Empty;
                    MarkerType = WorldMarkerType.None;
                }
            }

            public void UpdateForMarker(TrinityMarker marker)
            {
                if (marker == null) return;
                MarkerHash = marker.NameHash;
                MarkerType = marker.MarkerType;

                var actor = Core.Actors.Where(a => !a.IsMe && !a.IsExcludedId && a.Position.Distance(marker.Position) <= 3f).OrderBy(a => a.IsGizmo).ThenBy(a => a.Distance).FirstOrDefault();
                UpdateForActor(actor);
            }

            public void SetPosition(Vector3 position)
            {
                if (AdvDia.CurrentWorldScene == null)
                {
                    Core.Scenes.Update();
                }

                var relativePosition = AdvDia.CurrentWorldScene?.GetRelativePosition(position) ?? Vector3.Zero;
                X = position.X;
                Y = position.Y;
                Z = position.Z;
                SceneX = relativePosition.X;
                SceneY = relativePosition.Y;
                SceneZ = relativePosition.Z;
            }

            public string StartAnimation { get; set; }
            public WorldMarkerType MarkerType { get; set; }
            public int MarkerHash { get; set; }
            public string MarkerName { get; set; }
            public SNOActor ActorSno { get; set; }
            public SNOActor ActorId { get; set; }
            public string ActorName { get; set; }
            public int WaypointNumber { get; set; }
            public float SceneZ { get; set; }
            public float SceneY { get; set; }
            public float SceneX { get; set; }
            public float Z { get; set; }
            public float Y { get; set; }
            public float X { get; set; }
            public int QuestStep { get; set; }
            public SNOLevelArea LevelAreaSnoId { get; set; }
            public SNOWorld WorldSnoId { get; set; }
            public string SceneName { get; set; }
            public SNOScene SceneSnoId { get; set; }
            public SNOQuest QuestId { get; set; }
        }

        public static string GenerateActorTags<T>(Func<TrinityActor, bool> actorSelector) where T : ProfileBehavior
        {
            var sb = new StringBuilder();
            var s = StateSnapshot.Create();
            foreach (var actor in Core.Actors.Actors.Where(a => !a.IsMe && actorSelector(a)).OrderBy(a => a.Distance))
            {
                s.UpdateForActor(actor);
                var minimapEntry = Core.Minimap.MinimapIcons.FirstOrDefault(m => m.Position.Distance(actor.Position) < 5f);
                var minimapMsg = minimapEntry != null ? $"MinimapName={minimapEntry.Name} " : string.Empty;
                var markerMsg = !string.IsNullOrEmpty(s.MarkerName) ? $"Marker={s.MarkerName} ({s.MarkerHash}, {s.MarkerType}) " : string.Empty;
                sb.AppendLine($"     <!-- {actor.Name} ({actor.ActorSnoId}) {(SNOActor)actor.ActorSnoId} Distance={actor.Distance} Type={actor.Type} Anim={actor.Animation} {minimapMsg}{markerMsg}-->");
                sb.AppendLine(GenerateTag<T>(s));
                sb.AppendLine(Environment.NewLine);
            }
            return sb.ToString();
        }

        public static string GenerateMarkerTags<T>(Func<TrinityMarker, bool> markerSelector = null) where T : ProfileBehavior
        {
            var sb = new StringBuilder();
            var s = StateSnapshot.Create();
            foreach (var marker in Core.Markers.Where(a => markerSelector?.Invoke(a) ?? true))
            {
                s.UpdateForMarker(marker);
                var actorMsg = s.ActorId != 0 ? $"Actor={s.ActorName} ({s.ActorId}) " : string.Empty;
                sb.AppendLine($"     <!-- {marker.NameHash} {marker.MarkerType} Distance={marker.Distance} TextureId={marker.TextureId} WorldSnoId={marker.WorldSnoId} {actorMsg}-->");
                sb.AppendLine(GenerateTag<T>(s));
                sb.AppendLine(Environment.NewLine);
            }
            return sb.ToString();
        }


        public static string GenerateTag<T>(StateSnapshot snapshot = null) where T : ProfileBehavior
        {
            var result = string.Empty;
            var s = snapshot ?? StateSnapshot.Create();
            if (s == null) return result;

            var stateDict = s.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(k => k.Name, v => v.GetValue(s));

            result += $@"     <{typeof(T).Name.TrimEnd("Tag")} questId=""{s.QuestId}"" stepId=""{s.QuestStep}"" ";

            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var xmlAttributes = propertyInfo.GetCustomAttributes<XmlAttributeAttribute>();
                var attName = xmlAttributes.LastOrDefault()?.Name ?? string.Empty;

                if (string.IsNullOrEmpty(attName) || IgnoreXmlAttributeNames.Contains(attName))
                    continue;

                var valueMatch = stateDict.FirstOrDefault(i
                    => string.Equals(propertyInfo.Name.ToLowerInvariant(), i.Key.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(attName, i.Key.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase));

                var value = valueMatch.Value ?? GetDefaultValue(propertyInfo);
                if (value == null)
                {
                    continue;
                }

                if (IsNumericType(propertyInfo.PropertyType) && !propertyInfo.Name.Contains("Id"))
                {
                    value = Math.Round((decimal)Convert.ChangeType(value, typeof(decimal)), 3, MidpointRounding.AwayFromZero);
                    result += $@"{attName}=""{value:0.##}"" ";
                }
                else
                {
                    if (propertyInfo.PropertyType == typeof(bool))
                        value = value.ToString().ToLowerInvariant();

                    result += $@"{attName}=""{value}"" ";
                }
            }

            result += $@" worldSnoId=""{s.WorldSnoId}"" levelAreaSnoId=""{s.LevelAreaSnoId}"" />";
            return result;
        }

        private static object GetDefaultValue(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DefaultValueAttribute>()?.Value;
        }

        public static string GenerateQuestInfo()
        {
            var currentQuest = ZetaDia.Storage.Quests.ActiveQuests.FirstOrDefault();
            if (currentQuest == null) return null;
            var currentStep = currentQuest.QuestRecord.Steps.FirstOrDefault(q => q.StepId == currentQuest.QuestStep);
            var obj = currentStep?.QuestStepObjectiveSet?.QuestStepObjectives?.FirstOrDefault();
            var output = $"Quest: {currentQuest.Quest}: {currentQuest.DisplayName} ({currentQuest.QuestSNO}) {currentQuest.QuestType} Step: {currentStep?.Name} ({currentQuest.QuestStep})";
            return output;
        }

        public static string GenerateTagComment()
        {
            var sb = new StringBuilder();
            var indent = "        ";
            sb.AppendLine("");
            sb.AppendLine(indent + GenerateQuestInfoComment());
            foreach (var o in GetObjectiveInfo())
            {
                sb.AppendLine(indent + $"<!-- Objective: {o.Index}: {o.Description} ({o.State}) -->");
            }
            sb.AppendLine(indent + GenerateWorldInfoComment());
            return sb.ToString();
        }

        public static IEnumerable<TrinityObjectiveInfo> GetActiveObjectives() => GetObjectiveInfo().Where(o => o.State == QuestStepObjectiveElementState.InProgress);

        public static List<TrinityObjectiveInfo> GetObjectiveInfo()
        {
            // Why you make things so difficult for me nesox?
            var objectives = new List<TrinityObjectiveInfo>();
            var logger = (log4net.Repository.Hierarchy.Logger)log4net.LogManager.GetLogger(typeof(QuestObjectiveInfo))?.Logger;
            if (logger == null) return objectives;
            var dbLogger = (log4net.Repository.Hierarchy.Logger)log4net.LogManager.GetLogger(typeof(Demonbuddy.MainWindow))?.Logger;
            if (dbLogger == null) return objectives;
            var asyncAppender = dbLogger.Parent.Appenders.ToArray().OfType<Zeta.Common.Logger.AsyncAppender>().FirstOrDefault();
            var repository = (Hierarchy)LogManager.GetRepository();
            repository.Root.RemoveAppender(asyncAppender);
            var appender = new MemoryAppender();
            logger.AddAppender(appender);
            QuestObjectiveInfo.DumpObjectives();
            objectives.AddRange(appender.GetEvents().Select(e => new TrinityObjectiveInfo(e.RenderedMessage)));
            logger.RemoveAppender(appender);
            repository.Root.AddAppender(asyncAppender);
            return objectives;
        }

        public class TrinityObjectiveInfo
        {
            public TrinityObjectiveInfo(string input)
            {
                Description = new Regex("(?<=Description:\\W)[\\w\\s\\'\\`]+").Match(input).Value;
                Enum.TryParse(new Regex("(?<=State:\\W)[\\w\\s]+").Match(input).Value, out State);
                int.TryParse(new Regex("(?<=Index:\\W)[\\w\\s]+").Match(input).Value, out Index);
            }

            public QuestStepObjectiveElementState State;
            public string Description;
            public int Index;
        }

        public static string GenerateQuestInfoComment()
        {
            return $"<!-- {GenerateQuestInfo()} -->";
        }


        public static string GenerateWorldInfoComment()
        {
            var sceneSnoId = ZetaDia.CurrentLevelAreaSnoId;
            var sceneSnoName = ZetaDia.SNO.LookupSNOName(SNOGroup.LevelArea, (int)sceneSnoId);
            var worldSnoId = ZetaDia.Globals.WorldSnoId;
            var world = ZetaDia.SNO[SNOGroup.Worlds].GetRecord<SNORecordWorld>((int)worldSnoId);
            return $"<!-- World: {world.Name} ({worldSnoId}) Scene: {sceneSnoName} ({sceneSnoId}) Generated={world.IsGenerated} -->";
        }

        private static HashSet<string> IgnoreXmlAttributeNames { get; } = new HashSet<string>
        {
            "questId",
            "stepId",
            "worldSnoId",
            "levelAreaSnoId",
            "ignoreReset",
            "statusText",
            "objectiveIndex",
            "questName",
        };

        public static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

    }
}
