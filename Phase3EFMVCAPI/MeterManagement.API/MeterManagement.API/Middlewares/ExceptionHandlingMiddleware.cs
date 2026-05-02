using MeterManagement.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace company_smart_charging_system.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/json";

                var statusCode = 500;
                var message = "Something went wrong";

                if (ex is BusinessException be)
                {
                    statusCode = be.StatusCode;
                    message = be.Message;
                }
                else
                {
                    message = ex.Message;
                }
                var error = context.Features.Get<IExceptionHandlerFeature>();

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        message = error?.Error.Message ?? message,
                        statusCode
                    })
                );
            }
        }

    }
}
