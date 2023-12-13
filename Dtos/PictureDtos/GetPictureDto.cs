namespace MesseauftrittDatenerfassung.Dtos.PictureDtos
{
    public class GetPictureDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required byte[]? Image { get; set; }
    }
}