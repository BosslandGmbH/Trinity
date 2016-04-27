using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Trinity.UIComponents
{
    public class AxisValueToViewportCameraPosition : IValueConverter
    {
        public AxisType Axis { get; set; }

        public enum AxisType
        {
            None = 0,
            X, Y, Z,
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Point3D))
                return Binding.DoNothing;

          
            var dValue= (double)System.Convert.ChangeType(value, typeof(double));

            return new Point3D();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof (Point3D))
                return Binding.DoNothing;

            if (Axis == AxisType.None)
                return Binding.DoNothing;

            if (Axis == AxisType.X)
                return ((Point3D) value).X;

            if (Axis == AxisType.Y)
                return ((Point3D)value).Y;

            if (Axis == AxisType.Z)
                return ((Point3D)value).Z;

            return Binding.DoNothing;
        }
    }

}



