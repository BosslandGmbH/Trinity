using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Items;
using Trinity.Technicals;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace Trinity.Helpers
{
    public class Composites
    {
        private const int WaitForCacheDropDelay = 1500;
        private const int MultiOpenPauseDelay = 250;

        public static Composite CreateLootBehavior(Composite child)
        {
            return
            new PrioritySelector(
                CreateUseHoradricCache(),
                child
            );
        }

        private static DateTime _lastCheckedForHoradricCache = DateTime.MinValue;
        private static DateTime _lastFoundHoradricCache = DateTime.MinValue;

        public static DateTime LastCheckedForHoradricCache
        {
            get { return _lastCheckedForHoradricCache; }
            set { _lastCheckedForHoradricCache = value; }
        }

        public static DateTime LastFoundHoradricCache
        {
            get { return _lastFoundHoradricCache; }
            set { _lastFoundHoradricCache = value; }
        }

        public static Composite CreateUseHoradricCache()
        {
            return
            new Decorator(ret => Core.Settings.Loot.TownRun.OpenHoradricCaches && !BrainBehavior.IsVendoring && !TrinityTownRun.IsTryingToTownPortal() &&
                    DateTime.UtcNow.Subtract(LastCheckedForHoradricCache).TotalSeconds > 1,
                new Sequence(
                    new Action(ret => LastCheckedForHoradricCache = DateTime.UtcNow),
                    new Decorator(ret => HasHoradricCaches(),
                        new Action(ret => OpenHoradricCache())
                    )
                )
            );

        }

        internal static RunStatus OpenHoradricCache()
        {
            if (DateTime.UtcNow.Subtract(LastFoundHoradricCache).TotalMilliseconds < MultiOpenPauseDelay)
            {
                // Pause between opening caches
                return RunStatus.Running;
            }

            if (HasHoradricCaches() && ZetaDia.IsInTown)
            {
                var item = ZetaDia.Me.Inventory.Backpack.FirstOrDefault(i => CacheIds.Contains(i.ActorSnoId));
                ZetaDia.Me.Inventory.UseItem(item.AnnId);
                LastFoundHoradricCache = DateTime.UtcNow;
                TrinityPlugin.TotalBountyCachesOpened++;
                return RunStatus.Running;
            }

            if (DateTime.UtcNow.Subtract(LastFoundHoradricCache).TotalMilliseconds < WaitForCacheDropDelay)
            {
                Logger.Log("Waiting for Horadric Cache drops");
                return RunStatus.Running;
            }

            return RunStatus.Success;

        }

        public static HashSet<int> CacheIds = new HashSet<int>
        {
            (int)SNOActor.HoradricCacheA1,
            (int)SNOActor.GreaterHoradricCache,
            (int)SNOActor.HoradricCacheBonusAct,
        };

        internal static bool HasHoradricCaches()
        {
            return ZetaDia.Me.Inventory.Backpack.Any(i => CacheIds.Contains(i.ActorSnoId));
        }

    }
}
