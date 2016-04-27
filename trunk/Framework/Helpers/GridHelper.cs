using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventurer.Game.Exploration;
using Trinity.Framework.Avoidance;
using Zeta.Common;

namespace Trinity.Framework.Grid
{
    public class GridHelper
    {
        protected NavigationGrid _navigation
        {
            get { return NavigationGrid.Instance; }
        }

        private ExplorationGrid _exploration
        {
            get { return ExplorationGrid.Instance; }
        }

        private AvoidanceGrid _avoidance
        {
            get { return AvoidanceGrid.Instance; }
        }

        public bool CanRayCast(Vector3 @from, Vector3 to)
        {
            return _navigation.CanRayCast(@from, to);
        }

        public bool CanRayWalk(Vector3 @from, Vector3 to)
        {
            return _navigation.CanRayCast(@from, to);
        }
    }

}


