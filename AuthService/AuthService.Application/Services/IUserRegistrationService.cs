using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Request;
using DotnetBaseKit.Components.Application.Base;

namespace AuthService.Application.Services;

public interface IUserRegistrationService : IBaseServiceApplication
{
    Task RegisterUserAsync(RegisterUserRequestDto requestDto);
}