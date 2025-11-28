using FluentValidation;
using CustomerService.Application.DTOs;

namespace CustomerService.Application.Validations
{
    public class CustomerRequestDtoContract : AbstractValidator<CustomerRequestDto>
    {
        public CustomerRequestDtoContract()
        {
            RuleFor(x => x.AuthServiceId)
                .NotEmpty().WithMessage("AuthServiceId cannot be empty");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty");


        }
    }
}