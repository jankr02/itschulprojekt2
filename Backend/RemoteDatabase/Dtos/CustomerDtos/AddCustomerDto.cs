namespace RemoteDatabase.Dtos.CustomerDtos
{
    public class AddCustomerDto
    {
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Street { get; set; } = string.Empty;
        public string? HouseNumber { get; set; } = string.Empty;
        public string? PostalCode { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
  }
}