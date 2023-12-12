namespace MesseauftrittDatenerfassung.Converters
{
    public class CustomConverter
    {
        public byte[]? FormFileToByteArray(FormFile? imageIn)
        {
            byte[]? dataArray = null;
            if (imageIn != null)
            {
                using var fileStream = imageIn.OpenReadStream();
                using var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                dataArray = memoryStream.ToArray();
            }

            return dataArray;
        }
    }
}