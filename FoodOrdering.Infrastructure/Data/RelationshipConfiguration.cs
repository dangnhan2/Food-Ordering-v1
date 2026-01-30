using FoodOrdering.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Data
{
    public static class RelationshipConfiguration
    {
        public static ModelBuilder RelationshipConfigure(this ModelBuilder builder)
        {
            builder.Entity<VoucherRedemptions>()
                .HasKey(vr => new
                {
                    vr.UserID, vr.VoucherID
                });

            builder.Entity<OrderMenus>()
                .HasKey(om => new
                {
                    om.OrderId, om.MenuId
                });

            builder.Entity<CartItem>()
                .HasKey(ct => new {ct.MenuId, ct.CartId });

            builder.Entity<CartItem>()
                .HasOne(ct => ct.Menu)
                .WithMany(ct => ct.CartItems)
                .HasForeignKey(ct => ct.MenuId);

            builder.Entity<CartItem>()
                .HasOne(ct => ct.Cart)
                .WithMany(ct => ct.CartItems)
                .HasForeignKey(ct => ct.CartId);

            builder.Entity<VoucherRedemptions>()
                .HasOne(vr => vr.User)
                .WithMany(vr => vr.VoucherRedemptions)
                .HasForeignKey(vr => vr.UserID);

            builder.Entity<VoucherRedemptions>()
                .HasOne(vr => vr.Voucher)
                .WithMany(vr => vr.VoucherRedemptions)
                .HasForeignKey(vr => vr.VoucherID);

            builder.Entity<VoucherRedemptions>()
                .Property(x => x.VoucherRedemptionStatus)
                .HasConversion<string>();

            builder.Entity<Order>()
                .Property(x => x.Status)
                .HasConversion<string>();

            builder.Entity<Advertisement>()
                .Property(a => a.AdTargetType)
                .HasConversion<string>();

            builder.Entity<OrderMenus>()
                .HasOne(om => om.Menus)
                .WithMany(om => om.OrderMenus)
                .HasForeignKey(om => om.MenuId);

            builder.Entity<OrderMenus>()
                .HasOne(om => om.Orders)
                .WithMany(om => om.OrderMenus)
                .HasForeignKey(om => om.OrderId);

            builder.Entity<Category>()
                .HasMany(c => c.Menus)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoriesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Rating>()
                .HasIndex(r => new { r.OrderId, r.MenuId })
                .IsUnique();

            builder.Entity<Menu>(m =>
            {
                m.Property(m => m.Description)
                .HasColumnType("jsonb")
                .IsRequired(false);
            });

            return builder;
        }
    }
}
