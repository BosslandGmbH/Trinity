using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Trinity.Framework.Helpers;

namespace Trinity.UIComponents
{
    public class EnumVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string parameterString = parameter as string;
                if (parameterString == null)
                    return DependencyProperty.UnsetValue;

                if (Enum.IsDefined(value.GetType(), value) == false)
                    return DependencyProperty.UnsetValue;

                object parameterValue = Enum.Parse(value.GetType(), parameterString);

                if (!Reverse)
                    return parameterValue.Equals(value) ? Visibility.Visible : Visibility.Collapsed;

                return parameterValue.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                //Log.Error("Exception in EnumVisibilityConverter. Value={0} TargetType={1} Param={2} Exception={3} {4} {5}", value, targetType, parameter, ex.Message, ex.InnerException, ex.ToString());
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
        #endregion
    }
}
