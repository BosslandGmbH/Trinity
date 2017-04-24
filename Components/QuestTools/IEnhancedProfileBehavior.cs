
using System;
using System.Linq;
using Trinity;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.QuestTools
{    
    /// <summary>
    /// Interface to expose Internal/Protected members in ProfileBehavior
    /// Enables running independently of treesharp start/stop/update
    /// Specifically for BotBehaviorQueue/WhenTag
    /// </summary>
    public interface IEnhancedProfileBehavior
    {
        /// <summary>
        /// For ProfileBehavior to run correctly Behavior property has to contain the composite 
        /// Call UpdateBehavior() in implementation.    
        /// </summary>
        void Update();

        /// <summary>
        /// Many tags use OnStart for setup and default params
        /// Call OnStart() in implementation.
        /// </summary>
        void Start();

        /// <summary>
        /// Method that should end the ProfileBehavior
        /// Set _isDone = true in implementation.
        /// Call Done() on children if INodeContainer
        /// </summary>
        void Done();
    }
}
