using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Domain;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var category = modelBuilder.Entity<Category>();
        category.ToTable("categories");
        category.HasKey(x => x.Id);
        category.HasIndex(x => x.Slug).IsUnique();
        category.Property(x => x.Name).IsRequired().HasMaxLength(120);
        category.Property(x => x.Slug).IsRequired().HasMaxLength(120);
        category.Property(x => x.Description).HasMaxLength(1024);
        category.Property(x => x.CreatedAtUtc).IsRequired();
        category.Property(x => x.UpdatedAtUtc).IsRequired();

        var product = modelBuilder.Entity<Product>();
        product.ToTable("products");
        product.HasKey(x => x.Id);
        product.HasIndex(x => x.Sku).IsUnique();
        product.Property(x => x.Sku).IsRequired().HasMaxLength(64);
        product.Property(x => x.Name).IsRequired().HasMaxLength(256);
        product.Property(x => x.Description).HasMaxLength(2048);
        product.Property(x => x.Currency).IsRequired().HasMaxLength(3);
        product.Property(x => x.Price).HasColumnType("numeric(14,2)");
        product.Property(x => x.Tags)
            .HasConversion(
                tags => string.Join(',', tags ?? Array.Empty<string>()),
                value => string.IsNullOrWhiteSpace(value)
                    ? Array.Empty<string>()
                    : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .HasColumnType("text");
        product.Property(x => x.CreatedAtUtc).IsRequired();
        product.Property(x => x.UpdatedAtUtc).IsRequired();
        product.HasOne(x => x.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
