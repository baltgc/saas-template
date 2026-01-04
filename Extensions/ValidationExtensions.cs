using FluentValidation;
using FluentValidation.AspNetCore;
using saas_template.Validators;

namespace saas_template.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        return services;
    }
}

