using DotnetBaseKit.Components.Application.Base;
using UserService.Application.DTOs;

namespace UserService.Application;

public interface IUserProfileServiceApplication : IBaseServiceApplication
{
    Task<UserProfileResponseDto> GetByUserIdAsync(Guid id);
}