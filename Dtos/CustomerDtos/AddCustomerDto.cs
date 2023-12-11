namespace MesseauftrittDatenerfassung.Dtos.CustomerDtos
{
    public class AddCustomerDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
    }
}