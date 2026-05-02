using Serilog;

namespace company_smart_charging_system.Extensions
{
    public static class LoggingExtensions
    {
        public static IHostBuilder AddLoggingServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration));

            return hostBuilder;
        }
    }
}
