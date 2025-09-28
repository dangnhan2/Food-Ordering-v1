using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Data
{
    public static class SeedData
    {
        public static ModelBuilder Seed(this ModelBuilder builder)
        {
            builder.Entity<IdentityRole<Guid>>()
                .HasData(
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Customer", NormalizedName = "CUSTOMER" },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" }
                );
            return builder;
        }
    }
}
