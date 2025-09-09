using Food_Ordering.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; } 
        public DbSet<MenuItems> Menus { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<MenuCategories> MenuCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureRelationship();
            builder.Seed();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }

    
}
