namespace MesseauftrittDatenerfassung.Mappings
{
    public class CustomToImageMemberResolver : IMemberValueResolver<object?, object?, byte[]?, Image?>
    {
        public Image? Resolve(object? source, object? destination, byte[]? srcMember, Image? destMember, ResolutionContext context)
        {
            if (srcMember != null)
            {
                return new CustomImageConverter().ByteArrayToImage(srcMember);
            }

            return null;
        }
    }
}
