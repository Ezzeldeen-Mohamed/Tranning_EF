using MeterManagement.Application.Exceptions;

namespace company_smart_charging_system.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";

                int statusCode = StatusCodes.Status500InternalServerError;
                string message = "Internal Server Error";

                switch (ex)
                {
                    case BusinessException businessException:
                        statusCode = businessException.StatusCode;
                        message = businessException.Message;
                        break;

                    case UnauthorizedAccessException:
                        statusCode = StatusCodes.Status401Unauthorized;
                        message = "Unauthorized";
                        break;

                    case KeyNotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        message = "Resource Not Found";
                        break;
                }

                context.Response.StatusCode = statusCode;

                var response = new
                {
                    statusCode,
                    message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
