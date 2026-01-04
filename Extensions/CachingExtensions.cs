using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace saas_template.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "saas_template:";
        });

        // Optional: Add Redis connection multiplexer for advanced operations
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        return services;
    }
}

