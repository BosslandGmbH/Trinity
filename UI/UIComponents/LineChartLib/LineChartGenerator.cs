// LineChartGenerator.cs by Charles Petzold, September 2009
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Xml;
using System.ComponentModel;
using Logger = Trinity.Technicals.Logger;

namespace LineChartLib
{
    public class LineChartGenerator : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new LineChartGenerator();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                typeof(IList),
                typeof(LineChartGenerator),
                new PropertyMetadata(null, OnItemsSourceChanged));

        // Width and Height
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width",
                typeof(double),
                typeof(LineChartGenerator),
                new PropertyMetadata(100.0, OnWidthChanged));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height",
                typeof(double),
                typeof(LineChartGenerator),
                new PropertyMetadata(100.0, OnHeightChanged));

        // Horizontal and Vertical Axis Strategies
        static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register("HorizontalAxis",
                typeof(AxisStrategy),
                typeof(LineChartGenerator),
                new PropertyMetadata(OnHorizontalAxisChanged));

        static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis",
                typeof(AxisStrategy),
                typeof(LineChartGenerator),
                new PropertyMetadata(OnVerticalAxisChanged));

        // Two read-only dependency properties
        static readonly DependencyPropertyKey ItemPointsKey =
            DependencyProperty.RegisterReadOnly("ItemPoints",
                typeof(ItemPointCollection),
                typeof(LineChartGenerator),
                new PropertyMetadata());

        public static readonly DependencyProperty ItemPointsProperty =
            ItemPointsKey.DependencyProperty;

        static readonly DependencyPropertyKey PointsKey =
            DependencyProperty.RegisterReadOnly("Points",
                typeof(PointCollection),
                typeof(LineChartGenerator),
                new PropertyMetadata(new PointCollection()));

        public static readonly DependencyProperty PointsProperty =
            PointsKey.DependencyProperty;

        public LineChartGenerator()
        {
            Points = Points.Clone();
            ItemPoints = new ItemPointCollection();
        }

        public IList ItemsSource
        {
            set { SetValue(ItemsSourceProperty, value); }
            get { return (IList)GetValue(ItemsSourceProperty); }
        }

        public double Width
        {
            set { SetValue(WidthProperty, value); }
            get { return (double)GetValue(WidthProperty); }
        }

        public double Height
        {
            set { SetValue(HeightProperty, value); }
            get { return (double)GetValue(HeightProperty); }
        }

        public AxisStrategy HorizontalAxis
        {
            set { SetValue(HorizontalAxisProperty, value); }
            get { return (AxisStrategy)GetValue(HorizontalAxisProperty); }
        }

        public AxisStrategy VerticalAxis
        {
            set { SetValue(VerticalAxisProperty, value); }
            get { return (AxisStrategy)GetValue(VerticalAxisProperty); }
        }

        public ItemPointCollection ItemPoints
        {
            protected set { SetValue(ItemPointsKey, value); }
            get { return (ItemPointCollection)GetValue(ItemPointsProperty); }
        }

        public PointCollection Points
        {
            protected set { SetValue(PointsKey, value); }
            get { return (PointCollection)GetValue(PointsProperty); }
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as LineChartGenerator).OnItemsSourceChanged(args);
        }

        void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            // Transfer new collection of items in ItemsSource to ItemPoints
            ItemPoints.Clear();
            Points.Clear();

            if (args.NewValue != null)
            {
                foreach (object item in ItemsSource)
                {
                    ItemPoints.Add(new ItemPoint(item));
                    Points.Add(new Point());
                }
            }

            // Install (or remove) handlers for CollectionChanged event
            if (args.OldValue is INotifyCollectionChanged)
            {
                (args.OldValue as INotifyCollectionChanged).CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            if (args.NewValue is INotifyCollectionChanged)
            {
                (args.NewValue as INotifyCollectionChanged).CollectionChanged += OnItemsSourceCollectionChanged;
            }

            CalculateAxisScalers();
        }

        void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ItemPoints.Clear();
            Points.Clear();

            foreach (object item in ItemsSource)
            {
                ItemPoints.Add(new ItemPoint(item));
                Points.Add(new Point());
            }

            // Also check for INotifyPropertyChanged
            if (args.OldItems != null)
            {
                foreach (object item in args.OldItems)
                    if (item is INotifyPropertyChanged)
                        (item as INotifyPropertyChanged).PropertyChanged -= OnItemPropertyChanged;

            }
            if (args.NewItems != null)
            {
                foreach (object item in args.NewItems)
                    if (item is INotifyPropertyChanged)
                        (item as INotifyPropertyChanged).PropertyChanged += OnItemPropertyChanged;
            }

            CalculateAxisScalers();
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalculateAxisScalers();
        }

        static void OnWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as LineChartGenerator).CalculateHorizontalAxisScalers();
        }

        static void OnHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as LineChartGenerator).CalculateVerticalAxisScalers();
        }

        static void OnHorizontalAxisChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as LineChartGenerator).OnHorizontalAxisChanged(args);
        }

        void OnHorizontalAxisChanged(DependencyPropertyChangedEventArgs args)
        {
            if (HorizontalAxis != null)
            {
                HorizontalAxis.Parent = this;
                HorizontalAxis.PointProperty = "X";
                HorizontalAxis.Recalculate();
            }
        }

        static void OnVerticalAxisChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as LineChartGenerator).OnVerticalAxisChanged(args);
        }

        void OnVerticalAxisChanged(DependencyPropertyChangedEventArgs args)
        {
            if (VerticalAxis != null)
            {
                VerticalAxis.Parent = this;
                VerticalAxis.PointProperty = "Y";
                VerticalAxis.Recalculate();
            }
        }

        void CalculateAxisScalers()
        {
            CalculateHorizontalAxisScalers();
            CalculateVerticalAxisScalers();
        }

        void CalculateHorizontalAxisScalers()
        {
            if (HorizontalAxis != null)
                HorizontalAxis.Recalculate();
        }

        void CalculateVerticalAxisScalers()
        {
            if (VerticalAxis != null)
                VerticalAxis.Recalculate();
        }
    }
}
