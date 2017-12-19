using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Game.Exploration.SceneMapping
{
    public class RegionGroup : IWorldRegion, IEnumerable<IWorldRegion>
    {
        public List<IWorldRegion> Regions { get; set; } = new List<IWorldRegion>();
        public CombineType CombineType { get; set; } = CombineType.Add;

        public bool Contains(Vector3 position)
        {
            var added = false;
            foreach (var r in Regions)
            {
                // any 'add' regions that contain provided result in add, except if also in subtract region.
                if (!added && r.CombineType == CombineType.Add && r.Contains(position))
                {
                    added = true;
                    continue;
                }

                // Any hit within a subtracted region means position is not contained within group.
                if (r.CombineType == CombineType.Subtract && r.Contains(position))
                    return false;
            }
            return added;
        }

        public IEnumerator<IWorldRegion> GetEnumerator() => Regions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IWorldRegion x) => Regions.Add(x);

        public IWorldRegion GetOffset(Vector2 min)
        {
            return new RegionGroup
            {
                Regions = Regions.Select(r => r.GetOffset(min)).ToList(),
                CombineType = CombineType,
            };
        }

    }
}
