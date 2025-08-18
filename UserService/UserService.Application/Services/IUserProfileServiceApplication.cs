using DotnetBaseKit.Components.Application.Base;
using UserService.Application.DTOs;

namespace UserService.Application;

public interface IUserProfileServiceApplication : IBaseServiceApplication
{
    Task<UserProfileResponseDto> GetByUserIdAsync(Guid id);
    Task<UserProfileResponseDto> CreateAsync(UserProfileRequestDto dto, Guid userId);
    Task<UserProfileResponseDto> UpdateAsync(Guid id, UserProfileRequestDto dto);
}