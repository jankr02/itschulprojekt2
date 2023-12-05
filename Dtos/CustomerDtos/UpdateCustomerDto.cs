namespace MesseauftrittDatenerfassung.Dtos.CustomerDtos
{
    public class UpdateCustomerDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public UpdatePictureDto? Picture { get; set; }
        public List<UpdateProductGroupDto>? ProductGroups { get; set; }
        public UpdateBusinessDto? Business { get; set; }
    }
}
