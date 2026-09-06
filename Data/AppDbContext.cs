using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Entities;

namespace ReactCommerce.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Name).HasMaxLength(60);
            e.Property(u => u.Email).HasMaxLength(255);
            e.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("customer");
        });

        // Category
        modelBuilder.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
            e.Property(c => c.Name).HasMaxLength(60);
            e.Property(c => c.Slug).HasMaxLength(60);
            e.Property(c => c.Description).HasMaxLength(255);
            e.Property(c => c.Icon).HasMaxLength(50);
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            e.Property(p => p.Name).HasMaxLength(200);
            e.Property(p => p.Slug).HasMaxLength(200);
            e.Property(p => p.Price).HasPrecision(10, 2);
            e.Property(p => p.Currency).HasMaxLength(3).HasDefaultValue("GHS");
            e.Property(p => p.Rating).HasPrecision(2, 1);
            e.HasQueryFilter(p => !p.IsDeleted);
            e.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
        });

        // ProductImage
        modelBuilder.Entity<ProductImage>(e =>
        {
            e.Property(i => i.Url).HasMaxLength(500);
        });

        // ProductTag
        modelBuilder.Entity<ProductTag>(e =>
        {
            e.Property(t => t.Tag).HasMaxLength(50);
            e.HasIndex(t => new { t.ProductId, t.Tag }).IsUnique();
        });

        // Address
        modelBuilder.Entity<Address>(e =>
        {
            e.Property(a => a.FullName).HasMaxLength(80);
            e.Property(a => a.PhoneNumber).HasMaxLength(15);
            e.Property(a => a.AddressLine1).HasMaxLength(120);
            e.Property(a => a.AddressLine2).HasMaxLength(120);
            e.Property(a => a.City).HasMaxLength(60);
            e.Property(a => a.Region).HasMaxLength(30);
            e.Property(a => a.District).HasMaxLength(60);
            e.Property(a => a.Landmark).HasMaxLength(120);
        });

        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.Property(o => o.Subtotal).HasPrecision(10, 2);
            e.Property(o => o.ShippingFee).HasPrecision(10, 2);
            e.Property(o => o.Total).HasPrecision(10, 2);
            e.Property(o => o.Currency).HasMaxLength(3).HasDefaultValue("GHS");
            e.Property(o => o.Status).HasMaxLength(20).HasDefaultValue("pending");
            e.HasOne(o => o.ShippingAddress).WithMany().HasForeignKey(o => o.ShippingAddressId);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.Property(i => i.ProductName).HasMaxLength(200);
            e.Property(i => i.UnitPrice).HasPrecision(10, 2);
            e.Property(i => i.ImageUrl).HasMaxLength(500);
        });

        // WishlistItem
        modelBuilder.Entity<WishlistItem>(e =>
        {
            e.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
        });
    }
}
