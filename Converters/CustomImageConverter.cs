namespace MesseauftrittDatenerfassung.Converters
{
    public class CustomImageConverter
    {
        public byte[]? ImageToByteArray(Image? imageIn)
        {
            if(imageIn == null)
            {
                return Array.Empty<byte>();
            }

            using var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();
        }

        public Image? ByteArrayToImage(byte[]? byteArrayIn)
        {

            if (byteArrayIn == null)
            {
                return null;
            }
            using var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);

            return returnImage;
        }
    }
}
