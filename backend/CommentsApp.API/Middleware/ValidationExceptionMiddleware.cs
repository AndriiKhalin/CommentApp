using System.Net;
using System.Text.Json;
using FluentValidation;

namespace CommentsApp.API.Middleware;

public sealed class ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(e => e.ErrorMessage).ToArray();
            var payload = JsonSerializer.Serialize(new { errors });
            await context.Response.WriteAsync(payload);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // Never expose internal details to the client
            var payload = JsonSerializer.Serialize(new { error = "Internal server error" });
            await context.Response.WriteAsync(payload);
        }
    }
}