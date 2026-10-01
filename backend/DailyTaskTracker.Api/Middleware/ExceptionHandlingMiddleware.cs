using System.Net;
using System.Text.Json;

namespace DailyTaskTracker.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try { await next(context); }
        catch (KeyNotFoundException ex) { await Write(context, HttpStatusCode.NotFound, "NotFound", ex.Message); }
        catch (ArgumentException ex) { await Write(context, HttpStatusCode.BadRequest, "ValidationError", ex.Message); }
        catch (InvalidOperationException ex) { await Write(context, HttpStatusCode.BadRequest, "BusinessRuleViolation", ex.Message); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await Write(context, HttpStatusCode.InternalServerError, "InternalServerError", "An unexpected error occurred.");
        }
    }

    private static async Task Write(HttpContext context, HttpStatusCode status, string code, string message)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = new { code, message } }));
    }
}
