using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace MesseauftrittDatenerfassung_UI.Converters
{
    public static class CustomImageConverter
    {
        public static BitmapImage ConvertByteArrayToBitmapImage(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    
        public static BitmapImage ConvertStringToBitmapImage(string rawImageData)
        {
            if (string.IsNullOrEmpty(rawImageData))
            {
              return null;
            }
            var bytes = Convert.FromBase64String(rawImageData);
            return ConvertByteArrayToBitmapImage(bytes);
        }
  }
}
