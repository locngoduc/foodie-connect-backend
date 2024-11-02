using foodie_connect_backend.Shared.Classes;
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
    public DbSet<SocialLink> SocialLinks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Area Configuration
        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            // Required fields
            entity.Property(e => e.FormattedAddress)
                .IsRequired();

            entity.Property(e => e.StreetAddress)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Country)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.AdministrativeAreaLevel1)
                .IsRequired();

            // Optional fields
            entity.Property(e => e.Route);
            entity.Property(e => e.Intersection);
            entity.Property(e => e.PoliticalEntity);
            entity.Property(e => e.AdministrativeAreaLevel2);
            entity.Property(e => e.AdministrativeAreaLevel3);
            entity.Property(e => e.Locality);
            entity.Property(e => e.Sublocality);
            entity.Property(e => e.Neighborhood);
            entity.Property(e => e.Premise);
            entity.Property(e => e.Subpremise);
            entity.Property(e => e.PostalCode);
            entity.Property(e => e.PlusCode);
            entity.Property(e => e.NaturalFeature);
            entity.Property(e => e.Airport);
            entity.Property(e => e.Park);
            entity.Property(e => e.PointOfInterest);

            // Timestamps
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);
    
            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

        });
        
        // Restaurant Configuration
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(64);
            entity.HasIndex(e => e.Name)
                .IsUnique();

            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.Location)
                .IsRequired()
                .HasColumnType("geography (point)");
            
            entity.HasIndex(e => e.Location)
                .HasMethod("GIST");
            
            entity.Property(e => e.Images)
                .HasColumnType("text[]")
                .HasDefaultValue(new List<string>());

            entity.Property(e => e.HeadId)
                .IsRequired();

            // Relationships
            entity.HasOne(r => r.Area)
                .WithMany(a => a.Restaurants)
                .HasForeignKey(r => r.AreaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(r => r.SocialLinks)
                .WithOne(s => s.Restaurant)
                .HasForeignKey(s => s.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Dish Configuration
        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(32);

            //Ensure dish names are unique per restaurant
            entity.HasIndex(e => new { e.Name, e.RestaurantId })
                .IsUnique();

            entity.Property(e => e.ImageId)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.Description)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            // Relationships
            entity.HasOne(d => d.Restaurant)
                .WithMany(r => r.Dishes)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DishesCategory Configuration
        modelBuilder.Entity<DishesCategory>(entity =>
        {
            entity.HasKey(dc => new { dc.DishId, dc.CategoryId });

            entity.HasOne(dc => dc.Dish)
                .WithMany(d => d.DishesCategories)
                .HasForeignKey(dc => dc.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dc => dc.Category)
                .WithMany(c => c.DishesCategories)
                .HasForeignKey(dc => dc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Promotion Configuration
        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(e => e.Target)
                .IsRequired()
                .HasMaxLength(64);

            entity.Property(e => e.BannerUrl)
                .HasMaxLength(256);

            // Relationships
            entity.HasOne(p => p.Restaurant)
                .WithMany(r => r.Promotions)
                .HasForeignKey(p => p.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Dish)
                .WithMany(d => d.Promotions)
                .HasForeignKey(p => p.DishId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Review Configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Type)
                .IsRequired();

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(e => e.Rating)
                .IsRequired();

            // Relationships
            entity.HasOne(r => r.Dish)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Restaurant)
                .WithMany(res => res.Reviews)
                .HasForeignKey(r => r.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Service Configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(64);
        });


        modelBuilder.Entity<SocialLink>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Url)
                .IsRequired();

            entity.Property(e => e.Platform)
                .IsRequired();
        });
    }
}