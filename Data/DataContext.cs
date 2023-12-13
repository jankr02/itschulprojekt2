namespace MesseauftrittDatenerfassung.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductGroup>().HasData(
                new ProductGroup { Id = 1, Name = ProductGroupName.Produktgruppe_1 },
                new ProductGroup { Id = 2, Name = ProductGroupName.Produktgruppe_2 },
                new ProductGroup { Id = 3, Name = ProductGroupName.Produktgruppe_3 },
                new ProductGroup { Id = 4, Name = ProductGroupName.Produktgruppe_4 },
                new ProductGroup { Id = 5, Name = ProductGroupName.Produktgruppe_5 }
            );
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Picture> Pictures => Set<Picture>();
        public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
        public DbSet<Business> Businesses => Set<Business>();
    }
}