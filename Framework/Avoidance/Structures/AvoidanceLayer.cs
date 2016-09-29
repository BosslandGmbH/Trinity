using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Zeta.Common;

namespace Trinity.Framework.Avoidance.Structures
{
    public class AvoidanceLayer : ICollection<AvoidanceNode>
    {
        public List<AvoidanceNode> Nodes = new List<AvoidanceNode>();
        public List<Vector3> Positions = new List<Vector3>();

        public AvoidanceLayer(IEnumerable<AvoidanceNode> list)
        {
            var avoidanceNodes = list as IList<AvoidanceNode> ?? list.ToList();
            Nodes = new List<AvoidanceNode>(avoidanceNodes);
            Positions = avoidanceNodes.Select(n => n.NavigableCenter).ToList();
        }

        public AvoidanceLayer()
        {
        }

        public Vector3 GetCentroid()
        {
            return MathUtil.Centroid(Positions);
        }

        public IEnumerator<AvoidanceNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(AvoidanceNode item)
        {
            Nodes.Add(item);
            Positions.Add(item.NavigableCenter);
        }

        public void Clear()
        {
            Nodes.Clear();
            Positions.Clear();
        }

        public bool Contains(AvoidanceNode item)
        {
            return Nodes.Contains(item);
        }

        public void CopyTo(AvoidanceNode[] array, int index)
        {
            foreach (var item in Nodes)
            {
                array.SetValue(item, index);
            }
        }

        public bool Remove(AvoidanceNode item)
        {
            return Nodes.Remove(item);
        }

        public int Count => Nodes.Count;

        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<AvoidanceNode> items)
        {
            foreach(var node in items)
            {
                Add(node);
            }
        }

    }
}
