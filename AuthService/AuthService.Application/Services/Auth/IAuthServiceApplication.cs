using DotnetBaseKit.Components.Application.Base;

namespace AuthService.Application.Services.Auth;

public interface IAuthServiceApplication : IBaseServiceApplication
{
    Task ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(string email, string newPassword, string rawToken);
}