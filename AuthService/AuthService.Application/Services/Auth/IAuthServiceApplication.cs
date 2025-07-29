using DotnetBaseKit.Components.Application.Base;

namespace AuthService.Application.Services.Auth;

public interface IAuthServiceApplication : IBaseServiceApplication
{
    Task ResetPasswordAsync(string email, string newPassword);
}