using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Restaurant> Restaurants { get; set; } = null!;
    public DbSet<Dish> Dishes { get; set; } = null!;
    public DbSet<Promotion> Promotions { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<DishesCategory> DishesCategories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DishesCategory>()
            .HasKey(dc => new { dc.DishId, dc.CategoryId });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Images)
                .HasColumnType("text[]")
                .HasDefaultValue(new string[] { });

            entity.HasMany(r => r.SocialLinks)
                .WithOne(s => s.Restaurant)
                .HasForeignKey("RestaurantId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}