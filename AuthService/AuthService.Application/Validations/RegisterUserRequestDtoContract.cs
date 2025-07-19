using AuthService.Application.DTOs.Request;
using FluentValidation;

namespace AuthService.Application.Validations;

public class RegisterUserRequestDtoContract : AbstractValidator<RegisterUserRequestDto>
{
    public RegisterUserRequestDtoContract()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}