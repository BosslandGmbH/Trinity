// AxisStrategy.cs by Charles Petzold, September 2009
using System;
using System.Windows;
using System.Globalization;
using System.Xml;

namespace LineChartLib
{
    public abstract class AxisStrategy : DependencyObject
    {
        // Crucial property indicating what data property is on this axis
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName",
                typeof(string),
                typeof(AxisStrategy),
                new PropertyMetadata(OnAxisPropertyChanged));

        // Properties that are common to many axis strategies
        public static readonly DependencyProperty IsFlippedProperty =
            DependencyProperty.Register("IsFlipped",
                typeof(bool),
                typeof(AxisStrategy),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty IncludeZeroProperty =
            DependencyProperty.Register("IncludeZero",
                typeof(bool),
                typeof(AxisStrategy),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty IsSymmetricAroundZeroProperty =
            DependencyProperty.Register("IsSymmetricAroundZero",
                typeof(bool),
                typeof(AxisStrategy),
                new PropertyMetadata(OnAxisPropertyChanged));

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin",
                typeof(string),
                typeof(AxisStrategy),
                new PropertyMetadata(OnAxisPropertyChanged));

        // Read-only dependency property has collection of axis items
        static readonly DependencyPropertyKey AxisItemsKey =
            DependencyProperty.RegisterReadOnly("AxisItems",
                typeof(AxisItemCollection),
                typeof(AxisStrategy),
                new PropertyMetadata());

        public static readonly DependencyProperty AxisItemsProperty =
            AxisItemsKey.DependencyProperty;

        public AxisStrategy()
        {
            AxisItems = new AxisItemCollection();
        }

        protected internal LineChartGenerator Parent { set; get; }

        // Set by parent when created to distinguish between X and Y
        internal string PointProperty { set; get; }

        public string PropertyName
        {
            set { SetValue(PropertyNameProperty, value); }
            get { return (string)GetValue(PropertyNameProperty); }
        }

        public bool IsFlipped
        {
            set { SetValue(IsFlippedProperty, value); }
            get { return (bool)GetValue(IsFlippedProperty); }
        }

        public bool IncludeZero
        {
            set { SetValue(IncludeZeroProperty, value); }
            get { return (bool)GetValue(IncludeZeroProperty); }
        }

        public bool IsSymmetricAroundZero
        {
            set { SetValue(IsSymmetricAroundZeroProperty, value); }
            get { return (bool)GetValue(IsSymmetricAroundZeroProperty); }
        }

        public string Margin
        {
            set { SetValue(MarginProperty, value); }
            get { return (string)GetValue(MarginProperty); }
        }

        public AxisItemCollection AxisItems
        {
            protected set { SetValue(AxisItemsKey, value); }
            get { return (AxisItemCollection)GetValue(AxisItemsProperty); }
        }

        protected double Length
        {
            get
            {
                if (Parent == null)
                    return 0;

                return PointProperty == "X" ? Parent.Width : Parent.Height;
            }
        }

        protected static void OnAxisPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as AxisStrategy).OnAxisPropertyChanged(args);
        }

        void OnAxisPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            Recalculate();
        }

        public void Recalculate()
        {
            AxisItems.Clear();

            if (Parent == null                     ||
                Parent.ItemsSource == null         ||
                Parent.ItemsSource.Count == 0      ||
                String.IsNullOrEmpty(PropertyName) ||
                Length == 0)
            {
                return;
            }

            Type propertyType = GetItemProperty(Parent.ItemsSource[0], PropertyName).GetType();

            if (!typeof(IConvertible).IsAssignableFrom(propertyType))
                throw new ApplicationException("Items being graphed must implement IConvertible");

            double minValue = Double.MaxValue;
            double maxValue = Double.MinValue;

            for (int index = 0; index < Parent.ItemsSource.Count ; index++)
            {
                object item = Parent.ItemsSource[index];
                double value = GetItemPropertyValue(item, PropertyName);

                minValue = Math.Min(value, minValue);
                maxValue = Math.Max(value, maxValue);
            }

            if (!String.IsNullOrEmpty(Margin))
            {
                double increment = ConvertIncrementStringToDouble(Margin, propertyType);
                maxValue += increment;
                minValue -= increment;
            }

            if (IncludeZero)
            {
                if (minValue > 0)
                    minValue = 0;

                if (maxValue < 0)
                    maxValue = 0;
            }

            if (IsSymmetricAroundZero)
            {
                maxValue = Math.Max(Math.Abs(minValue), Math.Abs(maxValue));
                minValue = -maxValue;
            }

            // Abstract method implemented in derived classes
            CalculateAxisItems(propertyType, ref minValue, ref maxValue);

            for (int index = 0; index < Parent.ItemsSource.Count ; index++)
            {
                object item = Parent.ItemsSource[index];
                double value = GetItemPropertyValue(item, PropertyName);
                double offset = Length * (value - minValue) / (maxValue - minValue);

                if (minValue == maxValue)
                    offset = Length / 2;

                if (IsFlipped)
                    offset = Length - offset;

                Point pt = Parent.Points[index];

                if (PointProperty == "X")
                    pt.X = offset;
                else
                    pt.Y = offset;

                Parent.Points[index] = pt;
                Parent.ItemPoints[index].Point = pt;
            }
        }

        protected abstract void CalculateAxisItems(Type propertyType, 
                                    ref double minValue, ref double maxValue);

        protected object GetItemProperty(object item, string propertyName)
        {
            object obj;

            if (item is XmlNode)
            {
                XmlNode node = item as XmlNode;
                string str;

                if (propertyName[0] == '@')
                    str = node.Attributes[propertyName.Substring(1)].Value;

                else
                    str = node[propertyName].InnerText;

                DateTime dateTime;

                if (DateTime.TryParseExact(str, new string[] { "R", "s", "u", "o" },
                        DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dateTime))
                {
                    obj = dateTime;
                }
                else
                {
                    obj = str;
                }
            }
            else
            {
                try
                {
                    obj = item.GetType().GetProperty(propertyName).GetValue(item, null);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Unable to find Property on Object for your Axis definition, check PropertyName is defined on Axis and exists on the target object");
                }                
            }

            return obj;
        }

        protected double GetItemPropertyValue(object item, string propertyName)
        {
            object obj = GetItemProperty(item, propertyName);
            return ConvertToDouble(obj);
        }

        protected double ConvertToDouble(object obj)
        {
            double returnValue = 0;

            if (obj is DateTime)
            {
                returnValue = Convert.ToDouble(((DateTime)obj).Ticks);
            }
            else if (obj is TimeSpan)
            {
                returnValue = Convert.ToDouble(((TimeSpan)obj).Ticks);
            }
            else if (obj is IConvertible)
            {
                returnValue = (obj as IConvertible).ToDouble(CultureInfo.CurrentCulture);
            }

            return returnValue;
        }

        protected object ConvertFromDouble(double value, Type type)
        {
            object obj;

            if (type == typeof(DateTime))
            {
                obj = new DateTime(Convert.ToInt64(value));
            }
            else if (type == typeof(TimeSpan))
            {
                obj = new TimeSpan(Convert.ToInt64(value));
            }
            else 
            {
                obj = Convert.ChangeType(value, type);
            }

            return obj;
        }

        protected double ConvertIncrementStringToDouble(string strIncrement, Type propertyType)
        {
            object typedIncrement;

            if (propertyType == typeof(DateTime))
                typedIncrement = TimeSpan.Parse(strIncrement);
            else
                typedIncrement = Convert.ChangeType(strIncrement, propertyType);

            return ConvertToDouble(typedIncrement);
        }
    }
}
