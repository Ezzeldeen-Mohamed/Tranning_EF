using MeterManagement.Application.Common;
using MeterManagement.Application.Exceptions;

namespace MeterManagement.API.Middlewares
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred");

                context.Response.ContentType =
                    "application/json";

                var response = new ErrorResponse
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError,

                    Message =
                        "Internal Server Error",

                    TraceId =
                        context.TraceIdentifier
                };

                switch (ex)
                {
                    case BusinessException businessException:

                        response.StatusCode =
                            businessException.StatusCode;

                        response.Message =
                            businessException.Message;

                        break;

                    case UnauthorizedAccessException:

                        response.StatusCode =
                            StatusCodes.Status401Unauthorized;

                        response.Message =
                            "Unauthorized";

                        break;

                    case KeyNotFoundException:

                        response.StatusCode =
                            StatusCodes.Status404NotFound;

                        response.Message =
                            "Resource Not Found";

                        break;
                }

                context.Response.StatusCode =
                    response.StatusCode;

                await context.Response.WriteAsJsonAsync(
                    response);
            }
        }
    }
}