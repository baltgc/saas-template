using Microsoft.EntityFrameworkCore;
using saas_template.Data;
using saas_template.Repositories;
using saas_template.Services;

namespace saas_template.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=saas_template.db"));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Services
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}

