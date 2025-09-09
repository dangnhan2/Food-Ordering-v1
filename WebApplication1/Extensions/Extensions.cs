namespace Food_Ordering.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddExtensions(this IServiceCollection services)
        {
            services.AddConnectionString();
            services.AddJwtConfig();
            return services;
        }
    }
}
