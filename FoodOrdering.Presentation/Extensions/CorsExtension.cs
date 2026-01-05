namespace FoodOrdering.Presentation.Configuration
{
    public static class CorsExtension
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {   
            services.AddCors(opt => {
                opt.AddPolicy("Happy Food",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
            return services;
        }
    }
}
