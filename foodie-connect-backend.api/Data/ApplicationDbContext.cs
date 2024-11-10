using foodie_connect_backend.Shared.Classes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    public DbSet<Area> Areas { get; init; } = null!;
    public DbSet<Restaurant> Restaurants { get; init; } = null!;
    public DbSet<Dish> Dishes { get; init; } = null!;
    public DbSet<DishReview> DishReviews { get; init; } = null!;
    public DbSet<Promotion> Promotions { get; init; } = null!;
    public DbSet<Service> Services { get; init; } = null!;
    public DbSet<DishReview> Reviews { get; init; } = null!;
    public DbSet<DishCategory> DishCategories { get; init; } = null!;
    public DbSet<SocialLink> SocialLinks { get; init; } = null!;
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>()
            .Property(restaurant => restaurant.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Area>()
            .Property(area => area.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Dish>()
            .Property(dish => dish.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<DishReview>()
            .Property(dishReview => dishReview.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Promotion>()
            .Property(promotion => promotion.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Service>()
            .Property(service => service.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<SocialLink>()
            .Property(socialLink => socialLink.Id)
            .ValueGeneratedOnAdd();
    }
}