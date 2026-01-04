using FluentValidation.AspNetCore;
using saas_template.Validators;

namespace saas_template.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
            fv.AutomaticValidationEnabled = true;
            fv.ImplicitlyValidateChildProperties = true;
        });

        return services;
    }
}

