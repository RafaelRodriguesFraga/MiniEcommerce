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
            return {};
        }

        return user.ToDto();
    }

    public async Task<UserProfileResponseDto> CreateAsync(UserProfileRequestDto dto, Guid userId, string userName, string userEmail)
    {
        // TODO: validation here

        var userProfile = dto.ToEntity(userId, userName, userEmail);

        await _userProfileWriteRepository.InsertAsync(userProfile);

        return userProfile.ToDto();
    }

    public Task<UserProfileResponseDto> UpdateAsync(Guid id, UserProfileRequestDto dto)
    {
        // TODO: validation here

        return default!;
    }
}