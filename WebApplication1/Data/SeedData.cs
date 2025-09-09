using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Data
{
    public static class SeedData
    {
        public static ModelBuilder Seed(this ModelBuilder model) {
            model.Entity<IdentityRole>().HasData(
                new IdentityRole<string> { Id = "bc170fd1-2d32-49cc-a09b-692989540f7b", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<string> { Id = "39de72c3-a4b3-46b2-96ac-035c782ea250", Name = "Customer", NormalizedName = "CUSTOMER" }
            );
            return model;
        }
    }
}
