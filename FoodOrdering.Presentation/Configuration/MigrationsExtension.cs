using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Runtime.CompilerServices;

namespace FoodOrdering.Presentation.Configuration
{
    public static class MigrationsExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            try
            {
                using IServiceScope scope = app.ApplicationServices.CreateScope();

                using FoodOrderingDbContext dbContext = scope.ServiceProvider.GetRequiredService<FoodOrderingDbContext>();

                dbContext.Database.Migrate();
            }catch(Exception ex)
            {
                Log.Error(ex.Message);
            }
            
        }
    }
}
