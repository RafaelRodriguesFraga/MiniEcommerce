using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using UserService.Application.DTOs;
using UserService.Application.Extensions;
using UserService.Domain.Repositories;

namespace UserService.Application;

public class UserProfileServiceApplication : BaseServiceApplication, IUserProfileServiceApplication
{
    private readonly IUserProfileReadRepository _userProfileReadRepository;
    private readonly IUserProfileWriteRepository _userProfileWriteRepository;


    public UserProfileServiceApplication(NotificationContext notificationContext, IUserProfileReadRepository userProfileReadRepository, IUserProfileWriteRepository userProfileWriteRepository) : base(notificationContext)
    {
        _userProfileReadRepository = userProfileReadRepository;
        _userProfileWriteRepository = userProfileWriteRepository;
    }

    public async Task<UserProfileResponseDto> GetByUserIdAsync(Guid id)
    {
        var user = await _userProfileReadRepository.GetByUserIdAsync(id);
        if (user == null)
        {
            return null!;
        }

        return user.ToDto();
    }

    public async Task<UserProfileResponseDto> CreateAsync(UserProfileRequestDto dto, Guid userId, string userName, string userEmail)
    {
        dto.Validate();

        if (dto.Invalid)
        {
            _notificationContext.AddNotifications(dto.Notifications);
            return default!;
        }

        var userProfile = dto.ToEntity(userId, userName, userEmail);

        await _userProfileWriteRepository.InsertAsync(userProfile);

        return userProfile.ToDto();
    }

    public async Task<UserProfileResponseDto> UpdateAsync(Guid id, UserProfileUpdateDto dto)
    {
        var userProfile = await _userProfileReadRepository.GetByUserIdAsync(id);
        if (userProfile == null)
        {
            _notificationContext.AddNotification("User", "User Not found");
            return default!;
        }

        userProfile.Update(dto.Name, dto.Email, dto.AvatarUrl);

        await _userProfileWriteRepository.UpdateAsync(userProfile);

        return userProfile.ToDto();
    }
}