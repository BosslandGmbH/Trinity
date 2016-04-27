// AdaptableIncrementAxis.cs by Charles Petzold, September 2009
using System;
using System.Windows;
using System.Windows.Media;

namespace LineChartLib
{
    public class AdaptableIncrementAxis : AxisStrategy
    {
        public static readonly DependencyProperty IncrementsProperty =
            DependencyProperty.Register("Increments",
                typeof(DoubleCollection),
                typeof(AdaptableIncrementAxis),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty MaximumItemsProperty =
            DependencyProperty.Register("MaximumItems",
                typeof(int),
                typeof(AdaptableIncrementAxis),
                new PropertyMetadata(10, OnAxisPropertyChanged));

        public AdaptableIncrementAxis()
        {
            Increments = new DoubleCollection(new double[] { 1, 2, 5 });
        }

        public DoubleCollection Increments
        {
            set { SetValue(IncrementsProperty, value); }
            get { return (DoubleCollection)GetValue(IncrementsProperty); }
        }

        public int MaximumItems
        {
            set { SetValue(MaximumItemsProperty, value); }
            get { return (int)GetValue(MaximumItemsProperty); }
        }

        protected override void CalculateAxisItems(Type propertyType, ref double minValue, ref double maxValue)
        {
            if (propertyType == typeof(DateTime))
                throw new NotImplementedException("AdaptableIncrementAxis is not supported for DateTime." + 
                                                  "Use AdaptableDateTimeAxis instead.");

            if (Increments == null || Increments.Count == 0)
                throw new ArgumentException("Increments collection must contain have at least one item");

            if (MaximumItems < 2)
                throw new ArgumentException("MaximumItems must be at least 2");

            if (minValue == maxValue)
            {
                minValue -= 1;
                maxValue += 1;
            }

            // Copy the increments to an array and sort them
            double[] increments = new double[Increments.Count];
            Increments.CopyTo(increments, 0);
            Array.Sort(increments);

            // Initialize IncrementFinder
            IncrementFinder finder = new IncrementFinder(increments, minValue, maxValue);

            // Try to find the optimum increment
            while (true)
            {
                double increment = finder.Increment;

                if (finder.TickCount == MaximumItems)
                    break;

                else if (finder.TickCount > MaximumItems)
                {
                    finder.BumpUp();

                    if (finder.TickCount <= MaximumItems)
                        break;
                }

                else if (finder.TickCount < MaximumItems)
                {
                    finder.KickDown();

                    if (finder.TickCount > MaximumItems)
                    {
                        finder.BumpUp();
                        break;
                    }
                }
            }

            // Now we're ready to find the axis items
            double tickLength = Length / (finder.MaxFactor - finder.MinFactor);

            // Calculate AxisItem objects and add to collection
            for (int factor = finder.MinFactor; factor <= finder.MaxFactor; factor++)
            {
                AxisItem axisItem = new AxisItem()
                {
                    Item = ConvertFromDouble(factor * finder.Increment, propertyType),
                    Offset = (factor - finder.MinFactor) * tickLength
                };

                if (IsFlipped)
                    axisItem.Offset = Length - axisItem.Offset;

                AxisItems.Add(axisItem);
            }

            // New minValue and maxValue for return
            minValue = finder.Increment * finder.MinFactor;
            maxValue = finder.Increment * finder.MaxFactor;
        }

        class IncrementFinder
        {
            double[] increments;
            double minValue, maxValue;
            int incrementsIndex = 0;
            int exponent = 0;

            public IncrementFinder(double[] increments, double minValue, double maxValue)
            {
                this.increments = increments;
                this.minValue = minValue;
                this.maxValue = maxValue;
            }

            public double Increment 
            { 
                get { return increments[incrementsIndex] * Math.Pow(10, exponent); }
            }

            public int MinFactor
            {
                get { return (int)Math.Floor(minValue / Increment); }
            }

            public int MaxFactor
            {
                get { return (int)Math.Ceiling(maxValue / Increment); }
            }

            public int TickCount
            {
                get { return MaxFactor - MinFactor + 1; }
            }

            public void BumpUp()
            {
                if (++incrementsIndex == increments.Length)
                {
                    incrementsIndex = 0;
                    exponent++;
                }
            }

            public void KickDown()
            {
                if (--incrementsIndex == -1)
                {
                    incrementsIndex = increments.Length - 1;
                    exponent--;
                }
            }
        }
    }
}
