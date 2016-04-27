using System.Linq;

namespace Trinity.Cache
{
    public static class TrinityCacheObjectUtils
    {
        private static TrinityCacheObject _currentCacheObject;

        private static bool _isNavBlocking;
        public static bool IsNavBlocking(this TrinityCacheObject o)
        {
            if (_currentCacheObject != null && _currentCacheObject.RActorGuid == o.RActorGuid)
                return _isNavBlocking;

            _isNavBlocking = TrinityPlugin.LastTargetACDGuid != o.ACDGuid && CacheData.NavigationObstacles.Any(ob => MathUtil.IntersectsPath(ob.Position, ob.Radius, CacheData.Player.Position, o.Position));
            _currentCacheObject = o;
            return _isNavBlocking;
        }
    }

}
