using Microsoft.EntityFrameworkCore;

namespace Contoso.Api.Models
{
    public class ContosoDbContext : DbContext
    {
        public ContosoDbContext(DbContextOptions<ContosoDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }
        
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();
        }
    }
}