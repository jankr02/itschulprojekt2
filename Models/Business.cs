namespace MesseauftrittDatenerfassung.Models
{
    public class Business
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; } = "Testunternehmen";
        public string Street { get; set; } = "Teststraße";

        public string HouseNumber { get; set; } = "12a";
        public int PostalCode { get; set; } = 01234;
        public string City { get; set; } = "Musterhausen";
    }
}
