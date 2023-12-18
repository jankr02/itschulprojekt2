namespace MesseauftrittDatenerfassung.Converters
{
    public static class CustomConverter
    {
        public static byte[]? FormFileToByteArray(FormFile? imageIn)
        {
            byte[]? dataArray = null;
            if (imageIn == null)
            {
              return dataArray;
            }

            using var fileStream = imageIn.OpenReadStream();
            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            dataArray = memoryStream.ToArray();

            return dataArray;
        }
    }
}