namespace RemoteDatabase.Dtos.CustomerDtos
{
    public class GetCustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public GetPictureDto? Picture { get; set; }
        public List<GetProductGroupDto>? ProductGroups { get; set; }
        public GetBusinessDto? Business { get; set; }
    }
}