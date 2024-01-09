namespace LocalDatabase.Dtos.CustomerDtos
{
    public class AddCompleteCustomerDto
    {
      public required string FirstName { get; set; }
      public required string LastName { get; set; }
      public required string Street { get; set; }
      public required string HouseNumber { get; set; }
      public required string PostalCode { get; set; }
      public required string City { get; set; }
      public AddPictureDto? Picture { get; set; }
      public List<GetProductGroupDto>? ProductGroups { get; set; }
      public AddBusinessDto? Business { get; set; }
    }
}
