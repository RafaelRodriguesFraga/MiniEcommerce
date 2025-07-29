using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.User;
using DotnetBaseKit.Components.Application.Base;

namespace AuthService.Application.Services;

public interface IUserServiceApplication : IBaseServiceApplication
{
    Task RegisterAsync(UserRequestDto requestDto);
    Task<UserResponseDto> AuthenticateAsync(LoginRequestDto requestDto); 
}