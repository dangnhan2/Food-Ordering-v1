using Food_Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Data
{
    public static class Configuration
    { 
        public static ModelBuilder ConfigureRelationship(this ModelBuilder model)
        {
            model.Entity<OrderItems>()
                .HasKey(it => new { it.OrderId, it.MenuItemsId });

            model.Entity<OrderItems>()
                .HasOne(it => it.Orders)
                .WithMany(it => it.Items)
                .HasForeignKey(it => it.OrderId);

            model.Entity<OrderItems>()
                .HasOne(it => it.MenuItems)
                .WithMany(it => it.Items)
                .HasForeignKey(it => it.MenuItemsId);

            model.Entity<MenuCategories>()
                .HasMany(mc => mc.MenuItems)
                .WithOne(mc => mc.MenuCategories)
                .HasForeignKey(mc => mc.MenuCategoriesId)
                .OnDelete(DeleteBehavior.Cascade);

            model.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            return model;
        }
    }
}
