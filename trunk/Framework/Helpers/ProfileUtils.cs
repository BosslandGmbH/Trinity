using System.Collections;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Buddy.Coroutines;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Bot.Profile.Composites;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Action = System.Action;

namespace Trinity.Framework.Helpers
{
    public class ProfileUtils
    {
        public static bool IsWithinRange(Vector3 position, float range = 12f)
        {
            return position != Vector3.Zero && !(position.Distance2D(ZetaDia.Me.Position) > range);
        }

        public static void RandomShuffle<T>(IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<Vector3> GetCirclePoints(int points, double radius, Vector3 center)
        {
            var result = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var newX = (int)(center.X + radius * Math.Cos(angle));
                var newY = (int)(center.Y + radius * Math.Sin(angle));

                var newpoint = new Vector3(newX, newY, center.Z);
                result.Add(newpoint);
                Logger.LogDebug("Calculated point {0}: {1}", i, newpoint.ToString());
            }
            return result;
        }

        //internal static void LoadAdditionalGameParams()
        //{
        //    // Only worry about GameParams if we're about to start a new game
        //    if (ZetaDia.IsInGame || ProfileManager.CurrentProfile == null)
        //        return;

        //    var document = ProfileManager.CurrentProfile.Element;

        //    // Set Difficulty
        //    var difficultyAttribute = ((IEnumerable)document.XPathEvaluate("/GameParams[1]/@difficulty")).Cast<XAttribute>().ToList().FirstOrDefault();
        //    if (difficultyAttribute != null)
        //    {
        //        var difficulty = difficultyAttribute.Value.ChangeType<GameDifficulty>();
        //        if (difficulty != CharacterSettings.Instance.GameDifficulty)
        //        {
        //            Logger.Log("Difficulty changed to " + difficulty + " by profile: " + ProfileManager.CurrentProfile.Name);
        //            CharacterSettings.Instance.GameDifficulty = difficulty;
        //        }
        //    }

        //}

        ///// <summary>
        ///// Replace some default DemonBuddy tags with enhanced Questtools versions
        ///// </summary>
        //internal static void ProcessProfile()
        //{
        //    // All unrecognized tags are set as null and moved to root.
        //    // Avoid these tags throwing exception by removing them.
        //    ProfileManager.CurrentProfile.Order.RemoveAll(t => t == null);

        //    RecurseBehaviors(Zeta.Bot.ProfileManager.CurrentProfile.Order, (node, i, type) =>
        //    {
        //        //if (node is IfTag && type == typeof(IfTag))
        //        //{
        //        //    return new EnhancedIfTag
        //        //    {
        //        //        Body = (node as IfTag).Body,
        //        //        Condition = (node as IfTag).Condition,
        //        //        Conditional = (node as IfTag).Conditional,
        //        //    };
        //        //}

        //        //if (node is WhileTag && type == typeof(WhileTag))
        //        //{
        //        //    return new EnhancedWhileTag
        //        //    {
        //        //        Body = (node as IfTag).Body,
        //        //        Condition = (node as IfTag).Condition,
        //        //        Conditional = (node as IfTag).Conditional,
        //        //    };
        //        //}

        //        return node;
        //    });
        //}

        //internal static void AsyncReplaceTags(IList<ProfileBehavior> tags)
        //{
        //    RecurseBehaviors(tags, (behavior, i, type) =>
        //    {
        //        if (behavior is IEnhancedProfileBehavior)
        //            return behavior;

        //        if (type == typeof(LoadProfileTag))
        //            return (behavior as LoadProfileTag).ToEnhanced();

        //        if (type == typeof(LeaveGameTag))
        //            return (behavior as LeaveGameTag).ToEnhanced();

        //        if (type == typeof(LogMessageTag))
        //            return (behavior as LogMessageTag).ToEnhanced();

        //        if (type == typeof(WaitTimerTag))
        //            return (behavior as WaitTimerTag).ToEnhanced();

        //        if (type == typeof(UseWaypointTag))
        //            return (behavior as UseWaypointTag).ToEnhanced();

        //        if (type == typeof(ToggleTargetingTag))
        //            return (behavior as ToggleTargetingTag).ToEnhanced();

        //        if (type == typeof(IfTag))
        //            return (behavior as IfTag).ToEnhanced();

        //        if (type == typeof(WhileTag))
        //            return (behavior as WhileTag).ToEnhanced();

        //        if (type == typeof(UseObjectTag))
        //            return (behavior as UseObjectTag).ToEnhanced();

        //        if (type == typeof(UsePowerTag))
        //            return (behavior as UsePowerTag).ToEnhanced();

        //        if (type == typeof(WaitWhileTag))
        //            return (behavior as WaitWhileTag).ToEnhanced();

        //        return behavior;
        //    });
        //}

        //public delegate ProfileBehavior TagProcessingDelegate(ProfileBehavior node, int index, Type type);

        ///// <summary>
        ///// Walks through profile nodes recursively, 
        ///// TagProcessingDelegate is called for every Tag.
        ///// The original tag is replaced by tag returned by TagProcessingDelegate
        ///// </summary>
        //public static void RecurseBehaviors(IList<ProfileBehavior> nodes, TagProcessingDelegate replacementDelegate, int depth = 0, int maxDepth = 20)
        //{
        //    if (nodes == null || !nodes.Any())
        //        return;

        //    if (replacementDelegate == null)
        //        return;

        //    if (depth == maxDepth)
        //    {
        //        Logger.Debug("MaxDepth ({0}) reached on ProfileUtils.ReplaceBehaviors()", maxDepth);
        //        return;
        //    }

        //    for (var i = 0; i < nodes.Count(); i++)
        //    {
        //        if (nodes[i] == null)
        //            continue;

        //        var node = nodes[i];
        //        var type = node.GetType();

        //        nodes[i] = replacementDelegate.Invoke(node, i, type);

        //        if (nodes[i] == null)
        //            continue;

        //        var newType = nodes[i].GetType();

        //        //if (QuestTools.EnableDebugLogging)
        //        //    Logger.Debug("".PadLeft(depth * 5) + "{0}> {1}", depth, newType != type ?
        //        //    string.Format("replaced {0} with {1}", type, newType) :
        //        //    string.Format("ignored {0}", newType)
        //        //    );

        //        if (node is INodeContainer)
        //        {
        //            RecurseBehaviors((node as INodeContainer).GetNodes() as List<ProfileBehavior>, replacementDelegate, depth + 1, maxDepth);
        //        }

        //    }
        //}

        public static bool CanPathToLocation(Vector3 position, int maxDistance = 200, int pathPrecision = 10)
        {
            var distance = position.Distance2D(ZetaDia.Me.Position);
            if (position == Vector3.Zero || distance > maxDistance)
            {
                Logger.LogDebug("Location is too far away! Distance={0}", distance);
                return false;
            }
            if (!NavExtensions.CanPathWithinDistance(position, pathPrecision) || Navigator.StuckHandler.IsStuck)
            {
                Logger.LogDebug("Can't navigate to position or currently stuck! Distance={0}", distance);
                return false;
            }
            Logger.LogVerbose("Found path to position! Distance={0}", distance);
            return true;
        }

        /// <summary>
        /// Checks if navigator can find a path to actor
        /// </summary>
        /// <returns></returns>
        public static bool CanPathToActor(int actorId, int maxDistance = 200, int pathPrecision = 10)
        {
            var actor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault(a => a.ActorSnoId == actorId);
            if (actor == null)
            {
                Logger.LogDebug("Can't find actor Id={0}", actorId);
                return false;
            }
            return CanPathToLocation(actor.Position, maxDistance, pathPrecision);
        }

        ///// <summary>
        ///// A Vector3 to the middle of current town.
        ///// </summary>
        //public static Vector3 TownApproachVector
        //{
        //    get
        //    {
        //        Logger.LogDebug("ZetaDia.CurrentAct={0}", ZetaDia.CurrentAct);

        //        switch (ZetaDia.CurrentLevelAreaSnoId)
        //        {
        //            case 332339: // Act1
        //                return new Vector3(403.2f, 569.4f, 24.0f);
        //            case 168314: // Act2
        //                return new Vector3(304.9f, 243.5f, 0.10f);
        //            case 92945:  // Act3/4
        //                return new Vector3(442.4f, 415.1f, 0.1f);
        //            case 270011: // Act5
        //                return new Vector3(576f, 775f, 2.6f);
        //        }

        //        switch (ZetaDia.CurrentAct)
        //        {
        //            case Act.A1:
        //                return new Vector3(403.2f, 569.4f, 24.0f);
        //            case Act.A2:
        //                return new Vector3(304.9f, 243.5f, 0.10f);
        //            case Act.A3:
        //            case Act.A4:
        //                return new Vector3(442.4f, 415.1f, 0.1f);
        //            case Act.A5:
        //                return new Vector3(576f, 775f, 2.6f);
        //        }

        //        Logger.LogDebug("Failed to find current act CurrentLevelAreaSnoId={0} ZetaDia.CurrentAct={1} WorldType={2}",
        //            ZetaDia.CurrentActSnoId, ZetaDia.CurrentAct, ZetaDia.WorldType);

        //        return Vector3.Zero;
        //    }
        //}

    }
}
