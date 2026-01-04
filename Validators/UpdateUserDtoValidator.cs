using FluentValidation;
using saas_template.Models.DTOs;

namespace saas_template.Validators;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Name)
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));
    }
}

