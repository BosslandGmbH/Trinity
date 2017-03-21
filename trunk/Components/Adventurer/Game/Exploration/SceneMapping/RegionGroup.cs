using System.Collections;
using System.Collections.Generic;
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
                if (!added && r.CombineType == CombineType.Add && r.Contains(position))
                {
                    added = true;
                    continue;
                }
                if (r.CombineType == CombineType.Subtract && r.Contains(position))
                    return false;
            }
            return added;
        }

        public IEnumerator<IWorldRegion> GetEnumerator() => Regions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IWorldRegion x) => Regions.Add(x);

        public IWorldRegion Offset(Vector2 min)
        {
            foreach (var region in Regions)
            {
                region.Offset(min);
            }
            return this;
        }
    }
}