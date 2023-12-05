using MesseauftrittDatenerfassung.Enums;

namespace MesseauftrittDatenerfassung.Models
{
    public class ProductGroup
    {
        public int Id { get; set; } = 1;
        public ProductGroupName Name { get; set; }
        public List<Customer>? Customers { get; set; }
    }
}
