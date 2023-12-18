namespace LocalDatabase.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductGroup>().HasData(
                new ProductGroup { Id = 1, Name = ProductGroupName.Produktgruppe1 },
                new ProductGroup { Id = 2, Name = ProductGroupName.Produktgruppe2 },
                new ProductGroup { Id = 3, Name = ProductGroupName.Produktgruppe3 },
                new ProductGroup { Id = 4, Name = ProductGroupName.Produktgruppe4 },
                new ProductGroup { Id = 5, Name = ProductGroupName.Produktgruppe5 }
            );
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Picture> Pictures => Set<Picture>();
        public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
        public DbSet<Business> Businesses => Set<Business>();
    }
}