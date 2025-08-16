using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using UserService.Application.DTOs;
using UserService.Domain.Repositories;

namespace UserService.Application;

public class UserProfileProfileServiceApplication : BaseServiceApplication, IUserProfileServiceApplication
{
    private readonly IUserProfileReadRepository _userProfileReadRepository;

    
    public UserProfileProfileServiceApplication(NotificationContext notificationContext, IUserProfileReadRepository userProfileReadRepository) : base(notificationContext)
    {
        _userProfileReadRepository = userProfileReadRepository;
    }

    public async Task<UserProfileResponseDto> GetByUserIdAsync(Guid id)
    {
        var user = await _userProfileReadRepository.GetByUserIdAsync(id);
        if (user == null)
        {
            _notificationContext.AddNotification("User", "User not found");
            return default;
        }

        return user;

    }
}