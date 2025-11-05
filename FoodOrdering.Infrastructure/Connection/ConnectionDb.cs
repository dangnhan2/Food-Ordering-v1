using DotNetEnv;
using FoodOrdering.Infrastructure.Data;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Configuration
{
    public static class ConnectionDb
    {
        public static void AddConnection(this IServiceCollection services)
        {
            try
            {
                Log.Information("Connecting DB");
                Env.Load();
                // Connect to Db
                services.AddDbContext<FoodOrderingDbContext>(otps =>
                {
                    otps.UseNpgsql(Env.GetString("CONNECTION_STRING"));
                });

                Log.Information("Connected");

                // Connect to HangfireDb
                services.AddHangfire(otps => otps
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Env.GetString("CONNECTION_STRING"))
                );

                // Connect to Redis
                services.AddStackExchangeRedisCache(opt =>
                {
                    opt.Configuration = Env.GetString("REDIS");
                });
            }catch (ConnectionAbortedException ex)
            {
                Log.Error($"{ex.InnerException.Message}");
            }
            
        }
    }
}
