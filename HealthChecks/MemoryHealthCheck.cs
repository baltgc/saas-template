using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace saas_template.HealthChecks;

public class MemoryHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var data = new Dictionary<string, object>
        {
            { "AllocatedBytes", allocated },
            { "Gen0Collections", GC.CollectionCount(0) },
            { "Gen1Collections", GC.CollectionCount(1) },
            { "Gen2Collections", GC.CollectionCount(2) }
        };

        // Consider healthy if allocated memory is less than 1GB
        var status = allocated < 1024L * 1024L * 1024L
            ? HealthStatus.Healthy
            : HealthStatus.Degraded;

        return Task.FromResult(new HealthCheckResult(status, data: data));
    }
}

