using System;
using System.Collections.Generic;
using Trinity.Helpers;

namespace Trinity.Framework.Avoidance.Structures
{
    public class AvoidanceAreaStats : NotifyBase
    {
        private int _nodesTotal;
        private double _weightPctAvg;
        private double _weightPctTotal;
        private DateTime _lastUpdated = DateTime.MinValue;
        private double _highestWeight;

        public void Update(IList<AvoidanceNode> nodes)
        {
            ThrottleChangeNotifications = DateTime.UtcNow.Subtract(_lastUpdated).TotalMilliseconds < 250;

            NodesTotal = nodes.Count;

            var weightPctSum = 0f;
            var highestWeight = 0;

            foreach (var node in nodes)
            {
                if(node.Weight > highestWeight)
                    highestWeight = node.Weight;

                weightPctSum += node.WeightPct;
            }

            WeightPctTotal = weightPctSum;
            HighestWeight = highestWeight;            

            WeightPctAvg = NodesTotal > 0 ? WeightPctTotal / (double)NodesTotal : 0;
        }

        public int NodesTotal
        {
            get { return _nodesTotal; }
            set { SetField(ref _nodesTotal, value); }
        }

        public double WeightPctAvg
        {
            get { return _weightPctAvg; }
            set { SetField(ref _weightPctAvg, value); }
        }

        public double WeightPctTotal
        {
            get { return _weightPctTotal; }
            set { SetField(ref _weightPctTotal, value); }
        }

        public double HighestWeight
        {
            get { return _highestWeight; }
            set { SetField(ref _highestWeight, value); }
        }
    }
}

