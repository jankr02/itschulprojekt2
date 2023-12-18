namespace LocalDatabase.Dtos.PictureDtos
{
    public class AddPictureDto
    {
        public required string Name { get; set; }
        public required FormFile? Image { get; set; }
    }
}