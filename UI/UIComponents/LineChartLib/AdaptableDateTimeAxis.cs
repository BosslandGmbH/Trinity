// AdaptableDateTimeAxis.cs by Charles Petzold, September 2009
using System;
using System.Windows;
using System.Windows.Media;

namespace LineChartLib
{
    public class AdaptableDateTimeAxis : AxisStrategy
    {
        public static readonly DependencyProperty SecondIncrementsProperty =
            DependencyProperty.Register("SecondIncrements",
                typeof(Int32Collection),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty MinuteIncrementsProperty =
            DependencyProperty.Register("MinuteIncrements",
                typeof(Int32Collection),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty HourIncrementsProperty =
            DependencyProperty.Register("HourIncrements",
                typeof(Int32Collection),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty DayIncrementsProperty =
            DependencyProperty.Register("DayIncrements",
                typeof(Int32Collection),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty MonthIncrementsProperty =
            DependencyProperty.Register("MonthIncrements",
                typeof(Int32Collection),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty YearIncrementsProperty =
            DependencyProperty.Register("YearIncrements",
                typeof(Int32Collection),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty MaximumItemsProperty =
            DependencyProperty.Register("MaximumItems",
                typeof(int),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(10, OnAxisPropertyChanged));

        static readonly DependencyPropertyKey DateTimeIntervalKey =
            DependencyProperty.RegisterReadOnly("DateTimeInterval",
                typeof(DateTimeInterval),
                typeof(AdaptableDateTimeAxis),
                new PropertyMetadata(DateTimeInterval.Second));

        public static readonly DependencyProperty DateTimeIntervalProperty =
            DateTimeIntervalKey.DependencyProperty;

        public AdaptableDateTimeAxis()
        {
            SecondIncrements = new Int32Collection(new int[] { 1, 2, 5, 15, 30 });
            MinuteIncrements = new Int32Collection(new int[] { 1, 2, 5, 15, 30 });
            HourIncrements = new Int32Collection(new int[] { 1, 2, 4, 6, 12 });
            DayIncrements = new Int32Collection(new int[] { 1, 2, 5, 10 });
            MonthIncrements = new Int32Collection(new int[] { 1, 2, 4, 6 });
            YearIncrements = new Int32Collection(new int[] { 1, 2, 5 });
        }

        public Int32Collection SecondIncrements
        {
            set { SetValue(SecondIncrementsProperty, value); }
            get { return (Int32Collection)GetValue(SecondIncrementsProperty); }
        }

        public Int32Collection MinuteIncrements
        {
            set { SetValue(MinuteIncrementsProperty, value); }
            get { return (Int32Collection)GetValue(MinuteIncrementsProperty); }
        }

        public Int32Collection HourIncrements
        {
            set { SetValue(HourIncrementsProperty, value); }
            get { return (Int32Collection)GetValue(HourIncrementsProperty); }
        }

        public Int32Collection DayIncrements
        {
            set { SetValue(DayIncrementsProperty, value); }
            get { return (Int32Collection)GetValue(DayIncrementsProperty); }
        }

        public Int32Collection MonthIncrements
        {
            set { SetValue(MonthIncrementsProperty, value); }
            get { return (Int32Collection)GetValue(MonthIncrementsProperty); }
        }

        public Int32Collection YearIncrements
        {
            set { SetValue(YearIncrementsProperty, value); }
            get { return (Int32Collection)GetValue(YearIncrementsProperty); }
        }

        public int MaximumItems
        {
            set { SetValue(MaximumItemsProperty, value); }
            get { return (int)GetValue(MaximumItemsProperty); }
        }

        public DateTimeInterval DateTimeInterval
        {
            protected set { SetValue(DateTimeIntervalKey, value); }
            get { return (DateTimeInterval)GetValue(DateTimeIntervalProperty); }
        }

        protected override void CalculateAxisItems(Type propertyType, ref double minValue, ref double maxValue)
        {
            if (propertyType != typeof(DateTime))
                throw new NotImplementedException("AdaptableDateTimeAxis is only for a DateTime axis.");

            TestIncrementCollection(SecondIncrements, "Year");
            TestIncrementCollection(MinuteIncrements, "Minute");
            TestIncrementCollection(HourIncrements, "Hour");
            TestIncrementCollection(DayIncrements, "Day");
            TestIncrementCollection(MonthIncrements, "Month");
            TestIncrementCollection(YearIncrements, "Year");

            if (MaximumItems < 2)
                throw new ArgumentException("MaximumItems must be at least 2");

            if (minValue == maxValue)
            {
                minValue -= ConvertToDouble(TimeSpan.FromSeconds(1));
                maxValue += ConvertToDouble(TimeSpan.FromSeconds(1));
            }

            DateTime minDateTime = (DateTime)ConvertFromDouble(minValue, propertyType);
            DateTime maxDateTime = (DateTime)ConvertFromDouble(maxValue, propertyType);

            Int32Collection[] incrementsArray = { SecondIncrements, MinuteIncrements, HourIncrements, 
                                                  DayIncrements, MonthIncrements, YearIncrements };
            int incrementsIndex = 0;
            int incrementMultiplier = 1;
            DateTimeInterval interval = DateTimeInterval.Second;

            while (true)
            {
                Int32Collection increments = incrementsArray[(int)interval];
                int increment = incrementMultiplier * increments[incrementsIndex];

                DateTime minAxisDateTime = DateTimeFloor(minDateTime, interval, increment); 
                DateTime maxAxisDateTime = DateTimeCeiling(maxDateTime, interval, increment);
                DateTime dtTick = minAxisDateTime;
                int count = 1;

                while (true)
                {
                    dtTick = AddIncrement(dtTick, interval, increment);
                    count++;

                    if (dtTick >= maxAxisDateTime)
                        break;

                    if (count > MaximumItems)
                        break;
                }

                if (count <= MaximumItems)
                {
                    minValue = ConvertToDouble(minAxisDateTime);
                    maxValue = ConvertToDouble(maxAxisDateTime);
                    dtTick = minAxisDateTime;

                    for (int i = 0; i < count; i++)
                    {
                        AxisItem axisItem = new AxisItem()
                        {
                            Item = dtTick,
                            Offset = Length * (ConvertToDouble(dtTick) - minValue) / (maxValue - minValue)
                        };

                        AxisItems.Add(axisItem);
                        dtTick = AddIncrement(dtTick, interval, increment);
                    }
                    DateTimeInterval = interval;
                    break;
                }

                if (incrementsIndex < increments.Count - 1)
                {
                    incrementsIndex++;
                }
                else
                {
                    incrementsIndex = 0;

                    if (interval != DateTimeInterval.Year)
                        interval++;
                    else
                        incrementMultiplier *= 10;
                }
            }
        }

        void TestIncrementCollection(Int32Collection increments, string txt)
        {
            if (increments == null || increments.Count == 0)
                throw new ArgumentException(txt + "Increments collection must contain have at least one item");
        }

        DateTime AddIncrement(DateTime dt, DateTimeInterval interval, int increment)
        {
            DateTime dtNew = dt.AddSeconds(1);
            return DateTimeCeiling(dt.AddMilliseconds(500), interval, increment);
        }

        DateTime DateTimeFloor(DateTime dt, DateTimeInterval interval, int increment)
        {
            return DateTimeFloorCeiling(dt, interval, increment, false);
        }

        DateTime DateTimeCeiling(DateTime dt, DateTimeInterval interval, int increment)
        {
            return DateTimeFloorCeiling(dt, interval, increment, true);
        }

        DateTime DateTimeFloorCeiling(DateTime dt, DateTimeInterval interval, int increment, bool isCeiling)
        {
            DateTime dtNew = dt;

            switch (interval)
            {
                case DateTimeInterval.Second:
                    if (dt.Millisecond != 0 || dt.Second % increment != 0)
                    {
                        dtNew = dtNew.AddMilliseconds(-dt.Millisecond);
                        dtNew = dtNew.AddSeconds(-dt.Second % increment);

                        if (isCeiling)
                            dtNew = dtNew.AddSeconds(increment);
                    }
                    break;

                case DateTimeInterval.Minute:
                    if (dt.Millisecond != 0 || dt.Second != 0 || dt.Minute % increment != 0)
                    {
                        dtNew = dtNew.AddMilliseconds(-dt.Millisecond);
                        dtNew = dtNew.AddSeconds(-dt.Second);
                        dtNew = dtNew.AddMinutes(-dt.Minute % increment);

                        if (isCeiling)
                            dtNew = dtNew.AddMinutes(increment);
                    }
                    break;

                case DateTimeInterval.Hour:
                    if (dt.Millisecond != 0 || dt.Second != 0 || dt.Minute != 0 || dt.Hour % increment != 0)
                    {
                        dtNew = dtNew.AddMilliseconds(-dt.Millisecond);
                        dtNew = dtNew.AddSeconds(-dt.Second);
                        dtNew = dtNew.AddMinutes(-dt.Minute);
                        dtNew = dtNew.AddHours(-dt.Hour % increment);

                        if (isCeiling)
                            dtNew = dtNew.AddHours(increment);
                    }
                    break;

                case DateTimeInterval.Day:
                    if (dt.Millisecond != 0 || dt.Second != 0 || dt.Minute != 0 || dt.Hour != 0 || (dt.Day - 1) % increment != 0)
                    {
                        dtNew = dtNew.AddMilliseconds(-dt.Millisecond);
                        dtNew = dtNew.AddSeconds(-dt.Second);
                        dtNew = dtNew.AddMinutes(-dt.Minute);
                        dtNew = dtNew.AddHours(-dt.Hour);
                        dtNew = dtNew.AddDays(-((dt.Day - 1) % increment));

                        if (isCeiling)
                            dtNew = dtNew.AddDays(increment);
                    }
                    break;

                case DateTimeInterval.Month:
                    if (dt.Millisecond != 0 || dt.Second != 0 || dt.Minute != 0 || dt.Hour != 0 || dt.Day != 1 || (dt.Month - 1)% increment != 0)
                    {
                        dtNew = dtNew.AddMilliseconds(-dt.Millisecond);
                        dtNew = dtNew.AddSeconds(-dt.Second);
                        dtNew = dtNew.AddMinutes(-dt.Minute);
                        dtNew = dtNew.AddHours(-dt.Hour);
                        dtNew = dtNew.AddDays(-dt.Day + 1);
                        dtNew = dtNew.AddMonths(-((dt.Month - 1) % increment));

                        if (isCeiling)
                            dtNew = dtNew.AddMonths(increment);
                    }
                    break;

                case DateTimeInterval.Year:
                    if (dt.Millisecond != 0 || dt.Second != 0 || dt.Minute != 0 || dt.Hour != 0 || dt.Day != 1 || dt.Month != 1 || dt.Year % increment != 0)
                    {
                        dtNew = dtNew.AddMilliseconds(-dt.Millisecond);
                        dtNew = dtNew.AddSeconds(-dt.Second);
                        dtNew = dtNew.AddMinutes(-dt.Minute);
                        dtNew = dtNew.AddHours(-dt.Hour);
                        dtNew = dtNew.AddDays(-dt.Day + 1);
                        dtNew = dtNew.AddMonths(-dt.Month + 1);
                        dtNew = dtNew.AddYears(-(dt.Year % increment));

                        if (isCeiling)
                            dtNew = dtNew.AddYears(increment);
                    }
                    break;
            }
            return dtNew;
        }
    }
}
