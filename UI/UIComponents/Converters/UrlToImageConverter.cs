using System;
using Trinity.Framework;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Trinity.UI.UIComponents.Converters
{
    /// <summary>
    /// Downloads image from URL and returns it for display
    /// </summary>
    public class UrlToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Core.Logger.Verbose("Creating Bitmap from url: {0}", value.ToString());

            var url = value as string;
            if (url != null)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(url, UriKind.Absolute);
                image.EndInit();
                return image;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
