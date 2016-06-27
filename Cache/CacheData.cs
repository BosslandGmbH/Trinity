using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Cache;
using Trinity.Framework;
using Trinity.Framework.Modules;
using Zeta;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity
{
    /// <summary>
    /// Note: this class is going away soon; to be replaced by Core.[Name] modules.
    /// </summary>
    public class CacheData
    {
        public static InventoryCache Inventory => Core.Inventory;
        public static PlayerCache Player => Core.Player;
        public static HotbarCache Hotbar => Core.Hotbar;
        public static BuffsCache Buffs => Core.Buffs;
        public static TargetsCache Targets => Core.Targets;

        internal static void Clear()
        {
            Core.Inventory.Clear();
            Core.Targets.Clear();
            Core.Hotbar.Clear();
            Core.Player.Clear();
            Core.Buffs.Clear();
        }

        /// <summary>
        /// Contains an RActorGUID and count of the number of times we've switched to this target
        /// </summary>
        internal static Dictionary<string, TargettingInfo> TargetHistory = new Dictionary<string, TargettingInfo>();

        /// <summary>
        /// How many times the player tried to interact with this object in total
        /// </summary>
        internal static Dictionary<int, int> InteractAttempts = new Dictionary<int, int>();
    

    }
}
