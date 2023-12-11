namespace MesseauftrittDatenerfassung.Dtos.PictureDtos
{
    public class AddPictureDto
    {
        public required string Name { get; set; }
        public Image? Image { get; set; }
    }
}