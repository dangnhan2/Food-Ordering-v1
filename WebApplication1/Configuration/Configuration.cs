using FoodOrdering.Presentation.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Food_Ordering.Extensions
{
    public static class Configuration
    {
        public static IServiceCollection AddExtensions(this IServiceCollection services)
        {
            services.AddIdentity();
            services.AddJwtConfig();
            services.AddDI();
            services.AddSwaggerConfigure();
            services.AddSignalR();
            services.AddCorsConfiguration();
            return services;
        }
    }
}
