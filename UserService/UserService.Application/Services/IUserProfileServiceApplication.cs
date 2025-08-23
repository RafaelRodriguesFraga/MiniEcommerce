using DotnetBaseKit.Components.Application.Base;
using UserService.Application.DTOs;

namespace UserService.Application;

public interface IUserProfileServiceApplication : IBaseServiceApplication
{
    Task<UserProfileResponseDto> GetByUserIdAsync(Guid id);
    Task<UserProfileResponseDto> CreateAsync(UserProfileRequestDto dto, Guid userId, string userName, string userEmail);
    Task<UserProfileResponseDto> UpdateAsync(Guid id, UserProfileUpdateDto dto);
}