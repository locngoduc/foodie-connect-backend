using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<Head> Heads { get; set; }
        public DbSet<Urole> URoles { get; set; }
        public DbSet<UsersUrole> UsersURoles { get; set; }
        public DbSet<Hrole> HRoles { get; set; }
        public DbSet<HeadsHrole> HeadsHRoles { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DishesCategory> DishesCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersUrole>()
                .HasKey(ur => new { ur.Userid, ur.Uroleid });

            modelBuilder.Entity<HeadsHrole>()
                .HasKey(hr => new { hr.Headid, hr.Hroleid });

            modelBuilder.Entity<DishesCategory>()
                .HasKey(dc => new { dc.Dishid, dc.Categoryid });

            base.OnModelCreating(modelBuilder);
        }
    }
}