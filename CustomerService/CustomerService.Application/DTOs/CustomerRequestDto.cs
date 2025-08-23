using DotnetBaseKit.Components.Shared.Notifications;
using CustomerService.Application.Validations;

namespace CustomerService.Application.DTOs;

public class CustomerRequestDto : Notifiable<Notification>
{
    public string AvatarUrl { get; set; } = string.Empty;


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
