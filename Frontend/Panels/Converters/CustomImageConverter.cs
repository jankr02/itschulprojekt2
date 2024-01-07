using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MesseauftrittDatenerfassung_UI.Converters
{
    public static class CustomImageConverter
    {
        public static BitmapImage ConvertByteArrayToImage(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
