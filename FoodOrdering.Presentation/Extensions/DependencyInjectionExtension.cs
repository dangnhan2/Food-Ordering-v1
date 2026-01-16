using CloudinaryDotNet;
using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.Email;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Services.Payment;
using FoodOrdering.Application.Services.Services;
using FoodOrdering.Infrastructure;
using FoodOrdering.Infrastructure.Options;
using FoodOrdering.Infrastructure.Repositories;
using FoodOrdering.Infrastructure.Repository;
using FoodOrdering.Infrastructure.Services.BackgroundJob;
using FoodOrdering.Infrastructure.Services.Caching;
using FoodOrdering.Infrastructure.Services.Email;
using FoodOrdering.Infrastructure.Services.Payment;
using FoodOrdering.Infrastructure.Services.Token;
using FoodOrdering.Infrastructure.SignalR_Hub;
using Microsoft.Extensions.Options;
using Net.payOS;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Food_Ordering.Extensions
{
    public static class DependencyInjectionExtension
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
            services.AddScoped<IBackgroundJobs, BackgroundJobsService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAddressRepo, AddressRepo>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<INotificationRepo, NotificationRepo>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationSenderService, SignalRNotificationService>();
            services.AddScoped<IRatingRepo, RatingRepo>();
            services.AddScoped<IRatingService, RatingService>();

            services.AddScoped<ICachingService, CachingService>();
            services.AddTransient<ICloudinaryService, CLoudinaryService>();
            services.AddTransient<IPayOsService, PayOsService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<ISearchService, SearchService>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = $"{Env.GetString("REDIS")},abortConnect=false";
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSingleton<Cloudinary>(cd =>
            {
                var options = cd.GetRequiredService<IOptions<CloudinaryOptions>>().Value;

                var account = new Account(
                     options.CloudName,
                     options.ApiKey,
                     options.ApiSecret
                    );

                return new Cloudinary(account);
            });

            services.AddSingleton<PayOS>(p =>
            {
                var options = p.GetRequiredService<IOptions<PayOsOptions>>().Value;

                var account = new PayOS(
                     options.ClientId,
                     options.ApiKey,
                     options.ChecksumKey
                    );

                return account;
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
