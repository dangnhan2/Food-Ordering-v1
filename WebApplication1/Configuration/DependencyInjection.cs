using CloudinaryDotNet;
using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.Caching;
using FoodOrdering.Application.Email;
using FoodOrdering.Application.Payment;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services;
using FoodOrdering.Application.Services.Auth;
using FoodOrdering.Application.Services.Auth.Token;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Infrastructure;
using FoodOrdering.Infrastructure.Cache;
using FoodOrdering.Infrastructure.Email;
using FoodOrdering.Infrastructure.Identity;
using FoodOrdering.Infrastructure.Payment;
using FoodOrdering.Infrastructure.Repositories;
using FoodOrdering.Infrastructure.Repository;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Food_Ordering.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            Env.Load();
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
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAddressRepo, AddressRepo>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<INotificationRepo, NotificationRepo>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICachingService, CachingService>();  

            services.AddTransient<ICloudinaryService, CLoudinaryService>();
            services.AddTransient<IPaymentGateway, PaymentGateway>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = $"{Env.GetString("REDIS")},abortConnect=false";
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSingleton<RedLockFactory>(sp =>
            {
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                return RedLockFactory.Create(new List<RedLockMultiplexer>
            {
                new RedLockMultiplexer(multiplexer)
                });
            });

            return services;
        }
    }
}
