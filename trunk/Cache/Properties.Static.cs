using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Game.Internals.Actors;

namespace Trinity.Cache
{
    /// <summary>
    /// A collection of properties that never (or very rarely) change
    /// </summary>
    public class StaticProperties : Properties.IPropertyCollection
    {
        public bool IsBountyObjective { get; set; }
        public bool IsMiniMapActive { get; set; }

        public void ApplyTo(TrinityCacheObject obj)
        {
            obj.IsBountyObjective = IsBountyObjective;
            obj.IsMinimapActive = IsMiniMapActive;
        }

        public void RefreshFrom(TrinityCacheObject obj)
        {
            this.IsBountyObjective = obj.CommonData.BountyObjective > 0;
            this.IsMiniMapActive = obj.CommonData.MinimapActive > 0;
        }

    }
}

