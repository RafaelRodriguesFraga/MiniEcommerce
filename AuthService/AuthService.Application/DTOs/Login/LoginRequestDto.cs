using DotnetBaseKit.Components.Shared.Notifications;
using AuthService.Application.Validations;

namespace AuthService.Application.DTOs.Login;

public class LoginRequestDto : Notifiable<Notification>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public void Validate()
    {
        var validator = new LoginRequestDtoContract();
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