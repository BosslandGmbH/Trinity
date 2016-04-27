using System.Linq;

namespace Trinity.Configuration
{

    /// <summary>
    /// <para>* Handlers can always be invoked even without subscribers</para>
    /// <para>* Events will discard multiple identical subscriptions</para>
    /// </summary>
    class Events
    {

        /// <summary>
        /// Event is triggered by Combat.Enemeies, when unit becomes known to trinity
        /// </summary>
        public static event UnitAliveHandler OnUnitAlive
        {
            add { if (OnUnitAliveHandler == null || !OnUnitAliveHandler.GetInvocationList().Contains(value)) OnUnitAliveHandler += value; }
            remove { OnUnitAliveHandler -= value; }
        }
        public delegate void UnitAliveHandler(TrinityCacheObject unit);
        private static UnitAliveHandler _onUnitAliveHandler;
        public static UnitAliveHandler OnUnitAliveHandler
        {
            get { return _onUnitAliveHandler ?? (_onUnitAliveHandler += unit => { }); }
            private set { _onUnitAliveHandler = value; }
        }

        /// <summary>
        /// Event is triggered by Combat.Enemeies, when unit is no longer in ObjectCache
        /// </summary>
        public static event UnitDeathHandler OnUnitDeath
        {
            add { if (OnUnitDeathHandler == null || !OnUnitDeathHandler.GetInvocationList().Contains(value)) OnUnitDeathHandler += value; }
            remove { OnUnitDeathHandler -= value; }
        }
        public delegate void UnitDeathHandler(TrinityCacheObject unit);
        private static UnitDeathHandler _onUnitDeathHandler;
        public static UnitDeathHandler OnUnitDeathHandler
        {
            get { return _onUnitDeathHandler ?? (_onUnitDeathHandler += unit => { }); }
            private set { _onUnitDeathHandler = value; }
        }

        /// <summary>
        /// Event is triggered by RefreshObjects, when cache has been successfully updated
        /// </summary>
        public static event CacheUpdatedHandler OnCacheUpdated
        {
            add { if (OnCacheUpdatedHandler == null || !OnCacheUpdatedHandler.GetInvocationList().Contains(value)) OnCacheUpdatedHandler += value; }
            remove { OnCacheUpdatedHandler -= value; }
        }
        public delegate void CacheUpdatedHandler();
        private static CacheUpdatedHandler _onCacheUpdatedHandler;
        public static CacheUpdatedHandler OnCacheUpdatedHandler
        {
            get { return _onCacheUpdatedHandler ?? (_onCacheUpdatedHandler += () => { }); }
            private set { _onCacheUpdatedHandler = value; }
        }



    }
}
