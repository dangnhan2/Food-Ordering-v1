using CloudinaryDotNet;
using FoodOrdering.Application;
using FoodOrdering.Application.Auth;
using FoodOrdering.Application.Email;
using FoodOrdering.Application.Payment;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Infrastructure;
using FoodOrdering.Infrastructure.Email;
using FoodOrdering.Infrastructure.Identity;
using FoodOrdering.Infrastructure.Payment;
using FoodOrdering.Infrastructure.Repositories;
using FoodOrdering.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Food_Ordering.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
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
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVoucherRepo, VoucherRepo>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IVoucherRedemptionRepo, VoucherRedemptionRepo>();
            services.AddScoped<IBackgroundJobScheduler, BackgroundJobScheduler>();

            services.AddTransient<ICloudinaryService, CLoudinaryService>();
            services.AddTransient<IPaymentGateway, PaymentGateway>();
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
