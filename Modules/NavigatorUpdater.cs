using System.Linq;
using Trinity.Framework.Objects;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Modules
{
    public class NavigatorUpdater : Module
    {
        protected override void OnPulse()
        {
            var provider = (Navigator.SearchGridProvider as MainGridProvider);

            foreach (var prop in ZetaDia.Actors.GetActorsOfType<DiaObject>().Where(a => a.ActorType == ActorType.ServerProp))
            {
                provider?.AddCellWeightingObstacle(prop.ActorSnoId, prop.ActorInfo.Sphere.Radius);
            }
        }
    }
}
