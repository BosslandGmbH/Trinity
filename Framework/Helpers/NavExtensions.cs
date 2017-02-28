using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Dungeons;
using Zeta.Common;
using Zeta.Bot.Navigation;

namespace Trinity.Framework.Helpers
{
    public static class NavExtensions
    {
        private static Coroutine _navigateToCoroutine;
        internal static MoveResult NavigateTo(Vector3 destination, string destinationName = "")
        {
            if (_navigateToCoroutine == null || _navigateToCoroutine.IsFinished)
                _navigateToCoroutine = new Coroutine(async () => await Navigator.MoveTo(destination, destinationName));

            _navigateToCoroutine.Resume();

            if (_navigateToCoroutine.Status == CoroutineStatus.RanToCompletion)
                return (MoveResult)_navigateToCoroutine.Result;

            return MoveResult.Moved;
        }

        private static DefaultNavigationProvider _defaultProvider;
        internal static DefaultNavigationProvider DefaultProvider
        {
            get { return _defaultProvider ?? (_defaultProvider = Navigator.GetNavigationProviderAs<DefaultNavigationProvider>()); }
        }

        private static Coroutine _canPathWithinDistance;
        internal static bool CanPathWithinDistance(Vector3 destination, float pathPrecision)
        {
            if (_canPathWithinDistance == null || _canPathWithinDistance.IsFinished || _canPathWithinDistance.IsDisposed || _canPathWithinDistance.Status == CoroutineStatus.Faulted)
                _canPathWithinDistance = new Coroutine(async () => await DefaultProvider.CanPathWithinDistance(destination, pathPrecision));

            _canPathWithinDistance.Resume();

            if (_canPathWithinDistance.Status == CoroutineStatus.RanToCompletion)
                return (bool)_canPathWithinDistance.Result;

            return false;
        }

        private static Coroutine _canPathFullyClientPathTo;
        internal static bool CanPathFullyClientPathTo(Vector3 destination)
        {
            if (_canPathFullyClientPathTo == null || _canPathFullyClientPathTo.IsFinished || _canPathFullyClientPathTo.IsDisposed || _canPathFullyClientPathTo.Status == CoroutineStatus.Faulted)
                _canPathFullyClientPathTo = new Coroutine(async () => await DefaultProvider.CanFullyClientPathTo(destination));

            _canPathFullyClientPathTo.Resume();

            if (_canPathFullyClientPathTo.Status == CoroutineStatus.RanToCompletion)
                return (bool)_canPathFullyClientPathTo.Result;

            return false;
        }
    }
}
