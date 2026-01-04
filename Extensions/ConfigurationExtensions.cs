using saas_template.Configuration;

namespace saas_template.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        return services;
    }
}

