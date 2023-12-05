namespace MesseauftrittDatenerfassung.Mappings
{
    public class CustomToByteArrayMemberResolver : IMemberValueResolver<object?, object?, Image?, byte[]?>
    {
        public byte[]? Resolve(object? source, object? destination, Image? srcMember, byte[]? destMember, ResolutionContext context)
        {
            if (srcMember != null)
            {
                return new CustomImageConverter().ImageToByteArray(srcMember);
            }

            return null;
        }
    }
}
