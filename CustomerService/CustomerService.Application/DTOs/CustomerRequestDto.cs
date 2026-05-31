using DotnetBaseKit.Components.Shared.Notifications;
using CustomerService.Application.Validations;

namespace CustomerService.Application.DTOs;

public class CustomerRequestDto : Notifiable<Notification>
{
    public CustomerRequestDto(Guid authServiceId, string firstName, string lastName, string cpf, string email, string? avatarUrl = null)
    {
        AuthServiceId = authServiceId;
        FirstName = firstName;
        LastName = lastName;
        Cpf = cpf;
        Email = email;
        AvatarUrl = avatarUrl;
    }

    public Guid AuthServiceId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }


    public void Validate()
    {
        var validator = new CustomerRequestDtoContract();
        var result = validator.Validate(this);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                AddNotification(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}
