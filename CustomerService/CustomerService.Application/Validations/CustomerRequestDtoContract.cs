using FluentValidation;
using CustomerService.Application.DTOs;
using CustomerService.Domain.Validations;
namespace CustomerService.Application.Validations
{
    public class CustomerRequestDtoContract : AbstractValidator<CustomerRequestDto>
    {
        public CustomerRequestDtoContract()
        {
            RuleFor(x => x.AuthServiceId)
                .NotEmpty().WithMessage("AuthServiceId cannot be empty");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("Cpf is required")
                .Must(CpfValidator.IsValid).WithMessage("Invalid Cpf");
        }
    }
}