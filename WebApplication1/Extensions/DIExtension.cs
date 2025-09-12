using Food_Ordering.Repositories;
using Food_Ordering.Repositories.UnitOfWork;
using Food_Ordering.Services.Auth;
using Food_Ordering.Services.Email;

namespace Food_Ordering.Extensions
{
    public static class DIExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddTransient<IEmailService, EmailService>();
            return services;
        }
    }
}
