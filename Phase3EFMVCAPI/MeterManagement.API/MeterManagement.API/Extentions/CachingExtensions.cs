namespace company_smart_charging_system.Extensions
{
    public static class CachingExtensions
    {
        public static IServiceCollection AddCachingServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            return services;
        }
    }
}
