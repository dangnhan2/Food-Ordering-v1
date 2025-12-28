using DotNetEnv;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodOrdering.Presentation.Configuration
{
    public static class DataSeeder
    {
        public static async Task<WebApplication> SeedAsync(this WebApplication app)
        {
            Env.Load();
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { "Customer", "Admin" };
            foreach (var role in roles) {
               if (!await roleManager.RoleExistsAsync(role))               
                 await roleManager.CreateAsync(new IdentityRole<Guid>(role));               
            }

            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    IsAdmin = true,
                    ImageUrl = Env.GetString("DEFAULT_AVATAR")
                };

                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            return app;

        }
    }
}
