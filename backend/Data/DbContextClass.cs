using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace DOTNETWEBAPI_DEV.Data
{
    public class DbContextClass : DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Stock-Product relationship (from Stock side)
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithOne()
                .HasForeignKey<Stock>(s => s.ProductCode);

            // Configure ShoppingCart (no navigation property to Product)
            modelBuilder.Entity<ShoppingCart>()
                .Property(sc => sc.ProductCode)
                .IsRequired();
        }
    }
}
