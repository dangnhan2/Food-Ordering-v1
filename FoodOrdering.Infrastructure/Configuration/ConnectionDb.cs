using DotNetEnv;
using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            Env.Load();
            services.AddEntityFrameworkNpgsql().AddDbContext<FoodOrderingDbContext>(otps =>
            {
                otps.UseNpgsql(Env.GetString("CONNECTION_STRING"));
            });
        }
    }
}
