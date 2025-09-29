using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Infrastructure.Data
{
    public class FoodOrderingDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public FoodOrderingDbContext(DbContextOptions<FoodOrderingDbContext> options) : base(options)
        {
        }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Voucher> Voucher { get; set; }
        public DbSet<VoucherRedemptions> VoucherRedemptions { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<OrderMenus> OrderMenus { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<CartItems> CartItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.RelationshipConfigure();
            builder.Seed();
        }
        
    }
}
