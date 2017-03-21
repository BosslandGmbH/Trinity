using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;
using Zeta.XmlEngine;

namespace Trinity.Components.QuestTools
{
    public class ProfileTagLogger
    {
        public class StateSnapshot
        {
            public void UpdateForActor(TrinityActor actor)
            {
                if (actor == null) return;
                ActorId = actor.ActorSnoId;
                ActorSno = (SNOActor)ActorId;
                SetPosition(actor.Position);
            }

            public static StateSnapshot Create()
            {
                ZetaDia.Actors.Update();
                if (!ZetaDia.IsInGame || ZetaDia.Me == null)
                    return null;

                var s = new StateSnapshot();
                using (ZetaDia.Memory.AcquireFrame())
                {
                    ScenesStorage.Update();
                    Core.Update();

                    var position = ZetaDia.Me.Position;
                    var currentQuest = ZetaDia.CurrentQuest;


                    s.QuestId = currentQuest?.QuestSnoId ?? 1;
                    s.QuestStep = currentQuest?.StepId ?? 1;
                    s.SceneId = ZetaDia.Me.CurrentScene.SceneInfo.SNOId;
                    s.SceneName = ZetaDia.Me.CurrentScene.Name;
                    s.WorldSnoId = ZetaDia.Globals.WorldSnoId;
                    s.LevelAreaSnoId = ZetaDia.CurrentLevelAreaSnoId;

                    var waypoint = ZetaDia.Storage.ActManager.GetWaypointByLevelAreaSnoId(s.LevelAreaSnoId);
                    s.WaypointNumber = waypoint.Number;

                    s.UpdateForActor(Core.Actors.Me);

                }
                return s;
            }

            public void SetPosition(Vector3 position)
            {
                var relativePosition = AdvDia.CurrentWorldScene.GetRelativePosition(position);
                X = position.X;
                Y = position.Y;
                Z = position.Z;
                SceneX = relativePosition.X;
                SceneY = relativePosition.Y;
                SceneZ = relativePosition.Z;
            }

            public SNOActor ActorSno { get; set; }
            public int ActorId { get; set; }
            public int WaypointNumber { get; set; }
            public float SceneZ { get; set; }
            public float SceneY { get; set; }
            public float SceneX { get; set; }
            public float Z { get; set; }
            public float Y { get; set; }
            public float X { get; set; }
            public int QuestStep { get; set; }
            public int LevelAreaSnoId { get; set; }
            public int WorldSnoId { get; set; }
            public string SceneName { get; set; }
            public int SceneId { get; set; }
            public int QuestId { get; set; }
        }

        public static string GenerateTags<T>(Func<TrinityActor, bool> actorSelector) where T  : ProfileBehavior
        {
            var sb = new StringBuilder();
            var s = StateSnapshot.Create();
            var actors = Core.Actors.AllRActors.Where(actorSelector);
            foreach (var actor in actors)
            {
                s.UpdateForActor(actor);
                sb.AppendLine($"     <!-- {actor.Name} ({actor.ActorSnoId}) {(SNOActor) actor.ActorSnoId} Distance={actor.Distance} Type={actor.Type} -->");
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

                var valueMatch = stateDict.FirstOrDefault(i => string.Equals(propertyInfo.Name.ToLowerInvariant(), i.Key.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase));       
                var value = valueMatch.Value ?? GetDefaultValue(propertyInfo);
                if (value == null)
                {
                    continue;
                }

                if (IsNumericType(propertyInfo.PropertyType) && !propertyInfo.Name.Contains("Id"))
                    value = Math.Round((double)Convert.ChangeType(value,typeof(double)),2);

                else if (propertyInfo.PropertyType == typeof(bool))
                    value = value.ToString().ToLowerInvariant();

                result += $@"{attName}=""{value}"" ";
            }

            result += $@" worldSnoId=""{s.WorldSnoId}"" levelAreaSnoId=""{s.LevelAreaSnoId}"" />";
            return result;
        }

        private static object GetDefaultValue(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DefaultValueAttribute>()?.Value;
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
