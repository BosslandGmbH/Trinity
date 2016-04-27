// IncrementAxis.cs by Charles Petzold, September 2009
using System;
using System.Windows;

namespace LineChartLib
{
    public class IncrementAxis : AxisStrategy
    {
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment",
                typeof(string),
                typeof(AxisStrategy),
                new PropertyMetadata(OnAxisPropertyChanged));

        public string Increment
        {
            set { SetValue(IncrementProperty, value); }
            get { return (string)GetValue(IncrementProperty); }
        }

        protected override void CalculateAxisItems(Type propertyType, ref double minValue, ref double maxValue)
        {
            if (String.IsNullOrEmpty(Increment))
            {
                return;
            }

            double increment = ConvertIncrementStringToDouble(Increment, propertyType);

            // Find integer factors from the minimum and maximum values
            int minFactor = (int)Math.Floor(minValue / increment);
            int maxFactor = (int)Math.Ceiling(maxValue / increment);

            // Try to avoid division by zero, please
            if (minFactor == maxFactor)
            {
                minFactor--;
                maxFactor++;
            }

            // Gaps between the ticks
            double tickLength = Length / (maxFactor - minFactor);

            // Calculate AxisItem objects and add to collection
            for (int factor = minFactor; factor <= maxFactor; factor++)
            {
                AxisItem axisItem = new AxisItem()
                {
                    Item = ConvertFromDouble(factor * increment, propertyType),
                    Offset = (factor - minFactor) * tickLength
                };

                if (IsFlipped)
                    axisItem.Offset = Length - axisItem.Offset;

                AxisItems.Add(axisItem);
            }

            // Calculate new minimums and maximums
            minValue = increment * minFactor;
            maxValue = increment * maxFactor;
        }
    }
}
