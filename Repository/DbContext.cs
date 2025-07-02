using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Example: Configure relationships if needed
        modelBuilder.Entity<Product>()
            .HasOne<Category>(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);

        base.OnModelCreating(modelBuilder);
    }
}
