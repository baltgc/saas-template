using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace saas_template.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitOptions = configuration.GetSection("RateLimiting").Get<RateLimitOptions>() 
            ?? new RateLimitOptions();

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User.Identity?.Name 
                        ?? context.Connection.RemoteIpAddress?.ToString() 
                        ?? "anonymous",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = rateLimitOptions.PermitLimit,
                        Window = TimeSpan.FromMinutes(rateLimitOptions.WindowMinutes)
                    }));

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync(
                    "Rate limit exceeded. Please try again later.", token);
            };
        });

        return services;
    }
}

public class RateLimitOptions
{
    public int PermitLimit { get; set; } = 100;
    public int WindowMinutes { get; set; } = 1;
}

