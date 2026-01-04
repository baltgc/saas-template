using Microsoft.EntityFrameworkCore;
using saas_template.Data;
using saas_template.Repositories;
using saas_template.Services;

namespace saas_template.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddApplicationConfiguration(configuration);

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=saas_template.db"));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IUserService, UserService>();

        // Resilience policies
        services.AddResiliencePolicies();

        // Health checks
        services.AddHealthChecks();

        // Redis caching
        services.AddRedisCaching(configuration);
        services.AddScoped<Services.ICacheService, Services.RedisCacheService>();

        // FluentValidation
        services.AddFluentValidation();

        return services;
    }
}

