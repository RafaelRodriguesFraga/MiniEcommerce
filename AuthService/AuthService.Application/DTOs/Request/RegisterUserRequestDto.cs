using AuthService.Application.Validations;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Application.DTOs.Request;

public class RegisterUserRequestDto : Notifiable<Notification>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public void Validate()
    {
        var validator = new RegisterUserRequestDtoContract();
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