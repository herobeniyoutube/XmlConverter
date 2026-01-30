using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace XmlConverter.Web.Middleware
{
    public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    logger.LogError(ex, "unhandled exception");
                    throw;
                }

                logger.LogError(ex, "unhandled exception");

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(new
                {
                    error = "Unhandled error",
                    detail = ex.Message
                });

                await context.Response.WriteAsync(payload);
            }
        }
    }
}
