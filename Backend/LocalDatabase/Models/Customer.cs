namespace LocalDatabase.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required Picture Picture { get; set; }
        public int? PictureId { get; set; }
        public List<ProductGroup>? ProductGroups { get; set; }
        public Business? Business { get; set; }
        public int? BusinessId { get; set; }
    }
}
