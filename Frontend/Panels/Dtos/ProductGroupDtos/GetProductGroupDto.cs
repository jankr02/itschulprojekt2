using MesseauftrittDatenerfassung_UI.Enums;

namespace MesseauftrittDatenerfassung_UI.Dtos.ProductGroupDtos
{
    public class GetProductGroupDto
    {
        public int Id { get; set; }
        public ProductGroupName Name { get; set; }
    }
}