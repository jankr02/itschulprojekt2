using LocalDatabase.Dtos.ProductGroupDtos;
using MesseauftrittDatenerfassung_UI.Dtos.BusinessDtos;
using MesseauftrittDatenerfassung_UI.Dtos.PictureDtos;
using System.Collections.Generic;

namespace MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos
{
    public class GetCustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public GetPictureDto Picture { get; set; }
        public List<GetProductGroupDto> ProductGroups { get; set; }
        public GetBusinessDto Business { get; set; }
    }
}