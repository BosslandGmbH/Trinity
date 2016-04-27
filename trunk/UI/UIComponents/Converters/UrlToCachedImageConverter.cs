using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Trinity.Technicals;

namespace Trinity.UIComponents
{
    /// <summary>
    ///     Attempts to use local file in the trinity images folder.
    ///     Otherwise, downloads the image and stores it locally.
    /// </summary>
    public sealed class UriToCachedImageConverter : IValueConverter
    {
        public Dictionary<string, ImageSource> ImageCache = new Dictionary<string, ImageSource>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var url = value as string;
                if (string.IsNullOrEmpty(url))
                    return null;

                var webUri = new Uri(url, UriKind.Absolute);
                var filename = Path.GetFileName(webUri.AbsolutePath);

                var localFilePath = Path.Combine(FileManager.TrinityImagesPath, filename);

                ImageSource cachedImage;
                if (ImageCache.TryGetValue(localFilePath, out cachedImage))
                {
                    Logger.LogVerbose("Found memory cached image: {0}", filename);
                    return cachedImage;
                }

                if (File.Exists(localFilePath))
                {
                    Logger.LogVerbose("Found image on disk: {0}", filename);
                    var diskImage = BitmapFrame.Create(new Uri(localFilePath, UriKind.Absolute), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnLoad);
                    ImageCache.Add(localFilePath, diskImage);
                    return diskImage;
                }

                Logger.LogVerbose("Creating image from url: {0}", value.ToString());

                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = webUri;
                image.EndInit();

                ImageCache.Add(localFilePath, image);
                SaveImage(image, localFilePath);
                return image;

            }
            catch (Exception ex)
            {
                Logger.LogDebug("Exception Loading ListItem Image: {0} {1}", ex.Message, ex.InnerException);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public void SaveImage(BitmapImage image, string localFilePath)
        {
            image.DownloadCompleted += (sender, args) =>
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)sender));
                using (var filestream = new FileStream(localFilePath, FileMode.Create))
                {
                    encoder.Save(filestream);
                }
            };
        }
    }
}

