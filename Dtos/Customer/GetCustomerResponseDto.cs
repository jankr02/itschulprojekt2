namespace MesseauftrittDatenerfassung.Dtos.Customer
{
    public class GetCustomerResponseDto
    {
        public int Id { get; set; } = 1;
        public string FirstName { get; set; } = "Test";
        public string LastName { get; set; } = "User";
        public string Street { get; set; } = "Teststraße";
        public string HouseNumber { get; set; } = "123";
        public int PostalCode { get; set; } = 12345;
        public string City { get; set; } = "Testhausen";
        public Image Image { get; set; } = new Image();
        public ProductGroups ProductGroup { get; set; } = ProductGroups.Testgruppe1;
        public Business Business { get; set; } = new Business();
    }
}
