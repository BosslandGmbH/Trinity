using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Trinity;
using Trinity.Components.QuestTools.Helpers;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Bot.Profile.Composites;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.QuestTools
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
            }
            return result;
        }

        internal static void LoadAdditionalGameParams()
        {           
            // Only worry about GameParams if we're about to start a new game
            if (ZetaDia.IsInGame || ProfileManager.CurrentProfile == null)
                return;
           
            var document = ProfileManager.CurrentProfile.Element;

            // Set Difficulty
            var difficultyAttribute = ((IEnumerable)document.XPathEvaluate("/GameParams[1]/@difficulty")).Cast<XAttribute>().ToList().FirstOrDefault();
            if (difficultyAttribute != null)
            {
                var difficulty = difficultyAttribute.Value.ChangeType<GameDifficulty>();
                if (difficulty != CharacterSettings.Instance.GameDifficulty)
                {
                    Core.Logger.Log("Difficulty changed to " + difficulty + " by profile: " + ProfileManager.CurrentProfile.Name);                      
                    CharacterSettings.Instance.GameDifficulty = difficulty;
                }              
            }

        }

        /// <summary>
        /// Replace some default DemonBuddy tags with enhanced Questtools versions
        /// </summary>
        internal static void ProcessProfile()
        {
            // All unrecognized tags are set as null and moved to root.
            // Avoid these tags throwing exception by removing them.
            ProfileManager.CurrentProfile?.Order?.RemoveAll(t => t == null);
        }        

        internal static void ReplaceTags(IList<ProfileBehavior> tags)
        {
            RecurseBehaviors(tags, (behavior, i, type) =>
            {
                if (behavior is IEnhancedProfileBehavior)
                    return behavior;

                if (type == typeof(LoadProfileTag))
                    return (behavior as LoadProfileTag).ToEnhanced();

                if (type == typeof(LeaveGameTag))
                    return (behavior as LeaveGameTag).ToEnhanced();

                if (type == typeof(LogMessageTag))
                    return (behavior as LogMessageTag).ToEnhanced();

                if (type == typeof(WaitTimerTag))
                    return (behavior as WaitTimerTag).ToEnhanced();

                if (type == typeof(UseWaypointTag))
                    return (behavior as UseWaypointTag).ToEnhanced();

                if (type == typeof(ToggleTargetingTag))
                    return (behavior as ToggleTargetingTag).ToEnhanced();

                if (type == typeof(IfTag))
                    return (behavior as IfTag).ToEnhanced();

                if (type == typeof(WhileTag))
                    return (behavior as WhileTag).ToEnhanced();

                if (type == typeof(UseObjectTag))
                    return (behavior as UseObjectTag).ToEnhanced();

                if (type == typeof(UsePowerTag))
                    return (behavior as UsePowerTag).ToEnhanced();

                if (type == typeof(WaitWhileTag))
                    return (behavior as WaitWhileTag).ToEnhanced();

                return behavior;
            });
        }

        public delegate ProfileBehavior TagProcessingDelegate(ProfileBehavior node, int index, Type type);

        /// <summary>
        /// Walks through profile nodes recursively, 
        /// TagProcessingDelegate is called for every Tag.
        /// The original tag is replaced by tag returned by TagProcessingDelegate
        /// </summary>
        public static void RecurseBehaviors(IList<ProfileBehavior> nodes, TagProcessingDelegate replacementDelegate, int depth = 0, int maxDepth = 20)
        {
            if (nodes == null || !nodes.Any())
                return;

            if (replacementDelegate == null)
                return;

            if (depth == maxDepth)
            {
                Core.Logger.Debug("MaxDepth ({0}) reached on ProfileUtils.ReplaceBehaviors()", maxDepth);
                return;
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] == null)
                    continue;
                 
                var node = nodes[i];
                var type = node.GetType();
                
                nodes[i] = replacementDelegate.Invoke(node, i, type);

                if(nodes[i] == null)
                    continue;

                if (node is INodeContainer)
                {
                    RecurseBehaviors((node as INodeContainer).GetNodes() 
                        as List<ProfileBehavior>, replacementDelegate, depth + 1, maxDepth);
                }

            }
        }

    }
}
