using RemoteDatabase.Enums;

namespace RemoteDatabase.Models
{
    public class ProductGroup
    {
        public int Id { get; set; }
        public ProductGroupName Name { get; set; }
        public List<Customer>? Customers { get; set; }
    }
}
