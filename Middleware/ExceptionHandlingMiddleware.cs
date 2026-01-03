using System.Net;
using System.Text.Json;
using saas_template.Common.Exceptions;
using saas_template.Common.Helpers;

namespace saas_template.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";

        if (exception is ApiException apiException)
        {
            code = (HttpStatusCode)apiException.StatusCode;
            message = apiException.Message;
        }

        var response = ApiResponse<object>.ErrorResponse(message, 
            exception is ApiException apiEx ? apiEx.Details : null);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}

