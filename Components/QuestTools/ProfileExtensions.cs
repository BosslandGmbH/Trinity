using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Bot.Profile.Composites;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace Trinity.Components.QuestTools
{
    public static class ProfileExtensions
    {

        /// <summary>
        /// Fetches value from Dictionary or adds and returns a default value.
        /// </summary>
        internal static TV GetOrCreateValue<TK, TV>(this Dictionary<TK, TV> dictionary, TK key, TV newValue = default(TV))
        {
            if (key == null)
                throw new ArgumentNullException("key");

            TV foundValue;
            if (dictionary.TryGetValue(key, out foundValue))
                return foundValue;

            if (newValue == null)
                newValue = (TV)Activator.CreateInstance(typeof(TV));

            dictionary.Add(key, newValue);
            return newValue;
        }

        public static List<ProfileBehavior> GetChildren(this ProfileBehavior behavior)
        {
            var result = new List<ProfileBehavior>();
            var container = behavior as INodeContainer;
            if (container != null)
                result = container.GetNodes().ToList();

            return result;
        }

        public static void SetChildrenDone(this ProfileBehavior behavior)
        {
            behavior.GetChildren().ForEach(b =>
            {
                (b as IEnhancedProfileBehavior)?.Done();
            });
        }

        public static void ResetChildren(this ProfileBehavior behavior)
        {
            behavior.GetChildren().ForEach(b => b.ResetCachedDone());
        }

        public static bool AreChildrenDone(this ProfileBehavior behavior)
        {
            return behavior.GetChildren().All(b => b.IsDone);
        }

        /// <summary>
        /// Prepare IEnhancedProfileBehavior to be executed as TreeSharp Composite
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        private static Composite RunEnhanced(this IEnhancedProfileBehavior behavior)
        {
            if (!(behavior is ProfileBehavior)) 
                return new Action(ret => RunStatus.Failure);

            behavior.Update();

            if ((behavior as ProfileBehavior).QuestId == 0)
                (behavior as ProfileBehavior).QuestId = 1;

            if ((behavior as ProfileBehavior).StepId == 0)
                (behavior as ProfileBehavior).StepId = 1;
            
            (behavior as ProfileBehavior).ResetCachedDone();

            behavior.Start();

            return (behavior as ProfileBehavior).Behavior;
        }

        /// <summary>
        /// Prepare ProfileBehavior to be executed as TreeSharp Composite
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static Composite Run(this ProfileBehavior behavior)
        {
            var type = behavior.GetType();            

            if (behavior is IEnhancedProfileBehavior)
                return (behavior as IEnhancedProfileBehavior).RunEnhanced();

            if (type == typeof(LoadProfileTag))
                return (behavior as LoadProfileTag).ToEnhanced().RunEnhanced();

            if (type == typeof(LeaveGameTag))
                return (behavior as LeaveGameTag).ToEnhanced().RunEnhanced();

            if (type == typeof(LogMessageTag))
                return (behavior as LogMessageTag).ToEnhanced().RunEnhanced();

            if (type == typeof(WaitTimerTag))
                return (behavior as WaitTimerTag).ToEnhanced().RunEnhanced();

            if (type == typeof(UseWaypointTag))
                return (behavior as UseWaypointTag).ToEnhanced().RunEnhanced();

            if (type == typeof(ToggleTargetingTag))
                return (behavior as ToggleTargetingTag).ToEnhanced().RunEnhanced();

            if (type == typeof(IfTag))
                return (behavior as IfTag).ToEnhanced().RunEnhanced();

            if (type == typeof(WhileTag))
                return (behavior as WhileTag).ToEnhanced().RunEnhanced();

            if (type == typeof(UseObjectTag))
                return (behavior as UseObjectTag).ToEnhanced().RunEnhanced();

            if (type == typeof(UsePowerTag))
                return (behavior as UsePowerTag).ToEnhanced().RunEnhanced();

            if (type == typeof(WaitWhileTag))
                return (behavior as WaitWhileTag).ToEnhanced().RunEnhanced();
            
            Core.Logger.Warn("You attempted to run a tag ({0}) that can't be converted to IEnhancedProfileBehavior ", behavior.GetType());

            return new Action(ret => RunStatus.Failure);           
        }

        public static void CopyTo(this ProfileBehavior a, ProfileBehavior b)
        {
            b.QuestId = a.QuestId;
            b.StepId = a.StepId;
            b.StatusText = a.StatusText;
            b.IgnoreReset = a.IgnoreReset;
            b.QuestName = a.QuestName;
        }

        public static EnhancedLoadProfileTag ToEnhanced(this LoadProfileTag tag)
        {
            var asyncVersion = new EnhancedLoadProfileTag();
            tag.CopyTo(asyncVersion);
            asyncVersion.Profile = tag.Profile;
            asyncVersion.LeaveGame = tag.LeaveGame;
            asyncVersion.LoadRandom = tag.LoadRandom;
            asyncVersion.Profiles = tag.Profiles;
            asyncVersion.StayInParty = tag.StayInParty;
            return asyncVersion;
        }

        public static EnhancedLeaveGameTag ToEnhanced(this LeaveGameTag tag)
        {
            var asyncVersion = new EnhancedLeaveGameTag();
            tag.CopyTo(asyncVersion);
            asyncVersion.Reason = tag.Reason;
            asyncVersion.StayInParty = tag.StayInParty;
            return asyncVersion;
        }

        public static EnhancedLogMessageTag ToEnhanced(this LogMessageTag tag)
        {
            var asyncVersion = new EnhancedLogMessageTag();
            tag.CopyTo(asyncVersion);
            asyncVersion.Output = tag.Output;
            return asyncVersion;
        }

        public static EnhancedWaitTimerTag ToEnhanced(this WaitTimerTag tag)
        {
            var asyncVersion = new EnhancedWaitTimerTag();
            tag.CopyTo(asyncVersion);
            asyncVersion.WaitTime = tag.WaitTime;
            return asyncVersion;
        }

        public static EnhancedUseWaypointTag ToEnhanced(this UseWaypointTag tag)
        {
            var asyncVersion = new EnhancedUseWaypointTag();
            tag.CopyTo(asyncVersion);
            asyncVersion.X = tag.X;
            asyncVersion.Y = tag.Y;
            asyncVersion.Z = tag.Z;
            asyncVersion.WaypointNumber = tag.WaypointNumber;
            return asyncVersion;
        }

        public static EnhancedToggleTargetingTag ToEnhanced(this ToggleTargetingTag tag)
        {
            var asyncVersion = new EnhancedToggleTargetingTag();
            tag.CopyTo(asyncVersion);
            asyncVersion.Combat = tag.Combat;
            asyncVersion.KillRadius = tag.KillRadius;
            asyncVersion.Looting = tag.Looting;
            asyncVersion.LootRadius = tag.LootRadius;
            return asyncVersion;
        }

        public static EnhancedIfTag ToEnhanced(this IfTag tag)
        {
            var asyncVersion = new EnhancedIfTag();
            asyncVersion.Body = tag.Body;
            asyncVersion.Condition = tag.Condition;
            asyncVersion.Conditional = tag.Conditional;
            tag.CopyTo(asyncVersion);
            return asyncVersion;
        }

        internal static EnhancedWhileTag ToEnhanced(this WhileTag tag)
        {
            var asyncVersion = new EnhancedWhileTag();
            asyncVersion.Body = tag.Body;
            asyncVersion.Condition = tag.Condition;
            asyncVersion.Conditional = tag.Conditional;
            tag.CopyTo(asyncVersion);
            return asyncVersion;
        }

        public static EnhancedUseObjectTag ToEnhanced(this UseObjectTag tag)
        {
            var asyncVersion = new EnhancedUseObjectTag();
            asyncVersion.ActorId = tag.ActorId;
            asyncVersion.Hotspots = tag.Hotspots;
            asyncVersion.IsPortal = tag.IsPortal;
            asyncVersion.InteractRange = tag.InteractRange;
            asyncVersion.X = tag.X;
            asyncVersion.Y = tag.Y;
            asyncVersion.Z = tag.Z;
            tag.CopyTo(asyncVersion);
            return asyncVersion;
        }

        public static EnhancedUsePowerTag ToEnhanced(this UsePowerTag tag)
        {
            var asyncVersion = new EnhancedUsePowerTag();
            asyncVersion.SNOPower = tag.SNOPower;
            asyncVersion.X = tag.X;
            asyncVersion.Y = tag.Y;
            asyncVersion.Z = tag.Z;
            tag.CopyTo(asyncVersion);
            return asyncVersion;
        }

        public static EnhancedWaitWhileTag ToEnhanced(this WaitWhileTag tag)
        {
            var asyncVersion = new EnhancedWaitWhileTag();
            asyncVersion.Condition = tag.Condition;
            asyncVersion.Conditional = tag.Conditional;
            tag.CopyTo(asyncVersion);
            return asyncVersion;
        }

    }
}

