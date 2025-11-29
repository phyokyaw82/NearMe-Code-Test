using FluentValidation;
using Newtonsoft.Json;
using System.Net;

namespace BMS.Api.Middleware;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context,
        RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException vex)
        {
            _logger.LogWarning(vex, "Validation failure.");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var result = new
            {
                status = 400,
                errors = vex.Errors.Select(e => e.ErrorMessage).ToArray(),
                traceId = context.TraceIdentifier
            };

            // Use Newtonsoft.Json for serialization
            var json = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var result = new
            {
                status = 500,
                title = "An unexpected error occurred.",
                traceId = context.TraceIdentifier
            };

            var json = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(json);
        }
    }
}
