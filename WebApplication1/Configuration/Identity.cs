using DotNetEnv;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Extensions
{
    public static class Identity
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            // Cấu hình Identity
            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddRoles<IdentityRole<Guid>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<FoodOrderingDbContext>()
                .AddDefaultTokenProviders();

            //services.AddHangfire(config =>
            //{
            //    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //          .UseSimpleAssemblyNameTypeSerializer()
            //          .UseRecommendedSerializerSettings()
            //          .UsePostgreSqlStorage(connection);
            //});
            return services;
        }
    }
}
