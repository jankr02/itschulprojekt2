namespace RemoteDatabase.Dtos.CustomerDtos
{
    public class GetCustomerDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public GetPictureDto? Picture { get; set; }
        public List<GetProductGroupDto>? ProductGroups { get; set; }
        public GetBusinessDto? Business { get; set; }
    }
}