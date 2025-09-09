using DotNetEnv;
using Food_Ordering.Data;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Extensions
{
    public static class ConnectionExtension
    {
        public static IServiceCollection AddConnectionString(this IServiceCollection services)
        {
            Env.Load();
            var connection = $"Host={Env.GetString("HOST")};Database={Env.GetString("DATABASE")};Username={Env.GetString("Username")};Password={Env.GetString("PASSWORD")}";
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(otps =>
            {
                otps.UseNpgsql(connection);
            });
            return services;
        }
    }
}
