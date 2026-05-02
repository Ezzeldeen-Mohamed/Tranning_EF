namespace company_smart_charging_system.Middlewares
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            logger.LogInformation("Handling request: {Method} {Url}", context.Request.Method, context.Request.Path);
            await next(context);
            logger.LogInformation("Finished handling request with status code: {StatusCode}", context.Response.StatusCode);
        }
    }
}
