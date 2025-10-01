using CloudinaryDotNet;
using FoodOrdering.Application;
using FoodOrdering.Application.Auth;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Infrastructure;
using FoodOrdering.Infrastructure.Identity;
using FoodOrdering.Infrastructure.Repositories;
using FoodOrdering.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Food_Ordering.Extensions
{
    public static class ConfigDependencyInjection
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddTransient<ICloudinaryService, CLoudinaryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IMenuRepo, MenuRepo>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<ICartRepo, CartRepo>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
