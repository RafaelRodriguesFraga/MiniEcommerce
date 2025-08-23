using DotnetBaseKit.Components.Shared.Notifications;
using UserService.Application.Validations;

namespace UserService.Application.DTOs;

public class UserProfileRequestDto : Notifiable<Notification>
{
    public string AvatarUrl { get; set; } = string.Empty;


    public void Validate()
    {
        var validator = new UserProfileRequestDtoContract();
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
