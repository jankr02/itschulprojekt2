namespace MesseauftrittDatenerfassung.Dtos.PictureDtos
{
    public class UpdatePictureDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Image? Image { get; set; }
    }
}