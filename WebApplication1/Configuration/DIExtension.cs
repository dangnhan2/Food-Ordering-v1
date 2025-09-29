using CloudinaryDotNet;
using FoodOrdering.Application;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Infrastructure;
using FoodOrdering.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Food_Ordering.Extensions
{
    public static class DIExtension
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
            //services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
            //services.AddScoped<IUserRepo, UserRepo>();
            //services.AddScoped<IDishRepo, MenuCategoryRepo>();
            //services.AddScoped<IMenuItemRepo , DishRepo>();
            //services.AddScoped<IOrderRepo, OrderRepo>();

            //services.AddScoped<IOrderService, OrderService>();
            //services.AddScoped<IJwtService, JwtService>();
            //services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            //services.AddScoped<ICloudinaryService , CloudinaryService>();
            //services.AddScoped<IAccountService, AccountService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddTransient<IEmailService, EmailService>();
            //services.AddScoped<IDishService, DishService>();
            //services.AddTransient<IPayOSService, PayOSService>();
            return services;
        }
    }
}
