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

            builder.Entity<VoucherRedemptions>()
                .HasOne(vr => vr.User)
                .WithMany(vr => vr.VoucherRedemptions)
                .HasForeignKey(vr => vr.UserID);

            builder.Entity<VoucherRedemptions>()
                .HasOne(vr => vr.Voucher)
                .WithMany(vr => vr.VoucherRedemptions)
                .HasForeignKey(vr => vr.VoucherID);

            builder.Entity<OrderMenus>()
                .HasOne(om => om.Menus)
                .WithMany(om => om.OrderMenus)
                .HasForeignKey(om => om.MenuId);

            builder.Entity<OrderMenus>()
                .HasOne(om => om.Orders)
                .WithMany(om => om.OrderMenus)
                .HasForeignKey(om => om.OrderId);

            builder.Entity<Categories>()
                .HasMany(c => c.Menus)
                .WithOne(c => c.Categories)
                .HasForeignKey(c => c.CategoriesId)
                .OnDelete(DeleteBehavior.Restrict);


            return builder;
        }
    }
}
