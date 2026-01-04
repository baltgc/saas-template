using Microsoft.Extensions.Diagnostics.HealthChecks;
using saas_template.Data;
using saas_template.HealthChecks;

namespace saas_template.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        var healthChecksBuilder = services.AddHealthChecks();
        
        healthChecksBuilder
            .AddDbContextCheck<ApplicationDbContext>("database", tags: new[] { "ready" })
            .AddCheck<MemoryHealthCheck>("memory", tags: new[] { "ready" })
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

        return services;
    }
}

