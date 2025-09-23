using Food_Ordering.Repositories;
using Food_Ordering.Repositories.UnitOfWork;
using Food_Ordering.Services;
using Food_Ordering.Services.Auth;
using Food_Ordering.Services.Email;
using Food_Ordering.Services.Storage;

namespace Food_Ordering.Extensions
{
    public static class DIExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IDishRepo, MenuCategoryRepo>();
            services.AddScoped<IMenuItemRepo , DishRepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            services.AddScoped<ICloudinaryService , CloudinaryService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IDishService, DishService>();
            return services;
        }
    }
}
